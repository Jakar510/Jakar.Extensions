﻿// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:54 PM


namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ) ]
public partial class DbTable<TRecord>( IConnectableDbRoot database ) : SqlCache<TRecord>( database ), IConnectableDb where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    protected readonly ConcurrentDictionary<int, string>    _deleteGuids     = new();
    protected readonly ConcurrentDictionary<int, string>    _whereParameters = new();
    protected readonly ConcurrentDictionary<string, string> _where           = new();


    public int? CommandTimeout
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _database.CommandTimeout;
    }
    public string CurrentSchema
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _database.CurrentSchema;
    }
    public DbInstance Instance
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _database.Instance;
    }
    public RecordGenerator<TRecord> Records => new(this);


    public virtual ValueTask DisposeAsync()
    {
        GC.SuppressFinalize( this );
        return default;
    }
    public ValueTask<DbConnection> ConnectAsync( CancellationToken token = default ) => _database.ConnectAsync( token );


    void IDbTable.ResetSqlCaches() => Reset();


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public Descriptor GetDescriptor( string columnName ) => _propertiesCache.Get( this, columnName );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public string KeyValuePair( string columnName ) => GetDescriptor( columnName )
       .KeyValuePair;

    public IAsyncEnumerable<TRecord> All( CancellationToken token = default ) => this.Call( All, token );

    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async IAsyncEnumerable<TRecord> All( DbConnection connection, DbTransaction? transaction, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        string                   sql    = _cache[_database.Instance][SqlStatement.All];
        await using DbDataReader reader = await _database.ExecuteReaderAsync( connection, transaction, sql, token );
        await foreach ( TRecord record in TRecord.CreateAsync( reader, token ) ) { yield return record; }
    }


    public ValueTask<TResult> Call<TResult>( string sql, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default ) =>
        this.TryCall( Call, sql, parameters, func, token );
    public virtual async ValueTask<TResult> Call<TResult>( DbConnection                                                      connection,
                                                           DbTransaction                                                     transaction,
                                                           string                                                            sql,
                                                           DynamicParameters?                                                parameters,
                                                           Func<SqlMapper.GridReader, CancellationToken, ValueTask<TResult>> func,
                                                           CancellationToken                                                 token = default
    )
    {
        try
        {
            await using SqlMapper.GridReader reader = await connection.QueryMultipleAsync( sql, parameters, transaction );
            return await func( reader, token );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    public ValueTask<TResult> Call<TResult>( SqlCommand sql, Func<DbDataReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default ) => this.TryCall( Call, sql, func, token );
    public virtual async ValueTask<TResult> Call<TResult>( DbConnection connection, DbTransaction transaction, SqlCommand sql, Func<DbDataReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default )
    {
        try
        {
            await using DbDataReader reader = await _database.ExecuteReaderAsync( connection, transaction, sql, token );
            return await func( reader, token );
        }
        catch ( Exception e ) { throw new SqlException( sql.SQL, sql.Parameters, e ); }
    }


    public async ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync( token );
        using DataTable          schema     = await Schema( connection, token );
        await func( schema, token );
    }
    public async ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, string collectionName, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync( token );
        using DataTable          schema     = await Schema( connection, collectionName, token );
        await func( schema, token );
    }
    public async ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, string collectionName, string?[] restrictionValues, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync( token );
        using DataTable          schema     = await Schema( connection, collectionName, restrictionValues, token );
        await func( schema, token );
    }


    public async ValueTask<DataTable> Schema( DbConnection connection, CancellationToken token = default ) => await connection.GetSchemaAsync( token );
    public async ValueTask<DataTable> Schema( DbConnection connection, string            collectionName, CancellationToken token = default ) => await connection.GetSchemaAsync( collectionName, token );
    public async ValueTask<DataTable> Schema( DbConnection connection, string            collectionName, string?[] restrictionValues, CancellationToken token = default ) => await connection.GetSchemaAsync( collectionName, restrictionValues, token );


    public async ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync( token );
        using DataTable          schema     = await Schema( connection, token );
        return await func( schema, token );
    }
    public async ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, string collectionName, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync( token );
        using DataTable          schema     = await Schema( connection, collectionName, token );
        return await func( schema, token );
    }
    public async ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, string collectionName, string?[] restrictionValues, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync( token );
        using DataTable          schema     = await Schema( connection, collectionName, restrictionValues, token );
        return await func( schema, token );
    }


    public virtual async ValueTask<string> ServerVersion( CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync( token );
        return connection.ServerVersion;
    }
}
