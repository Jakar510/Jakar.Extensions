namespace Jakar.Database.Implementations;
#nullable enable



public sealed class PostgresDbTable<TRecord> : DbTableBase<TRecord> where TRecord : TableRecord<TRecord>
{
    public override string TableName { get; } = $"\"{typeof(TRecord).GetTableName()}\"";


    public PostgresDbTable( IConnectableDb database ) : base( database ) { }
}
