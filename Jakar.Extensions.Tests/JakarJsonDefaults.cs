// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/23/2026

// The test models read like the library's: what an app that uses Jakar.Extensions' lenient defaults declares for its own [JsonModel] types.

[assembly: JsonDefaults(Enums = JsonEnumFormat.Number, NameMatching = JsonNameMatching.OrdinalIgnoreCase, NumbersFromStrings = JsonNumbersFromStrings.Allow, AllowComments = JsonToggle.On, AllowTrailingCommas = JsonToggle.On)]
