// Jakar.Extensions :: Jakar.Extensions
// 06/07/2022  3:25 PM

using System.Buffers;
using System.Runtime.InteropServices;


#nullable enable



namespace Jakar.Extensions.SpanAndMemory;


/// <summary><para>Based on System.Text.ValueStringBuilder</para></summary>
public ref struct ValueStringBuilder
{
    private char[]?    _arrayToReturnToPool;
    private Span<char> _chars;
    private int        _pos;


    public int Length
    {
        readonly get => _pos;
        set
        {
            Debug.Assert(value >= 0,             "Value must be zero or greater");
            Debug.Assert(value <= _chars.Length, $"Value must be less than '{_chars.Length}'");
            _pos = value;
        }
    }

    public readonly int Capacity => _chars.Length;

    public ref char this[ int index ]
    {
        get
        {
            Debug.Assert(index < _pos);
            return ref _chars[index];
        }
    }


    public ValueStringBuilder() : this(64) { }
    public ValueStringBuilder( int initialCapacity )
    {
        _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
        _chars               = _arrayToReturnToPool;
        _pos                 = 0;
    }
    public ValueStringBuilder( in Span<char> initialBuffer )
    {
        _arrayToReturnToPool = null;
        _chars               = initialBuffer;
        _pos                 = 0;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        char[]? toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if ( toReturn is not null ) { ArrayPool<char>.Shared.Return(toReturn); }
    }


    /// <summary>
    /// Resize the internal buffer either by doubling current buffer size or
    /// by adding <paramref name="additionalCapacityBeyondPos"/> to
    /// <see cref="_pos"/> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    /// Number of chars requested beyond current position.
    /// </param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow( int additionalCapacityBeyondPos )
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(_pos > _chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        char[] poolArray = ArrayPool<char>.Shared.Rent(Math.Max(_pos + additionalCapacityBeyondPos, _chars.Length * 2));

        _chars.CopyTo(poolArray);

        char[]? toReturn              = _arrayToReturnToPool;
        _chars = _arrayToReturnToPool = poolArray;
        if ( toReturn is not null ) { ArrayPool<char>.Shared.Return(toReturn); }
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend( char c )
    {
        Grow(1);
        Append(c);
    }
    public void EnsureCapacity( int capacity )
    {
        if ( capacity > _chars.Length ) { Grow(capacity - _pos); }
    }


    /// <summary>
    /// Get a pinnable reference to the builder.
    /// Does not ensure there is a null char after <see cref="Length"/>.
    /// This overload is pattern matched in the C# 7.3+ compiler so you can omit the explicit method call, and write eg "fixed (char* c = builder)"
    /// </summary>
    public ref char GetPinnableReference() => ref _chars.GetPinnableReference();

    /// <summary> Get a pinnable reference to the builder. </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
    public ref char GetPinnableReference( bool terminate )
    {
        if ( terminate )
        {
            EnsureCapacity(Length + 1);
            _chars[Length] = '\0';
        }

        return ref GetPinnableReference();
    }


    public override string ToString()
    {
        var s = _chars[.._pos].ToString();
        Dispose();
        return s;
    }


    /// <summary>Returns the underlying storage of the builder.</summary>
    public Span<char> RawChars => _chars;


    /// <summary> Returns a span around the contents of the builder. </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
    [Pure]
    public ReadOnlySpan<char> AsSpan( bool terminate )
    {
        if ( !terminate ) { return _chars[.._pos]; }

        EnsureCapacity(Length + 1);
        _chars[Length] = '\0';
        return _chars[.._pos];
    }

    [Pure] public readonly ReadOnlySpan<char> AsSpan() => _chars[.._pos];
    [Pure] public readonly ReadOnlySpan<char> AsSpan( int start ) => _chars.Slice(start,             _pos - start);
    [Pure] public readonly ReadOnlySpan<char> AsSpan( int start, int length ) => _chars.Slice(start, length);


    public bool TryCopyTo( in Span<char> destination, out int charsWritten )
    {
        if ( _chars[.._pos].TryCopyTo(destination) )
        {
            charsWritten = _pos;
            Dispose();
            return true;
        }

        charsWritten = 0;
        Dispose();
        return false;
    }

    public void Insert( int index, char value, int count )
    {
        if ( _pos > _chars.Length - count ) { Grow(count); }

        int remaining = _pos - index;
        _chars.Slice(index, remaining).CopyTo(_chars[( index + count )..]);
        _chars.Slice(index, count).Fill(value);
        _pos += count;
    }
    public void Insert( int index, string s )
    {
        int count = s.Length;

        if ( _pos > _chars.Length - count ) { Grow(count); }

        int remaining = _pos - index;
        _chars.Slice(index, remaining).CopyTo(_chars[( index + count )..]);
        s.AsSpan().CopyTo(_chars[index..]);
        _pos += count;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append( char c )
    {
        int pos = _pos;

        if ( (uint)pos < (uint)_chars.Length )
        {
            _chars[pos] = c;
            _pos        = pos + 1;
        }
        else { GrowAndAppend(c); }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append( string s )
    {
        int pos = _pos;

        switch ( s.Length )
        {
            // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            case 1 when (uint)pos < (uint)_chars.Length:
            {
                _chars[pos] =  s[0];
                _pos        += 1;
                return;
            }

            // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            case 2 when (uint)pos < (uint)_chars.Length:
            {
                _chars[pos] =  s[0];
                _pos        += 1;
                _chars[pos] =  s[1];
                _pos        += 1;
                return;
            }

            default:
            {
                AppendSlow(s);
                return;
            }
        }
    }

    private void AppendSlow( string s )
    {
        int pos = _pos;
        if ( pos > _chars.Length - s.Length ) { Grow(s.Length); }

        s.AsSpan().CopyTo(_chars[pos..]);
        _pos += s.Length;
    }

    public void Append( char c, int count )
    {
        if ( _pos > _chars.Length - count ) { Grow(count); }

        Span<char> dst = _chars.Slice(_pos, count);
        for ( var i = 0; i < dst.Length; i++ ) { dst[i] = c; }

        _pos += count;
    }


    // public unsafe void Append( char* value, int length )
    // {
    //     int pos = _pos;
    //     if ( pos > _chars.Length - length ) { Grow(length); }
    //
    //     Span<char> dst = _chars.Slice(_pos, length);
    //     for ( var i = 0; i < dst.Length; i++ ) { dst[i] = *value++; }
    //
    //     _pos += length;
    // }

    public void Append( in ReadOnlySpan<char> value )
    {
        int pos = _pos;
        if ( pos > _chars.Length - value.Length ) { Grow(value.Length); }

        value.CopyTo(_chars[_pos..]);
        _pos += value.Length;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AppendSpan( int length )
    {
        int origPos = _pos;
        if ( origPos > _chars.Length - length ) { Grow(length); }

        _pos = origPos + length;
        return _chars.Slice(origPos, length);
    }


    public void AppendFormat( string? format, object? arg0 ) => AppendFormatHelper(null,                             format, new ParamsArray(arg0));
    public void AppendFormat( string? format, object? arg0, object? arg1 ) => AppendFormatHelper(null,               format, new ParamsArray(arg0, arg1));
    public void AppendFormat( string? format, object? arg0, object? arg1, object? arg2 ) => AppendFormatHelper(null, format, new ParamsArray(arg0, arg1, arg2));
    public void AppendFormat( string? format, params object?[] args )
    {
        if ( args == null )
        {
            // To preserve the original exception behavior, throw an exception about format if both
            // args and format are null. The actual null check for format is in AppendFormatHelper.
            string paramName = format is null
                                   ? nameof(format)
                                   : nameof(args);

            throw new ArgumentNullException(paramName);
        }

        AppendFormatHelper(null, format, new ParamsArray(args));
    }


    public void AppendFormat( IFormatProvider? provider, string? format, object? arg0 ) => AppendFormatHelper(provider,                             format, new ParamsArray(arg0));
    public void AppendFormat( IFormatProvider? provider, string? format, object? arg0, object? arg1 ) => AppendFormatHelper(provider,               format, new ParamsArray(arg0, arg1));
    public void AppendFormat( IFormatProvider? provider, string? format, object? arg0, object? arg1, object? arg2 ) => AppendFormatHelper(provider, format, new ParamsArray(arg0, arg1, arg2));
    public void AppendFormat( IFormatProvider? provider, string? format, params object?[] args )
    {
        if ( args == null )
        {
            // To preserve the original exception behavior, throw an exception about format if both
            // args and format are null. The actual null check for format is in AppendFormatHelper.
            string paramName = format is null
                                   ? nameof(format)
                                   : nameof(args);

            throw new ArgumentNullException(paramName);
        }

        AppendFormatHelper(provider, format, new ParamsArray(args));
    }


#if NET6_0
    public void AppendSpanFormattable<T>( T value, string? format, IFormatProvider? provider ) where T : ISpanFormattable
    {
        if ( value.TryFormat(_chars[_pos..], out int charsWritten, format, provider) ) { _pos += charsWritten; }
        else { Append(value.ToString(format, provider)); }
    }

#endif

    // Copied from StringBuilder, can't be done via generic extension
    // as ValueStringBuilder is a ref struct and cannot be used in a generic.
    internal void AppendFormatHelper( IFormatProvider? provider, string? format, ParamsArray args )
    {
        // Undocumented exclusive limits on the range for Argument Hole Index and Argument Hole Alignment.
        const int IndexLimit = 1000000; // Note:            0 <= ArgIndex < IndexLimit
        const int WidthLimit = 1000000; // Note:  -WidthLimit <  ArgAlign < WidthLimit

        if ( format == null ) { throw new ArgumentNullException(nameof(format)); }

        var pos = 0;
        int len = format.Length;
        var ch  = '\0';
        var cf  = (ICustomFormatter?)provider?.GetFormat(typeof(ICustomFormatter));

        while ( true )
        {
            while ( pos < len )
            {
                ch = format[pos];

                pos++;

                // Is it a closing brace?
                if ( ch == '}' )
                {
                    // Check next character (if there is one) to see if it is escaped. eg }}
                    if ( pos < len && format[pos] == '}' ) { pos++; }
                    else
                    {
                        // Otherwise treat it as an error (Mismatched closing brace)
                        ThrowFormatError();
                    }
                }

                // Is it a opening brace?
                else if ( ch == '{' )
                {
                    // Check next character (if there is one) to see if it is escaped. eg {{
                    if ( pos < len && format[pos] == '{' ) { pos++; }
                    else
                    {
                        // Otherwise treat it as the opening brace of an Argument Hole.
                        pos--;
                        break;
                    }
                }

                // If it's neither then treat the character as just text.
                Append(ch);
            }

            //
            // Start of parsing of Argument Hole.
            // Argument Hole ::= { Index (, WS* Alignment WS*)? (: Formatting)? }
            //
            if ( pos == len ) { break; }

            //
            //  Start of parsing required Index parameter.
            //  Index ::= ('0'-'9')+ WS*
            //
            pos++;

            // If reached end of text then error (Unexpected end of text)
            // or character is not a digit then error (Unexpected Character)
            if ( pos == len || ( ch = format[pos] ) < '0' || ch > '9' ) { ThrowFormatError(); }

            var index = 0;

            do
            {
                index = index * 10 + ch - '0';
                pos++;

                // If reached end of text then error (Unexpected end of text)
                if ( pos == len ) { ThrowFormatError(); }

                ch = format[pos];

                // so long as character is digit and value of the index is less than 1000000 ( index limit )
            } while ( ch is >= '0' and <= '9' && index < IndexLimit );

            // If value of index is not within the range of the arguments passed in then error (Index out of range)
            if ( index >= args.Length ) { throw new FormatException("Format Index Out Of Range"); }

            // Consume optional whitespace.
            while ( pos < len && ( ch = format[pos] ) == ' ' ) { pos++; }

            // End of parsing index parameter.

            //
            //  Start of parsing of optional Alignment
            //  Alignment ::= comma WS* minus? ('0'-'9')+ WS*
            //
            var leftJustify = false;
            var width       = 0;

            // Is the character a comma, which indicates the start of alignment parameter.
            if ( ch == ',' )
            {
                pos++;

                // Consume Optional whitespace
                while ( pos < len && format[pos] == ' ' ) { pos++; }

                // If reached the end of the text then error (Unexpected end of text)
                if ( pos == len ) { ThrowFormatError(); }

                // Is there a minus sign?
                ch = format[pos];

                if ( ch == '-' )
                {
                    // Yes, then alignment is left justified.
                    leftJustify = true;
                    pos++;

                    // If reached end of text then error (Unexpected end of text)
                    if ( pos == len ) { ThrowFormatError(); }

                    ch = format[pos];
                }

                // If current character is not a digit then error (Unexpected character)
                if ( ch < '0' || ch > '9' ) { ThrowFormatError(); }

                // Parse alignment digits.
                do
                {
                    width = width * 10 + ch - '0';
                    pos++;

                    // If reached end of text then error. (Unexpected end of text)
                    if ( pos == len ) { ThrowFormatError(); }

                    ch = format[pos];

                    // So long a current character is a digit and the value of width is less than 100000 ( width limit )
                } while ( ch is >= '0' and <= '9' && width < WidthLimit );

                // end of parsing Argument Alignment
            }

            // Consume optional whitespace
            while ( pos < len && ( ch = format[pos] ) == ' ' ) { pos++; }

            //
            // Start of parsing of optional formatting parameter.
            //
            object? arg = args[index];

            ReadOnlySpan<char> itemFormatSpan = default; // used if itemFormat is null

            // Is current character a colon? which indicates start of formatting parameter.
            if ( ch == ':' )
            {
                pos++;
                int startPos = pos;

                while ( true )
                {
                    // If reached end of text then error. (Unexpected end of text)
                    if ( pos == len ) { ThrowFormatError(); }

                    ch = format[pos];

                    if ( ch == '}' )
                    {
                        // Argument hole closed
                        break;
                    }

                    if ( ch == '{' )
                    {
                        // Braces inside the argument hole are not supported
                        ThrowFormatError();
                    }

                    pos++;
                }

                if ( pos > startPos ) { itemFormatSpan = format.AsSpan(startPos, pos - startPos); }
            }
            else if ( ch != '}' )
            {
                // Unexpected character
                ThrowFormatError();
            }

            // Construct the output for this arg hole.
            pos++;
            string? s          = null;
            string? itemFormat = null;

            if ( cf is not null )
            {
                if ( itemFormatSpan.Length != 0 ) { itemFormat = new string(itemFormatSpan); }

                s = cf.Format(itemFormat, arg, provider);
            }

            if ( s == null )
            {
            #if NET6_0

                // If arg is ISpanFormattable and the beginning doesn't need padding, try formatting it into the remaining current chunk.
                if ( arg is ISpanFormattable spanFormattableArg && ( leftJustify || width == 0 ) && spanFormattableArg.TryFormat(_chars[_pos..], out int charsWritten, itemFormatSpan, provider) )
                {
                    _pos += charsWritten;

                    // Pad the end, if needed.
                    int padding = width - charsWritten;
                    if ( leftJustify && padding > 0 ) { Append(' ', padding); }

                    // Continue to parse other characters.
                    continue;
                }
            #endif

                // Otherwise, fallback to trying IFormattable or calling ToString.
                if ( arg is IFormattable formattableArg )
                {
                    if ( itemFormatSpan.Length != 0 ) { itemFormat ??= new string(itemFormatSpan); }

                    s = formattableArg.ToString(itemFormat, provider);
                }
                else if ( arg is not null ) { s = arg.ToString(); }
            }

            // Append it to the final output of the Format String.
            s ??= string.Empty;

            int pad = width - s.Length;
            if ( !leftJustify && pad > 0 ) { Append(' ', pad); }

            Append(s);
            if ( leftJustify && pad > 0 ) { Append(' ', pad); }

            // Continue to parse other characters.
        }
    }


    [DoesNotReturn] private static void ThrowFormatError() => throw new FormatException("Invalid Format String");


    // private          int                _pos                 = 0;
    // private          char[]?            _arrayToReturnToPool = default;
    // private readonly IFormatProvider?   _provider            = default;
    // private readonly bool               _hasCustomFormatter  = default;
    // internal         ReadOnlySpan<char> Text => _buffer[.._pos];
    // internal static DefaultInterpolatedStringHandler Handler( in ReadOnlySpan<char> span ) => new(span.Length, 1);


    // /// <summary>Gets the built <see cref="string"/> and clears the handler.</summary>
    // /// <returns>The built string.</returns>
    // /// <remarks>
    // /// This releases any resources used by the handler. The method should be invoked only
    // /// once and as the last thing performed on the handler. Subsequent use is erroneous, ill-defined,
    // /// and may destabilize the process, as may using any other copies of the handler after ToStringAndClear
    // /// is called on any one of them.
    // /// </remarks>
    // public string ToStringAndClear()
    // {
    //     string result = new string(Text);
    //     Clear();
    //     return result;
    // }
    //
    // /// <summary>Clears the handler, returning any rented array to the pool.</summary>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)] // used only on a few hot paths
    // internal void Clear()
    // {
    //     char[]? toReturn = _arrayToReturnToPool;
    //     this = default; // defensive clear
    //     if ( toReturn is not null ) { ArrayPool<char>.Shared.Return(toReturn); }
    // }
    //
    //
    // /// <summary>Writes the specified string to the handler.</summary>
    // /// <param name="value">The string to write.</param>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public void AppendLiteral( string value ) => AppendLiteral(value.AsSpan());
    //
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public void AppendLiteral( in ReadOnlySpan<char> value )
    // {
    //     // AppendLiteral is expected to always be called by compiler-generated code with a literal string.
    //     // By inlining it, the method body is exposed to the constant length of that literal, allowing the JIT to
    //     // prune away the irrelevant cases.  This effectively enables multiple implementations of AppendLiteral,
    //     // special-cased on and optimized for the literal's length.  We special-case lengths 1 and 2 because
    //     // they're very common, e.g.
    //     //     1: ' ', '.', '-', '\t', etc.
    //     //     2: ", ", "0x", "=>", ": ", etc.
    //     // but we refrain from adding more because, in the rare case where AppendLiteral is called with a non-literal,
    //     // there is a lot of code here to be inlined.
    //
    //     // TODO: https://github.com/dotnet/runtime/issues/41692#issuecomment-685192193
    //     // What we really want here is to be able to add a bunch of additional special-cases based on length,
    //     // e.g. a switch with a case for each length <= 8, not mark the method as AggressiveInlining, and have
    //     // it inlined when provided with a string literal such that all the other cases evaporate but not inlined
    //     // if called directly with something that doesn't enable pruning.  Even better, if "literal".TryCopyTo
    //     // could be unrolled based on the literal, ala https://github.com/dotnet/runtime/pull/46392, we might
    //     // be able to remove all special-casing here.
    //
    //     switch ( value.Length )
    //     {
    //         case 1:
    //         {
    //             Span<char> chars = _buffer;
    //             int        pos   = _pos;
    //
    //             if ( (uint)pos < (uint)chars.Length )
    //             {
    //                 chars[pos] = value[0];
    //                 _pos       = pos + 1;
    //             }
    //             else { GrowThenCopy(value); }
    //
    //             return;
    //         }
    //
    //         case 2:
    //         {
    //             int pos = _pos;
    //
    //             if ( (uint)pos < _buffer.Length - 1 )
    //             {
    //                 Unsafe.WriteUnaligned(ref Unsafe.As<char, byte>(ref Unsafe.Add(ref MemoryMarshal.GetReference(_buffer), pos)), Unsafe.ReadUnaligned<int>(ref Unsafe.As<char, byte>(ref MemoryMarshal.GetReference(value))));
    //                 _pos = pos + 2;
    //             }
    //             else { GrowThenCopy(value); }
    //
    //             return;
    //         }
    //
    //         default:
    //             AppendStringDirect(value);
    //             return;
    //     }
    // }
    //
    //
    // // Design note:
    // // The compiler requires a AppendFormatted overload for anything that might be within an interpolation expression;
    // // if it can't find an appropriate overload, for handlers in general it'll simply fail to compile.
    // // (For target-typing to string where it uses DefaultInterpolatedStringHandler implicitly, it'll instead fall back to
    // // its other mechanisms, e.g. using string.Format.  This fallback has the benefit that if we miss a case,
    // // interpolated strings will still work, but it has the downside that a developer generally won't know
    // // if the fallback is happening and they're paying more.)
    // //
    // // At a minimum, then, we would need an overload that accepts:
    // //     (object value, int alignment = 0, string? format = null)
    // // Such an overload would provide the same expressiveness as string.Format.  However, this has several
    // // shortcomings:
    // // - Every value type in an interpolation expression would be boxed.
    // // - ReadOnlySpan<char> could not be used in interpolation expressions.
    // // - Every AppendFormatted call would have three arguments at the call site, bloating the IL further.
    // // - Every invocation would be more expensive, due to lack of specialization, every call needing to account
    // //   for alignment and format, etc.
    // //
    // // To address that, we could just have overloads for T and ReadOnlySpan<char>:
    // //     (T)
    // //     (T, int alignment)
    // //     (T, string? format)
    // //     (T, int alignment, string? format)
    // //     (ReadOnlySpan<char>)
    // //     (ReadOnlySpan<char>, int alignment)
    // //     (ReadOnlySpan<char>, string? format)
    // //     (ReadOnlySpan<char>, int alignment, string? format)
    // // but this also has shortcomings:
    // // - Some expressions that would have worked with an object overload will now force a fallback to string.Format
    // //   (or fail to compile if the handler is used in places where the fallback isn't provided), because the compiler
    // //   can't always target type to T, e.g. `b switch { true => 1, false => null }` where `b` is a bool can successfully
    // //   be passed as an argument of type `object` but not of type `T`.
    // // - Reference types get no benefit from going through the generic code paths, and actually incur some overheads
    // //   from doing so.
    // // - Nullable value types also pay a heavy price, in particular around interface checks that would generally evaporate
    // //   at compile time for value types but don't (currently) if the Nullable<T> goes through the same code paths
    // //   (see https://github.com/dotnet/runtime/issues/50915).
    // //
    // // We could try to take a more elaborate approach for DefaultInterpolatedStringHandler, since it is the most common handler
    // // and we want to minimize overheads both at runtime and in IL size, e.g. have a complete set of overloads for each of:
    // //     (T, ...) where T : struct
    // //     (T?, ...) where T : struct
    // //     (object, ...)
    // //     (ReadOnlySpan<char>, ...)
    // //     (string, ...)
    // // but this also has shortcomings, most importantly:
    // // - If you have an unconstrained T that happens to be a value type, it'll now end up getting boxed to use the object overload.
    // //   This also necessitates the T? overload, since nullable value types don't meet a T : struct constraint, so without those
    // //   they'd all map to the object overloads as well.
    // // - Any reference type with an implicit cast to ROS<char> will fail to compile due to ambiguities between the overloads. string
    // //   is one such type, hence needing dedicated overloads for it that can be bound to more tightly.
    // //
    // // A middle ground we've settled on, which is likely to be the right approach for most other handlers as well, would be the set:
    // //     (T, ...) with no constraint
    // //     (ReadOnlySpan<char>) and (ReadOnlySpan<char>, int)
    // //     (object, int alignment = 0, string? format = null)
    // //     (string) and (string, int)
    // // This would address most of the concerns, at the expense of:
    // // - Most reference types going through the generic code paths and so being a bit more expensive.
    // // - Nullable types being more expensive until https://github.com/dotnet/runtime/issues/50915 is addressed.
    // //   We could choose to add a T? where T : struct set of overloads if necessary.
    // // Strings don't require their own overloads here, but as they're expected to be very common and as we can
    // // optimize them in several ways (can copy the contents directly, don't need to do any interface checks, don't
    // // need to pay the shared generic overheads, etc.) we can add overloads specifically to optimize for them.
    // //
    // // Hole values are formatted according to the following policy:
    // // 1. If an IFormatProvider was supplied and it provides an ICustomFormatter, use ICustomFormatter.Format (even if the value is null).
    // // 2. If the type implements ISpanFormattable, use ISpanFormattable.TryFormat.
    // // 3. If the type implements IFormattable, use IFormattable.ToString.
    // // 4. Otherwise, use object.ToString.
    // // This matches the behavior of string.Format, StringBuilder.AppendFormat, etc.  The only overloads for which this doesn't
    // // apply is ReadOnlySpan<char>, which isn't supported by either string.Format nor StringBuilder.AppendFormat, but more
    // // importantly which can't be boxed to be passed to ICustomFormatter.Format.
    //
    //
    // /// <summary>Writes the specified value to the handler.</summary>
    // /// <param name="value">The value to write.</param>
    // public void AppendFormatted<T>( T value )
    // {
    //     // This method could delegate to AppendFormatted with a null format, but explicitly passing
    //     // default as the format to TryFormat helps to improve code quality in some cases when TryFormat is inlined,
    //     // e.g. for Int32 it enables the JIT to eliminate code in the inlined method based on a length check on the format.
    //
    //     // If there's a custom formatter, always use it.
    //     if ( _hasCustomFormatter )
    //     {
    //         AppendCustomFormatter(value, format: null);
    //         return;
    //     }
    //
    //     // Check first for IFormattable, even though we'll prefer to use ISpanFormattable, as the latter
    //     // requires the former.  For value types, it won't matter as the type checks devolve into
    //     // JIT-time constants.  For reference types, they're more likely to implement IFormattable
    //     // than they are to implement ISpanFormattable: if they don't implement either, we save an
    //     // interface check over first checking for ISpanFormattable and then for IFormattable, and
    //     // if it only implements IFormattable, we come out even: only if it implements both do we
    //     // end up paying for an extra interface check.
    //     string? s;
    //
    //     if ( value is IFormattable )
    //     {
    //         // If the value can format itself directly into our buffer, do so.
    //         if ( value is ISpanFormattable )
    //         {
    //             int charsWritten;
    //
    //             while ( !( (ISpanFormattable)value ).TryFormat(_buffer[_pos..], out charsWritten, default, _provider) ) // constrained call avoiding boxing for value types
    //             {
    //                 Grow();
    //             }
    //
    //             _pos += charsWritten;
    //             return;
    //         }
    //
    //         s = ( (IFormattable)value ).ToString(format: null, _provider); // constrained call avoiding boxing for value types
    //     }
    //     else { s = value?.ToString(); }
    //
    //     if ( s is not null ) { AppendStringDirect(s); }
    // }
    // /// <summary>Writes the specified value to the handler.</summary>
    // /// <param name="value">The value to write.</param>
    // /// <param name="format">The format string.</param>
    // public void AppendFormatted<T>( T value, string? format )
    // {
    //     // If there's a custom formatter, always use it.
    //     if ( _hasCustomFormatter )
    //     {
    //         AppendCustomFormatter(value, format);
    //         return;
    //     }
    //
    //     // Check first for IFormattable, even though we'll prefer to use ISpanFormattable, as the latter
    //     // requires the former.  For value types, it won't matter as the type checks devolve into
    //     // JIT-time constants.  For reference types, they're more likely to implement IFormattable
    //     // than they are to implement ISpanFormattable: if they don't implement either, we save an
    //     // interface check over first checking for ISpanFormattable and then for IFormattable, and
    //     // if it only implements IFormattable, we come out even: only if it implements both do we
    //     // end up paying for an extra interface check.
    //     string? s;
    //
    //     if ( value is IFormattable formattable )
    //     {
    //         // If the value can format itself directly into our buffer, do so.
    //         if ( value is ISpanFormattable spanFormattable )
    //         {
    //             int charsWritten;
    //
    //             while ( !spanFormattable.TryFormat(_buffer[_pos..], out charsWritten, format, _provider) ) // constrained call avoiding boxing for value types
    //             {
    //                 Grow();
    //             }
    //
    //             _pos += charsWritten;
    //             return;
    //         }
    //
    //         s = formattable.ToString(format, _provider); // constrained call avoiding boxing for value types
    //     }
    //     else { s = value?.ToString(); }
    //
    //     if ( s is not null ) { AppendStringDirect(s); }
    // }
    //
    // /// <summary>Writes the specified value to the handler.</summary>
    // /// <param name="value">The value to write.</param>
    // /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
    // public void AppendFormatted<T>( T value, int alignment )
    // {
    //     int startingPos = _pos;
    //     AppendFormatted(value);
    //     if ( alignment != 0 ) { AppendOrInsertAlignmentIfNeeded(startingPos, alignment); }
    // }
    //
    // /// <summary>Writes the specified value to the handler.</summary>
    // /// <param name="value">The value to write.</param>
    // /// <param name="format">The format string.</param>
    // /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
    // public void AppendFormatted<T>( T value, int alignment, string? format )
    // {
    //     int startingPos = _pos;
    //     AppendFormatted(value, format);
    //     if ( alignment != 0 ) { AppendOrInsertAlignmentIfNeeded(startingPos, alignment); }
    // }
    //
    //
    // /// <summary>Writes the specified character span to the handler.</summary>
    // /// <param name="value">The span to write.</param>
    // public void AppendFormatted( in ReadOnlySpan<char> value )
    // {
    //     // Fast path for when the value fits in the current buffer
    //     if ( value.TryCopyTo(_buffer[_pos..]) ) { _pos += value.Length; }
    //     else { GrowThenCopy(value); }
    // }
    //
    // /// <summary>Writes the specified string of chars to the handler.</summary>
    // /// <param name="value">The span to write.</param>
    // /// <param name="alignment">Minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
    // /// <param name="format">The format string.</param>
    // public void AppendFormatted( in ReadOnlySpan<char> value, int alignment, string? format = null )
    // {
    //     var leftAlign = false;
    //
    //     if ( alignment < 0 )
    //     {
    //         leftAlign = true;
    //         alignment = -alignment;
    //     }
    //
    //     int paddingRequired = alignment - value.Length;
    //
    //     if ( paddingRequired <= 0 )
    //     {
    //         // The value is as large or larger than the required amount of padding,
    //         // so just write the value.
    //         AppendFormatted(value);
    //         return;
    //     }
    //
    //     // Write the value along with the appropriate padding.
    //     EnsureCapacityForAdditionalChars(value.Length + paddingRequired);
    //
    //     if ( leftAlign )
    //     {
    //         value.CopyTo(_buffer[_pos..]);
    //         _pos += value.Length;
    //         _buffer.Slice(_pos, paddingRequired).Fill(' ');
    //         _pos += paddingRequired;
    //     }
    //     else
    //     {
    //         _buffer.Slice(_pos, paddingRequired).Fill(' ');
    //         _pos += paddingRequired;
    //         value.CopyTo(_buffer[_pos..]);
    //         _pos += value.Length;
    //     }
    // }
    //
    //
    // /// <summary>Writes the specified value to the handler.</summary>
    // /// <param name="value">The value to write.</param>
    // public void AppendFormatted( string? value )
    // {
    //     switch ( _hasCustomFormatter )
    //     {
    //         // Fast-path for no custom formatter and a non-null string that fits in the current destination buffer.
    //         case false when value is not null && value.TryCopyTo(_buffer[_pos..]):
    //             _pos += value.Length;
    //             break;
    //
    //         case true:
    //             AppendCustomFormatter(value, format: null);
    //             break;
    //
    //         default:
    //         {
    //             if ( value is not null )
    //             {
    //                 EnsureCapacityForAdditionalChars(value.Length);
    //                 value.CopyTo(_buffer[_pos..]);
    //                 _pos += value.Length;
    //             }
    //
    //             break;
    //         }
    //     }
    // }
    //
    //
    // /// <summary>Gets whether the provider provides a custom formatter.</summary>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)] // only used in a few hot path call sites
    // internal static bool HasCustomFormatter( IFormatProvider provider )
    // {
    //     Debug.Assert(provider is not null);
    //     Debug.Assert(provider is not CultureInfo || provider.GetFormat(typeof(ICustomFormatter)) is null, "Expected CultureInfo to not provide a custom formatter");
    //
    //     return provider.GetType() != typeof(CultureInfo) && // optimization to avoid GetFormat in the majority case
    //            provider.GetFormat(typeof(ICustomFormatter)) != null;
    // }
    //
    // /// <summary>Formats the value using the custom formatter from the provider.</summary>
    // /// <param name="value">The value to write.</param>
    // /// <param name="format">The format string.</param>
    // [MethodImpl(MethodImplOptions.NoInlining)]
    // private void AppendCustomFormatter<T>( T value, string? format )
    // {
    //     // This case is very rare, but we need to handle it prior to the other checks in case a provider was used that supplied an ICustomFormatter which wanted to intercept the particular value.
    //     // We do the cast here rather than in the ctor, even though this could be executed multiple times per formatting, to make the cast pay for play.
    //     Debug.Assert(_hasCustomFormatter);
    //     Debug.Assert(_provider is not null);
    //
    //     var formatter = _provider?.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter;
    //     Debug.Assert(formatter is not null, "An incorrectly written provider said it implemented ICustomFormatter, and then didn't");
    //
    //
    //     string customFormatted = formatter.Format(format, value, _provider);
    //     if ( !string.IsNullOrWhiteSpace(customFormatted) ) { AppendStringDirect(customFormatted); }
    // }
    //
    // /// <summary>Handles adding any padding required for aligning a formatted value in an interpolation expression.</summary>
    // /// <param name="startingPos">The position at which the written value started.</param>
    // /// <param name="alignment">Non-zero minimum number of characters that should be written for this value.  If the value is negative, it indicates left-aligned and the required minimum is the absolute value.</param>
    // private void AppendOrInsertAlignmentIfNeeded( int startingPos, int alignment )
    // {
    //     Debug.Assert(startingPos >= 0 && startingPos <= _pos);
    //     Debug.Assert(alignment != 0);
    //
    //     int charsWritten = _pos - startingPos;
    //
    //     bool leftAlign = false;
    //
    //     if ( alignment < 0 )
    //     {
    //         leftAlign = true;
    //         alignment = -alignment;
    //     }
    //
    //     int paddingNeeded = alignment - charsWritten;
    //
    //     if ( paddingNeeded > 0 )
    //     {
    //         EnsureCapacityForAdditionalChars(paddingNeeded);
    //
    //         if ( leftAlign ) { _buffer.Slice(_pos, paddingNeeded).Fill(' '); }
    //         else
    //         {
    //             _buffer.Slice(startingPos, charsWritten).CopyTo(_buffer[( startingPos + paddingNeeded )..]);
    //             _buffer.Slice(startingPos, paddingNeeded).Fill(' ');
    //         }
    //
    //         _pos += paddingNeeded;
    //     }
    // }
    //
    //
    // /// <summary>Ensures <see cref="_buffer"/> has the capacity to store <paramref name="additionalChars"/> beyond <see cref="_pos"/>.</summary>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // private void EnsureCapacityForAdditionalChars( in int additionalChars )
    // {
    //     if ( _buffer.Length - _pos < additionalChars ) { Grow(additionalChars); }
    // }
    //
    //
    // /// <summary>Writes the specified string to the handler.</summary>
    // /// <param name="value">The string to write.</param>
    // private void AppendStringDirect( string value )
    // {
    //     if ( value.TryCopyTo(_buffer[_pos..]) ) { _pos += value.Length; }
    //     else { GrowThenCopy(value); }
    // }
    // /// <summary>Writes the specified string to the handler.</summary>
    // /// <param name="value">The string to write.</param>
    // private void AppendStringDirect( in ReadOnlySpan<char> value )
    // {
    //     if ( value.TryCopyTo(_buffer[_pos..]) ) { _pos += value.Length; }
    //     else { GrowThenCopy(value); }
    // }
    //
    //
    // /// <summary>Fallback for fast path in <see cref="AppendStringDirect(string)"/> when there's not enough space in the destination.</summary>
    // /// <param name="value">The string to write.</param>
    // [MethodImpl(MethodImplOptions.NoInlining)]
    // private void GrowThenCopy( string value )
    // {
    //     Grow(value.Length);
    //     value.CopyTo(_buffer[_pos..]);
    //     _pos += value.Length;
    // }
    // /// <summary>Fallback for AppendFormatted(ReadOnlySpan{char}) for when not enough space exists in the current buffer.</summary>
    // /// <param name="value">The span to write.</param>
    // [MethodImpl(MethodImplOptions.NoInlining)]
    // private void GrowThenCopy( in ReadOnlySpan<char> value )
    // {
    //     Grow(value.Length);
    //     value.CopyTo(_buffer[_pos..]);
    //     _pos += value.Length;
    // }
    //
    // /// <summary>
    // /// <para> Grows the size of <see cref="_buffer"/></para>
    // /// <para> This method is called when the remaining space in _buffer isn't sufficient to continue the operation. </para>
    // /// <para> Thus, we need at least one character beyond _buffer.Length. </para>
    // /// <para> <see cref="Grow(uint)"/> will handle growing by more than that if possible. </para>
    // /// </summary>
    // [MethodImpl(MethodImplOptions.NoInlining)] // keep consumers as streamlined as possible
    // private void Grow() => Grow((uint)_buffer.Length + 1);
    //
    // /// <summary>
    // /// <para>Grows <see cref="_buffer"/> to have the capacity to store at least <paramref name="additionalChars"/> beyond <see cref="_pos"/></para>
    // /// <para> This method is called when the remaining space (_buffer.Length - _pos) is insufficient to store a specific number of additional characters. </para>
    // /// <para> Thus, we need to grow to at least that new total. <see cref="Grow(uint)"/> will handle growing by more than that if possible. </para>
    // /// </summary>
    // [MethodImpl(MethodImplOptions.NoInlining)] // keep consumers as streamlined as possible
    // private void Grow( int additionalChars )
    // {
    //     Debug.Assert(additionalChars > _buffer.Length - _pos);
    //     Grow((uint)_pos + (uint)additionalChars);
    // }
    //
    // /// <summary>
    // /// <para>Grow the size of <see cref="_buffer"/> to at least the specified <paramref name="requiredMinCapacity"/></para>
    // /// <para> We want the max of how much space we actually required and doubling our capacity (without going beyond the max allowed length). </para>
    // /// <para> We also want to avoid asking for small arrays, to reduce the number of times we need to grow, and since we're working with unsigned ints that could technically overflow if someone tried to, for example, append a huge string to a huge string, we also clamp to int.MaxValue. </para>
    // /// <para> Even if the array creation fails in such a case, we may later fail in ToStringAndClear. </para>
    // /// </summary>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)] // but reuse this grow logic directly in both of the above grow routines
    // private void Grow( uint requiredMinCapacity )
    // {
    //     uint newCapacity = Math.Max(requiredMinCapacity, Math.Min((uint)_buffer.Length * 2, 0x3FFFFFDF));
    //     var  arraySize   = (int)Math.Clamp(newCapacity, MINIMUM_ARRAY_POOL_LENGTH, int.MaxValue);
    //
    //     char[] newArray = ArrayPool<char>.Shared.Rent(arraySize);
    //     _buffer[.._pos].CopyTo(newArray);
    //
    //     char[]? toReturn               = _arrayToReturnToPool;
    //     _buffer = _arrayToReturnToPool = newArray;
    //
    //     if ( toReturn is not null ) { ArrayPool<char>.Shared.Return(toReturn); }
    // }
}
