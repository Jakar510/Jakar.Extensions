using System.IO.Compression;
using System.Security;


namespace Jakar.Extensions.FileSystemExtensions;


[Serializable]
public class LocalDirectory : ILocalDirectory<LocalFile, LocalDirectory>
{
    protected DirectoryInfo? _info;

    [JsonIgnore]
    public DirectoryInfo Info
    {
        get
        {
            _info ??= new DirectoryInfo(FullPath);
            return _info;
        }
    }


    public string  FullPath { get; init; }
    public bool    Exists   => Info.Exists;
    public string  Name     => Info.Name;
    public string? Root     => Directory.GetDirectoryRoot(FullPath);

    public DateTime CreationTimeUtc
    {
        get => Directory.GetCreationTimeUtc(FullPath);
        set => Directory.SetCreationTimeUtc(FullPath, value);
    }

    public DateTime LastAccessTimeUtc
    {
        get => Directory.GetLastAccessTimeUtc(FullPath);
        set => Directory.SetLastWriteTimeUtc(FullPath, value);
    }

    public DateTime LastWriteTimeUtc
    {
        get => Directory.GetLastWriteTimeUtc(FullPath);
        set => Directory.SetLastWriteTimeUtc(FullPath, value);
    }

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


    /// <summary>
    /// Gets or sets the application's fully qualified path of the current working directory.
    /// </summary>
    public static LocalDirectory CurrentDirectory
    {
        get => new(Environment.CurrentDirectory);
        set => Environment.CurrentDirectory = Path.GetFullPath(value.FullPath);
    }


    public LocalDirectory() => FullPath = string.Empty;
    public LocalDirectory( ReadOnlySpan<char> path ) : this(path.ToString()) { }
    public LocalDirectory( DirectoryInfo      path ) : this(path.FullName) { }
    public LocalDirectory( string             path, params string[] subFolders ) : this(path.Combine(subFolders)) { }
    public LocalDirectory( string             path ) => FullPath = Path.GetFullPath(path);

    // public static implicit operator LocalDirectory( string             path ) => new(path);
    public static implicit operator LocalDirectory( DirectoryInfo      info ) => new(info);
    public static implicit operator LocalDirectory( ReadOnlySpan<char> info ) => new(info);


    /// <summary>
    /// Uses the <paramref name="path"/> and creates the tree structure based on <paramref name="subFolders"/>
    /// </summary>
    /// <param name="path"></param>
    /// <param name="subFolders"></param>
    /// <returns><see cref="LocalDirectory"/></returns>
    public static LocalDirectory Create( LocalDirectory path, params string[] subFolders ) => Directory.CreateDirectory(path.Combine(subFolders));

    /// <summary>
    /// Uses the <see cref="CurrentDirectory"/> and creates the tree structure based on <paramref name="subFolders"/>
    /// </summary>
    /// <param name="subFolders"></param>
    /// <returns><see cref="LocalDirectory"/></returns>
    public static LocalDirectory Create( params string[] subFolders ) => Create(CurrentDirectory, subFolders);

    /// <summary>
    /// Uses <see cref="Path.GetTempPath"/> and creates the tree structure based on <paramref name="subFolders"/>
    /// </summary>
    /// <param name="subFolders"></param>
    /// <returns><see cref="LocalDirectory"/></returns>
    public static LocalDirectory CreateTemp( params string[] subFolders )
    {
        LocalDirectory d = Create(Path.GetTempPath().Combine(subFolders));
        return d.SetTemporary();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="output">file path to write the zip to</param>
    /// <param name="compression">Defaults to <see cref="CompressionLevel.Optimal"/></param>
    /// <param name="encoding">Defaults to <see cref="Encoding.Default"/></param>
    public void Zip( in LocalFile output, in CompressionLevel compression = CompressionLevel.Optimal, in Encoding? encoding = null )
        => ZipFile.CreateFromDirectory(FullPath,
                                       output.FullPath,
                                       compression,
                                       true,
                                       encoding ?? Encoding.Default);

    public override string ToString() => FullPath;


    public LocalDirectory CreateSubDirectory( params string[] paths ) => Info.CreateSubdirectory(FullPath.Combine(paths));

    public LocalFile Join( string path ) => Info.Combine(path);

    public string Combine( string          path )  => Info.Combine(path);
    public string Combine( params string[] paths ) => Info.Combine(paths);


#region Implementation of IDisposable

    public void Dispose()
    {
        Dispose(this.IsTempFile());
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose( bool remove )
    {
        if ( string.IsNullOrWhiteSpace(FullPath) ) { return; }

        if ( remove && Exists ) { Delete(true); }
    }

    public async ValueTask DisposeAsync()
    {
        if ( !this.IsTempFile() ) { return; }

        await DeleteAllRecursivelyAsync();
    }

#endregion


#region Implementation of ITempFile

    private bool _isTemporary;

    bool TempFile.ITempFile.IsTemporary
    {
        get => _isTemporary;
        set => _isTemporary = value;
    }

#endregion


#region Implementation of IEquatable<LocalFile>

    public override bool Equals( object obj )
    {
        if ( obj is LocalDirectory file ) { return Equals(file); }

        return false;
    }

    public override int GetHashCode() => HashCode.Combine(FullPath, ( (TempFile.ITempFile)this ).IsTemporary);


    public bool Equals( LocalDirectory? other )
    {
        if ( ReferenceEquals(this, other) ) { return true; }

        if ( other is null ) { return false; }

        if ( GetType() != other.GetType() ) { return false; }

        return ( (TempFile.ITempFile)this ).IsTemporary == ( (TempFile.ITempFile)other ).IsTemporary && FullPath == other.FullPath;
    }

#endregion


#region Deletes

    /// <summary>
    /// Deletes this directory if it is empty.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public void Delete() => Info.Delete();


    /// <summary>
    /// Deletes sub-directories and files. This occurs on another thread in Windows, and is not blocking.
    /// </summary>
    /// <param name="recursive"></param>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public void Delete( bool recursive ) => Info.Delete(recursive);


    /// <summary>
    /// Deletes sub-directories and files.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
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

    /// <summary>
    /// Asynchronously deletes sub-directories and files.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public Task DeleteAllRecursivelyAsync()
    {
        var tasks = new List<Task>();

        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            foreach ( LocalFile file in dir.GetFiles() ) { file.Delete(); }

            tasks.Add(dir.DeleteAllRecursivelyAsync());
        }

        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            tasks.Add(dir.DeleteAllRecursivelyAsync());
            dir.Delete();
        }

        Delete();

        return Task.WhenAll(tasks);
    }


    /// <summary>
    /// Deletes sub-directories.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public void DeleteFiles()
    {
        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            foreach ( LocalFile file in dir.GetFiles() ) { file.Delete(); }

            dir.DeleteFiles();
        }
    }

    /// <summary>
    /// Asynchronously deletes files.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public Task DeleteFilesAsync()
    {
        var tasks = new List<Task>();

        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            foreach ( LocalFile file in dir.GetFiles() ) { file.Delete(); }

            tasks.Add(dir.DeleteFilesAsync());
        }

        return Task.WhenAll(tasks);
    }


    /// <summary>
    /// Deletes sub-directories.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public void DeleteSubFolders()
    {
        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            dir.DeleteSubFolders();
            dir.Delete();
        }
    }

    /// <summary>
    /// Asynchronously deletes sub-directories.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public Task DeleteSubFoldersAsync()
    {
        var tasks = new List<Task>();

        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            tasks.Add(dir.DeleteSubFoldersAsync());
            dir.Delete();
        }

        return Task.WhenAll(tasks);
    }

#endregion


#region sub Files

    public IEnumerable<LocalFile> GetFiles()                                                              => Info.EnumerateFiles().Select(ConvertFile);
    public IEnumerable<LocalFile> GetFiles( string searchPattern )                                        => Info.EnumerateFiles(searchPattern).Select(ConvertFile);
    public IEnumerable<LocalFile> GetFiles( string searchPattern, SearchOption       searchOption )       => Info.EnumerateFiles(searchPattern, searchOption).Select(ConvertFile);
    public IEnumerable<LocalFile> GetFiles( string searchPattern, EnumerationOptions enumerationOptions ) => Info.EnumerateFiles(searchPattern, enumerationOptions).Select(ConvertFile);


    private static LocalFile ConvertFile( FileInfo file ) => file;

#endregion


#region Sub Folders

    public IEnumerable<LocalDirectory> GetSubFolders()                                                              => Info.EnumerateDirectories().Select(ConvertDirectory);
    public IEnumerable<LocalDirectory> GetSubFolders( string searchPattern )                                        => Info.EnumerateDirectories(searchPattern).Select(ConvertDirectory);
    public IEnumerable<LocalDirectory> GetSubFolders( string searchPattern, SearchOption       searchOption )       => Info.EnumerateDirectories(searchPattern, searchOption).Select(ConvertDirectory);
    public IEnumerable<LocalDirectory> GetSubFolders( string searchPattern, EnumerationOptions enumerationOptions ) => Info.EnumerateDirectories(searchPattern, enumerationOptions).Select(ConvertDirectory);


    private static LocalDirectory ConvertDirectory( DirectoryInfo file ) => file;

#endregion



    public class Watcher : FileSystemWatcher
    {
        public Watcher() : base() { }

        public Watcher( LocalDirectory directory, string searchFilter = "*", NotifyFilters? filters = default ) : base(directory.FullPath, searchFilter)
            => NotifyFilter = filters ?? NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite;
    }



    [Serializable]
    public class Collection : ObservableCollection<LocalDirectory>
    {
        public Collection() : base() { }
        public Collection( IEnumerable<LocalDirectory> items ) : base(items) { }
        public Collection( LocalDirectory              directory ) : this(directory.GetSubFolders()) { }
    }



    [Serializable]
    public class Items : List<LocalDirectory>
    {
        public Items() : base() { }
        public Items( int                         capacity ) : base(capacity) { }
        public Items( IEnumerable<LocalDirectory> items ) : base(items) { }
        public Items( LocalDirectory              directory ) : this(directory.GetSubFolders()) { }
    }



    [Serializable]
    public class Set : HashSet<LocalDirectory>
    {
        public Set() : base() { }
        public Set( int                         capacity ) : base(capacity) { }
        public Set( IEnumerable<LocalDirectory> items ) : base(items) { }
        public Set( LocalDirectory              directory ) : this(directory.GetSubFolders()) { }
    }
}
