#nullable enable
namespace Jakar.Extensions;


public static class StringExtensions
{
    public static bool ContainsAbout( this string source, string search ) => source.Contains( search, StringComparison.OrdinalIgnoreCase );


    public static bool ContainsExact( this string source, string search ) => source.Contains( search, StringComparison.Ordinal );


    public static TResult ConvertTo<TResult>( this string value ) where TResult : IConvertible => (TResult)value.ConvertTo( typeof(TResult) );
    public static object ConvertTo( this           string value, Type target ) => Convert.ChangeType( value, target );


    public static string ConvertToString( this byte[] s, Encoding? encoding = default ) => (encoding ?? Encoding.Default).GetString( s );

    public static string ConvertToString( this Memory<byte> s, Encoding? encoding = default ) => s.ToArray()
                                                                                                  .ConvertToString( encoding ?? Encoding.Default );

    public static string ConvertToString( this ReadOnlyMemory<byte> s, Encoding? encoding = default ) => s.ToArray()
                                                                                                          .ConvertToString( encoding ?? Encoding.Default );
    public static string GetStringValue( this SecureString value )
    {
        IntPtr valuePtr = IntPtr.Zero;

        try
        {
            valuePtr = Marshal.SecureStringToBSTR( value );
            return Marshal.PtrToStringBSTR( valuePtr );
        }
        finally { Marshal.ZeroFreeBSTR( valuePtr ); }
    }


    /// <summary>
    ///     <seealso href = "https://www.codeproject.com/Tips/1175562/Check-for-Balanced-Parenthesis-in-a-String" />
    ///     <para>
    ///         <paramref name = "bracketPairs" />
    ///         defaults to matching:
    ///         <br />
    ///         <list type = "bullet" >
    ///             <item>
    ///                 <term> ( ) </term>
    ///                 <description> Parenthesis </description>
    ///             </item>
    /// 
    ///             <item>
    ///                 <term> [ ] </term>
    ///                 <description> Square Brackets </description>
    ///             </item>
    /// 
    ///             <item>
    ///                 <term> { } </term>
    ///                 <description> Curly Braces </description>
    ///             </item>
    /// 
    ///         </list>
    ///     </para>
    ///     <para>
    ///         Provide your own
    ///         <c> IDictionary{char, char} </c>
    ///         to
    ///         <paramref name = "bracketPairs" />
    ///         to customize the mapping.
    ///     </para>
    /// </summary>
    /// <returns>
    ///     <see langword = "true" />
    ///     if balanced; otherwise
    ///     <see langword = "false" />
    /// </returns>
    public static bool IsBalanced( this string input, IReadOnlyDictionary<char, char>? bracketPairs = default ) // TODO: ReadOnlySpan<char>
    {
        bracketPairs ??= new Dictionary<char, char>
                         {
                             { '(', ')' },
                             { '{', '}' },
                             { '[', ']' }
                         };

        var brackets = new Stack<char>();

        try
        {
            // Iterate through each character in the input string
            foreach (char c in input)
            {
                // check if the character is one of the 'opening' brackets
                if (bracketPairs.Keys.Contains( c ))
                {
                    // if yes, push to stack
                    brackets.Push( c );
                }
                else if (bracketPairs.Values.Contains( c )) // check if the character is one of the 'closing' brackets
                {
                    // check if the closing bracket matches the 'latest' 'opening' bracket
                    if (c == bracketPairs[brackets.First()]) { brackets.Pop(); }
                    else { return false; } // if not, its an unbalanced string
                }

                // continue looking
            }
        }
        catch
        {
            // an exception will be caught in case a closing bracket is found, before any opening bracket. that implies, the string is not balanced. Return false
            return false;
        }

        // Ensure all brackets are closed
        return !brackets.Any();
    }


    public static string RemoveAll( this string source, string old ) => source.Replace( old,                  "", StringComparison.Ordinal );
    public static string RemoveAll( this string source, char   old ) => source.Replace( new string( old, 1 ), "" );


    /// <summary>
    ///     <seealso href = "https://stackoverflow.com/a/48832421/9530917" />
    /// </summary>
    /// <param name = "c" > </param>
    /// <param name = "count" > </param>
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
    public static string Repeat( this char c, int count ) => new(c, count);

    /// <summary>
    ///     <seealso href = "https://stackoverflow.com/a/720915/9530917" />
    /// </summary>
    /// <param name = "value" > </param>
    /// <param name = "count" > </param>
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
    public static string Repeat( this string value, int count ) => new StringBuilder( value.Length * count ).Insert( 0, value, count )
                                                                                                            .ToString();

    public static string ReplaceAll( this string source, string old, string newString ) => source.Replace( old, newString, StringComparison.Ordinal );
    public static string ReplaceAll( this string source, char   old, char   newString ) => source.Replace( old, newString );


    public static IEnumerable<string> SplitAndTrimLines( this string text, char separator = '\n' ) => text.Split( separator )
                                                                                                          .Select( line => line.Trim() );
    public static IEnumerable<string> SplitAndTrimLines( this string text, string separator ) => text.Split( separator )
                                                                                                     .Select( line => line.Trim() );


    public static IEnumerable<string> SplitLines( this string text, char   separator = '\n' ) => text.Split( separator );
    public static IEnumerable<string> SplitLines( this string text, string separator ) => text.Split( separator );


    /// <summary>
    ///     <para>
    ///         <see href = "https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm" />
    ///     </para>
    /// </summary>
    /// <param name = "str" > </param>
    /// <param name = "separator" >
    ///     the
    ///     <see cref = "char" />
    ///     to split on
    /// </param>
    /// <returns>
    ///     <see cref = "SpanSplitEnumerator{T}" />
    /// </returns>
    public static SpanSplitEnumerator<char> SplitOn( this string str, char separator ) => str.AsSpan()
                                                                                             .SplitOn( separator );

    /// <summary>
    ///     <para>
    ///         <see href = "https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm" />
    ///     </para>
    ///     Default chars
    ///     <see cref = "char" />
    ///     to '\n' and '\r'
    /// </summary>
    /// <param name = "str" > </param>
    /// <returns>
    ///     <see cref = "SpanSplitEnumerator{T}" />
    /// </returns>
    public static SpanSplitEnumerator<char> SplitOn( this string str ) => str.AsSpan()
                                                                             .SplitOn();


    /// <summary>
    ///     <para>
    ///         <see href = "https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm" />
    ///     </para>
    /// </summary>
    /// <param name = "span" > </param>
    /// <param name = "separator" >
    ///     the
    ///     <see cref = "char" />
    ///     to split on
    /// </param>
    /// <returns>
    ///     <see cref = "SpanSplitEnumerator{T}" />
    /// </returns>
    public static SpanSplitEnumerator<T> SplitOn<T>( this ReadOnlySpan<T> span, T separator ) where T : IEquatable<T> => new(span, separator);

    /// <summary>
    ///     <para>
    ///         <see href = "https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm" />
    ///     </para>
    ///     Default chars
    ///     <see cref = "char" />
    ///     to '\n' and '\r'
    /// </summary>
    /// <param name = "span" > </param>
    /// <returns>
    ///     <see cref = "SpanSplitEnumerator{T}" />
    /// </returns>
    public static SpanSplitEnumerator<char> SplitOn( this ReadOnlySpan<char> span ) => new(span, '\n', '\r');


    public static byte[] ToByteArray( this string s, Encoding? encoding = default ) => (encoding ?? Encoding.Default).GetBytes( s );

    public static Memory<byte> ToMemory( this string s, Encoding? encoding = default ) => s.ToByteArray( encoding ?? Encoding.Default )
                                                                                           .AsMemory();

    public static ReadOnlyMemory<byte> ToReadOnlyMemory( this string s, Encoding? encoding = default ) => s.ToMemory( encoding ?? Encoding.Default );

    public static string ToScreamingCase( this string text ) => text.ToSnakeCase()
                                                                    .ToUpper()
                                                                    .Replace( "__", "_" );
    public static unsafe SecureString ToSecureString( this string value, bool makeReadonly = true )
    {
        fixed (char* token = value)
        {
            var secure = new SecureString( token, value.Length );
            if (makeReadonly) { secure.MakeReadOnly(); }

            return secure;
        }
    }


    /// <summary>
    ///     copied from
    ///     <seealso href = "https://stackoverflow.com/a/67332992/9530917" />
    /// </summary>
    /// <param name = "text" > </param>
    /// <returns> </returns>
    public static string ToSnakeCase( this string text )
    {
        if (string.IsNullOrEmpty( text )) { return text; }

        var builder          = new StringBuilder( text.Length + Math.Min( 2, text.Length / 5 ) );
        var previousCategory = default(UnicodeCategory?);

        for (int currentIndex = 0; currentIndex < text.Length; currentIndex++)
        {
            char currentChar = text[currentIndex];

            switch (currentChar)
            {
                case '_':
                    builder.Append( '_' );
                    previousCategory = null;
                    continue;

                case '.':
                    builder.Append( '.' );
                    previousCategory = null;
                    continue;
            }

            UnicodeCategory currentCategory = char.GetUnicodeCategory( currentChar );

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (currentCategory)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                    if (previousCategory is UnicodeCategory.SpaceSeparator or UnicodeCategory.LowercaseLetter ||
                        previousCategory != UnicodeCategory.DecimalDigitNumber && previousCategory != null && currentIndex > 0 && currentIndex + 1 < text.Length && char.IsLower( text[currentIndex + 1] )) { builder.Append( '_' ); }

                    currentChar = char.ToLower( currentChar, CultureInfo.InvariantCulture );
                    break;

                case UnicodeCategory.LowercaseLetter:
                    if (previousCategory == UnicodeCategory.SpaceSeparator) { builder.Append( '_' ); }

                    break;

                default:
                    if (previousCategory != null) { previousCategory = UnicodeCategory.SpaceSeparator; }

                    continue;
            }

            builder.Append( currentChar );
            previousCategory = currentCategory;
        }

        return builder.ToString();
    }


    /// <summary>
    ///     Wraps a string in
    ///     <paramref name = "c" > </paramref>
    ///     repeated
    ///     <paramref name = "padding" > </paramref>
    ///     times.
    /// </summary>
    /// <param name = "self" > </param>
    /// <param name = "c" > </param>
    /// <param name = "padding" > </param>
    /// <returns> </returns>
    public static string Wrapper( this string self, in char c, in int padding ) => self.PadLeft( padding, c )
                                                                                       .PadRight( padding, c );
}
