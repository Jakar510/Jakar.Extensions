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
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(string?[]))]
[JsonSerializable(typeof(double[]))]
[JsonSerializable(typeof(double?[]))]
[JsonSerializable(typeof(float[]))]
[JsonSerializable(typeof(float?[]))]
[JsonSerializable(typeof(long[]))]
[JsonSerializable(typeof(long?[]))]
[JsonSerializable(typeof(int[]))]
[JsonSerializable(typeof(int?[]))]
[JsonSerializable(typeof(short[]))]
[JsonSerializable(typeof(short?[]))]
[JsonSerializable(typeof(ushort[]))]
[JsonSerializable(typeof(ushort?[]))]
[JsonSerializable(typeof(uint[]))]
[JsonSerializable(typeof(uint?[]))]
[JsonSerializable(typeof(ulong[]))]
[JsonSerializable(typeof(ulong?[]))]
[JsonSerializable(typeof(DateTime[]))]
[JsonSerializable(typeof(DateTime?[]))]
[JsonSerializable(typeof(DateOnly[]))]
[JsonSerializable(typeof(DateOnly?[]))]
[JsonSerializable(typeof(DateTimeOffset[]))]
[JsonSerializable(typeof(DateTimeOffset?[]))]
[JsonSerializable(typeof(TimeSpan[]))]
[JsonSerializable(typeof(TimeSpan?[]))]
[JsonSerializable(typeof(Type[]))]
[JsonSerializable(typeof(AppVersion[]))]
[JsonSerializable(typeof(ParameterDetails[]))]
[JsonSerializable(typeof(MethodDetails[]))]
[JsonSerializable(typeof(ExceptionDetails[]))]
[JsonSerializable(typeof(MethodAttributes[]))]
[JsonSerializable(typeof(LocalDirectory[]))]
[JsonSerializable(typeof(LocalFile[]))]
[JsonSerializable(typeof(LocalFileWatcher[]))]
[JsonSerializable(typeof(LanguageCollection[]))]
[JsonSerializable(typeof(FileMetaData[]))]
[JsonSerializable(typeof(OneOfErrors[]))]
[JsonSerializable(typeof(JsonNode[]))]
[JsonSerializable(typeof(Error[]))]
[JsonSerializable(typeof(Errors[]))]
[JsonSerializable(typeof(Alert[]))]
[JsonSerializable(typeof(AppInformation[]))]
public sealed partial class JakarExtensionsContext : JsonSerializerContext
{
    static JakarExtensionsContext()
    {
        Default.StringArray.Register();
        Default.NullableSingleArray.Register();

        Default.Int16Array.Register();
        Default.NullableInt16Array.Register();

        Default.Int32Array.Register();
        Default.NullableInt32Array.Register();

        Default.Int64Array.Register();
        Default.NullableInt64Array.Register();

        Default.UInt64Array.Register();
        Default.NullableUInt64Array.Register();

        Default.SingleArray.Register();
        Default.NullableSingleArray.Register();

        Default.DoubleArray.Register();
        Default.NullableDoubleArray.Register();

        Default.DateTime.Register();
        Default.DateTimeArray.Register();

        Default.DateTimeOffset.Register();
        Default.DateTimeOffsetArray.Register();

        Default.DateOnly.Register();
        Default.DateOnlyArray.Register();

        Default.TimeSpan.Register();
        Default.TimeSpanArray.Register();

        Default.Type.Register();
        Default.TypeArray.Register();


        Default.AppVersion.Register();
        Default.AppVersionArray.Register();

        Default.MethodAttributes.Register();
        Default.MethodAttributesArray.Register();

        Default.ParameterDetails.Register();
        Default.ParameterDetailsArray.Register();

        Default.MethodDetails.Register();
        Default.MethodDetailsArray.Register();

        Default.ExceptionDetails.Register();
        Default.ExceptionDetailsArray.Register();

        Default.FileMetaData.Register();
        Default.FileMetaDataArray.Register();

        Default.LocalDirectory.Register();
        Default.LocalDirectoryArray.Register();

        Default.LocalFile.Register();
        Default.LocalFileArray.Register();

        Default.LocalFileWatcher.Register();
        Default.LocalFileWatcherArray.Register();

        Default.LanguageCollection.Register();
        Default.LanguageCollectionArray.Register();

        Default.OneOfErrors.Register();
        Default.OneOfErrorsArray.Register();

        Default.JsonNode.Register();
        Default.JsonNodeArray.Register();

        Default.Alert.Register();
        Default.AlertArray.Register();

        Default.Errors.Register();
        Default.ErrorsArray.Register();

        Default.Error.Register();
        Default.ErrorArray.Register();

        Default.AppInformation.Register();
        Default.AppInformationArray.Register();
    }
}
