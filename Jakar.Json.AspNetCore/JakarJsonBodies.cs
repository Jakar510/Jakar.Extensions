// Jakar.Json.AspNetCore
// 10/14/2026

namespace Jakar.Json.AspNetCore;


/// <summary> Request and response bodies: whole-document reads through the request's <see cref="PipeReader"/>, writes into the response's <see cref="PipeWriter"/>. </summary>
internal static class JakarJsonBodies
{
    public const string JSON_CONTENT_TYPE = "application/json; charset=utf-8";

    public static readonly string[] JsonContentTypes = ["application/json"];


    /// <summary> Parses one buffered body. </summary>
    public delegate TResult Parse<in TState, out TResult>( in ReadOnlySequence<byte> body, TState state );


    /// <summary> Buffers the whole body (at most <paramref name="maxBytes"/>), then parses it; <c>HasBody</c> is <see langword="false"/> for an empty body. </summary>
    /// <exception cref="BadHttpRequestException"> 413: the body is larger than <paramref name="maxBytes"/>. </exception>
    public static async ValueTask<(bool HasBody, TResult? Value)> ReadAsync<TState, TResult>( HttpRequest request, int maxBytes, TState state, Parse<TState, TResult> parse, CancellationToken token )
    {
        PipeReader reader = request.BodyReader;

        while ( true )
        {
            ReadResult             read   = await reader.ReadAsync(token).ConfigureAwait(false);
            ReadOnlySequence<byte> buffer = read.Buffer;

            if ( buffer.Length > maxBytes )
            {
                reader.AdvanceTo(buffer.End);
                throw new BadHttpRequestException($"The JSON request body is larger than {maxBytes} bytes (JsonReaderOptions.MaxDocumentBytes).", StatusCodes.Status413PayloadTooLarge);
            }

            if ( read.IsCanceled ) { throw new OperationCanceledException(token); }

            if ( !read.IsCompleted )
            {
                reader.AdvanceTo(buffer.Start, buffer.End); // nothing consumed; wait for the rest
                continue;
            }

            try
            {
                return buffer.IsEmpty
                           ? ( false, default )
                           : ( true, parse(in buffer, state) );
            }
            finally { reader.AdvanceTo(buffer.End); }
        }
    }


    /// <summary> 415 unless the request says it's JSON (<c>application/json</c>, <c>text/json</c> or <c>application/*+json</c>). </summary>
    public static void RequireJson( HttpRequest request )
    {
        if ( !request.HasJsonContentType() ) { throw new BadHttpRequestException($"Expected a JSON request body, not '{request.ContentType}'.", StatusCodes.Status415UnsupportedMediaType); }
    }

    /// <summary> 400 for malformed JSON, with Jakar.Json's message (position and path). </summary>
    public static BadHttpRequestException Malformed( JsonReadException exception ) => new($"Malformed JSON request body: {exception.Message}", StatusCodes.Status400BadRequest, exception);


    /// <summary> Status, content type, the JSON (written by <paramref name="write"/> into the response's <see cref="PipeWriter"/>), then a flush. </summary>
    public static async Task WriteAsync<TState>( HttpContext context, int? statusCode, string? contentType, TState state, Action<PipeWriter, TState> write )
    {
        HttpResponse response = context.Response;
        if ( statusCode is { } status ) { response.StatusCode = status; }

        response.ContentType = contentType ?? JSON_CONTENT_TYPE;

        write(response.BodyWriter, state);
        await response.BodyWriter.FlushAsync(context.RequestAborted).ConfigureAwait(false);
    }
}
