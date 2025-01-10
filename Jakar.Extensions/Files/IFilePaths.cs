namespace Jakar.Extensions;


public interface IFilePaths
{
    public LocalFile      AccountsFile     { get; }
    public LocalDirectory AppDataDirectory { get; }
    public LocalFile      AppStateFile     { get; }
    public LocalDirectory CacheDirectory   { get; }
    public LocalFile      DebugFile        { get; }
    public LocalFile      FeedbackFile     { get; }
    public LocalFile      IncomingFile     { get; }
    public LocalFile      OutgoingFile     { get; }
    public LocalFile      ScreenShot       { get; }
    public LocalFile      ZipFile          { get; }
}
