namespace Jakar.Extensions;


/// <summary>
///     Preferences persisted as JSON (<c> {TSelf}.json </c>): declare them as typed properties on <typeparamref name="TSelf"/>; keys it doesn't declare are kept in
///     <see cref="BaseClass.AdditionalData"/> (case-insensitive), so settings written by another version of the app survive a load/save round trip.
///     <code>
///     [JsonModel(typeof(AppJsonContext))]
///     public sealed partial class AppSettings : PreferenceFile&lt;AppSettings&gt;
///     {
///         public string Theme { get; set => SetProperty(ref field, value); } = "Dark";
///     }
///     </code>
///     <para> Fully source generated (AOT-safe). Reloads when the file changes on disk: a fresh instance is read and its values are copied in under a lock, so the live instance is never half-updated. </para>
///     <para> Migration from 10.x: if <c> {TSelf}.json </c> doesn't exist but <c> {TSelf}.ini </c> does, the first load imports it (sections become nested objects) and saves JSON. The <c> .ini </c> file is left in place as a backup. </para>
/// </summary>
public abstract class PreferenceFile<TSelf> : BaseClass<TSelf>, IAsyncDisposable
    where TSelf : PreferenceFile<TSelf>, IJsonModel<TSelf>, IEqualComparable<TSelf>, new()
{
    private readonly Lock               __lock = new();
    private          LocalFile          __file = new($"{typeof(TSelf).Name}.json");
    private          FileSystemWatcher? __watcher;
    private          long               __lastSavedTicks;
    private          int                __saving;


    /// <summary> The JSON file. Setting it starts watching the file for changes. </summary>
    [JsonIgnore] public LocalFile File
    {
        get => __file;
        init
        {
            __file = value;
            Watch();
        }
    }

    /// <summary> <c> {name}.ini </c> next to <see cref="File"/>, imported on the first load when the JSON file doesn't exist yet. </summary>
    [JsonIgnore] public LocalFile LegacyIniFile => new(Path.ChangeExtension(__file.FullPath, ".ini"));


    protected PreferenceFile() { }


    public virtual async ValueTask DisposeAsync()
    {
        FileSystemWatcher? watcher = Interlocked.Exchange(ref __watcher, null);

        if ( watcher is not null )
        {
            watcher.EnableRaisingEvents =  false;
            watcher.Changed             -= OnChanged;
            watcher.Dispose();
        }

        await SaveAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }


    /// <summary> Writes the preferences. Bag entries that duplicate a declared property are dropped first (the property wins). </summary>
    public virtual async Task SaveAsync( CancellationToken token = default )
    {
        string json;

        lock ( __lock )
        {
            RemoveShadowedKeys();
            json = ( (TSelf)this ).ToJson();
        }

        Interlocked.Increment(ref __saving);

        try
        {
            await __file.WriteAsync(json, token).ConfigureAwait(false);
            Interlocked.Exchange(ref __lastSavedTicks, __file.Info.LastWriteTimeUtc.Ticks);
        }
        finally { Interlocked.Decrement(ref __saving); }
    }


    /// <summary> Reads the file (or imports the legacy <c> .ini </c>) and copies the values into this instance. Nothing happens if neither file exists. </summary>
    public virtual async Task LoadAsync( CancellationToken token = default )
    {
        __file.Info.Refresh();

        if ( __file.Exists )
        {
            string json = await __file.ReadAsync().AsString(token).ConfigureAwait(false);
            if ( !string.IsNullOrWhiteSpace(json) ) { CopyFrom(JsonModel.FromJson(json, TSelf.JsonTypeInfo)); }

            return;
        }

        LocalFile ini = LegacyIniFile;
        if ( !ini.Exists ) { return; }

        IniConfig config = await IniConfig.ReadFromFileAsync(ini).ConfigureAwait(false);
        CopyFrom(FromIni(config));
        await SaveAsync(token).ConfigureAwait(false); // through this instance, so the watcher recognizes its own write
    }


    private void CopyFrom( TSelf loaded )
    {
        lock ( __lock )
        {
            JsonModel.CopyProperties(loaded, (TSelf)this, TSelf.JsonTypeInfo);
            _additionalData = Json.Merge(null, loaded.AdditionalData); // case-insensitive bag
        }
    }


    /// <summary> Converts 10.x INI preferences: each section becomes a nested object, <c> true </c>/<c> false </c> become booleans, everything else stays a string (numbers in strings are read by the options' <see cref="JsonNumberHandling.AllowReadingFromString"/>). </summary>
    public static TSelf FromIni( IniConfig config ) => JsonModel.FromJson(config.ToJsonObject().ToJsonString(), TSelf.JsonTypeInfo);


    private void RemoveShadowedKeys()
    {
        if ( _additionalData is null || _additionalData.Count == 0 ) { return; }

        foreach ( JsonPropertyInfo property in TSelf.JsonTypeInfo.Properties )
        {
            if ( !property.IsExtensionData ) { _additionalData.Remove(property.Name); }
        }
    }


    private void Watch()
    {
        string? directory = __file.DirectoryName;
        if ( string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory) ) { return; }

        FileSystemWatcher watcher = new(directory, __file.Name) { NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName };
        watcher.Changed             += OnChanged;
        watcher.EnableRaisingEvents =  true;
        Interlocked.Exchange(ref __watcher, watcher)?.Dispose();
    }


    private void OnChanged( object sender, FileSystemEventArgs e )
    {
        // Ignore the change our own SaveAsync is making, or just made.
        if ( Volatile.Read(ref __saving) > 0 ) { return; }

        __file.Info.Refresh();
        if ( __file.Info.LastWriteTimeUtc.Ticks == Interlocked.Read(ref __lastSavedTicks) ) { return; }

        _ = ReloadAsync();
    }


    private async Task ReloadAsync()
    {
        try { await LoadAsync().ConfigureAwait(false); }
        catch ( Exception e ) when ( e is IOException or JsonException ) { SelfLogger.WriteLine("{Type} reload failed: {Error}", typeof(TSelf).Name, e); }
    }


    public static TSelf Create()                 => new();
    public static TSelf Create( LocalFile file ) => new() { File = file };

    public static async ValueTask<TSelf> CreateAsync( CancellationToken token = default )
    {
        TSelf result = new();
        await result.LoadAsync(token).ConfigureAwait(false);
        return result;
    }

    public static async ValueTask<TSelf> CreateAsync( LocalFile file, CancellationToken token = default )
    {
        TSelf result = Create(file);
        await result.LoadAsync(token).ConfigureAwait(false);
        return result;
    }
}
