# Jakar.Extensions

Extensions and helpers for .NET 10, plus Blazor packages. **Contributions and ideas are welcome.**

## Packages

| Package | Version | What it is | Depends on |
|---|---|---|---|
| [Jakar.Extensions](./Jakar.Extensions/README.md) | 10.10.0 | Extensions and helpers: observable and concurrent collections, HTTP (`WebRequester`), files, security, async LINQ, spans, hashing, users and logins, view models. | Jakar.Json, Jakar.Spans |
| [Jakar.Extensions.Blazor](./Jakar.Extensions.Blazor/README.md) | 9.1.0 | Blazor base components and services. | Jakar.Extensions (10.9.3) |
| [Jakar.Extensions.Blazor.Syncfusion](./Jakar.Extensions.Blazor.Syncfusion/README.md) | 9.0.0 | Syncfusion Blazor helpers. | Syncfusion.Blazor |

Jakar.Extensions builds on two packages that have their own repositories:

| Package | Repository | What it is |
|---|---|---|
| Jakar.Spans 10.1.0 | [Jakar510/Jakar.Spans](https://github.com/Jakar510/Jakar.Spans) | Allocation-free span primitives: `ValueStringBuilder`, `ValueUtf8Builder`, `ValueSpanReader<T>`. |
| Jakar.Json 10.3.0, Jakar.Json.AspNetCore 10.3.0 | [Jakar510/Jakar.Json](https://github.com/Jakar510/Jakar.Json) | Standalone, reflection-free, allocation-free JSON with a `[JsonModel]` source generator; ASP.NET Core integration. |

They're referenced as NuGet packages. Until they're on nuget.org, `nuget.config` also reads the local feed `W:\WorkSpace\LocalNuGet`.

Release in dependency order: **Jakar.Spans → Jakar.Json → Jakar.Json.AspNetCore and Jakar.Extensions**.

## Versioning

Every package version is **`<.NET major>.<library major>.<library minor>`**:

| Part | Meaning | Example |
|---|---|---|
| .NET major | The .NET version the package targets. A package moved to a new .NET release changes this number. | **10**.10.0 targets .NET 10 |
| Library major | Breaking changes: removed or changed public API, or a different wire format. | 10.**9**.3 → 10.**10**.0 drops Newtonsoft.Json |
| Library minor | New features and fixes that don't break existing code. | 10.10.**0** → 10.10.**1** |

- A new package starts at `<.NET major>.1.0`: Jakar.Spans, Jakar.Json and Jakar.Json.AspNetCore start at 10.1.0.
- Packages released together depend on each other at the versions they were released with, as a minimum: Jakar.Extensions 10.11.0 needs Jakar.Json 10.3.0 or later.
- Jakar.Extensions.Blazor 9.1.0 and Jakar.Extensions.Blazor.Syncfusion 9.0.0 predate this scheme; their next releases are 10.1.0.

## Repository

- `Jakar.Extensions.Tests` — the NUnit test suite.
- `Jakar.Extensions.Experiments` — BenchmarkDotNet benchmarks. The JSON serializer benchmarks are in the Jakar.Json repository (`Jakar.Json.Benchmarks`).
- `AotCompatibility.TestApp` — checks that Jakar.Extensions publishes with Native AOT and full trimming without warnings.

## License

MIT. See [LICENSE.txt](./LICENSE.txt).
