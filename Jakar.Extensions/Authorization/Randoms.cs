﻿// Jakar.Extensions :: Jakar.Extensions
// 09/07/2022  4:21 PM

namespace Jakar.Extensions;


/// <summary> <see href = "https://www.educative.io/edpresso/how-to-generate-a-random-string-in-c-sharp" /> </summary>
public class Randoms
{
    public static char[]                AlphaNumeric { get; }      = @"abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789".ToArray();
    public static char[]                Numeric      { get; }      = @"0123456789".ToArray();
    public static char[]                LowerCase    { get; }      = @"abcdefghijklmnopqrstuvwxyz".ToArray();
    public static char[]                UpperCase    { get; }      = @"ABCDEFGHJKLMNOPQRSTUVWXYZ".ToArray();
    public static char[]                SpecialChars { get; }      = @"_-.!#@+/*^=>|/\".ToArray();
    public static RandomNumberGenerator Rng          { get; set; } = RandomNumberGenerator.Create();
    public static Random                Random       { get; set; } = new(69);


    public static string RandomString( in int length ) => RandomString(length, char.ToUpperInvariant);
    public static string RandomString( in int length, Func<char, char> converter )
    {
        Span<char> builder = stackalloc char[length];

        for ( var i = 0; i < length; i++ )
        {
            var letter = Convert.ToChar(RandomNumberGenerator.GetInt32(97, 122));
            builder[i] = converter(letter);
        }

        return builder.ToString();
    }


    public static string RandomString( in int length, IReadOnlyList<char> values ) => RandomString(length, values, Random);
    public static string RandomString( in int length, IReadOnlyList<char> values, Random random )
    {
        Span<char> builder = stackalloc char[length];

        for ( var i = 0; i < length; i++ )
        {
            int index = random.Next(values.Count);
            builder[i] = values[index];
        }

        return builder.ToString();
    }


    public static string GenerateToken( in int length = 32 )
    {
        var randomNumber = new byte[length];
        Rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}