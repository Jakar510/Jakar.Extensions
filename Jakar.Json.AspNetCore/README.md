# Jakar.Json.AspNetCore

Makes [Jakar.Json](../Jakar.Json/README.md) ASP.NET Core's JSON serializer for `[JsonModel]` types. In benchmarks it runs 1.2–1.9× faster than source-generated System.Text.Json and allocates less (SPEC §12.1). Every other type keeps using System.Text.Json, so you can adopt it one model at a time.

Covered types are:
- non-generic `[JsonModel]` types, registered automatically by the generator;
- arrays, `List<T>`, `IEnumerable<T>`, `IReadOnlyList<T>`, `IReadOnlyCollection<T>`, `IList<T>` and `ICollection<T>` of them.

## MVC controllers

```csharp
builder.Services.AddControllers().AddJakarJson();
```

That's all. Action parameters and results of `[JsonModel]` types are read and written by Jakar.Json:

```csharp
[HttpPost] public ActionResult<Order> Create( Order order ) => order;   // body in, body out
[HttpGet]  public ActionResult<List<Order>> All() => orders;
```

- **Formatter order.** The formatters go just before System.Text.Json's. MVC's own handling still comes first: `null` is a 204, and strings and streams go to their own formatters.
- **Bound from the body.** `[JsonModel]` types implement `ISpanParsable<T>`, so MVC would otherwise bind them from the query string. `AddJakarJson` makes those parameters bind from the body. `[FromQuery]` and the other binding attributes still win.
- **Malformed JSON** becomes a model-state error keyed by its JSON path (`$.Lines[0].Price`), so `[ApiController]` returns a 400 problem response, as it does with System.Text.Json.
- **Charsets.** Jakar.Json reads UTF-8. A request body in any other charset goes to System.Text.Json's formatter, which transcodes it.

## Minimal APIs

Minimal APIs always use System.Text.Json for request bodies and plain return values. Opt in per endpoint or per group:

```csharp
builder.Services.AddJakarJson();   // optional: JakarJsonOptions

RouteGroupBuilder api = app.MapGroup("/api").WithJakarJson();   // plain return values → Jakar.Json

api.MapGet("/orders/{id:int}", ( int id ) => store.Find(id));                   // returns Order
api.MapPost("/orders", ( JsonBody<Order> order ) => JakarResults.Created($"/api/orders/{order.Value.Id}", order.Value));
api.MapGet("/orders", () => JakarResults.JsonArray(store.All()));
```

- **`WithJakarJson()`** is an endpoint filter. When an endpoint returns a value (not an `IResult`) whose runtime type has a codec, Jakar.Json writes it. OpenAPI still sees the handler's declared return type.
- **`JsonBody<T>`** reads a request body with Jakar.Json, through minimal APIs' `BindAsync`, and tells OpenAPI the endpoint accepts JSON.
  - Errors: 415 if the request isn't JSON, 413 past `MaxDocumentBytes`, 400 for malformed JSON.
  - An empty body is a 400, unless the parameter is nullable (`JsonBody<Order>?`).
  - Why a wrapper: a bare `Order` parameter would bind from the query string, because the type is `ISpanParsable`.
- **`JakarResults`** provides `Json`, `Ok`, `Created` and `JsonArray`. Like `TypedResults`, they describe themselves to OpenAPI.
- **HTTP helpers:** `request.ReadFromJakarJsonAsync<T>()` and `response.WriteAsJakarJsonAsync(value)`.

## Options

```csharp
builder.Services.AddControllers().AddJakarJson(options =>
{
    options.WriterOptions = new JsonWriterOptions { Indented = true };
    options.ReaderOptions = new JsonReaderOptions { AllowComments = true, MaxDocumentBytes = 1 << 20 };
});
```

Leave an option `null` to keep each type's own settings, from `[JsonModel]` and `[assembly: JsonDefaults]`. Request bodies are buffered up to `MaxDocumentBytes`, then parsed in one pass. A larger body is a 413.

## Names and casing

A context-less `[JsonModel]` reads and writes the names it was compiled with. By default those are the C# names (`Id`), compared case-sensitively. MVC's System.Text.Json defaults are camelCase and case-insensitive.

To keep camelCase APIs, either:
- set the naming for the assembly (or on a model):

  ```csharp
  [assembly: JsonDefaults(Naming = JsonNaming.CamelCase, NameMatching = JsonNameMatching.OrdinalIgnoreCase)]
  ```

- or give the type a System.Text.Json context (`[JsonModel(typeof(MyContext))]`). Its options, including naming and case-insensitivity, are then mirrored into Jakar.Json.

The output also differs in whitespace: Jakar.Json writes `"name" : value`.
