#nullable enable
namespace Jakar.Extensions;


public static class StringExtensions
{
    private static readonly char[] _ends =
    {
        '\n',
        '\r',
    };


    public static bool ContainsAbout( this string source, string search ) => source.Contains( search, StringComparison.OrdinalIgnoreCase );
    public static bool ContainsExact( this string source, string search ) => source.Contains( search, StringComparison.Ordinal );


    /// <summary>
    ///     <seealso href="https://www.codeproject.com/Tips/1175562/Check-for-Balanced-Parenthesis-in-a-String"/>
    ///     <para>
    ///         <paramref name="bracketPairs"/> defaults to matching: <br/>
    ///         <list type="bullet">
    ///             <item>
    ///                 <term> ( ) </term> <description> Parenthesis </description>
    ///             </item>
    /// 
    ///             <item>
    ///                 <term> [ ] </term> <description> Square Brackets </description>
    ///             </item>
    /// 
    ///             <item>
    ///                 <term> { } </term> <description> Curly Braces </description>
    ///             </item>
    /// 
    ///         </list>
    ///     </para>
    ///     <para> Provide your own <c> IDictionary{char, char} </c> to <paramref name="bracketPairs"/> to customize the mapping. </para>
    /// </summary>
    /// <returns> <see langword="true"/> if balanced; otherwise <see langword="false"/> </returns>
    public static bool IsBalanced( this string input, IReadOnlyDictionary<char, char>? bracketPairs = default ) // TODO: ReadOnlySpan<char>
    {
        bracketPairs ??= new Dictionary<char, char>
                         {
                             { '(', ')' },
                             { '{', '}' },
                             { '[', ']' },
                         };

        var brackets = new Stack<char>();

        try
        {
            // Iterate through each character in the input string
            foreach ( char c in input )
            {
                // check if the character is one of the 'opening' brackets
                if ( bracketPairs.Keys.Contains( c ) )
                {
                    // if yes, push to stack
                    brackets.Push( c );
                }
                else if ( bracketPairs.Values.Contains( c ) ) // check if the character is one of the 'closing' brackets
                {
                    // check if the closing bracket matches the 'latest' 'opening' bracket
                    if ( c == bracketPairs[brackets.First()] ) { brackets.Pop(); }
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


    public static byte[] ToByteArray( this             string value, Encoding? encoding = default ) => (encoding ?? Encoding.Default).GetBytes( value );
    public static ReadOnlySpan<byte> AsSpanBytes( this string value, Encoding  encoding ) => AsSpanBytes( value.AsSpan(), encoding );
    public static unsafe ReadOnlySpan<byte> AsSpanBytes( this ReadOnlySpan<char> value, Encoding encoding )
    {
        Span<byte> span = stackalloc byte[encoding.GetByteCount( value )];
        encoding.GetBytes( value, span );
        return MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), span.Length );
    }


    public static IEnumerable<string> SplitAndTrimLines( this string value, char separator = '\n' ) => value.Split( separator )
                                                                                                            .Select( line => line.Trim() );
    public static IEnumerable<string> SplitAndTrimLines( this string value, string separator ) => value.Split( separator )
                                                                                                       .Select( line => line.Trim() );


    public static IEnumerable<string> SplitLines( this string value, char   separator = '\n' ) => value.Split( separator );
    public static IEnumerable<string> SplitLines( this string value, string separator ) => value.Split( separator );
    public static Memory<byte> ToMemory( this string value, Encoding? encoding = default ) => value.ToByteArray( encoding ?? Encoding.Default )
                                                                                                   .AsMemory();
    public static object ConvertTo( this                      string value, Type      target ) => Convert.ChangeType( value, target );
    public static ReadOnlyMemory<byte> ToReadOnlyMemory( this string value, Encoding? encoding = default ) => value.ToMemory( encoding ?? Encoding.Default );


    public static SecureString ToSecureString( this ReadOnlySpan<byte> value, bool makeReadonly = true ) => Convert.ToBase64String( value )
                                                                                                                   .AsSpan()
                                                                                                                   .ToSecureString( makeReadonly );
    public static SecureString ToSecureString( this string               value, bool makeReadonly = true ) => ToSecureString( value.AsSpan(), makeReadonly );
    public static SecureString ToSecureString( this Memory<char>         value, bool makeReadonly = true ) => value.Span.ToSecureString( makeReadonly );
    public static SecureString ToSecureString( this ReadOnlyMemory<char> value, bool makeReadonly = true ) => value.Span.ToSecureString( makeReadonly );
    public static SecureString ToSecureString( this Span<char>           value, bool makeReadonly = true ) => ((ReadOnlySpan<char>)value).ToSecureString( makeReadonly );
    public static unsafe SecureString ToSecureString( this ReadOnlySpan<char> value, bool makeReadonly = true )
    {
        fixed (char* token = &value.GetPinnableReference())
        {
            var secure = new SecureString( token, value.Length );
            if ( makeReadonly ) { secure.MakeReadOnly(); }

            return secure;
        }
    }
    public static string GetValue( this SecureString value )
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
    ///     <para>
    ///         <see href="https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm"/>
    ///     </para>
    /// </summary>
    /// <param name="str"> </param>
    /// <param name="separator"> the <see cref="char"/> to split on </param>
    public static SpanSplitEnumerator<char> SplitOn( this string str, char separator ) => str.AsSpan()
                                                                                             .SplitOn( separator );

    /// <summary>
    ///     <para>
    ///         <see href="https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm"/>
    ///     </para>
    ///     Default chars <see cref="char"/> to '\n' and '\r'
    /// </summary>
    /// <param name="str"> </param>
    public static SpanSplitEnumerator<char> SplitOn( this string str ) => str.AsSpan()
                                                                             .SplitOn();


    /// <summary>
    ///     <para>
    ///         <see href="https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm"/>
    ///     </para>
    ///     Default chars <see cref="char"/> to '\n' and '\r'
    /// </summary>
    /// <param name="span"> </param>
    public static SpanSplitEnumerator<char> SplitOn( this ReadOnlySpan<char> span ) => new(span, _ends);

    // public static SpanSplitEnumerator<T> SplitOn<T>( this Span<T>         span, ParamsArray<T> array ) where T : unmanaged, IEquatable<T> => new(span, array);
    public static SpanSplitEnumerator<T> SplitOn<T>( this Span<T> span, params T[]      separators ) where T : unmanaged, IEquatable<T> => new(span, separators);
    public static SpanSplitEnumerator<T> SplitOn<T>( this Span<T> span, T               separator ) where T : unmanaged, IEquatable<T> => new(span, Spans.Create( separator ));
    public static SpanSplitEnumerator<T> SplitOn<T>( this Span<T> span, ReadOnlySpan<T> separators ) where T : unmanaged, IEquatable<T> => new(span, separators);


    // public static SpanSplitEnumerator<T> SplitOn<T>( this ReadOnlySpan<T> span, ParamsArray<T> array ) where T : unmanaged, IEquatable<T> => new(span, array);
    public static SpanSplitEnumerator<T> SplitOn<T>( this ReadOnlySpan<T> span, params T[]      separators ) where T : unmanaged, IEquatable<T> => new(span, separators);
    public static SpanSplitEnumerator<T> SplitOn<T>( this ReadOnlySpan<T> span, T               separator ) where T : unmanaged, IEquatable<T> => new(span, Spans.Create( separator ));
    public static SpanSplitEnumerator<T> SplitOn<T>( this ReadOnlySpan<T> span, ReadOnlySpan<T> separators ) where T : unmanaged, IEquatable<T> => new(span, separators);


    public static string ConvertToString( this byte[] value, Encoding? encoding = default ) => (encoding ?? Encoding.Default).GetString( value );
    public static string ConvertToString( this Memory<byte> value, Encoding? encoding = default ) => value.ToArray()
                                                                                                          .ConvertToString( encoding ?? Encoding.Default );
    public static string ConvertToString( this ReadOnlyMemory<byte> value, Encoding? encoding = default ) => value.ToArray()
                                                                                                                  .ConvertToString( encoding ?? Encoding.Default );


    public static string RemoveAll( this string source, string old ) => source.Replace( old,                  string.Empty, StringComparison.Ordinal );
    public static string RemoveAll( this string source, char   old ) => source.Replace( new string( old, 1 ), string.Empty );


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
    public static string Repeat( this string value, int count ) => new StringBuilder( value.Length * count ).Insert( 0, value, count )
                                                                                                            .ToString();

    public static string ReplaceAll( this string source, string old, string newString ) => source.Replace( old, newString, StringComparison.Ordinal );
    public static string ReplaceAll( this string source, char   old, char   newString ) => source.Replace( old, newString );
    public static string ToScreamingCase( this string value ) => value.ToSnakeCase()
                                                                      .ToUpper()
                                                                      .Replace( "__", "_" );


    /// <summary> copied from <seealso href="https://stackoverflow.com/a/67332992/9530917"/> </summary>
    public static string ToSnakeCase( this string value ) => value.ToSnakeCase( CultureInfo.InvariantCulture );
    /// <summary> copied from <seealso href="https://stackoverflow.com/a/67332992/9530917"/> </summary>
    public static string ToSnakeCase( this string value, CultureInfo cultureInfo )
    {
        if ( string.IsNullOrWhiteSpace( value ) ) { return value; }


        using var        builder          = new ValueStringBuilder( value.Length + Math.Max( 2, value.Length / 5 ) );
        UnicodeCategory? previousCategory = default;

        for ( int currentIndex = 0; currentIndex < value.Length; currentIndex++ )
        {
            char currentChar = value[currentIndex];

            switch ( currentChar )
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
            switch ( currentCategory )
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                    if ( previousCategory is UnicodeCategory.SpaceSeparator or UnicodeCategory.LowercaseLetter ||
                         previousCategory is not UnicodeCategory.DecimalDigitNumber && previousCategory is not null && currentIndex > 0 && currentIndex + 1 < value.Length && char.IsLower( value[currentIndex + 1] ) ) { builder.Append( '_' ); }

                    currentChar = char.ToLower( currentChar, cultureInfo );
                    break;

                case UnicodeCategory.LowercaseLetter:
                    if ( previousCategory is UnicodeCategory.SpaceSeparator ) { builder.Append( '_' ); }

                    break;

                default:
                    if ( previousCategory is not null ) { previousCategory = UnicodeCategory.SpaceSeparator; }

                    continue;
            }

            builder.Append( currentChar );
            previousCategory = currentCategory;
        }

        return builder.ToString();
    }


    /// <summary> Wraps a string in <paramref name="c"> </paramref> repeated <paramref name="padding"> </paramref> times. </summary>
    /// <param name="self"> </param>
    /// <param name="c"> </param>
    /// <param name="padding"> </param>
    /// <returns> </returns>
    public static string Wrapper( this string self, char c, int padding ) => self.PadLeft( padding, c )
                                                                                 .PadRight( padding, c );


    public static TResult ConvertTo<TResult>( this string value ) where TResult : IConvertible => (TResult)value.ConvertTo( typeof(TResult) );
}
