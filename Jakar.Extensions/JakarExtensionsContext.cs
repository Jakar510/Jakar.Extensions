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
                             RespectRequiredConstructorParameters = true,
                             Converters = [typeof(EncodingConverter)])]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(double[]))]
[JsonSerializable(typeof(double?[]))]
[JsonSerializable(typeof(decimal[]))]
[JsonSerializable(typeof(decimal?[]))]
[JsonSerializable(typeof(float[]))]
[JsonSerializable(typeof(float?[]))]
[JsonSerializable(typeof(Int128[]))]
[JsonSerializable(typeof(UInt128[]))]
[JsonSerializable(typeof(Int128?[]))]
[JsonSerializable(typeof(UInt128?[]))]
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
[JsonSerializable(typeof(Uri[]))]
[JsonSerializable(typeof(DateTime[]))]
[JsonSerializable(typeof(DateTime?[]))]
[JsonSerializable(typeof(DateOnly[]))]
[JsonSerializable(typeof(DateOnly?[]))]
[JsonSerializable(typeof(TimeOnly[]))]
[JsonSerializable(typeof(TimeOnly?[]))]
[JsonSerializable(typeof(DateTimeOffset[]))]
[JsonSerializable(typeof(DateTimeOffset?[]))]
[JsonSerializable(typeof(TimeSpan[]))]
[JsonSerializable(typeof(TimeSpan?[]))]
[JsonSerializable(typeof(byte[]))]
[JsonSerializable(typeof(Memory<byte>[]))]
[JsonSerializable(typeof(Memory<byte>?[]))]
[JsonSerializable(typeof(ReadOnlyMemory<byte>[]))]
[JsonSerializable(typeof(ReadOnlyMemory<byte>?[]))]
[JsonSerializable(typeof(JsonNode[]))]
[JsonSerializable(typeof(JsonObject[]))]
[JsonSerializable(typeof(JsonArray[]))]
[JsonSerializable(typeof(JsonValue[]))]
[JsonSerializable(typeof(JsonDocument[]))]
[JsonSerializable(typeof(JsonElement[]))]
[JsonSerializable(typeof(Version[]))]
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
[JsonSerializable(typeof(Error[]))]
[JsonSerializable(typeof(Errors[]))]
[JsonSerializable(typeof(Alert[]))]
[JsonSerializable(typeof(LoginRequest[]))]
[JsonSerializable(typeof(LoginRequestVersion[]))]
[JsonSerializable(typeof(LoginRequestValue[]))]
[JsonSerializable(typeof(IDictionary[]))]
[JsonSerializable(typeof(AppInformation[]))]
[JsonSerializable(typeof(GcInfo[]))]
[JsonSerializable(typeof(GcMemoryInformation[]))]
[JsonSerializable(typeof(GcGenerationInformation[]))]
[JsonSerializable(typeof(ThreadInformation[]))]
[JsonSerializable(typeof(HashSet<string>))]
[JsonSerializable(typeof(HashSet<double>))]
[JsonSerializable(typeof(HashSet<float>))]
[JsonSerializable(typeof(HashSet<long>))]
[JsonSerializable(typeof(HashSet<ulong>))]
[JsonSerializable(typeof(HashSet<int>))]
[JsonSerializable(typeof(HashSet<uint>))]
[JsonSerializable(typeof(HashSet<short>))]
[JsonSerializable(typeof(HashSet<ushort>))]
[JsonSerializable(typeof(HashSet<Int128>))]
[JsonSerializable(typeof(HashSet<UInt128>))]
[JsonSerializable(typeof(HashSet<Guid>))]
[JsonSerializable(typeof(HashSet<DateTime>))]
[JsonSerializable(typeof(HashSet<DateTimeOffset>))]
[JsonSerializable(typeof(HashSet<DateOnly>))]
[JsonSerializable(typeof(HashSet<TimeOnly>))]
[JsonSerializable(typeof(HashSet<TimeSpan>))]
public sealed partial class JakarExtensionsContext : JsonSerializerContext
{
    static JakarExtensionsContext()
    {
        Register_Natives();
        Register_ByteCollections();
        Register_Jsons();
        Register_Misc();
        Register_Debug();
        Register_FileSystem();
        Register_Logins();
        Register_Errors();
        Register_GcInfo();
        Register_HashSet();
    }


    private static void Register_Natives()
    {
        Default.String.Register();
        Default.StringArray.Register();


        Default.Int16.Register();
        Default.NullableInt16.Register();
        Default.Int16Array.Register();
        Default.NullableInt16Array.Register();


        Default.Int32.Register();
        Default.NullableInt32.Register();
        Default.Int32Array.Register();
        Default.NullableInt32Array.Register();


        Default.Int64.Register();
        Default.NullableInt64.Register();
        Default.Int64Array.Register();
        Default.NullableInt64Array.Register();


        Default.UInt64.Register();
        Default.NullableUInt64.Register();
        Default.UInt64Array.Register();
        Default.NullableUInt64Array.Register();


        Default.Single.Register();
        Default.NullableSingle.Register();
        Default.SingleArray.Register();
        Default.NullableSingleArray.Register();


        Default.Double.Register();
        Default.NullableDouble.Register();
        Default.DoubleArray.Register();
        Default.NullableDoubleArray.Register();


        Default.Decimal.Register();
        Default.NullableDecimal.Register();
        Default.DecimalArray.Register();
        Default.NullableDecimalArray.Register();


        Default.Uri.Register();
        Default.UriArray.Register();


        Default.DateTime.Register();
        Default.DateTimeArray.Register();
        Default.NullableDateTime.Register();
        Default.NullableDateTimeArray.Register();


        Default.DateTimeOffset.Register();
        Default.DateTimeOffsetArray.Register();
        Default.NullableDateTimeOffset.Register();
        Default.NullableDateTimeOffsetArray.Register();


        Default.DateOnly.Register();
        Default.DateOnlyArray.Register();
        Default.NullableDateOnly.Register();
        Default.NullableDateOnlyArray.Register();


        Default.TimeOnly.Register();
        Default.TimeOnlyArray.Register();
        Default.NullableTimeOnly.Register();
        Default.NullableTimeOnlyArray.Register();


        Default.TimeSpan.Register();
        Default.TimeSpanArray.Register();
        Default.NullableTimeSpan.Register();
        Default.NullableTimeSpanArray.Register();
    }
    private static void Register_ByteCollections()
    {
        Default.ByteArray.Register();
        Default.MemoryByte.Register();
        Default.MemoryByteArray.Register();
        Default.NullableMemoryByte.Register();
        Default.NullableMemoryByteArray.Register();
    }
    private static void Register_Jsons()
    {
        Default.JsonNode.Register();
        Default.JsonNodeArray.Register();
        Default.JsonObject.Register();
        Default.JsonObjectArray.Register();
        Default.JsonArray.Register();
        Default.JsonArrayArray.Register();
        Default.JsonValue.Register();
        Default.JsonValueArray.Register();
        Default.JsonDocument.Register();
        Default.JsonDocumentArray.Register();
        Default.JsonElement.Register();
        Default.JsonElementArray.Register();
    }
    private static void Register_Misc()
    {
        Default.AppVersion.Register();
        Default.AppVersionArray.Register();
        Default.FileMetaData.Register();
        Default.FileMetaDataArray.Register();
        Default.LanguageCollection.Register();
        Default.LanguageCollectionArray.Register();
        Default.AppInformation.Register();
        Default.AppInformationArray.Register();
    }
    private static void Register_Debug()
    {
        Default.MethodAttributes.Register();
        Default.MethodAttributesArray.Register();
        Default.ParameterDetails.Register();
        Default.ParameterDetailsArray.Register();
        Default.MethodDetails.Register();
        Default.MethodDetailsArray.Register();
        Default.ExceptionDetails.Register();
        Default.ExceptionDetailsArray.Register();
    }
    private static void Register_FileSystem()
    {
        Default.LocalDirectory.Register();
        Default.LocalDirectoryArray.Register();
        Default.LocalFile.Register();
        Default.LocalFileArray.Register();
        Default.LocalFileWatcher.Register();
        Default.LocalFileWatcherArray.Register();
    }
    private static void Register_Logins()
    {
        Default.LoginRequest.Register();
        Default.LoginRequestArray.Register();
        Default.LoginRequestVersion.Register();
        Default.LoginRequestVersionArray.Register();
        Default.LoginRequestValue.Register();
        Default.LoginRequestValueArray.Register();
    }
    private static void Register_Errors()
    {
        Default.OneOfErrors.Register();
        Default.OneOfErrorsArray.Register();
        Default.Alert.Register();
        Default.AlertArray.Register();
        Default.Errors.Register();
        Default.ErrorsArray.Register();
        Default.Error.Register();
        Default.ErrorArray.Register();
    }
    private static void Register_GcInfo()
    {
        Default.GcInfo.Register();
        Default.GcInfoArray.Register();
        Default.GcMemoryInformation.Register();
        Default.GcMemoryInformationArray.Register();
        Default.GcGenerationInformation.Register();
        Default.GcGenerationInformationArray.Register();
        Default.ThreadInformation.Register();
        Default.ThreadInformationArray.Register();
    }
    private static void Register_HashSet()
    {
        Default.HashSetString.Register();
        Default.HashSetDouble.Register();
        Default.HashSetSingle.Register();
        Default.HashSetInt64.Register();
        Default.HashSetUInt64.Register();
        Default.HashSetInt32.Register();
        Default.HashSetUInt32.Register();
        Default.HashSetInt16.Register();
        Default.HashSetUInt16.Register();
        Default.HashSetGuid.Register();
        Default.HashSetDateTime.Register();
        Default.HashSetDateTimeOffset.Register();
        Default.HashSetDateOnly.Register();
        Default.HashSetTimeOnly.Register();
        Default.HashSetTimeSpan.Register();
    }
}
