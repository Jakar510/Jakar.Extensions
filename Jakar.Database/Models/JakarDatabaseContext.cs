// Jakar.Extensions :: Jakar.Database
// 09/24/2025  11:25

using Jakar.Database.Resx;



namespace Jakar.Database;


[JsonSourceGenerationOptions(MaxDepth = 128,
                             IndentSize = 4,
                             NewLine = "\n",
                             IndentCharacter = ' ',
                             WriteIndented = true,
                             RespectNullableAnnotations = true,
                             AllowTrailingCommas = true,
                             AllowOutOfOrderMetadataProperties = true,
                             IgnoreReadOnlyProperties = true,
                             IncludeFields = true,
                             IgnoreReadOnlyFields = false,
                             PropertyNameCaseInsensitive = false,
                             ReadCommentHandling = JsonCommentHandling.Skip,
                             UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
                             RespectRequiredConstructorParameters = true)]
[JsonSerializable(typeof(DbOptions[]))]
[JsonSerializable(typeof(EmailSettings[]))]
[JsonSerializable(typeof(ResxRowRecord[]))]
[JsonSerializable(typeof(UserRecord[]))]
[JsonSerializable(typeof(AddressRecord[]))]
[JsonSerializable(typeof(FileRecord[]))]
[JsonSerializable(typeof(GroupRecord[]))]
[JsonSerializable(typeof(RoleRecord[]))]
[JsonSerializable(typeof(UserAddressRecord[]))]
[JsonSerializable(typeof(UserGroupRecord[]))]
[JsonSerializable(typeof(UserLoginProviderRecord[]))]
[JsonSerializable(typeof(UserRoleRecord[]))]
[JsonSerializable(typeof(RecoveryCodeRecord[]))]
public sealed partial class JakarDatabaseContext : JsonSerializerContext
{
    static JakarDatabaseContext()
    {
        Default.EmailSettings.Register();
        Default.EmailSettingsArray.Register();

        Default.ResxRowRecord.Register();
        Default.ResxRowRecordArray.Register();

        Default.UserRecord.Register();
        Default.UserRecordArray.Register();

        Default.AddressRecord.Register();
        Default.AddressRecordArray.Register();

        Default.FileRecord.Register();
        Default.FileRecordArray.Register();

        Default.GroupRecord.Register();
        Default.GroupRecordArray.Register();

        Default.RoleRecord.Register();
        Default.RoleRecordArray.Register();

        Default.UserAddressRecord.Register();
        Default.UserAddressRecordArray.Register();

        Default.UserLoginProviderRecord.Register();
        Default.UserLoginProviderRecordArray.Register();

        Default.UserRoleRecord.Register();
        Default.UserRoleRecordArray.Register();

        Default.RecoveryCodeRecord.Register();
        Default.RecoveryCodeRecordArray.Register();
    }
}
