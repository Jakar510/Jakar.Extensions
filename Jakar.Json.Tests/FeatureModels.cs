// Jakar.Json.Tests
// 10/07/2026

using System.Text.Json.Serialization;



namespace Jakar.Json.Tests;


// ─── Settings ────────────────────────────────────────────────────────────────



[GenerateJson(Naming = JsonNaming.SnakeCaseLower, NullValues = JsonNullValues.Omit, DefaultValues = JsonDefaultValues.Omit)]
public sealed partial class SnakeOmit
{
    public int     HTTPStatusCode { get; set; }
    public string? DisplayName    { get; set; }
    public double  Score          { get; set; }
}



[GenerateJson(NameMatching = JsonNameMatching.OrdinalIgnoreCase, DuplicateMembers = JsonDuplicateMembers.LastWins, NumbersFromStrings = JsonNumbersFromStrings.Allow, AllowComments = JsonToggle.On, AllowTrailingCommas = JsonToggle.On)]
public sealed partial class Lenient
{
    public int    Count { get; set; }
    public double Ratio { get; set; }
    public string Name  { get; set; } = "";
}



[GenerateJson(LargeIntegers = JsonLargeIntegers.String, NonFiniteFloats = JsonNonFiniteFloats.AsString, Enums = JsonEnumFormat.Number)]
public sealed partial class Numeric
{
    public long   Big    { get; set; }
    public long   Small  { get; set; }
    public double Weird  { get; set; }
    public Status Status { get; set; }
}



[GenerateJson(Indented = JsonToggle.On, IndentChar = JsonIndentChar.Space, IndentSize = 4, GenerateToString = JsonToggle.Off)]
public sealed partial class Pretty
{
    public int[] Values { get; set; } = [];

    public override string ToString() => "custom";
}



// ─── Polymorphism ────────────────────────────────────────────────────────────



[GenerateJson(Discriminator = "kind")]
[JsonDerived(typeof(Dog),   "dog")]
[JsonDerived(typeof(Cat),   "cat")]
[JsonDerived(typeof(Puppy), "puppy")]
public abstract partial class Animal
{
    public string Name { get; set; } = "";
}



[GenerateJson]
public partial class Dog : Animal
{
    public bool GoodBoy { get; set; }
}



[GenerateJson]
public sealed partial class Puppy : Dog
{
    public int Weeks { get; set; }
}



[GenerateJson]
public sealed partial class Cat : Animal
{
    public int Lives { get; set; } = 9;
}



[GenerateJson]
public sealed partial class Zoo
{
    public List<Animal> Animals { get; set; } = [];
    public Animal?      Star    { get; set; }
}



// ─── Extension data, converters, nodes ───────────────────────────────────────



[GenerateJson]
public sealed partial class Bag
{
    public string Title { get; set; } = "";

    [JsonMember(ExtensionData = true)] public JObjectNode? Extra { get; set; }
}



/// <summary> Unix seconds instead of ISO 8601. </summary>
public readonly struct UnixSecondsConverter : IJsonConverter<DateTimeOffset>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in DateTimeOffset value )
        where TWriter : IJsonWriter, allows ref struct => writer.WriteInteger(value.ToUnixTimeSeconds());

    public static bool TryRead<TReader>( ref TReader reader, out DateTimeOffset value )
        where TReader : IJsonReader, allows ref struct
    {
        value = default;
        if ( !reader.TryReadInteger(out long seconds) ) { return false; }

        value = DateTimeOffset.FromUnixTimeSeconds(seconds);
        return true;
    }
}



[GenerateJson]
public sealed partial class Converted
{
    [JsonMember(Converter = typeof(UnixSecondsConverter))] public DateTimeOffset  When  { get; set; }
    [JsonMember(Converter = typeof(UnixSecondsConverter))] public DateTimeOffset? Maybe { get; set; }
    public                                                        JNode?          Any   { get; set; }
    public                                                        JArrayNode      Array { get; set; } = [];
}



// ─── Structs, generics, fields, constructors ─────────────────────────────────



[GenerateJson]
public partial struct Point
{
    public          int X;
    public          int Y;
    public readonly int Sum => X + Y;
}



[GenerateJson] public readonly partial record struct Money( decimal Amount, string Currency = "USD" );



[GenerateJson]
public sealed partial class Page<T>
    where T : IJsonSerializable<T>
{
    public List<T> Items { get; set; } = [];
    public int     Total { get; set; }
}



[GenerateJson]
public sealed partial class Keyed<TKey>
    where TKey : ISpanFormattable, ISpanParsable<TKey>
{
    public TKey?                   Single { get; set; }
    public Dictionary<string, int> Counts { get; set; } = [];
}



[GenerateJson]
public sealed partial class Account
{
    [JsonConstructor] public Account( string login, int age )
    {
        Login = login;
        Age   = age;
    }

    public string Login { get; }
    public int    Age   { get; }

    [JsonMember(Required = true)] public string Email { get; set; } = "";
}



// ─── System.Text.Json attribute migration ────────────────────────────────────



[GenerateJson]
public sealed partial class Migrated
{
    [JsonPropertyName("full_name")] public string Name { get; set; } = "";

    [JsonPropertyOrder(-1)] public int First { get; set; }

    [JsonIgnore] public string Secret { get; set; } = "hidden";

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string? Optional { get; set; }

    [JsonInclude] internal int Internal { get; set; }

    [JsonMember(Name = "wins")] [JsonPropertyName("loses")] public int Both { get; set; }
}



// ─── Nested types ────────────────────────────────────────────────────────────



public static partial class Outer
{
    [GenerateJson(Naming = JsonNaming.KebabCaseLower)] public sealed partial record Inner( string FirstValue, int? SecondValue );
}
