using Jakar.Extensions.Serilog.Server.Data.Tables;
using Microsoft.Extensions.Options;
using Npgsql;
using ZiggyCreatures.Caching.Fusion;



namespace Jakar.Extensions.Serilog.Server.Data;


public class SerilogDb : Database.Database
{
    public DbTable<ApplicationRecord> Applications { get; }

    public SerilogDb( IConfiguration configuration, IOptions<DbOptions> options, FusionCache cache ) : base(configuration, options, cache) { Applications = Create<ApplicationRecord>(); }


    protected override NpgsqlConnection CreateConnection( in SecuredString secure ) => new(secure.ToString());
}
