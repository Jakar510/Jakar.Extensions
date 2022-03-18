// unset

using System.Security;
using System.Web;


namespace Jakar.Extensions.FileSystemExtensions;


[Serializable]
public class LocalFile : ILocalFile<LocalFile, LocalDirectory>
{
    protected FileInfo? _info;

    [JsonIgnore]
    public FileInfo Info
    {
        get
        {
            _info ??= new FileInfo(FullPath);
            return _info;
        }
    }


    public string   FullPath      { get; init; }
    public string   Name          => Info.Name;
    public string   Extension     => Info.Extension;
    public bool     Exists        => Info.Exists;
    public string?  DirectoryName => Info.DirectoryName;
    public MimeType Mime          => Extension.FromExtension();
    public string   ContentType   => Mime.ToContentType();
    public string?  Root          => Directory.GetDirectoryRoot(FullPath);

    [JsonIgnore]
    public LocalDirectory? Parent
    {
        get
        {
            DirectoryInfo? parent = Directory.GetParent(FullPath);
            if ( parent is null ) { return null; }

            return new LocalDirectory(parent);
        }
    }


    public LocalFile() => FullPath = string.Empty;
    public LocalFile( Uri                path ) : this(FromUri(path)) { }
    public LocalFile( ReadOnlySpan<char> path ) : this(path.ToString()) { }
    public LocalFile( FileInfo           path ) : this(path.FullName) { }
    public LocalFile( string             path, params string[] args ) : this(path.Combine(args)) { }
    public LocalFile( DirectoryInfo      path, string          fileName ) : this(path.Combine(fileName)) { }
    public LocalFile( string             path, string          fileName ) : this(new DirectoryInfo(path), fileName) { }

    public LocalFile( string path )
    {
        this.SetNormal();
        FullPath = Path.GetFullPath(path);
    }

    public static implicit operator LocalFile( string             info ) => new(info);
    public static implicit operator LocalFile( FileInfo           info ) => new(info);
    public static implicit operator LocalFile( Uri                info ) => new(info);
    public static implicit operator LocalFile( ReadOnlySpan<char> info ) => new(info);


    public LocalFile ChangeExtension( MimeType ext ) => ChangeExtension(ext.ToExtension());

    public LocalFile ChangeExtension( string? ext ) => Path.ChangeExtension(FullPath, ext);


    /// <summary>
    ///
    /// </summary>
    /// <param name="uri"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns></returns>
    protected static FileInfo FromUri( Uri uri )
    {
        if ( !uri.IsFile ) { throw new ArgumentException("Uri is not a file Uri.", nameof(uri)); }

        return new FileInfo(uri.AbsolutePath);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns></returns>
    public static FileStream CreateTempFileAndOpen( out LocalFile file )
    {
        FileStream stream = CreateAndOpen(Path.GetTempFileName(), out file);
        file.SetTemporary();
        return stream;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="file"></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns></returns>
    public static FileStream CreateAndOpen( string path, out LocalFile file )
    {
        file = new LocalFile(path);
        return File.Create(path);
    }


    public void Delete() => new FileInfo(FullPath).Delete();


    public void Encrypt() => File.Encrypt(FullPath);
    public void Decrypt() => File.Decrypt(FullPath);


    public Uri ToUri( MimeType? mime = null )
    {
        if ( string.IsNullOrWhiteSpace(FullPath) ) { throw new NullReferenceException(nameof(FullPath)); }

        MimeType type = mime ?? Mime;

        if ( !FullPath.StartsWith("/", StringComparison.InvariantCultureIgnoreCase) ) { return new Uri($"{type.ToUriScheme()}://{FullPath}", UriKind.Absolute); }

        string path = FullPath.Remove(0, 1);
        return new Uri($"{type.ToUriScheme()}://{path}", UriKind.Absolute);
    }

    public Uri ToUri( Uri baseUri, in string prefix = "?path=", MimeType? mime = null )
    {
        if ( string.IsNullOrWhiteSpace(FullPath) ) { throw new NullReferenceException(nameof(FullPath)); }

        MimeType type = mime ?? Mime;

        if ( !FullPath.StartsWith("/", StringComparison.InvariantCultureIgnoreCase) ) { return new Uri($"{type.ToUriScheme()}://{FullPath}", UriKind.Absolute); }

        return new Uri(baseUri, $"{prefix}{HttpUtility.UrlEncode(FullPath)}");
    }


    public async Task Clone( LocalFile newFile )
    {
        FileStream stream = OpenRead();
        await newFile.WriteToFileAsync(stream);
    }

    public void Move( string    path ) { Info.MoveTo(path); }
    public void Move( LocalFile file ) { Info.MoveTo(file.FullPath); }


    public string Hash_MD5()    => Hash(MD5.Create());
    public string Hash_SHA1()   => Hash(SHA1.Create());
    public string Hash_SHA256() => Hash(SHA256.Create());
    public string Hash_SHA384() => Hash(SHA384.Create());
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


#region Openers

    public FileStream Create() => Info.Create();

    public FileStream Open( FileMode   mode,
                            FileAccess access,
                            FileShare  share,
                            int        bufferSize = 4096,
                            bool       useAsync   = true )
    {
        if ( string.IsNullOrWhiteSpace(FullPath) ) { throw new NullReferenceException(nameof(FullPath)); }


        return new FileStream(FullPath,
                              mode,
                              access,
                              share,
                              bufferSize,
                              useAsync);
    }


    public FileStream OpenRead( int bufferSize = 4096, bool useAsync = true ) => Open(FileMode.Open,
                                                                                      FileAccess.Read,
                                                                                      FileShare.Read,
                                                                                      bufferSize,
                                                                                      useAsync);


    public FileStream OpenWrite( FileMode mode, int bufferSize = 4096, bool useAsync = true ) => Open(mode,
                                                                                                      FileAccess.Write,
                                                                                                      FileShare.None,
                                                                                                      bufferSize,
                                                                                                      useAsync);

#endregion


#region Read

    public async Task<T> ReadFromFileAsync<T>( Encoding? encoding = default ) where T : class
    {
        string content = await ReadFromFileAsync(encoding);

        return content.FromJson<T>();
    }

    public async Task<string> ReadFromFileAsync( Encoding? encoding = default )
    {
        using var stream = new StreamReader(OpenRead(), encoding ?? Encoding.Default);
        return await stream.ReadToEndAsync();
    }

    public string ReadFromFile( Encoding? encoding = default )
    {
        using var stream = new StreamReader(OpenRead(), encoding ?? Encoding.Default);
        return stream.ReadToEnd();
    }

    public ReadOnlySpan<char> ReadFromFileAsSpan( Encoding? encoding = default )
    {
        using var stream = new StreamReader(OpenRead(), encoding ?? Encoding.Default);
        return stream.ReadToEnd();
    }


    public async Task<byte[]> RawReadFromFileAsBytesAsync( CancellationToken token = default )
    {
        await using FileStream file   = OpenRead();
        await using var        stream = new MemoryStream();
        await file.CopyToAsync(stream, token);
        return stream.ToArray();
    }

    public async Task<ReadOnlyMemory<byte>> RawReadFromFileAsync( CancellationToken token = default )
    {
        ReadOnlyMemory<byte> results = await RawReadFromFileAsBytesAsync(token);
        return results;
    }

    public byte[] RawReadFromFileAsBytes()
    {
        using FileStream file   = OpenRead();
        using var        stream = new MemoryStream();
        file.CopyTo(stream);
        return stream.ToArray();
    }

    public ReadOnlyMemory<byte> RawReadFromFile()
    {
        ReadOnlyMemory<byte> results = RawReadFromFileAsBytes();
        return results;
    }

#endregion


#region Write

    public       void WriteToFile( StringBuilder      payload, Encoding? encoding = default, FileMode mode = FileMode.Create ) => WriteToFile(payload.ToString(), encoding, mode);
    public async Task WriteToFileAsync( StringBuilder payload, Encoding? encoding = default, FileMode mode = FileMode.Create ) => await WriteToFileAsync(payload.ToString(), encoding, mode);


    public void WriteToFile( string payload, Encoding? encoding = default, FileMode mode = FileMode.Create )
    {
        if ( string.IsNullOrWhiteSpace(payload) ) { throw new ArgumentNullException(nameof(payload)); }

        using FileStream stream = Create();
        using var        writer = new StreamWriter(stream, encoding ?? Encoding.Default);
        writer.Write(payload);
    }

    public async Task WriteToFileAsync( string payload, Encoding? encoding = default, FileMode mode = FileMode.Create )
    {
        if ( string.IsNullOrWhiteSpace(payload) ) { throw new ArgumentNullException(nameof(payload)); }

        await using FileStream stream = Create();
        await using var        writer = new StreamWriter(stream, encoding ?? Encoding.Default);
        await writer.WriteAsync(payload);
    }


    public void WriteToFile( ReadOnlySpan<byte> payload )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        using FileStream stream = Create();
        stream.Write(payload);
    }

    public async Task WriteToFileAsync( ReadOnlyMemory<char> payload, Encoding? encoding = default, FileMode mode = FileMode.Create )
    {
        await using FileStream stream = Create();
        await using var        writer = new StreamWriter(stream, encoding ?? Encoding.Default);
        await writer.WriteAsync(payload);
    }


    public void WriteToFile( byte[] payload )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        using FileStream stream = Create();
        stream.Write(payload, 0, payload.Length);
    }

    public async Task WriteToFileAsync( byte[] payload, CancellationToken token = default )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        await using FileStream stream = Create();
        await stream.WriteAsync(payload, token);
    }


    public async Task WriteToFileAsync( ReadOnlyMemory<byte> payload, CancellationToken token = default )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        await using FileStream stream = Create();
        await stream.WriteAsync(payload, token);
    }


    public void WriteToFile( Stream payload )
    {
        if ( payload is null ) { throw new ArgumentNullException(nameof(payload)); }

        using var memory = new MemoryStream();
        payload.CopyTo(memory);
        ReadOnlySpan<byte> data = memory.ToArray();
        WriteToFile(data);
    }

    public async Task WriteToFileAsync( Stream payload, CancellationToken token = default )
    {
        if ( payload is null ) { throw new ArgumentNullException(nameof(payload)); }

        await using var memory = new MemoryStream();
        await payload.CopyToAsync(memory, token);
        ReadOnlyMemory<byte> data = memory.ToArray();
        await WriteToFileAsync(data, token);
    }

#endregion


#region Statics

    /// <summary>
    /// Write the <paramref name="payload"/> to the file.
    /// </summary>
    /// <param name="payload">the data being written to the file</param>
    /// <param name="path"></param>
    /// <param name="token"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="WebException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns></returns>
    public static async Task<LocalFile> SaveFileAsync( string path, Stream payload, CancellationToken token = default )
    {
        var file = new LocalFile(path);
        await file.WriteToFileAsync(payload, token);
        return file;
    }


    /// <summary>
    /// Write the <paramref name="payload"/> to the file.
    /// </summary>
    /// <param name="payload">the data being written to the file</param>
    /// <param name="path"></param>
    /// <param name="token"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="WebException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns><see cref="LocalFile"/></returns>
    public static async Task<LocalFile> SaveFileAsync( string path, ReadOnlyMemory<byte> payload, CancellationToken token = default )
    {
        var file = new LocalFile(path);
        await file.WriteToFileAsync(payload, token);
        return file;
    }


    /// <summary>
    /// Downloads the content of the web url to the specified file.
    /// </summary>
    /// <param name="path">file path / location</param>
    /// <param name="uri">the link to download</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="WebException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns></returns>
    public static LocalFile SaveFile( string path, string uri ) => SaveFile(path, new Uri(uri));


    /// <summary>
    /// Downloads the content of the web url to the specified file.
    /// </summary>
    /// <param name="path">file path / location</param>
    /// <param name="uri">the link to download</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="WebException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns></returns>
    public static LocalFile SaveFile( string path, Uri uri )
    {
        if ( uri.IsFile ) { return new LocalFile(uri); }

        using var client = new WebClient();

        client.DownloadFile(uri, path);
        return new LocalFile(path);
    }


    /// <summary>
    /// Downloads the content of the web url to the specified file, asynchronously.
    /// </summary>
    /// <param name="path">file path / location</param>
    /// <param name="uri">the link to download</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="WebException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns></returns>
    public static Task<LocalFile> SaveFileAsync( string path, string uri ) => SaveFileAsync(path, new Uri(uri));


    /// <summary>
    /// Downloads the content of the web url to the specified file, asynchronously.
    /// </summary>
    /// <param name="path">file path / location</param>
    /// <param name="uri">the link to download</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="WebException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns></returns>
    public static async Task<LocalFile> SaveFileAsync( string path, Uri uri )
    {
        if ( uri.IsFile ) { return new LocalFile(uri); }

        var       file   = new LocalFile(path);
        using var client = new WebClient();
        await client.DownloadFileTaskAsync(uri, file.FullPath);
        return file;
    }

#endregion


    public override bool Equals( object other ) => other is LocalFile file && Equals(file);

    public override int GetHashCode() => HashCode.Combine(FullPath, this.IsTempFile());


    public bool Equals( LocalFile? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }


        return ( (TempFile.ITempFile)this ).IsTemporary == ( (TempFile.ITempFile)other ).IsTemporary && FullPath == other.FullPath;
    }


    public static bool operator ==( LocalFile left, LocalFile? right ) => left.Equals(right);

    public static bool operator !=( LocalFile left, LocalFile? right ) => !( left == right );


    private bool _isTemporary;

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

    protected virtual void Dispose( bool remove )
    {
        if ( remove && Exists ) { Delete(); }
    }
    

    public int CompareTo( LocalFile? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( ReferenceEquals(null, other) ) { return 1; }

        int isTemporaryComparison = _isTemporary.CompareTo(other._isTemporary);
        if ( isTemporaryComparison != 0 ) { return isTemporaryComparison; }

        return string.Compare(FullPath, other.FullPath, StringComparison.Ordinal);
    }



    [Serializable]
    public class Collection : ObservableCollection<LocalFile>
    {
        public Collection() : base() { }
        public Collection( IEnumerable<LocalFile> items ) : base(items) { }
        public Collection( LocalDirectory         directory ) : this(directory.GetFiles()) { }
    }



    [Serializable]
    public class Items : List<LocalFile>
    {
        public Items() : base() { }
        public Items( int                    capacity ) : base(capacity) { }
        public Items( IEnumerable<LocalFile> items ) : base(items) { }
        public Items( LocalDirectory         directory ) : this(directory.GetFiles()) { }
    }



    [Serializable]
    public class Set : HashSet<LocalFile>
    {
        public Set() : base() { }
        public Set( int                    capacity ) : base(capacity) { }
        public Set( IEnumerable<LocalFile> items ) : base(items) { }
        public Set( LocalDirectory         directory ) : this(directory.GetFiles()) { }
    }
}
