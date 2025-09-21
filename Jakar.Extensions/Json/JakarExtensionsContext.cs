// Jakar.Extensions :: Jakar.Extensions
// 09/18/2025  17:05

namespace Jakar.Extensions;


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
[JsonSerializable(typeof(AppVersion))]
[JsonSerializable(typeof(MethodDetails))]
[JsonSerializable(typeof(ExceptionDetails))]
[JsonSerializable(typeof(LocalDirectory))]
[JsonSerializable(typeof(LocalFile))]
[JsonSerializable(typeof(LocalFileWatcher))]
[JsonSerializable(typeof(LanguageCollection))]
[JsonSerializable(typeof(FileMetaData))]
public sealed partial class JakarExtensionsContext : JsonSerializerContext
{
    static JakarExtensionsContext()
    {
        Default.AppVersion.Register();
        Default.MethodAttributes.Register();
        Default.ParameterDetails.Register();
        Default.MethodDetails.Register();
        Default.ExceptionDetails.Register();
        Default.FileMetaData.Register();

        Default.LocalDirectory.Register();
        Default.LocalFile.Register();
        Default.LocalFileWatcher.Register();

        Default.LanguageCollection.Register();
    }
}
