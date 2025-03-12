namespace Jakar.Extensions;


public class FilePaths : IDisposable
{
    public const string ACCOUNTS_FILE      = "Accounts.json";
    public const string APP_CACHE_ZIP_FILE = "App.Cache.zip";
    public const string APP_DATA_DIRECTORY = "AppData";
    public const string APP_DATA_ZIP_FILE  = "App.Data.zip";
    public const string APP_STATE_FILE     = "AppState.json";
    public const string CACHE_DIRECTORY    = "Cache";
    public const string CRASH_DATA         = "Crash.dat";
    public const string FEEDBACK_FILE      = "Feedback.txt";
    public const string INCOMING_FILE      = "Incoming.json";
    public const string LOGS_DIRECTORY     = "Logs";
    public const string LOGS_FILE          = "App.Logs";
    public const string LOGS_ZIP_FILE_NAME = "App.logs.zip";
    public const string OUTGOING_FILE      = "Outgoing.json";
    public const string SCREEN_SHOT_FILE   = "ScreenShot.png";


    protected       LocalFile?                              _screenShotAddress;
    public readonly LocalFile                               AccountsFile;
    public readonly LocalFile                               AppCacheZipFile;
    public readonly LocalDirectory                          AppData;
    public readonly LocalDirectory.Watcher                  AppDataWatcher;
    public readonly LocalFile                               AppDataZipFile;
    public readonly LocalFile                               AppStateFile;
    public readonly LocalDirectory                          Cache;
    public readonly LocalDirectory.Watcher                  CacheWatcher;
    public readonly LocalFile                               CrashFile;
    public readonly LocalFile                               FeedbackFile;
    public readonly LocalFile                               IncomingFile;
    public readonly LocalDirectory                          Logs;
    public readonly LocalDirectory.Watcher                  LogWatcher;
    public readonly LocalFile                               LogsFile;
    public readonly LocalFile                               OutgoingFile;
    public readonly LocalFile                               Screenshot;
    public readonly LocalFile                               LogsZipFile;
    public readonly ConcurrentDictionary<string, LocalFile> AdditionalFiles = new(Environment.ProcessorCount, DEFAULT_CAPACITY, StringComparer.Ordinal);


    public LocalFile? ScreenShotAddress
    {
        get => _screenShotAddress ??= Cache.Join( SCREEN_SHOT_FILE );
        set
        {
            _screenShotAddress?.Dispose();
            _screenShotAddress = value?.SetTemporary();
        }
    }


    public FilePaths() : this( LocalDirectory.CurrentDirectory ) { }
    public FilePaths( LocalDirectory currentDirectory ) : this( currentDirectory.Combine( APP_DATA_DIRECTORY ), currentDirectory.Combine( CACHE_DIRECTORY ) ) { }
    public FilePaths( LocalDirectory appData, LocalDirectory cache )
    {
        AppData         = appData;
        AppDataWatcher  = new LocalDirectory.Watcher( AppData );
        Cache           = cache;
        CacheWatcher    = new LocalDirectory.Watcher( Cache );
        Logs            = Path.Join( appData, LOGS_DIRECTORY );
        LogWatcher      = new LocalDirectory.Watcher( Logs );
        LogsFile        = Logs.Join( LOGS_FILE );
        AccountsFile    = appData.Join( ACCOUNTS_FILE );
        AppCacheZipFile = appData.Join( APP_CACHE_ZIP_FILE );
        LogsZipFile     = cache.Join( LOGS_ZIP_FILE_NAME );
        AppDataZipFile  = cache.Join( APP_DATA_ZIP_FILE );
        AppStateFile    = cache.Join( APP_STATE_FILE );
        CrashFile       = cache.Join( CRASH_DATA );
        FeedbackFile    = cache.Join( FEEDBACK_FILE );
        IncomingFile    = cache.Join( INCOMING_FILE );
        OutgoingFile    = cache.Join( OUTGOING_FILE );
        Screenshot      = cache.Join( SCREEN_SHOT_FILE );
        Directory.CreateDirectory( Cache );
        Directory.CreateDirectory( AppData );
        Directory.CreateDirectory( Logs );
    }

    protected virtual void Dispose( bool disposing )
    {
        if ( disposing is false ) { return; }

        AccountsFile.Dispose();
        AppCacheZipFile.Dispose();
        AppDataZipFile.Dispose();
        AppStateFile.Dispose();
        CrashFile.Dispose();
        FeedbackFile.Dispose();
        IncomingFile.Dispose();
        LogsFile.Dispose();
        OutgoingFile.Dispose();
        Screenshot.Dispose();
        LogsZipFile.Dispose();
        ScreenShotAddress?.Dispose();
        AppData.Dispose();
        Cache.Dispose();
        Logs.Dispose();
        foreach ( LocalFile file in AdditionalFiles.Values ) { file.Dispose(); }

        AdditionalFiles.Clear();
    }
    public void Dispose()
    {
        Dispose( true );
        GC.SuppressFinalize( this );
    }


    public void ClearCache()
    {
        foreach ( LocalFile file in Cache.GetFiles() ) { file.Delete(); }
    }


    public ReadOnlyMemory<byte>       ZipLogs()       => Zip( Logs );
    public Task<ReadOnlyMemory<byte>> ZipLogsAsync()  => Task.Run( ZipLogs );
    public ReadOnlyMemory<byte>       ZipCache()      => Zip( Cache );
    public Task<ReadOnlyMemory<byte>> ZipCacheAsync() => Task.Run( ZipCache );
    public ReadOnlyMemory<byte>       ZipData()       => Zip( AppData );
    public Task<ReadOnlyMemory<byte>> ZipDataAsync()  => Task.Run( ZipData );
    public static ReadOnlyMemory<byte> Zip( LocalDirectory? directory )
    {
        if ( directory is null || directory.DoesNotExist ) { return ReadOnlyMemory<byte>.Empty; }

        System.Diagnostics.Debug.Assert( directory.Exists );
        using MemoryStream destination = new(10240);
        ZipFile.CreateFromDirectory( directory.FullPath, destination, CompressionLevel.SmallestSize, true, Encoding.Default );

        return destination.ToArray();
    }


    public async ValueTask<LocalFile> SaveFileAsync( string fileName, Stream stream, CancellationToken token )
    {
        LocalFile file = Cache.Join( fileName );
        await file.WriteAsync( stream, token );
        return file;
    }
    public async ValueTask<LocalFile> SaveFileAsync( string fileName, ReadOnlyMemory<byte> payload, CancellationToken token )
    {
        LocalFile file = Cache.Join( fileName );
        await file.WriteAsync( payload, token );
        return file;
    }


    public        Task<LocalFile> ZipLogsToFile()                                                                  => Zip( Logs,    LogsZipFile );
    public        Task<LocalFile> ZipCacheToFile()                                                                 => Zip( Cache,   AppCacheZipFile );
    public        Task<LocalFile> ZipDataToFile()                                                                  => Zip( AppData, AppDataZipFile );
    public static Task<LocalFile> Zip( LocalDirectory input, LocalFile output, CancellationToken token = default ) => input.ZipAsync( output, token );


    public IEnumerable<string> GetFilesPaths()
    {
        yield return FeedbackFile.FullPath;
        yield return IncomingFile.FullPath;
        yield return OutgoingFile.FullPath;
        yield return AccountsFile.FullPath;
        yield return AppStateFile.FullPath;
        yield return LogsFile.FullPath;
        yield return Screenshot.FullPath;
        yield return LogsZipFile.FullPath;
        yield return AppDataZipFile.FullPath;
        yield return AppCacheZipFile.FullPath;
        yield return CrashFile.FullPath;
    }
    public IEnumerable<LocalFile> GetFiles()
    {
        if ( FeedbackFile.Exists ) { yield return FeedbackFile; }

        if ( IncomingFile.Exists ) { yield return IncomingFile; }

        if ( OutgoingFile.Exists ) { yield return OutgoingFile; }

        if ( AccountsFile.Exists ) { yield return AccountsFile; }

        if ( AppStateFile.Exists ) { yield return AppStateFile; }

        if ( LogsFile.Exists ) { yield return LogsFile; }

        if ( LogsZipFile.Exists ) { yield return LogsZipFile; }

        if ( Screenshot.Exists ) { yield return Screenshot; }

        if ( AppDataZipFile.Exists ) { yield return AppDataZipFile; }

        if ( AppCacheZipFile.Exists ) { yield return AppCacheZipFile; }

        if ( CrashFile.Exists ) { yield return CrashFile; }
    }
    public virtual IEnumerable<LocalFile> GetFiles( bool overrideIfExists )
    {
        if ( overrideIfExists || FeedbackFile.Exists ) { yield return FeedbackFile; }

        if ( overrideIfExists || IncomingFile.Exists ) { yield return IncomingFile; }

        if ( overrideIfExists || OutgoingFile.Exists ) { yield return OutgoingFile; }

        if ( overrideIfExists || AccountsFile.Exists ) { yield return AccountsFile; }

        if ( overrideIfExists || AppStateFile.Exists ) { yield return AppStateFile; }

        if ( overrideIfExists || LogsFile.Exists ) { yield return LogsFile; }

        if ( overrideIfExists || LogsZipFile.Exists ) { yield return LogsZipFile; }

        if ( overrideIfExists || Screenshot.Exists ) { yield return Screenshot; }

        if ( overrideIfExists || AppDataZipFile.Exists ) { yield return AppDataZipFile; }

        if ( overrideIfExists || AppCacheZipFile.Exists ) { yield return AppCacheZipFile; }

        if ( overrideIfExists || CrashFile.Exists ) { yield return CrashFile; }
    }
    public virtual IEnumerable<LocalFile> GetFiles( bool includeLogs, bool includeCache, bool includeData, bool includeScreenshot, bool includeState )
    {
        if ( includeState )
        {
            if ( FeedbackFile.Exists ) { yield return FeedbackFile; }

            if ( IncomingFile.Exists ) { yield return IncomingFile; }

            if ( OutgoingFile.Exists ) { yield return OutgoingFile; }

            if ( AccountsFile.Exists ) { yield return AccountsFile; }

            if ( AppStateFile.Exists ) { yield return AppStateFile; }
        }

        if ( includeLogs )
        {
            if ( LogsFile.Exists ) { yield return LogsFile; }

            if ( LogsZipFile.Exists ) { yield return LogsZipFile; }
        }

        if ( includeScreenshot && Screenshot.Exists ) { yield return Screenshot; }

        if ( includeData && AppDataZipFile.Exists ) { yield return AppDataZipFile; }

        if ( includeCache && AppCacheZipFile.Exists ) { yield return AppCacheZipFile; }

        if ( CrashFile.Exists ) { yield return CrashFile; }
    }
}
