// Jakar.Extensions :: Jakar.Shapes
// 09/24/2025  23:36

using System.Text.Json;



namespace Jakar.Shapes;


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
[JsonSerializable(typeof(Circle[]))]
[JsonSerializable(typeof(Triangle[]))]
[JsonSerializable(typeof(ReadOnlyThickness[]))]
[JsonSerializable(typeof(ReadOnlyLine[]))]
[JsonSerializable(typeof(ReadOnlyPoint[]))]
[JsonSerializable(typeof(ReadOnlyPointF[]))]
[JsonSerializable(typeof(ReadOnlyRectangle[]))]
[JsonSerializable(typeof(ReadOnlyRectangleF[]))]
[JsonSerializable(typeof(MutableRectangle[]))]
[JsonSerializable(typeof(ReadOnlySize[]))]
[JsonSerializable(typeof(ReadOnlySizeF[]))]
[JsonSerializable(typeof(MutableSize[]))]
[JsonSerializable(typeof(Spline[]))]
[JsonSerializable(typeof(Polygon[]))]
public sealed partial class JakarShapesContext : JsonSerializerContext
{
    static JakarShapesContext()
    {
        Default.Circle.Register();
        Default.CircleArray.Register();

        Default.Triangle.Register();
        Default.TriangleArray.Register();

        Default.ReadOnlyLine.Register();
        Default.ReadOnlyLineArray.Register();

        Default.ReadOnlyPointF.Register();
        Default.ReadOnlyPointFArray.Register();

        Default.ReadOnlyPoint.Register();
        Default.ReadOnlyPointArray.Register();

        Default.ReadOnlyRectangle.Register();
        Default.ReadOnlyRectangleArray.Register();

        Default.ReadOnlyRectangleF.Register();
        Default.ReadOnlyRectangleFArray.Register();

        Default.MutableRectangle.Register();
        Default.MutableRectangleArray.Register();

        Default.ReadOnlySize.Register();
        Default.ReadOnlySizeArray.Register();

        Default.ReadOnlySizeF.Register();
        Default.ReadOnlySizeFArray.Register();

        Default.MutableSize.Register();
        Default.MutableSizeArray.Register();
    }
}
