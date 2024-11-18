using System;



namespace Jakar.Extensions;


[Serializable]
public class LocalDirectory : ObservableClass, IEquatable<LocalDirectory>, IComparable<LocalDirectory>, IComparable, TempFile.ITempFile, IAsyncDisposable
{
    private   bool           _isTemporary;
    protected DirectoryInfo? _info;


    /// <summary> Gets or sets the application's fully qualified path of the current working directory. </summary>
    public static LocalDirectory CurrentDirectory { get => new(Environment.CurrentDirectory); set => Environment.CurrentDirectory = Path.GetFullPath( value.FullPath ); }
    public static       Equalizer<LocalDirectory> Equalizer         => Equalizer<LocalDirectory>.Default;
    public static       Sorter<LocalDirectory>    Sorter            => Sorter<LocalDirectory>.Default;
    public              DateTime                  CreationTimeUtc   { get => Directory.GetCreationTimeUtc( FullPath ); set => Directory.SetCreationTimeUtc( FullPath, value ); }
    public              bool                      DoesNotExist      => !Exists;
    public              bool                      Exists            => Info.Exists;
    public              string                    FullPath          { get; init; }
    [JsonIgnore] public DirectoryInfo             Info              => _info ??= new DirectoryInfo( FullPath );
    bool TempFile.ITempFile.                      IsTemporary       { get => _isTemporary;                               set => _isTemporary = value; }
    public              DateTime                  LastAccessTimeUtc { get => Directory.GetLastAccessTimeUtc( FullPath ); set => Directory.SetLastWriteTimeUtc( FullPath, value ); }
    public              DateTime                  LastWriteTimeUtc  { get => Directory.GetLastWriteTimeUtc( FullPath );  set => Directory.SetLastWriteTimeUtc( FullPath, value ); }
    public              string                    Name              => Info.Name;
    [JsonIgnore] public LocalDirectory?           Parent            => GetParent();
    public              string                    Root              => Directory.GetDirectoryRoot( FullPath );


    public LocalDirectory() => FullPath = string.Empty;
    public LocalDirectory( scoped in ReadOnlySpan<char> path ) : this( path.ToString() ) { }
    public LocalDirectory( DirectoryInfo                path ) : this( path.FullName ) { }
    public LocalDirectory( string                       path, params ReadOnlySpan<string> subFolders ) : this( path.Combine( subFolders ) ) { }
    public LocalDirectory( string                       path ) => FullPath = Path.GetFullPath( path );
    public void Dispose()
    {
        GC.SuppressFinalize( this );
        DisposeAsync().CallSynchronously();
    }
    public virtual async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize( this );
        if ( DoesNotExist || this.IsTempFile() is false ) { return; }

        await DeleteAllRecursivelyAsync();
    }


    public static implicit operator string( LocalDirectory                         directory ) => directory.FullPath;
    public static implicit operator DirectoryInfo( LocalDirectory                  directory ) => directory.Info;
    public static implicit operator ReadOnlySpan<char>( LocalDirectory             directory ) => directory.FullPath;
    public static implicit operator LocalDirectory( DirectoryInfo                  info )      => new(info);
    public static implicit operator LocalDirectory( ReadOnlySpan<char>             path )      => new(path);
    public static implicit operator LocalDirectory( string                         path )      => new(path);
    public static implicit operator Watcher( LocalDirectory                        directory ) => new(directory);
    public static implicit operator Set( LocalDirectory                            directory ) => new(directory.GetSubFolders());
    public static implicit operator Collection( LocalDirectory                     directory ) => new(directory.GetSubFolders());
    public static implicit operator ConcurrentCollection( LocalDirectory           directory ) => new(directory.GetSubFolders());
    public static implicit operator Items( LocalDirectory                          directory ) => new(directory.GetSubFolders());
    public static implicit operator LocalFile.Collection( LocalDirectory           directory ) => new(directory.GetFiles());
    public static implicit operator LocalFile.Set( LocalDirectory                  directory ) => new(directory.GetFiles());
    public static implicit operator LocalFile.Files( LocalDirectory                directory ) => new(directory.GetFiles());
    public static implicit operator LocalFile.ConcurrentCollection( LocalDirectory directory ) => new(directory.GetFiles());


    public static LocalDirectory AppData( string subPath )
    {
        if ( OperatingSystem.IsWindows() ) { subPath = $"%APPDATA%/{subPath}"; }

        else if ( OperatingSystem.IsMacCatalyst() ) { subPath = $"~/Library/{subPath}"; }

        else if ( OperatingSystem.IsMacOS() ) { subPath = $"~/Library/{subPath}"; }

        else if ( OperatingSystem.IsLinux() ) { subPath = Path.Join( Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ), subPath ); }

        else if ( OperatingSystem.IsAndroid() ) { subPath = Path.Join( Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ), subPath ); }

        else if ( OperatingSystem.IsIOS() ) { subPath = Path.Join( Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ), subPath ); }

        string path = Path.GetFullPath( subPath );
        return path;
    }
    private static LocalDirectory ConvertDirectory( DirectoryInfo file ) => file;


    /// <summary> Uses the <paramref name="path"/> and creates the tree structure based on <paramref name="subFolders"/> </summary>
    /// <param name="path"> </param>
    /// <param name="subFolders"> </param>
    /// <returns>
    ///     <see cref="LocalDirectory"/>
    /// </returns>
    public static LocalDirectory Create( LocalDirectory path, params ReadOnlySpan<string> subFolders ) => Directory.CreateDirectory( path.Combine( subFolders ) );

    /// <summary> Uses the <see cref="CurrentDirectory"/> and creates the tree structure based on <paramref name="subFolders"/> </summary>
    /// <param name="subFolders"> </param>
    /// <returns>
    ///     <see cref="LocalDirectory"/>
    /// </returns>
    public static LocalDirectory Create( params ReadOnlySpan<string> subFolders ) => Create( CurrentDirectory, subFolders );

    /// <summary> Uses <see cref="Path.GetTempPath"/> and creates the tree structure based on <paramref name="subFolders"/> </summary>
    /// <param name="subFolders"> </param>
    /// <returns>
    ///     <see cref="LocalDirectory"/>
    /// </returns>
    public static LocalDirectory CreateTemp( params ReadOnlySpan<string> subFolders )
    {
        LocalDirectory d = Create( Path.GetTempPath().Combine( subFolders ) );

        return d.SetTemporary();
    }

    private static LocalFile ConvertFile( FileInfo file ) => file;


    // public static implicit operator LocalFile.Watcher( LocalDirectory  directory ) => new(new Watcher( directory ));


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public IEnumerable<LocalDirectory> GetSubFolders()                                                              => Info.EnumerateDirectories().Select( ConvertDirectory );
    public IEnumerable<LocalDirectory> GetSubFolders( string searchPattern )                                        => Info.EnumerateDirectories( searchPattern ).Select( ConvertDirectory );
    public IEnumerable<LocalDirectory> GetSubFolders( string searchPattern, SearchOption       searchOption )       => Info.EnumerateDirectories( searchPattern, searchOption ).Select( ConvertDirectory );
    public IEnumerable<LocalDirectory> GetSubFolders( string searchPattern, EnumerationOptions enumerationOptions ) => Info.EnumerateDirectories( searchPattern, enumerationOptions ).Select( ConvertDirectory );


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public IEnumerable<LocalFile> GetFiles()                                                              => Info.EnumerateFiles().Select( ConvertFile );
    public IEnumerable<LocalFile> GetFiles( string searchPattern )                                        => Info.EnumerateFiles( searchPattern ).Select( ConvertFile );
    public IEnumerable<LocalFile> GetFiles( string searchPattern, SearchOption       searchOption )       => Info.EnumerateFiles( searchPattern, searchOption ).Select( ConvertFile );
    public IEnumerable<LocalFile> GetFiles( string searchPattern, EnumerationOptions enumerationOptions ) => Info.EnumerateFiles( searchPattern, enumerationOptions ).Select( ConvertFile );


    /// <summary> Gets the <see cref="LocalDirectory"/> object of the directory in this <see cref="LocalDirectory"/> </summary>
    /// <param name="paths"> </param>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="PathTooLongException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    /// <exception cref="NotSupportedException"> </exception>
    /// <returns>
    ///     <see cref="LocalDirectory"/>
    /// </returns>
    public LocalDirectory CreateSubDirectory( params ReadOnlySpan<string> paths ) => Info.CreateSubdirectory( FullPath.Combine( paths ) );
    protected virtual LocalDirectory? GetParent()
    {
        DirectoryInfo? info = Info.Parent;

        return info is null
                   ? default(LocalDirectory)
                   : info;
    }


    /// <summary> Gets the <see cref="LocalFile"/> object of the file in this <see cref="LocalDirectory"/> </summary>
    /// <param name="path"> </param>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="PathTooLongException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    /// <exception cref="NotSupportedException"> </exception>
    /// <returns>
    ///     <see cref="LocalFile"/>
    /// </returns>
    public LocalFile Join( string path ) => new(Info.Combine( path ));


    /// <summary> Gets the <see cref="LocalFile"/> object of the file in this <see cref="LocalDirectory"/> </summary>
    /// <param name="path"> </param>
    /// <param name="encoding"> </param>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="PathTooLongException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    /// <exception cref="NotSupportedException"> </exception>
    /// <returns>
    ///     <see cref="LocalFile"/>
    /// </returns>
    public LocalFile Join( string path, Encoding encoding ) => new(Info.Combine( path ), encoding);


    /// <summary> Gets the path of the directory or file in this <see cref="LocalDirectory"/> </summary>
    /// <param name="subPaths"> </param>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="PathTooLongException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    /// <exception cref="NotSupportedException"> </exception>
    /// <returns>
    ///     <see cref="string"/>
    /// </returns>
    public string Combine( string subPaths ) => Info.Combine( subPaths );

    /// <summary> Gets the path of the directory or file in this <see cref="LocalDirectory"/> </summary>
    /// <param name="subPaths"> </param>
    /// <exception cref="ArgumentException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="PathTooLongException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    /// <exception cref="NotSupportedException"> </exception>
    /// <returns>
    ///     <see cref="string"/>
    /// </returns>
    public string Combine( params ReadOnlySpan<string> subPaths ) => Info.Combine( subPaths );


    public sealed override string ToString() => FullPath;

    /// <summary> Asynchronously deletes sub-directories and files. </summary>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    public Task DeleteAllRecursivelyAsync()
    {
        List<Task> tasks = new(64);

        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            foreach ( LocalFile file in dir.GetFiles() ) { file.Delete(); }

            tasks.Add( dir.DeleteAllRecursivelyAsync() );
        }

        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            tasks.Add( dir.DeleteAllRecursivelyAsync() );
            dir.Delete();
        }

        Delete();

        return tasks.WhenAll();
    }

    /// <summary> Asynchronously deletes files. </summary>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    public Task DeleteFilesAsync()
    {
        List<Task> tasks = new List<Task>();

        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            foreach ( LocalFile file in dir.GetFiles() ) { file.Delete(); }

            tasks.Add( dir.DeleteFilesAsync() );
        }

        return tasks.WhenAll();
    }

    /// <summary> Asynchronously deletes sub-directories. </summary>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    public Task DeleteSubFoldersAsync()
    {
        List<Task> tasks = new List<Task>();

        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            tasks.Add( dir.DeleteSubFoldersAsync() );
            dir.Delete();
        }

        return tasks.WhenAll();
    }


    public async ValueTask<LocalFile> ZipAsync( LocalFile zipFilePath, CancellationToken token )
    {
        await using FileStream zipToOpen = File.Create( zipFilePath.FullPath );
        using ZipArchive       archive   = new ZipArchive( zipToOpen, ZipArchiveMode.Update );

        foreach ( LocalFile file in GetFiles() )
        {
            ZipArchiveEntry    entry  = archive.CreateEntry( file.FullPath );
            await using Stream stream = entry.Open();

            ReadOnlyMemory<byte> data = await file.ReadAsync().AsMemory( token );

            await stream.WriteAsync( data, token );
        }

        return zipFilePath;
    }
    public async ValueTask<LocalFile> ZipAsync( LocalFile zipFilePath, string searchPattern, CancellationToken token )
    {
        await using FileStream zipToOpen = File.Create( zipFilePath.FullPath );
        using ZipArchive       archive   = new ZipArchive( zipToOpen, ZipArchiveMode.Update );

        foreach ( LocalFile file in GetFiles( searchPattern ) )
        {
            ZipArchiveEntry    entry  = archive.CreateEntry( file.FullPath );
            await using Stream stream = entry.Open();

            ReadOnlyMemory<byte> data = await file.ReadAsync().AsMemory( token );

            await stream.WriteAsync( data, token );
        }

        return zipFilePath;
    }
    public async ValueTask<LocalFile> ZipAsync( LocalFile zipFilePath, string searchPattern, SearchOption searchOption, CancellationToken token )
    {
        await using FileStream zipToOpen = File.Create( zipFilePath.FullPath );
        using ZipArchive       archive   = new ZipArchive( zipToOpen, ZipArchiveMode.Update );

        foreach ( LocalFile file in GetFiles( searchPattern, searchOption ) )
        {
            ZipArchiveEntry    entry  = archive.CreateEntry( file.FullPath );
            await using Stream stream = entry.Open();

            ReadOnlyMemory<byte> data = await file.ReadAsync().AsMemory( token );

            await stream.WriteAsync( data, token );
        }

        return zipFilePath;
    }
    public async ValueTask<LocalFile> ZipAsync( LocalFile zipFilePath, string searchPattern, EnumerationOptions enumerationOptions, CancellationToken token )
    {
        await using FileStream zipToOpen = File.Create( zipFilePath.FullPath );
        using ZipArchive       archive   = new ZipArchive( zipToOpen, ZipArchiveMode.Update );

        foreach ( LocalFile file in GetFiles( searchPattern, enumerationOptions ) )
        {
            ZipArchiveEntry    entry  = archive.CreateEntry( file.FullPath );
            await using Stream stream = entry.Open();

            ReadOnlyMemory<byte> data = await file.ReadAsync().AsMemory( token );

            await stream.WriteAsync( data, token );
        }

        return zipFilePath;
    }


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    /// <summary> Deletes this directory if it is empty. </summary>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    public void Delete() => Info.Delete();


    /// <summary> Deletes sub-directories and files. This occurs on another thread in Windows, and is not blocking. </summary>
    /// <param name="recursive"> </param>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    public void Delete( bool recursive ) => Info.Delete( recursive );


    /// <summary> Deletes sub-directories and files. </summary>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    public void DeleteAllRecursively()
    {
        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            foreach ( LocalFile file in dir.GetFiles() ) { file.Delete(); }

            dir.DeleteAllRecursively();
        }

        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            dir.DeleteAllRecursively();
            dir.Delete();
        }

        Delete();
    }


    /// <summary> Deletes sub-directories. </summary>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    public void DeleteFiles()
    {
        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            foreach ( LocalFile file in dir.GetFiles() ) { file.Delete(); }

            dir.DeleteFiles();
        }

        foreach ( LocalFile file in GetFiles() ) { file.Delete(); }
    }


    /// <summary> Deletes sub-directories. </summary>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    public void DeleteSubFolders()
    {
        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            dir.DeleteSubFolders();
            dir.Delete();
        }
    }


    /// <summary> Uses the <see cref="Encoding.Default"/> encoding to used for the file names </summary>
    /// <param name="output"> file path to write the zip to </param>
    /// <param name="compression"> Defaults to <see cref="CompressionLevel.Optimal"/> </param>
    public void Zip( in LocalFile output, CompressionLevel compression = CompressionLevel.Optimal ) => Zip( output, Encoding.Default, compression );
    /// <summary> </summary>
    /// <param name="output"> file path to write the zip to </param>
    /// <param name="compression"> Defaults to <see cref="CompressionLevel.Optimal"/> </param>
    /// <param name="encoding"> The encoding used for the file names </param>
    public void Zip( in LocalFile output, Encoding encoding, CompressionLevel compression = CompressionLevel.Optimal ) => ZipFile.CreateFromDirectory( FullPath, output.FullPath, compression, true, encoding );


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        return other is LocalDirectory value
                   ? CompareTo( value )
                   : throw new ExpectedValueTypeException( nameof(other), other, typeof(LocalDirectory) );
    }
    public int CompareTo( LocalDirectory? other )
    {
        if ( ReferenceEquals( this, other ) ) { return 0; }

        if ( ReferenceEquals( null, other ) ) { return 1; }

        return string.Compare( FullPath, other.FullPath, StringComparison.Ordinal );
    }
    public bool Equals( LocalDirectory? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return this.IsTempFile() == other.IsTempFile() && FullPath == other.FullPath;
    }
    public override bool Equals( object? other ) => other is LocalDirectory directory && Equals( directory );
    public override int  GetHashCode()           => HashCode.Combine( FullPath, this.IsTempFile() );


    public static bool operator ==( LocalDirectory? left, LocalDirectory? right ) => Equalizer.Equals( left, right );
    public static bool operator >( LocalDirectory?  left, LocalDirectory? right ) => Sorter.Compare( left, right ) > 0;
    public static bool operator >=( LocalDirectory? left, LocalDirectory? right ) => Sorter.Compare( left, right ) >= 0;
    public static bool operator !=( LocalDirectory? left, LocalDirectory? right ) => !Equalizer.Equals( left, right );
    public static bool operator <( LocalDirectory?  left, LocalDirectory? right ) => Sorter.Compare( left, right ) < 0;
    public static bool operator <=( LocalDirectory? left, LocalDirectory? right ) => Sorter.Compare( left, right ) <= 0;


    // ---------------------------------------------------------------------------------------------------------------------------------------------------



    [Serializable]
    public class Collection : ObservableCollection<LocalDirectory>
    {
        public Collection() : base() { }
        public Collection( IEnumerable<LocalDirectory> items ) : base( items ) { }
    }



    [Serializable]
    public class ConcurrentCollection : ConcurrentObservableCollection<LocalDirectory>
    {
        public ConcurrentCollection() : base( Sorter ) { }
        public ConcurrentCollection( IEnumerable<LocalDirectory> items ) : base( items, Sorter ) { }
    }



    [Serializable]
    public class Deque : ConcurrentDeque<LocalDirectory>
    {
        public Deque() : base() { }
        public Deque( IEnumerable<LocalDirectory> items ) : base( items ) { }
    }



    [Serializable]
    public class Items : List<LocalDirectory>
    {
        public Items() : base() { }
        public Items( int                         capacity ) : base( capacity ) { }
        public Items( IEnumerable<LocalDirectory> items ) : base( items ) { }
    }



    [Serializable]
    public class Queue : Queue<LocalDirectory>
    {
        public Queue() : base() { }
        public Queue( IEnumerable<LocalDirectory> items ) : base( items ) { }
    }



    [Serializable]
    public class ConcurrentQueue : ConcurrentQueue<LocalDirectory>
    {
        public ConcurrentQueue() : base() { }
        public ConcurrentQueue( IEnumerable<LocalDirectory> items ) : base( items ) { }
    }



    [Serializable]
    public class Set : HashSet<LocalDirectory>
    {
        public Set() : base() { }
        public Set( int                         capacity ) : base( capacity ) { }
        public Set( IEnumerable<LocalDirectory> items ) : base( items ) { }
    }



    [SuppressMessage( "ReSharper", "IntroduceOptionalParameters.Global" )]
    public class Watcher : FileSystemWatcher
    {
        public LocalDirectory Directory { get; init; }


        /// <summary> Uses the <see cref="CurrentDirectory"/> </summary>
        public Watcher() : this( CurrentDirectory ) { }
        public Watcher( LocalDirectory directory ) : this( directory, "*" ) { }
        public Watcher( LocalDirectory directory, string searchFilter ) : this( directory, searchFilter, NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite ) { }
        public Watcher( LocalDirectory directory, string searchFilter, NotifyFilters filters ) : base( directory.FullPath, searchFilter )
        {
            Directory    = directory;
            NotifyFilter = filters;
        }
    }
}
