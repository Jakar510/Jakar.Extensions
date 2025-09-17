// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:09 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TClass>
{
    public IAsyncEnumerable<TClass> Insert( ImmutableArray<TClass>   records, CancellationToken token = default ) => this.TryCall(Insert, records, token);
    public IAsyncEnumerable<TClass> Insert( IEnumerable<TClass>      records, CancellationToken token = default ) => this.TryCall(Insert, records, token);
    public IAsyncEnumerable<TClass> Insert( IAsyncEnumerable<TClass> records, CancellationToken token = default ) => this.TryCall(Insert, records, token);
    public ValueTask<TClass>        Insert( TClass                   record,  CancellationToken token = default ) => this.TryCall(Insert, record,  token);


    public virtual async IAsyncEnumerable<TClass> Insert( NpgsqlConnection connection, DbTransaction transaction, IEnumerable<TClass> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( TClass record in records ) { yield return await Insert(connection, transaction, record, token); }
    }
    public virtual async IAsyncEnumerable<TClass> Insert( NpgsqlConnection connection, DbTransaction transaction, ReadOnlyMemory<TClass> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        for ( int i = 0; i < records.Length; i++ ) { yield return await Insert(connection, transaction, records.Span[i], token); }
    }
    public virtual async IAsyncEnumerable<TClass> Insert( NpgsqlConnection connection, DbTransaction transaction, ImmutableArray<TClass> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( TClass record in records ) { yield return await Insert(connection, transaction, record, token); }
    }
    public virtual async IAsyncEnumerable<TClass> Insert( NpgsqlConnection connection, DbTransaction transaction, IAsyncEnumerable<TClass> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TClass record in records.WithCancellation(token) ) { yield return await Insert(connection, transaction, record, token); }
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<TClass> Insert( NpgsqlConnection connection, DbTransaction transaction, TClass record, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetInsert(record);

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            RecordID<TClass>  id      = RecordID<TClass>.Create(await connection.ExecuteScalarAsync<Guid>(command));
            return record.NewID(id);
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<ErrorOrResult<TClass>> TryInsert( NpgsqlConnection connection, DbTransaction transaction, TClass record, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetTryInsert(record, matchAll, parameters);

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            RecordID<TClass>? id      = RecordID<TClass>.TryCreate(await connection.ExecuteScalarAsync<Guid?>(command));
            if ( id is null ) { return Error.NotFound(); }

            return record.NewID(id.Value);
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<ErrorOrResult<TClass>> InsertOrUpdate( NpgsqlConnection connection, DbTransaction transaction, TClass record, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.InsertOrUpdate(record, matchAll, parameters);

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            RecordID<TClass>? id      = RecordID<TClass>.TryCreate(await connection.ExecuteScalarAsync<Guid?>(command));
            if ( id is null ) { return Error.NotFound(); }

            return record.NewID(id.Value);
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }
}
