// Jakar.Json.Tests
// 10/07/2026

namespace Jakar.Json.Tests;


[TestFixture]
public sealed class FeatureTests
{
    // ─── Settings ────────────────────────────────────────────────────────────

    [Test] public void SnakeCase_AndOmittedNullsAndDefaults()
    {
        Assert.That(new SnakeOmit
                    {
                        HTTPStatusCode = 404,
                        DisplayName    = "x",
                        Score          = 1.5
                    }.ToJson(),
                    Is.EqualTo("""{"http_status_code" : 404,"display_name" : "x","score" : 1.5}"""));

        Assert.That(new SnakeOmit().ToJson(), Is.EqualTo("{}"));
    }

    [Test] public void Lenient_IgnoresCase_LastWins_NumbersFromStrings_Comments_TrailingCommas()
    {
        const string JSON = """
                            {
                                // a comment
                                "COUNT": "5",
                                "ratio": "0.5",
                                "name": "first",
                                "Name": "second", /* last wins */
                            }
                            """;

        Lenient value = Lenient.FromJson(JSON);
        Assert.That(value.Count,                                         Is.EqualTo(5));
        Assert.That(value.Ratio,                                         Is.EqualTo(0.5));
        Assert.That(value.Name,                                          Is.EqualTo("second"));
        Assert.That(Lenient.FromJson(Encoding.UTF8.GetBytes(JSON)).Name, Is.EqualTo("second"), "UTF-8 ignore-case matching");
    }

    [Test] public void LargeIntegers_NonFinite_EnumNumbers()
    {
        Numeric value = new()
                        {
                            Big    = long.MaxValue,
                            Small  = 42,
                            Weird  = double.NegativeInfinity,
                            Status = Status.Paid
                        };

        string json = value.ToJson();

        Assert.That(json, Is.EqualTo("""{"Big" : "9223372036854775807","Small" : 42,"Weird" : "-Infinity","Status" : 2}"""));

        Numeric back = Numeric.FromJson(json);
        Assert.That(back.Big,                                      Is.EqualTo(long.MaxValue));
        Assert.That(back.Weird,                                    Is.EqualTo(double.NegativeInfinity));
        Assert.That(back.Status,                                   Is.EqualTo(Status.Paid));
        Assert.That(Numeric.FromJson("""{"Weird":"NaN"}""").Weird, Is.NaN);
    }

    [Test] public void NaN_WithoutAsString_IsAWriteError() { Assert.Throws<JsonWriteException>(() => new Scalars { Double = double.NaN }.ToJson()); }

    [Test] public void TypeDefaults_ApplyToItsOwnOutput()
    {
        Pretty value = new() { Values = [1, 2] };
        Assert.That(value.ToJson(),            Is.EqualTo("{\n    \"Values\" : [\n        1,\n        2\n    ]\n}"));
        Assert.That(value.ToString(),          Is.EqualTo("custom"),                 "GenerateToString = Off keeps the user's ToString");
        Assert.That(value.ToString("c", null), Is.EqualTo("""{"Values" : [1,2]}"""), "format c = compact");
    }


    // ─── Polymorphism ────────────────────────────────────────────────────────

    [Test] public void Polymorphism_WritesTheDiscriminatorFirst_AndReadsAnyOrder()
    {
        Zoo zoo = new()
                  {
                      Animals =
                      [
                          new Dog
                          {
                              Name    = "Rex",
                              GoodBoy = true
                          },
                          new Cat { Name = "Tom" },
                          new Puppy
                          {
                              Name  = "Bit",
                              Weeks = 8
                          }
                      ],
                      Star = new Cat
                             {
                                 Name  = "Star",
                                 Lives = 3
                             }
                  };

        string json = zoo.ToJson();
        Assert.That(json, Is.EqualTo("""{"Animals" : [{"kind" : "dog","Name" : "Rex","GoodBoy" : true},{"kind" : "cat","Name" : "Tom","Lives" : 9},{"kind" : "puppy","Name" : "Bit","GoodBoy" : false,"Weeks" : 8}],"Star" : {"kind" : "cat","Name" : "Star","Lives" : 3}}"""));

        Zoo back = Zoo.FromJson(json);
        Assert.That(back.Animals[0],                  Is.TypeOf<Dog>());
        Assert.That(back.Animals[1],                  Is.TypeOf<Cat>());
        Assert.That(back.Animals[2],                  Is.TypeOf<Puppy>());
        Assert.That(( (Puppy)back.Animals[2] ).Weeks, Is.EqualTo(8));
        Assert.That(back.ToJson(),                    Is.EqualTo(json));

        // The discriminator doesn't have to come first when reading.
        Animal late = Animal.FromJson("""{"Name":"Late","Lives":2,"kind":"cat"}""");
        Assert.That(late,                Is.TypeOf<Cat>());
        Assert.That(( (Cat)late ).Lives, Is.EqualTo(2));
    }

    [Test] public void Polymorphism_UnknownOrMissingDiscriminator_Fails()
    {
        Assert.That(Animal.TryFromJson("""{"kind":"cow","Name":"x"}""", out _), Is.False);
        Assert.That(Animal.TryFromJson("""{"Name":"x"}""",              out _), Is.False,        "abstract base without a discriminator");
        Assert.That(Cat.TryFromJson("""{"kind":"dog","Name":"x"}""", out _),    Is.False,        "a derived type checks its own tag");
        Assert.That(Cat.FromJson("""{"Name":"x"}""").Name,                      Is.EqualTo("x"), "a derived type doesn't need the tag");
    }


    // ─── Extension data, converters, nodes ───────────────────────────────────

    [Test] public void ExtensionData_CapturesUnknownMembers_InOrder()
    {
        const string JSON = """{"Title" : "t","z" : [1,2],"a" : {"b" : null}}""";

        Bag bag = Bag.FromJson(JSON);
        Assert.That(bag.Extra!.Count,       Is.EqualTo(2));
        Assert.That(bag.Extra.GetAt(0).Key, Is.EqualTo("z"));
        Assert.That(bag.ToJson(),           Is.EqualTo(JSON));
    }

    [Test] public void CustomConverter_AndNodeMembers()
    {
        Converted value = new()
                          {
                              When  = DateTimeOffset.FromUnixTimeSeconds(1_700_000_000),
                              Maybe = null,
                              Any   = JNode.Parse("""{"x":[1,"two",null]}"""),
                              Array = new JArrayNode(1, true, "s")
                          };

        string json = value.ToJson();
        Assert.That(json, Is.EqualTo("""{"When" : 1700000000,"Maybe" : null,"Any" : {"x" : [1,"two",null]},"Array" : [1,true,"s"]}"""));

        Converted back = Converted.FromJson(json);
        Assert.That(back.When,                                        Is.EqualTo(value.When));
        Assert.That(back.Maybe,                                       Is.Null);
        Assert.That(JNode.DeepEquals(back.Any, value.Any),            Is.True);
        Assert.That(back.ToJson(),                                    Is.EqualTo(json));
        Assert.That(Converted.TryFromJson("""{"Array":{}}""", out _), Is.False, "a JArrayNode member rejects an object");
    }


    // ─── Structs, generics, constructors, fields ─────────────────────────────

    [Test] public void Struct_WithFields()
    {
        Point point = new()
                      {
                          X = 3,
                          Y = 4
                      };

        Assert.That(point.ToJson(), Is.EqualTo("""{"X" : 3,"Y" : 4,"Sum" : 7}"""));

        Point back = Point.FromJson("""{"X":3,"Y":4,"Sum":100}""");
        Assert.That(back.X + back.Y, Is.EqualTo(7), "Sum is computed: written, never read");
    }

    [Test] public void RecordStruct_WithOptionalConstructorParameter()
    {
        Assert.That(Money.FromJson("""{"Amount":1.5}""").Currency,         Is.EqualTo("USD"));
        Assert.That(Money.FromJson("""{"Amount":1.5,"Currency":"EUR"}"""), Is.EqualTo(new Money(1.5m, "EUR")));
        Assert.That(Money.TryFromJson("""{"Currency":"EUR"}""", out _),    Is.False, "Amount is required");
    }

    [Test] public void GenericModels_DispatchStatically()
    {
        Page<LineItem> page = new()
                              {
                                  Items = [new LineItem("a", 1, 1m)],
                                  Total = 1
                              };

        string json = page.ToJson();

        Assert.That(Page<LineItem>.FromJson(json).Items[0].Sku, Is.EqualTo("a"));

        Keyed<Guid> keyed = new()
                            {
                                Single = Guid.Empty,
                                Counts = new Dictionary<string, int> { ["x"] = 1 }
                            };

        Assert.That(Keyed<Guid>.FromJson(keyed.ToJson()).Single, Is.EqualTo(Guid.Empty));
    }

    [Test] public void JsonConstructor_AndRequiredAttribute()
    {
        Account account = Account.FromJson("""{"Login":"ada","Age":36,"Email":"a@b.c"}""");
        Assert.That(account.Login,                                              Is.EqualTo("ada"));
        Assert.That(account.Age,                                                Is.EqualTo(36));
        Assert.That(Account.TryFromJson("""{"Login":"ada","Age":36}""", out _), Is.False, "Email is [JsonMember(Required = true)]");
    }

    [Test] public void SystemTextJsonAttributes_AreHonored()
    {
        Migrated value = new()
                         {
                             Name     = "Ada",
                             First    = 1,
                             Secret   = "s",
                             Optional = null,
                             Internal = 7,
                             Both     = 3
                         };

        string json = value.ToJson();

        Assert.That(json, Is.EqualTo("""{"First" : 1,"full_name" : "Ada","Internal" : 7,"wins" : 3}"""));

        Migrated back = Migrated.FromJson(json);
        Assert.That(back.Secret,   Is.EqualTo("hidden"));
        Assert.That(back.Internal, Is.EqualTo(7));
    }

    [Test] public void NestedType_KebabCase()
    {
        Outer.Inner inner = new("a", null);
        Assert.That(inner.ToJson(),                                                   Is.EqualTo("""{"first-value" : "a","second-value" : null}"""));
        Assert.That(Outer.Inner.FromJson("""{"first-value":"a","second-value":2}"""), Is.EqualTo(new Outer.Inner("a", 2)));
    }


    // ─── ISpanFormattable / ISpanParsable ────────────────────────────────────

    [Test] public void SpanFormattable_WritesIntoTheCallerBuffer()
    {
        LineItem   item   = new("a", 1, 2m);
        Span<char> buffer = stackalloc char[256];

        Assert.That(item.TryFormat(buffer, out int written, default, null), Is.True);
        Assert.That(buffer[..written].ToString(),                           Is.EqualTo(item.ToJson()));

        Assert.That(item.TryFormat(stackalloc char[10], out written, default, null), Is.False);
        Assert.That(written,                                                         Is.Zero);

        Span<byte> bytes = stackalloc byte[256];
        Assert.That(item.TryFormat(bytes, out int bytesWritten, "i", null), Is.True);
        Assert.That(Encoding.UTF8.GetString(bytes[..bytesWritten]),         Is.EqualTo(item.ToJson(JsonWriterOptions.Indent)));
    }

    [Test] public void SpanParsable_GenericCode()
    {
        Assert.That(Parse<LineItem>("""{"sku":"a","quantity":1,"price":2}""").Sku,  Is.EqualTo("a"));
        Assert.That(LineItem.TryParse("nope", CultureInfo.InvariantCulture, out _), Is.False);
        Assert.Throws<JsonReadException>(() => LineItem.Parse("{", null));

        static T Parse<T>( string text )
            where T : ISpanParsable<T> => T.Parse(text.AsSpan(), CultureInfo.InvariantCulture);
    }

    [Test] public void FormatString_Rejected() { Assert.Throws<FormatException>(() => new LineItem("a", 1, 1m).ToString("x", null)); }
}
