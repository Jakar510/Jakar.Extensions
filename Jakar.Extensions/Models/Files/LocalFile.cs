// unset

#nullable enable
using System.Runtime.InteropServices;
using System.Security;
using System.Web;



namespace Jakar.Extensions;


[Serializable]
public class LocalFile : BaseCollections<LocalFile>, TempFile.ITempFile, LocalFile.IReadHandler, LocalFile.IAsyncReadHandler
{
    protected FileInfo? _info;


    private bool _isTemporary;

    [JsonIgnore] public FileInfo Info => _info ??= new FileInfo(FullPath);


    public string   FullPath      { get; init; }
    public string   Name          => Info.Name;
    public string   Extension     => Info.Extension;
    public bool     Exists        => Info.Exists;
    public string?  DirectoryName => Info.DirectoryName;
    public MimeType Mime          => Extension.FromExtension();
    public string ContentType
    {
        get
        {
            try { return Mime.ToContentType(); }
            catch ( ArgumentOutOfRangeException )
            {
                FullPath.WriteToConsole();
                FullPath.WriteToDebug();
                throw;
            }
        }
    }
    public string   Root         => Directory.GetDirectoryRoot(FullPath);
    public Encoding FileEncoding { get; init; } = Encoding.Default;


    [JsonIgnore]
    public LocalDirectory? Parent
    {
        get
        {
            DirectoryInfo? parent = Directory.GetParent(FullPath);
            if ( parent is null ) { return default; }

            return new LocalDirectory(parent);
        }
    }


    public LocalFile() => FullPath = string.Empty;
    public LocalFile( Uri                path ) : this(FromUri(path)) { }
    public LocalFile( ReadOnlySpan<char> path ) : this(path.ToString()) { }
    public LocalFile( FileInfo           path ) : this(path.FullName) { }
    public LocalFile( string             path, params string[] args ) : this(path, Encoding.Default, args) { }
    public LocalFile( string             path, Encoding?       encoding, params string[] args ) : this(path.Combine(args), encoding) { }
    public LocalFile( DirectoryInfo      path, string          fileName ) : this(path.Combine(fileName)) { }
    public LocalFile( string             path, string          fileName ) : this(new DirectoryInfo(path), fileName) { }

    public LocalFile( string path, Encoding? encoding = default )
    {
        this.SetNormal();
        FullPath     = Path.GetFullPath(path);
        FileEncoding = encoding ?? Encoding.Default;
    }


    public static implicit operator LocalFile( string             info ) => new(info);
    public static implicit operator LocalFile( FileInfo           info ) => new(info);
    public static implicit operator LocalFile( Uri                info ) => new(info);
    public static implicit operator LocalFile( ReadOnlySpan<char> info ) => new(info);


    public sealed override string ToString() => FullPath;


    /// <summary>
    ///     Changes the extension of the file.
    /// </summary>
    /// <param name = "ext" > </param>
    /// <returns>
    ///     The modified path information.
    ///     On Windows, if ext is null or empty, the path information is unchanged.
    ///     If the extension is null, the returned path is with the extension removed.
    ///     If the path has no extension, and the extension is not null, the returned string contains the extension appended to the end of the path.
    /// </returns>
    public LocalFile ChangeExtension( MimeType ext ) => ChangeExtension(ext.ToExtension());

    /// <summary>
    ///     Changes the extension of the file.
    /// </summary>
    /// <param name = "ext" > </param>
    /// <returns>
    ///     The modified path information.
    ///     On Windows, if ext is null or empty, the path information is unchanged.
    ///     If the extension is null, the returned path is with the extension removed.
    ///     If the path has no extension, and the extension is not null, the returned string contains the extension appended to the end of the path.
    /// </returns>
    public LocalFile ChangeExtension( string? ext ) => Path.ChangeExtension(FullPath, ext);


    /// <summary>
    /// 
    /// </summary>
    /// <param name = "uri" > </param>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "UnauthorizedAccessException" > </exception>
    /// <exception cref = "PathTooLongException" > </exception>
    /// <exception cref = "NotSupportedException" > </exception>
    /// <returns> </returns>
    protected static FileInfo FromUri( Uri uri )
    {
        if ( !uri.IsFile ) { throw new ArgumentException("Uri is not a file Uri.", nameof(uri)); }

        return new FileInfo(uri.AbsolutePath);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <exception cref = "PathTooLongException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "NotSupportedException" > </exception>
    /// <returns> </returns>
    public static FileStream CreateTempFileAndOpen( out LocalFile file )
    {
        FileStream stream = CreateAndOpen(Path.GetTempFileName(), out file);
        file.SetTemporary();
        return stream;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <exception cref = "PathTooLongException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "NotSupportedException" > </exception>
    /// <returns> </returns>
    public static FileStream CreateTempFileAndOpen( MimeType type, out LocalFile file )
    {
        ReadOnlySpan<char> ext  = type.ToExtension(true);
        ReadOnlySpan<char> name = Path.GetRandomFileName();
        name = name[..name.IndexOf('.')];
        Span<char> span = stackalloc char[name.Length + ext.Length];
        name.CopyTo(span);
        ext.CopyTo(span[name.Length..]);

        string     path   = Path.Combine(Path.GetTempPath(), span.ToString());
        FileStream stream = CreateAndOpen(path, out file);
        file.SetTemporary();
        return stream;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name = "path" > </param>
    /// <param name = "file" > </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <exception cref = "PathTooLongException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "NotSupportedException" > </exception>
    /// <returns> </returns>
    public static FileStream CreateAndOpen( string path, out LocalFile file )
    {
        file = new LocalFile(path);
        return File.Create(path);
    }


    /// <summary>
    ///     Permanently deletes a file.
    /// </summary>
    public void Delete() => Info.Delete();


    /// <summary>
    ///     Encrypts the file so that only the account used to encrypt the file can decrypt it.
    /// </summary>
    public void Encrypt()
    {
        if ( !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ) { throw new InvalidOperationException(); }

        File.Encrypt(FullPath);
    }
    /// <summary>
    ///     Decrypts a file that was encrypted by the current account using the
    ///     <see cref = "File.Encrypt(string)" />
    ///     method.
    /// </summary>
    public void Decrypt()
    {
        if ( !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ) { throw new InvalidOperationException(); }

        File.Decrypt(FullPath);
    }


    /// <summary>
    ///     Creates an
    ///     <see cref = "UriKind.Absolute" />
    ///     based on the detected
    ///     <see cref = "Mime" />
    /// </summary>
    /// <param name = "mime" >
    ///     To override the detected
    ///     <see cref = "Mime" />
    ///     , provide a non-null value
    /// </param>
    /// <returns> </returns>
    public Uri ToUri( MimeType? mime = null )
    {
        if ( string.IsNullOrWhiteSpace(FullPath) ) { throw new NullReferenceException(nameof(FullPath)); }

        MimeType type = mime ?? Mime;

        if ( !FullPath.StartsWith("/", StringComparison.InvariantCultureIgnoreCase) ) { return new Uri($"{type.ToUriScheme()}://{FullPath}", UriKind.Absolute); }

        string path = FullPath.Remove(0, 1);
        return new Uri($"{type.ToUriScheme()}://{path}", UriKind.Absolute);
    }

    /// <summary>
    ///     Creates a
    ///     <see cref = "Uri" />
    ///     using provided prefix, and
    ///     <see cref = "HttpUtility.UrlEncode(string)" />
    ///     to encode the
    ///     <see cref = "FullPath" />
    ///     .
    /// </summary>
    /// <param name = "baseUri" > </param>
    /// <param name = "prefix" >
    ///     The key to attach the
    ///     <see cref = "FullPath" />
    ///     to. Defaults to "?path="
    /// </param>
    /// <param name = "mime" >
    ///     To override the detected
    ///     <see cref = "Mime" />
    ///     , provide a non-null value
    /// </param>
    /// <returns> </returns>
    public Uri ToUri( Uri baseUri, in string prefix = "?path=", MimeType? mime = null )
    {
        if ( string.IsNullOrWhiteSpace(FullPath) ) { throw new NullReferenceException(nameof(FullPath)); }

        MimeType type = mime ?? Mime;

        if ( !FullPath.StartsWith("/", StringComparison.InvariantCultureIgnoreCase) ) { return new Uri($"{type.ToUriScheme()}://{FullPath}", UriKind.Absolute); }

        return new Uri(baseUri, $"{prefix}{HttpUtility.UrlEncode(FullPath)}");
    }


    /// <summary>
    ///     Copies this file to the
    ///     <paramref name = "newFile" />
    /// </summary>
    /// <param name = "newFile" > </param>
    /// <param name = "token" > </param>
    /// <returns> </returns>
    public async Task Clone( LocalFile newFile, CancellationToken token )
    {
        FileStream stream = OpenRead();
        await newFile.WriteAsync(stream, token);
    }

    /// <summary>
    ///     Moves this file to the new
    ///     <paramref name = "path" />
    /// </summary>
    /// <param name = "path" > </param>
    public void Move( string path ) => Info.MoveTo(path);
    /// <summary>
    ///     Moves this file to the new
    ///     <paramref name = "file" />
    ///     location
    /// </summary>
    /// <param name = "file" > </param>
    public void Move( LocalFile file ) => Info.MoveTo(file.FullPath);


    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "MD5" />
    /// </summary>
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
    public string Hash_MD5() => Hash(MD5.Create());
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA1" />
    /// </summary>
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
    public string Hash_SHA1() => Hash(SHA1.Create());
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA256" />
    /// </summary>
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
    public string Hash_SHA256() => Hash(SHA256.Create());
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA384" />
    /// </summary>
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
    public string Hash_SHA384() => Hash(SHA384.Create());
    /// <summary>
    ///     Calculates a file hash using
    ///     <see cref = "SHA512" />
    /// </summary>
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
    public string Hash_SHA512() => Hash(SHA512.Create());

    protected string Hash( HashAlgorithm hasher )
    {
        using ( hasher )
        {
            using FileStream stream = OpenRead();
            byte[]           hash   = hasher.ComputeHash(stream);
            return BitConverter.ToString(hash);
        }
    }

#if NET6_0
    public async Task<string> HashAsync()
    {
        using var              md5    = SHA256.Create();
        await using FileStream stream = OpenRead();
        byte[]                 hash   = await md5.ComputeHashAsync(stream);

        return BitConverter.ToString(hash);
    }
#endif

    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public FileStream Create() => Info.Create();

    /// <summary>
    ///     <para>
    ///         <seealso href = "https://stackoverflow.com/a/11541330/9530917" />
    ///     </para>
    /// </summary>
    /// <param name = "mode" > </param>
    /// <param name = "access" > </param>
    /// <param name = "share" > </param>
    /// <param name = "bufferSize" > </param>
    /// <param name = "useAsync" > </param>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "NotSupportedException" > </exception>
    /// <exception cref = "ArgumentOutOfRangeException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "UnauthorizedAccessException" > </exception>
    /// <exception cref = "PathTooLongException" > </exception>
    /// <returns>
    ///     <see cref = "FileStream" />
    /// </returns>
    public FileStream Open( FileMode mode, FileAccess access, FileShare share, int bufferSize = 4096, bool useAsync = true )
    {
        if ( string.IsNullOrWhiteSpace(FullPath) ) { throw new NullReferenceException(nameof(FullPath)); }

        return new FileStream(FullPath, mode, access, share, bufferSize, useAsync);
    }


    /// <summary>
    ///     Opens file for read only actions. If it doesn't exist,
    ///     <see cref = "FileNotFoundException" />
    ///     will be raised.
    /// </summary>
    /// <param name = "bufferSize" > </param>
    /// <param name = "useAsync" > </param>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "NotSupportedException" > </exception>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentOutOfRangeException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "UnauthorizedAccessException" > </exception>
    /// <exception cref = "PathTooLongException" > </exception>
    /// <returns>
    ///     <see cref = "FileStream" />
    /// </returns>
    public FileStream OpenRead( int bufferSize = 4096, bool useAsync = true ) => Open(FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync);
    /// <summary>
    ///     Opens file for write only actions. If it doesn't exist, file will be created.
    /// </summary>
    /// <param name = "mode" > </param>
    /// <param name = "bufferSize" > </param>
    /// <param name = "useAsync" > </param>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "NotSupportedException" > </exception>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentOutOfRangeException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "UnauthorizedAccessException" > </exception>
    /// <exception cref = "PathTooLongException" > </exception>
    /// <returns>
    ///     <see cref = "FileStream" />
    /// </returns>
    public FileStream OpenWrite( FileMode mode, int bufferSize = 4096, bool useAsync = true ) => Open(mode, FileAccess.Write, FileShare.None, bufferSize, useAsync);


    // ---------------------------------------------------------------------------------------------------------------------------------------------------

    // /// <summary>
    // /// Reads the contents of the file as a <see cref="string"/>.
    // /// </summary>
    // /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    // /// <exception cref="FileNotFoundException">if file is not found</exception>	'
    // /// <returns><see cref="string"/></returns>
    // public string ReadAsString()
    // {
    //     using var stream = new StreamReader(OpenRead(), _encoding);
    //     return stream.ReadToEnd();
    // }
    //
    // /// <summary>
    // /// Reads the contents of the file as a <see cref="string"/>, asynchronously.
    // /// </summary>
    // /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    // /// <exception cref="FileNotFoundException">if file is not found</exception>
    // /// <returns><see cref="string"/></returns>
    // public async Task<string> ReadAsStringAsync()
    // {
    //     using var stream = new StreamReader(OpenRead(), _encoding);
    //     return await stream.ReadToEndAsync();
    // }
    //
    //
    // /// <summary>
    // /// Reads the contents of the file as a <see cref="string"/>.
    // /// </summary>
    // /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    // /// <exception cref="FileNotFoundException">if file is not found</exception>
    // /// <returns><see cref="string"/></returns>
    // public T Read<T>()
    // {
    //     string content = ReadAsString();
    //     return content.FromJson<T>();
    //
    //     /// <summary>
    //     /// Reads the contents of the file as a <see cref="string"/>, then calls <see cref="JsonExtensions.FromJson{TResult}(string)"/> on it, asynchronously.
    //     /// </summary>
    //     /// <typeparam name="T"></typeparam>
    //     /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    //     /// <exception cref="FileNotFoundException">if file is not found</exception>
    //     /// <exception cref="JsonReaderException">if an error in deserialization occurs</exception>
    //     /// <returns><typeparamref name="T"/></returns>
    // }
    // /// <summary>
    // /// Reads the contents of the file as a <see cref="string"/>, then calls <see cref="JsonExtensions.FromJson{TResult}(string)"/> on it, asynchronously.
    // /// </summary>
    // /// <typeparam name="T"></typeparam>
    // /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    // /// <exception cref="FileNotFoundException">if file is not found</exception>
    // /// <exception cref="JsonReaderException">if an error in deserialization occurs</exception>
    // /// <returns><typeparamref name="T"/></returns>
    // public async Task<T> ReadAsync<T>()
    // {
    //     string content = await ReadAsStringAsync();
    //     return content.FromJson<T>();
    // }
    //
    //
    // /// <summary>
    // /// Reads the contents of the file as a <see cref="ReadOnlySpan{byte}"/>.
    // /// </summary>
    // /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    // /// <exception cref="FileNotFoundException">if file is not found</exception>
    // /// <returns><see cref="ReadOnlySpan{byte}"/></returns>
    // public ReadOnlySpan<char> ReadAsSpan()
    // {
    //     using var stream = new StreamReader(OpenRead(), _encoding);
    //     return stream.ReadToEnd();
    // }
    //
    //
    // /// <summary>
    // /// Reads the contents of the file as a byte array.
    // /// </summary>
    // /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    // /// <exception cref="FileNotFoundException">if file is not found</exception>
    // /// <returns><see cref="byte[]"/></returns>
    // public byte[] ReadAsBytes()
    // {
    //     using FileStream file   = OpenRead();
    //     using var        stream = new MemoryStream();
    //     file.CopyTo(stream);
    //     return stream.ToArray();
    // }
    // /// <summary>
    // /// Reads the contents of the file as a byte array.
    // /// </summary>
    // /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    // /// <exception cref="FileNotFoundException">if file is not found</exception>
    // /// <returns><see cref="byte[]"/></returns>
    // public async Task<byte[]> ReadAsBytesAsync( CancellationToken token )
    // {
    //     await using FileStream file   = OpenRead();
    //     await using var        stream = new MemoryStream();
    //     await file.CopyToAsync(stream, token);
    //     return stream.ToArray();
    // }
    //
    //
    // /// <summary>
    // /// Reads the contents of the file as a <see cref="ReadOnlyMemory{byte}"/>.
    // /// </summary>
    // /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    // /// <exception cref="FileNotFoundException">if file is not found</exception>
    // /// <returns><see cref="ReadOnlyMemory{byte}"/></returns>
    // public ReadOnlyMemory<byte> ReadAsMemory()
    // {
    //     ReadOnlyMemory<byte> results = ReadAsBytes();
    //     return results;
    // }
    // /// <summary>
    // /// Reads the contents of the file as a <see cref="ReadOnlyMemory{byte}"/>, asynchronously.
    // /// </summary>
    // /// <param name="token"></param>
    // /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    // /// <exception cref="FileNotFoundException">if file is not found</exception>
    // /// <returns><see cref="ReadOnlyMemory{byte}"/></returns>
    // public async Task<ReadOnlyMemory<byte>> ReadAsMemoryAsync( CancellationToken token )
    // {
    //     ReadOnlyMemory<byte> results = await ReadAsBytesAsync(token);
    //     return results;
    // }
    // public async Task<MemoryStream> ReadAsStreamAsync( CancellationToken token )
    // {
    //     await using FileStream file   = OpenRead();
    //     var                    stream = new MemoryStream();
    //     await file.CopyToAsync(stream, token).ConfigureAwait(false);
    //     return stream;
    // }


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public IReadHandler Read() => this;


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public IAsyncReadHandler ReadAsync() => this;


    // ---------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <returns>
    ///     <see cref = "Task" />
    /// </returns>
    public void Write( StringBuilder payload ) => Write(payload.ToString());
    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <returns>
    ///     <see cref = "Task" />
    /// </returns>
    public async Task WriteAsync( StringBuilder payload ) => await WriteAsync(payload.ToString())
                                                                .ConfigureAwait(false);


    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <returns>
    ///     <see cref = "Task" />
    /// </returns>
    public void Write( string payload )
    {
        if ( string.IsNullOrWhiteSpace(payload) ) { throw new ArgumentNullException(nameof(payload)); }

        using FileStream stream = Create();
        using var        writer = new StreamWriter(stream, FileEncoding);
        writer.Write(payload);
    }
    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <returns>
    ///     <see cref = "Task" />
    /// </returns>
    public async Task WriteAsync( string payload )
    {
        if ( string.IsNullOrWhiteSpace(payload) ) { throw new ArgumentNullException(nameof(payload)); }

        await using FileStream stream = Create();
        await using var        writer = new StreamWriter(stream, FileEncoding);

        await writer.WriteAsync(payload)
                    .ConfigureAwait(false);
    }


    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <returns>
    ///     <see cref = "Task" />
    /// </returns>
    public void Write( byte[] payload )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        using FileStream stream = Create();
        stream.Write(payload, 0, payload.Length);
    }
    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <param name = "token" > </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <returns>
    ///     <see cref = "Task" />
    /// </returns>
    public async Task WriteAsync( byte[] payload, CancellationToken token )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        await using FileStream stream = Create();

        await stream.WriteAsync(payload, token)
                    .ConfigureAwait(false);
    }


    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <returns>
    ///     <see cref = "Task" />
    /// </returns>
    public void Write( Span<byte> payload )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        using FileStream stream = Create();
        stream.Write(payload);
    }


    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <returns>
    ///     <see cref = "Task" />
    /// </returns>
    public void Write( ReadOnlySpan<byte> payload )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        using FileStream stream = Create();
        stream.Write(payload);
    }


    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <returns>
    ///     <see cref = "Task" />
    /// </returns>
    public void Write( ReadOnlyMemory<byte> payload ) => Write(payload.ToArray());
    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <param name = "token" > </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <returns>
    ///     <see cref = "Task" />
    /// </returns>
    public async Task WriteAsync( ReadOnlyMemory<byte> payload, CancellationToken token )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        await using FileStream stream = Create();

        await stream.WriteAsync(payload, token)
                    .ConfigureAwait(false);
    }


    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <returns>
    ///     <see cref = "Task" />
    /// </returns>
    public void Write( ReadOnlyMemory<char> payload )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        using FileStream stream = Create();
        using var        writer = new StreamWriter(stream, FileEncoding);
        writer.Write(payload);
    }
    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <param name = "token" > </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <returns>
    ///     <see cref = "Task" />
    /// </returns>
    public async Task WriteAsync( ReadOnlyMemory<char> payload, CancellationToken token )
    {
        await using FileStream stream = Create();
        await using var        writer = new StreamWriter(stream, FileEncoding);

        await writer.WriteAsync(payload, token)
                    .ConfigureAwait(false);
    }


    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <returns>
    ///     <see cref = "Task" />
    /// </returns>
    public void Write( Stream payload )
    {
        if ( payload is null ) { throw new ArgumentNullException(nameof(payload)); }

        using var memory = new MemoryStream();
        payload.CopyTo(memory);
        ReadOnlySpan<byte> data = memory.ToArray();
        Write(data);
    }
    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <param name = "token" > </param>
    /// <exception cref = "NullReferenceException" > </exception>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <returns>
    ///     <see cref = "Task" />
    /// </returns>
    public async Task WriteAsync( Stream payload, CancellationToken token )
    {
        if ( payload is null ) { throw new ArgumentNullException(nameof(payload)); }

        await using var memory = new MemoryStream();
        await payload.CopyToAsync(memory, token);
        ReadOnlyMemory<byte> data = memory.ToArray();

        await WriteAsync(data, token)
           .ConfigureAwait(false);
    }


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <param name = "path" > </param>
    /// <param name = "token" > </param>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "WebException" > </exception>
    /// <exception cref = "NotSupportedException" > </exception>
    /// <returns> </returns>
    public static async Task<LocalFile> SaveToFileAsync( string path, Stream payload, CancellationToken token )
    {
        var file = new LocalFile(path);

        await file.WriteAsync(payload, token)
                  .ConfigureAwait(false);

        return file;
    }


    /// <summary>
    ///     Write the
    ///     <paramref name = "payload" />
    ///     to the file.
    /// </summary>
    /// <param name = "payload" > the data being written to the file </param>
    /// <param name = "path" > </param>
    /// <param name = "token" > </param>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "WebException" > </exception>
    /// <exception cref = "NotSupportedException" > </exception>
    /// <returns>
    ///     <see cref = "LocalFile" />
    /// </returns>
    public static async Task<LocalFile> SaveToFileAsync( string path, ReadOnlyMemory<byte> payload, CancellationToken token )
    {
        var file = new LocalFile(path);

        await file.WriteAsync(payload, token)
                  .ConfigureAwait(false);

        return file;
    }


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public static bool operator ==( LocalFile? left, LocalFile? right ) => Equalizer.Instance.Equals(left, right);
    public static bool operator !=( LocalFile? left, LocalFile? right ) => !Equalizer.Instance.Equals(left, right);
    public static bool operator <( LocalFile?  left, LocalFile? right ) => Sorter.Instance.Compare(left, right) < 0;
    public static bool operator >( LocalFile?  left, LocalFile? right ) => Sorter.Instance.Compare(left, right) > 0;
    public static bool operator <=( LocalFile? left, LocalFile? right ) => Sorter.Instance.Compare(left, right) <= 0;
    public static bool operator >=( LocalFile? left, LocalFile? right ) => Sorter.Instance.Compare(left, right) >= 0;
    public override int GetHashCode() => HashCode.Combine(FullPath, this.IsTempFile());
    protected virtual void Dispose( bool remove )
    {
        if ( remove && Exists ) { Delete(); }
    }


    async Task<string> IAsyncReadHandler.AsString()
    {
        using var stream = new StreamReader(OpenRead(), FileEncoding);

        return await stream.ReadToEndAsync()
                           .ConfigureAwait(false);
    }


    async Task<T> IAsyncReadHandler.AsJson<T>()
    {
        using var stream = new StreamReader(OpenRead(), FileEncoding);

        string content = await stream.ReadToEndAsync()
                                     .ConfigureAwait(false);

        return content.FromJson<T>();
    }


    async Task<byte[]> IAsyncReadHandler.AsBytes( CancellationToken token )
    {
        await using FileStream file   = OpenRead();
        await using var        stream = new MemoryStream();

        await file.CopyToAsync(stream, token)
                  .ConfigureAwait(false);

        return stream.ToArray();
    }


    async Task<ReadOnlyMemory<byte>> IAsyncReadHandler.AsMemory( CancellationToken token )
    {
        await using FileStream file   = OpenRead();
        await using var        stream = new MemoryStream();

        await file.CopyToAsync(stream, token)
                  .ConfigureAwait(false);

        ReadOnlyMemory<byte> results = stream.ToArray();
        return results;
    }


    async Task<MemoryStream> IAsyncReadHandler.AsStream( CancellationToken token )
    {
        await using FileStream file   = OpenRead();
        var                    stream = new MemoryStream();

        await file.CopyToAsync(stream, token)
                  .ConfigureAwait(false);

        return stream;
    }


    public override int CompareTo( LocalFile? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( ReferenceEquals(null, other) ) { return 1; }

        return string.Compare(FullPath, other.FullPath, StringComparison.Ordinal);
    }
    public override bool Equals( LocalFile? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return this.IsTempFile() == other.IsTempFile() && FullPath == other.FullPath;
    }


    /// <summary>
    ///     Reads the contents of the file as a
    ///     <see cref = "string" />
    ///     .
    /// </summary>
    /// <exception cref = "NullReferenceException" > if FullPath is null or empty </exception>
    /// <exception cref = "FileNotFoundException" > if file is not found </exception>
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
    T IReadHandler.AsJson<T>()
    {
        using var stream  = new StreamReader(OpenRead(), FileEncoding);
        string    content = stream.ReadToEnd();
        return content.FromJson<T>();
    }

    /// <summary>
    ///     Reads the contents of the file as a
    ///     <see cref = "string" />
    ///     .
    /// </summary>
    /// <exception cref = "NullReferenceException" > if FullPath is null or empty </exception>
    /// <exception cref = "FileNotFoundException" > if file is not found </exception>
    /// '
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
    string IReadHandler.AsString()
    {
        using var stream = new StreamReader(OpenRead(), FileEncoding);
        return stream.ReadToEnd();
    }


    /// <summary>
    ///     Reads the contents of the file as a
    ///     <see cref = "ReadOnlySpan{byte}" />
    ///     .
    /// </summary>
    /// <exception cref = "NullReferenceException" > if FullPath is null or empty </exception>
    /// <exception cref = "FileNotFoundException" > if file is not found </exception>
    /// <returns>
    ///     <see cref = "ReadOnlySpan{byte}" />
    /// </returns>
    ReadOnlySpan<char> IReadHandler.AsSpan()
    {
        using var          stream = new StreamReader(OpenRead(), FileEncoding);
        ReadOnlySpan<char> result = stream.ReadToEnd();
        return result;
    }


    /// <summary>
    ///     Reads the contents of the file as a byte array.
    /// </summary>
    /// <exception cref = "NullReferenceException" > if FullPath is null or empty </exception>
    /// <exception cref = "FileNotFoundException" > if file is not found </exception>
    /// <returns>
    ///     <see cref = "byte[]" />
    /// </returns>
    byte[] IReadHandler.AsBytes()
    {
        using FileStream file   = OpenRead();
        using var        stream = new MemoryStream();
        file.CopyTo(stream);
        return stream.ToArray();
    }


    /// <summary>
    ///     Reads the contents of the file as a
    ///     <see cref = "ReadOnlyMemory{byte}" />
    ///     .
    /// </summary>
    /// <exception cref = "NullReferenceException" > if FullPath is null or empty </exception>
    /// <exception cref = "FileNotFoundException" > if file is not found </exception>
    /// <returns>
    ///     <see cref = "ReadOnlyMemory{byte}" />
    /// </returns>
    ReadOnlyMemory<byte> IReadHandler.AsMemory()
    {
        using FileStream file   = OpenRead();
        using var        stream = new MemoryStream();
        file.CopyTo(stream);
        ReadOnlyMemory<byte> results = stream.ToArray();
        return results;
    }

    bool TempFile.ITempFile.IsTemporary
    {
        get => _isTemporary;
        set => _isTemporary = value;
    }


    public void Dispose()
    {
        Dispose(this.IsTempFile());
        GC.SuppressFinalize(this);
    }



    public interface IReadHandler
    {
        /// <summary>
        ///     Reads the contents of the file as a
        ///     <see cref = "ReadOnlySpan{byte}" />
        ///     .
        /// </summary>
        /// <exception cref = "NullReferenceException" > if FullPath is null or empty </exception>
        /// <exception cref = "FileNotFoundException" > if file is not found </exception>
        /// <returns>
        ///     <see cref = "ReadOnlySpan{byte}" />
        /// </returns>
        ReadOnlySpan<char> AsSpan();
        /// <summary>
        ///     Reads the contents of the file as a
        ///     <see cref = "string" />
        ///     .
        /// </summary>
        /// <exception cref = "NullReferenceException" > if FullPath is null or empty </exception>
        /// <exception cref = "FileNotFoundException" > if file is not found </exception>
        /// '
        /// <returns>
        ///     <see cref = "string" />
        /// </returns>
        string AsString();
        /// <summary>
        ///     Reads the contents of the file as a
        ///     <see cref = "string" />
        ///     .
        /// </summary>
        /// <exception cref = "NullReferenceException" > if FullPath is null or empty </exception>
        /// <exception cref = "FileNotFoundException" > if file is not found </exception>
        /// <returns>
        ///     <see cref = "string" />
        /// </returns>
        T AsJson<T>();
        /// <summary>
        ///     Reads the contents of the file as a byte array.
        /// </summary>
        /// <exception cref = "NullReferenceException" > if FullPath is null or empty </exception>
        /// <exception cref = "FileNotFoundException" > if file is not found </exception>
        /// <returns>
        ///     <see cref = "byte[]" />
        /// </returns>
        byte[] AsBytes();
        /// <summary>
        ///     Reads the contents of the file as a
        ///     <see cref = "ReadOnlyMemory{byte}" />
        ///     .
        /// </summary>
        /// <exception cref = "NullReferenceException" > if FullPath is null or empty </exception>
        /// <exception cref = "FileNotFoundException" > if file is not found </exception>
        /// <returns>
        ///     <see cref = "ReadOnlyMemory{byte}" />
        /// </returns>
        ReadOnlyMemory<byte> AsMemory();
    }



    public interface IAsyncReadHandler
    {
        /// <summary>
        ///     Reads the contents of the file as a
        ///     <see cref = "string" />
        ///     , then calls
        ///     <see cref = "JsonExtensions.FromJson{TResult}(string)" />
        ///     on it, asynchronously.
        /// </summary>
        /// <typeparam name = "T" > </typeparam>
        /// <exception cref = "NullReferenceException" > if FullPath is null or empty </exception>
        /// <exception cref = "FileNotFoundException" > if file is not found </exception>
        /// <exception cref = "JsonReaderException" > if an error in deserialization occurs </exception>
        /// <returns>
        ///     <typeparamref name = "T" />
        /// </returns>
        Task<T> AsJson<T>();

        /// <summary>
        ///     Reads the contents of the file as a
        ///     <see cref = "string" />
        ///     , asynchronously.
        /// </summary>
        /// <exception cref = "NullReferenceException" > if FullPath is null or empty </exception>
        /// <exception cref = "FileNotFoundException" > if file is not found </exception>
        /// <returns>
        ///     <see cref = "string" />
        /// </returns>
        Task<string> AsString();

        /// <summary>
        ///     Reads the contents of the file as a byte array.
        /// </summary>
        /// <exception cref = "NullReferenceException" > if FullPath is null or empty </exception>
        /// <exception cref = "FileNotFoundException" > if file is not found </exception>
        /// <returns>
        ///     <see cref = "byte[]" />
        /// </returns>
        Task<byte[]> AsBytes( CancellationToken token );

        /// <summary>
        ///     Reads the contents of the file as a
        ///     <see cref = "ReadOnlyMemory{byte}" />
        ///     , asynchronously.
        /// </summary>
        /// <param name = "token" > </param>
        /// <exception cref = "NullReferenceException" > if FullPath is null or empty </exception>
        /// <exception cref = "FileNotFoundException" > if file is not found </exception>
        /// <returns>
        ///     <see cref = "ReadOnlyMemory{byte}" />
        /// </returns>
        Task<ReadOnlyMemory<byte>> AsMemory( CancellationToken token );


        Task<MemoryStream> AsStream( CancellationToken token );
    }



    // ---------------------------------------------------------------------------------------------------------------------------------------------------



    /// <summary>
    ///     A collection of files that are in the
    ///     <see cref = "LocalDirectory" />
    /// </summary>
    [Serializable]
    public class Watcher : ConcurrentCollection, IDisposable
    {
        private readonly LocalDirectory.Watcher _watcher;


        public Watcher( LocalDirectory.Watcher watcher ) : base(watcher.Directory.GetFiles())
        {
            _watcher                     =  watcher;
            _watcher.Created             += OnCreated;
            _watcher.Changed             += OnChanged;
            _watcher.Deleted             += OnDeleted;
            _watcher.Renamed             += OnRenamed;
            _watcher.Error               += OnError;
            _watcher.EnableRaisingEvents =  true;
        }
        public event ErrorEventHandler? Error;


        private void OnRenamed( object sender, RenamedEventArgs e )
        {
            LocalFile? file = this.FirstOrDefault(x => x.FullPath == e.OldFullPath);
            if ( file is not null ) { Remove(file); }

            Add(e.FullPath);
        }
        private void OnDeleted( object sender, FileSystemEventArgs e ) => Remove(e.FullPath);
        private void OnChanged( object sender, FileSystemEventArgs e )
        {
            var file = new LocalFile(e.FullPath);
            Replaced(file, file);
        }
        private void OnCreated( object sender, FileSystemEventArgs e ) => Add(e.FullPath);
        private void OnError( object   sender, ErrorEventArgs      e ) => Error?.Invoke(sender, e);


        public void Dispose()
        {
            _watcher.EnableRaisingEvents = false;

            _watcher.Created -= OnCreated;
            _watcher.Changed -= OnChanged;
            _watcher.Deleted -= OnDeleted;
            _watcher.Renamed -= OnRenamed;
            _watcher.Error   -= OnError;

            _watcher.Dispose();
        }
    }
}
