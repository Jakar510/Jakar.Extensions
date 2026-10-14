// Jakar.Json.AspNetCore
// 10/14/2026

namespace Jakar.Json.AspNetCore;


/// <summary>
///     A minimal API parameter read from the JSON request body by Jakar.Json: <c>app.MapPost("/orders", (JsonBody&lt;Order&gt; order) =&gt; ...)</c>. Minimal APIs bind request bodies with
///     System.Text.Json; this binds through <see cref="BindAsync"/> instead, and tells OpenAPI the endpoint accepts a JSON <typeparamref name="T"/>.
/// </summary>
/// <remarks>
///     Results: 415 if the request isn't JSON, 413 past <see cref="JsonReaderOptions.MaxDocumentBytes"/>, 400 for malformed JSON. An empty body leaves the parameter <see langword="null"/>:
///     400 unless it's declared nullable (<c>JsonBody&lt;Order&gt;?</c>).
/// </remarks>
public sealed class JsonBody<T> : IEndpointParameterMetadataProvider
    where T : IJsonSerializable<T>
{
    private JsonBody( T value ) => Value = value;


    public T Value { get; }


    public static implicit operator T( JsonBody<T> body ) => body.Value;


    /// <summary> Called by minimal APIs to bind the parameter. </summary>
    public static async ValueTask<JsonBody<T>?> BindAsync( HttpContext context, ParameterInfo parameter )
    {
        ArgumentNullException.ThrowIfNull(context);
        HttpRequest request = context.Request;
        if ( request.ContentLength == 0 ) { return null; }

        JakarJsonBodies.RequireJson(request);
        JsonReaderOptions options = JakarJsonOptions.From(context).ReaderOptions ?? T.DefaultReaderOptions;

        try
        {
            ( bool hasBody, T? value ) = await JakarJsonBodies.ReadAsync(request, options.MaxDocumentBytes, options, static ( in ReadOnlySequence<byte> body, JsonReaderOptions settings ) => JsonCodec.FromJson<T>(in body, settings), context.RequestAborted).ConfigureAwait(false);

            return hasBody
                       ? new JsonBody<T>(value!)
                       : null;
        }
        catch ( JsonReadException exception ) { throw JakarJsonBodies.Malformed(exception); }
    }

    static void IEndpointParameterMetadataProvider.PopulateMetadata( ParameterInfo parameter, EndpointBuilder builder )
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Metadata.Add(new AcceptsMetadata(JakarJsonBodies.JsonContentTypes, typeof(T)));
    }

    public override string? ToString() => Value?.ToString();
}
