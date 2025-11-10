using ZLinq;
using ZLinq.Linq;
using ZLinq.Traversables;



namespace Jakar.Extensions;


[Serializable]
public class LocalDirectory : BaseClass<LocalDirectory>, TempFile.ITempFile, IAsyncDisposable, IEqualComparable<LocalDirectory>, IJsonModel<LocalDirectory>
{
    protected readonly DirectoryInfo _info;
    public readonly    string        FullPath;
    private            bool          __isTemporary;


    /// <summary> Gets or sets the application's fully qualified path of the current working directory. </summary>
    public static LocalDirectory CurrentDirectory { get => new(Environment.CurrentDirectory); set => Environment.CurrentDirectory = value.FullPath; }
    public static       JsonTypeInfo<LocalDirectory[]> JsonArrayInfo     => JakarExtensionsContext.Default.LocalDirectoryArray;
    public static       JsonSerializerContext          JsonContext       => JakarExtensionsContext.Default;
    public static       JsonTypeInfo<LocalDirectory>   JsonTypeInfo      => JakarExtensionsContext.Default.LocalDirectory;
    public              DateTime                       CreationTimeUtc   { get => Directory.GetCreationTimeUtc(FullPath); set => Directory.SetCreationTimeUtc(FullPath, value); }
    public              bool                           DoesNotExist      => !Exists;
    public              bool                           Exists            => Info.Exists;
    [JsonIgnore] public DirectoryInfo                  Info              => _info;
    bool TempFile.ITempFile.                           IsTemporary       { get => __isTemporary;                            set => __isTemporary = value; }
    public              DateTime                       LastAccessTimeUtc { get => Directory.GetLastAccessTimeUtc(FullPath); set => Directory.SetLastWriteTimeUtc(FullPath, value); }
    public              DateTime                       LastWriteTimeUtc  { get => Directory.GetLastWriteTimeUtc(FullPath);  set => Directory.SetLastWriteTimeUtc(FullPath, value); }
    public              string                         Name              => Info.Name;
    [JsonIgnore] public LocalDirectory?                Parent            => GetParent();
    public              string                         Root              => Directory.GetDirectoryRoot(FullPath);


    public LocalDirectory( string path ) : this(Directory.CreateDirectory(path)) { }
    public LocalDirectory( string path, params ReadOnlySpan<string> subFolders ) : this(new DirectoryInfo(path.Combine(subFolders))) { }
    public LocalDirectory( DirectoryInfo info )
    {
        info.Create();
        _info    = info;
        FullPath = info.FullName;
    }

    
    protected override void Dispose( bool disposing )
    {
        base.Dispose(disposing);
        if ( !disposing ) { return; } 

        DisposeAsync()
           .CallSynchronously();
    }

    public virtual async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        if ( DoesNotExist || !this.IsTempFile() ) { return; }

        await DeleteAllRecursivelyAsync();
    }


    public static implicit operator string( LocalDirectory             directory ) => directory.FullPath;
    public static implicit operator DirectoryInfo( LocalDirectory      directory ) => directory.Info;
    public static implicit operator ReadOnlySpan<char>( LocalDirectory directory ) => directory.FullPath;
    public static implicit operator LocalDirectory( DirectoryInfo      info )      => new(info);
    public static implicit operator LocalDirectory( ReadOnlySpan<char> path )      => new(path.ToString());
    public static implicit operator LocalDirectory( string             path )      => new(path);
    public static implicit operator LocalFileWatcher( LocalDirectory   directory ) => new(directory);


    public static LocalDirectory Create( DirectoryInfo      file ) => file;
    public static LocalDirectory Create( string             file ) => file;
    public static LocalDirectory Create( ReadOnlySpan<char> file ) => file;
    public static LocalDirectory? TryCreate( [NotNullIfNotNull(nameof(info))] DirectoryInfo? info ) => info is not null
                                                                                                           ? Create(info)
                                                                                                           : null;
    public static LocalDirectory? TryCreate( [NotNullIfNotNull(nameof(info))] string? info ) => info is not null
                                                                                                    ? Create(info)
                                                                                                    : null;

    public static LocalDirectory AppData( string subPath )
    {
        if ( OperatingSystem.IsWindows() ) { subPath = $"%APPDATA%/{subPath}"; }

        else if ( OperatingSystem.IsMacCatalyst() ) { subPath = $"~/Library/{subPath}"; }

        else if ( OperatingSystem.IsMacOS() ) { subPath = $"~/Library/{subPath}"; }

        else if ( OperatingSystem.IsLinux() ) { subPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), subPath); }

        else if ( OperatingSystem.IsAndroid() ) { subPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), subPath); }

        else if ( OperatingSystem.IsIOS() ) { subPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), subPath); }

        string path = Path.GetFullPath(subPath);
        return path;
    }


    /// <summary> Uses the <paramref name="path"/> and creates the tree structure based on <paramref name="subFolders"/> </summary>
    /// <param name="path"> </param>
    /// <param name="subFolders"> </param>
    /// <returns>
    ///     <see cref="LocalDirectory"/>
    /// </returns>
    public static LocalDirectory Create( LocalDirectory path, params ReadOnlySpan<string> subFolders ) => Directory.CreateDirectory(path.Combine(subFolders));

    /// <summary> Uses the <see cref="CurrentDirectory"/> and creates the tree structure based on <paramref name="subFolders"/> </summary>
    /// <param name="subFolders"> </param>
    /// <returns>
    ///     <see cref="LocalDirectory"/>
    /// </returns>
    public static LocalDirectory Create( params ReadOnlySpan<string> subFolders ) => Create(CurrentDirectory, subFolders);

    /// <summary> Uses <see cref="Path.GetTempPath"/> and creates the tree structure based on <paramref name="subFolders"/> </summary>
    /// <param name="subFolders"> </param>
    /// <returns>
    ///     <see cref="LocalDirectory"/>
    /// </returns>
    public static LocalDirectory CreateTemp( params ReadOnlySpan<string> subFolders )
    {
        LocalDirectory d = Create(Path.GetTempPath()
                                      .Combine(subFolders));

        return d.SetTemporary();
    }


    // public static implicit operator LocalFile.Watcher( LocalDirectory  directory ) => new(new Watcher( directory ));


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public LocalDirectory[] GetSubFolders() => Info.EnumerateDirectories()
                                                   .Select(Create)
                                                   .ToArray()
                                                   .ToArray();
    public LocalDirectory[] GetSubFolders( string searchPattern ) => Info.EnumerateDirectories(searchPattern)
                                                                         .Select(Create)
                                                                         .ToArray()
                                                                         .ToArray();
    public LocalDirectory[] GetSubFolders( string searchPattern, SearchOption searchOption ) => Info.EnumerateDirectories(searchPattern, searchOption)
                                                                                                    .Select(Create)
                                                                                                    .ToArray()
                                                                                                    .ToArray();
    public LocalDirectory[] GetSubFolders( string searchPattern, EnumerationOptions enumerationOptions ) => Info.EnumerateDirectories(searchPattern, enumerationOptions)
                                                                                                                .Select(Create)
                                                                                                                .ToArray();


    // ---------------------------------------------------------------------------------------------------------------------------------------------------


    public LocalFile[] GetFiles() => Info.EnumerateFiles()
                                         .Select(LocalFile.Create)
                                         .ToArray();
    public LocalFile[] GetFiles( string searchPattern ) => Info.EnumerateFiles(searchPattern)
                                                               .Select(LocalFile.Create)
                                                               .ToArray();
    public LocalFile[] GetFiles( string searchPattern, SearchOption searchOption ) => Info.EnumerateFiles(searchPattern, searchOption)
                                                                                          .Select(LocalFile.Create)
                                                                                          .ToArray();
    public LocalFile[] GetFiles( string searchPattern, EnumerationOptions enumerationOptions ) => Info.EnumerateFiles(searchPattern, enumerationOptions)
                                                                                                      .Select(LocalFile.Create)
                                                                                                      .ToArray();


    public ValueEnumerable<Select<Descendants<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo, OneOf<LocalFile, LocalDirectory>>, OneOf<LocalFile, LocalDirectory>> Descendants() => Info.Descendants()
                                                                                                                                                                                                  .Select(GetFileOrDirectory);
    public ValueEnumerable<Select<Children<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo, OneOf<LocalFile, LocalDirectory>>, OneOf<LocalFile, LocalDirectory>> Children() => Info.Children()
                                                                                                                                                                                            .Select(GetFileOrDirectory);
    public ValueEnumerable<Select<Ancestors<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo, OneOf<LocalFile, LocalDirectory>>, OneOf<LocalFile, LocalDirectory>> Ancestors() => Info.Ancestors()
                                                                                                                                                                                              .Select(GetFileOrDirectory);
    public static OneOf<LocalFile, LocalDirectory> GetFileOrDirectory( FileSystemInfo info ) => info is DirectoryInfo directory
                                                                                                    ? Create(directory)
                                                                                                    : LocalFile.Create(info);


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
    public LocalDirectory CreateSubDirectory( params ReadOnlySpan<string> paths ) => Info.CreateSubdirectory(Path.Combine(paths));
    protected virtual LocalDirectory? GetParent() => TryCreate(Info.Parent);


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
    public LocalFile Join( string path ) => new(Info.Combine(path));


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
    public LocalFile Join( string path, Encoding encoding ) => new(Info.Combine(path), encoding);


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
    public string Combine( string subPaths ) => Info.Combine(subPaths);

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
    public string Combine( params ReadOnlySpan<string> subPaths ) => Info.Combine(subPaths);


    public sealed override string ToString() => FullPath;

    /// <summary> Asynchronously deletes subdirectories and files. </summary>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    public Task DeleteAllRecursivelyAsync()
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        List<Task>          tasks         = new(64);

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
        return Task.WhenAll(CollectionsMarshal.AsSpan(tasks));
    }

    /// <summary> Asynchronously deletes files. </summary>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    public Task DeleteFilesAsync()
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        List<Task>          tasks         = new(DEFAULT_CAPACITY);

        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            foreach ( LocalFile file in dir.GetFiles() ) { file.Delete(); }

            tasks.Add(dir.DeleteFilesAsync());
        }

        return Task.WhenAll(CollectionsMarshal.AsSpan(tasks));
    }

    /// <summary> Asynchronously deletes subdirectories. </summary>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="FileNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    public Task DeleteSubFoldersAsync()
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        List<Task>          tasks         = new(DEFAULT_CAPACITY);

        foreach ( LocalDirectory dir in GetSubFolders() )
        {
            tasks.Add(dir.DeleteSubFoldersAsync());
            dir.Delete();
        }

        return Task.WhenAll(CollectionsMarshal.AsSpan(tasks));
    }


    public async Task<LocalFile> ZipAsync( LocalFile zipFilePath, CancellationToken token = default )
    {
        using TelemetrySpan    telemetrySpan = TelemetrySpan.Create();
        await using FileStream zipToOpen     = File.Create(zipFilePath.FullPath);
        using ZipArchive       archive       = new(zipToOpen, ZipArchiveMode.Update);

        foreach ( LocalFile file in GetFiles() )
        {
            ZipArchiveEntry    entry  = archive.CreateEntry(file.FullPath);
            await using Stream stream = entry.Open();

            using MemoryStream data = await file.ReadAsync()
                                                .AsStream(token);

            await data.CopyToAsync(stream, token);
        }

        return zipFilePath;
    }
    public async Task<LocalFile> ZipAsync( LocalFile zipFilePath, string searchPattern, CancellationToken token = default )
    {
        using TelemetrySpan    telemetrySpan = TelemetrySpan.Create();
        await using FileStream zipToOpen     = File.Create(zipFilePath.FullPath);
        using ZipArchive       archive       = new(zipToOpen, ZipArchiveMode.Update);

        foreach ( LocalFile file in GetFiles(searchPattern) )
        {
            ZipArchiveEntry    entry  = archive.CreateEntry(file.FullPath);
            await using Stream stream = entry.Open();

            ReadOnlyMemory<byte> data = await file.ReadAsync()
                                                  .AsMemory(token);

            await stream.WriteAsync(data, token);
        }

        return zipFilePath;
    }
    public async Task<LocalFile> ZipAsync( LocalFile zipFilePath, string searchPattern, SearchOption searchOption, CancellationToken token = default )
    {
        using TelemetrySpan    telemetrySpan = TelemetrySpan.Create();
        await using FileStream zipToOpen     = File.Create(zipFilePath.FullPath);
        using ZipArchive       archive       = new(zipToOpen, ZipArchiveMode.Update);

        foreach ( LocalFile file in GetFiles(searchPattern, searchOption) )
        {
            ZipArchiveEntry    entry  = archive.CreateEntry(file.FullPath);
            await using Stream stream = entry.Open();

            ReadOnlyMemory<byte> data = await file.ReadAsync()
                                                  .AsMemory(token);

            await stream.WriteAsync(data, token);
        }

        return zipFilePath;
    }
    public async Task<LocalFile> ZipAsync( LocalFile zipFilePath, string searchPattern, EnumerationOptions enumerationOptions, CancellationToken token = default )
    {
        using TelemetrySpan    telemetrySpan = TelemetrySpan.Create();
        await using FileStream zipToOpen     = File.Create(zipFilePath.FullPath);
        using ZipArchive       archive       = new(zipToOpen, ZipArchiveMode.Update);

        foreach ( LocalFile file in GetFiles(searchPattern, enumerationOptions) )
        {
            ZipArchiveEntry    entry  = archive.CreateEntry(file.FullPath);
            await using Stream stream = entry.Open();

            ReadOnlyMemory<byte> data = await file.ReadAsync()
                                                  .AsMemory(token);

            await stream.WriteAsync(data, token);
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


    /// <summary> Deletes subdirectories and files. This occurs on another thread in Windows, and is not blocking. </summary>
    /// <param name="recursive"> </param>
    /// <exception cref="UnauthorizedAccessException"> </exception>
    /// <exception cref="DirectoryNotFoundException"> </exception>
    /// <exception cref="IOException"> </exception>
    /// <exception cref="SecurityException"> </exception>
    public void Delete( bool recursive ) => Info.Delete(recursive);


    /// <summary> Deletes subdirectories and files. </summary>
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
    public void Zip( in LocalFile output, in CompressionLevel compression = CompressionLevel.Optimal ) => Zip(output, Encoding.Default, compression);
    /// <summary> </summary>
    /// <param name="output"> file path to write the zip to </param>
    /// <param name="compression"> Defaults to <see cref="CompressionLevel.Optimal"/> </param>
    /// <param name="encoding"> The encoding used for the file names </param>
    public void Zip( in LocalFile output, Encoding encoding, in CompressionLevel compression = CompressionLevel.Optimal )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        ZipFile.CreateFromDirectory(FullPath, output.FullPath, compression, true, encoding);
    }


    public override int CompareTo( LocalDirectory? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( ReferenceEquals(null, other) ) { return 1; }

        return string.Compare(FullPath, other.FullPath, StringComparison.Ordinal);
    }
    public override bool Equals( LocalDirectory? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return this.IsTempFile() == other.IsTempFile() && FullPath == other.FullPath;
    }
    public override bool Equals( object? other ) => other is LocalDirectory x && Equals(x);
    public override int  GetHashCode()           => HashCode.Combine(FullPath, this.IsTempFile());


    public static bool operator ==( LocalDirectory? left, LocalDirectory? right ) => EqualityComparer<LocalDirectory>.Default.Equals(left, right);
    public static bool operator !=( LocalDirectory? left, LocalDirectory? right ) => !EqualityComparer<LocalDirectory>.Default.Equals(left, right);
    public static bool operator >( LocalDirectory   left, LocalDirectory  right ) => Comparer<LocalDirectory>.Default.Compare(left, right) > 0;
    public static bool operator >=( LocalDirectory  left, LocalDirectory  right ) => Comparer<LocalDirectory>.Default.Compare(left, right) >= 0;
    public static bool operator <( LocalDirectory   left, LocalDirectory  right ) => Comparer<LocalDirectory>.Default.Compare(left, right) < 0;
    public static bool operator <=( LocalDirectory  left, LocalDirectory  right ) => Comparer<LocalDirectory>.Default.Compare(left, right) <= 0;



    public sealed class PathComparer : IComparer<LocalDirectory>
    {
        public static readonly PathComparer Default = new();
        public int Compare( LocalDirectory? x, LocalDirectory? y )
        {
            if ( ReferenceEquals(x, y) ) { return 0; }

            if ( y is null ) { return 1; }

            if ( x is null ) { return -1; }

            return string.Compare(x.FullPath, y.FullPath, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
