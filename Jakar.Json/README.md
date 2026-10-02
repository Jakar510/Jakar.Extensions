# Jakar.Json

Native AOT friendly System.Text.Json models.

```csharp
using System.Text.Json.Serialization;
using Jakar.Extensions;

[JsonSerializable(typeof(Invoice))]
public sealed partial class AppJsonContext : JsonSerializerContext;

[JsonModel(typeof(AppJsonContext))]
public sealed partial class Invoice
{
    public decimal Total { get; init; }
}

Invoice invoice = Invoice.FromJson(json);   // generated, uses AppJsonContext.Default.Invoice
string  text    = invoice.ToJson();
```

- `[JsonModel]` implements `IJsonModel<TSelf>` on a `partial` class, record or struct: the `JsonTypeInfo` property, `FromJson` / `TryFromJson` / `FromJsonAsync`, and a registration in `JsonModelRegistry`.
- The type must be registered in its context (`[JsonSerializable(typeof(Invoice))]`). The System.Text.Json generator can't see other generators' output, so this can't be done for you; diagnostic **JAKAR_JSON001** has a code fix that adds it.
- `[assembly: JsonModelContext(typeof(AppJsonContext))]` sets a default context so `[JsonModel]` can be used without arguments.
- Root-level JSON arrays are read and written element by element with only the element's metadata, so `List<T>`, `T[]`, `ImmutableArray<T>`, ... never need registering: `JsonModel.FromJsonList(json, Invoice.JsonTypeInfo)`, `invoices.ToJson()`, or `JsonModel.FromJsonArrayPooled(...)` for a pooled buffer (`RentedArray<T>`).
- `SerializeAsStringJsonConverter<T>` serializes any `ISpanParsable<T>` + `ISpanFormattable` type as a JSON string.

## Diagnostics

| ID | Severity | Meaning |
|---|---|---|
| JAKAR_JSON001 | Error | The type isn't registered in its context (code fix adds `[JsonSerializable(typeof(T))]`) |
| JAKAR_JSON002 | Error | The type, or a containing type, isn't `partial` |
| JAKAR_JSON003 | Error | Open generic types can't be source generated; put `[JsonModel]` on the closed derived type, or implement `JsonTypeInfo` with `JsonModel.GetRequiredTypeInfo<T>()` |
| JAKAR_JSON004 | Error | The context isn't a `JsonSerializerContext`, or none was given and there's no assembly default |
| JAKAR_JSON005 | Warning | The type hand-writes `JsonTypeInfo`; the generator leaves it alone |
| JAKAR_JSON006 | Error | The type lists `IJsonModel<T>` without `[JsonModel]` and doesn't implement it (code fix adds `[JsonModel]`) |
| JAKAR_JSON007 | Warning | An `IJsonModel.AdditionalData` implementation lacks `[JsonExtensionData]`, so unknown members are dropped |

Types live in the `Jakar.Extensions` namespace.
