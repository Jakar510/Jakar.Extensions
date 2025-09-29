using Microsoft.Extensions.Options;
using Npgsql;



namespace Jakar.Extensions.Telemetry.Server.Data;


public class Db : Database.Database
{
    public DbTable<ApplicationRecord> Applications { get; }

    public Db( IConfiguration configuration, ISqlCacheFactory sqlCacheFactory, ITableCache tableCache, IOptions<DbOptions> options ) : base( configuration, sqlCacheFactory, tableCache, options ) { Applications = Create<ApplicationRecord>(); }


    protected override DbConnection CreateConnection( in SecuredString secure ) => new NpgsqlConnection( secure.ToString() );
}
