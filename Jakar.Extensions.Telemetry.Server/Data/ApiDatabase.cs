using System.Data.Common;
using Jakar.Database;
using Jakar.Database.Caches;
using Microsoft.Extensions.Options;
using Npgsql;



namespace Jakar.Extensions.Telemetry.Server.Data;


public class ApiDatabase : Database.Database
{
    public ApiDatabase( IConfiguration configuration, ISqlCacheFactory sqlCacheFactory, ITableCache tableCache, IOptions<DbOptions> options ) : base( configuration, sqlCacheFactory, tableCache, options ) { }


    protected override DbConnection CreateConnection( in SecuredString secure ) => new NpgsqlConnection( secure.ToString() );
}
