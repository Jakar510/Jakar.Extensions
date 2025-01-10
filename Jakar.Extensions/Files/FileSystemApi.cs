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
    public             LocalFile      FeedbackFile      { get; }
    public             LocalFile      IncomingFile      { get; }
    public             LocalFile      OutgoingFile      { get; }
    public             LocalFile      ScreenShot        { get; }
    public             LocalFile      ZipFile           { get; }


    protected BaseFileSystemApi()
    {
        AppDataDirectory = _AppDataDirectory;
        CacheDirectory   = _CacheDirectory;
        AccountsFile     = AppDataDirectory.Join( "accounts.json" );
        ZipFile          = AppDataDirectory.Join( "AppData.zip" );
        AppStateFile     = CacheDirectory.Join( "AppState.json" );
        DebugFile        = CacheDirectory.Join( "debug.txt" );
        FeedbackFile     = CacheDirectory.Join( "feedback.json" );
        IncomingFile     = CacheDirectory.Join( "Incoming.json" );
        OutgoingFile     = CacheDirectory.Join( "Outgoing.json" );
        ScreenShot       = CacheDirectory.Join( "ScreenShot.png" );
    }
    public void ClearCache()
    {
        foreach ( LocalFile file in CacheDirectory.GetFiles() ) { file.Delete(); }
    }


    public async ValueTask<LocalFile> SaveFileAsync( string filename, Stream stream, CancellationToken token )
    {
        LocalFile file = CacheDirectory.Join( filename );
        await file.WriteAsync( stream, token );
        return file;
    }
    public async ValueTask<LocalFile> SaveFileAsync( string filename, byte[] payload, CancellationToken token )
    {
        LocalFile file = CacheDirectory.Join( filename );
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
