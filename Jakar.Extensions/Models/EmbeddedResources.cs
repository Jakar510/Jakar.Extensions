#nullable enable
namespace Jakar.Extensions;


public class ResourceException : Exception
{
    public string Path { get; init; }

    public ResourceException( string path, string message ) : base(message)
    {
        Path               = path;
        Data[nameof(Path)] = path;
    }
}



public class ResourceException<T> : ResourceException
{
    public ResourceException( string path ) : base(path, GetMessage(path)) { }

    public static string GetMessage( string path ) => @$"{nameof(EmbeddedResources<T>)}.{nameof(EmbeddedResources<T>.GetResourceStream)}.{nameof(Assembly.GetManifestResourceStream)}: ""{path}"".";
}



public class EmbeddedResources<T>
{
    // ReSharper disable once MemberCanBeMadeStatic.Local
    protected         Type     _Type      => typeof(T);
    protected         Assembly _Assembly  => _Type.Assembly;
    protected virtual string   _Namespace => _Type.Namespace ?? throw new NullReferenceException(nameof(_Type.Namespace));


    public EmbeddedResources() { }


    protected string GetPath( string fileName ) => $"{_Namespace}.{fileName}";


    public Stream GetResourceStream( string fileName )
    {
        string path = GetPath(fileName);

        return _Assembly.GetManifestResourceStream(path) ?? throw new ResourceException<T>(path);
    }


    public string GetResourceText( string fileName ) => GetResourceText(fileName, Encoding.Default);

    public string GetResourceText( string fileName, Encoding encoding )
    {
        Stream    stream = GetResourceStream(fileName);
        using var reader = new StreamReader(stream, encoding);
        string    text   = reader.ReadToEnd();
        return text;
    }

    public async Task<string> GetResourceTextAsync( string fileName ) => await GetResourceTextAsync(fileName, Encoding.Default);

    public async Task<string> GetResourceTextAsync( string fileName, Encoding encoding )
    {
        Stream    stream = GetResourceStream(fileName);
        using var reader = new StreamReader(stream, encoding);
        return await reader.ReadToEndAsync();
    }

    public async Task<TValue> GetResourceTextAsync<TValue>( string fileName ) => await GetResourceTextAsync<TValue>(fileName, Encoding.Default);

    public async Task<TValue> GetResourceTextAsync<TValue>( string fileName, Encoding encoding )
    {
        string text = await GetResourceTextAsync(fileName, encoding);
        return text.FromJson<TValue>();
    }


    public ReadOnlyMemory<byte> GetResourceBytes( string fileName )
    {
        Stream    stream = GetResourceStream(fileName);
        using var reader = new MemoryStream();
        stream.CopyTo(stream);
        return reader.ToArray();
    }

    public async Task<ReadOnlyMemory<byte>> GetResourceBytesAsync( string fileName )
    {
        Stream          stream = GetResourceStream(fileName);
        await using var reader = new MemoryStream();
        await stream.CopyToAsync(stream);
        return reader.ToArray();
    }


    public Task SaveToFile( string fileName, LocalDirectory directory, CancellationToken token ) => SaveToFile(fileName, directory.Join(fileName), token);
    public async Task SaveToFile( string fileName, LocalFile file, CancellationToken token )
    {
        Stream stream = GetResourceStream(fileName);
        await file.WriteAsync(stream, token);
    }
}
