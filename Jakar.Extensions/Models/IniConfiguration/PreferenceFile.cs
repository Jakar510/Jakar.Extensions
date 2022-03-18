namespace Jakar.Extensions.Models.IniConfiguration;


public class PreferenceFile : IniConfig, IDisposable, IAsyncDisposable
{
    private readonly object     _lock = new();
    private          LocalFile? _file;
    protected        string?    _fileName;

    public PreferenceFile() : this(StringComparer.OrdinalIgnoreCase) => Load();
    public PreferenceFile( IEqualityComparer<string>                  comparer ) : base(comparer) => Load();
    public PreferenceFile( IDictionary<string, Section>               dictionary ) : base(dictionary) { }
    public PreferenceFile( IDictionary<string, Section>               dictionary, IEqualityComparer<string> comparer ) : base(dictionary, comparer) { }
    public PreferenceFile( IEnumerable<KeyValuePair<string, Section>> collection ) : base(collection) { }
    public PreferenceFile( IEnumerable<KeyValuePair<string, Section>> collection, IEqualityComparer<string> comparer ) : base(collection, comparer) { }


    public         LocalFile Path       => _file ??= LocalDirectory.CurrentDirectory.Join(FileName());
    public virtual string    FileName() => _fileName ??= $"{GetType().Name}.json";


    protected void Load() => Task.Run(LoadAsync);

    protected virtual async Task LoadAsync()
    {
        IniConfig? cfg = await ReadFromFile(Path).ConfigureAwait(false);
        if ( cfg is null ) { return; }

        foreach ( ( string? key, Section? section ) in cfg ) { Add(key, section); }
    }


    protected               Task Save()      => Task.Run(SaveAsync);
    protected virtual async Task SaveAsync() => await WriteToFile(Path).ConfigureAwait(false);


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

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
}
