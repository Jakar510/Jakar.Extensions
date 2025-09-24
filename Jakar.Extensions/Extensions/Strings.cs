namespace Jakar.Extensions;


public static class Strings
{
    public static readonly ImmutableDictionary<char, char> BracketPairs = new Dictionary<char, char>
                                                                          {
                                                                              { '(', ')' },
                                                                              { '{', '}' },
                                                                              { '[', ']' }
                                                                          }.ToImmutableDictionary();
    private static readonly char[] __ends = ['\n', '\r'];

    public static bool ContainsAbout( this string source, string search ) => source.Contains(search, StringComparison.OrdinalIgnoreCase);
    public static bool ContainsExact( this string source, string search ) => source.Contains(search, StringComparison.Ordinal);


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
    public static bool IsBalanced( this string input, IReadOnlyDictionary<char, char>? bracketPairs = null ) => input.AsSpan().IsBalanced(bracketPairs); // TODO: ReadOnlySpan<char>


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


    public static                       byte[]       ToByteArray( this string value, Encoding? encoding = null ) => ( encoding ?? Encoding.Default ).GetBytes(value);
    [MustDisposeResource] public static Buffer<byte> AsSpanBytes( this string value, Encoding  encoding )        => AsSpanBytes(value.AsSpan(), encoding);
    [MustDisposeResource]
    public static Buffer<byte> AsSpanBytes( this ReadOnlySpan<char> value, Encoding encoding )
    {
        Buffer<byte> span = new(encoding.GetByteCount(value));
        encoding.GetBytes(value, span.Span);
        return span;
    }


    public static string[] SplitAndTrimLines( this string value, char separator = '\n' )
    {
        string[] array = value.Split(separator);

        for ( int i = 0; i < array.Length; i++ ) { array[i] = array[i].Trim(); }

        return array;
    }
    public static string[] SplitAndTrimLines( this string value, string separator )
    {
        string[] array = value.Split(separator);

        for ( int i = 0; i < array.Length; i++ ) { array[i] = array[i].Trim(); }

        return array;
    }


    public static string[]             SplitLines( this       string value, char      separator = '\n' ) => value.Split(separator);
    public static string[]             SplitLines( this       string value, string    separator )        => value.Split(separator);
    public static Memory<byte>         ToMemory( this         string value, Encoding? encoding = null )  => value.ToByteArray(encoding ?? Encoding.Default).AsMemory();
    public static object               ConvertTo( this        string value, Type      target )           => Convert.ChangeType(value, target);
    public static ReadOnlyMemory<byte> ToReadOnlyMemory( this string value, Encoding? encoding = null )  => value.ToMemory(encoding ?? Encoding.Default);


    public static SecureString ToSecureString( this ReadOnlySpan<byte>   value, bool makeReadonly = true ) => Convert.ToBase64String(value).AsSpan().ToSecureString(makeReadonly);
    public static SecureString ToSecureString( this string               value, bool makeReadonly = true ) => ToSecureString(value.AsSpan(), makeReadonly);
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
    public static string GetValue( this SecureString value )
    {
        IntPtr valuePtr = IntPtr.Zero;

        try
        {
            valuePtr = Marshal.SecureStringToBSTR(value);
            return Marshal.PtrToStringBSTR(valuePtr);
        }
        finally { Marshal.ZeroFreeBSTR(valuePtr); }
    }


    /// <summary>
    ///     <para>
    ///         <see href="https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm"/>
    ///     </para>
    /// </summary>
    /// <param name="str"> </param>
    /// <param name="separator"> the <see cref="char"/> to split on </param>
    public static SpanSplitEnumerator<char> SplitOn( this string str, char separator ) => str.AsSpan().SplitOn(separator);

    /// <summary>
    ///     <para>
    ///         <see href="https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm"/>
    ///     </para>
    ///     Default chars <see cref="char"/> to '\n' and '\r'
    /// </summary>
    /// <param name="str"> </param>
    public static SpanSplitEnumerator<char> SplitOn( this string str ) => str.AsSpan().SplitOn();


    /// <summary>
    ///     <para>
    ///         <see href="https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm"/>
    ///     </para>
    ///     Default chars <see cref="char"/> to '\n' and '\r'
    /// </summary>
    /// <param name="span"> </param>
    public static SpanSplitEnumerator<char> SplitOn( this ReadOnlySpan<char> span ) => new(span, __ends);


    public static SpanSplitEnumerator<TValue> SplitOn<TValue>( this Span<TValue> span, TValue separator )
        where TValue : unmanaged, IEquatable<TValue> => new(span, separator);
    public static SpanSplitEnumerator<TValue> SplitOn<TValue>( this Span<TValue> span, params TValue[] separators )
        where TValue : unmanaged, IEquatable<TValue> => new(span, separators);


    public static SpanSplitEnumerator<TValue> SplitOn<TValue>( this ReadOnlySpan<TValue> span, TValue separator )
        where TValue : unmanaged, IEquatable<TValue> => new(span, separator);
    public static SpanSplitEnumerator<TValue> SplitOn<TValue>( this ReadOnlySpan<TValue> span, params TValue[] separators )
        where TValue : unmanaged, IEquatable<TValue> => new(span, separators);


    public static string ConvertToString( this byte[]               value, Encoding encoding ) => encoding.GetString(value);
    public static string ConvertToString( this Span<byte>           value, Encoding encoding ) => encoding.GetString(value);
    public static string ConvertToString( this ReadOnlySpan<byte>   value, Encoding encoding ) => encoding.GetString(value);
    public static string ConvertToString( this Memory<byte>         value, Encoding encoding ) => value.Span.ConvertToString(encoding);
    public static string ConvertToString( this ReadOnlyMemory<byte> value, Encoding encoding ) => value.Span.ConvertToString(encoding);


    public static string RemoveAll( this string source, string old ) => source.Replace(old,           string.Empty, StringComparison.Ordinal);
    public static string RemoveAll( this string source, char   old ) => source.Replace(old.Repeat(1), string.Empty);


    /// <summary>
    ///     <seealso href="https://stackoverflow.com/a/48832421/9530917"/>
    /// </summary>
    /// <param name="c"> </param>
    /// <param name="count"> </param>
    /// <returns>
    ///     <see cref="string"/>
    /// </returns>
    public static string Repeat( this char c, int count ) => new(c, count);

    /// <summary>
    ///     <seealso href="https://stackoverflow.com/a/720915/9530917"/>
    /// </summary>
    /// <param name="value"> </param>
    /// <param name="count"> </param>
    /// <returns>
    ///     <see cref="string"/>
    /// </returns>
    public static string Repeat( this string value, int count ) => new StringBuilder(value.Length * count).Insert(0, value, count).ToString();

    public static string ReplaceAll( this      string source, string old, string newString ) => source.Replace(old, newString, StringComparison.Ordinal);
    public static string ReplaceAll( this      string source, char   old, char   newString ) => source.Replace(old, newString);
    public static string ToScreamingCase( this string value ) => value.ToSnakeCase().ToUpper().Replace("__", "_");


    /// <summary> inspired from <seealso href="https://stackoverflow.com/a/67332992/9530917"/> </summary>
    public static string ToSnakeCase( this string value ) => value.ToSnakeCase(CultureInfo.InvariantCulture);

    /// <summary> inspired from <seealso href="https://stackoverflow.com/a/67332992/9530917"/> </summary>
    public static string ToSnakeCase( this string value, CultureInfo cultureInfo ) => ToSnakeCase(value.AsSpan(), cultureInfo);
    public static string ToSnakeCase( scoped in ReadOnlySpan<char> span, CultureInfo cultureInfo )
    {
        if ( span.IsNullOrWhiteSpace() ) { return string.Empty; }

        StringBuilder    builder          = new(span.Length + span.Count(' ') + span.Count(Randoms.UPPER_CASE));
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


    /// <summary> Wraps a string in <paramref name="c"> </paramref> repeated <paramref name="padding"> </paramref> times. </summary>
    /// <param name="self"> </param>
    /// <param name="c"> </param>
    /// <param name="padding"> </param>
    /// <returns> </returns>
    public static string Wrapper( this string self, char c, int padding ) => self.PadLeft(padding, c).PadRight(padding, c);


    public static TResult ConvertTo<TResult>( this string value )
        where TResult : IConvertible => (TResult)value.ConvertTo(typeof(TResult));
}
