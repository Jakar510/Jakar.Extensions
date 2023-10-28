namespace Jakar.Extensions;


public interface IAppInfo
{
    public string BuildNumber   { get; }
    public string FullVersion   => $"{VersionNumber}.{BuildNumber}";
    public string PackageName   { get; }
    public string VersionNumber { get; }
}
