// Jakar.Extensions :: Jakar.Extensions
// 09/07/2022  4:21 PM


namespace Jakar.Extensions;


/// <summary>
///     <see href="https://www.educative.io/edpresso/how-to-generate-a-random-string--c-sharp"/>
/// </summary>
[SuppressMessage( "ReSharper", "RedundantVerbatimStringPrefix" )]
public class Randoms : ObservableClass
{
    public const string ALPHANUMERIC  = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    public const string LOWER_CASE    = @"abcdefghijklmnopqrstuvwxyz";
    public const string NUMERIC       = @"0123456789";
    public const string SPECIAL_CHARS = @"_-.!#@+/*^=>|/\";
    public const string UPPER_CASE    = @"ABCDEFGHJKLMNOPQRSTUVWXYZ";


    public static ReadOnlyMemory<char>  AlphaNumeric { get; }      = ALPHANUMERIC.ToArray();
    public static ReadOnlyMemory<char>  LowerCase    { get; }      = LOWER_CASE.ToArray();
    public static ReadOnlyMemory<char>  Numeric      { get; }      = NUMERIC.ToArray();
    public static Random                Random       { get; set; } = new(69420);
    public static RandomNumberGenerator Rng          { get; set; } = RandomNumberGenerator.Create();
    public static ReadOnlyMemory<char>  SpecialChars { get; }      = SPECIAL_CHARS.ToArray();
    public static ReadOnlyMemory<char>  UpperCase    { get; }      = UPPER_CASE.ToArray();


    public static string GenerateToken( int length = 32 )
    {
        Span<byte> randomNumber = stackalloc byte[length];
        Rng.GetNonZeroBytes( randomNumber );
        return Convert.ToBase64String( randomNumber );
    }


    public static char RandomChar( char startInclusive, char endExclusive ) => Convert.ToChar( RandomNumberGenerator.GetInt32( startInclusive, endExclusive ) );
    public static char RandomChar( int  startInclusive, int  endExclusive ) => Convert.ToChar( RandomNumberGenerator.GetInt32( startInclusive, endExclusive ) );


    public static string RandomString( int length ) => RandomString( length, char.ToUpperInvariant );
    public static string RandomString( int length, Func<char, char> converter )
    {
        Span<char> span = stackalloc char[length];
        for ( int i = 0; i < length; i++ ) { span[i] = converter( RandomChar( 97, 123 ) ); }

        return span.ToString();
    }


    public static string RandomString<T>( int length, T values )
        where T : IReadOnlyList<char> => RandomString( length, values, Random );
    public static string RandomString<T>( int length, T values, Random random )
        where T : IReadOnlyList<char>
    {
        Span<char> builder = stackalloc char[length];

        for ( int i = 0; i < length; i++ )
        {
            int index = random.Next( values.Count );
            builder[i] = values[index];
        }

        return builder.ToString();
    }
    public static string RandomString( int length, char[] values ) => RandomString( length, values, Random );
    public static string RandomString( int length, char[] values, Random random )
    {
        ReadOnlySpan<char> span = values;
        return RandomString( length, span, random );
    }
    public static string RandomString( int length, ReadOnlySpan<char> values ) => RandomString( length, values, Random );
    public static string RandomString( int length, ReadOnlySpan<char> values, Random random )
    {
        Span<char> builder = stackalloc char[length];

        for ( int i = 0; i < length; i++ )
        {
            int index = random.Next( values.Length );
            builder[i] = values[index];
        }

        return builder.ToString();
    }
}
