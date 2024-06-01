// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:09 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public IAsyncEnumerable<TRecord> Insert( Activity? activity, ImmutableArray<TRecord>   records, CancellationToken token = default ) => this.TryCall( Insert, activity, records, token );
    public IAsyncEnumerable<TRecord> Insert( Activity? activity, IEnumerable<TRecord>      records, CancellationToken token = default ) => this.TryCall( Insert, activity, records, token );
    public IAsyncEnumerable<TRecord> Insert( Activity? activity, IAsyncEnumerable<TRecord> records, CancellationToken token = default ) => this.TryCall( Insert, activity, records, token );
    public ValueTask<TRecord>        Insert( Activity? activity, TRecord                   record,  CancellationToken token = default ) => this.TryCall( Insert, activity, record,  token );


    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, Activity? activity, IEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { yield return await Insert( connection, transaction, activity, record, token ); }
    }
    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, Activity? activity, ReadOnlyMemory<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        for ( int i = 0; i < records.Length; i++ ) { yield return await Insert( connection, transaction, activity, records.Span[i], token ); }
    }
    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, Activity? activity, ImmutableArray<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { yield return await Insert( connection, transaction, activity, record, token ); }
    }
    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, Activity? activity, IAsyncEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in records.WithCancellation( token ) ) { yield return await Insert( connection, transaction, activity, record, token ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord> Insert( DbConnection connection, DbTransaction transaction, Activity? activity, TRecord record, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Insert( record );

        try
        {
            CommandDefinition command = _database.GetCommand( activity, sql, transaction, token );
            Guid              id      = await connection.ExecuteScalarAsync<Guid>( command );
            return record.NewID( RecordID<TRecord>.Create( id ) );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }

    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> TryInsert( DbConnection connection, DbTransaction transaction, Activity? activity, TRecord record, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.TryInsert( record, matchAll, parameters );

        try
        {
            CommandDefinition command = _database.GetCommand( activity, sql, transaction, token );
            Guid?             id      = await connection.ExecuteScalarAsync<Guid?>( command );

            return id.HasValue
                       ? record.NewID( RecordID<TRecord>.Create( id.Value ) )
                       : default;
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> InsertOrUpdate( DbConnection connection, DbTransaction transaction, Activity? activity, TRecord record, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.InsertOrUpdate( record, matchAll, parameters );

        try
        {
            CommandDefinition command = _database.GetCommand( activity, sql, transaction, token );
            Guid?             id      = await connection.ExecuteScalarAsync<Guid?>( command );

            return id.HasValue
                       ? record.NewID( RecordID<TRecord>.Create( id.Value ) )
                       : default;
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
