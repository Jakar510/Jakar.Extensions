#nullable enable
namespace Jakar.Extensions;


[Obsolete( $"Use {nameof(WebRequester)} instead" )]
public static class WebResponses
{
    public static T VerifyReply<T>( this string reply )
    {
        if ( string.IsNullOrWhiteSpace( reply ) ) { throw new EmptyServerResponseException( nameof(reply) ); }

        return reply.FromJson<T>();
    }
    public static async Task<byte[]> AsBytes( this WebResponse resp, CancellationToken token )
    {
        await using MemoryStream sr = await resp.AsStream( token )
                                                .ConfigureAwait( false );

        return sr.ToArray();
    }


    public static async Task<JToken> AsJson( this WebResponse resp, Encoding encoding ) => await resp.AsJson( encoding,                          JsonNet.LoadSettings, CancellationToken.None );
    public static async Task<JToken> AsJson( this WebResponse resp, Encoding encoding, CancellationToken token ) => await resp.AsJson( encoding, JsonNet.LoadSettings, token );
    public static async Task<JToken> AsJson( this WebResponse resp, Encoding encoding, JsonLoadSettings settings, CancellationToken token )
    {
        await using Stream s      = resp.GetResponseStream() ?? throw new NullReferenceException( nameof(WebResponse.GetResponseStream) );
        using var          sr     = new StreamReader( s, encoding );
        using JsonReader   reader = new JsonTextReader( sr );
        return await JToken.ReadFromAsync( reader, settings, token );
    }

    public static async Task<LocalFile> AsFile( this WebResponse resp, CancellationToken token )
    {
        await using FileStream stream = LocalFile.CreateTempFileAndOpen( out LocalFile file );

        await using MemoryStream sr = await resp.AsStream( token )
                                                .ConfigureAwait( false );

        await sr.CopyToAsync( stream, token );

        return file;
    }

    public static async Task<MemoryStream> AsStream( this WebResponse resp, CancellationToken token )
    {
        await using Stream? stream = resp.GetResponseStream();
        if ( stream is null ) { throw new NullReferenceException( nameof(stream) ); }

        var sr = new MemoryStream();

        await stream.CopyToAsync( sr, token )
                    .ConfigureAwait( false );

        return sr;
    }

    public static async Task<ReadOnlyMemory<byte>> AsMemory( this WebResponse resp, CancellationToken token )
    {
        byte[] bytes = await resp.AsBytes( token )
                                 .ConfigureAwait( false );

        return bytes.AsMemory();
    }


    public static async Task<string> AsString( this WebResponse resp, Encoding encoding )
    {
        await using Stream? stream = resp.GetResponseStream();
        using var           sr     = new StreamReader( stream ?? throw new NullReferenceException( nameof(stream) ), encoding );

        string result = await sr.ReadToEndAsync()
                                .ConfigureAwait( false );

        return result.Trim();
    }


    public static async Task<TResult> AsJson<TResult>( this WebResponse resp, Encoding encoding ) => await resp.AsJson<TResult>( encoding, JsonNet.Serializer );
    public static async Task<TResult> AsJson<TResult>( this WebResponse resp, Encoding encoding, JsonSerializer serializer )
    {
        await using Stream s      = resp.GetResponseStream() ?? throw new NullReferenceException( nameof(WebResponse.GetResponseStream) );
        using var          sr     = new StreamReader( s, encoding );
        using JsonReader   reader = new JsonTextReader( sr );

        return serializer.Deserialize<TResult>( reader ) ?? throw new NullReferenceException( nameof(JsonConvert.DeserializeObject) );
    }
}
