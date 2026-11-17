# Jakar.Extensions

Extensions and helpers for .NET 10, plus Blazor packages. **Contributions and ideas are welcome.**

## Packages

| Package | Version | What it is | Depends on |
|---|---|---|---|
| [Jakar.Extensions](./Jakar.Extensions/README.md) | 10.12.0 | Extensions and helpers: observable and concurrent collections, security, async LINQ, spans, hashing, JSON models, diagnostics, view models. | Jakar.Json, Jakar.Spans |
| [Jakar.Extensions.Blazor](./Jakar.Extensions.Blazor/README.md) | 9.1.0 | Blazor base components and services. | Jakar.Extensions (10.9.3) |
| [Jakar.Extensions.Blazor.Syncfusion](./Jakar.Extensions.Blazor.Syncfusion/README.md) | 9.0.0 | Syncfusion Blazor helpers. | Syncfusion.Blazor |

Jakar.Extensions builds on two packages that have their own repositories:

| Package | Repository | What it is |
|---|---|---|
| Jakar.Spans 10.1.0 | [Jakar510/Jakar.Spans](https://github.com/Jakar510/Jakar.Spans) | Allocation-free span primitives: `ValueStringBuilder`, `ValueUtf8Builder`, `ValueSpanReader<T>`. |
| Jakar.Json 10.3.0, Jakar.Json.AspNetCore 10.3.0 | [Jakar510/Jakar.Json](https://github.com/Jakar510/Jakar.Json) | Standalone, reflection-free, allocation-free JSON with a `[JsonModel]` source generator; ASP.NET Core integration. |

They're referenced as NuGet packages. Until they're on nuget.org, `nuget.config` also reads the local feed `W:\WorkSpace\LocalNuGet`.

Since 10.13 Jakar.Extensions sits on top of these packages, each in its own repository. None of them depends on Jakar.Extensions; the types keep the `Jakar.Extensions` namespace and Jakar.Extensions type-forwards the ones that moved, so code compiled against an older Jakar.Extensions still finds them.

| Package | Repository | What it is | Depends on |
|---|---|---|---|
| Jakar.Core 10.1.0 | [Jakar510/Jakar.Core](https://github.com/Jakar510/Jakar.Core) | `BaseClass`, `BaseRecord`, `IObservableObject`, `IJsonModel`, `IUniqueID`, the equality/comparison interfaces, `ExpectedValueTypeException`, `OutOfRangeException`. | Jakar.Json |
| Jakar.ErrorOr 10.2.0 | [Jakar510/Jakar.ErrorOr](https://github.com/Jakar510/Jakar.ErrorOr) | `Error`, `Errors`, `Alert`, `ErrorOrResult<T>`, `Pair`, `StringTags`, `Status`. | Jakar.Json |
| Jakar.Logging 10.3.0 | [Jakar510/Jakar.Logging](https://github.com/Jakar510/Jakar.Logging) | Serilog setup and enrichers, `TelemetrySource`/`TelemetrySpan`, `AppVersion`, `AppInformation`, `DeviceInformation`, `GcInfo`, `SelfLogger`, `ExceptionDetails`. | Jakar.Core, Serilog |
| Jakar.Collections 10.1.0 | [Jakar510/Jakar.Collections](https://github.com/Jakar510/Jakar.Collections) | Observable and concurrent collections, `ArrayBuffer<T>`/`Buffer<T>`, `Disposables`, `Guids`. | Jakar.Core, Jakar.Logging |
| Jakar.Files 10.2.0 | [Jakar510/Jakar.Files](https://github.com/Jakar510/Jakar.Files) | Files, directories, watchers, embedded resources, INI and preference files, MIME types. | Jakar.Core, Jakar.Logging, Jakar.Collections |
| Jakar.Users 10.2.0 | [Jakar510/Jakar.Users](https://github.com/Jakar510/Jakar.Users) | User, group, role and address models, logins, claims, permissions, JWT parsing, `PasswordValidator`, `Email`, `Regexes`. | Jakar.Files (+ Core, Logging, Collections) |
| Jakar.Http 10.2.0 | [Jakar510/Jakar.Http](https://github.com/Jakar510/Jakar.Http) | `WebRequester`, `WebHandler`, `WebResponse<T>`, `ErrorResponse`, `IHostInfo`. | Jakar.Files, Jakar.ErrorOr (+ Core, Logging, Collections) |

Release in dependency order: **Jakar.Spans → Jakar.Json → Jakar.Core and Jakar.ErrorOr → Jakar.Logging → Jakar.Collections → Jakar.Files → Jakar.Users and Jakar.Http → Jakar.Extensions → Jakar.Database**. Jakar.Logging.Serilog (10.1.0 on nuget.org) is retired: its types are in Jakar.Logging 10.3.0.

## Versioning

Every package version is **`<.NET major>.<library major>.<library minor>`**:

| Part | Meaning | Example |
|---|---|---|
| .NET major | The .NET version the package targets. A package moved to a new .NET release changes this number. | **10**.10.0 targets .NET 10 |
| Library major | Breaking changes: removed or changed public API, or a different wire format. | 10.**9**.3 → 10.**10**.0 drops Newtonsoft.Json |
| Library minor | New features and fixes that don't break existing code. | 10.10.**0** → 10.10.**1** |

- A new package starts at `<.NET major>.1.0`: Jakar.Spans, Jakar.Json, Jakar.Json.AspNetCore, Jakar.Files, Jakar.ErrorOr, Jakar.Http, Jakar.Users and Jakar.Logging start at 10.1.0.
- Packages released together depend on each other at the versions they were released with, as a minimum: Jakar.Extensions 10.12.1 needs Jakar.Json 10.3.0 and Jakar.ErrorOr 10.2.0 or later (Jakar.ErrorOr 10.1.0 still depended on Jakar.Extensions 10.12.0), and Jakar.Files 10.1.0 needs Jakar.Extensions 10.12.1 or later. Use Jakar.Extensions 10.12.1 or later with Jakar.ErrorOr: 10.12.0 defines `Pair`, `StringTags` and `Status` itself.
- Jakar.Extensions.Blazor 9.1.0 and Jakar.Extensions.Blazor.Syncfusion 9.0.0 predate this scheme; their next releases are 10.1.0.

## Repository

- `Jakar.Extensions.Tests` — the NUnit test suite.
- `Jakar.Extensions.Experiments` — BenchmarkDotNet benchmarks. The JSON serializer benchmarks are in the Jakar.Json repository (`Jakar.Json.Benchmarks`).
- `AotCompatibility.TestApp` — checks that Jakar.Extensions publishes with Native AOT and full trimming without warnings (Jakar.Extensions only; the split-out packages are tested in their repositories).

## License

MIT. See [LICENSE.txt](./LICENSE.txt).
