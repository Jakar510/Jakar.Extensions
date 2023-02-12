﻿// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:36 AM

using System;
using static Jakar.Extensions.WebRequester;



namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class WebHandler : IDisposable
{
    private readonly HttpClient         _client;
    private readonly HttpRequestMessage _request;
    public AppVersion Version
    {
        get => _request.Version;
        set => _request.Version = value.ToVersion();
    }
    protected internal CancellationToken   Token          { get; }
    protected internal Encoding            Encoding       { get; }
    public             HttpContentHeaders? ContentHeaders => _request.Content?.Headers;
    public             HttpRequestHeaders  Headers        => _request.Headers;
    protected internal RetryPolicy         RetryPolicy    { get; private set; }
    public             string              Method         => _request.Method.Method;


    public WebHandler( HttpClient client, HttpRequestMessage request, Encoding encoding, in RetryPolicy retryPolicy, in CancellationToken token )
    {
        _request    = request;
        _client     = client;
        Encoding    = encoding;
        Token       = token;
        RetryPolicy = retryPolicy;
    }
    public const string FILE_NAME = "FileName";


    public TaskAwaiter<HttpResponseMessage> GetAwaiter() => _client.SendAsync( _request, Token )
                                                                   .GetAwaiter();


    public virtual async ValueTask NoResponse()
    {
        using ( this )
        {
            using HttpResponseMessage response = await this;
            response.EnsureSuccessStatusCode();
        }
    }
    public virtual async ValueTask<bool> AsBool( HttpResponseMessage response )
    {
        string content = await AsString( response );
        return bool.TryParse( content, out bool result ) && result;
    }
    public virtual async ValueTask<Guid?> AsGuid( HttpResponseMessage response )
    {
        string content = await AsString( response );

        return Guid.TryParse( content, out Guid result )
                   ? result
                   : default;
    }
    public virtual async ValueTask<byte[]> AsBytes( HttpResponseMessage response )
    {
        await using MemoryStream stream = await AsStream( response );
        return stream.ToArray();
    }
    public virtual async ValueTask<JToken> AsJson( HttpResponseMessage response, JsonLoadSettings settings )
    {
        await using MemoryStream stream = await AsStream( response );
        using var                sr     = new StreamReader( stream, Encoding );
        using JsonReader         reader = new JsonTextReader( sr );
        return await JToken.ReadFromAsync( reader, settings, Token );
    }
    public virtual async ValueTask<LocalFile> AsFile( HttpResponseMessage response )
    {
        await using MemoryStream stream = await AsStream( response );
        await using FileStream   fs     = LocalFile.CreateTempFileAndOpen( out LocalFile file );
        await stream.CopyToAsync( fs, Token );

        return file;
    }
    public virtual async ValueTask<LocalFile> AsFile( HttpResponseMessage response, string fileNameHeader )
    {
        if ( response.Headers.Contains( fileNameHeader ) )
        {
            var mimeType = response.Headers.GetValues( fileNameHeader )
                                   .First()
                                   .ToMimeType();

            return await AsFile( response, mimeType );
        }


        if ( response.Content.Headers.Contains( fileNameHeader ) )
        {
            var mimeType = response.Content.Headers.GetValues( fileNameHeader )
                                   .First()
                                   .ToMimeType();

            return await AsFile( response, mimeType );
        }


        await using Stream     stream = await AsStream( response );
        await using FileStream fs     = LocalFile.CreateTempFileAndOpen( out LocalFile file );
        await stream.CopyToAsync( fs, Token );

        return file;
    }
    public virtual async ValueTask<LocalFile> AsFile( HttpResponseMessage response, FileInfo path )
    {
        var file = new LocalFile( path );
        return await AsFile( response, file );
    }
    public virtual async ValueTask<LocalFile> AsFile( HttpResponseMessage response, LocalFile file )
    {
        await using MemoryStream stream = await AsStream( response );
        await file.WriteAsync( stream, Token );
        return file;
    }
    public virtual async ValueTask<LocalFile> AsFile( HttpResponseMessage response, MimeType type )
    {
        await using MemoryStream stream = await AsStream( response );
        await using FileStream   fs     = LocalFile.CreateTempFileAndOpen( type, out LocalFile file );
        await stream.CopyToAsync( fs, Token );
        return file;
    }
    public virtual async ValueTask<MemoryStream> AsStream( HttpResponseMessage response )
    {
        response.EnsureSuccessStatusCode();
        using HttpContent content = response.Content;

    #if NETSTANDARD2_1
        await using Stream stream = await content.ReadAsStreamAsync();
    #else
        await using Stream stream = await content.ReadAsStreamAsync( Token );
    #endif

        var buffer = new MemoryStream( (int)stream.Length );
        await stream.CopyToAsync( buffer, Token );

        buffer.Seek( 0, SeekOrigin.Begin );
        return buffer;
    }
    public virtual async ValueTask<ReadOnlyMemory<byte>> AsMemory( HttpResponseMessage response ) => await AsBytes( response );
    public virtual async ValueTask<string> AsString( HttpResponseMessage response )
    {
        response.EnsureSuccessStatusCode();
        using HttpContent content = response.Content;

    #if NETSTANDARD2_1
        return await content.ReadAsStringAsync();
    #else
        return await content.ReadAsStringAsync( Token );
    #endif
    }
    public virtual async ValueTask<TResult> AsJson<TResult>( HttpResponseMessage response, JsonSerializer serializer )
    {
        await using MemoryStream stream = await AsStream( response );
        using var                sr     = new StreamReader( stream, Encoding );
        using JsonReader         reader = new JsonTextReader( sr );

        return serializer.Deserialize<TResult>( reader ) ?? throw new NullReferenceException( nameof(JsonConvert.DeserializeObject) );
    }


    public virtual ValueTask<WebResponse<bool>> AsBool() => WebResponse<bool>.Create( this, AsBool );
    public virtual ValueTask<WebResponse<byte[]>> AsBytes() => WebResponse<byte[]>.Create( this, AsBytes );
    public virtual ValueTask<WebResponse<JToken>> AsJson() => AsJson( JsonNet.LoadSettings );
    public virtual async ValueTask<WebResponse<JToken>> AsJson( JsonLoadSettings settings ) => await WebResponse<JToken>.Create( this, settings, AsJson );
    public virtual ValueTask<WebResponse<LocalFile>> AsFile() => WebResponse<LocalFile>.Create( this,                           AsFile );
    public virtual ValueTask<WebResponse<LocalFile>> AsFile( string    fileNameHeader ) => WebResponse<LocalFile>.Create( this, fileNameHeader, AsFile );
    public virtual ValueTask<WebResponse<LocalFile>> AsFile( FileInfo  path ) => WebResponse<LocalFile>.Create( this,           path,           AsFile );
    public virtual ValueTask<WebResponse<LocalFile>> AsFile( LocalFile file ) => WebResponse<LocalFile>.Create( this,           file,           AsFile );
    public virtual ValueTask<WebResponse<LocalFile>> AsFile( MimeType  type ) => WebResponse<LocalFile>.Create( this,           type,           AsFile );
    public virtual ValueTask<WebResponse<MemoryStream>> AsStream() => WebResponse<MemoryStream>.Create( this, AsStream );
    public virtual ValueTask<WebResponse<ReadOnlyMemory<byte>>> AsMemory() => WebResponse<ReadOnlyMemory<byte>>.Create( this, AsMemory );
    public virtual ValueTask<WebResponse<string>> AsString() => WebResponse<string>.Create( this, AsString );


    public virtual ValueTask<WebResponse<TResult>> AsJson<TResult>() => AsJson<TResult>( JsonNet.Serializer );
    public virtual ValueTask<WebResponse<TResult>> AsJson<TResult>( JsonSerializer serializer ) => WebResponse<TResult>.Create( this, serializer, AsJson<TResult> );
    public virtual void Dispose()
    {
        _request.Dispose();
        RetryPolicy = default;
    }


#if NET6_0_OR_GREATER
    public HttpVersionPolicy VersionPolicy
    {
        get => _request.VersionPolicy;
        set => _request.VersionPolicy = value;
    }
    public HttpRequestOptions Options => _request.Options;
#endif
}
