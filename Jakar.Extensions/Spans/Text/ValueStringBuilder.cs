// Jakar.Extensions :: Jakar.Extensions
// 06/07/2022  3:25 PM


namespace Jakar.Extensions;


/// <summary>
///     <para> Based on System.Text.ValueStringBuilder </para>
/// </summary>
public ref struct ValueStringBuilder : ISpanFormattable, IDisposable
{
    private Buffer<char> __chars;


    public readonly bool       IsEmpty  { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __chars.IsEmpty; }
    public readonly int        Capacity { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __chars.Capacity; }
    public readonly Span<char> Next     { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __chars.Next; }
    public readonly Span<char> Span     { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __chars.Span; }
    public readonly Span<char> this[ Range range ] => __chars[range];
    public ref char this[ Index            index ] => ref __chars[index];
    public ref char this[ int              index ] => ref __chars[index];
    public readonly ReadOnlySpan<char> Result { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __chars.Span; }
    public          int                Length { [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => __chars.Length; set => __chars.Length = value; }
    public readonly ReadOnlySpan<char> Values => __chars.Values;


    public ValueStringBuilder() : this(DEFAULT_CAPACITY) { }
    public ValueStringBuilder( int                       initialCapacity ) => __chars = new Buffer<char>(initialCapacity);
    public ValueStringBuilder( params ReadOnlySpan<char> span ) => __chars = new Buffer<char>(span);
    public void Dispose() => __chars.Dispose();


    public ValueStringBuilder Reset()
    {
        __chars.Span.Fill('\0');
        return this;
    }
    public void EnsureCapacity<TValue>( ref readonly ReadOnlySpan<char> format )   => EnsureCapacity(Math.Max(format.Length, Sizes.GetBufferSize<TValue>()));
    public void EnsureCapacity( int                                     capacity ) => __chars.EnsureCapacity(capacity);


    /// <summary> Get a pinnable reference to the builder. Does not ensure there is a null char after <see cref="Length"/> . This overload is pattern matched  the C# 7.3+ compiler so you can omit the explicit method call, and write eg "fixed (char* c = builder)" </summary>
    [Pure] public readonly ref char GetPinnableReference() => ref __chars.Span.GetPinnableReference();


    /// <summary> Get a pinnable reference to the builder. </summary>
    /// <param name="terminate"> Ensures that the builder has a null char after <see cref="Length"/> </param>
    [Pure] public ref char GetPinnableReference( bool terminate )
    {
        if ( terminate ) { return ref GetPinnableReference('\0'); }

        return ref GetPinnableReference();
    }


    /// <summary> Get a pinnable reference to the builder. </summary>
    /// <param name="terminate"> Ensures that the builder has a null char after <see cref="Length"/> </param>
    [Pure] public ref char GetPinnableReference( char terminate )
    {
        EnsureCapacity(Length + 1);
        __chars[++Length] = terminate;

        return ref GetPinnableReference();
    }


    [Pure] public readonly ReadOnlySpan<char> AsSpan()                       => __chars.Span;
    [Pure] public readonly ReadOnlySpan<char> Slice( int start )             => __chars[start..];
    [Pure] public readonly ReadOnlySpan<char> Slice( int start, int length ) => __chars.Span.Slice(start, length);


    public bool TryCopyTo( scoped ref Span<char> destination, out int charsWritten )
    {
        if ( __chars.Span.TryCopyTo(destination) )
        {
            charsWritten = __chars.Length;
            Dispose();
            return true;
        }

        charsWritten = 0;
        Dispose();
        return false;
    }


    public ValueStringBuilder Trim( char value )
    {
        __chars.Trim(value);
        return this;
    }
    public ValueStringBuilder Trim( params ReadOnlySpan<char> value )
    {
        __chars.Trim(value);
        return this;
    }
    public ValueStringBuilder TrimEnd( char value )
    {
        __chars.TrimEnd(value);
        return this;
    }
    public ValueStringBuilder TrimEnd( params ReadOnlySpan<char> value )
    {
        __chars.TrimEnd(value);
        return this;
    }
    public ValueStringBuilder TrimStart( char value )
    {
        __chars.TrimStart(value);
        return this;
    }
    public ValueStringBuilder TrimStart( params ReadOnlySpan<char> value )
    {
        __chars.TrimStart(value);
        return this;
    }


    public ValueStringBuilder Replace( int index, char value )
    {
        __chars.Span[index] = value;
        return this;
    }
    public ValueStringBuilder Replace( int index, char value, int count )
    {
        for ( int i = 0; i < count; i++ ) { __chars.Span[index + i] = value; }

        return this;
    }
    public ValueStringBuilder Replace( int index, params ReadOnlySpan<char> value )
    {
        value.CopyTo(__chars.Span[index..]);
        return this;
    }


    public ValueStringBuilder Insert( int index, char value )
    {
        __chars.Insert(index, value);
        return this;
    }
    public ValueStringBuilder Insert( int index, char value, int count )
    {
        __chars.Insert(index, value, count);
        return this;
    }
    public ValueStringBuilder Insert( int index, params ReadOnlySpan<char> value )
    {
        __chars.Insert(index, value);
        return this;
    }


    public ValueStringBuilder Append( char c )
    {
        __chars.Add(c);
        return this;
    }
    public ValueStringBuilder Append( IEnumerable<string> values )
    {
        foreach ( string value in values ) { __chars.Add(value); }

        return this;
    }
    public ValueStringBuilder Append( params ReadOnlySpan<string> values )
    {
        foreach ( string value in values ) { __chars.Add(value); }

        return this;
    }
    public ValueStringBuilder Append( char c, int count )
    {
        for ( int i = 0; i < count; i++ ) { __chars.Add(c); }

        return this;
    }
    public ValueStringBuilder Append( ReadOnlySpan<char> value )
    {
        __chars.Add(value);
        return this;
    }


    public ValueStringBuilder AppendFormat<TValue>( ReadOnlySpan<char> format, TValue arg0, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        AppendFormatHelper(provider, format, arg0);
        return this;
    }
    public ValueStringBuilder AppendFormat<TValue>( ReadOnlySpan<char> format, TValue arg0, TValue arg1, IFormatProvider? provider = null )
        where TValue : ISpanFormattable

    {
        AppendFormatHelper(provider, format, arg0, arg1);
        return this;
    }
    public ValueStringBuilder AppendFormat<TValue>( ReadOnlySpan<char> format, TValue arg0, TValue arg1, TValue arg2, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        AppendFormatHelper(provider, format, arg0, arg1, arg2);
        return this;
    }
    public ValueStringBuilder AppendFormat<TValue>( ReadOnlySpan<char> format, IFormatProvider? provider, params ReadOnlySpan<TValue> args )
        where TValue : ISpanFormattable
    {
        if ( args.IsEmpty )
        {
            // To preserve the original exception behavior, throw an exception about format if both args and format are null. The actual null check for format is  AppendFormatHelper.
            string paramName = format.IsEmpty
                                   ? nameof(format)
                                   : nameof(args);

            throw new ArgumentNullException(paramName);
        }

        AppendFormatHelper(provider, format, args);
        return this;
    }


    [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")]
    public ValueStringBuilder AppendJoin( char separator, IEnumerable<string> enumerable )
    {
        ReadOnlySpan<string> span = enumerable.GetInternalArray();
        return AppendJoin(separator, span);
    }
    [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")]
    public ValueStringBuilder AppendJoin( ReadOnlySpan<char> separator, IEnumerable<string> enumerable )
    {
        ReadOnlySpan<string> span = enumerable.GetInternalArray();
        return AppendJoin(separator, span);
    }


    public ValueStringBuilder AppendJoin( char separator, params ReadOnlySpan<string> span )
    {
        EnsureCapacity(span.Sum(static x => x.Length) + span.Length * 2 + 1);
        ReadOnlySpan<string>.Enumerator enumerator     = span.GetEnumerator();
        bool                            shouldContinue = enumerator.MoveNext();

        while ( shouldContinue )
        {
            ReadOnlySpan<char> current = enumerator.Current;
            __chars.Add(current);
            shouldContinue = enumerator.MoveNext();

            if ( shouldContinue ) { __chars.Add(separator); }
        }

        return this;
    }
    public ValueStringBuilder AppendJoin( ReadOnlySpan<char> separator, params ReadOnlySpan<string> span )
    {
        EnsureCapacity(span.Sum(static x => x.Length) + separator.Length * span.Length + 1);
        ReadOnlySpan<string>.Enumerator enumerator     = span.GetEnumerator();
        bool                            shouldContinue = enumerator.MoveNext();

        while ( shouldContinue )
        {
            ReadOnlySpan<char> current = enumerator.Current;
            __chars.Add(current);
            shouldContinue = enumerator.MoveNext();

            if ( shouldContinue ) { __chars.Add(separator); }
        }

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)] public ValueStringBuilder AppendJoin<TValue>( char separator, ReadOnlySpan<TValue> enumerable, ReadOnlySpan<char> format = default, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        ReadOnlySpan<TValue>.Enumerator enumerator     = enumerable.GetEnumerator();
        bool                            shouldContinue = enumerator.MoveNext();

        while ( shouldContinue )
        {
            if ( !enumerator.Current.TryFormat(Next, out int charsWritten, format, provider) ) { continue; }

            __chars.Length += charsWritten;
            shouldContinue =  enumerator.MoveNext();

            if ( shouldContinue ) { __chars.Add(separator); }
        }

        return this;
    }
    [MethodImpl(MethodImplOptions.AggressiveOptimization)] public ValueStringBuilder AppendJoin<TValue>( ReadOnlySpan<char> separator, ReadOnlySpan<TValue> enumerable, ReadOnlySpan<char> format = default, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        ReadOnlySpan<TValue>.Enumerator enumerator     = enumerable.GetEnumerator();
        bool                            shouldContinue = enumerator.MoveNext();

        while ( shouldContinue )
        {
            if ( !enumerator.Current.TryFormat(__chars.Next, out int charsWritten, format, provider) ) { continue; }

            __chars.Length += charsWritten;
            shouldContinue =  enumerator.MoveNext();
            if ( shouldContinue ) { __chars.Add(separator); }
        }

        return this;
    }
    [MethodImpl(MethodImplOptions.AggressiveOptimization)] public ValueStringBuilder AppendJoin<TValue>( char separator, IEnumerable<TValue> enumerable, ReadOnlySpan<char> format = default, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        using IEnumerator<TValue> enumerator     = enumerable.GetEnumerator();
        bool                      shouldContinue = enumerator.MoveNext();

        while ( shouldContinue )
        {
            if ( enumerator.Current is not null ) { AppendSpanFormattable(enumerator.Current, format, provider); }

            shouldContinue = enumerator.MoveNext();
            if ( shouldContinue ) { __chars.Add(separator); }
        }

        return this;
    }
    [MethodImpl(MethodImplOptions.AggressiveOptimization)] public ValueStringBuilder AppendJoin<TValue>( ReadOnlySpan<char> separator, IEnumerable<TValue> enumerable, ReadOnlySpan<char> format = default, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        using IEnumerator<TValue> enumerator     = enumerable.GetEnumerator();
        bool                      shouldContinue = enumerator.MoveNext();

        while ( shouldContinue )
        {
            if ( enumerator.Current is not null ) { AppendSpanFormattable(enumerator.Current, format, provider); }

            shouldContinue = enumerator.MoveNext();
            if ( shouldContinue ) { __chars.Add(separator); }
        }

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)] public ValueStringBuilder AppendSpanFormattable<TValue>( TValue value, ReadOnlySpan<char> format, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        EnsureCapacity<TValue>(in format);
        if ( value.TryFormat(Next, out int charsWritten, format, provider) ) { __chars.Length += charsWritten; }

        Debug.Assert(charsWritten > 0, $"No values added to {nameof(__chars)}");
        return this;
    }


    /// <summary> Copied from StringBuilder, can't be done via generic extension as ValueStringBuilder is a ref struct and cannot be used a generic. </summary>
    /// <param name="provider"> </param>
    /// <param name="formatSpan"> </param>
    /// <param name="args"> </param>
    /// <returns> </returns>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FormatException"> </exception>
    internal void AppendFormatHelper<TValue>( IFormatProvider? provider, ReadOnlySpan<char> formatSpan, params ReadOnlySpan<TValue> args )
        where TValue : ISpanFormattable
    {
        // Undocumented exclusive limits on the range for Argument Hole Count and Argument Hole Alignment.
        const int INDEX_LIMIT = 1000000; // Note:            0 <= ArgIndex < IndexLimit
        const int WIDTH_LIMIT = 1000000; // Note:  -WidthLimit <  ArgAlign < WidthLimit

        if ( formatSpan.IsEmpty ) { throw new ArgumentNullException(nameof(formatSpan)); }

        EnsureCapacity<TValue>(in formatSpan);
        int               pos             = 0;
        char              ch              = '\0';
        ICustomFormatter? customFormatter = provider?.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter;

        while ( true )
        {
            while ( pos < formatSpan.Length )
            {
                ch = formatSpan[pos++];

                // Is it a closing brace?
                if ( ch == '}' )
                {
                    // Check next character (if there is one) to see if it is escaped. eg }}
                    if ( pos < formatSpan.Length && formatSpan[pos] == '}' ) { pos++; }
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
                    if ( pos < formatSpan.Length && formatSpan[pos] == '{' ) { pos++; }
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
            // Argument Hole ::= { Count (, WS* Alignment WS*)? (: Formatting)? }
            //
            if ( pos == formatSpan.Length ) { break; }

            //
            //  Start of parsing required Count parameter.
            //  Count ::= ('0'-'9')+ WS*
            //
            pos++;

            // If reached end of text then error (Unexpected end of text)
            // or character is not a digit then error (Unexpected Character)
            if ( pos == formatSpan.Length || ( ch = formatSpan[pos] ) < '0' || ch > '9' ) { ThrowFormatError(); }

            int index = 0;

            do
            {
                index = index * 10 + ch - '0';
                if ( ++pos == formatSpan.Length ) { ThrowFormatError(); } // If reached end of text then error (Unexpected end of text)

                ch = formatSpan[pos]; // so long as character is digit and value of the index is less than 1000000 ( index limit )
            }
            while ( ch is >= '0' and <= '9' && index < INDEX_LIMIT );

            // If value of index is not within the range of the arguments passed  then error (Count out of range)
            if ( index >= args.Length ) { throw new FormatException("Format Count Out Of Range"); }

            // Consume optional whitespace.
            while ( pos < formatSpan.Length && ( ch = formatSpan[pos] ) == ' ' ) { pos++; }

            // End of parsing index parameter.

            //
            //  Start of parsing of optional Alignment
            //  Alignment ::= comma WS* minus? ('0'-'9')+ WS*
            //
            bool leftJustify = false;
            int  width       = 0;

            // Is the character a comma, which indicates the start of alignment parameter.
            if ( ch == ',' )
            {
                pos++;

                // Consume Optional whitespace
                while ( pos < formatSpan.Length && formatSpan[pos] == ' ' ) { pos++; }

                // If reached the end of the text then error (Unexpected end of text)
                if ( pos == formatSpan.Length ) { ThrowFormatError(); }

                // Is there a minus sign?
                ch = formatSpan[pos];

                if ( ch == '-' )
                {
                    // Yes, then alignment is left justified.
                    leftJustify = true;
                    pos++;

                    // If reached end of text then error (Unexpected end of text)
                    if ( pos == formatSpan.Length ) { ThrowFormatError(); }

                    ch = formatSpan[pos];
                }

                // If current character is not a digit then error (Unexpected character)
                if ( ch < '0' || ch > '9' ) { ThrowFormatError(); }

                // Parse alignment digits.
                do
                {
                    width = width * 10 + ch - '0';
                    pos++;

                    // If reached end of text then error. (Unexpected end of text)
                    if ( pos == formatSpan.Length ) { ThrowFormatError(); }

                    ch = formatSpan[pos];

                    // So long a current character is a digit and the value of width is less than 100000 ( width limit )
                }
                while ( ch is >= '0' and <= '9' && width < WIDTH_LIMIT );

                // end of parsing Argument Alignment
            }

            // Consume optional whitespace
            while ( pos < formatSpan.Length && ( ch = formatSpan[pos] ) == ' ' ) { pos++; }

            //
            // Start of parsing of optional formatting parameter.
            //
            TValue arg = args[index];

            ReadOnlySpan<char> itemFormatSpan = default; // used if itemFormat is null

            // Is current character a colon? which indicates start of formatting parameter.
            if ( ch == ':' )
            {
                pos++;
                int startPos = pos;

                while ( true )
                {
                    // If reached end of text then error. (Unexpected end of text)
                    if ( pos == formatSpan.Length ) { ThrowFormatError(); }

                    ch = formatSpan[pos];

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

                if ( pos > startPos ) { itemFormatSpan = formatSpan.Slice(startPos, pos - startPos); }
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

            if ( customFormatter is not null )
            {
                if ( itemFormatSpan.Length != 0 ) { itemFormat = new string(itemFormatSpan); }

                s = customFormatter.Format(itemFormat, arg, provider);
            }

            if ( s is not null )
            {
                // If arg is ISpanFormattable and the beginning doesn't need padding, try formatting it into the remaining current chunk.
                if ( ( leftJustify || width == 0 ) && arg.TryFormat(Next, out int charsWritten, itemFormatSpan, provider) )
                {
                    __chars.Length += charsWritten;

                    // Pad the end, if needed.
                    int padding = width - charsWritten;
                    if ( leftJustify && padding > 0 ) { Append(' ', padding); }

                    // Continue to parse other characters.
                    continue;
                }

                if ( itemFormatSpan.Length != 0 ) { itemFormat ??= new string(itemFormatSpan); }

                s = arg.ToString(itemFormat, provider);
            }

            // Append it to the final output of the Format String.
            s ??= EMPTY;
            int pad = width - s.Length;
            if ( !leftJustify && pad > 0 ) { Append(' ', pad); }

            Append(s);
            if ( leftJustify && pad > 0 ) { Append(' ', pad); }

            // Continue to parse other characters.
        }
    }


    [DoesNotReturn] private static void ThrowFormatError() => throw new FormatException("Invalid Format String");


    public string ToString( string? format, IFormatProvider? formatProvider ) => Values.ToString();
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        ReadOnlySpan<char> values = Values;

        if ( values.TryCopyTo(destination) )
        {
            charsWritten = values.Length;
            return true;
        }

        charsWritten = 0;
        return false;
    }
    public override string ToString()
    {
        string result = Span.ToString();

        Dispose();
        return result;
    }
}



/*
public ref struct ValueStringBuilder
{
    private char[]?    _arrayToReturnToPool;
    private Span<char> _chars;
    private int        _pos;

    public ValueStringBuilder( Span<char> initialBuffer )
    {
        _arrayToReturnToPool = null;
        _chars               = initialBuffer;
        _pos                 = 0;
    }

    public ValueStringBuilder( int initialCapacity )
    {
        _arrayToReturnToPool = ArrayPool<char>.Shared.Rent( initialCapacity );
        _chars               = _arrayToReturnToPool;
        _pos                 = 0;
    }

    public int Length
    {
        get => _pos;
        set
        {
            Debug.Assert( value >= 0 );
            Debug.Assert( value <= _chars.Length );
            _pos = value;
        }
    }

    public int Capacity => _chars.Length;

    public void EnsureCapacity( int capacity )
    {
        // This is not expected to be called this with negative capacity
        Debug.Assert( capacity >= 0 );

        // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
        if ( (uint)capacity > (uint)_chars.Length ) Grow( capacity - _pos );
    }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// Does not ensure there is a null char after <see cref="Length"/>
    /// This overload is pattern matched in the C# 7.3+ compiler so you can omit
    /// the explicit method call, and write eg "fixed (char* c = builder)"
    /// </summary>
    public ref char GetPinnableReference() { return ref MemoryMarshal.GetReference( _chars ); }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
    public ref char GetPinnableReference( bool terminate )
    {
        if ( terminate )
        {
            EnsureCapacity( Length + 1 );
            _chars[Length] = '\0';
        }

        return ref MemoryMarshal.GetReference( _chars );
    }

    public ref char this[ int index ]
    {
        get
        {
            Debug.Assert( index < _pos );
            return ref _chars[index];
        }
    }

    public override string ToString()
    {
        string s = _chars.Slice( 0, _pos ).ToString();
        Dispose();
        return s;
    }

    /// <summary>Returns the underlying storage of the builder.</summary>
    public Span<char> RawChars => _chars;

    /// <summary>
    /// Returns a span around the contents of the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
    public ReadOnlySpan<char> AsSpan( bool terminate )
    {
        if ( terminate )
        {
            EnsureCapacity( Length + 1 );
            _chars[Length] = '\0';
        }

        return _chars.Slice( 0, _pos );
    }

    public ReadOnlySpan<char> AsSpan()                        => _chars.Slice( 0,     _pos );
    public ReadOnlySpan<char> AsSpan( int start )             => _chars.Slice( start, _pos - start );
    public ReadOnlySpan<char> AsSpan( int start, int length ) => _chars.Slice( start, length );

    public bool TryCopyTo( Span<char> destination, out int charsWritten )
    {
        if ( _chars.Slice( 0, _pos ).TryCopyTo( destination ) )
        {
            charsWritten = _pos;
            Dispose();
            return true;
        }
        else
        {
            charsWritten = 0;
            Dispose();
            return false;
        }
    }

    public void Insert( int index, char value, int count )
    {
        if ( _pos > _chars.Length - count ) { Grow( count ); }

        int remaining = _pos - index;
        _chars.Slice( index, remaining ).CopyTo( _chars.Slice( index + count ) );
        _chars.Slice( index, count ).Fill( value );
        _pos += count;
    }

    public void Insert( int index, string? s )
    {
        if ( s is not null ) { return; }

        int count = s.Length;

        if ( _pos > (_chars.Length - count) ) { Grow( count ); }

        int remaining = _pos - index;
        _chars.Slice( index, remaining ).CopyTo( _chars.Slice( index + count ) );

        s
        #if !NET
                .AsSpan()
        #endif
           .CopyTo( _chars.Slice( index ) );

        _pos += count;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public void Append( char c )
    {
        int        pos   = _pos;
        Span<char> chars = _chars;

        if ( (uint)pos < (uint)chars.Length )
        {
            chars[pos] = c;
            _pos       = pos + 1;
        }
        else { GrowAndAppend( c ); }
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public void Append( string? s )
    {
        if ( s is not null ) { return; }

        int pos = _pos;

        if ( s.Length == 1 && (uint)pos < (uint)_chars.Length ) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
        {
            _chars[pos] = s[0];
            _pos        = pos + 1;
        }
        else { AppendSlow( s ); }
    }

    private void AppendSlow( string s )
    {
        int pos = _pos;
        if ( pos > _chars.Length - s.Length ) { Grow( s.Length ); }

        s
        #if !NET
                .AsSpan()
        #endif
           .CopyTo( _chars.Slice( pos ) );

        _pos += s.Length;
    }

    public void Append( char c, int count )
    {
        if ( _pos > _chars.Length - count ) { Grow( count ); }

        Span<char> dst = _chars.Slice( _pos, count );
        for ( int i = 0; i < dst.Length; i++ ) { dst[i] = c; }

        _pos += count;
    }

    public unsafe void Append( char* value, int length )
    {
        int pos = _pos;
        if ( pos > _chars.Length - length ) { Grow( length ); }

        Span<char> dst = _chars.Slice( _pos, length );
        for ( int i = 0; i < dst.Length; i++ ) { dst[i] = *value++; }

        _pos += length;
    }

    public void Append( scoped ReadOnlySpan<char> value )
    {
        int pos = _pos;
        if ( pos > _chars.Length - value.Length ) { Grow( value.Length ); }

        value.CopyTo( _chars.Slice( _pos ) );
        _pos += value.Length;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public Span<char> AppendSpan( int length )
    {
        int origPos = _pos;
        if ( origPos > _chars.Length - length ) { Grow( length ); }

        _pos = origPos + length;
        return _chars.Slice( origPos, length );
    }

    [MethodImpl( MethodImplOptions.NoInlining )]
    private void GrowAndAppend( char c )
    {
        Grow( 1 );
        Append( c );
    }

    /// <summary>
    /// Resize the internal buffer either by doubling current buffer size or
    /// by adding <paramref name="additionalCapacityBeyondPos"/> to
    /// <see cref="_pos"/> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    /// Number of chars requested beyond current position.
    /// </param>
    [MethodImpl( MethodImplOptions.NoInlining )]
    private void Grow( int additionalCapacityBeyondPos )
    {
        Debug.Assert( additionalCapacityBeyondPos > 0 );
        Debug.Assert( _pos                        > _chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed." );

        const uint ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

        // Increase to at least the required size (_pos + additionalCapacityBeyondPos), but try
        // to double the size if possible, bounding the doubling to not go beyond the max array length.
        int newCapacity = (int)Math.Max( (uint)(_pos + additionalCapacityBeyondPos), Math.Min( (uint)_chars.Length * 2, ArrayMaxLength ) );

        // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative.
        // This could also go negative if the actual required length wraps around.
        char[] poolArray = ArrayPool<char>.Shared.Rent( newCapacity );

        _chars.Slice( 0, _pos ).CopyTo( poolArray );

        char[]? toReturn              = _arrayToReturnToPool;
        _chars = _arrayToReturnToPool = poolArray;
        if ( toReturn != null ) { ArrayPool<char>.Shared.Return( toReturn ); }
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public void Dispose()
    {
        char[]? toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if ( toReturn != null ) { ArrayPool<char>.Shared.Return( toReturn ); }
    }
}
*/
