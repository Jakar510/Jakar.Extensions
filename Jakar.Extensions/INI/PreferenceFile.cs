namespace Jakar.Extensions;


public class PreferenceFile : ObservableClass, IAsyncDisposable // TODO: Add watcher to update if file changes
{
    private readonly object     _lock = new();
    protected        DateTime   _lastWriteTimeUtc;
    private          IniConfig? _config;
    protected        LocalFile? _file;
    public IniConfig Config
    {
        get
        {
            lock (_lock)
            {
                if ( LastWriteTimeUtc <= _lastWriteTimeUtc ) { return _config ??= IniConfig.ReadFromFile( File ); }

                _lastWriteTimeUtc = LastWriteTimeUtc;
                return _config = IniConfig.ReadFromFile( File );
            }
        }
        protected set
        {
            lock (_lock) { SetProperty( ref _config, value ); }
        }
    }
    public LocalFile File => _file ??= $"{GetType().Name}.ini";


    protected internal DateTime LastWriteTimeUtc => File.Info.LastWriteTimeUtc;


    public PreferenceFile() { }
    public PreferenceFile( LocalFile file ) => _file = file;


    public static PreferenceFile Create() => new();
    public static async ValueTask<PreferenceFile> CreateAsync()
    {
        PreferenceFile result = new();
        await result.LoadAsync();
        return result;
    }
    public static PreferenceFile Create( LocalFile file ) => new(file);
    public static async ValueTask<PreferenceFile> CreateAsync( LocalFile file )
    {
        PreferenceFile result = new( file );
        await result.LoadAsync();
        return result;
    }
    protected ValueTask SaveAsync() => Config.WriteToFile( File );


    protected async ValueTask<IniConfig> LoadAsync() => Config = await IniConfig.ReadFromFileAsync( File );


    public virtual async ValueTask DisposeAsync()
    {
        await SaveAsync();
        _file?.Dispose();
        _file = null;
    }
}
