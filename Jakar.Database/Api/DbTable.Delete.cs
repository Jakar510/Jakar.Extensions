// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:06 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf>
{
    public ValueTask Delete( TSelf                             record,   CancellationToken token                               = default ) => this.TryCall(Delete, record,   token);
    public ValueTask Delete( IEnumerable<TSelf>                records,  CancellationToken token                               = default ) => this.TryCall(Delete, records,  token);
    public ValueTask Delete( IAsyncEnumerable<TSelf>           records,  CancellationToken token                               = default ) => this.TryCall(Delete, records,  token);
    public ValueTask Delete( RecordID<TSelf>                   id,       CancellationToken token                               = default ) => this.TryCall(Delete, id,       token);
    public ValueTask Delete( IEnumerable<RecordID<TSelf>>      ids,      CancellationToken token                               = default ) => this.TryCall(Delete, ids,      token);
    public ValueTask Delete( IAsyncEnumerable<RecordID<TSelf>> ids,      CancellationToken token                               = default ) => this.TryCall(Delete, ids,      token);
    public ValueTask Delete( bool                               matchAll, DynamicParameters parameters, CancellationToken token = default ) => this.TryCall(Delete, matchAll, parameters, token);


    public virtual ValueTask Delete( NpgsqlConnection connection, DbTransaction transaction, TSelf              record,  CancellationToken token = default ) => Delete(connection, transaction, record.ID,                        token);
    public virtual ValueTask Delete( NpgsqlConnection connection, DbTransaction transaction, IEnumerable<TSelf> records, CancellationToken token = default ) => Delete(connection, transaction, records.Select(static x => x.ID), token);
    public async ValueTask Delete( NpgsqlConnection connection, DbTransaction transaction, IAsyncEnumerable<TSelf> records, CancellationToken token = default )
    {
        HashSet<TSelf> ids = await records.ToHashSet(token);
        await Delete(connection, transaction, ids, token);
    }
    public virtual async ValueTask Delete( NpgsqlConnection connection, DbTransaction transaction, IAsyncEnumerable<RecordID<TSelf>> ids, CancellationToken token = default )
    {
        HashSet<RecordID<TSelf>> records = await ids.ToHashSet(token);
        await Delete(connection, transaction, records, token);
    }
    public virtual async ValueTask Delete( NpgsqlConnection connection, DbTransaction transaction, RecordID<TSelf> id, CancellationToken token = default )
    {
        SqlCommand        sql     = SQLCache.GetDeleteID(in id);
        CommandDefinition command = _database.GetCommand(in sql, transaction, token);
        await connection.ExecuteScalarAsync(command);
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask Delete( NpgsqlConnection connection, DbTransaction transaction, IEnumerable<RecordID<TSelf>> ids, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetDelete(ids);

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            await connection.ExecuteScalarAsync(command);
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask Delete( NpgsqlConnection connection, DbTransaction transaction, bool matchAll, DynamicParameters parameters, CancellationToken token )
    {
        SqlCommand        sql     = SQLCache.GetDelete(matchAll, parameters);
        CommandDefinition command = _database.GetCommand(in sql, transaction, token);
        await connection.ExecuteScalarAsync(command);
    }
}
