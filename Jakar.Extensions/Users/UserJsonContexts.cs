// Jakar.Extensions :: Jakar.Extensions
// 10/02/2026

namespace Jakar.Extensions.UserGuid
{
    /// <summary> Source-generated JSON metadata for the <see cref="Guid"/>-keyed user models. Same options as <see cref="JakarExtensionsContext"/>. </summary>
    [JsonSourceGenerationOptions(JsonSerializerDefaults.General,
                                 WriteIndented = true,
                                 AllowTrailingCommas = true,
                                 ReadCommentHandling = JsonCommentHandling.Skip,
                                 PropertyNameCaseInsensitive = true,
                                 IncludeFields = true,
                                 NumberHandling = JsonNumberHandling.AllowReadingFromString,
                                 UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
                                 // net10.0 can't attach a converter to the open generic ObservableCollection<T>, so register the closed ones the models use:
                                 // they serialize the unfiltered contents and read with one bulk insert (AOT-plan §2.13).
                                 Converters = [typeof(EncodingConverter), typeof(ObservableCollectionJsonConverter<UserAddress>), typeof(ObservableCollectionJsonConverter<GroupModel>), typeof(ObservableCollectionJsonConverter<RoleModel>)])]
    [JsonSerializable(typeof(UserAddress))]
    [JsonSerializable(typeof(GroupModel))]
    [JsonSerializable(typeof(RoleModel))]
    [JsonSerializable(typeof(FileData))]
    [JsonSerializable(typeof(CurrentLocation))]
    [JsonSerializable(typeof(UserModel))]
    [JsonSerializable(typeof(CreateUserModel))]
    [JsonSerializable(typeof(SessionToken))]
    [JsonSerializable(typeof(UserLoginRequest))]
    [JsonSerializable(typeof(UserDevice))]
    public sealed partial class UserGuidJsonContext : JsonSerializerContext;
}



namespace Jakar.Extensions.UserLong
{
    /// <summary> Source-generated JSON metadata for the <see cref="long"/>-keyed user models. Same options as <see cref="JakarExtensionsContext"/>. </summary>
    [JsonSourceGenerationOptions(JsonSerializerDefaults.General,
                                 WriteIndented = true,
                                 AllowTrailingCommas = true,
                                 ReadCommentHandling = JsonCommentHandling.Skip,
                                 PropertyNameCaseInsensitive = true,
                                 IncludeFields = true,
                                 NumberHandling = JsonNumberHandling.AllowReadingFromString,
                                 UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
                                 // net10.0 can't attach a converter to the open generic ObservableCollection<T>, so register the closed ones the models use:
                                 // they serialize the unfiltered contents and read with one bulk insert (AOT-plan §2.13).
                                 Converters = [typeof(EncodingConverter), typeof(ObservableCollectionJsonConverter<UserAddress>), typeof(ObservableCollectionJsonConverter<GroupModel>), typeof(ObservableCollectionJsonConverter<RoleModel>)])]
    [JsonSerializable(typeof(UserAddress))]
    [JsonSerializable(typeof(GroupModel))]
    [JsonSerializable(typeof(RoleModel))]
    [JsonSerializable(typeof(FileData))]
    [JsonSerializable(typeof(CurrentLocation))]
    [JsonSerializable(typeof(UserModel))]
    [JsonSerializable(typeof(CreateUserModel))]
    [JsonSerializable(typeof(SessionToken))]
    [JsonSerializable(typeof(UserLoginRequest))]
    [JsonSerializable(typeof(UserDevice))]
    public sealed partial class UserLongJsonContext : JsonSerializerContext;
}
