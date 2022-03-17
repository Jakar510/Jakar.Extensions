namespace Jakar.Extensions.Http;


public static class WebResponses
{
    public static T VerifyReply<T>( this string reply )
    {
        if ( string.IsNullOrWhiteSpace(reply) ) { throw new EmptyServerResponseException(nameof(reply)); }

        return reply.FromJson<T>();
    }


    public static async Task<Stream?> AsStream( this WebResponse resp ) => await Task.FromResult(resp.GetResponseStream());
    
    public static async Task<TResult> AsJson<TResult>( this WebResponse resp, Encoding encoding )
    {
        string reply = await resp.AsString(encoding);

        return reply.FromJson<TResult>();
    }

    public static async Task<string> AsString( this WebResponse resp, Encoding encoding )
    {
        await using Stream? stream = resp.GetResponseStream();
        using var           sr     = new StreamReader(stream ?? throw new NullReferenceException(nameof(stream)), encoding);
        string              result = await sr.ReadToEndAsync().ConfigureAwait(false);

        return result.Trim();
    }


    public static async Task<byte[]> AsBytes( this WebResponse resp )
    {
        await using Stream? stream = resp.GetResponseStream();
        if ( stream is null ) { throw new NullReferenceException(nameof(stream)); }

        await using var sr = new MemoryStream();
        await stream.CopyToAsync(sr).ConfigureAwait(false);

        return sr.ToArray();
    }

    public static async Task<ReadOnlyMemory<byte>> AsReadyOnlyBytes( this WebResponse resp )
    {
        byte[] bytes = await resp.AsBytes().ConfigureAwait(false);
        return bytes.AsMemory();
    }
}
