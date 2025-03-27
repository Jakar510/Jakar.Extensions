// unset

using AsyncAwaitBestPractices;


#pragma warning disable CS1066 // The default value specified will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
#pragma warning disable CS1584



namespace Jakar.Extensions;


[Serializable]
public class LocalFile : ObservableClass, IEquatable<LocalFile>, IComparable<LocalFile>, IComparable, TempFile.ITempFile, LocalFile.IReadHandler, LocalFile.IAsyncReadHandler
{
    private   bool      _isTemporary;
    protected FileInfo? _info;


    public static       Equalizer<LocalFile> Equalizer       { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Equalizer<LocalFile>.Default; }
    public static       Sorter<LocalFile>    Sorter          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Sorter<LocalFile>.Default; }
    public              string               ContentType     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Mime.ToContentType(); }
    public              string?              DirectoryName   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Info.DirectoryName; }
    public              bool                 DoesNotExist    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => !Exists; }
    public              bool                 Exists          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Info.Exists; }
    public              string               Extension       { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Info.Extension; }
    public              Encoding             FileEncoding    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; } = Encoding.Default;
    public              string               FullPath        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    [JsonIgnore] public FileInfo             Info            { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _info ??= new FileInfo( FullPath ); }
    bool TempFile.ITempFile.                 IsTemporary     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _isTemporary; set => _isTemporary = value; }
    public DateTimeOffset                    LastAccess      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Info.LastAccessTime; }
    public DateTimeOffset                    CreationTimeUtc { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Info.CreationTimeUtc; }
    public MimeType                          Mime            { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Extension.FromExtension(); }
    public string                            Name            { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Info.Name; }

    [JsonIgnore]
    public LocalDirectory? Parent
    {
        get
        {
            DirectoryInfo? parent = Directory.GetParent( FullPath );

            return parent is null
                       ? null
                       : new LocalDirectory( parent );
        }
    }
    public string Root { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Directory.GetDirectoryRoot( FullPath ); }


    public LocalFile() => FullPath = string.Empty;
    public LocalFile( Uri                       path ) : this( FromUri( path ) ) { }
    public LocalFile( params ReadOnlySpan<char> path ) : this( path.ToString() ) { }
    public LocalFile( FileInfo                  path ) : this( path.FullName ) { }
    public LocalFile( string                    path, params ReadOnlySpan<string> subFolders ) : this( path, Encoding.Default, subFolders ) { }
    public LocalFile( string                    path, Encoding?                   encoding, params ReadOnlySpan<string> subFolders ) : this( path.Combine( subFolders ), encoding ) { }
    public LocalFile( DirectoryInfo             path, string                      fileName ) : this( path.Combine( fileName ) ) { }
    public LocalFile( string                    path, string                      fileName ) : this( new DirectoryInfo( path ), fileName ) { }
    public LocalFile( string path, Encoding? encoding = null )
    {
        this.SetNormal();
        FullPath     = Path.GetFullPath( path );
        FileEncoding = encoding ?? Encoding.Default;
    }
    public void Dispose()
    {
        Dispose( this.IsTempFile() );
        GC.SuppressFinalize( this );
    }
    protected virtual void Dispose( bool remove )
    {
        if ( remove && Exists ) { Delete(); }
    }


    public static implicit operator LocalFile( string             info ) => new(info);
    public static implicit operator LocalFile( FileInfo           info ) => new(info);
    public static implicit operator LocalFile( Uri                info ) => new(info);
    public static implicit operator LocalFile( ReadOnlySpan<char> info ) => new(info);


    public static LocalFile Create( FileInfo file ) => file;
    public static LocalFile Create( string   file ) => new(file);
    public static LocalFile Create( Uri      file ) => new(file);


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    /// <summary> </summary>
    /// <param name="uri"> </param>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="PathTooLongException"> </exception>
    /// <exception cref="NotSupportedException"> </exception>
    /// <returns> </returns>
    protected static FileInfo FromUri( Uri uri )
    {
        if ( uri.IsFile is false ) { throw new ArgumentException( "Uri is not a file Uri.", nameof(uri) ); }

        return new FileInfo( uri.AbsolutePath );
    }

    /// <summary> </summary>
    /// <param name="path"> </param>
    /// <param name="file"> </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="PathTooLongException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="NotSupportedException"> </exception>
    /// <returns> </returns>
    public static FileStream CreateAndOpen( string path, out LocalFile file )
    {
        file = new LocalFile( path );
        return File.Create( path );
    }

    /// <summary> </summary>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="PathTooLongException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="NotSupportedException"> </exception>
    /// <returns> </returns>
    public static FileStream CreateTempFileAndOpen( out LocalFile file )
    {
        FileStream stream = CreateAndOpen( Path.GetTempFileName(), out file );
        file.SetTemporary();
        return stream;
    }

    /// <summary> </summary>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="PathTooLongException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="NotSupportedException"> </exception>
    /// <returns> </returns>
    public static FileStream CreateTempFileAndOpen( MimeType type, out LocalFile file )
    {
        ReadOnlySpan<char> ext  = type.ToExtensionWithPeriod();
        ReadOnlySpan<char> name = Path.GetRandomFileName();
        name = name[..name.IndexOf( '.' )];
        Span<char> span = stackalloc char[name.Length + ext.Length];
        name.CopyTo( span );
        ext.CopyTo( span[name.Length..] );

        string     path   = Path.Combine( Path.GetTempPath(), span.ToString() );
        FileStream stream = CreateAndOpen( path, out file );
        file.SetTemporary();
        return stream;
    }


    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <param name="path"> </param>
    /// <param name="token"> </param>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="WebException"> </exception>
    /// <exception cref="NotSupportedException"> </exception>
    /// <returns> </returns>
    public static async ValueTask<LocalFile> SaveToFileAsync( string path, Stream payload, CancellationToken token = default )
    {
        var file = new LocalFile( path );
        await file.WriteAsync( payload, token );
        return file;
    }


    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <param name="path"> </param>
    /// <param name="token"> </param>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="WebException"> </exception>
    /// <exception cref="NotSupportedException"> </exception>
    /// <returns>
    ///     <see cref="LocalFile"/>
    /// </returns>
    public static async ValueTask<LocalFile> SaveToFileAsync( string path, ReadOnlyMemory<byte> payload, CancellationToken token = default )
    {
        var file = new LocalFile( path );
        await file.WriteAsync( payload, token );
        return file;
    }


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public FileStream Create() => Info.Create();

    /// <summary>
    ///     <para>
    ///         <seealso href="https://stackoverflow.com/a/11541330/9530917"/>
    ///     </para>
    /// </summary>
    /// <param name="mode"> </param>
    /// <param name="access"> </param>
    /// <param name="share"> </param>
    /// <param name="bufferSize"> </param>
    /// <param name="useAsync"> </param>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="NotSupportedException"> </exception>
    /// <exception cref="ArgumentOutOfRangeException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="PathTooLongException"> </exception>
    /// <returns>
    ///     <see cref="FileStream"/>
    /// </returns>
    public FileStream Open( FileMode mode, FileAccess access, FileShare share, int bufferSize = 4096, bool useAsync = true )
    {
        if ( string.IsNullOrWhiteSpace( FullPath ) ) { throw new NullReferenceException( nameof(FullPath) ); }

        return new FileStream( FullPath, mode, access, share, bufferSize, useAsync );
    }


    /// <summary> Opens file for read only actions. If it doesn't exist, <see cref="FileNotFoundException"/> will be raised. </summary>
    /// <param name="bufferSize"> </param>
    /// <param name="useAsync"> </param>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="NotSupportedException"> </exception>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentOutOfRangeException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="PathTooLongException"> </exception>
    /// <returns>
    ///     <see cref="FileStream"/>
    /// </returns>
    public FileStream OpenRead( int bufferSize = 4096, bool useAsync = true ) => Open( FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync );
    /// <summary> Opens file for write only actions. If it doesn't exist, file will be created. </summary>
    /// <param name="mode"> </param>
    /// <param name="bufferSize"> </param>
    /// <param name="useAsync"> </param>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="NotSupportedException"> </exception>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentOutOfRangeException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="PathTooLongException"> </exception>
    /// <returns>
    ///     <see cref="FileStream"/>
    /// </returns>
    public FileStream OpenWrite( FileMode mode, int bufferSize = 4096, bool useAsync = true ) => Open( mode, FileAccess.Write, FileShare.None, bufferSize, useAsync );


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public          IAsyncReadHandler ReadAsync()   => this;
    public override int               GetHashCode() => HashCode.Combine( FullPath, this.IsTempFile() );


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
    // public async ValueTask<string> ReadAsStringAsync()
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
    // public TValue Read<TValue>()
    // {
    //     string content = ReadAsString();
    //     return content.FromJson<TValue>();
    //
    //     /// <summary>
    //     /// Reads the contents of the file as a <see cref="string"/>, then calls <see cref="JsonExtensions.FromJson{TResult}(string)"/> on it, asynchronously.
    //     /// </summary>
    //     /// <typeparam name="TValue"></typeparam>
    //     /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    //     /// <exception cref="FileNotFoundException">if file is not found</exception>
    //     /// <exception cref="JsonReaderException">if an error  deserialization occurs</exception>
    //     /// <returns><typeparamref name="TValue"/></returns>
    // }
    // /// <summary>
    // /// Reads the contents of the file as a <see cref="string"/>, then calls <see cref="JsonExtensions.FromJson{TResult}(string)"/> on it, asynchronously.
    // /// </summary>
    // /// <typeparam name="TValue"></typeparam>
    // /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    // /// <exception cref="FileNotFoundException">if file is not found</exception>
    // /// <exception cref="JsonReaderException">if an error  deserialization occurs</exception>
    // /// <returns><typeparamref name="TValue"/></returns>
    // public async ValueTask<TValue> ReadAsync<TValue>()
    // {
    //     string content = await ReadAsStringAsync();
    //     return content.FromJson<TValue>();
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
    //     return stream.GetBuffer();
    // }
    // /// <summary>
    // /// Reads the contents of the file as a byte array.
    // /// </summary>
    // /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    // /// <exception cref="FileNotFoundException">if file is not found</exception>
    // /// <returns><see cref="byte[]"/></returns>
    // public async ValueTask<byte[]> ReadAsBytesAsync( CancellationToken token  = default )
    // {
    //     await using FileStream file   = OpenRead();
    //     await using var        stream = new MemoryStream();
    //     await file.CopyToAsync(stream, token);
    //     return stream.GetBuffer();
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
    // public async ValueTask<ReadOnlyMemory<byte>> ReadAsMemoryAsync( CancellationToken token  = default )
    // {
    //     ReadOnlyMemory<byte> results = await ReadAsBytesAsync(token);
    //     return results;
    // }
    // public async ValueTask<MemoryStream> ReadAsStreamAsync( CancellationToken token  = default )
    // {
    //     await using FileStream file   = OpenRead();
    //     var                    stream = new MemoryStream();
    //     await file.CopyToAsync(stream, token);
    //     return stream;
    // }


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public IReadHandler Read() => this;


    /// <summary> Changes the extension of the file. </summary>
    /// <param name="ext"> </param>
    /// <returns> The modified path information. On Windows, if ext is null or empty, the path information is unchanged. If the extension is null, the returned path is with the extension removed. If the path has no extension, and the extension is not null, the returned string contains the extension appended to the end of the path. </returns>
    public LocalFile ChangeExtension( MimeType ext ) => ChangeExtension( ext.ToExtension() );

    /// <summary> Changes the extension of the file. </summary>
    /// <param name="ext"> </param>
    /// <returns> The modified path information. On Windows, if ext is null or empty, the path information is unchanged. If the extension is null, the returned path is with the extension removed. If the path has no extension, and the extension is not null, the returned string contains the extension appended to the end of the path. </returns>
    public LocalFile ChangeExtension( string? ext ) => Path.ChangeExtension( FullPath, ext );


    protected string Hash( HashAlgorithm hasher )
    {
        using ( hasher )
        {
            using FileStream stream = OpenRead();
            byte[]           hash   = hasher.ComputeHash( stream );
            return BitConverter.ToString( hash );
        }
    }


    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public string Hash_MD5() => Hash( MD5.Create() );
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public string Hash_SHA1() => Hash( SHA1.Create() );
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public string Hash_SHA256() => Hash( SHA256.Create() );
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public string Hash_SHA384() => Hash( SHA384.Create() );
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public string Hash_SHA512() => Hash( SHA512.Create() );


    public sealed override string ToString() => FullPath;


    /// <summary> Creates an <see cref="UriKind.Absolute"/> based on the detected <see cref="Mime"/> </summary>
    /// <param name="mime"> To override the detected <see cref="Mime"/> , provide a non-null value </param>
    /// <returns> </returns>
    public Uri ToUri( MimeType? mime = null )
    {
        if ( string.IsNullOrWhiteSpace( FullPath ) ) { throw new NullReferenceException( nameof(FullPath) ); }

        MimeType type = mime ?? Mime;

        if ( !FullPath.StartsWith( "/", StringComparison.InvariantCultureIgnoreCase ) ) { return new Uri( $"{type.ToUriScheme()}://{FullPath}", UriKind.Absolute ); }

        string path = FullPath.Remove( 0, 1 );
        return new Uri( $"{type.ToUriScheme()}://{path}", UriKind.Absolute );
    }

    /// <summary> Creates a <see cref="Uri"/> using provided prefix, and <see cref="HttpUtility.UrlEncode(string)"/> to encode the <see cref="FullPath"/> . </summary>
    /// <param name="baseUri"> </param>
    /// <param name="prefix"> The key to attach the <see cref="FullPath"/> to. Defaults to "?path=" </param>
    /// <param name="mime"> To override the detected <see cref="Mime"/> , provide a non-null value </param>
    /// <returns> </returns>
    public Uri ToUri( Uri baseUri, string prefix = "?path=", MimeType? mime = null )
    {
        if ( string.IsNullOrWhiteSpace( FullPath ) ) { throw new NullReferenceException( nameof(FullPath) ); }

        MimeType type = mime ?? Mime;

        if ( !FullPath.StartsWith( "/", StringComparison.InvariantCultureIgnoreCase ) ) { return new Uri( $"{type.ToUriScheme()}://{FullPath}", UriKind.Absolute ); }

        return new Uri( baseUri, $"{prefix}{HttpUtility.UrlEncode( FullPath )}" );
    }


    /// <summary> Copies this file to the <paramref name="newFile"/> </summary>
    /// <param name="newFile"> </param>
    /// <param name="token"> </param>
    /// <returns> </returns>
    public async ValueTask Clone( LocalFile newFile, CancellationToken token = default )
    {
        FileStream stream = OpenRead();
        await newFile.WriteAsync( stream, token );
    }


    public async ValueTask<LocalFile> ZipAsync( CancellationToken token, ReadOnlyMemory<LocalFile> files )
    {
        await using FileStream zipToOpen = File.Create( FullPath );
        using ZipArchive       archive   = new(zipToOpen, ZipArchiveMode.Update);

        for ( int i = 0; i < files.Length; i++ )
        {
            LocalFile            file   = files.Span[i];
            ZipArchiveEntry      entry  = archive.CreateEntry( file.FullPath );
            await using Stream   stream = entry.Open();
            ReadOnlyMemory<byte> data   = await file.ReadAsync().AsMemory( token );

            await stream.WriteAsync( data, token );
        }

        return this;
    }
    public ValueTask<LocalFile> ZipAsync( IEnumerable<string> files, CancellationToken token ) => ZipAsync( files.Select( static item => new LocalFile( item ) ), token );
    public async ValueTask<LocalFile> ZipAsync( IEnumerable<LocalFile> files, CancellationToken token = default )
    {
        await using FileStream zipToOpen = File.Create( FullPath );
        using ZipArchive       archive   = new(zipToOpen, ZipArchiveMode.Update);

        foreach ( LocalFile file in files )
        {
            ZipArchiveEntry      entry  = archive.CreateEntry( file.FullPath );
            await using Stream   stream = entry.Open();
            ReadOnlyMemory<byte> data   = await file.ReadAsync().AsMemory( token );

            await stream.WriteAsync( data, token );
        }

        return this;
    }


    /// <summary> Decrypts a file that was encrypted by the current account using the <see cref="File.Encrypt(string)"/> method. </summary>
    public void Decrypt()
    {
        if ( OperatingSystem.IsWindows() ) { File.Decrypt( FullPath ); }

        throw new InvalidOperationException( "Windows Only" );
    }

    /// <summary> Encrypts the file so that only the account used to encrypt the file can decrypt it. </summary>
    public void Encrypt()
    {
        if ( OperatingSystem.IsWindows() ) { File.Encrypt( FullPath ); }

        throw new InvalidOperationException( "Windows Only" );
    }


    /// <summary> Permanently deletes a file. </summary>
    public void Delete() => Info.Delete();


    /// <summary> Moves this file to the new <paramref name="path"/> </summary>
    /// <param name="path"> </param>
    public void Move( string path ) => Info.MoveTo( path );

    /// <summary> Moves this file to the new <paramref name="file"/> location </summary>
    /// <param name="file"> </param>
    public void Move( LocalFile file ) => Info.MoveTo( file.FullPath );


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public void Write( StringBuilder payload ) => Write( payload.ToString() );

    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public void Write( ValueStringBuilder payload ) => Write( payload.ToString() );

    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public void Write( string payload )
    {
        if ( string.IsNullOrWhiteSpace( payload ) ) { throw new ArgumentNullException( nameof(payload) ); }

        using FileStream stream = Create();
        using var        writer = new StreamWriter( stream, FileEncoding );
        writer.Write( payload );
    }

    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public void Write( byte[] payload )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException( @"payload.Length == 0", nameof(payload) ); }

        using FileStream stream = Create();
        stream.Write( payload, 0, payload.Length );
    }

    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public void Write( Span<byte> payload )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException( @"payload.Length == 0", nameof(payload) ); }

        using FileStream stream = Create();
        stream.Write( payload );
    }

    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public void Write( ReadOnlySpan<byte> payload )
    {
        if ( payload.IsEmpty ) { throw new ArgumentException( @"payload.Length == 0", nameof(payload) ); }

        using FileStream stream = Create();
        stream.Write( payload );
    }

    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public void Write( ReadOnlyMemory<byte> payload ) => Write( payload.Span );

    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public void Write( ReadOnlyMemory<char> payload )
    {
        if ( payload.IsEmpty ) { throw new ArgumentException( @"payload.Length == 0", nameof(payload) ); }

        using FileStream stream = Create();
        using var        writer = new StreamWriter( stream, FileEncoding );
        writer.Write( payload );
    }

    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public void Write( Stream payload )
    {
        if ( payload is null ) { throw new ArgumentNullException( nameof(payload) ); }

        using var memory = new MemoryStream();
        payload.CopyTo( memory );
        ReadOnlySpan<byte> data = memory.GetBuffer();
        Write( data );
    }


    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public ValueTask WriteAsync( StringBuilder payload ) => WriteAsync( payload.ToString() );

    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public ValueTask WriteAsync( ValueStringBuilder payload ) => WriteAsync( payload.ToString() );

    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public async ValueTask WriteAsync( string payload )
    {
        if ( string.IsNullOrWhiteSpace( payload ) ) { throw new ArgumentNullException( nameof(payload) ); }

        await using FileStream stream = Create();
        await using var        writer = new StreamWriter( stream, FileEncoding );
        await writer.WriteAsync( payload );
    }

    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <param name="token"> </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public async ValueTask WriteAsync( byte[] payload, CancellationToken token = default )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException( @"payload.Length == 0", nameof(payload) ); }

        await using FileStream stream = Create();
        await stream.WriteAsync( payload, token );
    }

    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <param name="token"> </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public async ValueTask WriteAsync( ReadOnlyMemory<byte> payload, CancellationToken token = default )
    {
        if ( payload.Length == 0 ) { throw new ArgumentException( @"payload.Length == 0", nameof(payload) ); }

        await using FileStream stream = Create();
        await stream.WriteAsync( payload, token );
    }

    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <param name="token"> </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public async ValueTask WriteAsync( ReadOnlyMemory<char> payload, CancellationToken token = default )
    {
        await using FileStream stream = Create();
        await using var        writer = new StreamWriter( stream, FileEncoding );

        await writer.WriteAsync( payload, token );
    }

    /// <summary> Write the <paramref name="payload"/> to the file. </summary>
    /// <param name="payload"> the data being written to the file </param>
    /// <param name="token"> </param>
    /// <exception cref="NullReferenceException"> </exception>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <returns>
    ///     <see cref="ValueTask"/>
    /// </returns>
    public async ValueTask WriteAsync( Stream payload, CancellationToken token = default )
    {
        if ( payload is null ) { throw new ArgumentNullException( nameof(payload) ); }

        await using FileStream stream = Create();
        await payload.CopyToAsync( stream, token );
    }


    async ValueTask<string> IAsyncReadHandler.AsString( CancellationToken token = default )
    {
        await using FileStream file   = OpenRead();
        using var              stream = new StreamReader( file, FileEncoding );
        return await stream.ReadToEndAsync( token );
    }

    async ValueTask<TValue> IAsyncReadHandler.AsJson<TValue>()
    {
        using var stream  = new StreamReader( OpenRead(), FileEncoding );
        string    content = await stream.ReadToEndAsync();
        return content.FromJson<TValue>();
    }

    async ValueTask<byte[]> IAsyncReadHandler.AsBytes( CancellationToken token = default )
    {
        await using FileStream file   = OpenRead();
        await using var        stream = new MemoryStream();
        await file.CopyToAsync( stream, token );
        return stream.GetBuffer();
    }

    async ValueTask<ReadOnlyMemory<byte>> IAsyncReadHandler.AsMemory( CancellationToken token = default )
    {
        await using FileStream file   = OpenRead();
        await using var        stream = new MemoryStream( (int)file.Length );
        await file.CopyToAsync( stream, token );
        ReadOnlyMemory<byte> results = stream.GetBuffer();
        return results;
    }

    async ValueTask<MemoryStream> IAsyncReadHandler.AsStream( CancellationToken token = default )
    {
        await using FileStream file   = OpenRead();
        var                    stream = new MemoryStream();
        await file.CopyToAsync( stream, token );
        return stream;
    }

    async IAsyncEnumerable<string> IAsyncReadHandler.AsLines( [EnumeratorCancellation] CancellationToken token = default )
    {
        await using FileStream file   = OpenRead();
        using StreamReader     stream = new(file, FileEncoding);

        while ( token.ShouldContinue() && stream.EndOfStream is false ) { yield return await stream.ReadLineAsync( token ) ?? string.Empty; }
    }


    TValue IReadHandler.AsJson<TValue>()
    {
        using var stream  = new StreamReader( OpenRead(), FileEncoding );
        string    content = stream.ReadToEnd();
        return content.FromJson<TValue>();
    }

    string IReadHandler.AsString()
    {
        using var stream = new StreamReader( OpenRead(), FileEncoding );
        return stream.ReadToEnd();
    }

    byte[] IReadHandler.AsBytes()
    {
        using FileStream file   = OpenRead();
        using var        stream = new MemoryStream();
        file.CopyTo( stream );
        return stream.GetBuffer();
    }

    ReadOnlyMemory<byte> IReadHandler.AsMemory()
    {
        IReadHandler handler = this;
        return handler.AsMemory();
    }
    [MustDisposeResource]
    Buffer<byte> IReadHandler.AsSpan()
    {
        using FileStream file   = OpenRead();
        int              length = (int)file.Length;
        Span<byte>       span   = stackalloc byte[length];
        length = file.Read( span );

        span = span[..length];
        return new Buffer<byte>( span );
    }


    public override bool Equals( object? other ) => other is LocalFile file && Equals( file );
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        return other is LocalFile value
                   ? CompareTo( value )
                   : throw new ExpectedValueTypeException( nameof(other), other, typeof(LocalFile) );
    }
    public int CompareTo( LocalFile? other )
    {
        if ( ReferenceEquals( this, other ) ) { return 0; }

        if ( ReferenceEquals( null, other ) ) { return 1; }

        return string.Compare( FullPath, other.FullPath, StringComparison.Ordinal );
    }
    public bool Equals( LocalFile? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return this.IsTempFile() == other.IsTempFile() && FullPath == other.FullPath;
    }


    public static bool operator ==( LocalFile? left, LocalFile? right ) => Equalizer.Equals( left, right );
    public static bool operator >( LocalFile?  left, LocalFile? right ) => Sorter.Compare( left, right ) > 0;
    public static bool operator >=( LocalFile? left, LocalFile? right ) => Sorter.Compare( left, right ) >= 0;
    public static bool operator !=( LocalFile? left, LocalFile? right ) => !Equalizer.Equals( left, right );
    public static bool operator <( LocalFile?  left, LocalFile? right ) => Sorter.Compare( left, right ) < 0;
    public static bool operator <=( LocalFile? left, LocalFile? right ) => Sorter.Compare( left, right ) <= 0;



    [Serializable]
    public class Collection : ObservableCollection<LocalFile>
    {
        public Collection() : base() { }
        public Collection( IEnumerable<LocalFile> items ) : base( items ) { }
    }



    [Serializable]
    public class ConcurrentCollection : ConcurrentObservableCollection<LocalFile>
    {
        public ConcurrentCollection() : base( Sorter ) { }
        public ConcurrentCollection( IEnumerable<LocalFile> items ) : base( items, Sorter ) { }
    }



    [Serializable]
    public class ConcurrentQueue : ConcurrentQueue<LocalFile>
    {
        public ConcurrentQueue() : base() { }
        public ConcurrentQueue( IEnumerable<LocalFile> items ) : base( items ) { }
    }



    // ---------------------------------------------------------------------------------------------------------------------------------------------------



    [Serializable]
    public class Deque : ConcurrentDeque<LocalFile>
    {
        public Deque() : base() { }
        public Deque( IEnumerable<LocalFile> items ) : base( items ) { }
    }



    public interface IAsyncReadHandler
    {
        IAsyncEnumerable<string> AsLines( CancellationToken token = default );
        /// <summary> Reads the contents of the file as a byte array. </summary>
        /// <exception cref="NullReferenceException"> if FullPath is null or empty </exception>
        /// <exception cref="FileNotFoundException"> if file is not found </exception>
        /// <returns>
        ///     <see cref="byte[]"/>
        /// </returns>
        ValueTask<byte[]> AsBytes( CancellationToken token = default );

        ValueTask<MemoryStream> AsStream( CancellationToken token = default );

        /// <summary> Reads the contents of the file as a <see cref="ReadOnlyMemory{byte}"/> , asynchronously. </summary>
        /// <param name="token"> </param>
        /// <exception cref="NullReferenceException"> if FullPath is null or empty </exception>
        /// <exception cref="FileNotFoundException"> if file is not found </exception>
        /// <returns>
        ///     <see cref="ReadOnlyMemory{byte}"/>
        /// </returns>
        ValueTask<ReadOnlyMemory<byte>> AsMemory( CancellationToken token = default );

        /// <summary> Reads the contents of the file as a <see cref="string"/> , asynchronously. </summary>
        /// <exception cref="NullReferenceException"> if FullPath is null or empty </exception>
        /// <exception cref="FileNotFoundException"> if file is not found </exception>
        /// <returns>
        ///     <see cref="string"/>
        /// </returns>
        ValueTask<string> AsString( CancellationToken token = default );

        /// <summary> Reads the contents of the file as a <see cref="string"/> , then calls <see cref="JsonNet.FromJson(string)"/> on it, asynchronously. </summary>
        /// <typeparam name="TValue"> </typeparam>
        /// <exception cref="NullReferenceException"> if FullPath is null or empty </exception>
        /// <exception cref="FileNotFoundException"> if file is not found </exception>
        /// <exception cref="JsonReaderException"> if an error  deserialization occurs </exception>
        /// <returns>
        ///     <typeparamref name="TValue"/>
        /// </returns>
        ValueTask<TValue> AsJson<TValue>();
    }



    public interface IReadHandler
    {
        /// <summary> Reads the contents of the file as a byte array. </summary>
        /// <exception cref="NullReferenceException"> if FullPath is null or empty </exception>
        /// <exception cref="FileNotFoundException"> if file is not found </exception>
        /// <returns>
        ///     <see cref="byte[]"/>
        /// </returns>
        byte[] AsBytes();

        /// <summary> Reads the contents of the file as a <see cref="ReadOnlyMemory{byte}"/> . </summary>
        /// <exception cref="NullReferenceException"> if FullPath is null or empty </exception>
        /// <exception cref="FileNotFoundException"> if file is not found </exception>
        /// <returns>
        ///     <see cref="ReadOnlyMemory{byte}"/>
        /// </returns>
        ReadOnlyMemory<byte> AsMemory();

        /// <summary> Reads the contents of the file as a <see cref="ReadOnlySpan{byte}"/> . </summary>
        /// <exception cref="NullReferenceException"> if FullPath is null or empty </exception>
        /// <exception cref="FileNotFoundException"> if file is not found </exception>
        /// <returns>
        ///     <see cref="ReadOnlySpan{byte}"/>
        /// </returns>
        [MustDisposeResource]
        Buffer<byte> AsSpan();

        /// <summary> Reads the contents of the file as a <see cref="string"/> . </summary>
        /// <exception cref="NullReferenceException"> if FullPath is null or empty </exception>
        /// <exception cref="FileNotFoundException"> if file is not found </exception>
        /// '
        /// <returns>
        ///     <see cref="string"/>
        /// </returns>
        string AsString();

        /// <summary> Reads the contents of the file as a <see cref="string"/> . </summary>
        /// <exception cref="NullReferenceException"> if FullPath is null or empty </exception>
        /// <exception cref="FileNotFoundException"> if file is not found </exception>
        /// <returns>
        ///     <see cref="string"/>
        /// </returns>
        TValue AsJson<TValue>();
    }



    [Serializable]
    public class Files : List<LocalFile>
    {
        public Files() : base() { }
        public Files( int                    capacity ) : base( capacity ) { }
        public Files( IEnumerable<LocalFile> items ) : base( items ) { }
    }



    [Serializable]
    public class Queue : Queue<LocalFile>
    {
        public Queue() : base() { }
        public Queue( IEnumerable<LocalFile> items ) : base( items ) { }
    }



    [Serializable]
    public class Set : HashSet<LocalFile>
    {
        public Set() : base() { }
        public Set( int                    capacity ) : base( capacity ) { }
        public Set( IEnumerable<LocalFile> items ) : base( items ) { }
    }



    /// <summary> A collection of files that are  the <see cref="LocalDirectory"/> </summary>
    [Serializable]
    public class Watcher : ConcurrentObservableCollection<LocalFile>
    {
        private readonly WeakEventManager<ErrorEventArgs> _errorEventManager = new();
        private readonly LocalDirectory.Watcher           _watcher;


        public event ErrorEventHandler? Error { add => _errorEventManager.AddEventHandler( x => value?.Invoke( this, x ) ); remove => _errorEventManager.RemoveEventHandler( x => value?.Invoke( this, x ) ); }


        public Watcher( LocalDirectory.Watcher watcher ) : base( watcher.Directory.GetFiles(), Sorter )
        {
            _watcher                     =  watcher;
            _watcher.Created             += OnCreated;
            _watcher.Changed             += OnChanged;
            _watcher.Deleted             += OnDeleted;
            _watcher.Renamed             += OnRenamed;
            _watcher.Error               += OnError;
            _watcher.EnableRaisingEvents =  true;
        }
        private void OnChanged( object sender, FileSystemEventArgs e )
        {
            LocalFile file = new(e.FullPath);
            AddOrUpdate( file );
        }
        private void OnCreated( object sender, FileSystemEventArgs e ) => Add( e.FullPath );
        private void OnDeleted( object sender, FileSystemEventArgs e ) => Remove( e.FullPath );
        private void OnError( object   sender, ErrorEventArgs      e ) => _errorEventManager.RaiseEvent( e, nameof(Error) );
        private void OnRenamed( object sender, RenamedEventArgs e )
        {
            LocalFile? file = Values.FirstOrDefault( x => x.FullPath == e.OldFullPath );
            if ( file is not null ) { Remove( file ); }

            Add( e.FullPath );
        }


        public override void Dispose()
        {
            base.Dispose();
            _watcher.EnableRaisingEvents = false;

            _watcher.Created -= OnCreated;
            _watcher.Changed -= OnChanged;
            _watcher.Deleted -= OnDeleted;
            _watcher.Renamed -= OnRenamed;
            _watcher.Error   -= OnError;

            _watcher.Dispose();
            GC.SuppressFinalize( this );
        }
    }



    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public ValueTask<string> HashAsync_MD5() => HashAsync( MD5.Create() );
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public ValueTask<string> HashAsync_SHA1() => HashAsync( SHA1.Create() );
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public ValueTask<string> HashAsync_SHA256() => HashAsync( SHA256.Create() );
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public ValueTask<string> HashAsync_SHA384() => HashAsync( SHA384.Create() );
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public ValueTask<string> HashAsync_SHA512() => HashAsync( SHA512.Create() );


    public async ValueTask<string> HashAsync( HashAlgorithm hasher )
    {
        using ( hasher )
        {
            await using FileStream stream = OpenRead();
            byte[]                 hash   = await hasher.ComputeHashAsync( stream );

            return BitConverter.ToString( hash );
        }
    }
}
