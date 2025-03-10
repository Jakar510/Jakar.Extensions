// Jakar.Extensions :: TestMauiApp
// 07/08/2024  11:07

namespace TestMauiApp;


public sealed class TestMauiApp : IApp
{
    private static ActivitySource? _source;


    public static ActivitySource ActivitySource { get => _source ??= new ActivitySource( AppName ); set => _source = value; }
    public static Guid           AppID          { get; } = Guid.NewGuid();
    public static string         AppName        => nameof(TestMauiApp);
    public static AppVersion     AppVersion     { get; }      = new(1, 0, 0);
    public static Guid           DebugID        { get; set; } = Guid.NewGuid();
    public static Guid           DeviceID       { get; set; } = Guid.NewGuid();
    public static string         DeviceName     { get; set; } = string.Empty;
}
