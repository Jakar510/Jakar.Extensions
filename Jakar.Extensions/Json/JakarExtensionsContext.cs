// Jakar.Extensions :: Jakar.Extensions
// 09/18/2025  17:05

namespace Jakar.Extensions;


/// <summary>
///     Source-generated System.Text.Json metadata for Jakar.Extensions' own types and the common built-in shapes.
///     <para>
///         The options are chosen so JSON written by Jakar.Extensions 10.x (Newtonsoft) still reads: case-insensitive property names, numbers in strings, comments, trailing commas and public fields.
///         Property names stay PascalCase, and output is indented, as before.
///     </para>
///     <para> <see cref="Json.Options"/> chains this context after any resolvers the app registers with <see cref="Json.AddResolver"/>. The user models have their own contexts (<c> UserGuid.UserGuidJsonContext </c>, <c> UserLong.UserLongJsonContext </c>). </para>
/// </summary>
[JsonSourceGenerationOptions(JsonSerializerDefaults.General,
                             WriteIndented = true,
                             AllowTrailingCommas = true,
                             ReadCommentHandling = JsonCommentHandling.Skip,
                             PropertyNameCaseInsensitive = true,
                             IncludeFields = true,
                             NumberHandling = JsonNumberHandling.AllowReadingFromString,
                             UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
                             Converters = [typeof(EncodingConverter)])]

// ─── Built-in shapes ──────────────────────────────────────────────────────────
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(bool?))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(HashSet<string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(Dictionary<string, string?>))]
[JsonSerializable(typeof(Dictionary<string, object?>))]
[JsonSerializable(typeof(char))]
[JsonSerializable(typeof(byte))]
[JsonSerializable(typeof(byte[]))]
[JsonSerializable(typeof(sbyte))]
[JsonSerializable(typeof(short))]
[JsonSerializable(typeof(short?))]
[JsonSerializable(typeof(ushort))]
[JsonSerializable(typeof(ushort?))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(int?))]
[JsonSerializable(typeof(int[]))]
[JsonSerializable(typeof(uint))]
[JsonSerializable(typeof(uint?))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(long?))]
[JsonSerializable(typeof(long[]))]
[JsonSerializable(typeof(ulong))]
[JsonSerializable(typeof(ulong?))]
[JsonSerializable(typeof(Int128))]
[JsonSerializable(typeof(UInt128))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(float?))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(double?))]
[JsonSerializable(typeof(double[]))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(decimal?))]
[JsonSerializable(typeof(Guid))]
[JsonSerializable(typeof(Guid?))]
[JsonSerializable(typeof(HashSet<Guid>))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(DateTime?))]
[JsonSerializable(typeof(DateTimeOffset))]
[JsonSerializable(typeof(DateTimeOffset?))]
[JsonSerializable(typeof(DateOnly))]
[JsonSerializable(typeof(DateOnly?))]
[JsonSerializable(typeof(TimeOnly))]
[JsonSerializable(typeof(TimeOnly?))]
[JsonSerializable(typeof(TimeSpan))]
[JsonSerializable(typeof(TimeSpan?))]
[JsonSerializable(typeof(Uri))]
[JsonSerializable(typeof(Version))]
[JsonSerializable(typeof(JsonNode))]
[JsonSerializable(typeof(JsonObject))]
[JsonSerializable(typeof(JsonArray))]
[JsonSerializable(typeof(JsonValue))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(JsonDocument))]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]

// ─── Jakar.Extensions types ───────────────────────────────────────────────────
[JsonSerializable(typeof(Pair))]
[JsonSerializable(typeof(StringTags))]
[JsonSerializable(typeof(Status))]
[JsonSerializable(typeof(Error))]
[JsonSerializable(typeof(Errors))]
[JsonSerializable(typeof(Alert))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(AppVersion))]
[JsonSerializable(typeof(AppInformation))]
[JsonSerializable(typeof(GcInfo))]
[JsonSerializable(typeof(ThreadInformation))]
[JsonSerializable(typeof(ExceptionDetails))]
[JsonSerializable(typeof(MethodDetails))]
[JsonSerializable(typeof(ParameterDetails))]
[JsonSerializable(typeof(FileMetaData))]
[JsonSerializable(typeof(LocalDirectory))]
[JsonSerializable(typeof(LocalFile))]
[JsonSerializable(typeof(LocalFileWatcher))]
[JsonSerializable(typeof(Language))]
[JsonSerializable(typeof(LanguageCollection))]
[JsonSerializable(typeof(Email))]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(LoginRequestVersion))]
[JsonSerializable(typeof(LoginRequestValue))]
public sealed partial class JakarExtensionsContext : JsonSerializerContext;
