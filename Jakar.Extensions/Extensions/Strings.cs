namespace Jakar.Extensions;


public static class Strings
{
    private static readonly char[] __ends = ['\n', '\r'];
    public static readonly ImmutableDictionary<char, char> BracketPairs = new Dictionary<char, char>
                                                                          {
                                                                              { '(', ')' },
                                                                              { '{', '}' },
                                                                              { '[', ']' }
                                                                          }.ToImmutableDictionary();



    extension( string source )
    {
        public bool ContainsAbout( string search ) => source.Contains(search, StringComparison.OrdinalIgnoreCase);
        public bool ContainsExact( string search ) => source.Contains(search, StringComparison.Ordinal);
        /// <summary>
        ///     <seealso href="https://www.codeproject.com/Tips/1175562/Check-for-Balanced-Parenthesis-in-a-String"/>
        ///     <para>
        ///         <paramref name="bracketPairs"/> defaults to matching: <br/>
        ///         <list type="bullet">
        ///             <item>
        ///                 <term> ( ) </term> <description> Parenthesis </description>
        ///             </item>
        ///             <item>
        ///                 <term> [ ] </term> <description> Square Brackets </description>
        ///             </item>
        ///             <item>
        ///                 <term> { } </term> <description> Curly Braces </description>
        ///             </item>
        ///         </list>
        ///     </para>
        ///     <para> Provide your own <c> IDictionary{char, char} </c> to <paramref name="bracketPairs"/> to customize the mapping. </para>
        /// </summary>
        /// <returns> <see langword="true"/> if balanced; otherwise <see langword="false"/> </returns>
        public bool IsBalanced( IReadOnlyDictionary<char, char>? bracketPairs = null ) => source.AsSpan()
                                                                                                .IsBalanced(bracketPairs); // TODO: ReadOnlySpan<char>
    }



    /// <summary>
    ///     <seealso href="https://www.codeproject.com/Tips/1175562/Check-for-Balanced-Parenthesis-in-a-String"/>
    ///     <para>
    ///         <paramref name="bracketPairs"/> defaults to matching: <br/>
    ///         <list type="bullet">
    ///             <item>
    ///                 <term> ( ) </term> <description> Parenthesis </description>
    ///             </item>
    ///             <item>
    ///                 <term> [ ] </term> <description> Square Brackets </description>
    ///             </item>
    ///             <item>
    ///                 <term> { } </term> <description> Curly Braces </description>
    ///             </item>
    ///         </list>
    ///     </para>
    ///     <para> Provide your own <c> IDictionary{char, char} </c> to <paramref name="bracketPairs"/> to customize the mapping. </para>
    /// </summary>
    /// <returns> <see langword="true"/> if balanced; otherwise <see langword="false"/> </returns>
    public static bool IsBalanced( this ReadOnlySpan<char> input, IReadOnlyDictionary<char, char>? bracketPairs = null ) // TODO: ReadOnlySpan<char>
    {
        bracketPairs ??= BracketPairs;
        using IMemoryOwner<char> buffer = MemoryPool<char>.Shared.Rent(bracketPairs.Count);
        foreach ( ( int i, char item ) in bracketPairs.Values.Enumerate(0) ) { buffer.Memory.Span[i] = item; }

        ReadOnlySpan<char> values   = buffer.Memory.Span[..bracketPairs.Count];
        Stack<char>        brackets = new(input.Length);

        try
        {
            // Iterate through each character in the input string
            foreach ( char c in input )
            {
                // Check if the character is one of the 'opening' brackets. If yes, push to stack
                if ( bracketPairs.ContainsKey(c) ) { brackets.Push(c); }
                else if ( values.Contains(c) ) // check if the character is one of the 'closing' brackets
                {
                    // Check if the closing bracket matches the 'latest' 'opening' bracket 
                    if ( c == bracketPairs[brackets.Peek()] ) { brackets.Pop(); }
                    else { return false; } // if not, it's an unbalanced string
                }

                // Continue looking
            }
        }
        catch ( Exception e )
        {
            // An exception will be caught in case a closing bracket is found, before any opening bracket. that implies, the string is not balanced. Return false
            Console.WriteLine(e);
            return false;
        }

        // Ensure all brackets are closed
        return brackets.Count == 0;
    }



    extension( string value )
    {
        public                       byte[]       ToByteArray( Encoding? encoding = null ) => ( encoding ?? Encoding.Default ).GetBytes(value);
        [MustDisposeResource] public Buffer<byte> AsSpanBytes( Encoding  encoding )        => AsSpanBytes(value.AsSpan(), encoding);
    }



    [MustDisposeResource] public static Buffer<byte> AsSpanBytes( this ReadOnlySpan<char> value, Encoding encoding )
    {
        Buffer<byte> span = new(encoding.GetByteCount(value));
        encoding.GetBytes(value, span.Span);
        return span;
    }



    extension( string value )
    {
        public string[] SplitAndTrimLines( char separator = '\n' )
        {
            string[] array = value.Split(separator);

            for ( int i = 0; i < array.Length; i++ )
            {
                array[i] = array[i]
                   .Trim();
            }

            return array;
        }

        public string[] SplitAndTrimLines( string separator )
        {
            string[] array = value.Split(separator);

            for ( int i = 0; i < array.Length; i++ )
            {
                array[i] = array[i]
                   .Trim();
            }

            return array;
        }

        public string[] SplitLines( char separator = '\n' ) => value.Split(separator);

        public string[] SplitLines( string separator ) => value.Split(separator);

        public Memory<byte> ToMemory( Encoding? encoding = null ) => value.ToByteArray(encoding ?? Encoding.Default)
                                                                          .AsMemory();

        public object ConvertTo( Type target ) => Convert.ChangeType(value, target);

        public ReadOnlyMemory<byte> ToReadOnlyMemory( Encoding? encoding = null ) => value.ToMemory(encoding ?? Encoding.Default);
    }



    public static SecureString ToSecureString( this ReadOnlySpan<byte> value, bool makeReadonly = true ) => Convert.ToBase64String(value)
                                                                                                                   .AsSpan()
                                                                                                                   .ToSecureString(makeReadonly);

    public static SecureString ToSecureString( this string value, bool makeReadonly = true ) => value.AsSpan()
                                                                                                     .ToSecureString(makeReadonly);
    public static SecureString ToSecureString( this Memory<char>         value, bool makeReadonly = true ) => value.Span.ToSecureString(makeReadonly);
    public static SecureString ToSecureString( this ReadOnlyMemory<char> value, bool makeReadonly = true ) => value.Span.ToSecureString(makeReadonly);
    public static SecureString ToSecureString( this Span<char>           value, bool makeReadonly = true ) => ( (ReadOnlySpan<char>)value ).ToSecureString(makeReadonly);

    public static unsafe SecureString ToSecureString( this ReadOnlySpan<char> value, bool makeReadonly = true )
    {
        fixed ( char* token = &value.GetPinnableReference() )
        {
            SecureString secure = new(token, value.Length);
            if ( makeReadonly ) { secure.MakeReadOnly(); }

            return secure;
        }
    }
    public static string GetValue( [NotNullIfNotNull(nameof(secure))] this SecureString? secure )
    {
        if ( secure is null || secure.Length == 0 ) { return EMPTY; }

        IntPtr  ptr = IntPtr.Zero;
        string result;

        try
        {
            ptr    = Marshal.SecureStringToGlobalAllocUnicode(secure);
            result = Marshal.PtrToStringUni(ptr)!;
        }
        finally
        {
            if ( ptr != IntPtr.Zero ) { Marshal.ZeroFreeGlobalAllocUnicode(ptr); }
        }

        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        return result ?? EMPTY;
    }



    /// <param name="str"> </param>
    extension( string str )
    {
        /// <summary>
        ///     <para>
        ///         <see href="https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm"/>
        ///     </para>
        /// </summary>
        /// <param name="separator"> the <see cref="char"/> to split on </param>
        public SpanSplitEnumerator<char> SplitOn( char separator ) => str.AsSpan()
                                                                         .SplitOn(separator);

        /// <summary>
        ///     <para>
        ///         <see href="https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm"/>
        ///     </para>
        ///     Default chars <see cref="char"/> to '\n' and '\r'
        /// </summary>
        public SpanSplitEnumerator<char> SplitOn() => str.AsSpan()
                                                         .SplitOn();
    }



    /// <summary>
    ///     <para>
    ///         <see href="https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm"/>
    ///     </para>
    ///     Default chars <see cref="char"/> to '\n' and '\r'
    /// </summary>
    /// <param name="span"> </param>
    public static SpanSplitEnumerator<char> SplitOn( this ReadOnlySpan<char> span ) => new(span, __ends);



    extension<TValue>( Span<TValue> span )
        where TValue : unmanaged, IEquatable<TValue>
    {
        public SpanSplitEnumerator<TValue> SplitOn( TValue separator ) => new(span, separator);

        public SpanSplitEnumerator<TValue> SplitOn( params TValue[] separators ) => new(span, separators);
    }



    extension<TValue>( ReadOnlySpan<TValue> span )
        where TValue : unmanaged, IEquatable<TValue>
    {
        public SpanSplitEnumerator<TValue> SplitOn( TValue separator ) => new(span, separator);

        public SpanSplitEnumerator<TValue> SplitOn( params TValue[] separators ) => new(span, separators);
    }



    public static string ConvertToString( this byte[]               value, Encoding encoding ) => encoding.GetString(value);
    public static string ConvertToString( this Span<byte>           value, Encoding encoding ) => encoding.GetString(value);
    public static string ConvertToString( this ReadOnlySpan<byte>   value, Encoding encoding ) => encoding.GetString(value);
    public static string ConvertToString( this Memory<byte>         value, Encoding encoding ) => value.Span.ConvertToString(encoding);
    public static string ConvertToString( this ReadOnlyMemory<byte> value, Encoding encoding ) => value.Span.ConvertToString(encoding);



    extension( string self )
    {
        public string RemoveAll( string old ) => self.Replace(old,           EMPTY, StringComparison.Ordinal);
        public string RemoveAll( char   old ) => self.Replace(old.Repeat(1), EMPTY);
    }



    /// <summary>
    ///     <seealso href="https://stackoverflow.com/a/48832421/9530917"/>
    /// </summary>
    /// <param name="c"> </param>
    /// <param name="count"> </param>
    /// <returns>
    ///     <see cref="string"/>
    /// </returns>
    public static string Repeat( this char c, int count ) => new(c, count);



    extension( string self )
    {
        /// <summary>
        ///     <seealso href="https://stackoverflow.com/a/720915/9530917"/>
        /// </summary>
        /// <param name="count"> </param>
        /// <returns>
        ///     <see cref="string"/>
        /// </returns>
        public string Repeat( int count ) => new StringBuilder(self.Length * count).Insert(0, self, count)
                                                                                   .ToString();
        public string ReplaceAll( string old, string newString ) => self.Replace(old, newString, StringComparison.Ordinal);
        public string ReplaceAll( char   old, char   newString ) => self.Replace(old, newString);
        public string ToScreamingCase() => self.ToSnakeCase()
                                               .ToUpper()
                                               .Replace("__", "_");
        /// <summary> inspired from <seealso href="https://stackoverflow.com/a/67332992/9530917"/> </summary>
        public string ToSnakeCase() => self.ToSnakeCase(CultureInfo.InvariantCulture);
        /// <summary> inspired from <seealso href="https://stackoverflow.com/a/67332992/9530917"/> </summary>
        public string ToSnakeCase( CultureInfo cultureInfo ) => ToSnakeCase(self.AsSpan(), cultureInfo);
    }



    public static string ToSnakeCase( this scoped in ReadOnlySpan<char> span, CultureInfo cultureInfo )
    {
        if ( span.IsNullOrWhiteSpace() ) { return EMPTY; }

        StringBuilder    builder          = new(span.Length + span.Count(' '));
        UnicodeCategory? previousCategory = null;

        for ( int currentIndex = 0; currentIndex < span.Length; currentIndex++ )
        {
            char currentChar = span[currentIndex];

            switch ( currentChar )
            {
                case '_':
                    builder.Append('_');
                    previousCategory = null;
                    continue;

                case '.':
                    builder.Append('.');
                    previousCategory = null;
                    continue;
            }

            UnicodeCategory currentCategory = char.GetUnicodeCategory(currentChar);

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch ( currentCategory )
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                    if ( isSpaceOrLower(previousCategory) || isNextLower(previousCategory, currentIndex, in span) ) { builder.Append('_'); }

                    currentChar = char.ToLower(currentChar, cultureInfo);
                    break;

                case UnicodeCategory.LowercaseLetter:
                    if ( previousCategory is UnicodeCategory.SpaceSeparator ) { builder.Append('_'); }

                    break;

                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.LetterNumber:
                case UnicodeCategory.OtherNumber:
                    break;

                default:
                    if ( previousCategory is not null ) { previousCategory = UnicodeCategory.SpaceSeparator; }

                    continue;
            }

            builder.Append(currentChar);
            previousCategory = currentCategory;
        }

        return builder.ToString();

        static bool isSpaceOrLower( in UnicodeCategory? category ) => category is UnicodeCategory.SpaceSeparator or UnicodeCategory.LowercaseLetter;

        static bool isNextLower( in UnicodeCategory? category, in int index, scoped ref readonly ReadOnlySpan<char> span ) => category.HasValue && category is not UnicodeCategory.DecimalDigitNumber && index > 0 && index + 1 < span.Length && char.IsLower(span[index + 1]);
    }



    /// <param name="self"> </param>
    extension( string self )
    {
        /// <summary> Wraps a string in <paramref name="c"> </paramref> repeated <paramref name="padding"> </paramref> times. </summary>
        /// <param name="c"> </param>
        /// <param name="padding"> </param>
        /// <returns> </returns>
        public string Wrapper( char c, int padding ) => self.PadLeft(padding, c)
                                                            .PadRight(padding, c);
        public TResult ConvertTo<TResult>()
            where TResult : IConvertible => (TResult)self.ConvertTo(typeof(TResult));
    }
}
