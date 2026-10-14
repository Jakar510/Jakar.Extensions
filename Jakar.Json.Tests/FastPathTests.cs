// Jakar.Json.Tests
// 10/13/2026

using System.Numerics;

namespace Jakar.Json.Tests;


/// <summary> The number and date fast paths (<see cref="JsonFastNumbers"/>, <see cref="JsonNumbers"/>, <see cref="JsonFastDates"/>) must give exactly what the BCL gives, for every input they accept. </summary>
[TestFixture]
public sealed class FastPathTests
{
    private const int COUNT = 200_000;


    [Test] public void DecimalParse_MatchesTheBcl()
    {
        Random random = new(510);

        for ( int i = 0; i < COUNT; i++ )
        {
            string text = RandomNumberText(random, 19);
            AssertDecimal(text);
        }

        foreach ( string text in new[] { "0", "-0", "0.00", "1.10", "-0.005", "9999999999999999999", "0.0000000000000000001", "18446744073709551615", "1e3", "1.5E-2", "12345678901234567890" } ) { AssertDecimal(text); }

        static void AssertDecimal( string text )
        {
            decimal expected = decimal.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);

            foreach ( decimal actual in new[] { Parse<decimal>(text), ParseUtf8<decimal>(text) } )
            {
                Assert.That(decimal.GetBits(actual), Is.EqualTo(decimal.GetBits(expected)), text); // scale and sign included
            }
        }
    }

    [Test] public void DoubleParse_MatchesTheBcl()
    {
        Random random = new(511);

        for ( int i = 0; i < COUNT; i++ )
        {
            string text = RandomNumberText(random, 17);
            double expected = double.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);

            Assert.That(BitConverter.DoubleToInt64Bits(Parse<double>(text)),     Is.EqualTo(BitConverter.DoubleToInt64Bits(expected)), text);
            Assert.That(BitConverter.DoubleToInt64Bits(ParseUtf8<double>(text)), Is.EqualTo(BitConverter.DoubleToInt64Bits(expected)), text);

            float single = float.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
            Assert.That(BitConverter.SingleToInt32Bits(Parse<float>(text)), Is.EqualTo(BitConverter.SingleToInt32Bits(single)), text);
        }
    }

    [Test] public void Dates_MatchTheBcl()
    {
        Random random = new(513);
        int    fast   = 0;

        for ( int i = 0; i < COUNT; i++ )
        {
            DateTime moment = new(random.NextInt64(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks));

            string text = random.Next(6) switch
                          {
                              0 => moment.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                              1 => moment.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture) + "Z",
                              2 => moment.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture) + "Z",
                              3 => moment.ToString("yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture) + Offset(random),
                              4 => moment.ToString("yyyy-MM-ddTHH:mm:ss.ffffffff".Substring(0, 20 + random.Next(1, 8)), CultureInfo.InvariantCulture) + Offset(random),
                              _ => Mangle(random, moment.ToString("O", CultureInfo.InvariantCulture))
                          };

            if ( AssertDates(text) ) { fast++; }
        }

        foreach ( string text in new[]
                                 {
                                     "0001-01-01T00:00:00Z", "9999-12-31T23:59:59.9999999Z", "0001-01-01T00:00:00+01:00", "9999-12-31T23:59:59-01:00", "2024-02-29T12:00:00",
                                     "2023-02-29T12:00:00", "2024-01-01T24:00:00", "2024-01-01T23:59:60Z", "2024-01-01T00:00:00.12345678Z", "2024-01-01T00:00:00.Z",
                                     "2024-01-01T00:00:00+14:00", "2024-01-01T00:00:00+14:01", "2024-01-01T00:00:00-00:00", "2024-01-01T00:00:00+0530", "2024-01-01 00:00:00",
                                     "2024-01-01T00:00:00z", "2024-01-01T00:00:00Z ", "2024-1-01T00:00:00Z", "0000-01-01T00:00:00Z"
                                 } ) { AssertDates(text); }

        Assert.That(fast, Is.GreaterThan(COUNT / 2), "the fast path should take the common shapes");

        static string Offset( Random random ) => random.Next(3) == 0
                                                     ? "Z"
                                                     : $"{( random.Next(2) == 0 ? '+' : '-' )}{random.Next(0, 15):00}:{random.Next(4) * 15:00}";

        static string Mangle( Random random, string text )
        {
            char[] chars = text.ToCharArray();
            chars[random.Next(chars.Length)] = "0123456789-:T.Z+ x"[random.Next(18)];
            return new string(chars);
        }

        // Every input the fast path accepts must give the BCL's value, kind and offset; returns whether it was accepted.
        static bool AssertDates( string text )
        {
            bool accepted = false;

            if ( JsonFastDates.TryParseDateTimeOffset(text, out DateTimeOffset offset) )
            {
                accepted = true;
                Assert.That(DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset expected), Is.True, text);
                Assert.That((offset.Ticks, offset.Offset), Is.EqualTo((expected.Ticks, expected.Offset)), text);
            }

            if ( JsonFastDates.TryParseDateTime(text, out DateTime date) )
            {
                accepted = true;
                Assert.That(DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime expected), Is.True, text);
                Assert.That((date.Ticks, date.Kind), Is.EqualTo((expected.Ticks, expected.Kind)), text);
            }

            return accepted;
        }
    }


    /// <summary> UTF-8 string scanning checks 16 bytes per vector compare; it must find the same end, escapes and errors as the UTF-16 (scalar) scanner, wherever they fall. </summary>
    [Test] public void StringScan_Utf8MatchesUtf16()
    {
        Random random = new(514);
        string pieces = "abcXYZ019 _-é€"; // ASCII, 2- and 3-byte UTF-8

        for ( int i = 0; i < 50_000; i++ )
        {
            System.Text.StringBuilder body = new();
            int length = random.Next(0, 40);

            for ( int j = 0; j < length; j++ )
            {
                switch ( random.Next(40) )
                {
                    case 0:  body.Append("\\\"");    break; // JSON \"
                    case 1:  body.Append("\\\\");    break; // JSON \\
                    case 2:  body.Append("\\n");     break;
                    case 3:  body.Append("\\u00e9"); break;
                    case 4:  body.Append('\t');      break; // a raw control character: invalid
                    case 5:  body.Append("\\q");     break; // an invalid escape
                    default: body.Append(pieces[random.Next(pieces.Length)]); break;
                }
            }

            string json = "{\"k\" : \"" + body + "\", \"tail\" : [1, \"" + new string('x', random.Next(0, 20)) + "\"]}";

            bool utf16 = JsonTape.TryParse(json, out JsonTape? a, out JsonError e16);
            bool utf8  = JsonTape.TryParse(Encoding.UTF8.GetBytes(json), out JsonTape? b, out JsonError e8);

            Assert.That(utf8, Is.EqualTo(utf16), json);

            if ( utf16 )
            {
                Assert.That(b!.Root["k"].GetString(), Is.EqualTo(a!.Root["k"].GetString()), json);
                Assert.That(b.Root["tail"][1].GetString(), Is.EqualTo(a.Root["tail"][1].GetString()), json);
            }
            else { Assert.That(e8.Kind, Is.EqualTo(e16.Kind), json); }

            a?.Dispose();
            b?.Dispose();
        }
    }


    [Test] public void DecimalFormat_MatchesTheBcl()
    {
        Random random = new(512);

        for ( int i = 0; i < COUNT; i++ )
        {
            decimal value = new(random.Next(), random.Next(4) == 0 ? random.Next() : 0, random.Next(16) == 0 ? random.Next() : 0, random.Next(2) == 0, (byte)random.Next(29));
            AssertFormat(value);
        }

        foreach ( decimal value in new[] { 0m, -0m, 0.00m, 1.10m, -0.005m, decimal.MaxValue, decimal.MinValue, 1m / 3m, ulong.MaxValue, -(decimal)ulong.MaxValue } ) { AssertFormat(value); }

        static void AssertFormat( decimal value )
        {
            string expected = value.ToString(CultureInfo.InvariantCulture);

            JsonWriter utf16 = new(JsonWriterOptions.Default);
            utf16.WriteFloat(value);
            Assert.That(utf16.ToString(), Is.EqualTo(expected), "UTF-16");

            JsonUtf8Writer utf8 = new(JsonWriterOptions.Default);
            utf8.WriteFloat(value);
            Assert.That(Encoding.UTF8.GetString(utf8.Written), Is.EqualTo(expected), "UTF-8");
            utf16.Dispose();
            utf8.Dispose();
        }
    }

    [Test] public void SizeHints_DoNotChangeTheOutput()
    {
        Collections small = new() { List = ["a"] };
        Collections big   = new() { List = [.. Enumerable.Range(0, 5_000).Select(static i => $"item-{i}")] };

        string smallJson = small.ToJson();
        string bigJson   = big.ToJson();
        Assert.That(big.ToJson(),   Is.EqualTo(bigJson),   "the second write starts from the hint");
        Assert.That(small.ToJson(), Is.EqualTo(smallJson), "a smaller document after a big one");
        Assert.That(Encoding.UTF8.GetString(big.ToJsonUtf8()), Is.EqualTo(bigJson));
        Assert.That(Encoding.UTF8.GetString(big.ToJsonUtf8()), Is.EqualTo(bigJson));
    }


    [Test] public void ExpectedNames_FallBackWhenOrderOrEscapingDiffers()
    {
        LineItem expected = new("a", 1, 2m);

        string[] inputs =
        [
            """{"sku":"a","quantity":1,"price":2}""",           // declaration order: every name is the expected one
            """{"price":2,"quantity":1,"sku":"a"}""",           // reversed: every expected-name check misses
            """{"quantity":1,"sku":"a","extra":[1],"price":2}""", // shuffled, with an unknown member
            """{"\u0073ku":"a","quantity":1,"price":2}""",      // the same name, escaped differently than it's written
            """{ "sku" : "a" , "quantity" : 1 , "price" : 2 }"""   // whitespace everywhere
        ];

        foreach ( string json in inputs )
        {
            Assert.That(LineItem.FromJson(json),                          Is.EqualTo(expected), json);
            Assert.That(LineItem.FromJson(Encoding.UTF8.GetBytes(json)), Is.EqualTo(expected), json + " (UTF-8)");
        }

        Assert.That(LineItem.TryFromJson("""{"sku":"a","sku":"b","quantity":1,"price":2}""", out _), Is.False, "duplicates are still caught when the first is an expected-name hit");
    }


    /// <summary> <c>-?digits(.digits)?</c> with up to <paramref name="maxDigits"/> digits (so most take the fast path), sometimes with an exponent or a leading zero (so some don't). </summary>
    private static string RandomNumberText( Random random, int maxDigits )
    {
        StringBuilder text = new();
        if ( random.Next(3) == 0 ) { text.Append('-'); }

        int whole = random.Next(0, maxDigits);
        if ( whole == 0 ) { text.Append('0'); }
        else
        {
            text.Append((char)( '1' + random.Next(9) ));
            for ( int i = 1; i < whole; i++ ) { text.Append((char)( '0' + random.Next(10) )); }
        }

        int fraction = random.Next(0, Math.Max(1, maxDigits - whole + 2));

        if ( fraction > 0 )
        {
            text.Append('.');
            for ( int i = 0; i < fraction; i++ ) { text.Append((char)( '0' + random.Next(10) )); }
        }

        if ( random.Next(10) == 0 ) { text.Append('e').Append(random.Next(-5, 6)); }

        return text.ToString();
    }

    private static T Parse<T>( string text )
        where T : struct, IFloatingPoint<T>
    {
        JsonReader<char> reader = new(text.AsSpan(), JsonReaderOptions.Default);
        Assert.That(reader.TryReadFloat(out T value), Is.True, text);
        return value;
    }

    private static T ParseUtf8<T>( string text )
        where T : struct, IFloatingPoint<T>
    {
        JsonReader<byte> reader = new(Encoding.UTF8.GetBytes(text), JsonReaderOptions.Default);
        Assert.That(reader.TryReadFloat(out T value), Is.True, text);
        return value;
    }
}
