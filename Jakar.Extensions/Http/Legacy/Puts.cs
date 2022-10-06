#nullable enable
namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <seealso href = "https://www.hanselman.com/blog/HTTPPOSTsAndHTTPGETsWithWebClientAndCAndFakingAPutBack.aspx" />
///     </para>
///     <para>
///         <seealso href = "https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads" />
///     </para>
///     <para>
///         <seealso href = "https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html" />
///     </para>
/// </summary>
[Obsolete($"Use {nameof(WebRequester)} instead")]
public static class Puts
{
    public static async Task<WebResponse> Put( this Uri url, ReadOnlyMemory<byte> payload, HeaderCollection headers, CancellationToken token, int? timeout = default )
    {
        HttpWebRequest req = WebRequest.CreateHttp(url);
        if ( timeout.HasValue ) { req.Timeout = timeout.Value; }

        req.Method = "PUT";

        req.SetHeaders(headers);

        req.ContentLength = payload.Length;

        await using ( Stream os = await req.GetRequestStreamAsync()
                                           .ConfigureAwait(false) )
        {
            await os.WriteAsync(payload, token)
                    .ConfigureAwait(false); //Push it out there
        }

        return await req.GetResponseAsync(token)
                        .ConfigureAwait(false);
    }

    public static async Task<string> TryPut( this Uri url, string payload, Encoding? encoding = default, CancellationToken token = default )
    {
        encoding ??= Encoding.Default;
        var headers = new HeaderCollection(MimeType.PlainText, encoding);

        return await url.TryPut(WebResponses.AsString,
                                encoding.GetBytes(payload)
                                        .AsMemory(),
                                headers,
                                encoding,
                                token)
                        .ConfigureAwait(false);
    }


    public static async Task<TResult> TryPut<TResult>( this Uri url, Func<WebResponse, Encoding, Task<TResult>> handler, string payload, MimeType contentType, Encoding? encoding = default, CancellationToken token = default ) =>
        await url.TryPut(handler, payload, contentType.ToContentType(), encoding, token)
                 .ConfigureAwait(false);


    public static async Task<TResult> TryPut<TResult>( this Uri url, Func<WebResponse, Encoding, Task<TResult>> handler, string payload, string contentType, Encoding? encoding = default, CancellationToken token = default )
    {
        encoding ??= Encoding.Default;
        var headers = new HeaderCollection(contentType, encoding);

        return await url.TryPut(handler,
                                encoding.GetBytes(payload)
                                        .AsMemory(),
                                headers,
                                encoding,
                                token)
                        .ConfigureAwait(false);
    }

    public static async Task<TResult> TryPut<TResult>( this Uri url, Func<WebResponse, Encoding, Task<TResult>> handler, string payload, HeaderCollection headers, Encoding? encoding, CancellationToken token = default )
    {
        encoding ??= Encoding.Default;

        return await url.TryPut(handler,
                                encoding.GetBytes(payload)
                                        .AsMemory(),
                                headers,
                                encoding,
                                token)
                        .ConfigureAwait(false);
    }


    public static async Task<TResult> TryPut<TResult>( this Uri url, Func<WebResponse, Encoding, Task<TResult>> handler, ReadOnlyMemory<byte> payload, HeaderCollection headers, Encoding encoding, CancellationToken token = default )
    {
        try
        {
            using WebResponse response = await url.Put(payload, headers, token)
                                                  .ConfigureAwait(false);

            return await handler(response, encoding)
                      .ConfigureAwait(false);
        }
        catch ( WebException we )
        {
            Exception? e = we.ConvertException(token);
            if ( e is not null ) { throw e; }

            throw;
        }
    }


    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    public static async Task<TResult> TryPut<TResult>( this Uri url, Func<WebResponse, CancellationToken, Task<TResult>> handler, string payload, HeaderCollection headers, Encoding encoding, CancellationToken token = default ) =>
        await url.TryPut(handler,
                         encoding.GetBytes(payload)
                                 .AsMemory(),
                         headers,
                         token)
                 .ConfigureAwait(false);


    public static async Task<TResult> TryPut<TResult>( this Uri url, Func<WebResponse, CancellationToken, Task<TResult>> handler, ReadOnlyMemory<byte> payload, HeaderCollection headers, CancellationToken token = default )
    {
        try
        {
            using WebResponse response = await url.Put(payload, headers, token)
                                                  .ConfigureAwait(false);

            return await handler(response, token)
                      .ConfigureAwait(false);
        }
        catch ( WebException we )
        {
            Exception? e = we.ConvertException(token);
            if ( e is not null ) { throw e; }

            throw;
        }
    }


    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    public static async Task<WebResponse> Put( this Uri url, MultipartFormDataContent payload, HeaderCollection? headers, CancellationToken token, int? timeout = default )
    {
        HttpWebRequest req = WebRequest.CreateHttp(url);
        if ( timeout.HasValue ) { req.Timeout = timeout.Value; }

        req.Method = "PUT";

        req.SetHeaders(payload);
        req.SetHeaders(headers);

        await using ( Stream os = await req.GetRequestStreamAsync()
                                           .ConfigureAwait(false) )
        {
            await payload.CopyToAsync(os)
                         .ConfigureAwait(false); // Push it out there
        }


        return await req.GetResponseAsync(token)
                        .ConfigureAwait(false);
    }


    public static async Task<TResult> TryPut<TResult>( this Uri url, Func<WebResponse, Encoding, Task<TResult>> handler, MultipartFormDataContent payload, Encoding encoding, HeaderCollection? headers = default, CancellationToken token = default )
    {
        try
        {
            using WebResponse response = await url.Put(payload, headers, token)
                                                  .ConfigureAwait(false);

            return await handler(response, encoding)
                      .ConfigureAwait(false);
        }
        catch ( WebException we )
        {
            Exception? e = we.ConvertException(token);
            if ( e is not null ) { throw e; }

            throw;
        }
    }

    public static async Task<TResult> TryPut<TResult>( this Uri url, Func<WebResponse, Task<TResult>> handler, MultipartFormDataContent payload, HeaderCollection? headers = default, CancellationToken token = default )
    {
        try
        {
            using WebResponse response = await url.Put(payload, headers, token)
                                                  .ConfigureAwait(false);

            return await handler(response)
                      .ConfigureAwait(false);
        }
        catch ( WebException we )
        {
            Exception? e = we.ConvertException(token);
            if ( e is not null ) { throw e; }

            throw;
        }
    }
}
