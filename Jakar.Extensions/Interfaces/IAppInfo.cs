namespace Jakar.Extensions.Interfaces;


public interface IAppInfo
{
    public string VersionNumber { get; }
    public string BuildNumber   { get; }
    public string PackageName   { get; }
    public string FullVersion   => $"{VersionNumber}.{BuildNumber}";
}
