// Jakar.Extensions :: Jakar.Extensions.Telemetry.Server
// 06/25/2024  11:06

namespace Jakar.Extensions.Telemetry.Server.Data;


public class ConfigureApiDatabase : ConfigureDbServices<ConfigureApiDatabase, TelemetryServer, Db, SqlCacheFactory, TableCacheFactory>
{
    public override bool UseApplicationCookie => true;
    public override bool UseAuth              => true;
    public override bool UseAuthCookie        => true;
    public override bool UseExternalCookie    => false;
    public override bool UseGoogleAccount     => false;
    public override bool UseMicrosoftAccount  => false;
    public override bool UseOpenIdConnect     => false;
    public override bool UseRedis             => true;


    public ConfigureApiDatabase() { }
}
