# Jakar.Json — Specification

Status: **Implemented** · 10/07/2026 (drafted 10/06/2026). Sections describe the shipped design; §12 and §13 record what was verified.

A source-generated JSON serializer that does not use `System.Text.Json`. Generated code reads and writes JSON directly: writes go through `ValueStringBuilder` (UTF-16) or its UTF-8 sibling, and reads go through a span cursor. This package uses the same pattern as `Jakar.SystemTextJson`: you mark a `partial` type with an attribute, and an incremental generator implements a static-abstract interface on it.

---

## 1. Goals and non-goals

### Goals (in priority order)

1. **Reliability.** Every read is strict and bounded. No input can crash the process, overflow the stack or hang the parser. Malformed input always fails in a defined, documented way.
2. **Performance.** No reflection, no runtime metadata, and no options lookups. Settings become constants at compile time. On a hot path, the JIT should see straight-line code that it can specialize per encoding.
3. **Zero incidental allocations.** The only GC allocations are the values you asked for: the output `string`, or the deserialized object graph (§6).
4. **Idempotency.** For the same input, the output is the same bytes on every machine, OS, culture, thread and run (§5).
5. **AOT and trimming.** Native AOT publishes with no warnings. Settings attributes are `[Conditional]`, so they don't exist at runtime.
6. **Ergonomics.** Models implement `ISpanFormattable`/`ISpanParsable<T>` and their UTF-8 counterparts. One-line `ToJson`/`FromJson` helpers cover `string`, spans, `Stream`, `IBufferWriter<T>` and `ReadOnlySequence<byte>`. A Newtonsoft-style dynamic DOM is available.

### Non-goals (v1)

- **Reflection-based fallback for unannotated types.** If the generator can't see a type, it can't be serialized. You get a compile-time error, never a runtime surprise.
- **The C# `dynamic` keyword.** The `Microsoft.CSharp` runtime binder uses reflection and isn't supported under Native AOT. The dynamic DOM (§10) offers indexers, implicit conversions and typed accessors instead.
- **Resumable, incremental parsing across buffer boundaries** (the `Utf8JsonReader` `isFinalBlock` model). Stream reads buffer the whole document into pooled memory first (§8.3). A resumable reader is a v2 candidate. Large line-delimited files (NDJSON) are streamed one line at a time instead (§8.2).
- **Reference preservation (`$id`/`$ref`).** A cyclic graph fails at `MaxDepth` with a clear error.
- **JSONPath / `SelectToken`.** This is a v2 candidate.
- **Compatibility with `System.Text.Json` or Newtonsoft attributes at runtime.** The generator *does* read a few STJ attributes at compile time to make migration easier (§4.4).

---

## 2. Packages and projects

The layout mirrors `Jakar.SystemTextJson`:

| Project | TFM | Contents |
|---|---|---|
| `Jakar.Spans` | `net10.0` | `ValueStringBuilder`, `ValueUtf8Builder`, `ValueSpanReader<T>` and `Sizes`, moved out of `Jakar.Extensions` (§3.1). No dependencies. |
| `Jakar.Json` | `net10.0` | Attributes, `IJsonWriter`/`IJsonReader`, `JsonWriter`, `JsonUtf8Writer`, `JsonReader<TChar>`, the converters (`Jakar.Json.Converters`), `JsonCodec` helpers, `JsonTape`, the `JNode` DOM and its reader/writer, errors. Packs the generator and code fixes into `analyzers/dotnet/cs`. |
| `Jakar.Json.Generator` | `netstandard2.0` | `JsonGenerator` (`IIncrementalGenerator`) and `JsonAnalyzer` (JJSON015). Pinned to Roslyn 4.12, the same as the STJ generator. |
| `Jakar.Json.Generator.CodeFixes` | `netstandard2.0` | Code fixes: add `partial` (JJSON001), freeze the order with `[JsonMember(Order = n)]` (JJSON006), add `[GenerateJson]` (JJSON015). |
| `Jakar.Json.Tests` | `net10.0` | Runtime, conformance, idempotency and allocation tests (§12). |
| `Jakar.Json.Generator.Tests` | `net10.0` | Generated-source, diagnostic (JJSON001–015), code-fix and incrementality tests, using the `GeneratorHarness` pattern. |
| `Jakar.Extensions.Experiments` | — | Benchmarks against STJ source generation and Newtonsoft (§12.5). |

**Namespace: `Jakar.Json`.** This breaks with the package-split convention (`Jakar.Extensions`) on purpose. `Jakar.Extensions` already contains `Json`, `JsonModelAttribute`, `IJsonModel<T>` and friends, and consumers commonly import `System.Text.Json` and `Newtonsoft.Json` too. A separate namespace plus collision-free type names (§4) lets all three coexist in one file.

**Dependency.** `Jakar.Json` references only `Jakar.Spans` (§3.1), never `Jakar.Extensions`, so it doesn't pull in the core library or `Jakar.SystemTextJson` (decision D1).

---

## 3. Low-level I/O

### 3.1 The `Jakar.Spans` package

The span types `Jakar.Json` builds on move out of `Jakar.Extensions` into a small, dependency-free package, `Jakar.Spans` (decision D1). `Jakar.Extensions` references `Jakar.Spans` and keeps `[assembly: TypeForwardedTo]` for `ValueStringBuilder` and `Sizes`, so code compiled against older `Jakar.Extensions` keeps working without recompiling: the move is neither a source nor a binary break. The types keep the `Jakar.Extensions` namespace, as every package split out of `Jakar.Extensions` does. `Sizes` no longer names `AppVersion` (it lives above `Jakar.Spans`); `Jakar.Extensions` registers it from a module initializer.

| Type | Purpose |
|---|---|
| `Sizes` *(moved)* | Per-type maximum formatted lengths, used by `EnsureCapacity<TValue>`. |
| `ValueStringBuilder` *(moved)* | UTF-16 output. Already does what we need: `Next` + `Length` give `GetSpan`/`Advance` semantics, `EnsureCapacity` pre-sizes, and storage is a stack buffer or a pooled array. |
| `ValueUtf8Builder` *(done)* | Mirror of `ValueStringBuilder` over `Span<byte>`/`ArrayPool<byte>` ([ValueUtf8Builder.cs](../Jakar.Spans/ValueUtf8Builder.cs)). `Append(byte)`, `Append(byte, count)`, `Append(ReadOnlySpan<byte>)`, `Append(char)` / `Append(string?)` / `Append(ReadOnlySpan<char>)` (transcoded with `Utf8.FromUtf16`; lone surrogates → U+FFFD), `AppendUtf8Formattable<T>`, `AppendJoin<T>`, Trim/Replace/Insert, `ToArray()` / `ToString()` (both dispose), and `IUtf8SpanFormattable` + `ISpanFormattable` (decoded). |
| `ValueSpanReader<T>` *(done)* | `ref struct` forward cursor over `ReadOnlySpan<T>` where `T : unmanaged, IEquatable<T>` ([ValueSpanReader.cs](../Jakar.Spans/ValueSpanReader.cs)). It exposes `Position`, `Consumed`, `Remaining`, `RemainingCount`, `End`, `Peek()`, `TryPeek(out T)`, `TryPeek(offset, out T)`, `IsNext`, `Read()`, `TryRead(out T)`, `TryRead(count, out span)`, `TryReadExact(T | ReadOnlySpan<T>)`, `TryReadTo`, `TryReadToAny`, `IndexOf`, `IndexOfAny`, `IndexOfAnyExcept`, `SkipWhile(T | SearchValues<T>)`, `Advance`, `Rewind(checkpoint)`, `Reset` and `Slice(start, end)`. `Try*` members never throw and never move the cursor on `false`. |

Both new types follow `ValueStringBuilder`'s conventions: `[UnscopedRef] ref` returns for chaining, an idempotent `Dispose`, and `AggressiveInlining` on the hot members. Each has its own test file next to `ValueStringBuilder_Tests.cs` in `Jakar.Extensions.Tests`. The `format` parameters of the builders' formatting methods are `scoped`, so callers can pass stack-allocated formats.

### 3.2 The writer and reader abstractions

Generated code is written **once**, generic over the writer and reader (`where TWriter : IJsonWriter, allows ref struct`). The JIT specializes it per `ref struct`, so UTF-16 and UTF-8 each get native code with no interface dispatch and no transcoding. The full contracts are in [Core/Contracts.cs](Core/Contracts.cs).

```csharp
public interface IJsonWriter
{
    static abstract bool IsUtf8 { get; }
    int Depth { get; }

    void WriteStartObject();   void WriteEndObject();
    void WriteStartArray();    void WriteEndArray();
    void WritePropertyName( JsonName name );                      // pre-escaped, pre-encoded (§3.3)
    void WritePropertyName( scoped ReadOnlySpan<char> name );     // runtime names (keys, extension data): escaped by the writer
    void WriteNull();
    void WriteBoolean( bool value );
    void WriteString( scoped ReadOnlySpan<char> value );
    void WriteInteger<T>( T value ) where T : IBinaryInteger<T>;  // every integer type, incl. Int128, BigInteger
    void WriteFloat<T>( T value ) where T : IFloatingPoint<T>;    // float, double, Half, decimal: canonical (§5.2); NaN/±Infinity throw
    void WriteFormatted<T>( T value, scoped ReadOnlySpan<char> format = default ) where T : ISpanFormattable;  // invariant, as an escaped string
    void WriteRawNumber( scoped ReadOnlySpan<char> number );      // validated number text, verbatim (the DOM keeps number text)
}

public interface IJsonReader
{
    static abstract bool IsUtf8 { get; }     // a hint: name matching checks JsonSpan.IsUtf8 (a JsonTapeReader may hold either encoding)
    int       Depth { get; }
    JsonError Error { get; }                 // the first failure; nothing throws

    JsonTokenKind PeekKind();
    bool TryReadStartObject();
    bool TryReadProperty( out JsonSpan name, out bool end );      // the next member's name, or the object's end
    bool TryReadStartArray();
    bool TryReadNextElement( out bool end );                      // before each element, or the array's end
    bool TryReadNull();                                           // consumes a null if that's next; never an error
    bool TryReadBoolean( out bool value );
    bool TryReadInteger<T>( out T value, bool allowString = false ) where T : struct, IBinaryInteger<T>;
    bool TryReadFloat<T>( out T value, JsonFloatRead mode = JsonFloatRead.Strict ) where T : struct, IFloatingPoint<T>;
    bool TryReadString( [NotNullWhen(true)] out string? value );  // the only allocating read
    bool TryReadStringSpan( out JsonSpan value );                 // unescaped view, valid until the next read
    bool TryReadRawNumber( out JsonSpan number );
    bool TrySkipValue();                                          // validates, depth-checked, without recursion
    JsonReaderCheckpoint Checkpoint();
    void Rewind( in JsonReaderCheckpoint checkpoint );            // cheap lookahead (polymorphism, §7.5)
    bool Fail( JsonErrorKind kind );                              // records the first error, returns false
    string GetPath();                                             // "$.lines[3].price": for errors only
}
```

`JsonSpan` is a `readonly ref struct` holding either a `ReadOnlySpan<char>` or a `ReadOnlySpan<byte>`; generated code matches names on whichever it holds (`__name.IsUtf8 ? __JsonMatch(__name.Utf8) : __JsonMatch(__name.Utf16)`).

Implementations, all `ref struct`s:

| Type | Input/output | Built on |
|---|---|---|
| `JsonWriter` | UTF-16 out | `ValueStringBuilder` |
| `JsonUtf8Writer` | UTF-8 out, or straight to a `Stream` in 16 KB chunks | `ValueUtf8Builder` |
| `JsonReader<char>` / `JsonReader<byte>` | UTF-16 / UTF-8 in: one implementation, generic over the code unit | `ValueSpanReader<TChar>` and the shared `JsonLexer<TChar>` |
| `JsonTapeReader` | a `JsonTape` (§10.1) | the tape's entries |
| `JNodeReader` / `JNodeWriter` | a `JNode` tree (§10.2) | the nodes |

One reader generic over `TChar` replaced the separate `JsonReader` / `JsonUtf8Reader` of the draft: the lexing (`JsonLexer<TChar>`) is shared with `JsonTape`, so the reader and the tape accept and reject exactly the same documents.

### 3.3 Pre-encoded names

For every member, the generator emits the final JSON name already escaped, quoted and followed by the name separator `" : "` (space, colon, space), in both encodings. Both are compile-time constants:

```csharp
private static JsonName __JsonName2 => new("\"total\" : ", "\"total\" : "u8);
```

`WritePropertyName(JsonName)` is then one `CopyTo` (plus a comma or indentation when needed). The UTF-8 literal is static data, and the `string` literal is interned once by the runtime. Neither allocates per call.

Only names that every escaping mode writes the same way are pre-encoded: printable ASCII without `" \ < > & ' +`. Any other name is written with `WritePropertyName(ReadOnlySpan<char>)`, so the writer's `Escaping` option applies to it.

### 3.4 Writer behavior

- **Separators.** The writer tracks "has items" and "is an object" per depth in two inline bit stacks (`[InlineArray]` of 17 `ulong`s: depth up to 1024, no allocation). Generated code never writes commas, and misuse (a value with no name inside an object, two roots) throws `JsonWriteException`.
- **Indentation.** Indentation is a writer option (§4.2), not a compile-time constant, so a single generated method serves both compact and indented output. Newlines are always `\n` and never `Environment.NewLine` (§5).
- **In-place formatting.** Numbers and formatted values call `TryFormat` straight into the builder's free space (`AppendSpanFormattable` / `AppendUtf8Formattable`), growing and retrying only if they don't fit. Floats are then canonicalized in place (§5.2). Nothing is formatted through an intermediate string.
- **String escaping.** Escaping is a vectorized scan with `IndexOfAny(SearchValues)` over the escape set. Runs that need no escaping are bulk-copied.
- **Depth guard.** The writer enforces `MaxDepth` too. A cyclic graph fails with `JsonErrorKind.DepthExceeded` instead of a `StackOverflowException`.

### 3.5 Reader behavior

- **Strict RFC 8259 by default.** Comments and trailing commas are opt-in. A leading BOM is skipped. Anything after the root value other than whitespace is an error.
- **Bounded.** `MaxDepth` (default 64) and `MaxStringLength` (default unbounded, configurable) are enforced. Every loop makes forward progress, so no input can hang the reader.
- **Exception-free core.** Every primitive returns `bool` and records the first error, along with its position, in `Error`. Generated `TryRead` methods propagate `false`. Only the public throwing APIs convert `Error` into a `JsonReadException`, in one `[DoesNotReturn]`, non-inlined throw helper. This keeps `TryFromJson` on bad input as fast as on good input. It also keeps `try`/`catch` regions out of hot generated code.
- **JSON path on error, for free.** The reader records, per depth, the offset of the current member name or the current element index (an inline `int` stack covering the first 64 levels). If a read fails, it builds the path (`$.lines[3].price`) from those offsets *only then*. A successful read never pays for it.
- **Escaped property names.** A name with escapes (`"\u0074otal"`) is unescaped into the reader's pooled scratch space before matching, in the reader's encoding. A name without escapes is matched directly against the input slice.
- **Strings.** One vectorized pass (`IndexOfAny` over quote, backslash and control characters) finds the closing quote and whether there are any escapes. Without escapes, the read is `new string(slice)` (or `Encoding.UTF8.GetString`). With escapes, the content is unescaped into pooled scratch space first. Either way the string is allocated exactly once, at its exact size.
- **Numbers.** The reader validates them against the JSON grammar *first*, then parses with invariant culture, so `+1`, `01`, `.5`, `1.`, `NaN` and hex are always rejected. Overflow is an error and is never saturated. Integer targets reject fractions and exponents (`1.0` and `1e2` don't fit `int`). Floating-point parsing is correctly rounded, the IEEE behavior of the BCL since .NET Core 3.0.
- **Invalid text.** UTF-8 input is validated in one vectorized pass (`Utf8.IsValid`) before reading; invalid input fails with `InvalidUtf8`. In UTF-16 input, a raw unpaired surrogate inside a string fails with `InvalidString`. An *escaped* lone surrogate (`"\uD800"`) is legal JSON and reads back as that `char`, so any .NET string round-trips.

---

## 4. Attributes and settings

### 4.1 Precedence

```
[JsonMember(...)]  on the property/field      (highest)
[GenerateJson(...)] on the type
[assembly: JsonDefaults(...)]
built-in defaults                              (lowest)
```

Settings are resolved **per type at compile time** and baked into that type's generated code. A nested model always serializes with **its own** settings, never its container's. Type `A`'s JSON is the same whether it's a root, a member of `B` or an element of a list. That is a deliberate idempotency property. The exception is writer-level options (indentation), which flow from the root call (§4.2).

All settings attributes are `[Conditional("JAKAR_JSON_KEEP_ATTRIBUTES")]`, as in `Jakar.SystemTextJson`. They are read by the generator and erased from the compiled metadata.

Because attribute properties can't be nullable, every settings enum has `Inherit = 0`. "Not set" therefore means "use the next level up".

### 4.2 Settings

`JsonDefaults` and `GenerateJson` both derive from `JsonSettingsAttribute`, which defines:

| Setting | Values (built-in default **bold**) | Applies to |
|---|---|---|
| `Naming` | **`AsDeclared`**, `CamelCase`, `PascalCase`, `SnakeCaseLower`, `SnakeCaseUpper`, `KebabCaseLower`, `KebabCaseUpper` | names (compile time) |
| `NameMatching` | **`Exact`**, `OrdinalIgnoreCase` | read |
| `NullValues` | **`Write`**, `Omit` | write |
| `DefaultValues` | **`Write`**, `Omit` (omit members equal to `default(T)`) | write |
| `UnknownMembers` | **`Skip`**, `Error`, `Capture` (needs a `[JsonMember(ExtensionData = true)]` member) | read |
| `DuplicateMembers` | **`Error`**, `LastWins` | read |
| *(required members)* | always **`Error`** when missing: `required` members, constructor parameters without a default, `[JsonMember(Required = true)]`, STJ `[JsonRequired]` | read |
| `Enums` | **`Name`**, `Number` | both |
| `EnumNaming` | **`AsDeclared`**, plus the `Naming` values | names (compile time) |
| `NumbersFromStrings` | **`Disallow`**, `Allow` (`"42"` reads as `42`) | read |
| `LargeIntegers` | **`Number`**, `String` (write `long`/`ulong`/`Int128` beyond ±2^53 as strings, for JavaScript consumers) | write |
| `NonFiniteFloats` | **`Error`**, `AsString` (`"NaN"`, `"Infinity"`, `"-Infinity"`) | both |
| `LocalDateTimes` | **`ConvertToUtc`** (decision D2), `WriteOffset`, `Error` | write (§5.3) |
| `UnorderedCollections` | **`Sorted`**, `Enumeration` | write (§5.2) |
| `Escaping` | **`Minimal`**, `AsciiOnly`, `HtmlSafe` (also escapes `<>&'+`) | write (writer option) |
| `Indented` | **`false`** | write (writer option) |
| `IndentChar` | **`Tab`** (`'\t'`), `Space` (`' '`) | write (writer option) |
| `IndentSize` | **`1`**: `IndentChar`s per depth level | write (writer option) |
| `AllowComments` | **`false`** | read (reader option) |
| `AllowTrailingCommas` | **`false`** | read (reader option) |
| `MaxDepth` | **`64`** | both (reader/writer option) |
| `GenerateToString` | **`true`**: `ToString()` returns compact JSON unless the type already overrides it | type |

**Writer and reader options.** `Indented`, `IndentChar`, `IndentSize`, `Escaping` and `MaxDepth` configure the writer; `AllowComments`, `AllowTrailingCommas` and `MaxDepth` configure the reader, which also takes `MaxDocumentBytes` (streams and NDJSON lines, default 64 MB) and `MaxStringLength`. They live in `JsonWriterOptions` / `JsonReaderOptions` (`readonly record struct`s whose `default` is the built-in default), not in generated member code. The generated `DefaultWriterOptions` / `DefaultReaderOptions` carry the type's resolved values, and every helper takes an override. All other settings can only change at compile time.

### 4.3 Attributes

```csharp
[assembly: JsonDefaults(Naming = JsonNaming.CamelCase, NullValues = JsonNullValues.Omit)]

[GenerateJson(UnknownMembers = JsonUnknownMembers.Error)]          // type-level override
public sealed partial record Invoice( Guid Id, decimal Total )
{
    public required List<LineItem> Lines { get; init; }

    [JsonMember(Name = "memo", Order = -1)]                        // per-member override
    public string? Notes { get; init; }

    [JsonMember(Ignore = true)]
    public decimal Tax => Total * 0.07m;

    [JsonMember(Converter = typeof(UnixSecondsConverter))]         // custom converter, §7.4
    public DateTimeOffset Created { get; init; }
}
```

| Attribute | Targets | Members |
|---|---|---|
| `JsonDefaults` | assembly | every `JsonSettingsAttribute` setting |
| `GenerateJson` | class, struct, record, record struct | every setting, plus `Discriminator` (§7.5) |
| `JsonMember` | property, field | `Name`, `Order`, `Ignore`, `Required`, `ExtensionData`, `Converter`, and the per-member settings `NullValues`, `DefaultValues`, `Enums`, `NumbersFromStrings`, `LargeIntegers`, `NonFiniteFloats`, `LocalDateTimes`, `UnorderedCollections` |
| `JsonDerived(Type, string tag)` | class | polymorphism (§7.5); the derived type needs `[GenerateJson]` too |
| `JsonEnumName(string)` | enum field | per-value name override. Not `[Conditional]`: it stays in metadata, so an assembly that uses the enum generates its codec from it. |

Names checked to be free of collisions with `System.Text.Json`, Newtonsoft and `Jakar.Extensions`: `GenerateJson`, `JsonDefaults`, `JsonMember`, `JsonDerived`, `JsonEnumName`.

### 4.4 Migration from System.Text.Json attributes

When the generator finds these attributes by metadata name, it honors them, with no package dependency: `JsonPropertyName`, `JsonPropertyOrder`, `JsonIgnore` (`Always` ignores; `WhenWritingDefault` / `WhenWritingNull` map to `DefaultValues` / `NullValues = Omit`), `JsonRequired`, `JsonInclude`, `JsonExtensionData`, `JsonConstructor` and `JsonStringEnumMemberName`. If a `[JsonMember]` is present on the same member, it wins. A type can therefore keep its System.Text.Json attributes during a migration; only Jakar.SystemTextJson's `[JsonModel]` conflicts with `[GenerateJson]` (JJSON010).

---

## 5. Idempotency

### 5.1 Guarantees

For every `[GenerateJson]` type `T`, value `x` and JSON text `s`:

| # | Property | Statement |
|---|---|---|
| I1 | **Deterministic write** | `ToJson(x)` depends only on the value of `x` and the compile-time settings. Culture, time zone, OS, machine, thread, process, run and allocation history make no difference. |
| I2 | **Deterministic read** | `FromJson(s)` either produces the same value or fails with the same `JsonError` at the same position. |
| I3 | **Round trip** | `FromJson(ToJson(x))` equals `x` (value equality) for every supported member type. The exceptions are listed in §5.4. |
| I4 | **Canonical fixed point** | For `c = ToJson(FromJson(s))`, `ToJson(FromJson(c)) == c`, byte for byte. One pass always reaches the canonical form. |
| I5 | **Encoding parity** | The UTF-8 output is exactly `Encoding.UTF8.GetBytes` of the UTF-16 output. |

### 5.2 Canonical output rules

- **Member order.** Members are ordered by `Order`, which defaults to 0, then by declaration position: base type members first, then by `(file path, ordinal)` and then by span start. Order therefore doesn't depend on the order in which files are passed to the compiler. When a type's members span several partial files, JJSON006 suggests explicit `Order` values.
- **Unordered collections.** These are `Dictionary<,>`, `HashSet<>`, `ConcurrentDictionary<,>`, `FrozenDictionary<,>` and `FrozenSet<>`. Their enumeration order is an implementation detail, so with `UnorderedCollections = Sorted` they are written **sorted by key or element**. The comparison is chosen at compile time and passed as a type argument (`IJsonOrder<T>`): ordinal for strings, an enum's numeric value, `IComparable<T>` otherwise, with `null` first for `T?`. Elements that can't be compared are written in enumeration order, with warning JJSON013. The sort works on a pooled copy, with no GC allocation and no runtime comparer lookup. Types with a defined order (`List<>`, arrays, `SortedDictionary<,>`, `ImmutableSortedSet<>`, `OrderedDictionary<,>`) are written in their own order. `Enumeration` turns sorting off for speed, at the cost of I1 for those members.
- **Whitespace.** The name separator is always `" : "` (space, colon, space), in compact and indented output alike. It is baked into the pre-encoded names (§3.3), and the writer uses the same separator for runtime keys such as dictionary keys. Otherwise, compact output has no whitespace. Indented output adds `\n` newlines and, by default, one tab per depth level (`IndentSize` × `IndentChar`), with no trailing newline.
- **Strings.** Under `Minimal`, only `"`, `\` and U+0000–U+001F are escaped, using the short forms `\b \f \n \r \t` and otherwise `\u00XX` with **uppercase** hex. `/` is never escaped. Lone surrogates are written as `\uXXXX`, so any .NET `string` round-trips losslessly. `AsciiOnly` and `HtmlSafe` add to the escape set but follow the same rules.
- **Numbers.** Integers have no leading zeros and no `+`. `double`/`float`/`Half` use the shortest round-trippable form (`"R"` semantics). `-0.0` is written as `-0`. Exponent formatting is fixed: `1E+20` and `1E-07` become `1e+20` and `1e-7` (lowercase `e`, sign always present for positive exponents, no padding). `decimal` keeps its scale, so `1.10m` is written `1.10` and reads back with scale 2.
- **Booleans and null.** `true`, `false`, `null`.

### 5.3 Canonical formats for common BCL types

All are written as JSON strings.

| Type | Format | Example |
|---|---|---|
| `Guid` | `D`, lowercase | `"0f8fad5b-d9cb-469f-a165-70867728950e"` |
| `DateTime` | ISO 8601 `O`. `Utc` → `Z`; `Unspecified` → no offset; `Local` → depends on `LocalDateTimes`. With the default `ConvertToUtc`, the value is converted to UTC and written with `Z`. | `"2026-10-06T21:49:53.0000000Z"` |
| `DateTimeOffset` | ISO 8601 `O` | `"2026-10-06T21:49:53.0000000-05:00"` |
| `DateOnly` | `yyyy-MM-dd` | `"2026-10-06"` |
| `TimeOnly` | `HH:mm:ss.fffffff` | `"21:49:53.0000000"` |
| `TimeSpan` | `c` | `"1.02:03:04.5000000"` |
| `Uri` | `OriginalString` | |
| `Version` | `ToString()` | `"1.2.3.4"` |
| enums | generated `switch`, never `Enum.ToString`. `[Flags]` values become `"A, B"` in ascending bit order. A value with no name is written as a number. | `"Paid"` |

### 5.4 Documented round-trip exceptions

- **`DateTime` with `Kind = Local`.** Under `ConvertToUtc`, it reads back as the same instant with `Kind = Utc`. Under `WriteOffset`, the output depends on the machine's time zone, which breaks I1. Use `DateTimeOffset` instead.
- **NaN payloads.** All NaNs are written as `"NaN"`, and only under `AsString`.
- **Reference identity.** Two members that point at the same object are read back as two separate objects.
- **`UnorderedCollections = Enumeration`.** I1 holds only as far as the BCL keeps enumeration order stable.

---

## 6. Allocation budget

"No allocations" means **no GC allocation except the values you asked for**. Tests assert the budget with `GC.GetAllocatedBytesForCurrentThread()` (§12.3).

| Operation | GC allocations |
|---|---|
| `value.TryFormat(Span<char>, …)` / `TryFormat(Span<byte>, …)` | **0** |
| `JsonCodec.WriteJson(value, IBufferWriter<byte>)` / `IBufferWriter<char>` | **0** (beyond what the buffer writer itself allocates) |
| `value.ToJson()` → `string` | **exactly 1**: the result string, at its exact length |
| `value.ToJsonUtf8()` → `byte[]` | **exactly 1**: the result array |
| `value.ToJson(Stream)` | **0** (pooled buffer, flushed in 16 KB chunks) |
| `await value.ToJsonAsync(Stream)` | **0** beyond a pooled async state machine (`PoolingAsyncValueTaskMethodBuilder`). Serialization runs synchronously into a pooled buffer, and only the stream write is awaited. |
| `T.FromJson(...)` / `T.Parse(...)` | **only the result graph**: the objects, strings and collections the model holds |
| `T.FromJson(Stream)` / `FromJsonAsync(Stream)` | the result graph only. The input is buffered in pooled memory (§8.3). |
| `JsonTape.Parse(...)` | **1 small object**: the `JsonTape` itself, which isn't pooled (§10.1). The index and any input copy are pooled and returned on `Dispose`; `Parse(string)` doesn't copy. |
| `JNode` DOM | allocates its nodes, by design (§10) |

The rules that keep the result graph tight:

- **Collections are read once, into exact-size storage.** Elements are read into a pooled scratch buffer, then copied into exact-size final storage:
  - `T[]` is allocated at its final length.
  - `List<T>` gets `new List<T>(count)` plus `CollectionsMarshal.SetCount` and a copy.
  - `ImmutableArray<T>` uses `ImmutableCollectionsMarshal.AsImmutableArray`.

  There are no growth reallocations.
- **No boxing or captured lambdas.** Generated code never boxes value types and never captures lambdas or closures.
- **Enum names and member names never allocate.** They are matched against constants, never materialized as strings.
- **Exceptions allocate only on the throwing path.**
- **Measured, not assumed.** `AllocationTests` checks the budget byte for byte: `TryFormat` and buffer writers allocate 0; `ToJson` allocates exactly a string of its length; `FromJson` (UTF-16 and UTF-8) allocates exactly what building the same graph by hand allocates; tape navigation allocates 0. The test project runs with tiered compilation off, because tier-0 code can allocate where optimized code doesn't. Writing these tests found one such case: `MemoryExtensions.IndexOfAnyInRange<char>` allocates until tier-1 replaces it, so the lexer uses a `SearchValues<char>` over the surrogate range instead.

---

## 7. Generated code

### 7.1 Interface

The interface is named `IJsonSerializable<TSelf>` rather than `IJsonModel<TSelf>`, so it never gets confused with `Jakar.SystemTextJson`'s `IJsonModel<TSelf>` when both packages are used together (decision D6).

```csharp
public interface IJsonSerializable<TSelf> : ISpanFormattable, IUtf8SpanFormattable, ISpanParsable<TSelf>, IUtf8SpanParsable<TSelf>
    where TSelf : IJsonSerializable<TSelf>
{
    static abstract void WriteJson<TWriter>( ref TWriter writer, scoped in TSelf value )
        where TWriter : IJsonWriter, allows ref struct;

    static abstract bool TryReadJson<TReader>( ref TReader reader, [MaybeNullWhen(false)] out TSelf value )
        where TReader : IJsonReader, allows ref struct;

    static abstract JsonWriterOptions DefaultWriterOptions { get; }
    static abstract JsonReaderOptions DefaultReaderOptions { get; }
}
```

Every other API is built on these two methods. The generator emits it on the type itself, because static-virtual defaults can't be reached through the concrete type name (the same reason as in `Jakar.SystemTextJson`). The generator skips any method you've already written; see JJSON007.

### 7.2 What `[GenerateJson]` emits

- The core:
  - `WriteJson<TWriter>`
  - `TryReadJson<TReader>`
  - `DefaultWriterOptions` / `DefaultReaderOptions`, from the resolved settings
- The interface implementations:
  - `ISpanFormattable.TryFormat`
  - `IFormattable.ToString(string?, IFormatProvider?)`
  - `IUtf8SpanFormattable.TryFormat`
  - `ISpanParsable<T>.Parse`/`TryParse`, for both `ReadOnlySpan<char>` and `string`
  - `IUtf8SpanParsable<T>.Parse`/`TryParse`
- The helpers in §8, as static and instance members on the type.
- `ToString()`, when `GenerateToString` is on.
- The private `JsonName` table (§3.3), the member-matching functions (§7.3), and an `[UnsafeAccessor]` per init-only member (§7.4).

When a base type is also generated, members that would hide the base's (`Parse`, `FromJson`, `ToJson`, the options, ...) are declared `new`; `TryParse` / `TryFromJson` overload on their `out` type instead.

**`format` and `provider` arguments.** JSON is culture-invariant, so `provider` is **ignored**: honoring it would break I1. `format` accepts `null`/`""` (the type's default), `"c"` (compact) or `"i"` (indented). Anything else throws `FormatException`.

**`TryFormat` with a short buffer.** The writer starts on `destination` itself. If the output fits, it never leaves it, and nothing is copied or allocated. If it doesn't fit, the writer grows into pooled memory, and `TryFormat` returns `false` with `charsWritten = 0`, as the `ISpanFormattable` contract requires, returning the pooled memory.

### 7.3 Example output (abridged)

Generated for the `Invoice` test model (`Jakar.Json.Tests/Models.cs`: camelCase, `UnknownMembers = Error`, `Notes` renamed `memo` with `Order = -1` and `NullValues = Omit`). Converters are composed as type arguments, so every call is static and specialized:

```csharp
// <auto-generated/> Jakar.Json.Generator 1.0.0
partial record Invoice : global::Jakar.Json.IJsonSerializable<global::Jakar.Json.Tests.Invoice>
{
    private static JsonName __JsonName0 => new("\"memo\" : ", "\"memo\" : "u8);
    private static JsonName __JsonName1 => new("\"id\" : ", "\"id\" : "u8);
    // ... total, lines, status, created

    public static void WriteJson<TWriter>( ref TWriter writer, scoped in Invoice value ) where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); return; }
        writer.WriteStartObject();
        if ( value.Notes is not null ) { writer.WritePropertyName(__JsonName0); JsonNullableReferenceConverter<string, JsonStringConverter>.Write(ref writer, value.Notes); }
        writer.WritePropertyName(__JsonName1); JsonGuidConverter.Write(ref writer, value.Id);
        writer.WritePropertyName(__JsonName2); JsonFloatConverter<decimal, JsonNumberModes.Flags0>.Write(ref writer, value.Total);
        writer.WritePropertyName(__JsonName3); JsonListConverter<LineItem, JsonModelConverter<LineItem>>.Write(ref writer, value.Lines);
        writer.WritePropertyName(__JsonName4); global::Jakar.Json.Generated.JsonEnums.Jakar_Json_Tests_Status__1.Write(ref writer, value.Status);
        writer.WritePropertyName(__JsonName5); JsonDateTimeOffsetConverter.Write(ref writer, value.Created);
        writer.WriteEndObject();
    }

    public static bool TryReadJson<TReader>( ref TReader reader, [MaybeNullWhen(false)] out Invoice value ) where TReader : IJsonReader, allows ref struct
    {
        value = default;
        if ( !reader.TryReadStartObject() ) { return false; }
        string? __m0 = default!;  Guid __m1 = default!;  decimal __m2 = default!;  /* ... */
        ulong __seen0 = 0;                                  // one bit per member: duplicates and required members cost nothing
        while ( true )
        {
            if ( !reader.TryReadProperty(out JsonSpan __name, out bool __end) ) { return false; }
            if ( __end ) { break; }
            switch ( __name.IsUtf8 ? __JsonMatch(__name.Utf8) : __JsonMatch(__name.Utf16) )
            {
                case 1:
                {
                    if ( ( __seen0 & 2UL ) != 0 ) { return reader.Fail(JsonErrorKind.DuplicateMember); }
                    if ( !JsonGuidConverter.TryRead(ref reader, out __m1) ) { return false; }
                    __seen0 |= 2UL;
                    break;
                }
                // ... one case per member
                default: return reader.Fail(JsonErrorKind.UnknownMember);   // UnknownMembers = Error
            }
        }
        if ( ( __seen0 & 14UL ) != 14UL ) { return reader.Fail(JsonErrorKind.MissingRequired); }   // id, total, lines
        value = new Invoice(__m1, __m2) { Lines = __m3 };                                         // ctor parameters + `required`
        if ( ( __seen0 & 1UL ) != 0 ) { __JsonInit0(value, __m0); }                             // init-only, only when present
        // ...
        return true;
    }

    // Name matching: a switch on length, then ordinal comparisons against constants, for UTF-16 and UTF-8.
    private static int __JsonMatch( ReadOnlySpan<char> name )
    {
        switch ( name.Length )
        {
            case 2: { if ( MemoryExtensions.SequenceEqual(name, "id") ) { return 1; } return -1; }
            case 5: { if ( MemoryExtensions.SequenceEqual(name, "total") ) { return 2; } if ( MemoryExtensions.SequenceEqual(name, "lines") ) { return 3; } return -1; }
            // ...
        }
        return -1;
    }
    private static int __JsonMatch( ReadOnlySpan<byte> name ) { /* the same, over "..."u8 */ }

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Notes")]
    private static extern void __JsonInit0( Invoice target, string? value );
    // ... options, ISpanFormattable / ISpanParsable (+ UTF-8), ToJson / FromJson helpers, ToString()
}
```

With `NameMatching = OrdinalIgnoreCase`, the comparisons become `MemoryExtensions.Equals(name, "...", StringComparison.OrdinalIgnoreCase)` and `Ascii.EqualsIgnoreCase(name, "..."u8)` (non-ASCII names decode first). Enum codecs are emitted once per assembly in `Jakar.Json.Enums.g.cs`: a `switch` for names (never `Enum.ToString`), `[Flags]` values as `"A, B"` in ascending bit order, and a `switch` on `ReadOnlySpan<char>` for reading.

### 7.4 Supported member types

| Category | Types | JSON |
|---|---|---|
| Primitives | `bool`, all integer types incl. `Int128`/`UInt128`/`BigInteger`, `float`, `double`, `Half`, `decimal`, `char` | number / bool / 1-char string |
| Text | `string`, `char` | string |
| BCL types | `Guid`, `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`, `TimeSpan`, `Uri`, `Version` (§5.3) | string |
| Enums | any | name or number |
| `Nullable<T>` | any supported `T` | `T` or `null` |
| Models | any `[GenerateJson]` type, or any type implementing `IJsonSerializable<T>` (incl. other assemblies) | object |
| Formattable | any `T : ISpanFormattable, ISpanParsable<T>` not covered above (e.g. `Email`, `AppVersion`) | string |
| Sequences | `T[]` (incl. `byte[]`, as numbers), `List<T>`, `IList<T>`, `IReadOnlyList<T>`, `ICollection<T>`, `IReadOnlyCollection<T>`, `IEnumerable<T>` (read as `List<T>`), `ImmutableArray<T>`, `ImmutableList<T>`, `HashSet<T>` / `ISet<T>` / `IReadOnlySet<T>`, `FrozenSet<T>`, `SortedSet<T>`, `Queue<T>`, `Stack<T>` (written top first, read back in order) | array |
| Maps | `Dictionary<K,V>`, `IDictionary`, `IReadOnlyDictionary`, `SortedDictionary`, `OrderedDictionary<K,V>`, `ImmutableDictionary`, `FrozenDictionary`, `ConcurrentDictionary`. Keys: `string`, enums, or any `ISpanFormattable + ISpanParsable` | object |
| Dynamic | `JNode` and its subtypes | any |
| Custom | `[JsonMember(Converter = typeof(C))]` where `C : IJsonConverter<T>` | any |

```csharp
public interface IJsonConverter<T>   // static abstract: called directly, no instance, no reflection
{
    static abstract void Write<TWriter>( ref TWriter writer, scoped in T value ) where TWriter : IJsonWriter, allows ref struct;
    static abstract bool TryRead<TReader>( ref TReader reader, out T value ) where TReader : IJsonReader, allows ref struct;
}
```

Every member is read and written through a converter struct, composed from type arguments: `JsonListConverter<LineItem, JsonModelConverter<LineItem>>`, `JsonDictionaryConverter<Dictionary<Guid, string>, Guid, string, JsonParsableKeyConverter<Guid>, JsonStringConverter, JsonOrders.Comparable<Guid>>`, and so on. Settings that change behaviour (numbers from strings, large integers, NaN, local `DateTime`s, sort order) are marker structs too (`JsonNumberModes.Flags3`, `JsonDateTimeModes.ConvertToUtc`). Shared logic lives in `JsonSequences` and `JsonMaps`. Everything is resolved at compile time, and the JIT specializes each combination.

Reference types are non-nullable unless annotated: a non-nullable member rejects JSON `null` with `InvalidValue`, and `T?` wraps its converter in `JsonNullableReferenceConverter` / `JsonNullableConverter`. Dictionaries reject repeated keys (`DuplicateMember`).

**Construction.** The generator picks the constructor in this order:

1. The constructor marked `[JsonConstructor]`.
2. The primary constructor.
3. The single public constructor.
4. The parameterless constructor.

Constructor parameters bind to members by name, ignoring case; a parameter with a default keeps it when the member is missing. `required` members go in an object initializer (they're always present: a missing one already failed). Every other member is assigned only when it was present in the JSON, so property initializers survive a missing member: settable members directly, init-only members through `[UnsafeAccessor]`, which binds at compile time with no reflection. Getter-only members that aren't constructor parameters are write-only: they're read (validated) and discarded, and JJSON011 is reported at info level.

**Structs.** `WriteJson` takes the value as `scoped in T`, so large structs aren't copied. `ref struct` models are not supported.

**Generics.** A generic model is supported when every member whose type involves a type parameter `T` is constrained so that it can be dispatched statically: `where T : IJsonSerializable<T>`, or `where T : ISpanFormattable, ISpanParsable<T>`. Anything else is error JJSON002.

### 7.5 Polymorphism

```csharp
[GenerateJson(Discriminator = "$type")]
[JsonDerived(typeof(Dog), "dog")]
[JsonDerived(typeof(Cat), "cat")]
public abstract partial class Animal { public string Name { get; init; } = ""; }
```

- **Write.** Dispatch is `value switch { Dog d => …, Cat c => …, _ => error }`, ordered most-derived first, with no `GetType()`. The discriminator is always written first, which is canonical.
- **Read.** The base takes a checkpoint (`reader.Checkpoint()`), scans the object for the discriminator (skipping other values with `TrySkipValue`), rewinds, and hands the whole object to the derived type's `TryReadJson`. Lookahead is cheap because every reader works on a complete buffer (§8.3); the tape and DOM readers support it too. The derived type reads the discriminator as a known member and checks it names itself.
- **Derived types written directly** (`dog.ToJson()`) also write their discriminator first, so they read back through the base.
- **Unknown tag.** An unknown discriminator value, or a missing one on an abstract base, is `JsonErrorKind.UnknownDiscriminator`.

### 7.6 Extension data

`[JsonMember(ExtensionData = true)]` (or STJ's `[JsonExtensionData]`) marks an `OrderedDictionary<string, JNode?>?` or a `JObjectNode?` member. Its presence implies `UnknownMembers = Capture` (an explicit `UnknownMembers = Error` is JJSON008). Unknown members are read into it in input order, as `JNode`s; when writing, they're written after the declared members, in insertion order.

---

## 8. Helpers

### 8.1 Write

Each of these is emitted on the type, and also exists generically on the static `JsonCodec` class, constrained `where T : IJsonSerializable<T>`:

```csharp
string           ToJson( JsonWriterOptions? options = null );
byte[]           ToJsonUtf8( JsonWriterOptions? options = null );
bool             TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider );   // ISpanFormattable
bool             TryFormat( Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider ); // IUtf8SpanFormattable
void             WriteJson( IBufferWriter<byte> output, JsonWriterOptions? options = null );
void             WriteJson( IBufferWriter<char> output, JsonWriterOptions? options = null );
void             WriteJson( ref ValueStringBuilder builder, JsonWriterOptions? options = null );   // append into a caller builder
void             ToJson( Stream stream, JsonWriterOptions? options = null );                       // UTF-8
ValueTask        ToJsonAsync( Stream stream, JsonWriterOptions? options = null, CancellationToken token = default );
void             ToJson( TextWriter writer, JsonWriterOptions? options = null );
```

`JsonCodec` also writes root-level sequences without wrapper types: `ToJsonArray<T>(ReadOnlySpan<T>)`, `ToJsonArray<T>(IEnumerable<T>)`, `ToJsonArrayUtf8<T>` and `ToJsonArray<T>(ReadOnlySpan<T>, Stream)`. This mirrors `JsonModel.Arrays` in `Jakar.SystemTextJson`.

### 8.2 Read

```csharp
static T    FromJson( string json, JsonReaderOptions? options = null );
static T    FromJson( ReadOnlySpan<char> json, JsonReaderOptions? options = null );
static T    FromJson( ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null );
static T    FromJson( ReadOnlySequence<byte> utf8Json, JsonReaderOptions? options = null );   // single-segment fast path, otherwise pooled copy
static T    FromJson( Stream stream, JsonReaderOptions? options = null );
static ValueTask<T> FromJsonAsync( Stream stream, JsonReaderOptions? options = null, CancellationToken token = default );
static T    FromJson( TextReader reader, JsonReaderOptions? options = null );
static bool TryFromJson( [NotNullWhen(true)] string? json, [MaybeNullWhen(false)] out T value, JsonReaderOptions? options = null );   // + ReadOnlySpan<char>, ReadOnlySpan<byte>
static bool TryFromJson( ReadOnlySpan<char> json, out T value, out JsonError error, JsonReaderOptions? options = null );               // JsonCodec: error details without exceptions
// ISpanParsable<T> / IUtf8SpanParsable<T>: Parse / TryParse, with provider ignored
```

All `TryFromJson` overloads are exception-free for malformed input. They still throw `ArgumentNullException` for a null `Stream` or `TextReader`, and they propagate I/O exceptions. `JsonCodec` adds `FromJsonArray<T>` (returns `T[]`), `FromJsonList<T>`, and `FromJsonArrayPooled<T>`, which returns a `JsonPooledArray<T>` to dispose. A thrown `JsonReadException` carries the JSON path; the `Try*` methods never build it.

**NDJSON.** `JsonCodec.ReadLines<T>(Stream)` and `ReadLinesAsync<T>(Stream, CancellationToken)` stream one document per line, as `IEnumerable<T>` and `IAsyncEnumerable<T>`, for files too large to buffer whole (decision D4):

- **Buffering.** Input is read into a pooled buffer, and each complete line is parsed in place with the normal reader. Memory is bounded by the longest line, not the file. A line longer than `MaxDocumentBytes` fails with `DocumentTooLarge`.
- **Line endings.** Both `\n` and `\r\n` work. Blank and whitespace-only lines are skipped. A final line without a newline is still read.
- **Errors** report the line number, and the position within that line.
- **Allocations.** The enumerator is one allocation per call. Results are as for `FromJson`.
- **Writing.** `JsonCodec.WriteLines<T>(Stream, IEnumerable<T>)` writes the matching format: compact JSON per value, each followed by `\n`.

### 8.3 Streams

- **Read.** The stream is read to the end into an `ArrayPool<byte>` buffer. Reading starts at `stream.Length` when `CanSeek` is true, otherwise at 16 KB, and doubles as needed. The buffer is capped at `JsonReaderOptions.MaxDocumentBytes`, which defaults to 64 MB; exceeding it fails with `JsonError.DocumentTooLarge`. After that the parse is synchronous and the buffer is returned. The async variant awaits only `ReadAsync`. This keeps the parser a simple, fast, span-based state machine with cheap lookahead. The trade-off is memory proportional to document size. Line-delimited files avoid it through `ReadLines<T>` (§8.2).
- **Write.** The writer serializes to a pooled UTF-8 buffer and flushes it to the stream in chunks once it passes a threshold (16 KB). The async variant buffers the full output synchronously, then awaits the writes; that is the only way to use `ref struct` writers with `async`.

---

## 9. Errors

```csharp
public enum JsonErrorKind : byte { None, UnexpectedEnd, UnexpectedToken, InvalidNumber, NumberOverflow, InvalidString, InvalidEscape,
                                   InvalidUtf8, DepthExceeded, DuplicateMember, UnknownMember, MissingRequired, UnknownDiscriminator,
                                   NonFiniteNumber, DocumentTooLarge, TrailingData, BufferTooSmall,
                                   InvalidValue,   // a well-formed token the type can't hold: unknown enum name, bad Guid/date, null for a non-nullable member
                                   Converter }

public readonly record struct JsonError( JsonErrorKind Kind, int Position, int Line, int Column );   // no references; line/column computed on failure only

public sealed class JsonReadException  : FormatException            { public JsonError Error { get; } public string Path { get; } }   // Path: "$.lines[3].price"
public sealed class JsonWriteException : InvalidOperationException { public JsonError Error { get; } }
```

`JsonReadException` derives from `FormatException` so that `ISpanParsable.Parse` callers can catch the type they expect. Positions are char offsets for UTF-16 input and byte offsets for UTF-8; NDJSON errors report the stream offset and the line number (§8.2).

---

## 10. Dynamic JSON

There are two tiers, matching two different needs. Both live in `Jakar.Json/Dynamic/`.

The regions below are the shipped sources of `Jakar.Json/Dynamic/`. They build on the interfaces in §3.2, the error types in §9, the shared `JsonLexer<TChar>` and `JsonStreams` helpers, `ValueSpanReader<T>` and `ValueStringBuilder`. The reader and writer adapters that let generated code read and write the DOM are in the same folder: [JsonTapeReader.cs](Dynamic/JsonTapeReader.cs), [JNodeReader.cs](Dynamic/JNodeReader.cs) and [JNodeWriter.cs](Dynamic/JNodeWriter.cs). Files assume these global usings:

`System`, `System.Buffers`, `System.Collections`, `System.Collections.Generic`, `System.Diagnostics`, `System.Diagnostics.CodeAnalysis`, `System.Globalization`, `System.IO`, `System.Numerics`, `System.Runtime.CompilerServices`, `System.Runtime.InteropServices`, `System.Text`, `System.Text.Unicode`, `System.Threading`, `System.Threading.Tasks`, `Jakar.Extensions`.

### 10.1 `JsonTape`: read-only and allocation-free navigation

`JsonTape` validates a whole document and records a structural index over it in one pass, in the spirit of the simdjson tape. The index is a pooled array of 20-byte entries. Each entry holds:

- the kind of token, and whether it contains escapes
- its start offset and length
- the index of the entry after its subtree
- for containers, the number of children

Navigation never allocates:

```csharp
using JsonTape tape = JsonTape.Parse(json);           // the index (and any input copy) is pooled; Dispose returns it
JsonItem root  = tape.Root;
decimal  price = root["lines"][3]["price"].GetDecimal();
foreach ( JsonItem line in root["lines"].EnumerateArray() ) { ... }   // struct enumerator
bool     has   = root.TryGetProperty("memo", out JsonItem memo);
bool     paid  = root["status"].ValueEquals("paid");   // compared in place, no string created
Invoice  model = root.Deserialize<Invoice>();          // JsonTapeReader : IJsonReader, no re-tokenizing (polymorphic lookahead works too)
```

- **Iterative parser.** Each nesting level costs one `int` in a stack-allocated array, pooled beyond 128 levels, rather than a stack frame. A depth bomb fails with `DepthExceeded` instead of overflowing the stack.
- **One parser for both encodings.** The parser is generic over `TChar` (`char` or `byte`) and runs on `ValueSpanReader<TChar>`, with the token-level scanning (`JsonLexer<TChar>`) shared with `JsonReader<TChar>`: the tape and the reader accept and reject exactly the same documents. The JIT specializes it for each encoding.
- **Complete validation.** The parse checks the grammar, escapes, number syntax, UTF-8 (one vectorized `Utf8.IsValid` pass) and raw lone surrogates in UTF-16 input. Strings and numbers are validated during the parse but decoded only when accessed.
- **Input ownership.** `Parse(string)` keeps a reference to the string. Span inputs are copied into a pooled buffer, because the tape outlives the span. Streams are read straight into the pooled buffer (§8.3). `MaxDepth` reports the deepest nesting, which sizes a `JsonTapeReader`'s pooled frame stack.
- **The instance itself isn't pooled.** A pooled instance would let a stale `JsonItem` or a second `Dispose` reach someone else's document; one small allocation per parse avoids that. `Dispose` returns the pooled arrays. After that, any access through the tape or its items throws `ObjectDisposedException`.
- **`JsonItem`** is a 16-byte `readonly struct` holding the tape and an entry index. It's safe to copy.
- **Lookups.** `TryGetProperty` scans the object's members and compares names against the raw input. An escaped name is first unescaped into a stack buffer.
  - The tape doesn't check for duplicate names, because that would cost a hash set per object. A lookup returns the first match. `JNode.Parse` does reject duplicates.
  - `item[i]` on an array walks `i` siblings. `EnumerateArray` costs O(1) per step.
- **Numbers.** They keep their raw text and are parsed on access with invariant culture. `GetInteger<T>` rejects fractions and exponents. `GetFloat<T>` rejects values that overflow to infinity.

<details>
<summary><code>JsonTape</code>, <code>JsonItem</code>, <code>JsonTapeProperty</code></summary>

`Dynamic/JsonTape.cs`

```csharp
// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary>
///     A validated, read-only structural index ("tape") over a JSON document. Navigate it through <see cref="Root"/>; nothing allocates except the strings you ask for.
/// </summary>
/// <remarks>
///     <para> The index and any copy of the input are pooled: always <see cref="Dispose"/> the tape. Every <see cref="JsonItem"/> from it becomes unusable then (<see cref="ObjectDisposedException"/>). </para>
///     <para> Safe for concurrent reads; not safe to read while another thread disposes it. </para>
/// </remarks>
public sealed class JsonTape : IDisposable
{
    private const int STACK_DEPTH = 128; // nesting levels tracked on the stack; deeper documents rent the depth stack


    private Entry[]? __entries;
    private int      __count;
    private string?  __string; // Parse(string): the caller's string, not copied
    private char[]?  __chars;  // Parse(ReadOnlySpan<char>): a pooled copy
    private byte[]?  __bytes;  // UTF-8 input: a pooled copy, or the pooled stream buffer
    private int      __length;
    private bool     __utf8;
    private int      __maxDepthSeen;


    /// <summary> Whether the input was UTF-8 (positions are byte offsets) rather than UTF-16 (char offsets). </summary>
    public bool     IsUtf8 => __utf8;
    /// <summary> The input's length, in <see cref="char"/>s or <see cref="byte"/>s. </summary>
    public int      Length => __length;
    /// <summary> The deepest nesting in the document. </summary>
    public int      MaxDepth => __maxDepthSeen;
    /// <summary> The document's root value. </summary>
    public JsonItem Root   { get { _ = Entries; return new JsonItem(this, 0); } }


    internal int                Count      => __count;
    internal Entry[]            Entries    => __entries ?? throw new ObjectDisposedException(nameof(JsonTape));
    internal ReadOnlySpan<char> InputUtf16 => __string is not null ? __string.AsSpan() : __chars.AsSpan(0, __length);
    internal ReadOnlySpan<byte> InputUtf8  => __bytes.AsSpan(0, __length);


    private JsonTape() { }


    // ─── Parse ───────────────────────────────────────────────────────────────

    /// <exception cref="JsonReadException"> The JSON is malformed. </exception>
    public static JsonTape Parse( string json, JsonReaderOptions? options = null )
    {
        ArgumentNullException.ThrowIfNull(json);
        JsonTape tape = new() { __string = json, __length = json.Length };
        return tape.BuildOrThrow(json.AsSpan(), options ?? JsonReaderOptions.Default);
    }

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static JsonTape Parse( ReadOnlySpan<char> json, JsonReaderOptions? options = null )
    {
        char[] copy = ArrayPool<char>.Shared.Rent(json.Length);
        json.CopyTo(copy);
        JsonTape tape = new() { __chars = copy, __length = json.Length };
        return tape.BuildOrThrow<char>(copy.AsSpan(0, json.Length), options ?? JsonReaderOptions.Default);
    }

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static JsonTape Parse( ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null )
    {
        byte[] copy = ArrayPool<byte>.Shared.Rent(utf8Json.Length);
        utf8Json.CopyTo(copy);
        return FromPooledUtf8(copy, utf8Json.Length, options ?? JsonReaderOptions.Default);
    }

    /// <summary> Reads <paramref name="utf8Json"/> to the end into a pooled buffer (at most <see cref="JsonReaderOptions.MaxDocumentBytes"/>), then parses it. </summary>
    /// <exception cref="JsonReadException"> The JSON is malformed or too large. </exception>
    public static JsonTape Parse( Stream utf8Json, JsonReaderOptions? options = null )
    {
        JsonReaderOptions settings = options ?? JsonReaderOptions.Default;
        byte[]            buffer   = JsonStreams.ReadAll(utf8Json, settings.MaxDocumentBytes, out int length);
        return FromPooledUtf8(buffer, length, settings);
    }

    /// <inheritdoc cref="Parse(Stream, JsonReaderOptions?)"/>
    public static async ValueTask<JsonTape> ParseAsync( Stream utf8Json, JsonReaderOptions? options = null, CancellationToken token = default )
    {
        JsonReaderOptions settings = options ?? JsonReaderOptions.Default;
        (byte[] buffer, int length) = await JsonStreams.ReadAllAsync(utf8Json, settings.MaxDocumentBytes, token).ConfigureAwait(false);
        return FromPooledUtf8(buffer, length, settings);
    }


    /// <summary> Exception-free: <see langword="false"/> with <paramref name="error"/> for <see langword="null"/> or malformed input. </summary>
    public static bool TryParse( [NotNullWhen(true)] string? json, [NotNullWhen(true)] out JsonTape? tape, out JsonError error, JsonReaderOptions? options = null )
    {
        if ( json is null )
        {
            tape  = null;
            error = new JsonError(JsonErrorKind.UnexpectedEnd, 0, 1, 1);
            return false;
        }

        JsonTape result = new() { __string = json, __length = json.Length };
        return Finish(result, result.TryBuild(json.AsSpan(), options ?? JsonReaderOptions.Default, out error), out tape);
    }

    /// <inheritdoc cref="TryParse(string, out JsonTape, out JsonError, JsonReaderOptions?)"/>
    public static bool TryParse( ReadOnlySpan<byte> utf8Json, [NotNullWhen(true)] out JsonTape? tape, out JsonError error, JsonReaderOptions? options = null )
    {
        byte[] copy = ArrayPool<byte>.Shared.Rent(utf8Json.Length);
        utf8Json.CopyTo(copy);

        JsonTape result = new() { __bytes = copy, __length = utf8Json.Length, __utf8 = true };
        return Finish(result, result.TryBuild<byte>(copy.AsSpan(0, utf8Json.Length), options ?? JsonReaderOptions.Default, out error), out tape);
    }


    /// <summary> Returns the pooled index and input copy. Safe to call more than once. </summary>
    public void Dispose()
    {
        Entry[]? entries = __entries;
        if ( entries is null ) { return; }

        __entries = null;
        __count   = 0;
        __string  = null;
        ArrayPool<Entry>.Shared.Return(entries);

        if ( __chars is not null )
        {
            ArrayPool<char>.Shared.Return(__chars);
            __chars = null;
        }

        if ( __bytes is not null )
        {
            ArrayPool<byte>.Shared.Return(__bytes);
            __bytes = null;
        }
    }


    // ─── Build ───────────────────────────────────────────────────────────────

    private static JsonTape FromPooledUtf8( byte[] buffer, int length, in JsonReaderOptions options )
    {
        JsonTape tape = new() { __bytes = buffer, __length = length, __utf8 = true };
        return tape.BuildOrThrow<byte>(buffer.AsSpan(0, length), options);
    }

    private JsonTape BuildOrThrow<TChar>( ReadOnlySpan<TChar> input, in JsonReaderOptions options )
        where TChar : unmanaged, IBinaryInteger<TChar>
    {
        if ( TryBuild(input, options, out JsonError error) ) { return this; }

        Dispose();
        throw new JsonReadException(error);
    }

    private bool TryBuild<TChar>( ReadOnlySpan<TChar> input, in JsonReaderOptions options, out JsonError error )
        where TChar : unmanaged, IBinaryInteger<TChar>
    {
        __entries = ArrayPool<Entry>.Shared.Rent(Math.Clamp(input.Length / 8, 16, 1 << 16));
        __count   = 0;
        return Parser<TChar>.TryParse(this, input, options, out error);
    }

    private static bool Finish( JsonTape result, bool built, [NotNullWhen(true)] out JsonTape? tape )
    {
        if ( built )
        {
            tape = result;
            return true;
        }

        result.Dispose();
        tape = null;
        return false;
    }


    private int Add( JsonTokenKind kind, int start, int length, bool escaped )
    {
        Entry[] entries = __entries!;
        int     index   = __count;
        if ( index == entries.Length ) { entries = GrowEntries(); }

        entries[index] = new Entry(kind, escaped, start, length, index + 1);
        __count        = index + 1;
        return index;
    }

    [MethodImpl(MethodImplOptions.NoInlining)] private Entry[] GrowEntries()
    {
        Entry[] old    = __entries!;
        Entry[] larger = ArrayPool<Entry>.Shared.Rent(old.Length * 2); // entries never outnumber input chars, so this can't overflow
        old.AsSpan(0, __count).CopyTo(larger);
        ArrayPool<Entry>.Shared.Return(old);
        return __entries = larger;
    }




    // ─── Access (used by JsonItem and JNode) ─────────────────────────────────

    internal JsonError CreateError( JsonErrorKind kind, int position ) => __utf8
                                                                              ? JsonLexer<byte>.CreateError(kind, position, InputUtf8)
                                                                              : JsonLexer<char>.CreateError(kind, position, InputUtf16);

    internal string GetString( in Entry entry )
    {
        if ( !entry.Escaped )
        {
            return __utf8
                       ? Encoding.UTF8.GetString(InputUtf8.Slice(entry.Start, entry.Length))
                       : new string(InputUtf16.Slice(entry.Start, entry.Length));
        }

        ValueStringBuilder builder = new(stackalloc char[256]);
        Unescape(in entry, ref builder);
        return builder.ToString();
    }

    internal bool TryCopyString( in Entry entry, Span<char> destination, out int charsWritten )
    {
        if ( !entry.Escaped )
        {
            if ( __utf8 )
            {
                bool done = Utf8.ToUtf16(InputUtf8.Slice(entry.Start, entry.Length), destination, out _, out charsWritten) == OperationStatus.Done;
                if ( !done ) { charsWritten = 0; }

                return done;
            }

            ReadOnlySpan<char> text   = InputUtf16.Slice(entry.Start, entry.Length);
            bool               copied = text.TryCopyTo(destination);
            charsWritten = copied ? text.Length : 0;
            return copied;
        }

        ValueStringBuilder builder = new(stackalloc char[256]);
        Unescape(in entry, ref builder);
        bool fits = builder.Values.TryCopyTo(destination);
        charsWritten = fits ? builder.Length : 0;
        builder.Dispose();
        return fits;
    }

    /// <summary>
    ///     Compares a string or property name with a value, without allocating. An unescaped UTF-8 entry is compared with <paramref name="utf8"/>;
    ///     every other entry is compared with <paramref name="utf16"/>, unescaping into a stack buffer first if it has to.
    /// </summary>
    internal bool StringEquals( in Entry entry, ReadOnlySpan<char> utf16, ReadOnlySpan<byte> utf8 )
    {
        if ( !entry.Escaped )
        {
            return __utf8
                       ? InputUtf8.Slice(entry.Start, entry.Length).SequenceEqual(utf8)
                       : InputUtf16.Slice(entry.Start, entry.Length).SequenceEqual(utf16);
        }

        if ( utf16.Length > entry.Length ) { return false; } // unescaping (and UTF-8 → UTF-16) only ever shrinks the text

        ValueStringBuilder builder = new(stackalloc char[256]);
        Unescape(in entry, ref builder);
        bool equal = builder.Values.SequenceEqual(utf16);
        builder.Dispose();
        return equal;
    }

    internal string GetRawText( in Entry entry )
    {
        bool quoted = entry.Kind is JsonTokenKind.String or JsonTokenKind.PropertyName;
        int  start  = quoted ? entry.Start  - 1 : entry.Start;
        int  length = quoted ? entry.Length + 2 : entry.Length;

        return __utf8
                   ? Encoding.UTF8.GetString(InputUtf8.Slice(start, length))
                   : new string(InputUtf16.Slice(start, length));
    }

    private void Unescape( in Entry entry, ref ValueStringBuilder builder )
    {
        if ( __utf8 ) { JsonLexer<byte>.Unescape(InputUtf8.Slice(entry.Start, entry.Length), ref builder); }
        else { JsonLexer<char>.Unescape(InputUtf16.Slice(entry.Start, entry.Length), ref builder); }
    }


    /// <summary> Whether <paramref name="text"/> is exactly one JSON number (RFC 8259 grammar). </summary>
    internal static bool IsValidNumber( ReadOnlySpan<char> text ) => JsonLexer<char>.IsNumber(text);


    // ─── Entry ───────────────────────────────────────────────────────────────

    /// <summary> One token of the tape. Containers are completed (<see cref="Length"/>, <see cref="Next"/>) when their closing bracket is read. </summary>
    [StructLayout(LayoutKind.Auto)]
    internal struct Entry( JsonTokenKind kind, bool escaped, int start, int length, int next )
    {
        public readonly JsonTokenKind Kind    = kind;
        public readonly bool          Escaped = escaped; // strings and names that contain a backslash
        public readonly int           Start   = start;   // strings and names: just after the opening quote; others: the first char/byte
        public          int           Length  = length;  // strings and names: the content (no quotes); containers: through the closing bracket
        public          int           Next    = next;    // the entry after this value's subtree (the next sibling, or the parent's successor)
        public          int           Count;             // containers: members or elements
    }


    // ─── Parser ──────────────────────────────────────────────────────────────

    /// <summary> Validates and indexes JSON in one forward pass over <see cref="char"/> or <see cref="byte"/> input, using <see cref="JsonLexer{TChar}"/>. Iterative: nesting costs an <see cref="int"/>, not a stack frame. </summary>
    private static class Parser<TChar>
        where TChar : unmanaged, IBinaryInteger<TChar>
    {
        public static bool TryParse( JsonTape tape, ReadOnlySpan<TChar> input, in JsonReaderOptions options, out JsonError error )
        {
            if ( typeof(TChar) == typeof(byte) && !Utf8.IsValid(MemoryMarshal.Cast<TChar, byte>(input)) )
            {
                error = tape.CreateError(JsonErrorKind.InvalidUtf8, JsonText.IndexOfInvalidUtf8(MemoryMarshal.Cast<TChar, byte>(input)));
                return false;
            }

            int       maxDepth = options.MaxDepth;
            int[]?    rented   = null;
            Span<int> stack    = maxDepth <= STACK_DEPTH ? stackalloc int[STACK_DEPTH] : rented = ArrayPool<int>.Shared.Rent(maxDepth);

            try
            {
                JsonErrorKind kind = Run(tape, input, options, stack[..maxDepth], out int position);

                error = kind == JsonErrorKind.None
                            ? default
                            : tape.CreateError(kind, position);

                return kind == JsonErrorKind.None;
            }
            finally
            {
                if ( rented is not null ) { ArrayPool<int>.Shared.Return(rented); }
            }
        }


        private static JsonErrorKind Run( JsonTape tape, ReadOnlySpan<TChar> input, in JsonReaderOptions options, Span<int> stack, out int position )
        {
            ValueSpanReader<TChar> reader         = new(input);
            bool                   comments       = options.AllowComments;
            bool                   trailingCommas = options.AllowTrailingCommas;
            int                    maxString      = options.MaxStringLength;
            int                    depth          = 0;
            bool                   expectValue    = true;
            JsonErrorKind          error;

            reader.TryReadExact(JsonLexer<TChar>.Literal("﻿"u8, "﻿")); // a leading byte order mark

            while ( true )
            {
                if ( ( error = JsonLexer<TChar>.SkipTrivia(ref reader, comments) ) != JsonErrorKind.None ) { goto Fail; }

                if ( expectValue )
                {
                    if ( !reader.TryPeek(out TChar next) )
                    {
                        error = JsonErrorKind.UnexpectedEnd;
                        goto Fail;
                    }

                    int start = reader.Position;

                    switch ( JsonLexer<TChar>.U(next) )
                    {
                        case '{':
                        case '[':
                        {
                            if ( depth == stack.Length )
                            {
                                error = JsonErrorKind.DepthExceeded;
                                goto Fail;
                            }

                            bool isObject = JsonLexer<TChar>.U(next) == '{';
                            stack[depth] = AddValue(tape, stack, depth, isObject ? JsonTokenKind.Object : JsonTokenKind.Array, start, 0, false);
                            depth++;
                            if ( depth > tape.__maxDepthSeen ) { tape.__maxDepthSeen = depth; }

                            reader.Advance(1);

                            if ( ( error = JsonLexer<TChar>.SkipTrivia(ref reader, comments) ) != JsonErrorKind.None ) { goto Fail; }

                            if ( reader.TryReadExact(JsonLexer<TChar>.Char(isObject ? '}' : ']')) ) // an empty container is a complete value
                            {
                                Close(tape, stack[--depth], reader.Position);
                                expectValue = false;
                                continue;
                            }

                            if ( isObject && ( error = ReadMemberName(tape, ref reader, comments, maxString, stack[depth - 1]) ) != JsonErrorKind.None ) { goto Fail; }

                            continue; // expectValue stays true: the first element, or the first member's value
                        }

                        case '"':
                        {
                            if ( ( error = JsonLexer<TChar>.ReadString(ref reader, maxString, out int contentStart, out int length, out bool escaped) ) != JsonErrorKind.None ) { goto Fail; }

                            AddValue(tape, stack, depth, JsonTokenKind.String, contentStart, length, escaped);
                            break;
                        }

                        case '-' or ( >= '0' and <= '9' ):
                        {
                            if ( ( error = JsonLexer<TChar>.ReadNumber(ref reader) ) != JsonErrorKind.None ) { goto Fail; }

                            AddValue(tape, stack, depth, JsonTokenKind.Number, start, reader.Position - start, false);
                            break;
                        }

                        case 't' when reader.TryReadExact(JsonLexer<TChar>.Literal("true"u8, "true")):
                            AddValue(tape, stack, depth, JsonTokenKind.True, start, 4, false);
                            break;

                        case 'f' when reader.TryReadExact(JsonLexer<TChar>.Literal("false"u8, "false")):
                            AddValue(tape, stack, depth, JsonTokenKind.False, start, 5, false);
                            break;

                        case 'n' when reader.TryReadExact(JsonLexer<TChar>.Literal("null"u8, "null")):
                            AddValue(tape, stack, depth, JsonTokenKind.Null, start, 4, false);
                            break;

                        default:
                            error = JsonErrorKind.UnexpectedToken;
                            goto Fail;
                    }

                    expectValue = false;
                    continue;
                }

                // After a value: the end of the document, a separator, or the end of the enclosing container.
                if ( depth == 0 )
                {
                    if ( !reader.End )
                    {
                        error = JsonErrorKind.TrailingData;
                        goto Fail;
                    }

                    position = reader.Position;
                    return JsonErrorKind.None;
                }

                int  container = stack[depth - 1];
                bool inObject  = tape.Entries[container].Kind == JsonTokenKind.Object;
                char close     = inObject ? '}' : ']';

                if ( reader.TryReadExact(JsonLexer<TChar>.Char(',')) )
                {
                    if ( ( error = JsonLexer<TChar>.SkipTrivia(ref reader, comments) ) != JsonErrorKind.None ) { goto Fail; }

                    if ( trailingCommas && reader.TryReadExact(JsonLexer<TChar>.Char(close)) )
                    {
                        Close(tape, container, reader.Position);
                        depth--;
                        continue;
                    }

                    if ( inObject && ( error = ReadMemberName(tape, ref reader, comments, maxString, container) ) != JsonErrorKind.None ) { goto Fail; }

                    expectValue = true;
                    continue;
                }

                if ( reader.TryReadExact(JsonLexer<TChar>.Char(close)) )
                {
                    Close(tape, container, reader.Position);
                    depth--;
                    continue;
                }

                error = reader.End ? JsonErrorKind.UnexpectedEnd : JsonErrorKind.UnexpectedToken;
                goto Fail;
            }

            Fail:
            position = reader.Position;
            return error;
        }


        /// <summary> Adds a value; a value directly inside an array counts as one of its elements (object members are counted by their names). </summary>
        private static int AddValue( JsonTape tape, Span<int> stack, int depth, JsonTokenKind kind, int start, int length, bool escaped )
        {
            if ( depth > 0 )
            {
                ref Entry parent = ref tape.Entries[stack[depth - 1]];
                if ( parent.Kind == JsonTokenKind.Array ) { parent.Count++; } // before Add, which may move the entries
            }

            return tape.Add(kind, start, length, escaped);
        }

        private static void Close( JsonTape tape, int container, int end )
        {
            ref Entry entry = ref tape.Entries[container];
            entry.Length = end - entry.Start;
            entry.Next   = tape.__count;
        }


        /// <summary> <c>"name"</c>, optional trivia, then <c>:</c>. </summary>
        private static JsonErrorKind ReadMemberName( JsonTape tape, ref ValueSpanReader<TChar> reader, bool comments, int maxString, int container )
        {
            if ( !reader.IsNext(JsonLexer<TChar>.Char('"')) ) { return reader.End ? JsonErrorKind.UnexpectedEnd : JsonErrorKind.UnexpectedToken; }

            JsonErrorKind error = JsonLexer<TChar>.ReadString(ref reader, maxString, out int start, out int length, out bool escaped);
            if ( error != JsonErrorKind.None ) { return error; }

            tape.Entries[container].Count++;
            tape.Add(JsonTokenKind.PropertyName, start, length, escaped);

            if ( ( error = JsonLexer<TChar>.SkipTrivia(ref reader, comments) ) != JsonErrorKind.None ) { return error; }

            return reader.TryReadExact(JsonLexer<TChar>.Char(':'))
                       ? JsonErrorKind.None
                       : reader.End
                           ? JsonErrorKind.UnexpectedEnd
                           : JsonErrorKind.UnexpectedToken;
        }
    }
}



/// <summary> One value in a <see cref="JsonTape"/>: the tape plus an entry index (16 bytes, safe to copy). Reading it after the tape is disposed throws <see cref="ObjectDisposedException"/>. </summary>
public readonly struct JsonItem
{
    private readonly JsonTape? __tape;
    private readonly int       __index;


    internal JsonItem( JsonTape tape, int index )
    {
        __tape  = tape;
        __index = index;
    }


    internal JsonTape                         Tape => __tape ?? throw new InvalidOperationException("default(JsonItem) doesn't belong to a tape.");
    private  ref readonly JsonTape.Entry      Data => ref Tape.Entries[__index];

    public JsonTokenKind Kind     => Data.Kind;
    /// <summary> The offset of the value's first char or byte (a string's opening quote). </summary>
    public int           Position => Data.Kind is JsonTokenKind.String or JsonTokenKind.PropertyName ? Data.Start - 1 : Data.Start;
    public bool          IsNull   => Data.Kind == JsonTokenKind.Null;


    // ─── Objects ─────────────────────────────────────────────────────────────

    /// <exception cref="KeyNotFoundException"> The object has no such member. </exception>
    public JsonItem this[ string name ] => TryGetProperty(name, out JsonItem value)
                                               ? value
                                               : throw new KeyNotFoundException($"The object has no member '{name}'.");

    public int GetPropertyCount() => Expect(JsonTokenKind.Object).Count;

    /// <summary> The value of the first member named <paramref name="name"/> (the tape doesn't reject duplicate names). </summary>
    public bool TryGetProperty( ReadOnlySpan<char> name, out JsonItem value )
    {
        if ( !Tape.IsUtf8 ) { return Find(name, default, out value); }

        // A UTF-8 tape compares unescaped names byte for byte: encode the name once, not once per member.
        int        max    = name.Length * 3;
        byte[]?    rented = null;
        Span<byte> utf8   = max <= 256 ? stackalloc byte[256] : ( rented = ArrayPool<byte>.Shared.Rent(max) );

        try
        {
            Utf8.FromUtf16(name, utf8, out _, out int written);
            return Find(name, utf8[..written], out value);
        }
        finally
        {
            if ( rented is not null ) { ArrayPool<byte>.Shared.Return(rented); }
        }
    }

    /// <inheritdoc cref="TryGetProperty(ReadOnlySpan{char}, out JsonItem)"/>
    public bool TryGetProperty( ReadOnlySpan<byte> utf8Name, out JsonItem value )
    {
        char[]?    rented = null;
        Span<char> utf16  = utf8Name.Length <= 256 ? stackalloc char[256] : ( rented = ArrayPool<char>.Shared.Rent(utf8Name.Length) );

        try
        {
            if ( Utf8.ToUtf16(utf8Name, utf16, out _, out int written) != OperationStatus.Done )
            {
                value = default; // invalid UTF-8 can't name a member of a validated document
                return false;
            }

            return Find(utf16[..written], utf8Name, out value);
        }
        finally
        {
            if ( rented is not null ) { ArrayPool<char>.Shared.Return(rented); }
        }
    }

    private bool Find( ReadOnlySpan<char> utf16, ReadOnlySpan<byte> utf8, out JsonItem value )
    {
        ref readonly JsonTape.Entry entry   = ref Expect(JsonTokenKind.Object);
        JsonTape                    tape    = __tape!;
        JsonTape.Entry[]            entries = tape.Entries;

        // Members are (name, value) pairs: the name at i, its value at i + 1, the next name where the value's subtree ends.
        for ( int i = __index + 1, n = 0; n < entry.Count; n++, i = entries[i + 1].Next )
        {
            if ( !tape.StringEquals(in entries[i], utf16, utf8) ) { continue; }

            value = new JsonItem(tape, i + 1);
            return true;
        }

        value = default;
        return false;
    }

    public ObjectEnumerator EnumerateObject() => new(Tape, __index + 1, Expect(JsonTokenKind.Object).Count);


    // ─── Arrays ──────────────────────────────────────────────────────────────

    /// <summary> O(<paramref name="index"/>): walks the preceding elements. Use <see cref="EnumerateArray"/> to visit them all. </summary>
    public JsonItem this[ int index ]
    {
        get
        {
            ref readonly JsonTape.Entry entry = ref Expect(JsonTokenKind.Array);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)entry.Count, nameof(index));

            JsonTape.Entry[] entries = __tape!.Entries;
            int              i       = __index + 1;
            while ( index-- > 0 ) { i = entries[i].Next; }

            return new JsonItem(__tape, i);
        }
    }

    public int GetArrayLength() => Expect(JsonTokenKind.Array).Count;

    public ArrayEnumerator EnumerateArray() => new(Tape, __index + 1, Expect(JsonTokenKind.Array).Count);


    // ─── Scalars ─────────────────────────────────────────────────────────────

    public bool GetBoolean() => Data.Kind switch
                                {
                                    JsonTokenKind.True  => true,
                                    JsonTokenKind.False => false,
                                    var kind            => throw KindMismatch(kind, JsonTokenKind.True)
                                };

    /// <summary> Allocates the string (escapes decoded). Works on property names too. </summary>
    public string GetString() => Tape.GetString(in ExpectText());

    /// <summary> Decodes the string into <paramref name="destination"/>, without allocating; <see langword="false"/> if it doesn't fit. </summary>
    public bool TryCopyString( Span<char> destination, out int charsWritten ) => Tape.TryCopyString(in ExpectText(), destination, out charsWritten);

    /// <summary> Whether the string (or property name) equals <paramref name="text"/>, without allocating. </summary>
    public bool ValueEquals( ReadOnlySpan<char> text )
    {
        ref readonly JsonTape.Entry entry = ref ExpectText();
        JsonTape                    tape  = __tape!;
        if ( !tape.IsUtf8 || entry.Escaped ) { return tape.StringEquals(in entry, text, default); }

        int        max    = text.Length * 3;
        byte[]?    rented = null;
        Span<byte> utf8   = max <= 256 ? stackalloc byte[256] : ( rented = ArrayPool<byte>.Shared.Rent(max) );

        try
        {
            Utf8.FromUtf16(text, utf8, out _, out int written);
            return tape.StringEquals(in entry, text, utf8[..written]);
        }
        finally
        {
            if ( rented is not null ) { ArrayPool<byte>.Shared.Return(rented); }
        }
    }

    /// <summary> Parses the string with <typeparamref name="T"/>'s invariant-culture parser (<see cref="Guid"/>, <see cref="DateTimeOffset"/>, <c>Email</c>, ...) without allocating it. </summary>
    public T GetParsable<T>()
        where T : ISpanParsable<T>
    {
        ref readonly JsonTape.Entry entry  = ref ExpectText();
        char[]?                     rented = null;
        Span<char>                  buffer = entry.Length <= 256 ? stackalloc char[256] : ( rented = ArrayPool<char>.Shared.Rent(entry.Length) ); // decoding never grows the text

        try
        {
            __tape!.TryCopyString(in entry, buffer, out int written);
            return T.Parse(buffer[..written], CultureInfo.InvariantCulture);
        }
        finally
        {
            if ( rented is not null ) { ArrayPool<char>.Shared.Return(rented); }
        }
    }


    public int     GetInt32()   => GetInteger<int>();
    public long    GetInt64()   => GetInteger<long>();
    public ulong   GetUInt64()  => GetInteger<ulong>();
    public double  GetDouble()  => GetFloat<double>();
    public decimal GetDecimal() => GetFloat<decimal>();

    /// <exception cref="FormatException"> The number has a fraction or exponent, or doesn't fit <typeparamref name="T"/>. </exception>
    public T GetInteger<T>()
        where T : struct, IBinaryInteger<T> => TryGetInteger(out T value)
                                           ? value
                                           : throw new FormatException($"{GetRawText()} doesn't fit the integer type (a fraction, an exponent or out of range).");

    /// <exception cref="FormatException"> The number overflows <typeparamref name="T"/>. </exception>
    public T GetFloat<T>()
        where T : struct, IFloatingPoint<T> => TryGetFloat(out T value)
                                           ? value
                                           : throw new FormatException($"{GetRawText()} overflows the floating-point type.");

    /// <summary> Strict: <c>1.0</c> and <c>1e2</c> aren't integers. </summary>
    public bool TryGetInteger<T>( out T value )
        where T : struct, IBinaryInteger<T> => TryParseNumber(NumberStyles.AllowLeadingSign, out value);

    /// <summary> <see langword="false"/> for values that would overflow to infinity (spec: overflow is an error, never saturated). </summary>
    public bool TryGetFloat<T>( out T value )
        where T : struct, IFloatingPoint<T> => TryParseNumber(NumberStyles.Float, out value) && !T.IsInfinity(value);

    private bool TryParseNumber<T>( NumberStyles style, out T value )
        where T : struct, INumberBase<T>
    {
        ref readonly JsonTape.Entry entry = ref Expect(JsonTokenKind.Number);
        JsonTape                    tape  = __tape!;

        return tape.IsUtf8
                   ? T.TryParse(tape.InputUtf8.Slice(entry.Start, entry.Length), style, CultureInfo.InvariantCulture, out value)
                   : T.TryParse(tape.InputUtf16.Slice(entry.Start, entry.Length), style, CultureInfo.InvariantCulture, out value);
    }


    // ─── Whole values ────────────────────────────────────────────────────────

    /// <summary> The value's JSON text, exactly as it appears in the input. </summary>
    public string GetRawText() => Tape.GetRawText(in Data);

    /// <summary> Reads a model straight from the tape (no re-tokenizing). </summary>
    /// <exception cref="JsonReadException"> The value doesn't match <typeparamref name="T"/>. </exception>
    public T Deserialize<T>()
        where T : IJsonSerializable<T>
    {
        JsonTapeReader reader = new(Tape, __index); // §3.2 (P4)
        return T.TryReadJson(ref reader, out T? value)
                   ? value
                   : throw new JsonReadException(reader.Error);
    }

    public override string ToString() => __tape is null ? "" : GetRawText();


    // ─── Helpers ─────────────────────────────────────────────────────────────

    private ref readonly JsonTape.Entry Expect( JsonTokenKind kind )
    {
        ref readonly JsonTape.Entry entry = ref Data;
        if ( entry.Kind != kind ) { throw KindMismatch(entry.Kind, kind); }

        return ref entry;
    }

    private ref readonly JsonTape.Entry ExpectText()
    {
        ref readonly JsonTape.Entry entry = ref Data;
        if ( entry.Kind is not (JsonTokenKind.String or JsonTokenKind.PropertyName) ) { throw KindMismatch(entry.Kind, JsonTokenKind.String); }

        return ref entry;
    }

    private static InvalidOperationException KindMismatch( JsonTokenKind actual, JsonTokenKind expected ) => new($"The JSON value is {actual}, not {expected}.");


    // ─── Enumerators ─────────────────────────────────────────────────────────

    public struct ArrayEnumerator
    {
        private readonly JsonTape __tape;
        private          int      __next;
        private          int      __remaining;

        internal ArrayEnumerator( JsonTape tape, int first, int count )
        {
            __tape      = tape;
            __next      = first;
            __remaining = count;
            Current     = default;
        }

        public JsonItem Current { get; private set; }

        public readonly ArrayEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            if ( __remaining == 0 ) { return false; }

            __remaining--;
            Current = new JsonItem(__tape, __next);
            __next  = __tape.Entries[__next].Next;
            return true;
        }
    }


    public struct ObjectEnumerator
    {
        private readonly JsonTape __tape;
        private          int      __next;
        private          int      __remaining;

        internal ObjectEnumerator( JsonTape tape, int first, int count )
        {
            __tape      = tape;
            __next      = first;
            __remaining = count;
            Current     = default;
        }

        public JsonTapeProperty Current { get; private set; }

        public readonly ObjectEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            if ( __remaining == 0 ) { return false; }

            __remaining--;
            Current = new JsonTapeProperty(new JsonItem(__tape, __next), new JsonItem(__tape, __next + 1));
            __next  = __tape.Entries[__next + 1].Next;
            return true;
        }
    }
}



/// <summary> One object member of a <see cref="JsonTape"/>: its name (a <see cref="JsonTokenKind.PropertyName"/> item) and its value. </summary>
public readonly struct JsonTapeProperty( JsonItem name, JsonItem value )
{
    public JsonItem Name  { get; } = name;
    public JsonItem Value { get; } = value;

    public string GetName()                           => Name.GetString();
    public bool   NameEquals( ReadOnlySpan<char> text ) => Name.ValueEquals(text);
}
```

</details>

### 10.2 `JNode`: mutable, Newtonsoft-style

`JNode` plays the role of Newtonsoft's `JToken`, with names chosen to avoid collisions (decision D3). It allocates by design, since the nodes *are* user data. `JNode.Parse` builds the tree from a `JsonTape`, so validation, error positions and limits are exactly those of §10.1.

```csharp
JObjectNode order = JObjectNode.Parse(json);           // throws if the root isn't an object
order["status"]           = "paid";                    // implicit from string, bool, numbers, Guid, dates, ...
order["lines"]![0]!["qty"] = 3;                        // indexers return JNode? (JSON null / missing member)
int     qty   = (int)order["lines"]![0]!["qty"];       // explicit conversions; InvalidCastException on null or a kind mismatch
string? memo  = (string?)order["memo"];                // missing member → null, as in Newtonsoft
Invoice typed = order.ToModel<Invoice>();              // JNodeReader : IJsonReader
JNode?  node  = JNode.FromModel(invoice);              // JNodeWriter : IJsonWriter
JNode   copy  = order.DeepClone();
bool    same  = JNode.DeepEquals(order, copy);         // true
string  path  = order["lines"]![0]!["qty"]!.Path;      // "$.lines[0].qty"
string  text  = order.ToJson();                        // canonical (§5), member order preserved
JNode   lines = order["lines"]!.Detach();              // removed from order; order no longer keeps it alive, and vice versa
```

| Type | Holds |
|---|---|
| `JObjectNode` | `OrderedDictionary<string, JNode?>`, with ordinal keys in insertion order (the canonical order for the DOM) |
| `JArrayNode` | `List<JNode?>` |
| `JValueNode` | Immutable. Holds a kind, a `string` (the string value, or a number's JSON text), and 16 bytes of typed-number or boolean bits. No boxing. |

- **`null`.** JSON `null` is a `null` reference, as in System.Text.Json. A missing member also reads as `null`, as in Newtonsoft. Use `ContainsKey` to tell the two apart. `Parse` returns `null` for a `null` root.
- **No cycles.** A node has at most one `Parent`. Adding a node that already has a parent, or adding a node to itself or one of its descendants, throws `InvalidOperationException`; add a `DeepClone()` instead, or move the node with `Detach()`. Since a tree can never contain a cycle, writing, cloning and comparing always terminate. Recursion is guarded by `RuntimeHelpers.EnsureSufficientExecutionStack`, so a very deep tree that was built in code throws `InsufficientExecutionStackException` instead of crashing the process.
- **Numbers.**
  - Parsed numbers keep their exact text, so `1.10` stays `1.10`. `JValueNode.FromNumberText` does the same for numbers from code, after checking them against the JSON grammar.
  - Numbers created from CLR values are stored as typed bits, so there's no string. They are written with the writer's canonical formatting (§5.2).
  - NaN and ±Infinity are rejected when the node is constructed.
- **Conversions.**
  - Implicit conversions to a node exist for CLR primitives, `string`, `Guid` and the date and time types. Dates are stored as their canonical §5.3 strings, and a local `DateTime` is converted to UTC.
  - Explicit conversions back throw `InvalidCastException` for `null` or a kind mismatch. The nullable versions return `null` for `null`.
  - For any other type, use `GetValue<T>() where T : ISpanParsable<T>`. For models, use `ToModel<T>() where T : IJsonSerializable<T>`.
- **`DeepEquals`.**
  - Object members are compared regardless of order. Arrays are compared in order.
  - Numbers are compared by value across representations, so `1`, `1.0`, `1e0`, `1L` and `1m` are all equal.
- **Duplicate names.** `Parse` rejects duplicate member names with `JsonErrorKind.DuplicateMember`, matching `DuplicateMembers = Error`.
- **Byte-identical round trip.** `JNode.Parse(c).ToJson() == c` byte for byte, for any canonical `c`. This satisfies I4.
- **`ToString()`** returns the node's JSON. For a string value that means a quoted string; use `GetString()` or `(string?)node` for the value itself.

<details>
<summary><code>JNode</code>, <code>JNodeKind</code> (and <code>JNode.TryRead</code>, the node converters)</summary>

`Dynamic/JNode.cs`

```csharp
// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary> The kind of a <see cref="JNode"/>. JSON <c>null</c> has no node: it's a <see langword="null"/> reference. </summary>
public enum JNodeKind : byte { Object, Array, String, Number, Boolean }



/// <summary> A mutable JSON tree in the style of Newtonsoft's <c>JToken</c>: <see cref="JObjectNode"/>, <see cref="JArrayNode"/> or <see cref="JValueNode"/>. </summary>
/// <remarks>
///     <para> The hierarchy is closed (the constructor is <see langword="private protected"/>), so a switch over <see cref="Kind"/> is exhaustive. </para>
///     <para> A node has at most one <see cref="Parent"/>, so a tree can't contain a cycle. Not thread-safe for writes; concurrent reads of an unchanging tree are safe. </para>
/// </remarks>
public abstract partial class JNode
{
    private JNode? __parent;


    private protected JNode() { }


    public abstract JNodeKind Kind   { get; }
    public          JNode?    Parent => __parent;

    public JNode Root
    {
        get
        {
            JNode node = this;
            while ( node.__parent is not null ) { node = node.__parent; }

            return node;
        }
    }

    /// <summary> The node's location from the root, e.g. <c>$.lines[3].price</c> or <c>$['odd key']</c>. Diagnostic: O(depth × siblings). </summary>
    public string Path
    {
        get
        {
            ValueStringBuilder builder = new(stackalloc char[128]);
            AppendPath(ref builder);
            return builder.ToString();
        }
    }


    /// <summary> An object member (<see langword="null"/> when missing or JSON <c>null</c>). </summary>
    /// <exception cref="InvalidOperationException"> The node isn't an object. </exception>
    public virtual JNode? this[ string name ] { get => throw NotA("an object"); set => throw NotA("an object"); }

    /// <summary> An array element. </summary>
    /// <exception cref="InvalidOperationException"> The node isn't an array. </exception>
    public virtual JNode? this[ int index ] { get => throw NotA("an array"); set => throw NotA("an array"); }


    public JObjectNode AsObject() => this as JObjectNode ?? throw NotA("an object");
    public JArrayNode  AsArray()  => this as JArrayNode  ?? throw NotA("an array");
    public JValueNode  AsValue()  => this as JValueNode  ?? throw NotA("a value");


    // ─── Parse ───────────────────────────────────────────────────────────────

    /// <summary> <see langword="null"/> for a JSON <c>null</c> root. </summary>
    /// <exception cref="JsonReadException"> The JSON is malformed, or an object repeats a member name. </exception>
    public static JNode? Parse( string json, JsonReaderOptions? options = null )
    {
        using JsonTape tape = JsonTape.Parse(json, options);
        return FromTape(tape);
    }

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static JNode? Parse( ReadOnlySpan<char> json, JsonReaderOptions? options = null )
    {
        using JsonTape tape = JsonTape.Parse(json, options);
        return FromTape(tape);
    }

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static JNode? Parse( ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null )
    {
        using JsonTape tape = JsonTape.Parse(utf8Json, options);
        return FromTape(tape);
    }

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static JNode? Parse( Stream utf8Json, JsonReaderOptions? options = null )
    {
        using JsonTape tape = JsonTape.Parse(utf8Json, options);
        return FromTape(tape);
    }

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static async ValueTask<JNode?> ParseAsync( Stream utf8Json, JsonReaderOptions? options = null, CancellationToken token = default )
    {
        using JsonTape tape = await JsonTape.ParseAsync(utf8Json, options, token).ConfigureAwait(false);
        return FromTape(tape);
    }

    /// <summary> Exception-free. <paramref name="node"/> is <see langword="null"/> for a JSON <c>null</c> root, as well as on failure: check the return value. </summary>
    public static bool TryParse( [NotNullWhen(true)] string? json, out JNode? node, out JsonError error, JsonReaderOptions? options = null )
    {
        node = null;
        if ( !JsonTape.TryParse(json, out JsonTape? tape, out error, options) ) { return false; }

        using ( tape ) { return TryFromItem(tape.Root, out node, out error); }
    }


    private static JNode? FromTape( JsonTape tape ) => TryFromItem(tape.Root, out JNode? node, out JsonError error)
                                                           ? node
                                                           : throw new JsonReadException(error);

    internal static bool TryFromItem( JsonItem item, out JNode? node, out JsonError error )
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        error = default;

        switch ( item.Kind )
        {
            case JsonTokenKind.Object:
            {
                JObjectNode obj = new(item.GetPropertyCount());

                foreach ( JsonTapeProperty member in item.EnumerateObject() )
                {
                    if ( !TryFromItem(member.Value, out JNode? value, out error) )
                    {
                        node = null;
                        return false;
                    }

                    if ( !obj.TryAddNew(member.Name.GetString(), value) )
                    {
                        error = item.Tape.CreateError(JsonErrorKind.DuplicateMember, member.Name.Position); // DuplicateMembers = Error (§4.2)
                        node  = null;
                        return false;
                    }
                }

                node = obj;
                return true;
            }

            case JsonTokenKind.Array:
            {
                JArrayNode array = new(item.GetArrayLength());

                foreach ( JsonItem element in item.EnumerateArray() )
                {
                    if ( !TryFromItem(element, out JNode? value, out error) )
                    {
                        node = null;
                        return false;
                    }

                    array.AddNew(value);
                }

                node = array;
                return true;
            }

            case JsonTokenKind.String:
                node = new JValueNode(item.GetString());
                return true;

            case JsonTokenKind.Number:
                node = JValueNode.FromValidatedNumberText(item.GetRawText()); // keeps the text: 1.10 stays 1.10
                return true;

            case JsonTokenKind.True:
                node = new JValueNode(true);
                return true;

            case JsonTokenKind.False:
                node = new JValueNode(false);
                return true;

            default: // JsonTokenKind.Null
                node = null;
                return true;
        }
    }


    // ─── Write ───────────────────────────────────────────────────────────────

    /// <summary> Writes this node through any <see cref="IJsonWriter"/> (text, UTF-8, or another DOM). </summary>
    public abstract void WriteTo<TWriter>( ref TWriter writer )
        where TWriter : IJsonWriter, allows ref struct;

    public string ToJson( JsonWriterOptions? options = null )
    {
        JsonWriter writer = new(stackalloc char[512], options ?? JsonWriterOptions.Default); // §3.2: UTF-16 over ValueStringBuilder

        try
        {
            WriteTo(ref writer);
            return writer.ToString();
        }
        finally { writer.Dispose(); }
    }

    public byte[] ToJsonUtf8( JsonWriterOptions? options = null )
    {
        JsonUtf8Writer writer = new(stackalloc byte[512], options ?? JsonWriterOptions.Default); // §3.2: UTF-8 over ValueUtf8Builder

        try
        {
            WriteTo(ref writer);
            return writer.ToArray();
        }
        finally { writer.Dispose(); }
    }

    /// <summary> The node's JSON (compact). </summary>
    public override string ToString() => ToJson();

    /// <summary> Writes <paramref name="node"/>, or <c>null</c>. Generated code uses it for extension data. </summary>
    public static void Write<TWriter>( JNode? node, ref TWriter writer )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( node is null ) { writer.WriteNull(); }
        else { node.WriteTo(ref writer); }
    }


    // ─── Models ──────────────────────────────────────────────────────────────

    /// <summary> Builds a tree from a model through its generated <c>WriteJson</c>, with no intermediate text. </summary>
    public static JNode? FromModel<T>( T value )
        where T : IJsonSerializable<T>
    {
        JNodeWriter writer = new(); // §3.2 (P4)
        T.WriteJson(ref writer, in value);
        return writer.Result;
    }

    /// <summary> Reads a model from this tree through its generated <c>TryReadJson</c>, with no intermediate text. </summary>
    /// <exception cref="JsonReadException"> The tree doesn't match <typeparamref name="T"/>. </exception>
    public T ToModel<T>()
        where T : IJsonSerializable<T>
    {
        JNodeReader reader = new(this); // §3.2 (P4)
        return T.TryReadJson(ref reader, out T? value)
                   ? value
                   : throw new JsonReadException(reader.Error);
    }


    // ─── Clone / compare ─────────────────────────────────────────────────────

    /// <summary> A detached copy (no <see cref="Parent"/>) that can be added anywhere. </summary>
    public abstract JNode DeepClone();

    /// <summary> Structural equality: object members in any order, array elements in order, numbers by value (<c>1</c> == <c>1.0</c> == <c>1m</c>), strings ordinal. </summary>
    public static bool DeepEquals( JNode? left, JNode? right )
    {
        if ( ReferenceEquals(left, right) ) { return true; }

        if ( left is null || right is null || left.Kind != right.Kind ) { return false; }

        RuntimeHelpers.EnsureSufficientExecutionStack();
        return left.DeepEqualsCore(right);
    }

    /// <param name="other"> Same <see cref="Kind"/> as this node. </param>
    private protected abstract bool DeepEqualsCore( JNode other );


    // ─── Parent tracking ─────────────────────────────────────────────────────

    /// <summary>
    ///     Removes this node from its <see cref="Parent"/> (its array element or object member is removed) and returns it, ready to be added anywhere else.
    ///     Use it to keep a piece of a large document without keeping the rest alive: a parent link keeps the whole tree reachable. No-op for a root.
    /// </summary>
    /// <returns> This node, for chaining (<c>target["lines"] = source["lines"]!.Detach();</c>). </returns>
    public JNode Detach()
    {
        __parent?.RemoveChild(this); // clears __parent through Orphan
        return this;
    }

    /// <summary> Removes <paramref name="child"/> (by reference) from this container and orphans it. </summary>
    private protected abstract void RemoveChild( JNode child );


    /// <summary> Makes this node <paramref name="child"/>'s parent; throws if that would give it two parents or create a cycle. </summary>
    private protected void Adopt( JNode? child )
    {
        if ( child is null ) { return; }

        if ( child.__parent is not null ) { throw new InvalidOperationException($"The node already belongs to '{child.__parent.Path}'; add a DeepClone() of it instead."); }

        for ( JNode? ancestor = this; ancestor is not null; ancestor = ancestor.__parent )
        {
            if ( ReferenceEquals(ancestor, child) ) { throw new InvalidOperationException("A node can't be added to itself or to one of its descendants."); }
        }

        child.__parent = this;
    }

    /// <summary> For nodes created by this library a moment ago (parsing, cloning): no checks needed. </summary>
    private protected void AdoptNew( JNode? child )
    {
        if ( child is not null ) { child.__parent = this; }
    }

    private protected static void Orphan( JNode? child )
    {
        if ( child is not null ) { child.__parent = null; }
    }


    private void AppendPath( ref ValueStringBuilder builder )
    {
        if ( __parent is null )
        {
            builder.Append('$');
            return;
        }

        RuntimeHelpers.EnsureSufficientExecutionStack();
        __parent.AppendPath(ref builder);
        __parent.AppendSegment(this, ref builder);
    }

    /// <summary> Appends <paramref name="child"/>'s segment (<c>.name</c>, <c>['name']</c> or <c>[i]</c>). </summary>
    private protected abstract void AppendSegment( JNode child, ref ValueStringBuilder builder );

    private InvalidOperationException NotA( string expected ) => new($"The node is a JSON {Kind}, not {expected}.");


    // ─── Conversions: CLR → JNode ────────────────────────────────────────────

    public static implicit operator JNode( bool           value ) => new JValueNode(value);
    public static implicit operator JNode( sbyte          value ) => new JValueNode(value);
    public static implicit operator JNode( byte           value ) => new JValueNode((int)value); // byte and ushort would be ambiguous between int and UInt128
    public static implicit operator JNode( short          value ) => new JValueNode(value);
    public static implicit operator JNode( ushort         value ) => new JValueNode((int)value);
    public static implicit operator JNode( int            value ) => new JValueNode(value);
    public static implicit operator JNode( uint           value ) => new JValueNode(value);
    public static implicit operator JNode( long           value ) => new JValueNode(value);
    public static implicit operator JNode( ulong          value ) => new JValueNode(value);
    public static implicit operator JNode( Int128         value ) => new JValueNode(value);
    public static implicit operator JNode( UInt128        value ) => new JValueNode(value);
    public static implicit operator JNode( float          value ) => new JValueNode(value);
    public static implicit operator JNode( double         value ) => new JValueNode(value);
    public static implicit operator JNode( decimal        value ) => new JValueNode(value);
    public static implicit operator JNode( Guid           value ) => new JValueNode(value);
    public static implicit operator JNode( DateTime       value ) => new JValueNode(value);
    public static implicit operator JNode( DateTimeOffset value ) => new JValueNode(value);
    public static implicit operator JNode( DateOnly       value ) => new JValueNode(value);
    public static implicit operator JNode( TimeOnly       value ) => new JValueNode(value);
    public static implicit operator JNode( TimeSpan       value ) => new JValueNode(value);

    public static implicit operator JNode?( string?         value ) => value is null ? null : new JValueNode(value);
    public static implicit operator JNode?( bool?           value ) => value.HasValue ? new JValueNode(value.Value) : null;
    public static implicit operator JNode?( int?            value ) => value.HasValue ? new JValueNode(value.Value) : null;
    public static implicit operator JNode?( long?           value ) => value.HasValue ? new JValueNode(value.Value) : null;
    public static implicit operator JNode?( double?         value ) => value.HasValue ? new JValueNode(value.Value) : null;
    public static implicit operator JNode?( decimal?        value ) => value.HasValue ? new JValueNode(value.Value) : null;
    public static implicit operator JNode?( Guid?           value ) => value.HasValue ? new JValueNode(value.Value) : null;
    public static implicit operator JNode?( DateTime?       value ) => value.HasValue ? new JValueNode(value.Value) : null;
    public static implicit operator JNode?( DateTimeOffset? value ) => value.HasValue ? new JValueNode(value.Value) : null;


    // ─── Conversions: JNode → CLR ────────────────────────────────────────────

    public static explicit operator bool( JNode?           node ) => RequireValue(node, "Boolean").GetBoolean();
    public static explicit operator int( JNode?            node ) => RequireValue(node, "Int32").GetInt32();
    public static explicit operator long( JNode?           node ) => RequireValue(node, "Int64").GetInt64();
    public static explicit operator ulong( JNode?          node ) => RequireValue(node, "UInt64").GetUInt64();
    public static explicit operator float( JNode?          node ) => RequireValue(node, "Single").GetSingle();
    public static explicit operator double( JNode?         node ) => RequireValue(node, "Double").GetDouble();
    public static explicit operator decimal( JNode?        node ) => RequireValue(node, "Decimal").GetDecimal();
    public static explicit operator Guid( JNode?           node ) => RequireValue(node, "Guid").GetGuid();
    public static explicit operator DateTime( JNode?       node ) => RequireValue(node, "DateTime").GetDateTime();
    public static explicit operator DateTimeOffset( JNode? node ) => RequireValue(node, "DateTimeOffset").GetDateTimeOffset();

    public static explicit operator string?( JNode?         node ) => node is null ? null : RequireValue(node, "String").GetString();
    public static explicit operator bool?( JNode?           node ) => node is null ? null : (bool)node;
    public static explicit operator int?( JNode?            node ) => node is null ? null : (int)node;
    public static explicit operator long?( JNode?           node ) => node is null ? null : (long)node;
    public static explicit operator double?( JNode?         node ) => node is null ? null : (double)node;
    public static explicit operator decimal?( JNode?        node ) => node is null ? null : (decimal)node;
    public static explicit operator Guid?( JNode?           node ) => node is null ? null : (Guid)node;
    public static explicit operator DateTime?( JNode?       node ) => node is null ? null : (DateTime)node;
    public static explicit operator DateTimeOffset?( JNode? node ) => node is null ? null : (DateTimeOffset)node;

    private static JValueNode RequireValue( JNode? node, string target ) => node as JValueNode
                                                                           ?? throw new InvalidCastException(node is null
                                                                                                                 ? $"Can't convert JSON null to {target}."
                                                                                                                 : $"Can't convert a JSON {node.Kind} to {target}.");
}
```

`Dynamic/JNodeConverters.cs`

```csharp
// Jakar.Json
// 10/07/2026

using Jakar.Json.Converters;



namespace Jakar.Json
{
    public abstract partial class JNode
    {
        /// <summary> Reads any JSON value from <paramref name="reader"/> as a tree (JSON <c>null</c> → <see langword="null"/>). Numbers keep their text. A repeated member name fails with <see cref="JsonErrorKind.DuplicateMember"/>. </summary>
        public static bool TryRead<TReader>( ref TReader reader, out JNode? node )
            where TReader : IJsonReader, allows ref struct
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();
            node = null;

            switch ( reader.PeekKind() )
            {
                case JsonTokenKind.Object:
                {
                    if ( !reader.TryReadStartObject() ) { return false; }

                    JObjectNode obj = new();

                    while ( true )
                    {
                        JsonReaderCheckpoint mark = reader.Checkpoint();
                        if ( !reader.TryReadProperty(out JsonSpan name, out bool end) ) { return false; }

                        if ( end ) { break; }

                        string key = name.ToString();
                        if ( !TryRead(ref reader, out JNode? value) ) { return false; }

                        if ( obj.TryAddNew(key, value) ) { continue; }

                        reader.Rewind(mark);
                        return reader.Fail(JsonErrorKind.DuplicateMember);
                    }

                    node = obj;
                    return true;
                }

                case JsonTokenKind.Array:
                {
                    if ( !reader.TryReadStartArray() ) { return false; }

                    JArrayNode array = new();

                    while ( true )
                    {
                        if ( !reader.TryReadNextElement(out bool end) ) { return false; }

                        if ( end ) { break; }

                        if ( !TryRead(ref reader, out JNode? value) ) { return false; }

                        array.AddNew(value);
                    }

                    node = array;
                    return true;
                }

                case JsonTokenKind.String:
                    if ( !reader.TryReadString(out string? text) ) { return false; }

                    node = new JValueNode(text);
                    return true;

                case JsonTokenKind.Number:
                    if ( !reader.TryReadRawNumber(out JsonSpan number) ) { return false; }

                    node = JValueNode.FromValidatedNumberText(number.ToString());
                    return true;

                case JsonTokenKind.True or JsonTokenKind.False:
                    if ( !reader.TryReadBoolean(out bool flag) ) { return false; }

                    node = new JValueNode(flag);
                    return true;

                case JsonTokenKind.Null:
                    return reader.TryReadNull();

                default:
                    return reader.Fail(JsonErrorKind.UnexpectedToken);
            }
        }
    }
}



namespace Jakar.Json.Converters
{
    /// <summary> Any JSON value as a <see cref="JNode"/> (JSON <c>null</c> ↔ <see langword="null"/>). </summary>
    public readonly struct JsonNodeConverter : IJsonConverter<JNode?>
    {
        public static void Write<TWriter>( ref TWriter writer, scoped in JNode? value )
            where TWriter : IJsonWriter, allows ref struct => JNode.Write(value, ref writer);

        public static bool TryRead<TReader>( ref TReader reader, out JNode? value )
            where TReader : IJsonReader, allows ref struct => JNode.TryRead(ref reader, out value);
    }



    /// <summary> A <see cref="JObjectNode"/>, <see cref="JArrayNode"/> or <see cref="JValueNode"/> member: reading a different kind of value (or <c>null</c>) fails with <see cref="JsonErrorKind.InvalidValue"/>. </summary>
    public readonly struct JsonNodeConverter<TNode> : IJsonConverter<TNode>
        where TNode : JNode
    {
        public static void Write<TWriter>( ref TWriter writer, scoped in TNode value )
            where TWriter : IJsonWriter, allows ref struct => JNode.Write(value, ref writer);

        public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out TNode value )
            where TReader : IJsonReader, allows ref struct
        {
            value = null;
            JsonReaderCheckpoint mark = reader.Checkpoint();
            if ( !JNode.TryRead(ref reader, out JNode? node) ) { return false; }

            if ( node is TNode typed )
            {
                value = typed;
                return true;
            }

            reader.Rewind(mark);
            return reader.Fail(JsonErrorKind.InvalidValue);
        }
    }
}
```

</details>

<details>
<summary><code>JValueNode</code></summary>

`Dynamic/JValueNode.cs`

```csharp
// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary>
///     An immutable JSON string, number or boolean. Numbers are either the exact text they were parsed from (so <c>1.10</c> stays <c>1.10</c>),
///     or a typed CLR value stored as bits and written with the writer's canonical formatting (§5.2). Nothing is boxed.
/// </summary>
public sealed class JValueNode : JNode
{
    private enum NumberKind : byte { None, Text, Int64, UInt64, Int128, UInt128, Single, Double, Decimal }


    private readonly JNodeKind  __kind;
    private readonly NumberKind __number;
    private readonly string?    __text; // String: the value; NumberKind.Text: the validated JSON number text
    private readonly Int128     __bits; // Boolean: 0/1; typed numbers: the value's bits


    public JValueNode( string value )
    {
        ArgumentNullException.ThrowIfNull(value);
        __kind = JNodeKind.String;
        __text = value;
    }

    public JValueNode( bool value )
    {
        __kind = JNodeKind.Boolean;
        __bits = value ? 1 : 0;
    }

    // int and uint are spelled out: without them, int/byte/ushort/uint arguments are ambiguous between the long and UInt128 overloads.
    public JValueNode( int     value ) : this(NumberKind.Int64,   value) { }
    public JValueNode( uint    value ) : this(NumberKind.Int64,   value) { }
    public JValueNode( long    value ) : this(NumberKind.Int64,   value) { }
    public JValueNode( ulong   value ) : this(NumberKind.UInt64,  value) { }
    public JValueNode( Int128  value ) : this(NumberKind.Int128,  value) { }
    public JValueNode( UInt128 value ) : this(NumberKind.UInt128, unchecked((Int128)value)) { }
    public JValueNode( float   value ) : this(NumberKind.Single,  BitConverter.SingleToInt32Bits(Finite(value))) { }
    public JValueNode( double  value ) : this(NumberKind.Double,  BitConverter.DoubleToInt64Bits(Finite(value))) { }
    public JValueNode( decimal value ) : this(NumberKind.Decimal, Unsafe.BitCast<decimal, Int128>(value)) { }

    // §5.3 canonical strings.
    public JValueNode( Guid           value ) : this(value.ToString("D")) { } // lowercase
    public JValueNode( DateTimeOffset value ) : this(value.ToString("O", CultureInfo.InvariantCulture)) { }
    public JValueNode( DateOnly       value ) : this(value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)) { }
    public JValueNode( TimeOnly       value ) : this(value.ToString("HH:mm:ss.fffffff", CultureInfo.InvariantCulture)) { }
    public JValueNode( TimeSpan       value ) : this(value.ToString("c", CultureInfo.InvariantCulture)) { }

    /// <summary> <see cref="DateTimeKind.Local"/> values are converted to UTC first (<c>LocalDateTimes = ConvertToUtc</c>, §5.4). </summary>
    public JValueNode( DateTime value ) : this(( value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : value ).ToString("O", CultureInfo.InvariantCulture)) { }


    private JValueNode( NumberKind number, Int128 bits )
    {
        __kind   = JNodeKind.Number;
        __number = number;
        __bits   = bits;
    }

    private JValueNode( string text, NumberKind number )
    {
        __kind   = JNodeKind.Number;
        __number = number;
        __text   = text;
    }

    private JValueNode( JValueNode other )
    {
        __kind   = other.__kind;
        __number = other.__number;
        __text   = other.__text;
        __bits   = other.__bits;
    }


    /// <summary> A number from its JSON text, kept verbatim. </summary>
    /// <exception cref="FormatException"> <paramref name="text"/> isn't exactly one JSON number. </exception>
    public static JValueNode FromNumberText( string text )
    {
        ArgumentNullException.ThrowIfNull(text);

        return JsonTape.IsValidNumber(text)
                   ? new JValueNode(text, NumberKind.Text)
                   : throw new FormatException($"'{text}' isn't a JSON number.");
    }

    internal static JValueNode FromValidatedNumberText( string text ) => new(text, NumberKind.Text);


    public override JNodeKind Kind => __kind;

    /// <summary> A number with no fraction or exponent. </summary>
    public bool IsInteger => __number is NumberKind.Int64 or NumberKind.UInt64 or NumberKind.Int128 or NumberKind.UInt128
                             || ( __number == NumberKind.Text && __text.AsSpan().IndexOfAny('.', 'e', 'E') < 0 );


    // ─── Strings and booleans ────────────────────────────────────────────────

    public string GetString() => __kind == JNodeKind.String
                                     ? __text!
                                     : throw Mismatch(JNodeKind.String);

    public bool GetBoolean() => __kind == JNodeKind.Boolean
                                    ? __bits != 0
                                    : throw Mismatch(JNodeKind.Boolean);

    public Guid           GetGuid()           => Guid.ParseExact(GetString(), "D");
    public DateTime       GetDateTime()       => DateTime.ParseExact(GetString(), "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind); // never shifts to local time
    public DateTimeOffset GetDateTimeOffset() => DateTimeOffset.ParseExact(GetString(), "O", CultureInfo.InvariantCulture);


    // ─── Numbers ─────────────────────────────────────────────────────────────

    public int     GetInt32()   => GetInteger<int>();
    public long    GetInt64()   => GetInteger<long>();
    public ulong   GetUInt64()  => GetInteger<ulong>();
    public float   GetSingle()  => GetFloat<float>();
    public double  GetDouble()  => GetFloat<double>();
    public decimal GetDecimal() => GetFloat<decimal>();

    /// <exception cref="OverflowException"> The value doesn't fit <typeparamref name="T"/>. </exception>
    /// <exception cref="FormatException"> The number text has a fraction or exponent. </exception>
    /// <exception cref="InvalidOperationException"> The value isn't a number, or is a typed floating-point number. </exception>
    public T GetInteger<T>()
        where T : IBinaryInteger<T> => __number switch
                                       {
                                           NumberKind.Int64   => T.CreateChecked((long)__bits),
                                           NumberKind.UInt64  => T.CreateChecked((ulong)__bits),
                                           NumberKind.Int128  => T.CreateChecked(__bits),
                                           NumberKind.UInt128 => T.CreateChecked(unchecked((UInt128)__bits)),
                                           NumberKind.Text    => T.Parse(__text!, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture),
                                           NumberKind.None    => throw Mismatch(JNodeKind.Number),
                                           _                  => throw new InvalidOperationException("The number isn't an integer.")
                                       };

    /// <exception cref="OverflowException"> The value doesn't fit <typeparamref name="T"/>. </exception>
    public T GetFloat<T>()
        where T : IFloatingPoint<T> => __number switch
                                       {
                                           NumberKind.Single  => T.CreateChecked(BitConverter.Int32BitsToSingle((int)__bits)),
                                           NumberKind.Double  => T.CreateChecked(BitConverter.Int64BitsToDouble((long)__bits)),
                                           NumberKind.Decimal => T.CreateChecked(Unsafe.BitCast<Int128, decimal>(__bits)),
                                           NumberKind.Int64   => T.CreateChecked((long)__bits),
                                           NumberKind.UInt64  => T.CreateChecked((ulong)__bits),
                                           NumberKind.Int128  => T.CreateChecked(__bits),
                                           NumberKind.UInt128 => T.CreateChecked(unchecked((UInt128)__bits)),
                                           NumberKind.Text    => ParseFloat<T>(__text!),
                                           _                  => throw Mismatch(JNodeKind.Number)
                                       };

    /// <summary> Any <see cref="ISpanParsable{TSelf}"/> type, parsed from the value's text with invariant culture (e.g. <see cref="DateOnly"/>, <c>Email</c>, <see cref="BigInteger"/>). </summary>
    public T GetValue<T>()
        where T : ISpanParsable<T>
    {
        switch ( __kind )
        {
            case JNodeKind.String:  return T.Parse(__text!, CultureInfo.InvariantCulture);
            case JNodeKind.Boolean: return T.Parse(__bits != 0 ? "true" : "false", CultureInfo.InvariantCulture);
        }

        if ( __number == NumberKind.Text ) { return T.Parse(__text!, CultureInfo.InvariantCulture); }

        Span<char> buffer = stackalloc char[64];
        return T.Parse(buffer[..FormatNumber(buffer)], CultureInfo.InvariantCulture);
    }


    /// <summary> Exception-free <see cref="GetInteger{T}"/> for readers. </summary>
    internal bool TryGetInteger<T>( out T value, out JsonErrorKind error )
        where T : struct, IBinaryInteger<T>
    {
        value = default;
        error = JsonErrorKind.None;

        switch ( __number )
        {
            case NumberKind.Text:
                if ( T.TryParse(__text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value) ) { return true; }

                error = JsonLexer<char>.HasFractionOrExponent(__text) ? JsonErrorKind.InvalidNumber : JsonErrorKind.NumberOverflow;
                return false;

            case NumberKind.Int64 or NumberKind.UInt64 or NumberKind.Int128 or NumberKind.UInt128:
                bool fits = __number switch
                            {
                                NumberKind.Int64  => TryNarrow((long)__bits, out value),
                                NumberKind.UInt64 => TryNarrow((ulong)__bits, out value),
                                NumberKind.Int128 => TryNarrow(__bits, out value),
                                _                 => TryNarrow(unchecked((UInt128)__bits), out value)
                            };

                if ( fits ) { return true; }

                error = JsonErrorKind.NumberOverflow;
                return false;

            default:
                error = __kind == JNodeKind.Number ? JsonErrorKind.InvalidNumber : JsonErrorKind.UnexpectedToken;
                return false;
        }
    }

    /// <summary> Saturate, then check the value survives the round trip: it fits exactly when it does. </summary>
    private static bool TryNarrow<TFrom, T>( TFrom source, out T value )
        where TFrom : IBinaryInteger<TFrom>
        where T : IBinaryInteger<T>
    {
        value = T.CreateSaturating(source);
        return TFrom.CreateSaturating(value) == source;
    }

    /// <summary> Exception-free <see cref="GetFloat{T}"/> for readers. </summary>
    internal bool TryGetFloat<T>( out T value, out JsonErrorKind error )
        where T : struct, IFloatingPoint<T>
    {
        value = default;
        error = JsonErrorKind.None;

        if ( __number == NumberKind.Text )
        {
            if ( T.TryParse(__text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && !T.IsInfinity(value) ) { return true; }

            error = JsonErrorKind.NumberOverflow;
            return false;
        }

        if ( __number == NumberKind.None )
        {
            error = JsonErrorKind.UnexpectedToken;
            return false;
        }

        Span<char> buffer = stackalloc char[64];
        int        length = FormatNumber(buffer);
        if ( T.TryParse(buffer[..length], NumberStyles.Float, CultureInfo.InvariantCulture, out value) && !T.IsInfinity(value) ) { return true; }

        error = JsonErrorKind.NumberOverflow;
        return false;
    }

    /// <summary> The number's text: verbatim when parsed, invariant round-trippable otherwise. </summary>
    internal void AppendNumberText( ref ValueStringBuilder builder )
    {
        if ( __number == NumberKind.Text )
        {
            builder.Append(__text);
            return;
        }

        Span<char> buffer = stackalloc char[64];
        builder.Append(buffer[..FormatNumber(buffer)]);
    }


    private static T ParseFloat<T>( string text )
        where T : IFloatingPoint<T>
    {
        T value = T.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);

        return T.IsInfinity(value)
                   ? throw new OverflowException($"{text} overflows the floating-point type.")
                   : value;
    }

    private static T Finite<T>( T value )
        where T : IFloatingPointIeee754<T> => T.IsFinite(value)
                                                  ? value
                                                  : throw new ArgumentOutOfRangeException(nameof(value), value, "JSON has no NaN or Infinity; store a string instead.");

    /// <summary> Invariant text of a typed number (round-trippable for floats). 64 chars fit every typed number. </summary>
    private int FormatNumber( Span<char> destination )
    {
        int written = 0;

        bool formatted = __number switch
                         {
                             NumberKind.Int64   => ( (long)__bits ).TryFormat(destination, out written, default, CultureInfo.InvariantCulture),
                             NumberKind.UInt64  => ( (ulong)__bits ).TryFormat(destination, out written, default, CultureInfo.InvariantCulture),
                             NumberKind.Int128  => __bits.TryFormat(destination, out written, default, CultureInfo.InvariantCulture),
                             NumberKind.UInt128 => unchecked((UInt128)__bits).TryFormat(destination, out written, default, CultureInfo.InvariantCulture),
                             NumberKind.Single  => BitConverter.Int32BitsToSingle((int)__bits).TryFormat(destination, out written, "R", CultureInfo.InvariantCulture),
                             NumberKind.Double  => BitConverter.Int64BitsToDouble((long)__bits).TryFormat(destination, out written, "R", CultureInfo.InvariantCulture),
                             NumberKind.Decimal => Unsafe.BitCast<Int128, decimal>(__bits).TryFormat(destination, out written, default, CultureInfo.InvariantCulture),
                             _                  => throw Mismatch(JNodeKind.Number)
                         };

        return formatted ? written : throw new UnreachableException();
    }

    private ReadOnlySpan<char> NumberText( Span<char> buffer ) => __number == NumberKind.Text
                                                                      ? __text.AsSpan()
                                                                      : buffer[..FormatNumber(buffer)];

    private InvalidOperationException Mismatch( JNodeKind expected ) => new($"The value is a JSON {__kind}, not a {expected}.");


    // ─── JNode ───────────────────────────────────────────────────────────────

    public override void WriteTo<TWriter>( ref TWriter writer )
    {
        switch ( __kind )
        {
            case JNodeKind.String:
                writer.WriteString(__text!);
                return;

            case JNodeKind.Boolean:
                writer.WriteBoolean(__bits != 0);
                return;
        }

        switch ( __number )
        {
            case NumberKind.Text:    writer.WriteRawNumber(__text!); break; // validated when the node was created
            case NumberKind.Int64:   writer.WriteInteger((long)__bits); break;
            case NumberKind.UInt64:  writer.WriteInteger((ulong)__bits); break;
            case NumberKind.Int128:  writer.WriteInteger(__bits); break;
            case NumberKind.UInt128: writer.WriteInteger(unchecked((UInt128)__bits)); break;
            case NumberKind.Single:  writer.WriteFloat(BitConverter.Int32BitsToSingle((int)__bits)); break;
            case NumberKind.Double:  writer.WriteFloat(BitConverter.Int64BitsToDouble((long)__bits)); break;
            case NumberKind.Decimal: writer.WriteFloat(Unsafe.BitCast<Int128, decimal>(__bits)); break;
        }
    }

    public override JValueNode DeepClone() => new(this);

    private protected override bool DeepEqualsCore( JNode other )
    {
        JValueNode value = (JValueNode)other;

        return __kind switch
               {
                   JNodeKind.String  => string.Equals(__text, value.__text, StringComparison.Ordinal),
                   JNodeKind.Boolean => __bits == value.__bits,
                   _                 => NumberEquals(value)
               };
    }

    /// <summary>
    ///     Equal by value across representations. Doubles reject most mismatches cheaply; decimals then separate values that doubles can't
    ///     (<c>0.1</c> vs <c>0.10000000000000001</c>, 2^63 vs 2^63 + 1). Integers beyond decimal's range compare by their invariant text.
    /// </summary>
    private bool NumberEquals( JValueNode other )
    {
        if ( __number == other.__number && __number is NumberKind.Int64 or NumberKind.UInt64 or NumberKind.Int128 or NumberKind.UInt128 ) { return __bits == other.__bits; }

        if ( TryGetDouble(out double left) && other.TryGetDouble(out double right) && left != right ) { return false; }

        if ( TryGetDecimal(out decimal a) && other.TryGetDecimal(out decimal b) ) { return a == b; }

        Span<char> leftText = stackalloc char[64], rightText = stackalloc char[64];
        return NumberText(leftText).SequenceEqual(other.NumberText(rightText));
    }

    private bool TryGetDouble( out double value )
    {
        if ( __number == NumberKind.Text ) { return double.TryParse(__text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && double.IsFinite(value); }

        value = GetFloat<double>();
        return true;
    }

    private bool TryGetDecimal( out decimal value )
    {
        switch ( __number )
        {
            case NumberKind.Text:    return decimal.TryParse(__text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            case NumberKind.Decimal: value = Unsafe.BitCast<Int128, decimal>(__bits); return true;
            case NumberKind.Int64:   value = (long)__bits; return true;
            case NumberKind.UInt64:  value = (ulong)__bits; return true;

            case NumberKind.Int128 when __bits >= (Int128)decimal.MinValue && __bits <= (Int128)decimal.MaxValue:
                value = (decimal)__bits;
                return true;

            case NumberKind.UInt128 when unchecked((UInt128)__bits) <= (UInt128)decimal.MaxValue:
                value = (decimal)unchecked((UInt128)__bits);
                return true;

            case NumberKind.Single or NumberKind.Double:
            {
                double number = GetFloat<double>();

                if ( Math.Abs(number) <= (double)decimal.MaxValue )
                {
                    value = (decimal)number;
                    return true;
                }

                break;
            }
        }

        value = 0;
        return false;
    }

    private protected override void AppendSegment( JNode child, ref ValueStringBuilder builder ) { } // values have no children

    private protected override void RemoveChild( JNode child ) => throw new UnreachableException("A value has no children.");
}
```

</details>

<details>
<summary><code>JArrayNode</code></summary>

`Dynamic/JArrayNode.cs`

```csharp
// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary> A mutable JSON array. Elements may be <see langword="null"/> (JSON <c>null</c>). </summary>
public sealed class JArrayNode : JNode, IList<JNode?>, IReadOnlyList<JNode?>
{
    private readonly List<JNode?> __items;


    public JArrayNode() => __items = [];
    public JArrayNode( int capacity ) => __items = new List<JNode?>(capacity);
    /// <exception cref="InvalidOperationException"> An item already has a parent. </exception>
    public JArrayNode( params ReadOnlySpan<JNode?> items ) : this(items.Length)
    {
        foreach ( JNode? item in items ) { Add(item); }
    }


    /// <exception cref="JsonReadException"> The JSON is malformed. </exception>
    /// <exception cref="FormatException"> The root isn't an array. </exception>
    public static new JArrayNode Parse( string json, JsonReaderOptions? options = null ) => JNode.Parse(json, options) as JArrayNode ?? throw new FormatException("The JSON root isn't an array.");

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static new JArrayNode Parse( ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null ) => JNode.Parse(utf8Json, options) as JArrayNode ?? throw new FormatException("The JSON root isn't an array.");


    public override JNodeKind Kind  => JNodeKind.Array;
    public          int       Count => __items.Count;
    bool ICollection<JNode?>.IsReadOnly => false;


    /// <exception cref="InvalidOperationException"> <paramref name="value"/> already has a parent, or is this array or one of its ancestors. </exception>
    public override JNode? this[ int index ]
    {
        get => __items[index];
        set
        {
            JNode? old = __items[index];
            if ( ReferenceEquals(old, value) ) { return; }

            Adopt(value); // validates before anything changes
            __items[index] = value;
            Orphan(old);
        }
    }


    /// <inheritdoc cref="this[int]"/>
    public void Add( JNode? item )
    {
        Adopt(item);
        __items.Add(item);
    }

    /// <inheritdoc cref="this[int]"/>
    public void Insert( int index, JNode? item )
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)__items.Count, nameof(index));
        Adopt(item);
        __items.Insert(index, item);
    }

    public void RemoveAt( int index )
    {
        JNode? old = __items[index];
        __items.RemoveAt(index);
        Orphan(old);
    }

    /// <summary> Removes <paramref name="item"/> (by reference; <see langword="null"/> removes the first JSON <c>null</c>). </summary>
    public bool Remove( JNode? item )
    {
        int index = IndexOf(item);
        if ( index < 0 ) { return false; }

        RemoveAt(index);
        return true;
    }

    public void Clear()
    {
        foreach ( JNode? item in __items ) { Orphan(item); }

        __items.Clear();
    }

    /// <summary> By reference: nodes don't override <see cref="object.Equals(object)"/> (use <see cref="JNode.DeepEquals"/> for structure). </summary>
    public int  IndexOf( JNode?  item ) => __items.IndexOf(item);
    public bool Contains( JNode? item ) => __items.IndexOf(item) >= 0;

    public void CopyTo( JNode?[] array, int arrayIndex ) => __items.CopyTo(array, arrayIndex);

    public List<JNode?>.Enumerator GetEnumerator() => __items.GetEnumerator();
    IEnumerator<JNode?> IEnumerable<JNode?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    internal void AddNew( JNode? item )
    {
        AdoptNew(item);
        __items.Add(item);
    }


    // ─── JNode ───────────────────────────────────────────────────────────────

    public override void WriteTo<TWriter>( ref TWriter writer )
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        writer.WriteStartArray();

        foreach ( JNode? item in __items ) { Write(item, ref writer); }

        writer.WriteEndArray();
    }

    public override JArrayNode DeepClone()
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        JArrayNode clone = new(__items.Count);

        foreach ( JNode? item in __items ) { clone.AddNew(item?.DeepClone()); }

        return clone;
    }

    private protected override bool DeepEqualsCore( JNode other )
    {
        List<JNode?> items = ( (JArrayNode)other ).__items;
        if ( items.Count != __items.Count ) { return false; }

        for ( int i = 0; i < __items.Count; i++ )
        {
            if ( !DeepEquals(__items[i], items[i]) ) { return false; }
        }

        return true;
    }

    private protected override void AppendSegment( JNode child, ref ValueStringBuilder builder ) => builder.Append('[').AppendSpanFormattable(IndexOf(child), default, CultureInfo.InvariantCulture).Append(']');

    private protected override void RemoveChild( JNode child ) => RemoveAt(IndexOf(child)); // a child is always present: its Parent is this array
}
```

</details>

<details>
<summary><code>JObjectNode</code></summary>

`Dynamic/JObjectNode.cs`

```csharp
// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary>
///     A mutable JSON object: ordinal member names in insertion order (the order they are written in). Values may be <see langword="null"/> (JSON <c>null</c>).
/// </summary>
/// <remarks> Like Newtonsoft's <c>JObject</c>, the indexer returns <see langword="null"/> for a missing member instead of throwing, even through <see cref="IDictionary{TKey,TValue}"/>; use <see cref="ContainsKey"/> or <see cref="TryGetValue"/> to tell missing from JSON <c>null</c>. </remarks>
public sealed class JObjectNode : JNode, IDictionary<string, JNode?>, IReadOnlyDictionary<string, JNode?>
{
    private static readonly SearchValues<char> __identifierChars = SearchValues.Create("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_");

    private readonly OrderedDictionary<string, JNode?> __members;


    public JObjectNode() => __members = new OrderedDictionary<string, JNode?>(StringComparer.Ordinal);
    public JObjectNode( int capacity ) => __members = new OrderedDictionary<string, JNode?>(capacity, StringComparer.Ordinal);
    /// <exception cref="ArgumentException"> A name repeats. </exception>
    /// <exception cref="InvalidOperationException"> A value already has a parent. </exception>
    public JObjectNode( params ReadOnlySpan<KeyValuePair<string, JNode?>> members ) : this(members.Length)
    {
        foreach ( KeyValuePair<string, JNode?> member in members ) { Add(member.Key, member.Value); }
    }


    /// <exception cref="JsonReadException"> The JSON is malformed, or a member name repeats. </exception>
    /// <exception cref="FormatException"> The root isn't an object. </exception>
    public static new JObjectNode Parse( string json, JsonReaderOptions? options = null ) => JNode.Parse(json, options) as JObjectNode ?? throw new FormatException("The JSON root isn't an object.");

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static new JObjectNode Parse( ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null ) => JNode.Parse(utf8Json, options) as JObjectNode ?? throw new FormatException("The JSON root isn't an object.");


    public override JNodeKind Kind  => JNodeKind.Object;
    public          int       Count => __members.Count;

    public OrderedDictionary<string, JNode?>.KeyCollection   Keys   => __members.Keys;
    public OrderedDictionary<string, JNode?>.ValueCollection Values => __members.Values;

    ICollection<string> IDictionary<string, JNode?>.Keys   => __members.Keys;
    ICollection<JNode?> IDictionary<string, JNode?>.Values => __members.Values;
    IEnumerable<string> IReadOnlyDictionary<string, JNode?>.Keys   => __members.Keys;
    IEnumerable<JNode?> IReadOnlyDictionary<string, JNode?>.Values => __members.Values;
    bool ICollection<KeyValuePair<string, JNode?>>.IsReadOnly => false;


    /// <summary> Get: the member's value, or <see langword="null"/> when it's missing. Set: adds or replaces the member (a new member goes last). </summary>
    /// <exception cref="InvalidOperationException"> <paramref name="value"/> already has a parent, or is this object or one of its ancestors. </exception>
    public override JNode? this[ string name ]
    {
        get => __members.TryGetValue(name, out JNode? value) ? value : null;
        set
        {
            ArgumentNullException.ThrowIfNull(name);

            if ( __members.TryGetValue(name, out JNode? old) )
            {
                if ( ReferenceEquals(old, value) ) { return; }

                Adopt(value); // validates before anything changes
                __members[name] = value;
                Orphan(old);
                return;
            }

            Adopt(value);
            __members.Add(name, value);
        }
    }


    /// <summary> The member at <paramref name="index"/>, in document order. </summary>
    public KeyValuePair<string, JNode?> GetAt( int index ) => __members.GetAt(index);

    public bool ContainsKey( string name ) => __members.ContainsKey(name);

    public bool TryGetValue( string name, [MaybeNullWhen(false)] out JNode? value ) => __members.TryGetValue(name, out value);

    /// <exception cref="ArgumentException"> The object already has a member named <paramref name="name"/>. </exception>
    /// <exception cref="InvalidOperationException"> <paramref name="value"/> already has a parent, or is this object or one of its ancestors. </exception>
    public void Add( string name, JNode? value )
    {
        if ( !TryAdd(name, value) ) { throw new ArgumentException($"The object already has a member '{name}'.", nameof(name)); }
    }

    /// <summary> <see langword="false"/> (and nothing changes) if the name is taken. </summary>
    /// <exception cref="InvalidOperationException"> <paramref name="value"/> already has a parent, or is this object or one of its ancestors. </exception>
    public bool TryAdd( string name, JNode? value )
    {
        ArgumentNullException.ThrowIfNull(name);
        if ( __members.ContainsKey(name) ) { return false; }

        Adopt(value);
        __members.Add(name, value);
        return true;
    }

    public bool Remove( string name ) => Remove(name, out _);

    public bool Remove( string name, out JNode? value )
    {
        if ( !__members.Remove(name, out value) ) { return false; }

        Orphan(value);
        return true;
    }

    public void Clear()
    {
        foreach ( KeyValuePair<string, JNode?> member in __members ) { Orphan(member.Value); }

        __members.Clear();
    }

    public OrderedDictionary<string, JNode?>.Enumerator GetEnumerator() => __members.GetEnumerator();
    IEnumerator<KeyValuePair<string, JNode?>> IEnumerable<KeyValuePair<string, JNode?>>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    void ICollection<KeyValuePair<string, JNode?>>.Add( KeyValuePair<string, JNode?> member ) => Add(member.Key, member.Value);
    bool ICollection<KeyValuePair<string, JNode?>>.Contains( KeyValuePair<string, JNode?> member ) => __members.TryGetValue(member.Key, out JNode? value) && ReferenceEquals(value, member.Value);
    bool ICollection<KeyValuePair<string, JNode?>>.Remove( KeyValuePair<string, JNode?> member ) => ( (ICollection<KeyValuePair<string, JNode?>>)this ).Contains(member) && Remove(member.Key);
    void ICollection<KeyValuePair<string, JNode?>>.CopyTo( KeyValuePair<string, JNode?>[] array, int arrayIndex ) => ( (ICollection<KeyValuePair<string, JNode?>>)__members ).CopyTo(array, arrayIndex);


    internal bool TryAddNew( string name, JNode? value )
    {
        if ( !__members.TryAdd(name, value) ) { return false; }

        AdoptNew(value);
        return true;
    }


    // ─── JNode ───────────────────────────────────────────────────────────────

    public override void WriteTo<TWriter>( ref TWriter writer )
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        writer.WriteStartObject();

        foreach ( KeyValuePair<string, JNode?> member in __members )
        {
            writer.WritePropertyName(member.Key); // escaped by the writer
            Write(member.Value, ref writer);
        }

        writer.WriteEndObject();
    }

    public override JObjectNode DeepClone()
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        JObjectNode clone = new(__members.Count);

        foreach ( KeyValuePair<string, JNode?> member in __members ) { clone.TryAddNew(member.Key, member.Value?.DeepClone()); }

        return clone;
    }

    /// <summary> Same names with deep-equal values, in any order. </summary>
    private protected override bool DeepEqualsCore( JNode other )
    {
        OrderedDictionary<string, JNode?> members = ( (JObjectNode)other ).__members;
        if ( members.Count != __members.Count ) { return false; }

        foreach ( KeyValuePair<string, JNode?> member in __members )
        {
            if ( !members.TryGetValue(member.Key, out JNode? value) || !DeepEquals(member.Value, value) ) { return false; }
        }

        return true;
    }

    private protected override void AppendSegment( JNode child, ref ValueStringBuilder builder )
    {
        foreach ( KeyValuePair<string, JNode?> member in __members )
        {
            if ( !ReferenceEquals(member.Value, child) ) { continue; }

            string name = member.Key;

            if ( name.Length > 0 && !char.IsAsciiDigit(name[0]) && !name.AsSpan().ContainsAnyExcept(__identifierChars) ) { builder.Append('.').Append(name); }
            else { builder.Append("['").Append(name.Replace("'", "\\'")).Append("']"); }

            return;
        }
    }

    private protected override void RemoveChild( JNode child )
    {
        for ( int i = 0; i < __members.Count; i++ )
        {
            if ( !ReferenceEquals(__members.GetAt(i).Value, child) ) { continue; }

            __members.RemoveAt(i); // keeps the remaining members in order
            Orphan(child);
            return;
        }
    }
}
```

</details>

---

## 11. Diagnostics

The prefix is `JJSON`, distinct from `Jakar.SystemTextJson`'s `JAKAR_JSON`. IDs are never renumbered or reused.

| ID | Severity | Meaning | Code fix |
|---|---|---|---|
| JJSON001 | Error | The type, or a containing type, isn't `partial`. | add `partial` |
| JJSON002 | Error | A generic type parameter is used in a member without a static-dispatch constraint (§7.4). | add constraint |
| JJSON003 | Error | A member type isn't supported (names the member and the type). | add `[JsonMember(Converter = …)]` stub |
| JJSON004 | Error | No usable constructor, or a constructor parameter matches no member. | — |
| JJSON005 | Error | Two members produce the same JSON name after naming and matching (incl. `OrdinalIgnoreCase` collisions). | — |
| JJSON006 | Warning | Members span multiple partial declarations without explicit `Order`; ordering depends on file paths. | add `Order` values |
| JJSON007 | Info | A generated member was skipped because it's already declared by hand. | — |
| JJSON008 | Error | Contradictory settings (e.g. `Required` with `NullValues = Omit` on a nullable member, `Capture` without an extension-data member). | — |
| JJSON009 | Error | `Converter` doesn't implement `IJsonConverter<T>` for the member's type. | — |
| JJSON010 | Error | The type has both `[GenerateJson]` and `[JsonModel]` (Jakar.SystemTextJson). Both emit `FromJson`/`ToJson`, and generators can't see each other's output. | remove one |
| JJSON011 | Info | A get-only member isn't bound to a constructor parameter, so it's written but never read. | — |
| JJSON012 | Error | `ref struct`, pointer, delegate or `object`-typed member. | — |
| JJSON013 | Warning | `UnorderedCollections = Enumeration` or `LocalDateTimes = WriteOffset`: I1 is weakened (§5.4). | — |
| JJSON014 | Error | The `[JsonDerived]` tag is duplicated, or the derived type doesn't derive from the base. | — |
| JJSON015 | Error | An `IJsonSerializable<T>` is listed without `[GenerateJson]` and isn't implemented. | add `[GenerateJson]` |

The generator follows the existing incremental-pipeline rules. Pipeline models hold no `ISymbol`, `SyntaxNode` or `Compilation`, and are compared by value (`EquatableArray<T>`, `record` models). The pipeline uses `ForAttributeWithMetadataName("Jakar.Json.GenerateJsonAttribute")`, combined with a `CompilationProvider.Select` for `[assembly: JsonDefaults]`. One file is emitted per type, plus a shared file per assembly for enum codecs: each enum gets its codec emitted once per assembly, wherever it's used.

---

## 12. Verification

Reliability claims are only as good as their tests. Status as of 10/07/2026: 197 runtime and end-to-end tests (`Jakar.Json.Tests`), 24 generator and code-fix tests (`Jakar.Json.Generator.Tests`), 55 span tests (`Jakar.Extensions.Tests`), all passing.

1. **Conformance.** *Done, partly.* `ConformanceTests` runs 130 accept/reject cases modeled on the [JSONTestSuite](https://github.com/nst/JSONTestSuite) `y_` / `n_` corpus through four paths: the reader (UTF-16, UTF-8) and the tape (UTF-16, UTF-8), plus invalid UTF-8, depth bombs (100,000 levels) and error positions. *Open:* running the downloaded suite itself, including the `i_` cases.
2. **Idempotency.** *Done.* `DeterminismTests` asserts I1 under `tr-TR`, `ar-SA`, `de-DE`, `fr-FR`, `ja-JP`, `fa-IR` and invariant cultures, I4 on loosely spelled input, and I5 across models; model tests assert I3 and I4 for every supported type. *Open:* Bogus-driven property tests, and time zone / OS matrices in CI.
3. **Allocations.** *Done.* `AllocationTests` asserts the §6 budget exactly (see §6).
4. **Robustness.** *Done, partly.* `RobustnessTests` runs 40,000 seeded mutations and random byte strings through every read path with the invariant "returns or fails, never throws", plus 200,000-level mixed nesting. *Open:* coverage-guided fuzzing (SharpFuzz/libFuzzer) as a long-running CI job.
5. **Performance.** *Measured.* `JakarJson_Benchmarks` (`Jakar.Extensions.Experiments`, run with `-- --bench --filter *JakarJson*`) compares against the STJ source generator and Newtonsoft on ≈ 200 B, 10 KB and 1 MB documents; results are in §12.1.
6. **AOT.** *Done.* `AotCompatibility.TestApp` runs nine Jakar.Json checks (both encodings, sorted keys, `ISpanParsable`, polymorphism, NDJSON, tape, DOM, malformed input). The Native AOT publish (`PublishAot`, trimming from `Main`) and the rooted trim analysis (`TrimAnalysis=true`, which analyzes all of Jakar.Json and Jakar.Spans) both produce zero IL2xxx/IL3xxx warnings, and every check passes in both executables.
7. **Generator.** *Done.* Generated output compiles for every shape in §7.4 (end to end, in `Jakar.Json.Tests`); each diagnostic JJSON001–015 has a test; the three code fixes are applied and the result recompiled; editing an unrelated file leaves every generator step cached.

### 12.1 Benchmark results

BenchmarkDotNet 0.15.8, `--job short` (3 iterations: error bars are wide, so treat differences under ~10% as noise), .NET 10.0.12, x64 AVX2, 10/07/2026. The model is an order with 1, 75 or 7,500 lines (≈ 200 B, 10 KB, 1 MB). Ratio is time relative to System.Text.Json's source generator (lower is faster).

| Operation | ≈ 200 B | 10 KB | 1 MB | Allocated vs STJ |
|---|---|---|---|---|
| Serialize to `string` | 0.89 | 0.71 | 0.69 | 93–105% (the string) |
| Serialize into a caller `char` buffer (`TryFormat`) | 0.92 | 0.68 | 0.59 | **0 B** |
| Serialize to UTF-8 `byte[]` | 0.85 | 0.96 | 0.94 | 95–103% (the array) |
| Serialize into a caller `byte` buffer (`TryFormat`) | 0.96 | 0.93 | 0.87 | **0 B** |
| Deserialize from `string` | 0.94 | 0.89 | 0.89 | 43%, 86%, 95% |
| Deserialize from UTF-8 | 1.19 | 1.01 | 0.96 | 43%, 86%, 95% |
| Navigate: sum one field over every line (`JsonTape` vs `JsonDocument`) | 1.08 | 1.00 | 1.07 | 64 B vs 72 B |

Newtonsoft.Json is 2.0–2.6× slower than STJ when serializing and 1.2–1.7× slower when deserializing, allocating 1.3–6× as much.

Against §12.5's target (parity with STJ source generation, 0 B on writes): writes are at parity or faster, and 0 B into caller buffers. Reads are at parity or faster except small UTF-8 documents (1.19×, within this run's error bars), while allocating less in every case. Jakar.Json's output is slightly longer (the `" : "` separator), so its throughput per byte is a little better than these ratios suggest. Navigation is at parity. The obvious next optimizations are a first-character switch in `__JsonMatch` and a UTF-8 fast path for small documents.


---

## 13. Delivery plan

All phases are implemented. What remains open is listed per phase.

| Phase | Scope | Status |
|---|---|---|
| **P0 — spans** | `ValueUtf8Builder`, `ValueSpanReader<T>`; the `Jakar.Spans` package with `ValueStringBuilder` and `Sizes` type-forwarded from `Jakar.Extensions`; canonical number and escape primitives (`JsonNumbers`, `JsonEscaper`) | **Done.** |
| **P1 — core I/O** | `IJsonWriter`/`IJsonReader`, `JsonWriter`, `JsonUtf8Writer`, `JsonReader<TChar>`, `JsonLexer<TChar>`, `JsonError`, `JsonName`, writer and reader options | **Done.** Open: the downloaded JSONTestSuite run and long-running fuzzing (§12.1, §12.4). |
| **P2 — generator** | attributes, settings resolution, objects, constructors, nullable, enums, collections, maps, `ISpanFormattable`/`ISpanParsable` and UTF-8 counterparts, §8 helpers, JJSON001–013 | **Done.** |
| **P3 — advanced** | polymorphism, `IJsonConverter<T>`, extension data, generics, STJ attribute migration (§4.4), code fixes, JJSON014–015 | **Done.** |
| **P4 — dynamic** | `JsonTape`/`JsonItem`, `JNode` family, `JsonTapeReader`, `JNodeReader`, `JNodeWriter` | **Done.** |
| **P5 — release** | NDJSON (`ReadLines<T>`/`WriteLines<T>`), benchmarks, README, NuGet packaging (generator and code fixes in `analyzers/dotnet/cs`), Native AOT check | **Done.** Packages not yet published. |

---

## 14. Decisions

Resolved 10/07/2026. These were the open questions of the first draft.

| # | Question | Decision |
|---|---|---|
| D1 | `Jakar.Json` depended on the whole of `Jakar.Extensions` (and `Jakar.SystemTextJson`) just for its span types. | Move `ValueStringBuilder`, `ValueUtf8Builder`, `ValueSpanReader<T>` and `Sizes` into a new dependency-free `Jakar.Spans` package, keeping the `Jakar.Extensions` namespace. `Jakar.Json` and `Jakar.Extensions` both reference it (§2, §3.1). *Done:* `Jakar.Extensions` type-forwards the moved types, so the move breaks neither source nor binaries. |
| D2 | Default for `DateTime` values with `Kind = Local`. | `ConvertToUtc`: the only choice that satisfies I1. They read back as the same instant with `Kind = Utc` (§5.4). |
| D3 | DOM type names. | `JNode`, `JObjectNode`, `JArrayNode`, `JValueNode`. They don't collide with Newtonsoft (`JObject`) or System.Text.Json (`JsonObject`) in files that import several JSON libraries. |
| D4 | A resumable streaming reader in v1? | No. Streams are buffered whole, up to `MaxDocumentBytes`. Large line-delimited files are handled by `JsonCodec.ReadLines<T>` / `ReadLinesAsync<T>` in P5 (§8.2). |
| D5 | `UnorderedCollections = Sorted` costs O(n log n) per write of a hash-based collection. | Keep `Sorted` as the default: idempotency is a stated priority. Types and members can opt out with `Enumeration`. |
| D6 | Name clash between this package's `IJsonModel<T>` and `Jakar.SystemTextJson`'s. | Rename this package's interface to `IJsonSerializable<T>` (§7.1). |
