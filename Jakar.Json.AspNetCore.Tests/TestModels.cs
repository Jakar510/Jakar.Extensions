// Jakar.Json.AspNetCore.Tests
// 10/14/2026

using Jakar.Extensions;



namespace Jakar.Json.AspNetCore.Tests;


[JsonModel]
public sealed partial record TestOrder
{
    public int             Id       { get; init; }
    public string          Customer { get; init; } = "";
    public decimal         Total    { get; init; }
    public List<TestLine>  Lines    { get; init; } = [];

    public bool Equals( TestOrder? other ) => other is not null && Id == other.Id && Customer == other.Customer && Total == other.Total && Lines.SequenceEqual(other.Lines);
    public override int GetHashCode() => HashCode.Combine(Id, Customer, Total);
}



[JsonModel]
public sealed partial record TestLine
{
    public string  Sku      { get; init; } = "";
    public int     Quantity { get; init; }
    public decimal Price    { get; init; }
}



/// <summary> Not a [JsonModel]: System.Text.Json handles it. </summary>
public sealed record PlainDto( int Id, string Name );



internal static class Samples
{
    public static TestOrder Order( int id = 7 ) => new()
                                                   {
                                                       Id       = id,
                                                       Customer = "Ada",
                                                       Total    = 12.50m,
                                                       Lines    = [new TestLine { Sku = "A-1", Quantity = 2, Price = 1.25m }, new TestLine { Sku = "B-2", Quantity = 1, Price = 10m }]
                                                   };
}
