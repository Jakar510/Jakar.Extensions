// Jakar.Json.Tests
// 10/07/2026

using System.Numerics;



namespace Jakar.Json.Tests;


[TestFixture]
public sealed class ModelTests
{
    internal static Invoice SampleInvoice() => new(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"), 1.10m)
                                               {
                                                   Lines   = [new LineItem("A-1", 3, 0.25m), new LineItem("B\"2", 1, 10m)],
                                                   Notes   = "paid in full",
                                                   Status  = Status.Paid,
                                                   Created = new DateTimeOffset(2026, 10, 6, 21, 49, 53, TimeSpan.FromHours(-5))
                                               };


    [Test] public void Invoice_WritesCanonicalJson()
    {
        string json = SampleInvoice().ToJson();

        Assert.That(json, Is.EqualTo("""{"memo" : "paid in full","id" : "0f8fad5b-d9cb-469f-a165-70867728950e","total" : 1.10,"lines" : [{"sku" : "A-1","quantity" : 3,"price" : 0.25},{"sku" : "B\"2","quantity" : 1,"price" : 10}],"status" : "Paid","created" : "2026-10-06T21:49:53.0000000-05:00"}"""));
    }

    [Test] public void Invoice_RoundTrips_Utf16_And_Utf8()
    {
        Invoice invoice = SampleInvoice();

        Invoice fromText  = Invoice.FromJson(invoice.ToJson());
        Invoice fromBytes = Invoice.FromJson(invoice.ToJsonUtf8().AsSpan());

        AssertSame(invoice, fromText);
        AssertSame(invoice, fromBytes);
    }

    [Test] public void Utf8Output_IsTheUtf8OfUtf16Output()
    {
        Invoice invoice = SampleInvoice();
        Assert.That(invoice.ToJsonUtf8(), Is.EqualTo(Encoding.UTF8.GetBytes(invoice.ToJson())));
    }

    [Test] public void Indented_UsesTabsAndSpacedSeparator()
    {
        string json = new LineItem("A", 1, 2m).ToJson(JsonWriterOptions.Indent);
        Assert.That(json, Is.EqualTo("{\n\t\"sku\" : \"A\",\n\t\"quantity\" : 1,\n\t\"price\" : 2\n}"));

        string spaces = new LineItem("A", 1, 2m).ToJson(new JsonWriterOptions
                                                        {
                                                            Indented   = true,
                                                            IndentChar = JsonIndentChar.Space,
                                                            IndentSize = 2
                                                        });

        Assert.That(spaces, Is.EqualTo("{\n  \"sku\" : \"A\",\n  \"quantity\" : 1,\n  \"price\" : 2\n}"));
    }

    [Test] public void Invoice_RequiredMembers_AreEnforced()
    {
        Assert.That(Invoice.TryFromJson("""{"id":"0f8fad5b-d9cb-469f-a165-70867728950e","total":1}""", out _), Is.False, "lines is required");

        JsonReadException error = Assert.Throws<JsonReadException>(() => Invoice.FromJson("""{"id":"0f8fad5b-d9cb-469f-a165-70867728950e","lines":[]}"""))!;
        Assert.That(error.Error.Kind, Is.EqualTo(JsonErrorKind.MissingRequired));
    }

    [Test] public void Invoice_UnknownMember_IsAnError()
    {
        JsonReadException error = Assert.Throws<JsonReadException>(() => Invoice.FromJson("""{"id":"0f8fad5b-d9cb-469f-a165-70867728950e","total":1,"lines":[],"extra":1}"""))!;
        Assert.That(error.Error.Kind, Is.EqualTo(JsonErrorKind.UnknownMember));
    }

    [Test] public void DuplicateMember_IsAnError_WithPath()
    {
        JsonReadException error = Assert.Throws<JsonReadException>(() => LineItem.FromJson("""{"sku":"a","quantity":1,"price":1,"lines":[{"x":1}],"sku":"b"}"""))!;
        Assert.That(error.Error.Kind, Is.EqualTo(JsonErrorKind.DuplicateMember));
        Assert.That(error.Path,       Is.EqualTo("$.sku"));
    }

    [Test] public void ErrorPath_PointsIntoNestedArrays()
    {
        JsonReadException error = Assert.Throws<JsonReadException>(() => Invoice.FromJson("""{"id":"0f8fad5b-d9cb-469f-a165-70867728950e","total":1,"lines":[{"sku":"a","quantity":1,"price":1},{"sku":"b","quantity":"x","price":1}]}"""))!;
        Assert.That(error.Path,       Is.EqualTo("$.lines[1].quantity"));
        Assert.That(error.Error.Line, Is.EqualTo(1));
    }

    [Test] public void MissingOptionalMembers_KeepTheirInitializers()
    {
        Scalars value = Scalars.FromJson("{}");

        Assert.That(value.Char, Is.EqualTo('x'));

        Assert.That(value.Defaulted,
                    Is.EqualTo(new List<int>
                               {
                                   1,
                                   2,
                                   3
                               }));

        Assert.That(value.Text, Is.EqualTo(""));
    }

    [Test] public void Scalars_RoundTrip_AllTypes()
    {
        Scalars value = new()
                        {
                            Flag     = true,
                            Byte     = 255,
                            SByte    = -128,
                            Short    = short.MinValue,
                            UShort   = ushort.MaxValue,
                            Int      = int.MinValue,
                            UInt     = uint.MaxValue,
                            Long     = long.MinValue,
                            ULong    = ulong.MaxValue,
                            Int128   = Int128.MinValue,
                            UInt128  = UInt128.MaxValue,
                            Big      = BigInteger.Pow(10, 60) + 7,
                            Single   = 0.1f,
                            Double   = 1e20,
                            Half     = (Half)1.5,
                            Decimal  = 1.10m,
                            Char     = 'é',
                            Text     = "tab\t\"quote\" \\ \u0001 😀 \ud800",
                            Maybe    = null,
                            Guid     = Guid.NewGuid(),
                            DateTime = new DateTime(2026, 10, 7, 1, 2, 3, DateTimeKind.Utc),
                            Offset   = DateTimeOffset.UnixEpoch.ToOffset(TimeSpan.FromHours(3)),
                            Date     = new DateOnly(2026, 10, 7),
                            Time     = new TimeOnly(23, 59, 58, 123),
                            Span     = new TimeSpan(1, 2, 3, 4, 500),
                            Link     = new Uri("https://example.com/a?b=c"),
                            Version  = new Version(1, 2, 3, 4),
                            Status   = Status.Void,
                            Rights   = Permissions.Read | Permissions.Delete,
                            NullInt  = 5,
                            NullEnum = null
                        };

        string  json = value.ToJson();
        Scalars back = Scalars.FromJson(json);

        Assert.That(back.ToJson(),                                       Is.EqualTo(json), "fixed point (I4)");
        Assert.That(back.Big,                                            Is.EqualTo(value.Big));
        Assert.That(back.Text,                                           Is.EqualTo(value.Text), "escapes and a lone surrogate round-trip");
        Assert.That(back.Int128,                                         Is.EqualTo(value.Int128));
        Assert.That(back.UInt128,                                        Is.EqualTo(value.UInt128));
        Assert.That(back.Single,                                         Is.EqualTo(value.Single));
        Assert.That(back.Decimal.ToString(CultureInfo.InvariantCulture), Is.EqualTo("1.10"), "decimal keeps its scale");
        Assert.That(back.DateTime.Kind,                                  Is.EqualTo(DateTimeKind.Utc));
        Assert.That(back.Rights,                                         Is.EqualTo(value.Rights));
        Assert.That(back.NullInt,                                        Is.EqualTo(5));
        Assert.That(json,                                                Does.Contain("\"Double\" : 1e+20"));
        Assert.That(json,                                                Does.Contain("\"Rights\" : \"Read, Delete\""));
        Assert.That(json,                                                Does.Contain("\\ud800").IgnoreCase);
        Assert.That(json,                                                Does.Contain("\\u0001"));
    }

    [Test] public void Flags_UseNamesInBitOrder_AndOverrides()
    {
        Scalars value = new() { Rights = Permissions.All };
        Assert.That(value.ToJson(), Does.Contain("\"Rights\" : \"everything\""));

        Assert.That(Scalars.FromJson("""{"Rights":"Write, Read"}""").Rights, Is.EqualTo(Permissions.Read  | Permissions.Write));
        Assert.That(Scalars.FromJson("""{"Rights":6}""").Rights,             Is.EqualTo(Permissions.Write | Permissions.Delete));
        Assert.That(Scalars.TryFromJson("""{"Rights":"Nope"}""", out _),     Is.False);
    }

    [Test] public void LocalDateTime_IsWrittenAsUtc()
    {
        DateTime local = new(2026, 10, 7, 12, 0, 0, DateTimeKind.Local);
        Scalars  back  = Scalars.FromJson(new Scalars { DateTime = local }.ToJson());

        Assert.That(back.DateTime.Kind, Is.EqualTo(DateTimeKind.Utc));
        Assert.That(back.DateTime,      Is.EqualTo(local.ToUniversalTime()));
    }

    [Test] public void Collections_RoundTrip_AndUnorderedOnesAreSorted()
    {
        Collections value = new()
                            {
                                Array         = [3, 1, 2],
                                List          = ["b", "a"],
                                Enumerable    = [7, 8],
                                ReadOnlyList  = [9],
                                Immutable     = [4, 5],
                                ImmutableList = [6],
                                Set           = ["z", "a", "m"],
                                Frozen        = new[] { 30, 10, 20 }.ToFrozenSet(),
                                Sorted        = [3, 1],
                                Queue         = new Queue<int>([1, 2]),
                                Stack         = new Stack<int>([1, 2, 3]),
                                Map = new Dictionary<string, int>
                                      {
                                          ["b"] = 2,
                                          ["a"] = 1
                                      },
                                GuidMap = new Dictionary<Guid, string> { [Guid.Empty] = "zero" },
                                EnumMap = new Dictionary<Status, LineItem>
                                          {
                                              [Status.Sent]  = new("s", 1, 1m),
                                              [Status.Draft] = new("d", 2, 2m)
                                          },
                                SortedMap = new SortedDictionary<int, string>
                                            {
                                                [2] = "two",
                                                [1] = "one"
                                            },
                                OrderedMap = new OrderedDictionary<string, int>
                                             {
                                                 ["z"] = 1,
                                                 ["a"] = 2
                                             },
                                ImmutableMap = ImmutableDictionary<string, int>.Empty.Add("y", 1).Add("x", 2),
                                FrozenMap = new Dictionary<string, int>
                                            {
                                                ["q"] = 1,
                                                ["p"] = 2
                                            }.ToFrozenDictionary(),
                                ConcurrentMap = new ConcurrentDictionary<string, int>(new Dictionary<string, int>
                                                                                      {
                                                                                          ["n"] = 1,
                                                                                          ["m"] = 2
                                                                                      }),
                                Nested        = [[1], [2, 3]],
                                NullableItems = ["a", null],
                                Optional      = new Dictionary<string, List<LineItem>> { ["k"] = [new LineItem("x", 1, 1m)] }
                            };

        string      json = value.ToJson();
        Collections back = Collections.FromJson(json);

        Assert.That(back.ToJson(),    Is.EqualTo(json), "fixed point (I4)");
        Assert.That(json,             Does.Contain("\"Set\" : [\"a\",\"m\",\"z\"]"));
        Assert.That(json,             Does.Contain("\"Frozen\" : [10,20,30]"));
        Assert.That(json,             Does.Contain("\"Map\" : {\"a\" : 1,\"b\" : 2}"));
        Assert.That(json,             Does.Contain("\"OrderedMap\" : {\"z\" : 1,\"a\" : 2}"), "ordered maps keep insertion order");
        Assert.That(json,             Does.Contain("\"EnumMap\" : {\"Draft\" : "));
        Assert.That(json,             Does.Contain("\"Stack\" : [3,2,1]"));
        Assert.That(back.Stack.Pop(), Is.EqualTo(3), "stacks round-trip");

        Assert.That(back.NullableItems,
                    Is.EqualTo(new List<string?>
                               {
                                   "a",
                                   null
                               }));

        Assert.That(back.GuidMap[Guid.Empty],   Is.EqualTo("zero"));
        Assert.That(back.Optional!["k"][0].Sku, Is.EqualTo("x"));
    }

    [Test] public void EqualCollections_WriteTheSameBytes_RegardlessOfInsertionOrder()
    {
        Collections first = new()
                            {
                                Set = ["a", "b", "c"],
                                Map = new Dictionary<string, int>
                                      {
                                          ["a"] = 1,
                                          ["b"] = 2
                                      }
                            };

        Collections second = new()
                             {
                                 Set = ["c", "b", "a"],
                                 Map = new Dictionary<string, int>
                                       {
                                           ["b"] = 2,
                                           ["a"] = 1
                                       }
                             };

        Assert.That(second.ToJson(), Is.EqualTo(first.ToJson()));
    }

    [Test] public void DictionaryDuplicateKey_IsAnError() { Assert.That(Collections.TryFromJson("""{"Map":{"a":1,"a":2}}""", out _), Is.False); }

    [Test] public void NonNullableMember_RejectsNull()
    {
        Assert.That(Collections.TryFromJson("""{"List":null}""", out _),                            Is.False);
        Assert.That(Scalars.TryFromJson("""{"Maybe":null}""", out Scalars? ok) && ok.Maybe is null, Is.True);
    }


    internal static void AssertSame( Invoice expected, Invoice actual )
    {
        Assert.That(actual.Id,      Is.EqualTo(expected.Id));
        Assert.That(actual.Total,   Is.EqualTo(expected.Total));
        Assert.That(actual.Notes,   Is.EqualTo(expected.Notes));
        Assert.That(actual.Status,  Is.EqualTo(expected.Status));
        Assert.That(actual.Created, Is.EqualTo(expected.Created));
        Assert.That(actual.Lines,   Is.EqualTo(expected.Lines));
    }
}
