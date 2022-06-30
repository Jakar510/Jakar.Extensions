// Jakar.Extensions :: Jakar.Extensions.AppCenter
// 06/15/2022  9:48 AM

namespace Jakar.Extensions.AppCenter;


public sealed class AppCenterConfig : IniConfig
{
    private      LocalFile? _file;
    public const string     CONFIG_FILE_NAME = "AppCenter.conf";

    public LocalFile Path
    {
        get => _file ??= LocalDirectory.CurrentDirectory.Join(CONFIG_FILE_NAME);
        private set => _file = value;
    }


    public AppCenterConfig() : base() { }


    public static AppCenterConfig Create( LocalDirectory directory ) => Create(directory.Join(CONFIG_FILE_NAME));
    public static AppCenterConfig Create( LocalFile file )
    {
        AppCenterConfig config = ReadFromFile<AppCenterConfig>(file) ?? new AppCenterConfig();
        config.Path = file;
        return config;
    }


    public void Refresh()
    {
        AppCenterConfig config = Create(Path);
        foreach ( ( string? key, Section? value ) in config ) { this[key] = value; }
    }
    public Task RefreshAsync() => Task.Run(Refresh);


    public async Task SaveAsync() => await WriteToFile(Path);
}
