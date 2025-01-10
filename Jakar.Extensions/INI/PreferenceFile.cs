namespace Jakar.Extensions;


public abstract class PreferenceFile<TClass> : ObservableClass, IAsyncDisposable
    where TClass : PreferenceFile<TClass>, new()
{
#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#else
    private readonly object _lock = new();
#endif
    protected        DateTime                _lastWriteTimeUtc;
    protected        IniConfig?              _config;
    private readonly LocalFile               _file = $"{typeof(TClass).Name}.ini";
    protected        LocalDirectory.Watcher? _watcher;


    public virtual IniConfig Config
    {
        get
        {
            lock (_lock)
            {
                if ( _config is not null && LastWriteTimeUtc > _lastWriteTimeUtc ) { return _config; }

                _config           = IniConfig.ReadFromFile( _file );
                _lastWriteTimeUtc = LastWriteTimeUtc;
                return _config;
            }
        }
        protected set
        {
            lock (_lock) { SetProperty( ref _config, value ); }
        }
    }
    public LocalFile File
    {
        get => _file;
        init
        {
            _file = value;
            if ( string.IsNullOrWhiteSpace( _file.DirectoryName ) ) { return; }

            _watcher         =  new LocalDirectory.Watcher( _file.DirectoryName );
            _watcher.Changed += WatcherOnChanged;
        }
    }


    protected internal DateTime LastWriteTimeUtc => _file.Info.LastWriteTimeUtc;

    protected PreferenceFile() { }
    public virtual async ValueTask DisposeAsync()
    {
        if ( _watcher is not null )
        {
            _watcher.Changed -= WatcherOnChanged;
            _watcher.Dispose();
            _watcher = null;
        }

        await SaveAsync();
        GC.SuppressFinalize( this );
    }


    public virtual async    Task SaveAsync() => await Config.WriteToFile( _file );
    protected virtual async Task LoadAsync() => Config = await IniConfig.ReadFromFileAsync( _file );
    protected virtual void WatcherOnChanged( object sender, FileSystemEventArgs e )
    {
        if ( string.Equals( e.Name, _file.Name, StringComparison.Ordinal ) is false ) { return; }

        _ = LoadAsync();
    }


    public static TClass Create()                   => new();
    public static TClass Create( IniConfig config ) => new() { Config = config };
    public static async ValueTask<TClass> CreateAsync()
    {
        TClass result = new();
        await result.LoadAsync();
        return result;
    }
    public static TClass Create( LocalFile file ) => new() { File = file };
    public static async ValueTask<TClass> CreateAsync( LocalFile file )
    {
        TClass result = Create( file );
        await result.LoadAsync();
        return result;
    }
}
