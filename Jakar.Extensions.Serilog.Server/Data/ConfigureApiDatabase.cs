// Jakar.Extensions :: Jakar.Extensions.Telemetry.Server
// 06/25/2024  11:06

using ZiggyCreatures.Caching.Fusion.Backplane.Memory;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;



namespace Jakar.Extensions.Serilog.Server.Data;


public class ConfigureApiDatabase : ConfigureDbServices<ConfigureApiDatabase, TelemetryServer, SerilogDb>
{
    public override    AppLoggerOptions LoggerOptions                               { get; }
    public override    SeqConfig        SeqConfig                                   { get; }
    public override    TelemetrySource  TelemetrySource                             { get; }
    public override    bool             UseApplicationCookie                        => true;
    public override    bool             UseAuth                                     => true;
    public override    bool             UseAuthCookie                               => true;
    public override    bool             UseExternalCookie                           => false;
    public override    bool             UseGoogleAccount                            => false;
    public override    bool             UseMicrosoftAccount                         => false;
    public override    bool             UseOpenIdConnect                            => false;
    public override    bool             UseRedis                                    => true;
    protected override void             Configure( RedisBackplaneOptions  options ) { }
    protected override void             Configure( MemoryBackplaneOptions options ) { }


    public ConfigureApiDatabase() { }
}
