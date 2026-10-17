// Jakar.Extensions :: Experiments
// 10/07/2026

namespace Experiments;


/// <summary>
///     System.Text.Json metadata for <see cref="SharedUserModel"/>. A <c>[JsonModel]</c> type must be registered in its context, and <c>UserGuidJsonContext</c> lives in Jakar.Extensions,
///     so a model declared here needs its own. Same options as <c>UserGuidJsonContext</c>, except enums are written as names to match <see cref="SharedUserModel"/>'s
///     <c>[GenerateJson(Enums = Name)]</c> (System.Text.Json's string-enum converter also reads numbers, and Jakar.Json reads either form).
/// </summary>
[JsonSourceGenerationOptions(JsonSerializerDefaults.General,
                             WriteIndented = true,
                             AllowTrailingCommas = true,
                             ReadCommentHandling = JsonCommentHandling.Skip,
                             PropertyNameCaseInsensitive = true,
                             IncludeFields = true,
                             NumberHandling = JsonNumberHandling.AllowReadingFromString,
                             UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
                             UseStringEnumConverter = true,
                             Converters = [typeof(EncodingConverter), typeof(ObservableCollectionJsonConverter<UserAddress>), typeof(ObservableCollectionJsonConverter<GroupModel>), typeof(ObservableCollectionJsonConverter<RoleModel>)])]
[JsonSerializable(typeof(SharedUserModel))]
public sealed partial class SharedUserJsonContext : JsonSerializerContext;
