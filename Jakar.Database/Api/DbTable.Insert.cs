// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:09 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf>
{
    public IAsyncEnumerable<TSelf> Insert( ImmutableArray<TSelf>   records, CancellationToken token = default ) => this.TryCall(Insert, records, token);
    public IAsyncEnumerable<TSelf> Insert( IEnumerable<TSelf>      records, CancellationToken token = default ) => this.TryCall(Insert, records, token);
    public IAsyncEnumerable<TSelf> Insert( IAsyncEnumerable<TSelf> records, CancellationToken token = default ) => this.TryCall(Insert, records, token);
    public ValueTask<TSelf>        Insert( TSelf                   record,  CancellationToken token = default ) => this.TryCall(Insert, record,  token);


    public virtual async IAsyncEnumerable<TSelf> Insert( NpgsqlConnection connection, DbTransaction transaction, IEnumerable<TSelf> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( TSelf record in records ) { yield return await Insert(connection, transaction, record, token); }
    }
    public virtual async IAsyncEnumerable<TSelf> Insert( NpgsqlConnection connection, DbTransaction transaction, ReadOnlyMemory<TSelf> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        for ( int i = 0; i < records.Length; i++ ) { yield return await Insert(connection, transaction, records.Span[i], token); }
    }
    public virtual async IAsyncEnumerable<TSelf> Insert( NpgsqlConnection connection, DbTransaction transaction, ImmutableArray<TSelf> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( TSelf record in records ) { yield return await Insert(connection, transaction, record, token); }
    }
    public virtual async IAsyncEnumerable<TSelf> Insert( NpgsqlConnection connection, DbTransaction transaction, IAsyncEnumerable<TSelf> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSelf record in records.WithCancellation(token) ) { yield return await Insert(connection, transaction, record, token); }
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<TSelf> Insert( NpgsqlConnection connection, DbTransaction transaction, TSelf record, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetInsert(record);

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            RecordID<TSelf>  id      = RecordID<TSelf>.Create(await connection.ExecuteScalarAsync<Guid>(command));
            return record.NewID(id);
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<ErrorOrResult<TSelf>> TryInsert( NpgsqlConnection connection, DbTransaction transaction, TSelf record, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetTryInsert(record, matchAll, parameters);

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            RecordID<TSelf>? id      = RecordID<TSelf>.TryCreate(await connection.ExecuteScalarAsync<Guid?>(command));
            if ( id is null ) { return Error.NotFound(); }

            return record.NewID(id.Value);
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<ErrorOrResult<TSelf>> InsertOrUpdate( NpgsqlConnection connection, DbTransaction transaction, TSelf record, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.InsertOrUpdate(record, matchAll, parameters);

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            RecordID<TSelf>? id      = RecordID<TSelf>.TryCreate(await connection.ExecuteScalarAsync<Guid?>(command));
            if ( id is null ) { return Error.NotFound(); }

            return record.NewID(id.Value);
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }
}
