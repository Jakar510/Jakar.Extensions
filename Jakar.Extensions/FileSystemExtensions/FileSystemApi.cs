using System.IO.Compression;


namespace Jakar.Extensions.FileSystemExtensions;


public abstract class BaseFileSystemApi : IFilePaths
{
    protected abstract string _AppDataDirectory { get; }
    protected abstract string _CacheDirectory   { get; }

    public string AppStateFileName => GetCacheDataPath("AppState.json");
    public string DebugFileName    => GetCacheDataPath("debug.txt");
    public string FeedBackFileName => GetCacheDataPath("feedback.json");
    public string OutgoingFileName => GetCacheDataPath("Outgoing.json");
    public string IncomingFileName => GetCacheDataPath("Incoming.json");

    public string AccountsFileName => GetAppDataPath($"accounts.json");
    public string ZipFileName      => GetAppDataPath("AppData.zip");
    public string ScreenShot       => GetCacheDataPath("ScreenShot.png");


    protected BaseFileSystemApi() { }


    public string GetAppDataPath( string   fileName ) => Path.Combine(_AppDataDirectory, fileName);
    public string GetCacheDataPath( string fileName ) => Path.Combine(_CacheDirectory, fileName);


    public void CreateZipCache()
    {
        if ( File.Exists(ZipFileName) ) { File.Delete(ZipFileName); }

        ZipCache();
    }

    public void ZipCache( CompressionLevel level                        = CompressionLevel.Optimal ) => ZipCache(_CacheDirectory, level);
    public void ZipCache( LocalDirectory   path, CompressionLevel level = CompressionLevel.Optimal ) => ZipFile.CreateFromDirectory(path.FullPath, ZipFileName, level, true, Encoding.UTF8);
    public void ZipCache( string           path, CompressionLevel level = CompressionLevel.Optimal ) => ZipFile.CreateFromDirectory(path, ZipFileName, level, true, Encoding.UTF8);


    public async Task<LocalFile?> SaveFileAsync( string filename, Uri    uri )                              => await LocalFile.SaveToFileAsync(GetCacheDataPath(filename), uri).ConfigureAwait(false);
    public async Task<LocalFile?> SaveFileAsync( string filename, Stream stream,  CancellationToken token ) => await LocalFile.SaveToFileAsync(GetCacheDataPath(filename), stream, token).ConfigureAwait(false);
    public async Task<LocalFile?> SaveFileAsync( string filename, byte[] payload, CancellationToken token ) => await LocalFile.SaveToFileAsync(GetCacheDataPath(filename), payload, token).ConfigureAwait(false);
    public       LocalFile        SaveFile( string      filename, string link ) => LocalFile.SaveToFile(filename, new Uri(link));
    public       LocalFile        SaveFile( string      filename, Uri    link ) => LocalFile.SaveToFile(GetCacheDataPath(filename), link);
}
