using System.Net.Http;



#nullable enable
namespace Jakar.Extensions.Http;


/// <summary>
///     <para>
///         <seealso href = "https://www.hanselman.com/blog/HTTPPOSTsAndHTTPGETsWithWebClientAndCAndFakingADeleteBack.aspx" />
///     </para>
///     <para>
///         <seealso href = "https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads" />
///     </para>
///     <para>
///         <seealso href = "https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html" />
///     </para>
/// </summary>
#if NET6_0
[Obsolete]
#endif
public static class Deletes
{
    public static async Task<WebResponse> Delete( this Uri url, ReadOnlyMemory<byte> payload, HeaderCollection headers, CancellationToken token, int? timeout = default )
    {
        HttpWebRequest req = WebRequest.CreateHttp(url);
        if ( timeout.HasValue ) { req.Timeout = timeout.Value; }

        req.Method = "DELETE";

        req.SetHeaders(headers);

        req.ContentLength = payload.Length;

        await using ( Stream stream = await req.GetRequestStreamAsync().ConfigureAwait(false) )
        {
            await stream.WriteAsync(payload, token).ConfigureAwait(false); //Push it out there
        }

        return await req.GetResponseAsync(token).ConfigureAwait(false);
    }

    public static async Task<string> TryDelete( this Uri url, string payload, HeaderCollection? headers = default, Encoding? encoding = default, CancellationToken token = default )
    {
        encoding ??= Encoding.Default;
        headers  ??= new HeaderCollection(MimeType.PlainText, encoding);

        return await url.TryDelete(WebResponses.AsString, encoding.GetBytes(payload).AsMemory(), headers, encoding, token).ConfigureAwait(false);
    }


    public static async Task<TResult> TryDelete<TResult>( this Uri url, Func<WebResponse, Encoding, Task<TResult>> handler, string payload, MimeType contentType, Encoding encoding, CancellationToken token = default ) =>
        await url.TryDelete(handler, payload, contentType.ToContentType(), encoding, token).ConfigureAwait(false);


    public static async Task<TResult> TryDelete<TResult>( this Uri url, Func<WebResponse, Encoding, Task<TResult>> handler, string payload, string contentType, Encoding encoding, CancellationToken token = default ) =>
        await url.TryDelete(handler, payload, new HeaderCollection(contentType, encoding), encoding, token).ConfigureAwait(false);


    public static async Task<TResult> TryDelete<TResult>( this Uri url, Func<WebResponse, Encoding, Task<TResult>> handler, string payload, HeaderCollection headers, Encoding encoding, CancellationToken token = default )
    {
        try
        {
            using WebResponse response = await url.Delete(encoding.GetBytes(payload).AsMemory(), headers, token).ConfigureAwait(false);

            return await handler(response, encoding).ConfigureAwait(false);
        }
        catch ( WebException we )
        {
            Exception? e = we.ConvertException(token);
            if ( e is not null ) { throw e; }

            throw;
        }
    }


    public static async Task<TResult> TryDelete<TResult>( this Uri url, Func<WebResponse, Encoding, Task<TResult>> handler, ReadOnlyMemory<byte> payload, HeaderCollection headers, Encoding encoding, CancellationToken token = default )
    {
        try
        {
            using WebResponse response = await url.Delete(payload, headers, token).ConfigureAwait(false);

            return await handler(response, encoding).ConfigureAwait(false);
        }
        catch ( WebException we )
        {
            Exception? e = we.ConvertException(token);
            if ( e is not null ) { throw e; }

            throw;
        }
    }


    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    public static async Task<TResult> TryDelete<TResult>( this Uri url, Func<WebResponse, CancellationToken, Task<TResult>> handler, string payload, HeaderCollection headers, Encoding encoding, CancellationToken token = default ) =>
        await url.TryDelete(handler, encoding.GetBytes(payload).AsMemory(), headers, token);


    public static async Task<TResult> TryDelete<TResult>( this Uri url, Func<WebResponse, CancellationToken, Task<TResult>> handler, ReadOnlyMemory<byte> payload, HeaderCollection headers, CancellationToken token = default )
    {
        try
        {
            using WebResponse response = await url.Delete(payload, headers, token).ConfigureAwait(false);

            return await handler(response, token).ConfigureAwait(false);
        }
        catch ( WebException we )
        {
            Exception? e = we.ConvertException(token);
            if ( e is not null ) { throw e; }

            throw;
        }
    }


    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    public static async Task<WebResponse> Delete( this Uri url, MultipartFormDataContent payload, HeaderCollection? headers, CancellationToken token, int? timeout = default )
    {
        HttpWebRequest req = WebRequest.CreateHttp(url);
        if ( timeout.HasValue ) { req.Timeout = timeout.Value; }

        req.Method = "DELETE";

        req.SetHeaders(payload);
        req.SetHeaders(headers);

        await using ( Stream stream = await req.GetRequestStreamAsync().ConfigureAwait(false) )
        {
            await payload.CopyToAsync(stream).ConfigureAwait(false); // Push it out there
        }


        return await req.GetResponseAsync(token).ConfigureAwait(false);
    }


    public static async Task<TResult> TryDelete<TResult>( this Uri url, Func<WebResponse, Encoding, Task<TResult>> handler, MultipartFormDataContent payload, Encoding encoding, HeaderCollection? headers = default, CancellationToken token = default )
    {
        try
        {
            using WebResponse response = await url.Delete(payload, headers, token).ConfigureAwait(false);

            return await handler(response, encoding).ConfigureAwait(false);
        }
        catch ( WebException we )
        {
            Exception? e = we.ConvertException(token);
            if ( e is not null ) { throw e; }

            throw;
        }
    }

    public static async Task<TResult> TryDelete<TResult>( this Uri url, Func<WebResponse, Task<TResult>> handler, MultipartFormDataContent payload, HeaderCollection? headers = default, CancellationToken token = default )
    {
        try
        {
            using WebResponse response = await url.Delete(payload, headers, token).ConfigureAwait(false);

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
