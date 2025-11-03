// Jakar.Extensions :: Jakar.Extensions
// 09/19/2025  12:55


namespace Jakar.Extensions;


#pragma warning disable CS1066, CS1584
/// <summary> A collection of files that are  the <see cref="LocalDirectory"/> </summary>
[NotSerializable]
public sealed class LocalFileWatcher : ObservableCollection<LocalFileWatcher, LocalFile>, ICollectionAlerts<LocalFileWatcher, LocalFile>, IEqualComparable<LocalFileWatcher>
{
    private readonly WeakEventManager   __eventManager = new();
    private          FileSystemWatcher? __watcher;
    private          LocalDirectory?    __directory;


    public static JsonTypeInfo<LocalFileWatcher[]> JsonArrayInfo => JakarExtensionsContext.Default.LocalFileWatcherArray;
    public static JsonSerializerContext            JsonContext   => JakarExtensionsContext.Default;
    public static JsonTypeInfo<LocalFileWatcher>   JsonTypeInfo  => JakarExtensionsContext.Default.LocalFileWatcher;
    public LocalDirectory? Directory
    {
        get => __directory;
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

            SetProperty(ref __directory, value);
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


    public event ErrorEventHandler?      Error   { add => __eventManager.AddEventHandler(( object? sender, ErrorEventArgs      x ) => value?.Invoke(sender ?? this, x)); remove => __eventManager.RemoveEventHandler(( object? sender, ErrorEventArgs      x ) => value?.Invoke(sender ?? this, x)); }
    public event FileSystemEventHandler? Changed { add => __eventManager.AddEventHandler(( object? sender, FileSystemEventArgs x ) => value?.Invoke(sender ?? this, x)); remove => __eventManager.RemoveEventHandler(( object? sender, FileSystemEventArgs x ) => value?.Invoke(sender ?? this, x)); }
    public event FileSystemEventHandler? Created { add => __eventManager.AddEventHandler(( object? sender, FileSystemEventArgs x ) => value?.Invoke(sender ?? this, x)); remove => __eventManager.RemoveEventHandler(( object? sender, FileSystemEventArgs x ) => value?.Invoke(sender ?? this, x)); }
    public event FileSystemEventHandler? Deleted { add => __eventManager.AddEventHandler(( object? sender, FileSystemEventArgs x ) => value?.Invoke(sender ?? this, x)); remove => __eventManager.RemoveEventHandler(( object? sender, FileSystemEventArgs x ) => value?.Invoke(sender ?? this, x)); }
    public event RenamedEventHandler?    Renamed { add => __eventManager.AddEventHandler(( object? sender, RenamedEventArgs    x ) => value?.Invoke(sender ?? this, x)); remove => __eventManager.RemoveEventHandler(( object? sender, RenamedEventArgs    x ) => value?.Invoke(sender ?? this, x)); }


    public LocalFileWatcher() { }
    public LocalFileWatcher( IEnumerable<LocalFile>         files ) : this() { Add(files); }
    public LocalFileWatcher( params ReadOnlySpan<LocalFile> files ) : this() { Add(files); }
    public LocalFileWatcher( LocalDirectory?                directory ) : base() => Directory = directory;


    public override void Dispose()
    {
        Directory = null;
        base.Dispose();
    }


    private void OnChanged( object sender, FileSystemEventArgs e )
    {
        LocalFile file = new(e.FullPath);
        AddOrUpdate(file);
        __eventManager.RaiseEvent(e, nameof(Changed));
    }
    private void OnCreated( object sender, FileSystemEventArgs e )
    {
        Add(e.FullPath);
        __eventManager.RaiseEvent(e, nameof(Created));
    }
    private void OnDeleted( object sender, FileSystemEventArgs e )
    {
        Remove(e.FullPath);
        __eventManager.RaiseEvent(e, nameof(Deleted));
    }
    private void OnError( object sender, ErrorEventArgs e ) => __eventManager.RaiseEvent(e, nameof(Error));
    private void OnRenamed( object sender, RenamedEventArgs e )
    {
        LocalFile? file = this.FirstOrDefault(x => x.FullPath == e.OldFullPath);
        if ( file is not null ) { Remove(file); }

        Add(e.FullPath);
        __eventManager.RaiseEvent(e, nameof(Renamed));
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
