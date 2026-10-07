// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary>
///     A validated, read-only structural index ("tape") over a JSON document. Navigate it through <see cref="Root"/>; nothing allocates except the strings you ask for.
/// </summary>
/// <remarks>
///     <para> The index and any copy of the input are pooled: always <see cref="Dispose"/> the tape. Every <see cref="JsonItem"/> from it becomes unusable then (<see cref="ObjectDisposedException"/>). </para>
///     <para> Safe for concurrent reads; not safe to read while another thread disposes it. </para>
/// </remarks>
public sealed class JsonTape : IDisposable
{
    private const int STACK_DEPTH = 128; // nesting levels tracked on the stack; deeper documents rent the depth stack


    private Entry[]? __entries;
    private int      __count;
    private string?  __string; // Parse(string): the caller's string, not copied
    private char[]?  __chars;  // Parse(ReadOnlySpan<char>): a pooled copy
    private byte[]?  __bytes;  // UTF-8 input: a pooled copy, or the pooled stream buffer
    private int      __length;
    private bool     __utf8;
    private int      __maxDepthSeen;


    /// <summary> Whether the input was UTF-8 (positions are byte offsets) rather than UTF-16 (char offsets). </summary>
    public bool IsUtf8 => __utf8;

    /// <summary> The input's length, in <see cref="char"/>s or <see cref="byte"/>s. </summary>
    public int Length => __length;

    /// <summary> The deepest nesting in the document. </summary>
    public int MaxDepth => __maxDepthSeen;

    /// <summary> The document's root value. </summary>
    public JsonItem Root
    {
        get
        {
            _ = Entries;
            return new JsonItem(this, 0);
        }
    }


    internal int     Count   => __count;
    internal Entry[] Entries => __entries ?? throw new ObjectDisposedException(nameof(JsonTape));

    internal ReadOnlySpan<char> InputUtf16 => __string is not null
                                                  ? __string.AsSpan()
                                                  : __chars.AsSpan(0, __length);

    internal ReadOnlySpan<byte> InputUtf8 => __bytes.AsSpan(0, __length);


    private JsonTape() { }


    // ─── Parse ───────────────────────────────────────────────────────────────

    /// <exception cref="JsonReadException"> The JSON is malformed. </exception>
    public static JsonTape Parse( string json, JsonReaderOptions? options = null )
    {
        ArgumentNullException.ThrowIfNull(json);

        JsonTape tape = new()
                        {
                            __string = json,
                            __length = json.Length
                        };

        return tape.BuildOrThrow(json.AsSpan(), options ?? JsonReaderOptions.Default);
    }

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static JsonTape Parse( ReadOnlySpan<char> json, JsonReaderOptions? options = null )
    {
        char[] copy = ArrayPool<char>.Shared.Rent(json.Length);
        json.CopyTo(copy);

        JsonTape tape = new()
                        {
                            __chars  = copy,
                            __length = json.Length
                        };

        return tape.BuildOrThrow<char>(copy.AsSpan(0, json.Length), options ?? JsonReaderOptions.Default);
    }

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static JsonTape Parse( ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null )
    {
        byte[] copy = ArrayPool<byte>.Shared.Rent(utf8Json.Length);
        utf8Json.CopyTo(copy);
        return FromPooledUtf8(copy, utf8Json.Length, options ?? JsonReaderOptions.Default);
    }

    /// <summary> Reads <paramref name="utf8Json"/> to the end into a pooled buffer (at most <see cref="JsonReaderOptions.MaxDocumentBytes"/>), then parses it. </summary>
    /// <exception cref="JsonReadException"> The JSON is malformed or too large. </exception>
    public static JsonTape Parse( Stream utf8Json, JsonReaderOptions? options = null )
    {
        JsonReaderOptions settings = options ?? JsonReaderOptions.Default;
        byte[]            buffer   = JsonStreams.ReadAll(utf8Json, settings.MaxDocumentBytes, out int length);
        return FromPooledUtf8(buffer, length, settings);
    }

    /// <inheritdoc cref="Parse(Stream, JsonReaderOptions?)"/>
    public static async ValueTask<JsonTape> ParseAsync( Stream utf8Json, JsonReaderOptions? options = null, CancellationToken token = default )
    {
        JsonReaderOptions settings = options ?? JsonReaderOptions.Default;
        ( byte[] buffer, int length ) = await JsonStreams.ReadAllAsync(utf8Json, settings.MaxDocumentBytes, token).ConfigureAwait(false);
        return FromPooledUtf8(buffer, length, settings);
    }


    /// <summary> Exception-free: <see langword="false"/> with <paramref name="error"/> for <see langword="null"/> or malformed input. </summary>
    public static bool TryParse( [NotNullWhen(true)] string? json, [NotNullWhen(true)] out JsonTape? tape, out JsonError error, JsonReaderOptions? options = null )
    {
        if ( json is null )
        {
            tape  = null;
            error = new JsonError(JsonErrorKind.UnexpectedEnd, 0, 1, 1);
            return false;
        }

        JsonTape result = new()
                          {
                              __string = json,
                              __length = json.Length
                          };

        return Finish(result, result.TryBuild(json.AsSpan(), options ?? JsonReaderOptions.Default, out error), out tape);
    }

    /// <inheritdoc cref="TryParse(string, out JsonTape, out JsonError, JsonReaderOptions?)"/>
    public static bool TryParse( ReadOnlySpan<byte> utf8Json, [NotNullWhen(true)] out JsonTape? tape, out JsonError error, JsonReaderOptions? options = null )
    {
        byte[] copy = ArrayPool<byte>.Shared.Rent(utf8Json.Length);
        utf8Json.CopyTo(copy);

        JsonTape result = new()
                          {
                              __bytes  = copy,
                              __length = utf8Json.Length,
                              __utf8   = true
                          };

        return Finish(result, result.TryBuild<byte>(copy.AsSpan(0, utf8Json.Length), options ?? JsonReaderOptions.Default, out error), out tape);
    }


    /// <summary> Returns the pooled index and input copy. Safe to call more than once. </summary>
    public void Dispose()
    {
        Entry[]? entries = __entries;
        if ( entries is null ) { return; }

        __entries = null;
        __count   = 0;
        __string  = null;
        ArrayPool<Entry>.Shared.Return(entries);

        if ( __chars is not null )
        {
            ArrayPool<char>.Shared.Return(__chars);
            __chars = null;
        }

        if ( __bytes is not null )
        {
            ArrayPool<byte>.Shared.Return(__bytes);
            __bytes = null;
        }
    }


    // ─── Build ───────────────────────────────────────────────────────────────

    private static JsonTape FromPooledUtf8( byte[] buffer, int length, in JsonReaderOptions options )
    {
        JsonTape tape = new()
                        {
                            __bytes  = buffer,
                            __length = length,
                            __utf8   = true
                        };

        return tape.BuildOrThrow<byte>(buffer.AsSpan(0, length), options);
    }

    private JsonTape BuildOrThrow<TChar>( ReadOnlySpan<TChar> input, in JsonReaderOptions options )
        where TChar : unmanaged, IBinaryInteger<TChar>
    {
        if ( TryBuild(input, options, out JsonError error) ) { return this; }

        Dispose();
        throw new JsonReadException(error);
    }

    private bool TryBuild<TChar>( ReadOnlySpan<TChar> input, in JsonReaderOptions options, out JsonError error )
        where TChar : unmanaged, IBinaryInteger<TChar>
    {
        __entries = ArrayPool<Entry>.Shared.Rent(Math.Clamp(input.Length / 8, 16, 1 << 16));
        __count   = 0;
        return Parser<TChar>.TryParse(this, input, options, out error);
    }

    private static bool Finish( JsonTape result, bool built, [NotNullWhen(true)] out JsonTape? tape )
    {
        if ( built )
        {
            tape = result;
            return true;
        }

        result.Dispose();
        tape = null;
        return false;
    }


    private int Add( JsonTokenKind kind, int start, int length, bool escaped )
    {
        Entry[] entries = __entries!;
        int     index   = __count;
        if ( index == entries.Length ) { entries = GrowEntries(); }

        entries[index] = new Entry(kind, escaped, start, length, index + 1);
        __count        = index + 1;
        return index;
    }

    [MethodImpl(MethodImplOptions.NoInlining)] private Entry[] GrowEntries()
    {
        Entry[] old    = __entries!;
        Entry[] larger = ArrayPool<Entry>.Shared.Rent(old.Length * 2); // entries never outnumber input chars, so this can't overflow
        old.AsSpan(0, __count).CopyTo(larger);
        ArrayPool<Entry>.Shared.Return(old);
        return __entries = larger;
    }


    // ─── Access (used by JsonItem and JNode) ─────────────────────────────────

    internal JsonError CreateError( JsonErrorKind kind, int position ) => __utf8
                                                                              ? JsonLexer<byte>.CreateError(kind, position, InputUtf8)
                                                                              : JsonLexer<char>.CreateError(kind, position, InputUtf16);

    internal string GetString( in Entry entry )
    {
        if ( !entry.Escaped )
        {
            return __utf8
                       ? Encoding.UTF8.GetString(InputUtf8.Slice(entry.Start, entry.Length))
                       : new string(InputUtf16.Slice(entry.Start, entry.Length));
        }

        ValueStringBuilder builder = new(stackalloc char[256]);
        Unescape(in entry, ref builder);
        return builder.ToString();
    }

    internal bool TryCopyString( in Entry entry, Span<char> destination, out int charsWritten )
    {
        if ( !entry.Escaped )
        {
            if ( __utf8 )
            {
                bool done = Utf8.ToUtf16(InputUtf8.Slice(entry.Start, entry.Length), destination, out _, out charsWritten) == OperationStatus.Done;
                if ( !done ) { charsWritten = 0; }

                return done;
            }

            ReadOnlySpan<char> text   = InputUtf16.Slice(entry.Start, entry.Length);
            bool               copied = text.TryCopyTo(destination);

            charsWritten = copied
                               ? text.Length
                               : 0;

            return copied;
        }

        ValueStringBuilder builder = new(stackalloc char[256]);
        Unescape(in entry, ref builder);
        bool fits = builder.Values.TryCopyTo(destination);

        charsWritten = fits
                           ? builder.Length
                           : 0;

        builder.Dispose();
        return fits;
    }

    /// <summary>
    ///     Compares a string or property name with a value, without allocating. An unescaped UTF-8 entry is compared with <paramref name="utf8"/>;
    ///     every other entry is compared with <paramref name="utf16"/>, unescaping into a stack buffer first if it has to.
    /// </summary>
    internal bool StringEquals( in Entry entry, ReadOnlySpan<char> utf16, ReadOnlySpan<byte> utf8 )
    {
        if ( !entry.Escaped )
        {
            return __utf8
                       ? InputUtf8.Slice(entry.Start, entry.Length).SequenceEqual(utf8)
                       : InputUtf16.Slice(entry.Start, entry.Length).SequenceEqual(utf16);
        }

        if ( utf16.Length > entry.Length ) { return false; } // unescaping (and UTF-8 → UTF-16) only ever shrinks the text

        ValueStringBuilder builder = new(stackalloc char[256]);
        Unescape(in entry, ref builder);
        bool equal = builder.Values.SequenceEqual(utf16);
        builder.Dispose();
        return equal;
    }

    internal string GetRawText( in Entry entry )
    {
        bool quoted = entry.Kind is JsonTokenKind.String or JsonTokenKind.PropertyName;

        int start = quoted
                        ? entry.Start - 1
                        : entry.Start;

        int length = quoted
                         ? entry.Length + 2
                         : entry.Length;

        return __utf8
                   ? Encoding.UTF8.GetString(InputUtf8.Slice(start, length))
                   : new string(InputUtf16.Slice(start, length));
    }

    private void Unescape( in Entry entry, ref ValueStringBuilder builder )
    {
        if ( __utf8 ) { JsonLexer<byte>.Unescape(InputUtf8.Slice(entry.Start, entry.Length), ref builder); }
        else { JsonLexer<char>.Unescape(InputUtf16.Slice(entry.Start, entry.Length), ref builder); }
    }


    /// <summary> Whether <paramref name="text"/> is exactly one JSON number (RFC 8259 grammar). </summary>
    internal static bool IsValidNumber( ReadOnlySpan<char> text ) => JsonLexer<char>.IsNumber(text);


    // ─── Entry ───────────────────────────────────────────────────────────────



    /// <summary> One token of the tape. Containers are completed (<see cref="Length"/>, <see cref="Next"/>) when their closing bracket is read. </summary>
    [StructLayout(LayoutKind.Auto)]
    internal struct Entry( JsonTokenKind kind, bool escaped, int start, int length, int next )
    {
        public readonly JsonTokenKind Kind    = kind;
        public readonly bool          Escaped = escaped; // strings and names that contain a backslash
        public readonly int           Start   = start;   // strings and names: just after the opening quote; others: the first char/byte
        public          int           Length  = length;  // strings and names: the content (no quotes); containers: through the closing bracket
        public          int           Next    = next;    // the entry after this value's subtree (the next sibling, or the parent's successor)
        public          int           Count;             // containers: members or elements
    }



    // ─── Parser ──────────────────────────────────────────────────────────────



    /// <summary> Validates and indexes JSON in one forward pass over <see cref="char"/> or <see cref="byte"/> input, using <see cref="JsonLexer{TChar}"/>. Iterative: nesting costs an <see cref="int"/>, not a stack frame. </summary>
    private static class Parser<TChar>
        where TChar : unmanaged, IBinaryInteger<TChar>
    {
        public static bool TryParse( JsonTape tape, ReadOnlySpan<TChar> input, in JsonReaderOptions options, out JsonError error )
        {
            if ( typeof(TChar) == typeof(byte) && !Utf8.IsValid(MemoryMarshal.Cast<TChar, byte>(input)) )
            {
                error = tape.CreateError(JsonErrorKind.InvalidUtf8, JsonText.IndexOfInvalidUtf8(MemoryMarshal.Cast<TChar, byte>(input)));
                return false;
            }

            int    maxDepth = options.MaxDepth;
            int[]? rented   = null;

            Span<int> stack = maxDepth <= STACK_DEPTH
                                  ? stackalloc int[STACK_DEPTH]
                                  : rented = ArrayPool<int>.Shared.Rent(maxDepth);

            try
            {
                JsonErrorKind kind = Run(tape, input, options, stack[..maxDepth], out int position);

                error = kind == JsonErrorKind.None
                            ? default
                            : tape.CreateError(kind, position);

                return kind == JsonErrorKind.None;
            }
            finally
            {
                if ( rented is not null ) { ArrayPool<int>.Shared.Return(rented); }
            }
        }


        private static JsonErrorKind Run( JsonTape tape, ReadOnlySpan<TChar> input, in JsonReaderOptions options, Span<int> stack, out int position )
        {
            ValueSpanReader<TChar> reader         = new(input);
            bool                   comments       = options.AllowComments;
            bool                   trailingCommas = options.AllowTrailingCommas;
            int                    maxString      = options.MaxStringLength;
            int                    depth          = 0;
            bool                   expectValue    = true;
            JsonErrorKind          error;

            reader.TryReadExact(JsonLexer<TChar>.Literal("﻿"u8, "﻿")); // a leading byte order mark

            while ( true )
            {
                if ( ( error = JsonLexer<TChar>.SkipTrivia(ref reader, comments) ) != JsonErrorKind.None ) { goto Fail; }

                if ( expectValue )
                {
                    if ( !reader.TryPeek(out TChar next) )
                    {
                        error = JsonErrorKind.UnexpectedEnd;
                        goto Fail;
                    }

                    int start = reader.Position;

                    switch ( JsonLexer<TChar>.U(next) )
                    {
                        case '{':
                        case '[':
                        {
                            if ( depth == stack.Length )
                            {
                                error = JsonErrorKind.DepthExceeded;
                                goto Fail;
                            }

                            bool isObject = JsonLexer<TChar>.U(next) == '{';

                            stack[depth] = AddValue(tape,
                                                    stack,
                                                    depth,
                                                    isObject
                                                        ? JsonTokenKind.Object
                                                        : JsonTokenKind.Array,
                                                    start,
                                                    0,
                                                    false);

                            depth++;
                            if ( depth > tape.__maxDepthSeen ) { tape.__maxDepthSeen = depth; }

                            reader.Advance(1);

                            if ( ( error = JsonLexer<TChar>.SkipTrivia(ref reader, comments) ) != JsonErrorKind.None ) { goto Fail; }

                            if ( reader.TryReadExact(JsonLexer<TChar>.Char(isObject
                                                                               ? '}'
                                                                               : ']')) ) // an empty container is a complete value
                            {
                                Close(tape, stack[--depth], reader.Position);
                                expectValue = false;
                                continue;
                            }

                            if ( isObject && ( error = ReadMemberName(tape, ref reader, comments, maxString, stack[depth - 1]) ) != JsonErrorKind.None ) { goto Fail; }

                            continue; // expectValue stays true: the first element, or the first member's value
                        }

                        case '"':
                        {
                            if ( ( error = JsonLexer<TChar>.ReadString(ref reader, maxString, out int contentStart, out int length, out bool escaped) ) != JsonErrorKind.None ) { goto Fail; }

                            AddValue(tape, stack, depth, JsonTokenKind.String, contentStart, length, escaped);
                            break;
                        }

                        case '-' or (>= '0' and <= '9'):
                        {
                            if ( ( error = JsonLexer<TChar>.ReadNumber(ref reader) ) != JsonErrorKind.None ) { goto Fail; }

                            AddValue(tape, stack, depth, JsonTokenKind.Number, start, reader.Position - start, false);
                            break;
                        }

                        case 't' when reader.TryReadExact(JsonLexer<TChar>.Literal("true"u8, "true")):
                            AddValue(tape, stack, depth, JsonTokenKind.True, start, 4, false);
                            break;

                        case 'f' when reader.TryReadExact(JsonLexer<TChar>.Literal("false"u8, "false")):
                            AddValue(tape, stack, depth, JsonTokenKind.False, start, 5, false);
                            break;

                        case 'n' when reader.TryReadExact(JsonLexer<TChar>.Literal("null"u8, "null")):
                            AddValue(tape, stack, depth, JsonTokenKind.Null, start, 4, false);
                            break;

                        default:
                            error = JsonErrorKind.UnexpectedToken;
                            goto Fail;
                    }

                    expectValue = false;
                    continue;
                }

                // After a value: the end of the document, a separator, or the end of the enclosing container.
                if ( depth == 0 )
                {
                    if ( !reader.End )
                    {
                        error = JsonErrorKind.TrailingData;
                        goto Fail;
                    }

                    position = reader.Position;
                    return JsonErrorKind.None;
                }

                int  container = stack[depth - 1];
                bool inObject  = tape.Entries[container].Kind == JsonTokenKind.Object;

                char close = inObject
                                 ? '}'
                                 : ']';

                if ( reader.TryReadExact(JsonLexer<TChar>.Char(',')) )
                {
                    if ( ( error = JsonLexer<TChar>.SkipTrivia(ref reader, comments) ) != JsonErrorKind.None ) { goto Fail; }

                    if ( trailingCommas && reader.TryReadExact(JsonLexer<TChar>.Char(close)) )
                    {
                        Close(tape, container, reader.Position);
                        depth--;
                        continue;
                    }

                    if ( inObject && ( error = ReadMemberName(tape, ref reader, comments, maxString, container) ) != JsonErrorKind.None ) { goto Fail; }

                    expectValue = true;
                    continue;
                }

                if ( reader.TryReadExact(JsonLexer<TChar>.Char(close)) )
                {
                    Close(tape, container, reader.Position);
                    depth--;
                    continue;
                }

                error = reader.End
                            ? JsonErrorKind.UnexpectedEnd
                            : JsonErrorKind.UnexpectedToken;

                goto Fail;
            }

            Fail:
            position = reader.Position;
            return error;
        }


        /// <summary> Adds a value; a value directly inside an array counts as one of its elements (object members are counted by their names). </summary>
        private static int AddValue( JsonTape tape, Span<int> stack, int depth, JsonTokenKind kind, int start, int length, bool escaped )
        {
            if ( depth > 0 )
            {
                ref Entry parent = ref tape.Entries[stack[depth - 1]];
                if ( parent.Kind == JsonTokenKind.Array ) { parent.Count++; } // before Add, which may move the entries
            }

            return tape.Add(kind, start, length, escaped);
        }

        private static void Close( JsonTape tape, int container, int end )
        {
            ref Entry entry = ref tape.Entries[container];
            entry.Length = end - entry.Start;
            entry.Next   = tape.__count;
        }


        /// <summary> <c>"name"</c>, optional trivia, then <c>:</c>. </summary>
        private static JsonErrorKind ReadMemberName( JsonTape tape, ref ValueSpanReader<TChar> reader, bool comments, int maxString, int container )
        {
            if ( !reader.IsNext(JsonLexer<TChar>.Char('"')) )
            {
                return reader.End
                           ? JsonErrorKind.UnexpectedEnd
                           : JsonErrorKind.UnexpectedToken;
            }

            JsonErrorKind error = JsonLexer<TChar>.ReadString(ref reader, maxString, out int start, out int length, out bool escaped);
            if ( error != JsonErrorKind.None ) { return error; }

            tape.Entries[container].Count++;
            tape.Add(JsonTokenKind.PropertyName, start, length, escaped);

            if ( ( error = JsonLexer<TChar>.SkipTrivia(ref reader, comments) ) != JsonErrorKind.None ) { return error; }

            return reader.TryReadExact(JsonLexer<TChar>.Char(':'))
                       ? JsonErrorKind.None
                       : reader.End
                           ? JsonErrorKind.UnexpectedEnd
                           : JsonErrorKind.UnexpectedToken;
        }
    }
}



/// <summary> One value in a <see cref="JsonTape"/>: the tape plus an entry index (16 bytes, safe to copy). Reading it after the tape is disposed throws <see cref="ObjectDisposedException"/>. </summary>
public readonly struct JsonItem
{
    private readonly JsonTape? __tape;
    private readonly int       __index;


    internal JsonItem( JsonTape tape, int index )
    {
        __tape  = tape;
        __index = index;
    }


    internal             JsonTape       Tape => __tape ?? throw new InvalidOperationException("default(JsonItem) doesn't belong to a tape.");
    private ref readonly JsonTape.Entry Data => ref Tape.Entries[__index];

    public JsonTokenKind Kind => Data.Kind;

    /// <summary> The offset of the value's first char or byte (a string's opening quote). </summary>
    public int Position => Data.Kind is JsonTokenKind.String or JsonTokenKind.PropertyName
                               ? Data.Start - 1
                               : Data.Start;

    public bool IsNull => Data.Kind == JsonTokenKind.Null;


    // ─── Objects ─────────────────────────────────────────────────────────────

    /// <exception cref="KeyNotFoundException"> The object has no such member. </exception>
    public JsonItem this[ string name ] => TryGetProperty(name, out JsonItem value)
                                               ? value
                                               : throw new KeyNotFoundException($"The object has no member '{name}'.");

    public int GetPropertyCount() => Expect(JsonTokenKind.Object).Count;

    /// <summary> The value of the first member named <paramref name="name"/> (the tape doesn't reject duplicate names). </summary>
    public bool TryGetProperty( ReadOnlySpan<char> name, out JsonItem value )
    {
        if ( !Tape.IsUtf8 ) { return Find(name, default, out value); }

        // A UTF-8 tape compares unescaped names byte for byte: encode the name once, not once per member.
        int     max    = name.Length * 3;
        byte[]? rented = null;

        Span<byte> utf8 = max <= 256
                              ? stackalloc byte[256]
                              : ( rented = ArrayPool<byte>.Shared.Rent(max) );

        try
        {
            Utf8.FromUtf16(name, utf8, out _, out int written);
            return Find(name, utf8[..written], out value);
        }
        finally
        {
            if ( rented is not null ) { ArrayPool<byte>.Shared.Return(rented); }
        }
    }

    /// <inheritdoc cref="TryGetProperty(ReadOnlySpan{char}, out JsonItem)"/>
    public bool TryGetProperty( ReadOnlySpan<byte> utf8Name, out JsonItem value )
    {
        char[]? rented = null;

        Span<char> utf16 = utf8Name.Length <= 256
                               ? stackalloc char[256]
                               : ( rented = ArrayPool<char>.Shared.Rent(utf8Name.Length) );

        try
        {
            if ( Utf8.ToUtf16(utf8Name, utf16, out _, out int written) != OperationStatus.Done )
            {
                value = default; // invalid UTF-8 can't name a member of a validated document
                return false;
            }

            return Find(utf16[..written], utf8Name, out value);
        }
        finally
        {
            if ( rented is not null ) { ArrayPool<char>.Shared.Return(rented); }
        }
    }

    private bool Find( ReadOnlySpan<char> utf16, ReadOnlySpan<byte> utf8, out JsonItem value )
    {
        ref readonly JsonTape.Entry entry   = ref Expect(JsonTokenKind.Object);
        JsonTape                    tape    = __tape!;
        JsonTape.Entry[]            entries = tape.Entries;

        // Members are (name, value) pairs: the name at i, its value at i + 1, the next name where the value's subtree ends.
        for ( int i = __index + 1, n = 0; n < entry.Count; n++, i = entries[i + 1].Next )
        {
            if ( !tape.StringEquals(in entries[i], utf16, utf8) ) { continue; }

            value = new JsonItem(tape, i + 1);
            return true;
        }

        value = default;
        return false;
    }

    public ObjectEnumerator EnumerateObject() => new(Tape, __index + 1, Expect(JsonTokenKind.Object).Count);


    // ─── Arrays ──────────────────────────────────────────────────────────────

    /// <summary> O(<paramref name="index"/>): walks the preceding elements. Use <see cref="EnumerateArray"/> to visit them all. </summary>
    public JsonItem this[ int index ]
    {
        get
        {
            ref readonly JsonTape.Entry entry = ref Expect(JsonTokenKind.Array);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)entry.Count, nameof(index));

            JsonTape.Entry[] entries = __tape!.Entries;
            int              i       = __index + 1;
            while ( index-- > 0 ) { i = entries[i].Next; }

            return new JsonItem(__tape, i);
        }
    }

    public int GetArrayLength() => Expect(JsonTokenKind.Array).Count;

    public ArrayEnumerator EnumerateArray() => new(Tape, __index + 1, Expect(JsonTokenKind.Array).Count);


    // ─── Scalars ─────────────────────────────────────────────────────────────

    public bool GetBoolean() => Data.Kind switch
                                {
                                    JsonTokenKind.True  => true,
                                    JsonTokenKind.False => false,
                                    var kind            => throw KindMismatch(kind, JsonTokenKind.True)
                                };

    /// <summary> Allocates the string (escapes decoded). Works on property names too. </summary>
    public string GetString() => Tape.GetString(in ExpectText());

    /// <summary> Decodes the string into <paramref name="destination"/>, without allocating; <see langword="false"/> if it doesn't fit. </summary>
    public bool TryCopyString( Span<char> destination, out int charsWritten ) => Tape.TryCopyString(in ExpectText(), destination, out charsWritten);

    /// <summary> Whether the string (or property name) equals <paramref name="text"/>, without allocating. </summary>
    public bool ValueEquals( ReadOnlySpan<char> text )
    {
        ref readonly JsonTape.Entry entry = ref ExpectText();
        JsonTape                    tape  = __tape!;
        if ( !tape.IsUtf8 || entry.Escaped ) { return tape.StringEquals(in entry, text, default); }

        int     max    = text.Length * 3;
        byte[]? rented = null;

        Span<byte> utf8 = max <= 256
                              ? stackalloc byte[256]
                              : ( rented = ArrayPool<byte>.Shared.Rent(max) );

        try
        {
            Utf8.FromUtf16(text, utf8, out _, out int written);
            return tape.StringEquals(in entry, text, utf8[..written]);
        }
        finally
        {
            if ( rented is not null ) { ArrayPool<byte>.Shared.Return(rented); }
        }
    }

    /// <summary> Parses the string with <typeparamref name="T"/>'s invariant-culture parser (<see cref="Guid"/>, <see cref="DateTimeOffset"/>, <c>Email</c>, ...) without allocating it. </summary>
    public T GetParsable<T>()
        where T : ISpanParsable<T>
    {
        ref readonly JsonTape.Entry entry  = ref ExpectText();
        char[]?                     rented = null;

        Span<char> buffer = entry.Length <= 256
                                ? stackalloc char[256]
                                : ( rented = ArrayPool<char>.Shared.Rent(entry.Length) ); // decoding never grows the text

        try
        {
            __tape!.TryCopyString(in entry, buffer, out int written);
            return T.Parse(buffer[..written], CultureInfo.InvariantCulture);
        }
        finally
        {
            if ( rented is not null ) { ArrayPool<char>.Shared.Return(rented); }
        }
    }


    public int     GetInt32()   => GetInteger<int>();
    public long    GetInt64()   => GetInteger<long>();
    public ulong   GetUInt64()  => GetInteger<ulong>();
    public double  GetDouble()  => GetFloat<double>();
    public decimal GetDecimal() => GetFloat<decimal>();

    /// <exception cref="FormatException"> The number has a fraction or exponent, or doesn't fit <typeparamref name="T"/>. </exception>
    public T GetInteger<T>()
        where T : struct, IBinaryInteger<T> => TryGetInteger(out T value)
                                                   ? value
                                                   : throw new FormatException($"{GetRawText()} doesn't fit the integer type (a fraction, an exponent or out of range).");

    /// <exception cref="FormatException"> The number overflows <typeparamref name="T"/>. </exception>
    public T GetFloat<T>()
        where T : struct, IFloatingPoint<T> => TryGetFloat(out T value)
                                                   ? value
                                                   : throw new FormatException($"{GetRawText()} overflows the floating-point type.");

    /// <summary> Strict: <c>1.0</c> and <c>1e2</c> aren't integers. </summary>
    public bool TryGetInteger<T>( out T value )
        where T : struct, IBinaryInteger<T> => TryParseNumber(NumberStyles.AllowLeadingSign, out value);

    /// <summary> <see langword="false"/> for values that would overflow to infinity (spec: overflow is an error, never saturated). </summary>
    public bool TryGetFloat<T>( out T value )
        where T : struct, IFloatingPoint<T> => TryParseNumber(NumberStyles.Float, out value) && !T.IsInfinity(value);

    private bool TryParseNumber<T>( NumberStyles style, out T value )
        where T : struct, INumberBase<T>
    {
        ref readonly JsonTape.Entry entry = ref Expect(JsonTokenKind.Number);
        JsonTape                    tape  = __tape!;

        return tape.IsUtf8
                   ? T.TryParse(tape.InputUtf8.Slice(entry.Start, entry.Length),  style, CultureInfo.InvariantCulture, out value)
                   : T.TryParse(tape.InputUtf16.Slice(entry.Start, entry.Length), style, CultureInfo.InvariantCulture, out value);
    }


    // ─── Whole values ────────────────────────────────────────────────────────

    /// <summary> The value's JSON text, exactly as it appears in the input. </summary>
    public string GetRawText() => Tape.GetRawText(in Data);

    /// <summary> Reads a model straight from the tape (no re-tokenizing). </summary>
    /// <exception cref="JsonReadException"> The value doesn't match <typeparamref name="T"/>. </exception>
    public T Deserialize<T>()
        where T : IJsonSerializable<T>
    {
        JsonTapeReader reader = new(Tape, __index); // §3.2 (P4)

        return T.TryReadJson(ref reader, out T? value)
                   ? value
                   : throw new JsonReadException(reader.Error);
    }

    public override string ToString() => __tape is null
                                             ? ""
                                             : GetRawText();


    // ─── Helpers ─────────────────────────────────────────────────────────────

    private ref readonly JsonTape.Entry Expect( JsonTokenKind kind )
    {
        ref readonly JsonTape.Entry entry = ref Data;
        if ( entry.Kind != kind ) { throw KindMismatch(entry.Kind, kind); }

        return ref entry;
    }

    private ref readonly JsonTape.Entry ExpectText()
    {
        ref readonly JsonTape.Entry entry = ref Data;
        if ( entry.Kind is not (JsonTokenKind.String or JsonTokenKind.PropertyName) ) { throw KindMismatch(entry.Kind, JsonTokenKind.String); }

        return ref entry;
    }

    private static InvalidOperationException KindMismatch( JsonTokenKind actual, JsonTokenKind expected ) => new($"The JSON value is {actual}, not {expected}.");


    // ─── Enumerators ─────────────────────────────────────────────────────────



    public struct ArrayEnumerator
    {
        private readonly JsonTape __tape;
        private          int      __next;
        private          int      __remaining;

        internal ArrayEnumerator( JsonTape tape, int first, int count )
        {
            __tape      = tape;
            __next      = first;
            __remaining = count;
            Current     = default;
        }

        public JsonItem Current { get; private set; }

        public readonly ArrayEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            if ( __remaining == 0 ) { return false; }

            __remaining--;
            Current = new JsonItem(__tape, __next);
            __next  = __tape.Entries[__next].Next;
            return true;
        }
    }



    public struct ObjectEnumerator
    {
        private readonly JsonTape __tape;
        private          int      __next;
        private          int      __remaining;

        internal ObjectEnumerator( JsonTape tape, int first, int count )
        {
            __tape      = tape;
            __next      = first;
            __remaining = count;
            Current     = default;
        }

        public JsonTapeProperty Current { get; private set; }

        public readonly ObjectEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            if ( __remaining == 0 ) { return false; }

            __remaining--;
            Current = new JsonTapeProperty(new JsonItem(__tape, __next), new JsonItem(__tape, __next + 1));
            __next  = __tape.Entries[__next + 1].Next;
            return true;
        }
    }
}



/// <summary> One object member of a <see cref="JsonTape"/>: its name (a <see cref="JsonTokenKind.PropertyName"/> item) and its value. </summary>
public readonly struct JsonTapeProperty( JsonItem name, JsonItem value )
{
    public JsonItem Name  { get; } = name;
    public JsonItem Value { get; } = value;

    public string GetName()                             => Name.GetString();
    public bool   NameEquals( ReadOnlySpan<char> text ) => Name.ValueEquals(text);
}
