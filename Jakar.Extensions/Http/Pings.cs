#nullable enable
namespace Jakar.Extensions.Http;


#if NET6_0
[Obsolete]
#endif
public static class Pings
{
    /// <summary>
    ///     Sends a GET ping to target Uri
    /// </summary>
    /// <param name = "url" > </param>
    /// <param name = "timeout" > Defaults to 2.5 seconds </param>
    /// <param name = "headers" >
    ///     Defaults to
    ///     <see cref = "MimeType.UrlEncodedContent" />
    /// </param>
    /// <param name = "encoding" > </param>
    /// <param name = "token" > </param>
    /// <returns> </returns>
    public static async Task<bool> Ping( this Uri url, int timeout = 2500, HeaderCollection? headers = null, Encoding? encoding = null, CancellationToken token = default )
    {
        try
        {
            encoding ??= Encoding.Default;
            headers  ??= new HeaderCollection(MimeTypeNames.Application.URL_ENCODED_CONTENT, encoding);

            try
            {
                using WebResponse response = await url.Get(token, headers, timeout).ConfigureAwait(false);

                string? reply = await response.AsString(encoding).ConfigureAwait(false);
                return !string.IsNullOrWhiteSpace(reply);
            }
            catch ( WebException we )
            {
                Exception? e = we.ConvertException(token);
                if ( e is not null ) { throw e; }

                throw;
            }
        }
        catch ( ConnectFailureException ) { return false; }
        catch ( TimeoutException ) { return false; }
    }


    /// <summary>
    ///     Sends a POST ping to target Uri
    /// </summary>
    /// <param name = "url" > </param>
    /// <param name = "payload" > The data being sent </param>
    /// <param name = "timeout" > Defaults to 2.5 seconds </param>
    /// <param name = "headers" >
    ///     Defaults to
    ///     <see cref = "MimeType.PlainText" />
    /// </param>
    /// <param name = "token" > </param>
    /// <param name = "encoding" > </param>
    /// <returns> </returns>
    public static async Task<bool> Ping( this Uri url, string payload, int timeout = 2500, HeaderCollection? headers = null, Encoding? encoding = null, CancellationToken token = default )
    {
        try
        {
            encoding ??= Encoding.Default;
            headers  ??= new HeaderCollection(MimeTypeNames.Text.PLAIN, encoding);

            try
            {
                using WebResponse response = await url.Post(encoding.GetBytes(payload).AsMemory(), headers, token, timeout).ConfigureAwait(false);

                string? reply = await response.AsString(encoding).ConfigureAwait(false);
                return !string.IsNullOrWhiteSpace(reply);
            }
            catch ( WebException we )
            {
                Exception? e = we.ConvertException(token);
                if ( e is not null ) { throw e; }

                throw;
            }
        }
        catch ( ConnectFailureException ) { return false; }
        catch ( TimeoutException ) { return false; }
    }
}
