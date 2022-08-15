namespace Jakar.Database;


public record DbTable<TRecord> : BaseRecord, IDbTable<TRecord> where TRecord : BaseTableRecord<TRecord>
{
    private readonly Database _database;


    public DbTable( Database database ) => _database = database;
}
