#nullable enable
namespace Jakar.Extensions;


public abstract class BaseFileSystemApi : IFilePaths
{
    protected abstract string _AppDataDirectory { get; }
    protected abstract string _CacheDirectory   { get; }

    public string AccountsFileName => GetAppDataPath( "accounts.json" );


    public string AppStateFileName => GetCacheDataPath( "AppState.json" );
    public string DebugFileName    => GetCacheDataPath( "debug.txt" );
    public string FeedBackFileName => GetCacheDataPath( "feedback.json" );
    public string IncomingFileName => GetCacheDataPath( "Incoming.json" );
    public string OutgoingFileName => GetCacheDataPath( "Outgoing.json" );
    public string ScreenShot       => GetCacheDataPath( "ScreenShot.png" );
    public string ZipFileName      => GetAppDataPath( "AppData.zip" );


    protected BaseFileSystemApi() { }


    public string GetAppDataPath( string   fileName ) => Path.Combine( _AppDataDirectory, fileName );
    public string GetCacheDataPath( string fileName ) => Path.Combine( _CacheDirectory,   fileName );


    public async ValueTask<LocalFile?> SaveFileAsync( string filename, Stream stream, CancellationToken token ) => await LocalFile.SaveToFileAsync( GetCacheDataPath( filename ), stream, token )
                                                                                                                                  .ConfigureAwait( false );
    public async ValueTask<LocalFile?> SaveFileAsync( string filename, byte[] payload, CancellationToken token ) => await LocalFile.SaveToFileAsync( GetCacheDataPath( filename ), payload, token )
                                                                                                                                   .ConfigureAwait( false );


    public void CreateZipCache()
    {
        if ( File.Exists( ZipFileName ) ) { File.Delete( ZipFileName ); }

        ZipCache();
    }

    public void ZipCache( CompressionLevel level                        = CompressionLevel.Optimal ) => ZipCache( _CacheDirectory, level );
    public void ZipCache( LocalDirectory   path, CompressionLevel level = CompressionLevel.Optimal ) => ZipFile.CreateFromDirectory( path.FullPath, ZipFileName, level, true, Encoding.UTF8 );
    public void ZipCache( string           path, CompressionLevel level = CompressionLevel.Optimal ) => ZipFile.CreateFromDirectory( path,          ZipFileName, level, true, Encoding.UTF8 );
}
