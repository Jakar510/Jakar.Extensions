// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:09 PM

namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ) ]
public partial class DbTable<TRecord>
{
    public IAsyncEnumerable<TRecord> Insert( ImmutableArray<TRecord>   records, CancellationToken token = default ) => this.TryCall( Insert, records, token );
    public IAsyncEnumerable<TRecord> Insert( IEnumerable<TRecord>      records, CancellationToken token = default ) => this.TryCall( Insert, records, token );
    public IAsyncEnumerable<TRecord> Insert( IAsyncEnumerable<TRecord> records, CancellationToken token = default ) => this.TryCall( Insert, records, token );
    public ValueTask<TRecord>        Insert( TRecord                   record,  CancellationToken token = default ) => this.TryCall( Insert, record,  token );


    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, IEnumerable<TRecord> records, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { yield return await Insert( connection, transaction, record, token ); }
    }
    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, ImmutableArray<TRecord> records, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { yield return await Insert( connection, transaction, record, token ); }
    }
    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TRecord> records, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        await foreach ( TRecord record in records.WithCancellation( token ) ) { yield return await Insert( connection, transaction, record, token ); }
    }


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<TRecord> Insert( DbConnection connection, DbTransaction transaction, TRecord record, CancellationToken token = default )
    {
        string sql = _cache[_database.Instance][SqlStatement.SingleInsert];

        var parameters = new DynamicParameters( record );

        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, new SqlCommand( sql, parameters ), token );
            var               id      = await connection.ExecuteScalarAsync<Guid>( command );
            return record.NewID( id );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }

    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<TRecord?> TryInsert( DbConnection connection, DbTransaction transaction, TRecord record, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        string where = string.Join( matchAll
                                        ? "AND"
                                        : "OR",
                                    parameters.ParameterNames.Select( KeyValuePair ) );

        var p = new DynamicParameters( parameters );
        p.Add( nameof(where), where );

        string sql = _cache[_database.Instance][SqlStatement.TryInsert];


        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, new SqlCommand( sql, parameters ), token );
            var               id      = await connection.ExecuteScalarAsync<Guid?>( command );

            return id.HasValue
                       ? record.NewID( id.Value )
                       : default;
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async ValueTask<TRecord?> InsertOrUpdate( DbConnection connection, DbTransaction transaction, TRecord record, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        string where = string.Join( matchAll
                                        ? "AND"
                                        : "OR",
                                    parameters.ParameterNames.Select( KeyValuePair ) );

        var p = new DynamicParameters( parameters );
        p.Add( nameof(where), where );
        p.Add( ID,            record.ID.Value );

        string sql = _cache[_database.Instance][SqlStatement.InsertOrUpdate];

        try
        {
            CommandDefinition command = _database.GetCommandDefinition( transaction, new SqlCommand( sql, parameters ), token );
            var               id      = await connection.ExecuteScalarAsync<Guid?>( command );

            return id.HasValue
                       ? record.NewID( id.Value )
                       : default;
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }
}
