// Jakar.Json.AspNetCore
// 10/14/2026

using Microsoft.Extensions.DependencyInjection.Extensions;



namespace Jakar.Json.AspNetCore;


/// <summary> Registers Jakar.Json with MVC (controllers): <c>builder.Services.AddControllers().AddJakarJson()</c>. </summary>
public static class JakarJsonMvcExtensions
{
    /// <summary>
    ///     Reads and writes <c>[JsonModel]</c> types (and arrays, lists and collection interfaces of them) with Jakar.Json; every other type keeps System.Text.Json. The formatters go just
    ///     before System.Text.Json's.
    /// </summary>
    public static IMvcBuilder AddJakarJson( this IMvcBuilder builder, Action<JakarJsonOptions>? configure = null )
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.AddJakarJson(configure);
        builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, JakarJsonMvcOptionsSetup>());
        return builder;
    }

    /// <inheritdoc cref="AddJakarJson(IMvcBuilder, Action{JakarJsonOptions}?)"/>
    public static IMvcCoreBuilder AddJakarJson( this IMvcCoreBuilder builder, Action<JakarJsonOptions>? configure = null )
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.AddJakarJson(configure);
        builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, JakarJsonMvcOptionsSetup>());
        return builder;
    }

    /// <summary> <see cref="JakarJsonOptions"/> for minimal API results and <see cref="JsonBody{T}"/> (MVC's <c>AddJakarJson</c> calls this too). </summary>
    public static IServiceCollection AddJakarJson( this IServiceCollection services, Action<JakarJsonOptions>? configure = null )
    {
        ArgumentNullException.ThrowIfNull(services);
        OptionsBuilder<JakarJsonOptions> options = services.AddOptions<JakarJsonOptions>();
        if ( configure is not null ) { options.Configure(configure); }

        return services;
    }
}



/// <summary> Minimal APIs: Jakar.Json for plain return values. </summary>
public static class JakarJsonEndpointExtensions
{
    /// <summary>
    ///     Writes values that endpoints return directly (not an <see cref="IResult"/>) with Jakar.Json when their runtime type has a codec: a <c>[JsonModel]</c> type, or an array, list or
    ///     collection interface of one. Others are left to minimal APIs (System.Text.Json). Apply it to a group to cover every endpoint in it: <c>app.MapGroup("/api").WithJakarJson()</c>.
    /// </summary>
    /// <remarks> The handler's declared return type still describes the response to OpenAPI. Request bodies: see <see cref="JsonBody{T}"/>. </remarks>
    public static TBuilder WithJakarJson<TBuilder>( this TBuilder builder )
        where TBuilder : IEndpointConventionBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddEndpointFilter(static async ( context, next ) =>
                                         {
                                             object? result = await next(context).ConfigureAwait(false);

                                             return result is not null and not IResult and not string && JsonCodecRegistry.TryGet(result.GetType(), out JsonTypeCodec? codec)
                                                        ? new JakarJsonObjectResult(codec, result)
                                                        : result;
                                         });
    }
}



/// <summary> <see cref="HttpRequest"/> / <see cref="HttpResponse"/> helpers, like <c>ReadFromJsonAsync</c> / <c>WriteAsJsonAsync</c>. </summary>
public static class JakarJsonHttpExtensions
{
    /// <summary> Reads the body as a <typeparamref name="T"/>; <see langword="default"/> for an empty body. </summary>
    /// <exception cref="BadHttpRequestException"> 415 if the request isn't JSON, 413 if it's too large, 400 if it's malformed. </exception>
    public static async ValueTask<T?> ReadFromJakarJsonAsync<T>( this HttpRequest request, JsonReaderOptions? options = null, CancellationToken token = default )
        where T : IJsonSerializable<T>
    {
        ArgumentNullException.ThrowIfNull(request);
        JakarJsonBodies.RequireJson(request);
        JsonReaderOptions settings = options ?? JakarJsonOptions.From(request.HttpContext).ReaderOptions ?? T.DefaultReaderOptions;

        try
        {
            ( bool hasBody, T? value ) = await JakarJsonBodies.ReadAsync(request, settings.MaxDocumentBytes, settings, static ( in ReadOnlySequence<byte> body, JsonReaderOptions state ) => JsonCodec.FromJson<T>(in body, state), token).ConfigureAwait(false);

            return hasBody
                       ? value
                       : default;
        }
        catch ( JsonReadException exception ) { throw JakarJsonBodies.Malformed(exception); }
    }

    /// <summary> Writes <paramref name="value"/> as the response body (<c>application/json; charset=utf-8</c> unless <paramref name="contentType"/> says otherwise). </summary>
    public static Task WriteAsJakarJsonAsync<T>( this HttpResponse response, T value, JsonWriterOptions? options = null, string? contentType = null )
        where T : IJsonSerializable<T>
    {
        ArgumentNullException.ThrowIfNull(response);
        return new JakarJsonHttpResult<T>(value, null, contentType, options, null).ExecuteAsync(response.HttpContext);
    }
}
