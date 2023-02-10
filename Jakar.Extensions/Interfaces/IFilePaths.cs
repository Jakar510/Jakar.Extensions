#nullable enable
namespace Jakar.Extensions;


public interface IFilePaths
{
    public LocalDirectory AppDataDirectory { get; }
    public LocalDirectory CacheDirectory   { get; }
    public LocalFile      AccountsFile     { get; }
    public LocalFile      AppStateFile     { get; }
    public LocalFile      DebugFile        { get; }
    public LocalFile      FeedBackFile     { get; }
    public LocalFile      IncomingFile     { get; }
    public LocalFile      OutgoingFile     { get; }
    public LocalFile      ZipFile          { get; }
    public LocalFile      ScreenShot       { get; }
}
