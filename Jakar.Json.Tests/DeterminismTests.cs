// Jakar.Json.Tests
// 10/07/2026

using System.Numerics;



namespace Jakar.Json.Tests;


/// <summary> SPEC.md §5.1 I1/I4/I5: the same value writes the same bytes whatever the culture, and one round trip reaches the canonical form. </summary>
[TestFixture]
[NonParallelizable]
public sealed class DeterminismTests
{
    private static readonly string[] __cultures =
    [
        "tr-TR",
        "ar-SA",
        "de-DE",
        "fr-FR",
        "ja-JP",
        "fa-IR",
        ""
    ];


    private static Scalars Sample() => new()
                                       {
                                           Int      = -1234567,
                                           Double   = -1234.5678e-10,
                                           Decimal  = 12345.678m,
                                           Single   = 3.25f,
                                           Big      = BigInteger.Parse("-123456789012345678901234567890", CultureInfo.InvariantCulture),
                                           Text     = "İıiI",
                                           DateTime = new DateTime(2026, 10, 7, 13, 14, 15, 123, DateTimeKind.Utc),
                                           Offset   = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.FromHours(5.5)),
                                           Date     = new DateOnly(2026, 2, 28),
                                           Time     = new TimeOnly(1, 2, 3),
                                           Span     = TimeSpan.FromDays(-1.5),
                                           Rights   = Permissions.Read | Permissions.Write
                                       };


    [Test] public void Output_IsTheSameInEveryCulture()
    {
        CultureInfo original = CultureInfo.CurrentCulture;
        string      expected = Run(CultureInfo.InvariantCulture);

        try
        {
            foreach ( string name in __cultures )
            {
                CultureInfo culture = CultureInfo.GetCultureInfo(name);
                Assert.That(Run(culture), Is.EqualTo(expected), name);
            }
        }
        finally { CultureInfo.CurrentCulture = original; }

        static string Run( CultureInfo culture )
        {
            CultureInfo.CurrentCulture   = culture;
            CultureInfo.CurrentUICulture = culture;

            Scalars value = Sample();
            string  json  = value.ToJson();
            Assert.That(Scalars.FromJson(json).ToJson(), Is.EqualTo(json), $"{culture.Name}: read back");
            return json;
        }
    }

    [Test] public void Canonicalization_IsAFixedPoint()
    {
        // Same values, spelled differently: whitespace, escapes, exponent forms, member order.
        const string LOOSE = """
                             {   "Double" : -1.2345678E-07, "Text":"İıiI", "Int" : -1234567,
                                 "Rights" : "Write, Read" }
                             """;

        string canonical = Scalars.FromJson(LOOSE).ToJson();
        Assert.That(Scalars.FromJson(canonical).ToJson(), Is.EqualTo(canonical));
        Assert.That(canonical,                            Does.Contain("\"Double\" : -1.2345678e-7"));
        Assert.That(canonical,                            Does.Contain("\"Rights\" : \"Read, Write\""));
    }

    [Test] public void Utf8_IsAlwaysTheUtf8OfUtf16()
    {
        foreach ( object model in new object[] { Sample(), ModelTests.SampleInvoice(), new Collections { Set = ["b", "a"] }, new Zoo { Animals = [new Cat()] } } )
        {
            ( string text, byte[] utf8 ) = model switch
                                           {
                                               Scalars s     => ( s.ToJson(), s.ToJsonUtf8() ),
                                               Invoice i     => ( i.ToJson(), i.ToJsonUtf8() ),
                                               Collections c => ( c.ToJson(), c.ToJsonUtf8() ),
                                               Zoo z         => ( z.ToJson(), z.ToJsonUtf8() ),
                                               _             => ( "", [] )
                                           };

            Assert.That(utf8, Is.EqualTo(Encoding.UTF8.GetBytes(text)), model.GetType().Name);
        }
    }
}
