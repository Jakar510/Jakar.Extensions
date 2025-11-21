// Jakar.Extensions :: Jakar.Extensions
// 09/19/2025  12:55


namespace Jakar.Extensions;


#pragma warning disable CS1066, CS1584
/// <summary> A collection of files that are  the <see cref="LocalDirectory"/> </summary>
[NotSerializable]
public sealed class LocalFileWatcher : ObservableCollection<LocalFileWatcher, LocalFile>, ICollectionAlerts<LocalFileWatcher, LocalFile>, IEqualComparable<LocalFileWatcher>
{
    private FileSystemWatcher? __watcher;


    public LocalDirectory? Directory
    {
        get;
        set
        {
            if ( __watcher is not null )
            {
                __watcher.EnableRaisingEvents =  false;
                __watcher.Created             -= OnCreated;
                __watcher.Changed             -= OnChanged;
                __watcher.Deleted             -= OnDeleted;
                __watcher.Renamed             -= OnRenamed;
                __watcher.Error               -= OnError;
                __watcher.Dispose();
            }

            SetProperty(ref field, value);
            if ( value is null ) { return; }

            __watcher                     =  new FileSystemWatcher(value.FullPath, SearchFilter) { NotifyFilter = Filters };
            __watcher.Created             += OnCreated;
            __watcher.Changed             += OnChanged;
            __watcher.Deleted             += OnDeleted;
            __watcher.Renamed             += OnRenamed;
            __watcher.Error               += OnError;
            __watcher.EnableRaisingEvents =  true;
            Clear();

            Add(value.GetFiles()
                     .AsSpan());
        }
    }
    public bool EnableRaisingEvents
    {
        get => __watcher?.EnableRaisingEvents is true;
        set
        {
            if ( __watcher is not null ) { __watcher.EnableRaisingEvents = value; }

            OnPropertyChanged();
        }
    }
    public NotifyFilters Filters      { get; set; } = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.CreationTime;
    public string        SearchFilter { get; set; } = "*";


    public event ErrorEventHandler?      Error;
    public event FileSystemEventHandler? Changed;
    public event FileSystemEventHandler? Created;
    public event FileSystemEventHandler? Deleted;
    public event RenamedEventHandler?    Renamed;


    public LocalFileWatcher() { }
    public LocalFileWatcher( IEnumerable<LocalFile>         files ) : this() { Add(files); }
    public LocalFileWatcher( params ReadOnlySpan<LocalFile> files ) : this() { Add(files); }
    public LocalFileWatcher( LocalDirectory?                directory ) : base() => Directory = directory;

    protected override void Dispose( bool disposing )
    {
        base.Dispose(disposing);
        if ( disposing ) { Directory = null; }
    }


    private void OnChanged( object sender, FileSystemEventArgs e )
    {
        LocalFile file = new(e.FullPath);
        AddOrUpdate(file);
        Changed?.Invoke(this, e);
    }
    private void OnCreated( object sender, FileSystemEventArgs e )
    {
        Add(e.FullPath);
        Created?.Invoke(this, e);
    }
    private void OnDeleted( object sender, FileSystemEventArgs e )
    {
        Remove(e.FullPath);
        Deleted?.Invoke(this, e);
    }
    private void OnError( object sender, ErrorEventArgs e ) => Error?.Invoke(this, e);
    private void OnRenamed( object sender, RenamedEventArgs e )
    {
        using ArrayBuffer<LocalFile> arrayBuffer = FilteredValues();
        LocalFile?                   file        = firstOrDefault(in e, arrayBuffer.Span);
        if ( file is not null ) { Remove(file); }

        Add(e.FullPath);
        Renamed?.Invoke(this, e);
        return;

        static LocalFile? firstOrDefault( ref readonly RenamedEventArgs e, params ReadOnlySpan<LocalFile> files )
        {
            foreach ( LocalFile file in files )
            {
                if ( file.FullPath == e.OldFullPath ) { return file; }
            }

            return null;
        }
    }


    public static implicit operator LocalFileWatcher( List<LocalFile>           values ) => new(values);
    public static implicit operator LocalFileWatcher( HashSet<LocalFile>        values ) => new(values);
    public static implicit operator LocalFileWatcher( ConcurrentBag<LocalFile>  values ) => new(values);
    public static implicit operator LocalFileWatcher( Collection<LocalFile>     values ) => new(values);
    public static implicit operator LocalFileWatcher( LocalFile[]               values ) => new(values.AsSpan());
    public static implicit operator LocalFileWatcher( ImmutableArray<LocalFile> values ) => new(values.AsSpan());
    public static implicit operator LocalFileWatcher( ReadOnlyMemory<LocalFile> values ) => new(values.Span);
    public static implicit operator LocalFileWatcher( ReadOnlySpan<LocalFile>   values ) => new(values);


    public override int  GetHashCode()                                                  => RuntimeHelpers.GetHashCode(this);
    public override bool Equals( object?                other )                         => ReferenceEquals(this, other) || ( other is LocalFileWatcher x && Equals(x) );
    public static   bool operator ==( LocalFileWatcher? left, LocalFileWatcher? right ) => EqualityComparer<LocalFileWatcher>.Default.Equals(left, right);
    public static   bool operator !=( LocalFileWatcher? left, LocalFileWatcher? right ) => !EqualityComparer<LocalFileWatcher>.Default.Equals(left, right);
    public static   bool operator >( LocalFileWatcher   left, LocalFileWatcher  right ) => Comparer<LocalFileWatcher>.Default.Compare(left, right) > 0;
    public static   bool operator >=( LocalFileWatcher  left, LocalFileWatcher  right ) => Comparer<LocalFileWatcher>.Default.Compare(left, right) >= 0;
    public static   bool operator <( LocalFileWatcher   left, LocalFileWatcher  right ) => Comparer<LocalFileWatcher>.Default.Compare(left, right) < 0;
    public static   bool operator <=( LocalFileWatcher  left, LocalFileWatcher  right ) => Comparer<LocalFileWatcher>.Default.Compare(left, right) <= 0;
}
