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
                             RespectRequiredConstructorParameters = true,
                             GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(HashSet<string>))]
[JsonSerializable(typeof(DbOptions[]))]
[JsonSerializable(typeof(EmailSettings[]))]
[JsonSerializable(typeof(ResxRowRecord[]))]
[JsonSerializable(typeof(UserRecord[]))]
[JsonSerializable(typeof(AddressRecord[]))]
[JsonSerializable(typeof(UserAddressRecord[]))]
[JsonSerializable(typeof(FileRecord[]))]
[JsonSerializable(typeof(GroupRecord[]))]
[JsonSerializable(typeof(UserGroupRecord[]))]
[JsonSerializable(typeof(RoleRecord[]))]
[JsonSerializable(typeof(UserRoleRecord[]))]
[JsonSerializable(typeof(UserLoginProviderRecord[]))]
[JsonSerializable(typeof(RecoveryCodeRecord[]))]
[JsonSerializable(typeof(UserRecoveryCodeRecord[]))]
[JsonSerializable(typeof(MigrationRecord[]))]
[JsonSerializable(typeof(FusionCacheEntryOptionsWrapper))]
[JsonSerializable(typeof(FrozenDictionary<string, ColumnMetaData>))]
public sealed partial class JakarDatabaseContext : JsonSerializerContext
{
    static JakarDatabaseContext()
    {
        Default.HashSetString.Register();

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

        Default.FusionCacheEntryOptionsWrapper.Register();
    }
}



/*
public sealed class ObsoleteIgnoringResolver( IJsonTypeInfoResolver inner ) : IJsonTypeInfoResolver
{
    private readonly IJsonTypeInfoResolver __inner = inner;

    public JsonTypeInfo? GetTypeInfo( Type type, JsonSerializerOptions options )
    {
        JsonTypeInfo? info = __inner.GetTypeInfo(type, options);

        if ( info?.Kind is JsonTypeInfoKind.Object )
        {
            for ( int i = info.Properties.Count - 1; i >= 0; i-- )
            {
                MemberInfo? memberInfo = info.Properties[i].AttributeProvider as MemberInfo;
                if ( memberInfo?.GetCustomAttribute<ObsoleteAttribute>() is not null ) { info.Properties.RemoveAt(i); }
            }
        }

        return info;
    }
}
*/
