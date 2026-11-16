# Jakar.Extensions

Extensions and helpers for .NET 10: observable and concurrent collections, security, async LINQ, span and hashing helpers, JSON models, diagnostics and view models. It is trim and Native AOT compatible. **Contributions and ideas are welcome.**

## Installation

```sh
dotnet add package Jakar.Extensions
```

It brings in [Jakar.Json](https://www.nuget.org/packages/Jakar.Json/) (JSON, including the `[JsonModel]` source generator) and [Jakar.Spans](https://www.nuget.org/packages/Jakar.Spans/) (`ValueStringBuilder` and other span primitives).

## Related packages

Since 10.12 these live in their own packages. They depend on Jakar.Extensions and keep the `Jakar.Extensions` namespace, so code that used them only needs the package reference (and a recompile):

| Package | What it is |
|---|---|
| [Jakar.Files](https://github.com/Jakar510/Jakar.Files) | `LocalFile`, `LocalDirectory`, `LocalFileWatcher`, `TempFile`, `FilePaths`, `FileData`/`FileMetaData`, `EmbeddedResources<T>`, `IniConfig`, `PreferenceFile<TSelf>`, `AppPreference`, plus `IAppInfo`, `AppSettings`, `LanguageApi` and the `LocalFile` / `EmbeddedResources` overloads of `IDataProtector`, `DataProtector` and `PasswordRequirements` (as extension members). |
| [Jakar.ErrorOr](https://github.com/Jakar510/Jakar.ErrorOr) | `Error`, `Errors`, `Alert`, `ErrorOrResult<T>`: RFC 7807-style errors and results. Since 10.12.1 it is a dependency of Jakar.Extensions (it depends on Jakar.Json only), so it comes with this package. |
| [Jakar.Http](https://github.com/Jakar510/Jakar.Http) | `WebRequester`, `WebHandler`, `WebResponse<T>`, `ErrorResponse`, `HeaderCollection`, networking exceptions. Depends on Jakar.ErrorOr and Jakar.Files. |
| [Jakar.Users](https://github.com/Jakar510/Jakar.Users) | `UserModel`, `GroupModel`, `RoleModel`, `UserAddress`, `LoginRequest*`, `Claims`, `Permissions`, `UserRights`, `JwtParser`, `EmailBuilder`. Depends on Jakar.Files. |
| [Jakar.Logging](https://github.com/Jakar510/Jakar.Logging) | Serilog and file logging: `AppLoggerOptions`, `FileLoggerProvider`, `Enricher`, `OpenTelemetryActivityEnricher`, `ScreenShot`, and the Serilog properties of `AppInformation`, `DeviceInformation`, `GcInfo` and `ThreadInformation`. Depends on Jakar.Files. |

`Pair`, `StringTags`, `Status`, `Statuses`, `IErrorDetails`, `IErrorTitles`/`ErrorTitles` and `NullAsEmptyArrayConverter` live in Jakar.ErrorOr since 10.12.1 and are type-forwarded from here (no source or binary change). The MIME types, `None`, `Email`, `IUserName` and `SelfLogger` stay in Jakar.Extensions.
## JSON

Since 10.11 the library serializes with **[Jakar.Json](https://www.nuget.org/packages/Jakar.Json/) only**: source-generated readers and writers, no System.Text.Json, no Newtonsoft.Json and no reflection.

- **Models** are `[JsonModel]` types (`IJsonSerializable<TSelf>`). Put `[JsonModel]` on a `partial` class, record or struct; the generator writes `ToJson()`, `FromJson(json)`, `TryFromJson`, UTF-8 and stream overloads, and `ISpanFormattable` / `ISpanParsable<TSelf>`.

  ```csharp
  [JsonModel]
  public sealed partial class Invoice : BaseClass<Invoice>
  {
      public decimal Total { get; init; }
  }

  Invoice invoice = Invoice.FromJson(json);
  string  text    = invoice.ToJson();
  ```

- **Generic helpers:** `Json.Serialize(value)` / `json.FromJson<T>()` for models; `Json.Serialize<T, TConverter>` / `Json.Deserialize<T, TConverter>` for any other shape (e.g. `JsonArrayConverter<string, JsonStringConverter>`).
- **Root-level arrays:** `Json.Serialize(invoices)`, `JsonCodec.FromJsonList<Invoice>(json)` (and `WebHandler.AsJsonList<Invoice>(token)` in Jakar.Http).
- **Untyped JSON** is a `JNode` tree (`JObjectNode`, `JArrayNode`, `JValueNode`): `json.FromJson()`, and the helpers `Get<T>`, `TryGet<T>`, `GetOrDefault<T>`, `As<T>`, `TryGetValue<T>`, `TryAdd`, `TryUpdate`, `Set`.
- **Older JSON still reads.** The library's models accept JSON written by 10.10 and earlier: case-insensitive property names, numbers in strings, comments and trailing commas (`[assembly: JsonDefaults]`). Declare the same `JsonDefaults` in your assembly for your own models.

### Migrating from 10.10

- `IJsonModel<TSelf>`, `JsonTypeInfo`, `JsonSerializerContext`s (`JakarExtensionsContext`, `UserGuidJsonContext`, `UserLongJsonContext`), `Json.Options`, `Json.AddResolver` and `Json.GetTypeInfo` are gone: put `[JsonModel]` on your types, and constrain generic code to `IJsonSerializable<T>`.
- System.Text.Json attributes aren't read: `[JsonIgnore]` → `[JsonSkip]`, `[JsonConstructor]` → `[JsonCreator]`, `[JsonRequired]` → `[JsonMandatory]`, `[JsonPropertyName]` → `[JsonKey]`, `[JsonExtensionData]` → `[JsonOverflow]`.
- `AdditionalData` is a `JObjectNode?` (ordinal keys). `ErrorResponse`, `FeedBackTrackerException`, `ExceptionDetails.Data` and `LoginRequestValue.Data` use `JNode` / `JObjectNode`.
- Output is Jakar.Json's layout, compact by default (`{"Name" : "value"}`); preference files are written indented.
- `PreferenceFile<TSelf>` subclasses implement `CopyFrom(TSelf loaded)`, which copies their declared preferences on load and reload.
- `AppVersion` and `Email` are written as JSON strings (`ISpanFormattable` + `ISpanParsable<T>`); `AppVersion.ToJson()` / `AppVersion.FromJson(json)` remain.
- Observable collections implement `IJsonCollection<TSelf, T>`: as `[JsonModel]` members they write their unfiltered contents and read in bulk. As a root, `ToJson()` / `FromJson(json)` need `[JsonModel]` elements; use `ObservableJson.ToJson<TSelf, TValue, TConverter>` for others.
- `LocalFile` / `LocalDirectory` serialize as `{"FullPath", "FileEncoding"}` / `{"FullPath"}`; a plain path string no longer reads.
- `WebRequester`: `CreateJsonContent` / `CreateJsonArrayContent` take Jakar.Json writer options; `WebHandler.AsJson<T>`, `AsJsonArray<T>` and `AsJsonList<T>` take no `JsonTypeInfo`.

### Migrating from 10.11

- Add the package for the types you use (table above): Jakar.Files, Jakar.ErrorOr, Jakar.Http, Jakar.Users or Jakar.Logging. It's a binary break: recompile against 10.12.
- Jakar.Extensions no longer depends on Serilog, `Microsoft.IdentityModel.*`, `Microsoft.Extensions.Http` or `ZLinq.FileSystem`; they come with Jakar.Logging, Jakar.Users, Jakar.Http and Jakar.Files.
- `AppInformation`, `DeviceInformation`, `GcInfo` and `ThreadInformation` lost their Serilog members: Jakar.Logging adds `GetProperty()` / `ToProperty()` back as extension members. `DeviceInformation` is no longer an `ILogEventEnricher`: use `device.ToEnricher()` (`DeviceInformationEnricher`).
- `Error.TryCreate(WebResponse<T>, ...)` is in Jakar.Http; `global using static Jakar.Extensions.WebRequester` / `Claims` no longer come from Jakar.Extensions.

## Base classes

- `BaseClass` / `BaseClass<TSelf>`: a model base with property change notifications (`INotifyPropertyChanging`, `INotifyPropertyChanged`), JSON extension data (`AdditionalData`), and equality and comparison for `TSelf`.
- `BaseRecord`: the record counterpart, with JSON extension data.

## Collections

- **Observable:** `ObservableCollection<T>`, `ConcurrentObservableCollection<T>`, `ObservableDictionary<TKey, TValue>`, `ObservableConcurrentDictionary<TKey, TValue>`, `ObservableHashSet<T>`. They raise change notifications and can show a filtered view (`OverrideFilter`); the list types also keep a sort order (`Comparer<T>`).
- **Queues and stacks:** `ConcurrentDeque<T>`, `FixedSizedDeque<T>`, `FixedSizedQueue<T>`, `LockFreeDeque<T>`, `LockFreeStack<T>`.
- **Other:** `ArrayBuffer<T>` (a growable buffer struct), `AsyncChannel<T>`, `ValueLinkedList<T>`, `MultiDict<TKey>`.

## Status codes and MIME types

- `Status`, `Statuses`: HTTP status codes, their titles (`IErrorTitles`) and the status for an exception.
- `MimeType`, `MimeTypeNames`: MIME types, their names and file extensions.

## Security

- `SecuredString`: sensitive text kept in a `SecureString`.
- `PasswordValidator` and `PasswordRequirements`: password rules and validation.
- `OneTimePassword`: TOTP codes.
- `IDataProtector` / `IDataProtectorProvider` / `DataProtector`: encrypt and decrypt data.

## Errors

- `ExceptionDetails`: exceptions as serializable data.
- `StringTags`, `IErrorDetails` (from Jakar.ErrorOr): the details of an `Error` (inspired by [ErrorOr](https://github.com/amantinband/error-or)).

## Miscellaneous

- `Validate`: string and state validation.
- `Tasks`: task helpers such as `SafeFireAndForget`.
- `Types`: type inspection (`IsBuiltInType`, `IsNullableType`, `IsCollection`, `IsDictionary`, `IsList`, `IsSet`, `HasInterface`, `TryGetUnderlyingEnumType`, `MethodName`, `MethodSignature`, ...).
- `AppVersion`: application versions with formatting and parsing.
- `BaseViewModel` and the `Command` / `IAsyncCommand` types: view models.
- `TelemetrySource`, `StopWatch`, `SelfLogger`: diagnostics.

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
