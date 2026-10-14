// Jakar.Json.AspNetCore.Tests
// 10/14/2026

using NUnit.Framework;



namespace Jakar.Json.AspNetCore.Tests;


/// <summary> The README's camelCase recipe: what MVC's System.Text.Json defaults send is read, and the names are written camelCase. </summary>
[TestFixture]
public sealed class NamingTests
{
    [Test] public void CamelCaseRecipe_ReadsAndWritesCamelCase()
    {
        CamelOrder order = JsonCodec.FromJson<CamelOrder>("""{"orderId":3,"CUSTOMERNAME":"Ada"}""");

        Assert.That(order.OrderId,      Is.EqualTo(3));
        Assert.That(order.CustomerName, Is.EqualTo("Ada"));
        Assert.That(JsonCodec.ToJson(order), Does.Contain("\"orderId\"").And.Contain("\"customerName\""));
    }
}



[Jakar.Extensions.JsonModel(Naming = JsonNaming.CamelCase, NameMatching = JsonNameMatching.OrdinalIgnoreCase)]
public sealed partial class CamelOrder
{
    public int    OrderId      { get; set; }
    public string CustomerName { get; set; } = "";
}
