namespace Jakar.Extensions;


public abstract class PreferenceFile<TSelf> : BaseClass<TSelf>, IAsyncDisposable
    where TSelf : PreferenceFile<TSelf>, IJsonModel<TSelf>, IEqualComparable<TSelf>, new()
{
    private readonly Lock              __lock = new();
    protected        DateTime          _lastWriteTimeUtc;
    protected        IniConfig?        _config;
    private readonly LocalFile         __file = $"{typeof(TSelf).Name}.ini";
    protected        LocalFileWatcher? _watcher;


    public virtual IniConfig Config
    {
        get
        {
            lock ( __lock )
            {
                if ( _config is not null && LastWriteTimeUtc > _lastWriteTimeUtc ) { return _config; }

                _config           = IniConfig.ReadFromFile(__file);
                _lastWriteTimeUtc = LastWriteTimeUtc;
                return _config;
            }
        }
        protected set
        {
            lock ( __lock ) { SetProperty(ref _config, value); }
        }
    }
    public LocalFile File
    {
        get => __file;
        init
        {
            __file = value;
            if ( string.IsNullOrWhiteSpace(__file.DirectoryName) ) { return; }

            _watcher         =  new LocalFileWatcher(__file.Parent);
            _watcher.Changed += WatcherOnChanged;
        }
    }


    protected internal DateTime LastWriteTimeUtc => __file.Info.LastWriteTimeUtc;

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
        GC.SuppressFinalize(this);
    }


    public virtual async    Task SaveAsync() => await Config.WriteToFile(__file);
    protected virtual async Task LoadAsync() => Config = await IniConfig.ReadFromFileAsync(__file);
    protected virtual void WatcherOnChanged( object sender, FileSystemEventArgs e )
    {
        if ( !string.Equals(e.Name, __file.Name, StringComparison.Ordinal) ) { return; }

        _ = LoadAsync();
    }


    public static TSelf Create()                   => new();
    public static TSelf Create( IniConfig config ) => new() { Config = config };
    public static async ValueTask<TSelf> CreateAsync()
    {
        TSelf result = new();
        await result.LoadAsync();
        return result;
    }
    public static TSelf Create( LocalFile file ) => new() { File = file };
    public static async ValueTask<TSelf> CreateAsync( LocalFile file )
    {
        TSelf result = Create(file);
        await result.LoadAsync();
        return result;
    }
}
