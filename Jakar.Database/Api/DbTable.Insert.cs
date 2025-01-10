// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:09 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public IAsyncEnumerable<TRecord> Insert( ImmutableArray<TRecord>   records, CancellationToken token = default ) => this.TryCall( Insert, records, token );
    public IAsyncEnumerable<TRecord> Insert( IEnumerable<TRecord>      records, CancellationToken token = default ) => this.TryCall( Insert, records, token );
    public IAsyncEnumerable<TRecord> Insert( IAsyncEnumerable<TRecord> records, CancellationToken token = default ) => this.TryCall( Insert, records, token );
    public ValueTask<TRecord>        Insert( TRecord                   record,  CancellationToken token = default ) => this.TryCall( Insert, record,  token );


    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, IEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { yield return await Insert( connection, transaction, record, token ); }
    }
    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, ReadOnlyMemory<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        for ( int i = 0; i < records.Length; i++ ) { yield return await Insert( connection, transaction, records.Span[i], token ); }
    }
    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, ImmutableArray<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { yield return await Insert( connection, transaction, record, token ); }
    }
    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in records.WithCancellation( token ) ) { yield return await Insert( connection, transaction, record, token ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord> Insert( DbConnection connection, DbTransaction transaction, TRecord record, CancellationToken token = default )
    {
        SqlCommand sql = TRecord.SQL.Insert( record );

        try
        {
            CommandDefinition command = _database.GetCommand( sql, transaction, token );
            Guid              id      = await connection.ExecuteScalarAsync<Guid>( command );
            return record.NewID( RecordID<TRecord>.Create( id ) );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }

    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<ErrorOrResult<TRecord>> TryInsert( DbConnection connection, DbTransaction transaction, TRecord record, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        SqlCommand sql = TRecord.SQL.TryInsert( record, matchAll, parameters );

        try
        {
            CommandDefinition command = _database.GetCommand( sql, transaction, token );
            Guid?             id      = await connection.ExecuteScalarAsync<Guid?>( command );

            return id.HasValue
                       ? record.NewID( RecordID<TRecord>.Create( id.Value ) )
                       : Error.NotFound();
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<ErrorOrResult<TRecord>> InsertOrUpdate( DbConnection connection, DbTransaction transaction, TRecord record, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        SqlCommand sql = TRecord.SQL.InsertOrUpdate( record, matchAll, parameters );

        try
        {
            CommandDefinition command = _database.GetCommand( sql, transaction, token );
            Guid?             id      = await connection.ExecuteScalarAsync<Guid?>( command );

            return id.HasValue
                       ? record.NewID( RecordID<TRecord>.Create( id.Value ) )
                       : Error.NotFound();
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
