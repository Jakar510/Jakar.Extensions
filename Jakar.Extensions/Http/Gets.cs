namespace Jakar.Extensions.Http;


public static class Gets
{
    /// <summary>
    /// <seealso href="https://www.hanselman.com/blog/HTTPPOSTsAndHTTPGETsWithWebClientAndCAndFakingAPostBack.aspx"/>
    /// </summary>
    /// <param name="url"></param>
    /// <param name="timeout"></param>
    /// <param name="token"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    public static async Task<WebResponse> Get( this Uri url, int timeout, CancellationToken token, HeaderCollection? headers = null )
    {
        HttpWebRequest req = WebRequest.CreateHttp(url);
        req.Timeout = timeout;
        req.Method  = "GET";

        headers ??= new HeaderCollection(MimeType.UrlEncodedContent);

        req.SetHeaders(headers);

        return await req.GetResponseAsync(token).ConfigureAwait(false);
    }


    public static async Task<string> TryGet( this Uri url, int timeout, CancellationToken token, HeaderCollection? headers = null ) =>
        await url.TryGet(WebResponseExtensions.AsString, timeout, headers, token).ConfigureAwait(false);

    public static async Task<TResult> TryGet<TResult>( this Uri                         url,
                                                       Func<WebResponse, Task<TResult>> handler,
                                                       int                              timeout,
                                                       HeaderCollection?                headers = null,
                                                       CancellationToken                token   = default )
    {
        try
        {
            WebResponse reply = await url.Get(timeout, token, headers).ConfigureAwait(false);
            return await handler(reply).ConfigureAwait(false);
        }
        catch ( WebException we )
        {
            Exception? e = we.ConvertException(token);
            if ( e is not null ) { throw e; }

            throw;
        }
    }
}
