// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:09 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf>
{
    public IAsyncEnumerable<TSelf> Insert( ReadOnlyMemory<TSelf>   records, CancellationToken token = default ) => this.TryCall(Insert, records, token);
    public IAsyncEnumerable<TSelf> Insert( ImmutableArray<TSelf>   records, CancellationToken token = default ) => this.TryCall(Insert, records, token);
    public IAsyncEnumerable<TSelf> Insert( IEnumerable<TSelf>      records, CancellationToken token = default ) => this.TryCall(Insert, records, token);
    public IAsyncEnumerable<TSelf> Insert( IAsyncEnumerable<TSelf> records, CancellationToken token = default ) => this.TryCall(Insert, records, token);
    public ValueTask<TSelf>        Insert( TSelf                   record,  CancellationToken token = default ) => this.TryCall(Insert, record,  token);


    public virtual async IAsyncEnumerable<TSelf> Insert( NpgsqlConnection connection, NpgsqlTransaction transaction, IEnumerable<TSelf> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( TSelf record in records ) { yield return await Insert(connection, transaction, record, token); }
    }
    public virtual async IAsyncEnumerable<TSelf> Insert( NpgsqlConnection connection, NpgsqlTransaction transaction, ReadOnlyMemory<TSelf> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        for ( int i = 0; i < records.Length; i++ ) { yield return await Insert(connection, transaction, records.Span[i], token); }
    }
    public virtual async IAsyncEnumerable<TSelf> Insert( NpgsqlConnection connection, NpgsqlTransaction transaction, ImmutableArray<TSelf> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( TSelf record in records ) { yield return await Insert(connection, transaction, record, token); }
    }
    public virtual async IAsyncEnumerable<TSelf> Insert( NpgsqlConnection connection, NpgsqlTransaction transaction, IAsyncEnumerable<TSelf> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSelf record in records.WithCancellation(token) ) { yield return await Insert(connection, transaction, record, token); }
    }


    public virtual async ValueTask<TSelf> Insert( NpgsqlConnection connection, NpgsqlTransaction transaction, TSelf record, CancellationToken token = default )
    {
        SqlCommand<TSelf> command = SqlCommand<TSelf>.GetInsert(record);

        try
        {
            await using NpgsqlCommand    cmd    = command.ToCommand(connection, transaction);
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(token);
            RecordID<TSelf>              id;

            if ( await reader.ReadAsync(token) ) { id = RecordID<TSelf>.Create(reader.GetGuid(0)); }
            else { throw new InvalidOperationException($"Insert command did not return the new ID for type {typeof(TSelf).FullName}"); }

            return record.NewID(id);
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(command, e); }
    }

    public virtual async ValueTask<ErrorOrResult<TSelf>> TryInsert( NpgsqlConnection connection, NpgsqlTransaction transaction, TSelf record, bool matchAll, PostgresParameters parameters, CancellationToken token = default )
    {
        SqlCommand<TSelf> command = SqlCommand<TSelf>.GetTryInsert(record, matchAll, parameters);

        try
        {
            await using NpgsqlCommand    cmd    = command.ToCommand(connection, transaction);
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(token);
            RecordID<TSelf>              id;

            if ( await reader.ReadAsync(token) ) { id = RecordID<TSelf>.Create(reader.GetGuid(0)); }
            else { throw new InvalidOperationException($"Insert command did not return the new ID for type {typeof(TSelf).FullName}"); }

            return record.NewID(id);
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(command, e); }
    }


    public virtual async ValueTask<ErrorOrResult<TSelf>> InsertOrUpdate( NpgsqlConnection connection, NpgsqlTransaction transaction, TSelf record, bool matchAll, PostgresParameters parameters, CancellationToken token = default )
    {
        SqlCommand<TSelf> command = SqlCommand<TSelf>.InsertOrUpdate(record, matchAll, parameters);

        try
        {
            await using NpgsqlCommand    cmd    = command.ToCommand(connection, transaction);
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(token);
            RecordID<TSelf>              id;

            if ( await reader.ReadAsync(token) ) { id = RecordID<TSelf>.Create(reader.GetGuid(0)); }
            else { throw new InvalidOperationException($"Insert command did not return the new ID for type {typeof(TSelf).FullName}"); }

            return record.NewID(id);
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(command, e); }
    }
}
