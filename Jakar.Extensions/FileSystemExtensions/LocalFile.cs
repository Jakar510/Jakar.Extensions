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


    public           string   FullPath      { get; init; }
    public           string   Name          => Info.Name;
    public           string   Extension     => Info.Extension;
    public           bool     Exists        => Info.Exists;
    public           string?  DirectoryName => Info.DirectoryName;
    public           MimeType Mime          => Extension.FromExtension();
    public           string   ContentType   => Mime.ToContentType();
    public           string?  Root          => Directory.GetDirectoryRoot(FullPath);
    private readonly Encoding _encoding;


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
    public LocalFile( Uri                path, Encoding?       encoding = default ) : this(FromUri(path), encoding) { }
    public LocalFile( ReadOnlySpan<char> path, Encoding?       encoding = default ) : this(path.ToString(), encoding) { }
    public LocalFile( FileInfo           path, Encoding?       encoding = default ) : this(path.FullName, encoding) { }
    public LocalFile( string             path, params string[] args ) : this(path, Encoding.Default, args) { }
    public LocalFile( string             path, Encoding?       encoding, params string[] args ) : this(path.Combine(args), encoding) { }
    public LocalFile( DirectoryInfo      path, string          fileName, Encoding?       encoding = default ) : this(path.Combine(fileName), encoding) { }
    public LocalFile( string             path, string          fileName, Encoding?       encoding = default ) : this(new DirectoryInfo(path), fileName, encoding) { }

    public LocalFile( string path, Encoding? encoding = default )
    {
        this.SetNormal();
        FullPath  = Path.GetFullPath(path);
        _encoding = encoding ?? Encoding.Default;
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


    public async Task Clone( LocalFile newFile, CancellationToken token )
    {
        FileStream stream = OpenRead();
        await newFile.WriteAsync(stream, token);
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


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public FileStream Create() => Info.Create();

    public FileStream Open( FileMode mode, FileAccess access, FileShare share, int bufferSize = 4096, bool useAsync = true )
    {
        if ( string.IsNullOrWhiteSpace(FullPath) ) { throw new NullReferenceException(nameof(FullPath)); }

        return new FileStream(FullPath, mode, access, share, bufferSize, useAsync);
    }


    public FileStream OpenRead( int       bufferSize = 4096, bool useAsync   = true )                       => Open(FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync);
    public FileStream OpenWrite( FileMode mode,              int  bufferSize = 4096, bool useAsync = true ) => Open(mode, FileAccess.Write, FileShare.None, bufferSize, useAsync);


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public string ReadAsString()
    {
        using var stream = new StreamReader(OpenRead(), _encoding);
        return stream.ReadToEnd();
    }
    public async Task<string> ReadAsStringAsync()
    {
        using var stream = new StreamReader(OpenRead(), _encoding);
        return await stream.ReadToEndAsync();
    }

    public T Read<T>()
    {
        string content = ReadAsString();
        return content.FromJson<T>();
    }
    public async Task<T> ReadAsync<T>()
    {
        string content = await ReadAsStringAsync();
        return content.FromJson<T>();
    }


    public ReadOnlySpan<char> ReadAsSpan()
    {
        using var stream = new StreamReader(OpenRead(), _encoding);
        return stream.ReadToEnd();
    }


    public byte[] ReadAsBytes()
    {
        using FileStream file   = OpenRead();
        using var        stream = new MemoryStream();
        file.CopyTo(stream);
        return stream.ToArray();
    }
    public async Task<byte[]> ReadAsBytesAsync( CancellationToken token )
    {
        await using FileStream file   = OpenRead();
        await using var        stream = new MemoryStream();
        await file.CopyToAsync(stream, token);
        return stream.ToArray();
    }


    public ReadOnlyMemory<byte> ReadAsMemory()
    {
        ReadOnlyMemory<byte> results = ReadAsBytes();
        return results;
    }
    public async Task<ReadOnlyMemory<byte>> ReadAsMemoryAsync( CancellationToken token )
    {
        ReadOnlyMemory<byte> results = await ReadAsBytesAsync(token);
        return results;
    }


    public async Task<MemoryStream> ReadAsStreamAsync( CancellationToken token )
    {
        await using FileStream file   = OpenRead();
        var                    stream = new MemoryStream();
        await file.CopyToAsync(stream, token);
        return stream;
    }


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public       void Write( StringBuilder      payload ) => Write(payload.ToString());
    public async Task WriteAsync( StringBuilder payload ) => await WriteAsync(payload.ToString());


    public void Write( string payload )
    {
        if ( string.IsNullOrWhiteSpace(payload) ) { throw new ArgumentNullException(nameof(payload)); }

        using FileStream stream = Create();
        using var        writer = new StreamWriter(stream, _encoding);
        writer.Write(payload);
    }
    public async Task WriteAsync( string payload )
    {
        if ( string.IsNullOrWhiteSpace(payload) ) { throw new ArgumentNullException(nameof(payload)); }

        await using FileStream stream = Create();
        await using var        writer = new StreamWriter(stream, _encoding);
        await writer.WriteAsync(payload);
    }


    public void Write( byte[] payload )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        using FileStream stream = Create();
        stream.Write(payload, 0, payload.Length);
    }
    public async Task WriteAsync( byte[] payload, CancellationToken token )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        await using FileStream stream = Create();
        await stream.WriteAsync(payload, token);
    }


    public void Write( ReadOnlySpan<byte> payload )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        using FileStream stream = Create();
        stream.Write(payload);
    }
    public async Task WriteAsync( ReadOnlyMemory<byte> payload, CancellationToken token )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        await using FileStream stream = Create();
        await stream.WriteAsync(payload, token);
    }


    public void Write( ReadOnlySpan<char> payload )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException(@"payload.Length == 0", nameof(payload)); }

        using FileStream stream = Create();
        using var        writer = new StreamWriter(stream, _encoding);
        writer.Write(payload);
    }
    public async Task WriteAsync( ReadOnlyMemory<char> payload, CancellationToken token )
    {
        await using FileStream stream = Create();
        await using var        writer = new StreamWriter(stream, _encoding);
        await writer.WriteAsync(payload, token);
    }


    public void Write( Stream payload )
    {
        if ( payload is null ) { throw new ArgumentNullException(nameof(payload)); }

        using var memory = new MemoryStream();
        payload.CopyTo(memory);
        ReadOnlySpan<byte> data = memory.ToArray();
        Write(data);
    }

    public async Task WriteAsync( Stream payload, CancellationToken token )
    {
        if ( payload is null ) { throw new ArgumentNullException(nameof(payload)); }

        await using var memory = new MemoryStream();
        await payload.CopyToAsync(memory, token);
        ReadOnlyMemory<byte> data = memory.ToArray();
        await WriteAsync(data, token);
    }


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


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
    public static async Task<LocalFile> SaveToFileAsync( string path, Stream payload, CancellationToken token )
    {
        var file = new LocalFile(path);
        await file.WriteAsync(payload, token);
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
    public static async Task<LocalFile> SaveToFileAsync( string path, ReadOnlyMemory<byte> payload, CancellationToken token )
    {
        var file = new LocalFile(path);
        await file.WriteAsync(payload, token);
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
    public static LocalFile SaveToFile( string path, string uri ) => SaveToFile(path, new Uri(uri));


    /// <summary>
    /// Downloads the content of the web url to the specified file.
    /// </summary>
    /// <param name="path">file path / location</param>
    /// <param name="uri">the link to download</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="WebException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns></returns>
    public static LocalFile SaveToFile( string path, Uri uri )
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
    public static Task<LocalFile> SaveToFileAsync( string path, string uri ) => SaveToFileAsync(path, new Uri(uri));


    /// <summary>
    /// Downloads the content of the web url to the specified file, asynchronously.
    /// </summary>
    /// <param name="path">file path / location</param>
    /// <param name="uri">the link to download</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="WebException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns></returns>
    public static async Task<LocalFile> SaveToFileAsync( string path, Uri uri )
    {
        if ( uri.IsFile ) { return new LocalFile(uri); }

        var       file   = new LocalFile(path);
        using var client = new WebClient();
        await client.DownloadFileTaskAsync(uri, file.FullPath);
        return file;
    }


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


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


    // ---------------------------------------------------------------------------------------------------------------------------------------------------



    [Serializable]
    public class Collection : ObservableCollection<LocalFile>
    {
        public Collection() : base() { }
        public Collection( IEnumerable<LocalFile> items ) : base(items) { }
        public Collection( LocalDirectory         directory ) : this(directory.GetFiles()) { }
    }



    [Serializable]
    public class Watcher : Collection, IDisposable
    {
        public event ErrorEventHandler?         Error;
        private readonly LocalDirectory.Watcher _watcher;


        public Watcher( LocalDirectory.Watcher watcher ) : base(watcher.Directory)
        {
            _watcher         =  watcher;
            _watcher.Created += OnCreated;
            _watcher.Changed += OnChanged;
            _watcher.Deleted += OnDeleted;
            _watcher.Renamed += OnRenamed;
            _watcher.Error   += OnError;

            _watcher.EnableRaisingEvents = true;
        }


        private void OnRenamed( object sender, RenamedEventArgs e )
        {
            LocalFile? file = this.FirstOrDefault(x => x.FullPath == e.OldFullPath);
            if ( file is not null ) { Remove(file); }

            Add(e.FullPath);
        }
        private void OnDeleted( object sender, FileSystemEventArgs e ) => Remove(e.FullPath);
        private void OnChanged( object sender, FileSystemEventArgs e ) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new LocalFile(e.FullPath)));
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
