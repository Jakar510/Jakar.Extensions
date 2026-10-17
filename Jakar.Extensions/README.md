# Jakar.Extensions

Extensions and helpers for .NET 10: observable and concurrent collections, HTTP, files, security, async LINQ, span and hashing helpers, users and logins, and view models. It is trim and Native AOT compatible. **Contributions and ideas are welcome.**

## Installation

```sh
dotnet add package Jakar.Extensions
```

It brings in [Jakar.Json](https://www.nuget.org/packages/Jakar.Json/) (JSON, including the `[JsonModel]` source generator) and [Jakar.Spans](https://www.nuget.org/packages/Jakar.Spans/) (`ValueStringBuilder` and other span primitives).

## JSON

Since 10.10 the library serializes with **System.Text.Json source-generated metadata, and Jakar.Json where it supports the type**: no Newtonsoft.Json and no reflection-based serialization.

- **Models** are `IJsonModel<TSelf>` types. Put `[JsonModel(typeof(YourContext))]` on a `partial` class, record or struct, and register it in your `JsonSerializerContext`. The generator implements the model's members with Jakar.Json when it supports the type (faster, fewer allocations), otherwise with System.Text.Json.

  ```csharp
  [JsonSerializable(typeof(Invoice))]
  public sealed partial class AppJsonContext : JsonSerializerContext;

  [JsonModel(typeof(AppJsonContext))]
  public sealed partial class Invoice : BaseClass<Invoice>
  {
      public decimal Total { get; init; }
  }

  Invoice invoice = Invoice.FromJson(json);
  string  text    = invoice.ToJson();
  ```

- **Other types:** register their context once at startup with `Json.AddResolver(AppJsonContext.Default)`, then use `Json.Serialize(value)` / `json.FromJson<T>()`, or pass a `JsonTypeInfo<T>` explicitly.
- **Root-level arrays** never need a collection type registered: `JsonModel.FromJsonList(json, Invoice.JsonTypeInfo)`, `invoices.ToJson()`, `WebHandler.AsJsonList(Invoice.JsonTypeInfo, token)`.
- **`JsonNode` / `JsonElement` helpers:** `Get<T>`, `TryGet<T>`, `GetOrDefault<T>`, `TryAdd`, `TryUpdate`, `Set`, `Remove`.
- **Older JSON still reads.** JSON written by 10.9 and earlier (Newtonsoft.Json) is accepted: case-insensitive property names, numbers in strings, comments and trailing commas.

### Migrating from 10.9 and earlier

- `JToken` / `JObject` / `JValue` are gone:
  - `AdditionalData` is `Dictionary<string, JsonElement>?`.
  - `ErrorResponse`, `WebHandler.AsJson()` and `FeedBackTrackerException` use `JsonNode`.
  - `ExceptionDetails.Data` is a `JsonObject`, and `LoginRequestValue.Data` is a `JsonElement`.
- `value.ToJson()` works for `IJsonModel<TSelf>` types and arrays, lists and spans of them. For anything else, use `Json.Serialize(value)`.
- Your own `IJsonModel<TSelf>` types must provide `static JsonTypeInfo<TSelf> JsonTypeInfo`: add `[JsonModel]`. Diagnostic JAKAR_JSON006 has a code fix.
- `PreferenceFile<TSelf>` stores `{TSelf}.json` with typed properties. An existing `{TSelf}.ini` is imported on first load.
- Observable collections serialize their unfiltered contents. For System.Text.Json to do the same when such a collection is a model *property*, register the closed converter in your context: `Converters = [typeof(ObservableCollectionJsonConverter<Address>)]`.
- `Json.Settings` / `Json.LoadSettings` are replaced by `Json.Options` and `Json.AddResolver`.
- `ValueStringBuilder` and `Sizes` moved to Jakar.Spans. They keep the same namespace and are type-forwarded, so no code changes.

## Base classes

- `BaseClass` / `BaseClass<TSelf>`: a model base with property change notifications (`INotifyPropertyChanging`, `INotifyPropertyChanged`), JSON extension data (`AdditionalData`), and equality and comparison for `TSelf`.
- `BaseRecord`: the record counterpart, with JSON extension data.

## Collections

- **Observable:** `ObservableCollection<T>`, `ConcurrentObservableCollection<T>`, `ObservableDictionary<TKey, TValue>`, `ObservableConcurrentDictionary<TKey, TValue>`, `ObservableHashSet<T>`. They raise change notifications and can show a filtered view (`OverrideFilter`); the list types also keep a sort order (`Comparer<T>`).
- **Queues and stacks:** `ConcurrentDeque<T>`, `FixedSizedDeque<T>`, `FixedSizedQueue<T>`, `LockFreeDeque<T>`, `LockFreeStack<T>`.
- **Other:** `ArrayBuffer<T>` (a growable buffer struct), `AsyncChannel<T>`, `ValueLinkedList<T>`, `MultiDict<TKey>`.

## HTTP

- `WebRequester`: a wrapper around `HttpClient` that builds requests against a base URI (`IHostInfo`), with retries (`RetryPolicy`).
- `WebHandler`: one request. Every `As*` call sends it, retries per the requester's policy, and returns a `WebResponse<T>`.
- `WebResponse<T>`: the result: status, value or error details.
- `Status`, `MimeType`, `MimeTypeNames`: HTTP status codes, MIME types and their names.

## Files

- `LocalFile`, `LocalDirectory`: read, write, create, delete and enumerate local files and directories.
- `LocalFileWatcher`: an observable collection of a directory's files.
- `TempFile`, `EmbeddedResources`, `FilePaths`.
- `PreferenceFile<TSelf>`: typed preferences persisted as JSON.
- `IniConfig`: parses and formats INI files.

## Security

- `SecuredString`: sensitive text kept in a `SecureString`.
- `PasswordValidator` and `PasswordRequirements`: password rules and validation.
- `OneTimePassword`: TOTP codes.
- `IDataProtector` / `IDataProtectorProvider` / `DataProtector`: encrypt and decrypt data.

## Errors

- `ErrorOrResult<T>` and `Error`: a value or errors (inspired by [ErrorOr](https://github.com/amantinband/error-or)).
- `ExceptionDetails`: exceptions as serializable data.

## Miscellaneous

- `Validate`: string and state validation.
- `Tasks`: task helpers such as `SafeFireAndForget`.
- `Types`: type inspection (`IsBuiltInType`, `IsNullableType`, `IsCollection`, `IsDictionary`, `IsList`, `IsSet`, `HasInterface`, `TryGetUnderlyingEnumType`, `MethodName`, `MethodSignature`, ...).
- `LoginRequest`, `UserModel`, `JwtParser`: users, logins and tokens.
- `AppVersion`: application versions with formatting and parsing.
- `BaseViewModel` and the `Command` / `IAsyncCommand` types: view models.
- `TelemetrySource`, `StopWatch`: diagnostics.

## Method lists

Many of these have several overloads.

- **AsyncLinq:** `Add`, `AddDefault`, `AddOrUpdate`, `AsAsyncEnumerable`, `Average`, `CastSafe`, `Consolidate`, `ConsolidateUnique`, `Enumerate`, `EnumeratePairs`, `First`, `FirstOrDefault`, `ForEach`, `ForEachAsync`, `ForEachParallel`, `ForEachParallelAsync`, `GetArray`, `IsEmpty`, `Last`, `LastOrDefault`, `Max`, `Min`, `Random`, `RandomKeys`, `RandomValues`, `Range`, `Remove`, `Single`, `SingleOrDefault`, `Sorted`, `ToArray`, `ToArrayBuffer`, `ToConcurrentObservableCollection`, `ToHashSet`, `ToImmutableArray`, `ToList`, `ToObservableCollection`, `TryAdd`, `WhenAllParallelAsync`, `WhereNotNull`, `WhereNotNullAsync`.
- **Hashes:** `Concatenate`, `GetHash`, `Hash`, `Hash128`, `HashAsync`, `Hash_MD5`, `Hash_SHA1`, `Hash_SHA256`, `Hash_SHA384`, `Hash_SHA512`, `HashAsync_MD5`, `HashAsync_SHA1`, `HashAsync_SHA256`, `HashAsync_SHA384`, `HashAsync_SHA512`, `ToBytes`, `TryGetData`. Also `HashCodeU64` / `HashCodeU128`.
- **Types:** `Construct`, `GetNullabilityInfo`, `GetParameterInfo`, `HasInterface`, `IsAnyBuiltInType`, `IsBuiltInNullableType`, `IsBuiltInType`, `IsCollection`, `IsDictionary`, `IsDictionaryEntry`, `IsEqualType`, `IsGenericType`, `IsInitOnly`, `IsJsonNode`, `IsKeyValuePair`, `IsList`, `IsNullable`, `IsNullableType`, `IsOneOf`, `IsSet`, `MethodClass`, `MethodInfo`, `MethodName`, `MethodSignature`, `TryGetUnderlyingEnumType`, `TryGetUnderlyingType`.
- **Spans:** `All`, `Any`, `As`, `AsArraySegment`, `AsBytes`, `AsImmutableArray`, `AsMemory`, `AsReadOnlyMemory`, `AsReadOnlySpan`, `AsSpan`, `Average`, `ClampEnd`, `ClampStart`, `Contains`, `ContainsAll`, `ContainsAny`, `ContainsExact`, `ContainsNone`, `ConvertToString`, `CopyTo`, `Count`, `Create`, `CreateSpan`, `EndsWith`, `Enumerate`, `First`, `FirstOrDefault`, `GetVectors`, `IsNullOrWhiteSpace`, `Join`, `Last`, `LastIndexOf`, `LastOrDefault`, `Max`, `Min`, `QuickSort`, `RefSelect`, `Remove`, `RemoveAll`, `Replace`, `Select`, `SequenceEqual`, `SequenceEqualAny`, `Single`, `SingleOrDefault`, `Slice`, `StartsWith`, `Sum`, `ToMemory`, `ToMemoryStream`, `ToReadOnlyMemory`, `Trim`, `TrimEnd`, `TrimStart`, `TryAsSegment`, `TryCopyTo`, `TryGetCount`.

## Versioning

Versions follow `<.NET major>.<library major>.<library minor>`: 10.10.0 targets .NET 10, library major 10, minor 0. See the [repository README](https://github.com/Jakar510/Jakar.Extensions#versioning).

## License

MIT. See [LICENSE.txt](https://github.com/Jakar510/Jakar.Extensions/blob/master/LICENSE.txt). Contributions are welcome: open an issue or a pull request on [GitHub](https://github.com/Jakar510/Jakar.Extensions).
