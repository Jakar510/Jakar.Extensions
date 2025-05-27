namespace Jakar.Extensions;


public class ResourceException : Exception
{
    public string Path { get; init; }
    public ResourceException( string path, string message ) : base( message )
    {
        Path               = path;
        Data[nameof(Path)] = path;
    }
}



public class ResourceException<TValue>( string path ) : ResourceException( path, GetMessage( path ) )
{
    public static string GetMessage( string path ) => $"""{nameof(EmbeddedResources<TValue>)}.{nameof(EmbeddedResources<TValue>.GetResourceStream)}.{nameof(Assembly.GetManifestResourceStream)}: "{path}".""";
}



public class EmbeddedResources<TValue>
{
    private readonly Assembly _assembly = typeof(TValue).Assembly;
    public static    string   Namespace { get; } = typeof(TValue).Namespace ?? throw new NullReferenceException( nameof(Type.Namespace) );


    public Stream GetResourceStream( string fileName )
    {
        string path = GetPath( fileName );
        return _assembly.GetManifestResourceStream( path ) ?? throw new ResourceException<TValue>( path );
    }


    protected static string GetPath( string fileName ) => $"{Namespace}.{fileName}";


    public string GetResourceText( string fileName ) => GetResourceText( fileName, Encoding.Default );
    public string GetResourceText( string fileName, Encoding encoding )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using Stream        stream        = GetResourceStream( fileName );
        using StreamReader  reader        = new(stream, encoding);
        string              text          = reader.ReadToEnd();
        return text;
    }


    public ValueTask SaveToFile( string fileName, LocalDirectory directory, CancellationToken token = default ) => SaveToFile( fileName, directory.Join( fileName ), token );
    public async ValueTask SaveToFile( string fileName, LocalFile file, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan   = TelemetrySpan.Create();
        await using Stream  stream = GetResourceStream( fileName );
        await file.WriteAsync( stream,  token );
    }


    public byte[] GetResourceBytes( string fileName )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using Stream        stream        = GetResourceStream( fileName );
        using MemoryStream  memory        = new((int)stream.Length);
        stream.CopyTo( memory );
        return memory.GetBuffer();
    }
    public async ValueTask<byte[]> GetResourceBytesAsync( string fileName )
    {
        using TelemetrySpan      telemetrySpan = TelemetrySpan.Create();
        await using Stream       stream        = GetResourceStream( fileName );
        await using MemoryStream reader        = new();
        await stream.CopyToAsync( stream );
        return reader.GetBuffer();
    }


    public async ValueTask<string> GetResourceTextAsync( string fileName ) => await GetResourceTextAsync( fileName, Encoding.Default );
    public async ValueTask<string> GetResourceTextAsync( string fileName, Encoding encoding )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        await using Stream  stream        = GetResourceStream( fileName );
        using StreamReader  reader        = new(stream, encoding);
        return await reader.ReadToEndAsync();
    }


    public async ValueTask<T> GetResourceJsonAsync<T>( string fileName ) => await GetResourceJsonAsync<T>( fileName, Encoding.Default );
    public async ValueTask<T> GetResourceJsonAsync<T>( string fileName, Encoding encoding )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        string              text          = await GetResourceTextAsync( fileName, encoding );
        return text.FromJson<T>();
    }
}
