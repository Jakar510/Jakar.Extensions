#nullable enable
namespace Jakar.Extensions.Interfaces;


public interface IFilePaths
{
    public string AppStateFileName { get; }
    public string DebugFileName    { get; }
    public string FeedBackFileName { get; }
    public string OutgoingFileName { get; }
    public string IncomingFileName { get; }
    public string AccountsFileName { get; }
    public string ZipFileName      { get; }
    public string ScreenShot       { get; }
}
