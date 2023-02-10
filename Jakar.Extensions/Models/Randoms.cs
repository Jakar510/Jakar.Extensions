// Jakar.Extensions :: Jakar.Extensions
// 09/07/2022  4:21 PM


namespace Jakar.Extensions;


/// <summary>
///     <see href="https://www.educative.io/edpresso/how-to-generate-a-random-string--c-sharp"/>
/// </summary>
public class Randoms : ObservableClass
{
    public static char[]                AlphaNumeric { get; }      = @"abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789".ToArray();
    public static char[]                LowerCase    { get; }      = @"abcdefghijklmnopqrstuvwxyz".ToArray();
    public static char[]                Numeric      { get; }      = @"0123456789".ToArray();
    public static char[]                SpecialChars { get; }      = @"_-.!#@+/*^=>|/\".ToArray();
    public static char[]                UpperCase    { get; }      = @"ABCDEFGHJKLMNOPQRSTUVWXYZ".ToArray();
    public static Random                Random       { get; set; } = new(69420);
    public static RandomNumberGenerator Rng          { get; set; } = RandomNumberGenerator.Create();


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


    public static string RandomString( int length, IReadOnlyList<char> values ) => RandomString( length, values, Random );
    public static string RandomString( int length, IReadOnlyList<char> values, Random random )
    {
        Span<char> builder = stackalloc char[length];

        for ( int i = 0; i < length; i++ )
        {
            int index = random.Next( values.Count );
            builder[i] = values[index];
        }

        return builder.ToString();
    }
}
