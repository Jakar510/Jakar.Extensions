namespace Jakar.Extensions;


public interface IFilePaths : IScreenShotAddress, IDisposable
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


    public LocalFile      AccountsFile    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public LocalFile      AppCacheZipFile { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public LocalDirectory AppData         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public LocalFile      AppDataZipFile  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public LocalFile      AppStateFile    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public LocalDirectory Cache           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public LocalFile      CrashFile       { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public LocalFile      FeedbackFile    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public LocalFile      IncomingFile    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public LocalDirectory Logs            { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public LocalFile      LogsFile        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public LocalFile      OutgoingFile    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public LocalFile      Screenshot      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    public LocalFile      ZipLogsFile     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }


    public IEnumerable<LocalFile> GetFiles();
    public IEnumerable<LocalFile> GetFiles( bool includeLogs, bool includeCache, bool includeData, bool includeScreenshot, bool includeState );
}



public class FilePaths : IFilePaths
{
    public LocalFile      AccountsFile      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalFile      AppCacheZipFile   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalDirectory AppData           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalFile      AppDataZipFile    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalFile      AppStateFile      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalDirectory Cache             { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalFile      CrashFile         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalFile      FeedbackFile      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalFile      IncomingFile      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalDirectory Logs              { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalFile      LogsFile          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalFile      OutgoingFile      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalFile      Screenshot        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalFile      ZipLogsFile       { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalFile?     ScreenShotAddress { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }


    public FilePaths() : this( LocalDirectory.CurrentDirectory ) { }
    public FilePaths( LocalDirectory currentDirectory ) : this( Path.Join( currentDirectory, IFilePaths.APP_DATA_DIRECTORY ), Path.Join( currentDirectory, IFilePaths.CACHE_DIRECTORY ) ) { }
    public FilePaths( LocalDirectory appData, LocalDirectory cache )
    {
        AppData         = appData;
        Cache           = cache;
        Logs            = Path.Join( appData, IFilePaths.LOGS_DIRECTORY );
        LogsFile        = Logs.Join( IFilePaths.LOGS_FILE );
        AccountsFile    = appData.Join( IFilePaths.ACCOUNTS_FILE );
        AppCacheZipFile = appData.Join( IFilePaths.APP_CACHE_ZIP_FILE );
        ZipLogsFile     = cache.Join( IFilePaths.LOGS_ZIP_FILE_NAME );
        AppDataZipFile  = cache.Join( IFilePaths.APP_DATA_ZIP_FILE );
        AppStateFile    = cache.Join( IFilePaths.APP_STATE_FILE );
        CrashFile       = cache.Join( IFilePaths.CRASH_DATA );
        FeedbackFile    = cache.Join( IFilePaths.FEEDBACK_FILE );
        IncomingFile    = cache.Join( IFilePaths.INCOMING_FILE );
        OutgoingFile    = cache.Join( IFilePaths.OUTGOING_FILE );
        Screenshot      = cache.Join( IFilePaths.SCREEN_SHOT_FILE );
        Directory.CreateDirectory( cache );
        Directory.CreateDirectory( appData );
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
        ZipLogsFile.Dispose();
        ScreenShotAddress?.Dispose();
        AppData.Dispose();
        Cache.Dispose();
        Logs.Dispose();
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


    public virtual Task<LocalFile> ZipCache()                                                                       => Zip( Cache,   AppCacheZipFile );
    public virtual Task<LocalFile> ZipData()                                                                        => Zip( AppData, AppDataZipFile );
    public static  Task<LocalFile> Zip( LocalDirectory input, LocalFile output, CancellationToken token = default ) => input.ZipAsync( output, token );


    public IEnumerable<LocalFile> GetFiles() => GetFiles( true, true, true, true, true );
    public IEnumerable<LocalFile> GetFiles( bool includeLogs, bool includeCache, bool includeData, bool includeScreenshot, bool includeState )
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

            if ( ZipLogsFile.Exists ) { yield return ZipLogsFile; }
        }

        if ( includeScreenshot && Screenshot.Exists ) { yield return Screenshot; }

        if ( includeData && AppDataZipFile.Exists ) { yield return AppDataZipFile; }

        if ( includeCache && AppCacheZipFile.Exists ) { yield return AppCacheZipFile; }

        if ( CrashFile.Exists ) { yield return CrashFile; }
    }
}
