// Jakar.Json.Tests
// 10/07/2026

namespace Jakar.Json.Tests;


/// <summary>
///     Seeded fuzzing: mutations of valid documents and random bytes must never crash, hang or throw from a <c>Try*</c> API (SPEC.md §12.4).
///     The seed is fixed so failures reproduce; the long-running coverage-guided fuzzing is a separate CI job.
/// </summary>
[TestFixture]
public sealed class RobustnessTests
{
    private const int ITERATIONS = 20_000;

    private static readonly string[] __seeds = [ModelTests.SampleInvoice().ToJson(), new Zoo { Animals = [new Dog { Name = "d" }, new Cat { Name = "c" }] }.ToJson(JsonWriterOptions.Indent), """{"Title":"t","x":{"y":[1,2,{"z":"é\n"}]},"n":-1.5e-3}""", """[{"sku":"a","quantity":1,"price":1},[],{},"s",true,null]"""];

    private static readonly char[] __alphabet = "{}[]\",:\\/0123456789-+.eEtrufalsn \t\né\ud800abc".ToCharArray();


    [Test] public void MutatedDocuments_NeverThrow()
    {
        Random random = new(510);

        for ( int i = 0; i < ITERATIONS; i++ )
        {
            string json = Mutate(__seeds[i % __seeds.Length], random);
            byte[] utf8 = Encoding.UTF8.GetBytes(json);

            Assert.DoesNotThrow(() =>
                                {
                                    Invoice.TryFromJson(json,          out _);
                                    Invoice.TryFromJson(utf8.AsSpan(), out _);
                                    Zoo.TryFromJson(json, out _);
                                    Bag.TryFromJson(utf8.AsSpan(), out _);
                                    JNode.TryParse(json, out _, out _);
                                    if ( JsonTape.TryParse(utf8, out JsonTape? tape, out _) ) { tape.Dispose(); }
                                },
                                $"iteration {i}: {json}");
        }
    }

    [Test] public void RandomBytes_NeverThrow()
    {
        Random random = new(1024);
        byte[] buffer = new byte[64];

        for ( int i = 0; i < ITERATIONS; i++ )
        {
            Span<byte> bytes = buffer.AsSpan(0, random.Next(buffer.Length));
            random.NextBytes(bytes);
            byte[] copy = bytes.ToArray();

            Assert.DoesNotThrow(() =>
                                {
                                    Invoice.TryFromJson(copy.AsSpan(), out _);
                                    if ( JsonTape.TryParse(copy, out JsonTape? tape, out _) ) { tape.Dispose(); }
                                },
                                $"iteration {i}: {Convert.ToHexString(copy)}");
        }
    }

    [Test] public void SkippingNeverRecurses()
    {
        // Mixed nesting deeper than any stack would allow if skipping recursed.
        StringBuilder deep = new();

        for ( int i = 0; i < 200_000; i++ )
        {
            deep.Append(i % 2 == 0
                            ? "["
                            : "{\"a\":");
        }

        Assert.That(Bag.TryFromJson("{\"x\":" + deep + "}", out _), Is.False);
    }


    private static string Mutate( string json, Random random )
    {
        char[] chars = json.ToCharArray();
        int    edits = random.Next(1, 4);

        for ( int e = 0; e < edits; e++ )
        {
            int position = random.Next(chars.Length);

            switch ( random.Next(3) )
            {
                case 0:
                    chars[position] = __alphabet[random.Next(__alphabet.Length)];
                    break;

                case 1:
                    chars = [.. chars.AsSpan(0, position), __alphabet[random.Next(__alphabet.Length)], .. chars.AsSpan(position)];
                    break;

                default:
                    if ( chars.Length > 1 ) { chars = [.. chars.AsSpan(0, position), .. chars.AsSpan(position + 1)]; }

                    break;
            }
        }

        return new string(chars);
    }
}
