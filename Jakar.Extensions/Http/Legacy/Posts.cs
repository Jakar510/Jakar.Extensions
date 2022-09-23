#nullable enable
using System.Net.Http;



namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <seealso href = "https://www.hanselman.com/blog/HTTPPOSTsAndHTTPGETsWithWebClientAndCAndFakingAPostBack.aspx" />
///     </para>
///     <para>
///         <seealso href = "https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads" />
///     </para>
///     <para>
///         <seealso href = "https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html" />
///     </para>
/// </summary>
[Obsolete($"Use {nameof(WebRequestBuilder)} instead")]
public static class Posts
{
    public static async Task<WebResponse> Post( this Uri url, ReadOnlyMemory<byte> payload, HeaderCollection headers, CancellationToken token, int? timeout = default )
    {
        // var client = new HttpClient();
        HttpWebRequest req = WebRequest.CreateHttp(url);
        if ( timeout.HasValue ) { req.Timeout = timeout.Value; }

        req.Method = "POST";
        req.SetHeaders(headers);

        //req.Proxy = new WebProxy(ProxyString, true);

        await using ( Stream stream = await req.GetRequestStreamAsync()
                                               .ConfigureAwait(false) )
        {
            await stream.WriteAsync(payload, token)
                        .ConfigureAwait(false); // Push it out there
        }

        return await req.GetResponseAsync(token)
                        .ConfigureAwait(false);
    }


    public static async Task<string> TryPost( this Uri url, string payload, HeaderCollection? headers = default, Encoding? encoding = default, CancellationToken token = default )
    {
        encoding ??= Encoding.Default;
        headers  ??= new HeaderCollection(MimeType.PlainText, encoding);

        return await url.TryPost(WebResponses.AsString,
                                 encoding.GetBytes(payload)
                                         .AsMemory(),
                                 headers,
                                 encoding,
                                 token)
                        .ConfigureAwait(false);
    }


    public static async Task<TResult> TryPost<TResult>( this Uri                                   url,
                                                        Func<WebResponse, Encoding, Task<TResult>> handler,
                                                        string                                     payload,
                                                        MimeType                                   contentType,
                                                        HeaderCollection?                          headers  = default,
                                                        Encoding?                                  encoding = default,
                                                        CancellationToken                          token    = default
    ) => await url.TryPost(handler, payload, contentType.ToContentType(), headers, encoding, token)
                  .ConfigureAwait(false);


    public static async Task<TResult> TryPost<TResult>( this Uri                                   url,
                                                        Func<WebResponse, Encoding, Task<TResult>> handler,
                                                        string                                     payload,
                                                        string                                     contentType,
                                                        HeaderCollection?                          headers  = default,
                                                        Encoding?                                  encoding = default,
                                                        CancellationToken                          token    = default
    )
    {
        encoding ??= Encoding.Default;
        headers  ??= new HeaderCollection(contentType, encoding);

        return await url.TryPost(handler,
                                 encoding.GetBytes(payload)
                                         .AsMemory(),
                                 headers,
                                 encoding,
                                 token)
                        .ConfigureAwait(false);
    }


    public static async Task<TResult> TryPost<TResult>( this Uri url, Func<WebResponse, Encoding, Task<TResult>> handler, ReadOnlyMemory<byte> payload, HeaderCollection headers, Encoding encoding, CancellationToken token = default )
    {
        try
        {
            using WebResponse response = await url.Post(payload, headers, token)
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


    public static async Task<TResult> TryPost<TResult>( this Uri url, Func<WebResponse, Encoding, Task<TResult>> handler, string payload, HeaderCollection headers, Encoding? encoding = default, CancellationToken token = default )
    {
        encoding ??= Encoding.Default;

        try
        {
            using WebResponse response = await url.Post(encoding.GetBytes(payload)
                                                                .AsMemory(),
                                                        headers,
                                                        token)
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


    public static async Task<TResult> TryPost<TResult>( this Uri url, Func<WebResponse, CancellationToken, Task<TResult>> handler, string payload, HeaderCollection headers, Encoding encoding, CancellationToken token = default ) =>
        await url.TryPost(handler,
                          encoding.GetBytes(payload)
                                  .AsMemory(),
                          headers,
                          token)
                 .ConfigureAwait(false);


    public static async Task<TResult> TryPost<TResult>( this Uri url, Func<WebResponse, CancellationToken, Task<TResult>> handler, ReadOnlyMemory<byte> payload, HeaderCollection headers, CancellationToken token = default )
    {
        try
        {
            using WebResponse response = await url.Post(payload, headers, token)
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


    public static async Task<WebResponse> Post( this Uri url, MultipartFormDataContent payload, HeaderCollection? headers, CancellationToken token, int? timeout = default )
    {
        HttpWebRequest req = WebRequest.CreateHttp(url);
        if ( timeout.HasValue ) { req.Timeout = timeout.Value; }

        req.Method = "POST";
        req.SetHeaders(payload);
        req.SetHeaders(headers);

        await using ( Stream stream = await req.GetRequestStreamAsync()
                                               .ConfigureAwait(false) )
        {
            await payload.CopyToAsync(stream)
                         .ConfigureAwait(false); // Push it out there
        }


        return await req.GetResponseAsync(token)
                        .ConfigureAwait(false);
    }

    public static async Task<TResult> TryPost<TResult>( this Uri url, Func<WebResponse, Encoding, Task<TResult>> handler, MultipartFormDataContent payload, Encoding encoding, HeaderCollection? headers = default, CancellationToken token = default )
    {
        try
        {
            using WebResponse response = await url.Post(payload, headers, token)
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

    public static async Task<TResult> TryPost<TResult>( this Uri url, Func<WebResponse, Task<TResult>> handler, MultipartFormDataContent payload, HeaderCollection? headers = default, CancellationToken token = default )
    {
        try
        {
            using WebResponse response = await url.Post(payload, headers, token)
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
