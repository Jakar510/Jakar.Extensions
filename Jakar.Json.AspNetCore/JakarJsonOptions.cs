// Jakar.Json.AspNetCore
// 10/14/2026

namespace Jakar.Json.AspNetCore;


/// <summary> App-wide overrides for Jakar.Json in ASP.NET Core. Left <see langword="null"/>, each type uses its own settings (<c>[JsonModel]</c>, <c>[assembly: JsonDefaults]</c>). </summary>
public sealed class JakarJsonOptions
{
    /// <summary> Replaces every type's reader settings for request bodies, e.g. to allow comments or raise <see cref="JsonReaderOptions.MaxDocumentBytes"/>. </summary>
    public JsonReaderOptions? ReaderOptions { get; set; }

    /// <summary> Replaces every type's writer settings for responses, e.g. to indent them. </summary>
    public JsonWriterOptions? WriterOptions { get; set; }


    internal static JakarJsonOptions From( HttpContext context ) => context.RequestServices.GetService<IOptions<JakarJsonOptions>>()?.Value ?? Default;

    internal static readonly JakarJsonOptions Default = new();
}
