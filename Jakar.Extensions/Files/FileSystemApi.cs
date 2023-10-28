namespace Jakar.Extensions;


public abstract class BaseFileSystemApi : IFilePaths
{
    protected abstract string         _AppDataDirectory { get; }
    protected abstract string         _CacheDirectory   { get; }
    public             LocalFile      AccountsFile      { get; }
    public             LocalDirectory AppDataDirectory  { get; }
    public             LocalFile      AppStateFile      { get; }
    public             LocalDirectory CacheDirectory    { get; }
    public             LocalFile      DebugFile         { get; }
    public             LocalFile      FeedBackFile      { get; }
    public             LocalFile      IncomingFile      { get; }
    public             LocalFile      OutgoingFile      { get; }
    public             LocalFile      ScreenShot        { get; }
    public             LocalFile      ZipFile           { get; }


    protected BaseFileSystemApi()
    {
        AppDataDirectory = _AppDataDirectory;
        CacheDirectory   = _CacheDirectory;
        AccountsFile     = GetAppDataPath( "accounts.json" );
        ZipFile          = GetAppDataPath( "AppData.zip" );
        AppStateFile     = GetCacheDataPath( "AppState.json" );
        DebugFile        = GetCacheDataPath( "debug.txt" );
        FeedBackFile     = GetCacheDataPath( "feedback.json" );
        IncomingFile     = GetCacheDataPath( "Incoming.json" );
        OutgoingFile     = GetCacheDataPath( "Outgoing.json" );
        ScreenShot       = GetCacheDataPath( "ScreenShot.png" );
    }


    public LocalFile GetAppDataPath( string   file ) => AppDataDirectory.Join( file );
    public LocalFile GetCacheDataPath( string file ) => CacheDirectory.Join( file );


    public async ValueTask<LocalFile> SaveFileAsync( string filename, Stream stream, CancellationToken token )
    {
        LocalFile file = GetCacheDataPath( filename );
        await file.WriteAsync( stream, token );
        return file;
    }
    public async ValueTask<LocalFile> SaveFileAsync( string filename, byte[] payload, CancellationToken token )
    {
        LocalFile file = GetCacheDataPath( filename );
        await file.WriteAsync( payload, token );
        return file;
    }


    public void CreateZipCache()
    {
        if ( ZipFile.Exists ) { ZipFile.Delete(); }

        ZipCache();
    }
    public void ZipCache( CompressionLevel level                        = CompressionLevel.Optimal ) => ZipCache( _CacheDirectory, level );
    public void ZipCache( LocalDirectory   path, CompressionLevel level = CompressionLevel.Optimal ) => System.IO.Compression.ZipFile.CreateFromDirectory( path.FullPath, ZipFile.FullPath, level, true, Encoding.UTF8 );
    public void ZipCache( string           path, CompressionLevel level = CompressionLevel.Optimal ) => System.IO.Compression.ZipFile.CreateFromDirectory( path,          ZipFile.FullPath, level, true, Encoding.UTF8 );
}
