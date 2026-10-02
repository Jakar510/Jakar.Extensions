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
    public static readonly char[] AlphaNumeric = [.. ALPHANUMERIC];
    public static readonly char[] LowerCase    = [.. LOWER_CASE];
    public static readonly char[] Numeric      = [.. NUMERIC];
    public static readonly char[] SpecialChars = [.. SPECIAL_CHARS];
    public static readonly char[] UpperCase    = [.. UPPER_CASE];


    public static Random                Random { get; set; } = new(69420);
    public static RandomNumberGenerator Rng    { get; set; } = RandomNumberGenerator.Create();


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


    private const string LATIN_LOWER = "abcdefghijklmnopqrstuvwxyz";
    private const string LATIN_UPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


    /// <summary> <paramref name="length"/> uniformly random letters A-Z from <see cref="RandomNumberGenerator"/>. </summary>
    /// <remarks> One <see cref="RandomNumberGenerator.GetString"/> call instead of one <see cref="RandomNumberGenerator.GetInt32(int, int)"/> call per character. </remarks>
    public static string RandomString( int length ) => RandomNumberGenerator.GetString(LATIN_UPPER, length);
    /// <summary> <paramref name="length"/> uniformly random letters a-z from <see cref="RandomNumberGenerator"/>, each passed through <paramref name="converter"/>. </summary>
    public static string RandomString( int length, Func<char, char> converter )
    {
        Span<char> span = stackalloc char[length];
        RandomNumberGenerator.GetItems(LATIN_LOWER, span);
        for ( int i = 0; i < span.Length; i++ ) { span[i] = converter(span[i]); }

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
