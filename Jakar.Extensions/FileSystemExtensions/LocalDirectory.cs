using System.IO.Compression;
using System.Security;


#nullable enable
namespace Jakar.Extensions.FileSystemExtensions;


[Serializable]
public class LocalDirectory : BaseCollections<LocalDirectory>, TempFile.ITempFile, IEquatable<LocalDirectory>, IAsyncDisposable
{
    protected DirectoryInfo? _info;


    private bool _isTemporary;

    [JsonIgnore]
    public DirectoryInfo Info
    {
        get
        {
            _info ??= new DirectoryInfo(FullPath);
            return _info;
        }
    }


    public              string          FullPath { get; init; }
    public              bool            Exists   => Info.Exists;
    public              string          Name     => Info.Name;
    public              string          Root     => Directory.GetDirectoryRoot(FullPath);
    [JsonIgnore] public LocalDirectory? Parent   => GetParent();


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


    /// <summary>
    ///     Gets or sets the application's fully qualified path of the current working directory.
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

    public sealed override string ToString() => FullPath;
    protected virtual LocalDirectory? GetParent()
    {
        DirectoryInfo? info = Info.Parent;

        return info is null
                   ? default(LocalDirectory)
                   : info;
    }


    /// <summary>
    ///     Uses the
    ///     <paramref name = "path" />
    ///     and creates the tree structure based on
    ///     <paramref name = "subFolders" />
    /// </summary>
    /// <param name = "path" > </param>
    /// <param name = "subFolders" > </param>
    /// <returns>
    ///     <see cref = "LocalDirectory" />
    /// </returns>
    public static LocalDirectory Create( LocalDirectory path, params string[] subFolders ) => Directory.CreateDirectory(path.Combine(subFolders));

    /// <summary>
    ///     Uses the
    ///     <see cref = "CurrentDirectory" />
    ///     and creates the tree structure based on
    ///     <paramref name = "subFolders" />
    /// </summary>
    /// <param name = "subFolders" > </param>
    /// <returns>
    ///     <see cref = "LocalDirectory" />
    /// </returns>
    public static LocalDirectory Create( params string[] subFolders ) => Create(CurrentDirectory, subFolders);

    /// <summary>
    ///     Uses
    ///     <see cref = "Path.GetTempPath" />
    ///     and creates the tree structure based on
    ///     <paramref name = "subFolders" />
    /// </summary>
    /// <param name = "subFolders" > </param>
    /// <returns>
    ///     <see cref = "LocalDirectory" />
    /// </returns>
    public static LocalDirectory CreateTemp( params string[] subFolders )
    {
        LocalDirectory d = Create(Path.GetTempPath().Combine(subFolders));
        return d.SetTemporary();
    }


    /// <summary>
    ///     Uses the
    ///     <see cref = "Encoding.Default" />
    ///     encoding to used for the file names
    /// </summary>
    /// <param name = "output" > file path to write the zip to </param>
    /// <param name = "compression" >
    ///     Defaults to
    ///     <see cref = "CompressionLevel.Optimal" />
    /// </param>
    public void Zip( in LocalFile output, in CompressionLevel compression = CompressionLevel.Optimal ) => Zip(output, Encoding.Default, compression);
    /// <summary>
    /// 
    /// </summary>
    /// <param name = "output" > file path to write the zip to </param>
    /// <param name = "compression" >
    ///     Defaults to
    ///     <see cref = "CompressionLevel.Optimal" />
    /// </param>
    /// <param name = "encoding" > The encoding used for the file names </param>
    public void Zip( in LocalFile output, in Encoding encoding, in CompressionLevel compression = CompressionLevel.Optimal ) => ZipFile.CreateFromDirectory(FullPath, output.FullPath, compression, true, encoding);


    /// <summary>
    ///     Gets the
    ///     <see cref = "LocalDirectory" />
    ///     object of the directory in this
    ///     <see cref = "LocalDirectory" />
    /// </summary>
    /// <param name = "paths" > </param>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "PathTooLongException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
    /// <exception cref = "NotSupportedException" > </exception>
    /// <returns>
    ///     <see cref = "LocalDirectory" />
    /// </returns>
    public LocalDirectory CreateSubDirectory( params string[] paths ) => Info.CreateSubdirectory(FullPath.Combine(paths));

    /// <summary>
    ///     Gets the
    ///     <see cref = "LocalFile" />
    ///     object of the file in this
    ///     <see cref = "LocalDirectory" />
    /// </summary>
    /// <param name = "path" > </param>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "PathTooLongException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
    /// <exception cref = "NotSupportedException" > </exception>
    /// <returns>
    ///     <see cref = "LocalFile" />
    /// </returns>
    public LocalFile Join( string path ) => Info.Combine(path);

    /// <summary>
    ///     Gets the path of the directory or file in this
    ///     <see cref = "LocalDirectory" />
    /// </summary>
    /// <param name = "subPaths" > </param>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "PathTooLongException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
    /// <exception cref = "NotSupportedException" > </exception>
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
    public string Combine( string subPaths ) => Info.Combine(subPaths);

    /// <summary>
    ///     Gets the path of the directory or file in this
    ///     <see cref = "LocalDirectory" />
    /// </summary>
    /// <param name = "subPaths" > </param>
    /// <exception cref = "ArgumentException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "PathTooLongException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
    /// <exception cref = "NotSupportedException" > </exception>
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
    public string Combine( params string[] subPaths ) => Info.Combine(subPaths);


    public static bool operator ==( LocalDirectory? left, LocalDirectory? right ) => EqualityComparer.Instance.Equals(left, right);
    public static bool operator !=( LocalDirectory? left, LocalDirectory? right ) => !EqualityComparer.Instance.Equals(left, right);
    public static bool operator <( LocalDirectory?  left, LocalDirectory? right ) => RelationalComparer.Instance.Compare(left, right) < 0;
    public static bool operator >( LocalDirectory?  left, LocalDirectory? right ) => RelationalComparer.Instance.Compare(left, right) > 0;
    public static bool operator <=( LocalDirectory? left, LocalDirectory? right ) => RelationalComparer.Instance.Compare(left, right) <= 0;
    public static bool operator >=( LocalDirectory? left, LocalDirectory? right ) => RelationalComparer.Instance.Compare(left, right) >= 0;


    public override int CompareTo( LocalDirectory? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( ReferenceEquals(null, other) ) { return 1; }

        return string.Compare(FullPath, other.FullPath, StringComparison.Ordinal);
    }
    public override int GetHashCode() => HashCode.Combine(FullPath, this.IsTempFile());
    protected virtual void Dispose( bool remove )
    {
        if ( remove && Exists ) { Delete(true); }
    }


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    /// <summary>
    ///     Deletes this directory if it is empty.
    /// </summary>
    /// <exception cref = "UnauthorizedAccessException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
    public void Delete() => Info.Delete();


    /// <summary>
    ///     Deletes sub-directories and files. This occurs on another thread in Windows, and is not blocking.
    /// </summary>
    /// <param name = "recursive" > </param>
    /// <exception cref = "UnauthorizedAccessException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
    public void Delete( bool recursive ) => Info.Delete(recursive);


    /// <summary>
    ///     Deletes sub-directories and files.
    /// </summary>
    /// <exception cref = "UnauthorizedAccessException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
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
    ///     Asynchronously deletes sub-directories and files.
    /// </summary>
    /// <exception cref = "UnauthorizedAccessException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
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
    ///     Deletes sub-directories.
    /// </summary>
    /// <exception cref = "UnauthorizedAccessException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
    public void DeleteFiles()
    {
        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            foreach ( LocalFile file in dir.GetFiles() ) { file.Delete(); }

            dir.DeleteFiles();
        }
    }

    /// <summary>
    ///     Asynchronously deletes files.
    /// </summary>
    /// <exception cref = "UnauthorizedAccessException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
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
    ///     Deletes sub-directories.
    /// </summary>
    /// <exception cref = "UnauthorizedAccessException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
    public void DeleteSubFolders()
    {
        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            dir.DeleteSubFolders();
            dir.Delete();
        }
    }

    /// <summary>
    ///     Asynchronously deletes sub-directories.
    /// </summary>
    /// <exception cref = "UnauthorizedAccessException" > </exception>
    /// <exception cref = "DirectoryNotFoundException" > </exception>
    /// <exception cref = "FileNotFoundException" > </exception>
    /// <exception cref = "IOException" > </exception>
    /// <exception cref = "SecurityException" > </exception>
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


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public IEnumerable<LocalFile> GetFiles() => Info.EnumerateFiles().Select(ConvertFile);
    public IEnumerable<LocalFile> GetFiles( string searchPattern ) => Info.EnumerateFiles(searchPattern).Select(ConvertFile);
    public IEnumerable<LocalFile> GetFiles( string searchPattern, SearchOption       searchOption ) => Info.EnumerateFiles(searchPattern,       searchOption).Select(ConvertFile);
    public IEnumerable<LocalFile> GetFiles( string searchPattern, EnumerationOptions enumerationOptions ) => Info.EnumerateFiles(searchPattern, enumerationOptions).Select(ConvertFile);
    private static LocalFile ConvertFile( FileInfo file ) => file;


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public IEnumerable<LocalDirectory> GetSubFolders() => Info.EnumerateDirectories().Select(ConvertDirectory);
    public IEnumerable<LocalDirectory> GetSubFolders( string      searchPattern ) => Info.EnumerateDirectories(searchPattern).Select(ConvertDirectory);
    public IEnumerable<LocalDirectory> GetSubFolders( string      searchPattern, SearchOption       searchOption ) => Info.EnumerateDirectories(searchPattern,       searchOption).Select(ConvertDirectory);
    public IEnumerable<LocalDirectory> GetSubFolders( string      searchPattern, EnumerationOptions enumerationOptions ) => Info.EnumerateDirectories(searchPattern, enumerationOptions).Select(ConvertDirectory);
    private static LocalDirectory ConvertDirectory( DirectoryInfo file ) => file;


    public static implicit operator Collection( LocalDirectory                                      directory ) => new(directory.GetSubFolders());
    public static implicit operator ConcurrentCollection( LocalDirectory                            directory ) => new(directory.GetSubFolders());
    public static implicit operator Items( LocalDirectory                                           directory ) => new(directory.GetSubFolders());
    public static implicit operator Set( LocalDirectory                                             directory ) => new(directory.GetSubFolders());
    public static implicit operator Watcher( LocalDirectory                                         directory ) => new(directory);
    public static implicit operator BaseCollections<LocalFile>.Collection( LocalDirectory           directory ) => new(directory.GetFiles());
    public static implicit operator BaseCollections<LocalFile>.ConcurrentCollection( LocalDirectory directory ) => new(directory.GetFiles());
    public static implicit operator BaseCollections<LocalFile>.Items( LocalDirectory                directory ) => new(directory.GetFiles());
    public static implicit operator BaseCollections<LocalFile>.Set( LocalDirectory                  directory ) => new(directory.GetFiles());
    public static implicit operator LocalFile.Watcher( LocalDirectory                               directory ) => new(new Watcher(directory));
    public async ValueTask DisposeAsync()
    {
        if ( !this.IsTempFile() ) { return; }

        await DeleteAllRecursivelyAsync();
    }
    public override bool Equals( LocalDirectory? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return this.IsTempFile() == other.IsTempFile() && FullPath == other.FullPath;
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


    // ---------------------------------------------------------------------------------------------------------------------------------------------------



    private sealed class EqualityComparer : IEqualityComparer<LocalDirectory>
    {
        public static EqualityComparer Instance { get; } = new();


        public bool Equals( LocalDirectory? x, LocalDirectory? y )
        {
            if ( x is null ) { return false; }

            if ( y is null ) { return false; }

            if ( ReferenceEquals(x, y) ) { return true; }

            return x.Equals(y);
        }

        public int GetHashCode( LocalDirectory obj ) => obj.GetHashCode();
    }



    private sealed class RelationalComparer : IComparer<LocalDirectory>
    {
        public static RelationalComparer Instance { get; } = new();


        public int Compare( LocalDirectory? left, LocalDirectory? right )
        {
            if ( left is null ) { return 1; }

            if ( right is null ) { return -1; }

            if ( ReferenceEquals(left, right) ) { return 0; }

            return left.CompareTo(right);
        }
    }



    // ---------------------------------------------------------------------------------------------------------------------------------------------------



    [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
    public class Watcher : FileSystemWatcher
    {
        public LocalDirectory Directory { get; init; }


        /// <summary>
        ///     Uses the
        ///     <see cref = "CurrentDirectory" />
        /// </summary>
        public Watcher() : this(CurrentDirectory) { }
        public Watcher( LocalDirectory directory ) : this(directory, "*") { }
        public Watcher( LocalDirectory directory, string searchFilter ) : this(directory, searchFilter, NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite) { }
        public Watcher( LocalDirectory directory, string searchFilter, NotifyFilters filters ) : base(directory.FullPath, searchFilter)
        {
            Directory    = directory;
            NotifyFilter = filters;
        }
    }
}
