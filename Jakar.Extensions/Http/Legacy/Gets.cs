#nullable enable
namespace Jakar.Extensions;


[Obsolete($"Use {nameof(WebRequester)} instead")]
public static class Gets
{
    /// <summary>
    ///     <seealso href = "https://www.hanselman.com/blog/HTTPPOSTsAndHTTPGETsWithWebClientAndCAndFakingAPostBack.aspx" />
    /// </summary>
    /// <param name = "url" > </param>
    /// <param name = "token" > </param>
    /// <param name = "headers" > </param>
    /// <param name = "timeout" > </param>
    /// <returns> </returns>
    public static async Task<WebResponse> Get( this Uri url, CancellationToken token, HeaderCollection? headers = null, int? timeout = default )
    {
        HttpWebRequest req = WebRequest.CreateHttp(url);
        if ( timeout.HasValue ) { req.Timeout = timeout.Value; }

        req.Method = "GET";

        headers ??= new HeaderCollection(MimeType.UrlEncodedContent);
        req.SetHeaders(headers);

        return await req.GetResponseAsync(token)
                        .ConfigureAwait(false);
    }


    public static async Task<string> TryGet( this Uri url, CancellationToken token, HeaderCollection? headers = null ) => await url.TryGet(Encoding.Default, token, headers)
                                                                                                                                   .ConfigureAwait(false);

    public static async Task<string> TryGet( this Uri url, Encoding encoding, CancellationToken token, HeaderCollection? headers = null ) => await url.TryGet(WebResponses.AsString, encoding, headers, token)
                                                                                                                                                      .ConfigureAwait(false);

    public static async Task<TResult> TryGet<TResult>( this Uri url, Func<WebResponse, Encoding, Task<TResult>> handler, Encoding encoding, HeaderCollection? headers = null, CancellationToken token = default )
    {
        try
        {
            WebResponse reply = await url.Get(token, headers)
                                         .ConfigureAwait(false);

            return await handler(reply, encoding)
                      .ConfigureAwait(false);
        }
        catch ( WebException we )
        {
            Exception? e = we.ConvertException(token);
            if ( e is not null ) { throw e; }

            throw;
        }
    }

    public static async Task<TResult> TryGet<TResult>( this Uri url, Func<WebResponse, CancellationToken, Task<TResult>> handler, HeaderCollection? headers = null, CancellationToken token = default )
    {
        try
        {
            WebResponse reply = await url.Get(token, headers)
                                         .ConfigureAwait(false);

            return await handler(reply, token)
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
