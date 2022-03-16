namespace Jakar.Extensions.Http;


public static class Pings
{
    /// <summary>
    /// Sends a GET ping to target Uri
    /// </summary>
    /// <param name="url"></param>
    /// <param name="timeout">Defaults to 2.5 seconds</param>
    /// <param name="headers">Defaults to <see cref="MimeType.UrlEncodedContent"/></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async Task<bool> Ping( this Uri url, int timeout = 2500, HeaderCollection? headers = null, CancellationToken token = default )
    {
        try
        {
            string reply = await url.TryGet(timeout, token, headers).ConfigureAwait(false);

            return !string.IsNullOrWhiteSpace(reply);
        }
        catch ( ConnectFailureException ) { return false; }
        catch ( TimeoutException ) { return false; }
    }


    /// <summary>
    /// Sends a POST ping to target Uri
    /// </summary>
    /// <param name="url"></param>
    /// <param name="payload">The data being sent</param>
    /// <param name="timeout">Defaults to 2.5 seconds</param>
    /// <param name="headers">Defaults to <see cref="MimeType.PlainText"/></param>
    /// <param name="token"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static async Task<bool> Ping( this Uri url, string payload, int timeout = 2500, HeaderCollection? headers = null, Encoding? encoding = null, CancellationToken token = default )
    {
        try
        {
            string reply = await url.TryPost(payload,
                                             timeout,
                                             headers,
                                             encoding,
                                             token).ConfigureAwait(false);

            return !string.IsNullOrWhiteSpace(reply);
        }
        catch ( ConnectFailureException ) { return false; }
        catch ( TimeoutException ) { return false; }
    }
}
