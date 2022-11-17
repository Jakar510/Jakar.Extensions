#nullable enable
namespace Jakar.Extensions;


public interface IFilePaths
{
    public string AccountsFileName { get; }
    public string AppStateFileName { get; }
    public string DebugFileName    { get; }
    public string FeedBackFileName { get; }
    public string IncomingFileName { get; }
    public string OutgoingFileName { get; }
    public string ScreenShot       { get; }
    public string ZipFileName      { get; }
}
