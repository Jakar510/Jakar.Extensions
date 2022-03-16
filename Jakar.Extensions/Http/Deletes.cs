using System.Net.Http;


namespace Jakar.Extensions.Http;


/// <summary>
/// <para><seealso href="https://www.hanselman.com/blog/HTTPPOSTsAndHTTPGETsWithWebClientAndCAndFakingADeleteBack.aspx"/></para>
/// <para><seealso href="https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads"/></para>
/// <para><seealso href="https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html"/></para>
/// </summary>
public static class Deletes
{
    public static async Task<WebResponse> Delete( this Uri             url,
                                                  ReadOnlyMemory<byte> payload,
                                                  HeaderCollection     headers,
                                                  int                  timeout,
                                                  CancellationToken    token )
    {
        HttpWebRequest req = WebRequest.CreateHttp(url); //req.Proxy = new WebProxy(ProxyString, true);
        req.Timeout = timeout;
        req.Method  = "DELETE";

        req.SetHeaders(headers);

        req.ContentLength = payload.Length;

        await using ( Stream stream = await req.GetRequestStreamAsync().ConfigureAwait(false) )
        {
            await stream.WriteAsync(payload, token).ConfigureAwait(false); //Push it out there
        }

        return await req.GetResponseAsync(token).ConfigureAwait(false);
    }


    public static async Task<WebResponse> Delete( this Uri                 url,
                                                  MultipartFormDataContent payload,
                                                  int                      timeout,
                                                  CancellationToken        token )
    {
        HttpWebRequest req = WebRequest.CreateHttp(url); //req.Proxy = new WebProxy(ProxyString, true);
        req.Timeout = timeout;
        req.Method  = "DELETE";

        req.SetHeaders(payload);

        await using ( Stream stream = await req.GetRequestStreamAsync().ConfigureAwait(false) )
        {
            await payload.CopyToAsync(stream).ConfigureAwait(false); // Push it out there
        }


        return await req.GetResponseAsync(token).ConfigureAwait(false);
    }


    public static async Task<string> TryDelete( this Uri          url,
                                                string            payload,
                                                int               timeout,
                                                HeaderCollection? headers  = default,
                                                Encoding?         encoding = default,
                                                CancellationToken token    = default )
    {
        encoding ??= Encoding.Default;
        headers  ??= new HeaderCollection(MimeType.PlainText, encoding);

        return await url.TryDelete(WebResponseExtensions.AsString,
                                   encoding.GetBytes(payload).AsMemory(),
                                   timeout,
                                   headers,
                                   token).ConfigureAwait(false);
    }


    public static async Task<TResult> TryDelete<TResult>( this Uri                         url,
                                                          Func<WebResponse, Task<TResult>> handler,
                                                          string                           payload,
                                                          MimeType                         contentType,
                                                          int                              timeout,
                                                          HeaderCollection?                headers  = default,
                                                          Encoding?                        encoding = default,
                                                          CancellationToken                token    = default ) => await url.TryDelete(handler,
                                                                                                                                       payload,
                                                                                                                                       contentType.ToContentType(),
                                                                                                                                       timeout,
                                                                                                                                       headers,
                                                                                                                                       encoding,
                                                                                                                                       token).ConfigureAwait(false);


    public static async Task<TResult> TryDelete<TResult>( this Uri                         url,
                                                          Func<WebResponse, Task<TResult>> handler,
                                                          string                           payload,
                                                          string                           contentType,
                                                          int                              timeout,
                                                          HeaderCollection?                headers  = default,
                                                          Encoding?                        encoding = default,
                                                          CancellationToken                token    = default )
    {
        encoding ??= Encoding.Default;
        headers  ??= new HeaderCollection(contentType, encoding);

        return await url.TryDelete(handler,
                                   encoding.GetBytes(payload).AsMemory(),
                                   timeout,
                                   headers,
                                   token).ConfigureAwait(false);
    }


    public static async Task<TResult> TryDelete<TResult>( this Uri                         url,
                                                          Func<WebResponse, Task<TResult>> handler,
                                                          ReadOnlyMemory<byte>             payload,
                                                          int                              timeout,
                                                          HeaderCollection                 headers,
                                                          CancellationToken                token = default )
    {
        try
        {
            using WebResponse response = await url.Delete(payload,
                                                          headers,
                                                          timeout,
                                                          token).ConfigureAwait(false);

            return await handler(response).ConfigureAwait(false);
        }
        catch ( WebException we )
        {
            Exception? e = we.ConvertException(token);
            if ( e is not null ) { throw e; }

            throw;
        }
    }


    public static async Task<TResult> TryDelete<TResult, TPayload>( this Uri                             url,
                                                                    Func<WebResponse, Task<TResult>>     handler,
                                                                    Func<TPayload, ReadOnlyMemory<byte>> serializer,
                                                                    TPayload                             payload,
                                                                    int                                  timeout,
                                                                    HeaderCollection                     headers,
                                                                    CancellationToken                    token = default ) => await url.TryDelete(handler,
                                                                                                                                                  serializer(payload),
                                                                                                                                                  timeout,
                                                                                                                                                  headers,
                                                                                                                                                  token).ConfigureAwait(false);


    public static async Task<TResult> TryDelete<TResult>( this Uri                         url,
                                                          Func<WebResponse, Task<TResult>> handler,
                                                          MultipartFormDataContent         payload,
                                                          int                              timeout,
                                                          CancellationToken                token = default )
    {
        try
        {
            using WebResponse response = await url.Delete(payload,
                                                          timeout,
                                                          token).ConfigureAwait(false);

            return await handler(response).ConfigureAwait(false);
        }
        catch ( WebException we )
        {
            Exception? e = we.ConvertException(token);
            if ( e is not null ) { throw e; }

            throw;
        }
    }
}
