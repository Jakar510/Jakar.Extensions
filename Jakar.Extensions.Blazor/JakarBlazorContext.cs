// Jakar.Extensions :: Jakar.Extensions.Blazor
// 09/24/2025  21:31

using System.Text.Json;
using System.Text.Json.Serialization;



namespace Jakar.Extensions.Blazor;


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
[JsonSerializable(typeof(ModelErrorState[]))]
public sealed partial class JakarBlazorContext : JsonSerializerContext
{
    static JakarBlazorContext()
    {
        Default.ModelErrorState.Register();
        Default.ModelErrorStateArray.Register();
    }
}
