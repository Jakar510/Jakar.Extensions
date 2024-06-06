// Jakar.Extensions :: Jakar.Database.Experiments
// 1/10/2024  16:6

namespace Jakar.Database.Experiments;


internal class DbExperiments : IAppName
{
    public static string     Name    => nameof(DbExperiments);
    public static AppVersion Version { get; } = new(1, 0, 0, 1);
}
