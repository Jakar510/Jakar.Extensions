// Jakar.Extensions :: TestMauiApp
// 07/08/2024  11:07

namespace TestMauiApp;


public sealed class TestMauiApp : IAppID
{
    public static Guid       AppID      { get; } = Guid.NewGuid();
    public static string     AppName    => nameof(TestMauiApp);
    public static AppVersion AppVersion { get; } = new(1, 0, 0);
}
