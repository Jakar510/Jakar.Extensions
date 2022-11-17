// Jakar.Extensions :: Jakar.Database
// 10/16/2022  5:46 PM

namespace Jakar.Database;


public sealed class DbOptions : IOptions<DbOptions>
{
    public DbInstance DbType { get; set; } = DbInstance.MsSql;


    DbOptions IOptions<DbOptions>.Value => this;


    public DbOptions() { }
}
