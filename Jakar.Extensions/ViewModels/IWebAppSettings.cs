// Jakar.Extensions :: Jakar.Extensions
// 4/5/2024  21:36

using Microsoft.Extensions.Configuration;



namespace Jakar.Extensions;


public interface IWebAppSettings : IAppSettings, IHostInfo
{
    string[]       Args             { get; }
    IConfiguration Configuration    { get; }
    Uri?           Domain           { get; }
    bool           IsCloudVerified  { get; set; }
    bool           IsDevEnvironment { get; }
    IPAddress[]    SafeIPs          { get; }
    AppVersion     Version          { get; }
}
