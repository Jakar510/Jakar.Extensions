#nullable enable
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



public class ResourceException<T> : ResourceException
{
    public ResourceException( string path ) : base( path, GetMessage( path ) ) { }

    public static string GetMessage( string path ) => @$"{nameof(EmbeddedResources<T>)}.{nameof(EmbeddedResources<T>.GetResourceStream)}.{nameof(Assembly.GetManifestResourceStream)}: ""{path}"".";
}



public class EmbeddedResources<T>
{
    private readonly Assembly _assembly;
    public           string   Namespace { get; }


    public EmbeddedResources()
    {
        _assembly = typeof(T).Assembly;
        Namespace = typeof(T).Namespace ?? throw new NullReferenceException( nameof(Type.Namespace) );
    }


    public Stream GetResourceStream( string fileName )
    {
        string path = GetPath( fileName );
        return _assembly.GetManifestResourceStream( path ) ?? throw new ResourceException<T>( path );
    }


    protected string GetPath( string fileName ) => $"{Namespace}.{fileName}";


    public string GetResourceText( string fileName ) => GetResourceText( fileName, Encoding.Default );
    public string GetResourceText( string fileName, Encoding encoding )
    {
        using Stream stream = GetResourceStream( fileName );
        using var    reader = new StreamReader( stream, encoding );
        string       text   = reader.ReadToEnd();
        return text;
    }


    public ValueTask SaveToFile( string fileName, LocalDirectory directory, CancellationToken token ) => SaveToFile( fileName, directory.Join( fileName ), token );
    public async ValueTask SaveToFile( string fileName, LocalFile file, CancellationToken token )
    {
        await using Stream stream = GetResourceStream( fileName );
        await file.WriteAsync( stream, token );
    }


    public byte[] GetResourceBytes( string fileName )
    {
        using Stream stream = GetResourceStream( fileName );
        using var    memory = new MemoryStream( (int)stream.Length );
        stream.CopyTo( memory );
        return memory.GetBuffer();
    }
    public async ValueTask<byte[]> GetResourceBytesAsync( string fileName )
    {
        await using Stream stream = GetResourceStream( fileName );
        await using var    reader = new MemoryStream();
        await stream.CopyToAsync( stream );
        return reader.GetBuffer();
    }


    public async ValueTask<string> GetResourceTextAsync( string fileName ) => await GetResourceTextAsync( fileName, Encoding.Default );
    public async ValueTask<string> GetResourceTextAsync( string fileName, Encoding encoding )
    {
        await using Stream stream = GetResourceStream( fileName );
        using var          reader = new StreamReader( stream, encoding );
        return await reader.ReadToEndAsync();
    }


    public async ValueTask<TValue> GetResourceTextAsync<TValue>( string fileName ) => await GetResourceTextAsync<TValue>( fileName, Encoding.Default );
    public async ValueTask<TValue> GetResourceTextAsync<TValue>( string fileName, Encoding encoding )
    {
        string text = await GetResourceTextAsync( fileName, encoding );
        return text.FromJson<TValue>();
    }
}
