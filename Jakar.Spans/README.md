# Jakar.Spans

Dependency-free, allocation-free span primitives. Types live in the `Jakar.Extensions` namespace; `Jakar.Extensions` references this package and type-forwards `ValueStringBuilder` and `Sizes` here, so existing binaries keep working.

| Type | Purpose |
|---|---|
| `ValueStringBuilder` | A stack-only UTF-16 builder: starts from a caller buffer (`stackalloc char[256]`) or a pooled array, formats values in place through `ISpanFormattable`. |
| `ValueUtf8Builder` | The UTF-8 counterpart: `Append("..."u8)`, UTF-16 text transcoded in place, values formatted through `IUtf8SpanFormattable`. |
| `ValueSpanReader<T>` | A forward cursor over a `ReadOnlySpan<T>` (`char`, `byte`, ...): `Peek`, `TryRead`, `TryReadExact`, `SkipWhile(SearchValues<T>)`, checkpoints with `Position` / `Rewind`. `Try*` members never throw and never move on failure. |
| `Sizes` | Maximum formatted lengths per type, used to pre-size buffers. |

```csharp
using ValueUtf8Builder builder = new(stackalloc byte[256]);
builder.Append("{\"total\" : "u8).AppendUtf8Formattable(12.5m).Append((byte)'}');

ValueSpanReader<char> reader = new("key=value");
reader.TryReadTo('=', out ReadOnlySpan<char> key);   // "key"; reader.Remaining is "value"
```
