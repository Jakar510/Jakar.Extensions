namespace Jakar.Extensions.Models.IniConfiguration;


public class PreferenceFile : IniConfig, IDisposable, IAsyncDisposable // TODO: Add watcher to update if file changes
{
    private readonly object     _lock = new();
    private          LocalFile? _file;
    protected        string?    _fileName;


    public LocalFile Path => _file ??= LocalDirectory.CurrentDirectory.Join(FileName());

    public PreferenceFile() : this(StringComparer.OrdinalIgnoreCase) => Load();
    public PreferenceFile( IEqualityComparer<string>                  comparer ) : base(comparer) => Load();
    public PreferenceFile( IDictionary<string, Section>               dictionary ) : base(dictionary) { }
    public PreferenceFile( IDictionary<string, Section>               dictionary, IEqualityComparer<string> comparer ) : base(dictionary, comparer) { }
    public PreferenceFile( IEnumerable<KeyValuePair<string, Section>> collection ) : base(collection) { }
    public PreferenceFile( IEnumerable<KeyValuePair<string, Section>> collection, IEqualityComparer<string> comparer ) : base(collection, comparer) { }
    public virtual string FileName() => _fileName ??= $"{GetType().Name}.json";


    protected Task Load() => Task.Run(LoadAsync);


    [SuppressMessage("ReSharper", "UseDeconstruction", Justification = "Support NetFramework")]
    protected virtual async Task LoadAsync()
    {
        IniConfig? cfg = await ReadFromFile(Path).ConfigureAwait(false);
        if ( cfg is null ) { return; }

        foreach ( KeyValuePair<string, Section> pair in cfg ) { Add(pair); }
    }


    protected Task Save() => Task.Run(SaveAsync);
    protected virtual async Task SaveAsync() => await WriteToFile(Path).ConfigureAwait(false);

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
}
