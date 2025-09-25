// Jakar.Extensions :: Jakar.Extensions.SignalR
// 09/25/2025  09:47

using System.Text.Json;
using System.Text.Json.Serialization;



namespace Jakar.Extensions.SignalR.Chats;


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
[JsonSerializable(typeof(InstantMessage[]))]
[JsonSerializable(typeof(InstantMessageCollection[]))]
[JsonSerializable(typeof(ChatUserCollection[]))]
[JsonSerializable(typeof(ChatUser[]))]
public sealed partial class JakarSignalRContext : JsonSerializerContext
{
    static JakarSignalRContext()
    {
        Default.InstantMessage.Register();
        Default.InstantMessageArray.Register();

        Default.InstantMessageCollection.Register();
        Default.InstantMessageCollectionArray.Register();
    }
}
