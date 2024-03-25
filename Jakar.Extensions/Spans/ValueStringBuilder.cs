// Jakar.Extensions :: Jakar.Extensions
// 06/07/2022  3:25 PM


using System;



namespace Jakar.Extensions;


/// <summary>
///     <para> Based on System.Text.ValueStringBuilder </para>
/// </summary>
public ref struct ValueStringBuilder
{
    private Buffer<char> _chars;


    public readonly bool       IsEmpty  { [MethodImpl(            MethodImplOptions.AggressiveInlining )] get => _chars.Length == 0; }
    public readonly int        Capacity { [MethodImpl(            MethodImplOptions.AggressiveInlining )] get => _chars.Capacity; }
    public readonly Span<char> Next     { [MethodImpl(            MethodImplOptions.AggressiveInlining )] get => _chars.Next; }
    public readonly Span<char> Span     { [MethodImpl(            MethodImplOptions.AggressiveInlining )] get => _chars.Span; }
    public readonly Span<char> this[ Range range ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _chars[range]; }
    public ref char this[ Index            index ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _chars[index]; }
    public ref char this[ int              index ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _chars[index]; }
    public readonly ReadOnlySpan<char> Result { [MethodImpl(      MethodImplOptions.AggressiveInlining )] get => _chars.Span; }
    public          int                Length { [MethodImpl(      MethodImplOptions.AggressiveInlining )] readonly get => _chars.Index; set => _chars.Index = value; }


    public ValueStringBuilder() : this( 64 ) { }
    public ValueStringBuilder( int                          initialCapacity ) => _chars = new Buffer<char>( initialCapacity );
    public ValueStringBuilder( scoped in ReadOnlySpan<char> span ) => _chars = new Buffer<char>( span );


    // public ValueStringBuilder(  ReadOnlySpan<char> span ) : this(span, false) { }
    // public ValueStringBuilder(  ReadOnlySpan<char> span,  bool isReadOnly ) : this(Buffer<char>.Create(span, isReadOnly)) { }
    // public ValueStringBuilder(  Buffer<char>       buffer ) => _chars = buffer;


    public          void                    Dispose()       => _chars.Dispose();
    public readonly Buffer<char>.Enumerator GetEnumerator() => _chars.GetEnumerator();


    public void EnsureCapacity<T>( scoped in ReadOnlySpan<char> format )
    {
        int capacity = Sizes.GetBufferSize<T>();

        if ( typeof(T)      == typeof(DateTime) ) { capacity       = Math.Max( format.Length, capacity ); }
        else if ( typeof(T) == typeof(DateTimeOffset) ) { capacity = Math.Max( format.Length, capacity ); }
        else if ( typeof(T) == typeof(TimeSpan) ) { capacity       = Math.Max( format.Length, capacity ); }

        _chars.EnsureCapacity( capacity );
    }
    public void EnsureCapacity( int capacity ) => _chars.EnsureCapacity( capacity );


    /// <summary> Get a pinnable reference to the builder. Does not ensure there is a null char after <see cref="Length"/> . This overload is pattern matched  the C# 7.3+ compiler so you can omit the explicit method call, and write eg "fixed (char* c = builder)" </summary>
    [Pure]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public readonly ref char GetPinnableReference() => ref _chars.GetPinnableReference();


    /// <summary> Get a pinnable reference to the builder. </summary>
    /// <param name="terminate"> Ensures that the builder has a null char after <see cref="Length"/> </param>
    [Pure]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public ref char GetPinnableReference( bool terminate )
    {
        if ( terminate ) { return ref GetPinnableReference( '\0' ); }

        return ref GetPinnableReference();
    }


    /// <summary> Get a pinnable reference to the builder. </summary>
    /// <param name="terminate"> Ensures that the builder has a null char after <see cref="Length"/> </param>
    [Pure]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public ref char GetPinnableReference( char terminate )
    {
        EnsureCapacity( Length + 1 );
        _chars[++Length] = terminate;

        return ref GetPinnableReference();
    }


    public override string ToString()
    {
        string result = Span.ToString();

        Dispose();
        return result;
    }


    [Pure] public readonly ReadOnlySpan<char> AsSpan()                         => _chars.Span;
    [Pure] public          ReadOnlySpan<char> AsSpan( char terminate )         => _chars.AsSpan( terminate );
    [Pure] public readonly ReadOnlySpan<char> Slice( int   start )             => _chars[start..];
    [Pure] public readonly ReadOnlySpan<char> Slice( int   start, int length ) => _chars.Slice( start, length );
    public ValueStringBuilder Reset()
    {
        _chars.Reset( '\0' );
        return this;
    }


    public bool TryCopyTo( scoped in Span<char> destination, out int charsWritten )
    {
        if ( _chars.TryCopyTo( destination, out charsWritten ) )
        {
            Dispose();
            return true;
        }

        Dispose();
        return false;
    }


    public ValueStringBuilder Trim( char value )
    {
        _chars.Trim( value );
        return this;
    }
    public ValueStringBuilder Trim( scoped in ReadOnlySpan<char> value )
    {
        _chars.Trim( value );
        return this;
    }
    public ValueStringBuilder TrimEnd( char value )
    {
        _chars.TrimEnd( value );
        return this;
    }
    public ValueStringBuilder TrimEnd( scoped in ReadOnlySpan<char> value )
    {
        _chars.TrimEnd( value );
        return this;
    }
    public ValueStringBuilder TrimStart( char value )
    {
        _chars.TrimStart( value );
        return this;
    }
    public ValueStringBuilder TrimStart( scoped in ReadOnlySpan<char> value )
    {
        _chars.TrimStart( value );
        return this;
    }


    public ValueStringBuilder Replace( int index, char value )
    {
        _chars.Replace( index, value );
        return this;
    }
    public ValueStringBuilder Replace( int index, char value, int count )
    {
        _chars.Replace( index, value, count );
        return this;
    }
    public ValueStringBuilder Replace( int index, scoped in ReadOnlySpan<char> value )
    {
        _chars.Replace( index, value );
        return this;
    }


    public ValueStringBuilder Insert( int index, char value )
    {
        _chars.Insert( index, value );
        return this;
    }
    public ValueStringBuilder Insert( int index, char value, int count )
    {
        _chars.Insert( index, value, count );
        return this;
    }
    public ValueStringBuilder Insert( int index, scoped in ReadOnlySpan<char> value )
    {
        _chars.Insert( index, value );
        return this;
    }


    public ValueStringBuilder Append( char c )
    {
        _chars.Append( c );
        return this;
    }
    public ValueStringBuilder Append( IEnumerable<string> values )
    {
        foreach ( string value in values ) { _chars.Append( value ); }

        return this;
    }
    public ValueStringBuilder Append( scoped in ReadOnlySpan<string> values )
    {
        foreach ( string value in values ) { _chars.Append( value ); }

        return this;
    }
    public ValueStringBuilder Append( char c, int count )
    {
        _chars.Append( c, count );
        return this;
    }
    public ValueStringBuilder Append( scoped in ReadOnlySpan<char> value )
    {
        _chars.Append( value );
        return this;
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


    public ValueStringBuilder AppendFormat( scoped in ReadOnlySpan<char> format, object? arg0, IFormatProvider? provider = default )
    {
        AppendFormatHelper( provider, format, new ParamsArray( arg0 ) );
        return this;
    }
    public ValueStringBuilder AppendFormat( scoped in ReadOnlySpan<char> format, object? arg0, object? arg1, IFormatProvider? provider = default )
    {
        AppendFormatHelper( provider, format, new ParamsArray( arg0, arg1 ) );
        return this;
    }
    public ValueStringBuilder AppendFormat( scoped in ReadOnlySpan<char> format, object? arg0, object? arg1, object? arg2, IFormatProvider? provider = default )
    {
        AppendFormatHelper( provider, format, new ParamsArray( arg0, arg1, arg2 ) );
        return this;
    }
    public ValueStringBuilder AppendFormat( scoped in ReadOnlySpan<char> format, IFormatProvider? provider, params object?[] args )
    {
        if ( args is null )
        {
            // To preserve the original exception behavior, throw an exception about format if both args and format are null. The actual null check for format is  AppendFormatHelper.
            string paramName = format.IsEmpty
                                   ? nameof(format)
                                   : nameof(args);

            throw new ArgumentNullException( paramName );
        }

        AppendFormatHelper( provider, format, args );
        return this;
    }
    public ValueStringBuilder AppendFormat( scoped in ReadOnlySpan<char> format, scoped in ReadOnlySpan<object?> args, IFormatProvider? provider = default )
    {
        if ( args.IsEmpty )
        {
            // To preserve the original exception behavior, throw an exception about format if both args and format are null. The actual null check for format is  AppendFormatHelper.
            string paramName = format.IsEmpty
                                   ? nameof(format)
                                   : nameof(args);

            throw new ArgumentNullException( paramName );
        }

        AppendFormatHelper( provider, format, args );
        return this;
    }


    public ValueStringBuilder AppendFormat<T>( scoped in ReadOnlySpan<char> format, T arg0, IFormatProvider? provider = default )
        where T : unmanaged, IEquatable<T>
    {
        AppendFormatHelper( provider, format, ParamsArray<T>.Create( arg0 ) );
        return this;
    }
    public ValueStringBuilder AppendFormat<T>( scoped in ReadOnlySpan<char> format, T arg0, T arg1, IFormatProvider? provider = default )
        where T : unmanaged, IEquatable<T>
    {
        AppendFormatHelper( provider, format, ParamsArray<T>.Create( arg0, arg1 ) );
        return this;
    }
    public ValueStringBuilder AppendFormat<T>( scoped in ReadOnlySpan<char> format, T arg0, T arg1, T arg2, IFormatProvider? provider = default )
        where T : unmanaged, IEquatable<T>
    {
        AppendFormatHelper( provider, format, ParamsArray<T>.Create( arg0, arg1, arg2 ) );
        return this;
    }
    public ValueStringBuilder AppendFormat<T>( scoped in ReadOnlySpan<char> format, IFormatProvider? provider = default, params T[] args )
        where T : unmanaged, IEquatable<T>
    {
        if ( args is null )
        {
            // To preserve the original exception behavior, throw an exception about format if both args and format are null. The actual null check for format is  AppendFormatHelper.
            string paramName = format.IsEmpty
                                   ? nameof(format)
                                   : nameof(args);

            throw new ArgumentNullException( paramName );
        }

        AppendFormatHelper<T>( provider, format, args );
        return this;
    }
    public ValueStringBuilder AppendFormat<T>( scoped in ReadOnlySpan<char> format, scoped in ReadOnlySpan<T> args, IFormatProvider? provider = default )
        where T : unmanaged, IEquatable<T>
    {
        if ( args.IsEmpty )
        {
            // To preserve the original exception behavior, throw an exception about format if both args and format are null. The actual null check for format is  AppendFormatHelper.
            string paramName = format.IsEmpty
                                   ? nameof(format)
                                   : nameof(args);

            throw new ArgumentNullException( paramName );
        }

        AppendFormatHelper<T>( provider, format, args );
        return this;
    }


#if NET7_0_OR_GREATER
    [RequiresDynamicCode( nameof(ArrayExtensions.GetInternalArray) )]
#endif
    public ValueStringBuilder AppendJoin( char separator, IEnumerable<string> enumerable )
    {
        ReadOnlySpan<string> span = enumerable.GetInternalArray();
        return AppendJoin( separator, span );
    }
    public ValueStringBuilder AppendJoin( scoped in ReadOnlySpan<char> separator, IEnumerable<string> enumerable )
    {
        ReadOnlySpan<string> span = enumerable.ToArray();

        return AppendJoin( separator, span );
    }


    public ValueStringBuilder AppendJoin( char separator, scoped in ReadOnlySpan<string> span )
    {
        EnsureCapacity( span.Sum( static x => x.Length ) + span.Length * 2 + 1 );
        ReadOnlySpan<string>.Enumerator enumerator     = span.GetEnumerator();
        bool                            shouldContinue = enumerator.MoveNext();

        while ( shouldContinue )
        {
            string current = enumerator.Current;
            current.AsSpan().CopyTo( Next );
            _chars.Index   += current.Length;
            shouldContinue =  enumerator.MoveNext();

            if ( shouldContinue ) { _chars.Append( separator ); }
        }

        return this;
    }
    public ValueStringBuilder AppendJoin( scoped in ReadOnlySpan<char> separator, scoped in ReadOnlySpan<string> span )
    {
        EnsureCapacity( span.Sum( static x => x.Length ) + separator.Length * span.Length + 1 );
        ReadOnlySpan<string>.Enumerator enumerator     = span.GetEnumerator();
        bool                            shouldContinue = enumerator.MoveNext();

        while ( shouldContinue )
        {
            string current = enumerator.Current;
            current.AsSpan().CopyTo( Next );
            _chars.Index   += current.Length;
            shouldContinue =  enumerator.MoveNext();

            if ( shouldContinue ) { _chars.Append( separator ); }
        }

        return this;
    }


#if NET6_0_OR_GREATER
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
#endif
    public ValueStringBuilder AppendJoin<T>( char separator, scoped in ReadOnlySpan<T> enumerable, scoped in ReadOnlySpan<char> format = default, IFormatProvider? provider = default )
        where T : ISpanFormattable
    {
        ReadOnlySpan<T>.Enumerator enumerator     = enumerable.GetEnumerator();
        bool                       shouldContinue = enumerator.MoveNext();

        while ( shouldContinue )
        {
            if ( !enumerator.Current.TryFormat( Next, out int charsWritten, format, provider ) ) { continue; }

            _chars.Index   += charsWritten;
            shouldContinue =  enumerator.MoveNext();

            if ( shouldContinue ) { _chars.Append( separator ); }
        }

        return this;
    }

#if NET6_0_OR_GREATER
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
#endif
    public ValueStringBuilder AppendJoin<T>( scoped in ReadOnlySpan<char> separator, scoped in ReadOnlySpan<T> enumerable, scoped in ReadOnlySpan<char> format = default, IFormatProvider? provider = default )
        where T : ISpanFormattable
    {
        ReadOnlySpan<T>.Enumerator enumerator     = enumerable.GetEnumerator();
        bool                       shouldContinue = enumerator.MoveNext();

        while ( shouldContinue )
        {
            if ( !enumerator.Current.TryFormat( Next, out int charsWritten, format, provider ) ) { continue; }

            _chars.Index   += charsWritten;
            shouldContinue =  enumerator.MoveNext();
            if ( shouldContinue ) { _chars.Append( separator ); }
        }

        return this;
    }


#if NET6_0_OR_GREATER
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
#endif
    public ValueStringBuilder AppendJoin<T>( char separator, IEnumerable<T> enumerable, scoped in ReadOnlySpan<char> format = default, IFormatProvider? provider = default )
        where T : ISpanFormattable
    {
        using IEnumerator<T> enumerator     = enumerable.GetEnumerator();
        bool                 shouldContinue = enumerator.MoveNext();

        while ( shouldContinue )
        {
            if ( enumerator.Current is not null ) { AppendSpanFormattable( enumerator.Current, format, provider ); }

            shouldContinue = enumerator.MoveNext();
            if ( shouldContinue ) { _chars.Append( separator ); }
        }

        return this;
    }

#if NET6_0_OR_GREATER
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
#endif
    public ValueStringBuilder AppendJoin<T>( scoped in ReadOnlySpan<char> separator, IEnumerable<T> enumerable, scoped in ReadOnlySpan<char> format = default, IFormatProvider? provider = default )
        where T : ISpanFormattable
    {
        using IEnumerator<T> enumerator     = enumerable.GetEnumerator();
        bool                 shouldContinue = enumerator.MoveNext();

        while ( shouldContinue )
        {
            if ( enumerator.Current is not null ) { AppendSpanFormattable( enumerator.Current, format, provider ); }

            shouldContinue = enumerator.MoveNext();
            if ( shouldContinue ) { _chars.Append( separator ); }
        }

        return this;
    }


#if NET6_0_OR_GREATER
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
#endif
    public ValueStringBuilder AppendSpanFormattable<T>( T value, scoped in ReadOnlySpan<char> format, IFormatProvider? provider = default )
        where T : ISpanFormattable
    {
        EnsureCapacity<T>( format );

        if ( value.TryFormat( Next, out int charsWritten, format, provider ) ) { _chars.Index += charsWritten; }

        Debug.Assert( charsWritten > 0, $"No values added to {nameof(_chars)}" );
        return this;
    }


    /// <summary> Copied from StringBuilder, can't be done via generic extension as ValueStringBuilder is a ref struct and cannot be used a generic. </summary>
    /// <param name="provider"> </param>
    /// <param name="format"> </param>
    /// <param name="args"> </param>
    /// <returns> </returns>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FormatException"> </exception>
    internal void AppendFormatHelper( IFormatProvider? provider, scoped in ReadOnlySpan<char> format, scoped in ParamsArray args )
    {
        // Undocumented exclusive limits on the range for Argument Hole Index and Argument Hole Alignment.
        const int INDEX_LIMIT = 1000000; // Note:            0 <= ArgIndex < IndexLimit
        const int WIDTH_LIMIT = 1000000; // Note:  -WidthLimit <  ArgAlign < WidthLimit

        if ( format.IsEmpty ) { throw new ArgumentNullException( nameof(format) ); }

        int  pos             = 0;
        char ch              = '\0';
        var  customFormatter = provider?.GetFormat( typeof(ICustomFormatter) ) as ICustomFormatter;

        while ( true )
        {
            while ( pos < format.Length )
            {
                ch = format[pos++];

                // Is it a closing brace?
                if ( ch == '}' )
                {
                    // Check next character (if there is one) to see if it is escaped. eg }}
                    if ( pos < format.Length && format[pos] == '}' ) { pos++; }
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
                    if ( pos < format.Length && format[pos] == '{' ) { pos++; }
                    else
                    {
                        // Otherwise treat it as the opening brace of an Argument Hole.
                        pos--;
                        break;
                    }
                }

                // If it's neither then treat the character as just text.
                Append( ch );
            }

            //
            // Start of parsing of Argument Hole.
            // Argument Hole ::= { Index (, WS* Alignment WS*)? (: Formatting)? }
            //
            if ( pos == format.Length ) { break; }

            //
            //  Start of parsing required Index parameter.
            //  Index ::= ('0'-'9')+ WS*
            //
            pos++;

            // If reached end of text then error (Unexpected end of text)
            // or character is not a digit then error (Unexpected Character)
            if ( pos == format.Length || (ch = format[pos]) < '0' || ch > '9' ) { ThrowFormatError(); }

            int index = 0;

            do
            {
                index = index * 10 + ch - '0';
                if ( ++pos == format.Length ) { ThrowFormatError(); } // If reached end of text then error (Unexpected end of text)

                ch = format[pos]; // so long as character is digit and value of the index is less than 1000000 ( index limit )
            }
            while ( ch is >= '0' and <= '9' && index < INDEX_LIMIT );

            // If value of index is not within the range of the arguments passed  then error (Index out of range)
            if ( index >= args.Length ) { throw new FormatException( "Format Index Out Of Range" ); }

            // Consume optional whitespace.
            while ( pos < format.Length && (ch = format[pos]) == ' ' ) { pos++; }

            // End of parsing index parameter.

            //
            //  Start of parsing of optional Alignment ::= comma WS* minus? ('0'-'9')+ WS*
            //
            bool leftJustify = false;
            int  width       = 0;

            // Is the character a comma, which indicates the start of alignment parameter.
            if ( ch == ',' )
            {
                pos++;

                // Consume Optional whitespace
                while ( pos < format.Length && format[pos] == ' ' ) { pos++; }

                // If reached the end of the text then error (Unexpected end of text)
                if ( pos == format.Length ) { ThrowFormatError(); }

                // Is there a minus sign?
                ch = format[pos];

                if ( ch == '-' )
                {
                    // Yes, then alignment is left justified.
                    leftJustify = true;
                    pos++;

                    // If reached end of text then error (Unexpected end of text)
                    if ( pos == format.Length ) { ThrowFormatError(); }

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
                    if ( pos == format.Length ) { ThrowFormatError(); }

                    ch = format[pos];

                    // So long a current character is a digit and the value of width is less than 100000 ( width limit )
                }
                while ( ch is >= '0' and <= '9' && width < WIDTH_LIMIT );

                // end of parsing Argument Alignment
            }

            // Consume optional whitespace
            while ( pos < format.Length && (ch = format[pos]) == ' ' ) { pos++; }

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
                    if ( pos == format.Length ) { ThrowFormatError(); }

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

                if ( pos > startPos ) { itemFormatSpan = format.Slice( startPos, pos - startPos ); }
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
                if ( itemFormatSpan.Length != 0 ) { itemFormat = new string( itemFormatSpan ); }

                s = customFormatter.Format( itemFormat, arg, provider );
            }

            if ( s == null )
            {
            #if NET6_0_OR_GREATER

                // If arg is ISpanFormattable and the beginning doesn't need padding, try formatting it into the remaining current chunk.
                if ( arg is ISpanFormattable spanFormattableArg && (leftJustify || width == 0) && spanFormattableArg.TryFormat( Next, out int charsWritten, itemFormatSpan, provider ) )
                {
                    _chars.Index += charsWritten;

                    // Pad the end, if needed.
                    int padding = width - charsWritten;
                    if ( leftJustify && padding > 0 ) { Append( ' ', padding ); }

                    // Continue to parse other characters.
                    continue;
                }
            #endif

                // Otherwise, fallback to trying IFormattable or calling ToString.
                if ( arg is IFormattable formattableArg )
                {
                    if ( itemFormatSpan.Length != 0 ) { itemFormat ??= new string( itemFormatSpan ); }

                    s = formattableArg.ToString( itemFormat, provider );
                }
                else if ( arg is not null ) { s = arg.ToString(); }
            }

            // Append it to the final output of the Format String.
            s ??= string.Empty;

            int pad = width - s.Length;
            if ( !leftJustify && pad > 0 ) { Append( ' ', pad ); }

            Append( s );
            if ( leftJustify && pad > 0 ) { Append( ' ', pad ); }

            // Continue to parse other characters.
        }
    }


    /// <summary> Copied from StringBuilder, can't be done via generic extension as ValueStringBuilder is a ref struct and cannot be used a generic. </summary>
    /// <param name="provider"> </param>
    /// <param name="format"> </param>
    /// <param name="args"> </param>
    /// <returns> </returns>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FormatException"> </exception>
    internal void AppendFormatHelper<T>( IFormatProvider? provider, scoped in ReadOnlySpan<char> format, scoped in ParamsArray<T> args )
        where T : unmanaged, IEquatable<T>
    {
        // Undocumented exclusive limits on the range for Argument Hole Index and Argument Hole Alignment.
        const int INDEX_LIMIT = 1000000; // Note:            0 <= ArgIndex < IndexLimit
        const int WIDTH_LIMIT = 1000000; // Note:  -WidthLimit <  ArgAlign < WidthLimit

        if ( format.IsEmpty ) { throw new ArgumentNullException( nameof(format) ); }

        int  pos             = 0;
        char ch              = '\0';
        var  customFormatter = provider?.GetFormat( typeof(ICustomFormatter) ) as ICustomFormatter;

        while ( true )
        {
            while ( pos < format.Length )
            {
                ch = format[pos++];

                // Is it a closing brace?
                if ( ch == '}' )
                {
                    // Check next character (if there is one) to see if it is escaped. eg }}
                    if ( pos < format.Length && format[pos] == '}' ) { pos++; }
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
                    if ( pos < format.Length && format[pos] == '{' ) { pos++; }
                    else
                    {
                        // Otherwise treat it as the opening brace of an Argument Hole.
                        pos--;
                        break;
                    }
                }

                // If it's neither then treat the character as just text.
                Append( ch );
            }

            //
            // Start of parsing of Argument Hole.
            // Argument Hole ::= { Index (, WS* Alignment WS*)? (: Formatting)? }
            //
            if ( pos == format.Length ) { break; }

            //
            //  Start of parsing required Index parameter.
            //  Index ::= ('0'-'9')+ WS*
            //
            pos++;

            // If reached end of text then error (Unexpected end of text)
            // or character is not a digit then error (Unexpected Character)
            if ( pos == format.Length || (ch = format[pos]) < '0' || ch > '9' ) { ThrowFormatError(); }

            int index = 0;

            do
            {
                index = index * 10 + ch - '0';
                if ( ++pos == format.Length ) { ThrowFormatError(); } // If reached end of text then error (Unexpected end of text)

                ch = format[pos]; // so long as character is digit and value of the index is less than 1000000 ( index limit )
            }
            while ( ch is >= '0' and <= '9' && index < INDEX_LIMIT );

            // If value of index is not within the range of the arguments passed  then error (Index out of range)
            if ( index >= args.Length ) { throw new FormatException( "Format Index Out Of Range" ); }

            // Consume optional whitespace.
            while ( pos < format.Length && (ch = format[pos]) == ' ' ) { pos++; }

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
                while ( pos < format.Length && format[pos] == ' ' ) { pos++; }

                // If reached the end of the text then error (Unexpected end of text)
                if ( pos == format.Length ) { ThrowFormatError(); }

                // Is there a minus sign?
                ch = format[pos];

                if ( ch == '-' )
                {
                    // Yes, then alignment is left justified.
                    leftJustify = true;
                    pos++;

                    // If reached end of text then error (Unexpected end of text)
                    if ( pos == format.Length ) { ThrowFormatError(); }

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
                    if ( pos == format.Length ) { ThrowFormatError(); }

                    ch = format[pos];

                    // So long a current character is a digit and the value of width is less than 100000 ( width limit )
                }
                while ( ch is >= '0' and <= '9' && width < WIDTH_LIMIT );

                // end of parsing Argument Alignment
            }

            // Consume optional whitespace
            while ( pos < format.Length && (ch = format[pos]) == ' ' ) { pos++; }

            //
            // Start of parsing of optional formatting parameter.
            //
            T? arg = args[index];

            ReadOnlySpan<char> itemFormatSpan = default; // used if itemFormat is null

            // Is current character a colon? which indicates start of formatting parameter.
            if ( ch == ':' )
            {
                pos++;
                int startPos = pos;

                while ( true )
                {
                    // If reached end of text then error. (Unexpected end of text)
                    if ( pos == format.Length ) { ThrowFormatError(); }

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

                if ( pos > startPos ) { itemFormatSpan = format.Slice( startPos, pos - startPos ); }
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
                if ( itemFormatSpan.Length != 0 ) { itemFormat = new string( itemFormatSpan ); }

                s = customFormatter.Format( itemFormat, arg, provider );
            }

            if ( s == null )
            {
            #if NET6_0_OR_GREATER

                // If arg is ISpanFormattable and the beginning doesn't need padding, try formatting it into the remaining current chunk.
                if ( arg is ISpanFormattable spanFormattableArg && (leftJustify || width == 0) && spanFormattableArg.TryFormat( Next, out int charsWritten, itemFormatSpan, provider ) )
                {
                    _chars.Index += charsWritten;

                    // Pad the end, if needed.
                    int padding = width - charsWritten;
                    if ( leftJustify && padding > 0 ) { Append( ' ', padding ); }

                    // Continue to parse other characters.
                    continue;
                }
            #endif

                // Otherwise, fallback to trying IFormattable or calling ToString.
                if ( arg is IFormattable formattableArg )
                {
                    if ( itemFormatSpan.Length != 0 ) { itemFormat ??= new string( itemFormatSpan ); }

                    s = formattableArg.ToString( itemFormat, provider );
                }
                else { s = arg.ToString(); }
            }

            // Append it to the final output of the Format String.
            s ??= string.Empty;

            int pad = width - s.Length;
            if ( !leftJustify && pad > 0 ) { Append( ' ', pad ); }

            Append( s );
            if ( leftJustify && pad > 0 ) { Append( ' ', pad ); }

            // Continue to parse other characters.
        }
    }


    [DoesNotReturn] private static void ThrowFormatError() => throw new FormatException( "Invalid Format String" );
}
