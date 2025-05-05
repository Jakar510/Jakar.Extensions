namespace Jakar.Extensions;


public class FilePaths : BaseClass, IDisposable
{
    public const    string                                   ACCOUNTS_FILE      = "Accounts.json";
    public const    string                                   APP_CACHE_ZIP_FILE = "App.Cache.zip";
    public const    string                                   APP_DATA_DIRECTORY = "AppData";
    public const    string                                   APP_DATA_ZIP_FILE  = "App.Data.zip";
    public const    string                                   APP_STATE_FILE     = "AppState.json";
    public const    string                                   CACHE_DIRECTORY    = "Cache";
    public const    string                                   CRASH_DATA         = "Crash.dat";
    public const    string                                   FEEDBACK_FILE      = "Feedback.txt";
    public const    string                                   INCOMING_FILE      = "Incoming.json";
    public const    string                                   LOGS_DIRECTORY     = "Logs";
    public const    string                                   LOGS_FILE          = "App.Logs";
    public const    string                                   LOGS_ZIP_FILE_NAME = "App.logs.zip";
    public const    string                                   OUTGOING_FILE      = "Outgoing.json";
    public const    string                                   SCREEN_SHOT_FILE   = "ScreenShot.png";
    public readonly LocalDirectory                           AppData;
    public readonly LocalDirectory                           Cache;
    public readonly LocalDirectory                           Logs;
    protected       ConcurrentDictionary<string, LocalFile>? _additionalFiles;
    protected       LocalFile?                               _accountsFile;
    protected       LocalFile?                               _appCacheZipFile;
    protected       LocalFile?                               _appDataZipFile;
    protected       LocalFile?                               _appStateFile;
    protected       LocalFile?                               _crashFile;
    protected       LocalFile?                               _feedbackFile;
    protected       LocalFile?                               _incomingFile;
    protected       LocalFile?                               _logsFile;
    protected       LocalFile?                               _logsZipFile;
    protected       LocalFile?                               _outgoingFile;
    protected       LocalFile?                               _screenshot;
    protected       ReadOnlyMemory<byte>                     _screenshotData;


    public LocalFile                               AccountsFile    { get => GetAccountsFile(); set => _accountsFile = value; }
    public ConcurrentDictionary<string, LocalFile> AdditionalFiles => _additionalFiles ??= new ConcurrentDictionary<string, LocalFile>( Environment.ProcessorCount, DEFAULT_CAPACITY, StringComparer.Ordinal );
    public LocalFile                               AppCacheZipFile { get => GetAppCacheZipFile(); set => _appCacheZipFile = value; }
    public LocalFile                               AppDataZipFile  { get => GetAppDataZipFile();  set => _appDataZipFile = value; }
    public LocalFile                               AppStateFile    { get => GetAppStateFile();    set => _appStateFile = value; }
    public LocalFile                               CrashFile       { get => GetCrashFile();       set => _crashFile = value; }
    public LocalFile                               FeedbackFile    { get => GetFeedbackFile();    set => _feedbackFile = value; }
    public LocalFile                               IncomingFile    { get => GetIncomingFile();    set => _incomingFile = value; }
    public LocalFile                               LogsFile        { get => GetLogsFile();        set => _logsFile = value; }
    public LocalFile                               LogsZipFile     { get => GetLogsZipFile();     set => _logsZipFile = value; }
    public LocalFile                               OutgoingFile    { get => GetOutgoingFile();    set => _outgoingFile = value; }
    public LocalFile Screenshot
    {
        get => GetScreenshotFile();
        set
        {
            _screenshot?.Dispose();
            _screenshot = value?.SetTemporary();
        }
    }
    public ReadOnlyMemory<byte> ScreenshotData { get => _screenshotData; set => _screenshotData = value; }


    public FilePaths() : this( LocalDirectory.CurrentDirectory ) { }
    public FilePaths( LocalDirectory currentDirectory ) : this( currentDirectory.Combine( APP_DATA_DIRECTORY ), currentDirectory.Combine( CACHE_DIRECTORY ) ) { }
    public FilePaths( LocalDirectory appData, LocalDirectory cache ) : this( appData, cache, cache.Combine( LOGS_DIRECTORY ) ) { }
    public FilePaths( LocalDirectory appData, LocalDirectory cache, LocalDirectory logs )
    {
        AppData = appData;
        Cache   = cache;
        Logs    = logs;
        Directory.CreateDirectory( Cache );
        Directory.CreateDirectory( AppData );
        Directory.CreateDirectory( Logs );
    }


    protected virtual void Dispose( bool disposing )
    {
        if ( disposing is false ) { return; }

        ClearAndDispose( ref _accountsFile );
        ClearAndDispose( ref _appCacheZipFile );
        ClearAndDispose( ref _appDataZipFile );
        ClearAndDispose( ref _appStateFile );
        ClearAndDispose( ref _crashFile );
        ClearAndDispose( ref _feedbackFile );
        ClearAndDispose( ref _incomingFile );
        ClearAndDispose( ref _logsFile );
        ClearAndDispose( ref _outgoingFile );
        ClearAndDispose( ref _logsZipFile );
        ClearAndDispose( ref _screenshot );
        AppData.Dispose();
        Cache.Dispose();
        Logs.Dispose();
        _screenshotData = ReadOnlyMemory<byte>.Empty;

        if ( _additionalFiles is null ) { return; }

        foreach ( LocalFile file in _additionalFiles.Values ) { file.Dispose(); }

        _additionalFiles.Clear();
        _additionalFiles = null;
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


    protected LocalFile                  GetAppCacheZipFile() => _appCacheZipFile ??= AppData.Join( APP_CACHE_ZIP_FILE );
    protected LocalFile                  GetAppDataZipFile()  => _appDataZipFile ??= Cache.Join( APP_DATA_ZIP_FILE );
    protected LocalFile                  GetAppStateFile()    => _appStateFile ??= Cache.Join( APP_STATE_FILE );
    protected LocalFile                  GetCrashFile()       => _crashFile ??= Cache.Join( CRASH_DATA );
    protected LocalFile                  GetAccountsFile()    => _accountsFile ??= Cache.Join( ACCOUNTS_FILE );
    protected LocalFile                  GetIncomingFile()    => _incomingFile ??= Cache.Join( INCOMING_FILE );
    protected LocalFile                  GetFeedbackFile()    => _feedbackFile ??= Cache.Join( FEEDBACK_FILE );
    protected LocalFile                  GetLogsFile()        => _logsFile ??= Logs.Join( LOGS_FILE );
    protected LocalFile                  GetLogsZipFile()     => _logsZipFile ??= AppData.Join( LOGS_ZIP_FILE_NAME );
    protected LocalFile                  GetOutgoingFile()    => _outgoingFile ??= Cache.Join( OUTGOING_FILE );
    protected LocalFile                  GetScreenshotFile()  => _screenshot ??= Cache.Join( SCREEN_SHOT_FILE );
    public    ReadOnlyMemory<byte>       ZipLogs()            => Zip( Logs );
    public    Task<ReadOnlyMemory<byte>> ZipLogsAsync()       => Task.Run( ZipLogs );
    public    ReadOnlyMemory<byte>       ZipCache()           => Zip( Cache );
    public    Task<ReadOnlyMemory<byte>> ZipCacheAsync()      => Task.Run( ZipCache );
    public    ReadOnlyMemory<byte>       ZipData()            => Zip( AppData );
    public    Task<ReadOnlyMemory<byte>> ZipDataAsync()       => Task.Run( ZipData );
    public static ReadOnlyMemory<byte> Zip( LocalDirectory? directory )
    {
        if ( directory is null || directory.DoesNotExist ) { return ReadOnlyMemory<byte>.Empty; }

        Debug.Assert( directory.Exists );
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


    public virtual IEnumerable<string> GetFilesPaths()
    {
        yield return FeedbackFile.FullPath;
        yield return IncomingFile.FullPath;
        yield return OutgoingFile.FullPath;
        yield return AccountsFile.FullPath;
        yield return AppStateFile.FullPath;
        yield return LogsFile.FullPath;
        yield return LogsZipFile.FullPath;
        yield return AppDataZipFile.FullPath;
        yield return AppCacheZipFile.FullPath;
        yield return CrashFile.FullPath;
        yield return GetScreenshotFile().FullPath;
    }
    public virtual IEnumerable<LocalFile> GetFiles()
    {
        if ( FeedbackFile.Exists ) { yield return FeedbackFile; }

        if ( IncomingFile.Exists ) { yield return IncomingFile; }

        if ( OutgoingFile.Exists ) { yield return OutgoingFile; }

        if ( AccountsFile.Exists ) { yield return AccountsFile; }

        if ( AppStateFile.Exists ) { yield return AppStateFile; }

        if ( LogsFile.Exists ) { yield return LogsFile; }

        if ( LogsZipFile.Exists ) { yield return LogsZipFile; }

        LocalFile screenshot = GetScreenshotFile();
        if ( screenshot.Exists ) { yield return screenshot; }

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

        LocalFile screenshot = GetScreenshotFile();
        if ( overrideIfExists || screenshot.Exists ) { yield return screenshot; }

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
