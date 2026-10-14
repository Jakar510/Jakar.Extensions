// Jakar.Json.AspNetCore
// 10/14/2026

namespace Jakar.Json.AspNetCore;


/// <summary>
///     Reads MVC request bodies of <c>[JsonModel]</c> types (and arrays, lists and collection interfaces of them) with Jakar.Json. Other types, and bodies in a charset other than
///     UTF-8, are left to the next formatter (System.Text.Json's).
/// </summary>
public sealed class JakarJsonInputFormatter : TextInputFormatter, IInputFormatterExceptionPolicy
{
    private readonly JakarJsonOptions __options;


    public JakarJsonInputFormatter( JakarJsonOptions options )
    {
        __options = options;
        SupportedEncodings.Add(new UTF8Encoding(false, true));
        SupportedMediaTypes.Add("application/json");
        SupportedMediaTypes.Add("text/json");
        SupportedMediaTypes.Add("application/*+json");
    }


    /// <summary> Malformed JSON becomes a model state error (400 with <c>[ApiController]</c>); other exceptions propagate. </summary>
    public InputFormatterExceptionPolicy ExceptionPolicy => InputFormatterExceptionPolicy.MalformedInputExceptions;


    protected override bool CanReadType( Type type ) => JsonCodecRegistry.IsRegistered(type);

    /// <summary> Also requires UTF-8 (or no charset): Jakar.Json reads UTF-8, and System.Text.Json's formatter transcodes the rest. </summary>
    public override bool CanRead( InputFormatterContext context )
    {
        if ( !base.CanRead(context) ) { return false; }

        return !MediaTypeHeaderValue.TryParse(context.HttpContext.Request.ContentType, out MediaTypeHeaderValue? type) || !type.Charset.HasValue || type.Charset.Equals("utf-8", StringComparison.OrdinalIgnoreCase) || type.Charset.Equals("utf8", StringComparison.OrdinalIgnoreCase);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync( InputFormatterContext context, Encoding encoding )
    {
        if ( !JsonCodecRegistry.TryGet(context.ModelType, out JsonTypeCodec? codec) ) { return await InputFormatterResult.FailureAsync().ConfigureAwait(false); }

        JsonReaderOptions options = __options.ReaderOptions ?? codec.DefaultReaderOptions;

        try
        {
            ( bool hasBody, object? model ) = await JakarJsonBodies.ReadAsync(context.HttpContext.Request,
                                                                             options.MaxDocumentBytes,
                                                                             ( codec, options ),
                                                                             static ( in ReadOnlySequence<byte> body, (JsonTypeCodec Codec, JsonReaderOptions Options) state ) => state.Codec.Read(in body, state.Options),
                                                                             context.HttpContext.RequestAborted).ConfigureAwait(false);

            if ( hasBody ) { return await InputFormatterResult.SuccessAsync(model).ConfigureAwait(false); }

            return context.TreatEmptyInputAsDefaultValue
                       ? await InputFormatterResult.SuccessAsync(GetDefaultValueForType(context.ModelType)).ConfigureAwait(false)
                       : await InputFormatterResult.NoValueAsync().ConfigureAwait(false);
        }
        catch ( JsonReadException exception )
        {
            // Keyed by the JSON path, like System.Text.Json's formatter ("$.lines[0].price"); InputFormatterException's message is shown to clients.
            context.ModelState.TryAddModelError(exception.Path, new InputFormatterException(exception.Message, exception), context.Metadata);
            return await InputFormatterResult.FailureAsync().ConfigureAwait(false);
        }
    }
}



/// <summary> Writes MVC results of <c>[JsonModel]</c> types (and arrays, lists and collection interfaces of them) with Jakar.Json, as UTF-8; other types are left to the next formatter. </summary>
public sealed class JakarJsonOutputFormatter : TextOutputFormatter
{
    private readonly JakarJsonOptions __options;


    public JakarJsonOutputFormatter( JakarJsonOptions options )
    {
        __options = options;
        SupportedEncodings.Add(new UTF8Encoding(false, true));
        SupportedMediaTypes.Add("application/json");
        SupportedMediaTypes.Add("text/json");
        SupportedMediaTypes.Add("application/*+json");
    }


    protected override bool CanWriteType( Type? type ) => type is not null && JsonCodecRegistry.IsRegistered(type);

    public override async Task WriteResponseBodyAsync( OutputFormatterWriteContext context, Encoding selectedEncoding )
    {
        // ObjectType is the declared type when there is one (ActionResult<T>, ObjectResult.DeclaredType), so a derived value is written as its registered base.
        Type type = context.ObjectType ?? context.Object?.GetType() ?? typeof(object);
        if ( !JsonCodecRegistry.TryGet(type, out JsonTypeCodec? codec) ) { throw new InvalidOperationException($"{type} has no Jakar.Json codec."); }

        PipeWriter body = context.HttpContext.Response.BodyWriter;
        codec.Write(context.Object, body, __options.WriterOptions);
        await body.FlushAsync(context.HttpContext.RequestAborted).ConfigureAwait(false);
    }
}



/// <summary> Adds the formatters just before System.Text.Json's, so MVC's own formatters (204 for <see langword="null"/>, strings, streams) still run first. </summary>
internal sealed class JakarJsonMvcOptionsSetup( IOptions<JakarJsonOptions> options ) : IConfigureOptions<MvcOptions>
{
    public void Configure( MvcOptions mvc )
    {
        JakarJsonOptions settings = options.Value;

        if ( !Contains<JakarJsonInputFormatter>(mvc.InputFormatters) ) { mvc.InputFormatters.Insert(IndexOf<SystemTextJsonInputFormatter>(mvc.InputFormatters), new JakarJsonInputFormatter(settings)); }

        if ( !Contains<JakarJsonOutputFormatter>(mvc.OutputFormatters) ) { mvc.OutputFormatters.Insert(IndexOf<SystemTextJsonOutputFormatter>(mvc.OutputFormatters), new JakarJsonOutputFormatter(settings)); }

        if ( !mvc.ModelMetadataDetailsProviders.OfType<JakarJsonBindingMetadataProvider>().Any() ) { mvc.ModelMetadataDetailsProviders.Add(new JakarJsonBindingMetadataProvider()); }
    }


    private static bool Contains<TFormatter>( IList<IInputFormatter> formatters ) => IndexOf<TFormatter>(formatters, -1) >= 0;
    private static bool Contains<TFormatter>( IList<IOutputFormatter> formatters ) => IndexOf<TFormatter>(formatters, -1) >= 0;

    /// <summary> The first <typeparamref name="TFormatter"/>; <paramref name="missing"/> (the front by default) when there's none. </summary>
    private static int IndexOf<TFormatter, TItem>( IList<TItem> formatters, int missing = 0 )
    {
        for ( int i = 0; i < formatters.Count; i++ )
        {
            if ( formatters[i] is TFormatter ) { return i; }
        }

        return missing;
    }

    private static int IndexOf<TFormatter>( IList<IInputFormatter>  formatters, int missing = 0 ) => IndexOf<TFormatter, IInputFormatter>(formatters, missing);
    private static int IndexOf<TFormatter>( IList<IOutputFormatter> formatters, int missing = 0 ) => IndexOf<TFormatter, IOutputFormatter>(formatters, missing);
}



/// <summary>
///     <c>[JsonModel]</c> types implement <see cref="ISpanParsable{TSelf}"/>, so MVC would treat them as simple types and bind them from the route or query string. This makes action
///     parameters of registered types (and their collections) bind from the body instead, unless the parameter says otherwise (<c>[FromQuery]</c>, <c>[FromRoute]</c>, ...).
/// </summary>
internal sealed class JakarJsonBindingMetadataProvider : IBindingMetadataProvider
{
    public void CreateBindingMetadata( BindingMetadataProviderContext context )
    {
        if ( context.Key.MetadataKind == ModelMetadataKind.Parameter && context.BindingMetadata.BindingSource is null && JsonCodecRegistry.IsRegistered(context.Key.ModelType) ) { context.BindingMetadata.BindingSource = BindingSource.Body; }
    }
}
