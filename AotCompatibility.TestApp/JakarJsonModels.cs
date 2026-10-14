// Jakar.Json models for the Native AOT smoke test: every check in JakarJsonChecks runs in the published native executable.

using Jakar.Extensions;
using Jakar.Json;



namespace AotCompatibility.TestApp;


public enum Stage
{
    Draft,
    Final
}



[JsonModel(Naming = JsonNaming.CamelCase)]
public sealed partial record Order( Guid Id, decimal Total )
{
    public required List<Line>              Lines   { get; init; }
    public          Stage                   Stage   { get; init; }
    public          Dictionary<string, int> Tags    { get; init; } = [];
    public          DateTimeOffset          Created { get; init; }
}



[JsonModel(Naming = JsonNaming.CamelCase)] public sealed partial record Line( string Sku, int Quantity );



[JsonModel] [JsonDerived(typeof(Circle), "circle")] [JsonDerived(typeof(Square), "square")] public abstract partial class Shape;



[JsonModel]
public sealed partial class Circle : Shape
{
    public double Radius { get; set; }
}



[JsonModel]
public sealed partial class Square : Shape
{
    public double Side { get; set; }
}



public static class JakarJsonChecks
{
    public static IEnumerable<(string Name, Func<bool> Test)> All()
    {
        Order order = new(Guid.NewGuid(), 12.50m)
                      {
                          Lines = [new Line("a", 2)],
                          Stage = Stage.Final,
                          Tags = new Dictionary<string, int>
                                 {
                                     ["b"] = 2,
                                     ["a"] = 1
                                 },
                          Created = DateTimeOffset.UnixEpoch
                      };

        yield return ( "Jakar.Json round trip (UTF-16)", () => Order.FromJson(order.ToJson()).Lines[0].Sku      == "a" );
        yield return ( "Jakar.Json round trip (UTF-8)", () => Order.FromJson(order.ToJsonUtf8().AsSpan()).Stage == Stage.Final );
        yield return ( "Jakar.Json sorted keys", () => order.ToJson().Contains("\"tags\" : {\"a\" : 1,\"b\" : 2}") );
        yield return ( "Jakar.Json ISpanParsable", () => Parse<Order>(order.ToJson()).Total == 12.50m );
        yield return ( "Jakar.Json polymorphism", () => Shape.FromJson(new Square { Side = 2 }.ToJson()) is Square { Side: 2 } );
        yield return ( "Jakar.Json NDJSON", () => JsonCodec.ReadLines<Line>(new MemoryStream("{\"sku\":\"x\",\"quantity\":1}\n{\"sku\":\"y\",\"quantity\":2}"u8.ToArray())).Count() == 2 );

        yield return ( "Jakar.Json tape", () =>
                                          {
                                              using JsonTape tape = JsonTape.Parse(order.ToJsonUtf8());
                                              return tape.Root["lines"][0]["quantity"].GetInt32() == 2 && tape.Root.Deserialize<Order>().Total == 12.50m;
                                          } );

        yield return ( "Jakar.Json DOM", () =>
                                         {
                                             JObjectNode node = JObjectNode.Parse(order.ToJson());
                                             node["stage"] = "Draft";
                                             return node.ToModel<Order>().Stage == Stage.Draft && JNode.DeepEquals(JNode.FromModel(order), JNode.Parse(order.ToJson()));
                                         } );

        yield return ( "Jakar.Json malformed input", () => !Order.TryFromJson("{\"id\":", out _) );

        // Jakar.Extensions' user models are hybrid [JsonModel]s: IJsonModel's members run on Jakar.Json (static virtual dispatch through IJsonModel<TSelf>), JsonTypeInfo stays
        // System.Text.Json's. Covers init-only members of generic bases ([UnsafeAccessor] through the open definition), observable collections (IJsonCollection) and the
        // Dictionary<string, JsonElement> extension-data bag.
        yield return ( "[JsonModel] hybrid: Jakar.Json behind IJsonModel", () =>
                                                                           {
                                                                               Jakar.Extensions.UserGuid.UserModel user = new("Ada", "Lovelace") { UserName = "ada" };
                                                                               user.Addresses.Add(new Jakar.Extensions.UserGuid.UserAddress("1 Main St", "", "Springfield", "IL", "62701", "US", Guid.NewGuid()));

                                                                               string                              json  = user.ToJson(indented: false);
                                                                               Jakar.Extensions.UserGuid.UserModel back  = Jakar.Extensions.UserGuid.UserModel.FromJson(json[..^1] + ",\"extra\":[1,2.50]}");
                                                                               Jakar.Extensions.UserGuid.UserModel stj   = System.Text.Json.JsonSerializer.Deserialize(json, Jakar.Extensions.UserGuid.UserModel.JsonTypeInfo)!;

                                                                               return json == JsonCodec.ToJson(user, JsonWriterOptions.Default)
                                                                                   && back.Addresses.Single().ID == user.Addresses.Single().ID
                                                                                   && stj.UserName == "ada"
                                                                                   && back.AdditionalData!["extra"][1].GetRawText() == "2.50"
                                                                                   && back.ToJson(indented: false).EndsWith("\"extra\" : [1,2.50]}", StringComparison.Ordinal);
                                                                           } );
    }

    private static T Parse<T>( string text )
        where T : ISpanParsable<T> => T.Parse(text, null);
}
