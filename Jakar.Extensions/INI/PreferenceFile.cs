#nullable enable
namespace Jakar.Extensions;


public class PreferenceFile : IniConfig, IDisposable, IAsyncDisposable // TODO: Add watcher to update if file changes
{
    private readonly object     _lock = new();
    protected        LocalFile? _file;
    private          string?    _fileName;
    public virtual   string     FileName => _fileName ??= $"{GetType().Name}.ini";

    public LocalFile Path
    {
        get => _file ??= LocalDirectory.CurrentDirectory.Join(FileName);
        protected set => _file = value;
    }


    public PreferenceFile() : this(StringComparer.OrdinalIgnoreCase) { }
    public PreferenceFile( IEqualityComparer<string>                  comparer ) : base(comparer) => Load();
    public PreferenceFile( IDictionary<string, Section>               dictionary ) : base(dictionary) { }
    public PreferenceFile( IDictionary<string, Section>               dictionary, IEqualityComparer<string> comparer ) : base(dictionary, comparer) { }
    public PreferenceFile( IEnumerable<KeyValuePair<string, Section>> collection ) : base(collection) { }
    public PreferenceFile( IEnumerable<KeyValuePair<string, Section>> collection, IEqualityComparer<string> comparer ) : base(collection, comparer) { }
    public virtual void Dispose( bool disposing )
    {
        if ( !disposing ) { return; }

        Task.Run(async () => await DisposeAsync()).Wait();
    }
    public virtual async ValueTask DisposeAsync()
    {
        await SaveAsync().ConfigureAwait(false);

        _file?.Dispose();
        _file = null;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    public static PreferenceFile Create( LocalFile file )
    {
        ReadOnlySpan<char> content = file.Read().AsSpan();
        PreferenceFile     ini     = From<PreferenceFile>(content) ?? new PreferenceFile();
        ini.Path = file;
        return ini;
    }
    public static async Task<PreferenceFile> CreateAsync( LocalFile file )
    {
        string         content = await file.ReadAsync().AsString();
        PreferenceFile ini     = From<PreferenceFile>(content) ?? new PreferenceFile();
        ini.Path = file;
        return ini;
    }


    protected Task Load() => Task.Run(LoadAsync);


    protected virtual async Task LoadAsync()
    {
        IniConfig? cfg = await ReadFromFileAsync(Path).ConfigureAwait(false);
        if ( cfg is null ) { return; }

        foreach ( KeyValuePair<string, Section> pair in cfg ) { Add(pair); }
    }


    protected Task Save() => Task.Run(SaveAsync);
    protected virtual async Task SaveAsync() => await WriteToFile(Path).ConfigureAwait(false);
}
