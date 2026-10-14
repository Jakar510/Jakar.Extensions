// Jakar.Extensions :: Jakar.Extensions
// 10/07/2026

// Jakar.Json defaults for the library's [JsonModel] types. Context-backed ones mirror their System.Text.Json context's options first (enums, naming, case,
// numbers from strings, comments, trailing commas, indentation), so these mostly cover the context-less ones (UserRights). They match the contexts,
// so each serializer reads the other's output: enums as numbers, names matched ignoring case, numbers accepted from strings, comments and trailing commas allowed.

[assembly: Jakar.Json.JsonDefaults(Enums = Jakar.Json.JsonEnumFormat.Number, NameMatching = Jakar.Json.JsonNameMatching.OrdinalIgnoreCase, NumbersFromStrings = Jakar.Json.JsonNumbersFromStrings.Allow, AllowComments = Jakar.Json.JsonToggle.On, AllowTrailingCommas = Jakar.Json.JsonToggle.On)]
