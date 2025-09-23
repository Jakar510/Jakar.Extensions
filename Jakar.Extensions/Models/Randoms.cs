// Jakar.Extensions :: Jakar.Extensions
// 09/07/2022  4:21 PM


using OtpNet;



namespace Jakar.Extensions;


/// <summary>
///     <see href="https://www.educative.io/edpresso/how-to-generate-a-random-string--c-sharp"/>
/// </summary>
[SuppressMessage("ReSharper", "RedundantVerbatimStringPrefix")]
public class Randoms : BaseClass
{
    public const           string ALPHANUMERIC  = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    public const           string LOWER_CASE    = @"abcdefghijklmnopqrstuvwxyz";
    public const           string NUMERIC       = @"0123456789";
    public const           string SPECIAL_CHARS = @"_-.!#@+/*^=>|/\";
    public const           string UPPER_CASE    = @"ABCDEFGHJKLMNOPQRSTUVWXYZ";
    public static readonly char[] AlphaNumeric  = [.. ALPHANUMERIC];
    public static readonly char[] LowerCase     = [.. LOWER_CASE];
    public static readonly char[] Numeric       = [.. NUMERIC];
    public static readonly char[] SpecialChars  = [.. SPECIAL_CHARS];
    public static readonly char[] UpperCase     = [.. UPPER_CASE];


    public static Random                Random { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; set; } = new(69420);
    public static RandomNumberGenerator Rng    { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; set; } = RandomNumberGenerator.Create();


    public static string Hex( int length )
    {
        Span<byte> token = new byte[length];
        Rng.GetBytes(token);
        return Convert.ToHexString(token);
    }
    public static string GenerateToken( int length )
    {
        byte[] token = new byte[length];
        Rng.GetBytes(new Span<byte>(token));
        return Base32Encoding.ToString(token);
    }
    public static string GenerateTokenB64( int length )
    {
        Span<byte> token = stackalloc byte[length];
        Rng.GetBytes(token);
        return Convert.ToBase64String(token);
    }


    public static char RandomChar( char startInclusive, char endExclusive ) => Convert.ToChar(RandomNumberGenerator.GetInt32(startInclusive, endExclusive));
    public static char RandomChar( int  startInclusive, int  endExclusive ) => Convert.ToChar(RandomNumberGenerator.GetInt32(startInclusive, endExclusive));


    public static string RandomString( int length ) => RandomString(length, char.ToUpperInvariant);
    public static string RandomString( int length, Func<char, char> converter )
    {
        Span<char> span = stackalloc char[length];
        for ( int i = 0; i < length; i++ ) { span[i] = converter(RandomChar(97, 123)); }

        return span.ToString();
    }


    public static string RandomString<TValue>( int length, TValue values )
        where TValue : IReadOnlyList<char> => RandomString(length, values, Random);
    public static string RandomString<TValue>( int length, TValue values, Random random )
        where TValue : IReadOnlyList<char>
    {
        Span<char> builder = stackalloc char[length];

        for ( int i = 0; i < length; i++ )
        {
            int index = random.Next(values.Count);
            builder[i] = values[index];
        }

        return builder.ToString();
    }


    public static string RandomString( int length, params ReadOnlySpan<char> values ) => RandomString(length, Random, values);
    public static string RandomString( int length, Random random, params ReadOnlySpan<char> values )
    {
        Span<char> builder = stackalloc char[length];

        for ( int i = 0; i < length; i++ )
        {
            int index = random.Next(values.Length);
            builder[i] = values[index];
        }

        return builder.ToString();
    }
}
