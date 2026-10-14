// Jakar.Json.AspNetCore
// 10/14/2026

namespace Jakar.Json.AspNetCore;


/// <summary> Minimal API results written by Jakar.Json, like <c>TypedResults.Json</c> / <c>Ok</c> / <c>Created</c>. They describe themselves to OpenAPI (status code, type, <c>application/json</c>). </summary>
public static class JakarResults
{
    /// <summary> <paramref name="value"/> as JSON. </summary>
    /// <param name="value"> The response body. </param>
    /// <param name="statusCode"> The status code; <see langword="null"/> leaves the response's (200 by default). </param>
    /// <param name="contentType"> <see langword="null"/>: <c>application/json; charset=utf-8</c>. </param>
    /// <param name="options"> Writer settings; <see langword="null"/>: <see cref="JakarJsonOptions.WriterOptions"/>, then the type's own. </param>
    public static JakarJsonHttpResult<T> Json<T>( T value, int? statusCode = null, string? contentType = null, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T> => new(value, statusCode, contentType, options, null);

    /// <summary> 200 with <paramref name="value"/>. </summary>
    public static JakarJsonHttpResult<T> Ok<T>( T value, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T> => new(value, StatusCodes.Status200OK, null, options, null);

    /// <summary> 201 with <paramref name="value"/> and a <c>Location</c> header (when <paramref name="location"/> isn't <see langword="null"/>). </summary>
    public static JakarJsonHttpResult<T> Created<T>( string? location, T value, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T> => new(value, StatusCodes.Status201Created, null, options, location);

    /// <summary> A JSON array of <paramref name="values"/> (any <see cref="IEnumerable{T}"/>: arrays and lists are written without enumerators). </summary>
    public static JakarJsonArrayHttpResult<T> JsonArray<T>( IEnumerable<T> values, int? statusCode = null, string? contentType = null, JsonWriterOptions? options = null )
        where T : IJsonSerializable<T> => new(values, statusCode, contentType, options);
}



/// <summary> One <typeparamref name="T"/> as JSON (see <see cref="JakarResults"/>). </summary>
public sealed class JakarJsonHttpResult<T> : IResult, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<T>, IContentTypeHttpResult, IEndpointMetadataProvider
    where T : IJsonSerializable<T>
{
    private readonly JsonWriterOptions? __options;


    internal JakarJsonHttpResult( T value, int? statusCode, string? contentType, JsonWriterOptions? options, string? location )
    {
        Value       = value;
        StatusCode  = statusCode;
        ContentType = contentType ?? JakarJsonBodies.JSON_CONTENT_TYPE;
        Location    = location;
        __options   = options;
    }


    public T       Value       { get; }
    public int?    StatusCode  { get; }
    public string  ContentType { get; }
    public string? Location    { get; }

    object? IValueHttpResult.Value => Value;


    public Task ExecuteAsync( HttpContext context )
    {
        ArgumentNullException.ThrowIfNull(context);
        if ( Location is not null ) { context.Response.Headers.Location = Location; }

        JsonWriterOptions? options = __options ?? JakarJsonOptions.From(context).WriterOptions;

        return JakarJsonBodies.WriteAsync(context,
                                          StatusCode,
                                          ContentType,
                                          ( Value, options ),
                                          static ( body, state ) =>
                                          {
                                              if ( state.Value is null ) { body.Write("null"u8); }
                                              else { JsonCodec.WriteJson(in state.Value, body, state.options); }
                                          });
    }

    static void IEndpointMetadataProvider.PopulateMetadata( MethodInfo method, EndpointBuilder builder )
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status200OK, typeof(T), JakarJsonBodies.JsonContentTypes));
    }
}



/// <summary> A JSON array of <typeparamref name="T"/> (see <see cref="JakarResults.JsonArray{T}"/>). </summary>
public sealed class JakarJsonArrayHttpResult<T> : IResult, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<IEnumerable<T>>, IContentTypeHttpResult, IEndpointMetadataProvider
    where T : IJsonSerializable<T>
{
    private readonly JsonWriterOptions? __options;


    internal JakarJsonArrayHttpResult( IEnumerable<T> values, int? statusCode, string? contentType, JsonWriterOptions? options )
    {
        ArgumentNullException.ThrowIfNull(values);
        Value       = values;
        StatusCode  = statusCode;
        ContentType = contentType ?? JakarJsonBodies.JSON_CONTENT_TYPE;
        __options   = options;
    }


    public IEnumerable<T> Value       { get; }
    public int?           StatusCode  { get; }
    public string         ContentType { get; }

    object IValueHttpResult.Value => Value;


    public Task ExecuteAsync( HttpContext context )
    {
        ArgumentNullException.ThrowIfNull(context);
        JsonWriterOptions? options = __options ?? JakarJsonOptions.From(context).WriterOptions;

        return JakarJsonBodies.WriteAsync(context,
                                          StatusCode,
                                          ContentType,
                                          ( Value, options ),
                                          static ( body, state ) =>
                                          {
                                              switch ( state.Value )
                                              {
                                                  case T[] array:
                                                      JsonCodec.WriteJsonArray(array.AsSpan(), body, state.options);
                                                      break;

                                                  case List<T> list:
                                                      JsonCodec.WriteJsonArray((ReadOnlySpan<T>)System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list), body, state.options);
                                                      break;

                                                  default:
                                                      JsonCodec.WriteJsonArray(state.Value, body, state.options);
                                                      break;
                                              }
                                          });
    }

    static void IEndpointMetadataProvider.PopulateMetadata( MethodInfo method, EndpointBuilder builder )
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status200OK, typeof(IEnumerable<T>), JakarJsonBodies.JsonContentTypes));
    }
}



/// <summary> A value the endpoint filter found a codec for (<see cref="JakarJsonEndpointExtensions.WithJakarJson{TBuilder}"/>). </summary>
internal sealed class JakarJsonObjectResult( JsonTypeCodec codec, object value ) : IResult, IStatusCodeHttpResult, IValueHttpResult, IContentTypeHttpResult
{
    public int?    StatusCode  => StatusCodes.Status200OK;
    public object  Value       => value;
    public string  ContentType => JakarJsonBodies.JSON_CONTENT_TYPE;


    public Task ExecuteAsync( HttpContext context ) => JakarJsonBodies.WriteAsync(context,
                                                                                  StatusCodes.Status200OK,
                                                                                  JakarJsonBodies.JSON_CONTENT_TYPE,
                                                                                  ( Codec: codec, Value: value, Options: JakarJsonOptions.From(context).WriterOptions ),
                                                                                  static ( body, state ) => state.Codec.Write(state.Value, body, state.Options));
}
