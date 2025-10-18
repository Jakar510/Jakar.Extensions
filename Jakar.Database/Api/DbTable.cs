// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:54 PM


namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf> : IConnectableDb
    where TSelf : class, ITableRecord<TSelf>
{
    protected readonly     FusionCache        _cache;
    protected readonly     IConnectableDbRoot _database;


    public static TSelf[]                  Empty                     { get => []; }
    public static ImmutableArray<TSelf>    EmptyArray                { get => []; }
    public static FrozenSet<TSelf>         Set                       { get => FrozenSet<TSelf>.Empty; }
    public        FusionCacheEntryOptions? Options                   { get; set; }
    public        RecordGenerator<TSelf>   Records                   { get => new(this); }
    public        IsolationLevel           TransactionIsolationLevel { get => _database.TransactionIsolationLevel; }


    public DbTable( IConnectableDbRoot database, FusionCache cache )
    {
        _database = database;
        _cache    = cache;
        if ( TSelf.TableName != typeof(TSelf).GetTableName() ) { throw new InvalidOperationException($"{TSelf.TableName} != {typeof(TSelf).GetTableName()}"); }
    }
    public virtual ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return default;
    }


    public ValueTask<NpgsqlConnection> ConnectAsync( CancellationToken token = default ) => _database.ConnectAsync(token);


    public IAsyncEnumerable<TSelf> All( CancellationToken token = default ) => this.Call(All, token);
    public virtual async IAsyncEnumerable<TSelf> All( NpgsqlConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand               command = SQLCache.GetAll();
        await using DbDataReader reader  = await _database.ExecuteReaderAsync(connection, transaction, command, token);
        await foreach ( TSelf record in reader.CreateAsync<TSelf>(token) ) { yield return record; }
    }


    public ValueTask<TResult> Call<TResult>( string sql, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default ) => this.TryCall(Call, sql, parameters, func, token);
    public virtual async ValueTask<TResult> Call<TResult>( NpgsqlConnection connection, DbTransaction transaction, string sql, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default )
    {
        try
        {
            await using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(sql, parameters, transaction);
            return await func(reader, token);
        }
        catch ( Exception e ) { throw new SqlException(sql, parameters, e); }
    }


    public ValueTask<TResult> Call<TResult>( SqlCommand sql, Func<DbDataReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default ) => this.TryCall(Call, sql, func, token);
    public virtual async ValueTask<TResult> Call<TResult>( NpgsqlConnection connection, DbTransaction transaction, SqlCommand sql, Func<DbDataReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default )
    {
        try
        {
            await using DbDataReader reader = await _database.ExecuteReaderAsync(connection, transaction, sql, token);
            return await func(reader, token);
        }
        catch ( Exception e ) { throw new SqlException(sql.sql, sql.parameters, e); }
    }


    public async ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, CancellationToken token = default )
    {
        await using NpgsqlConnection connection = await ConnectAsync(token);
        using DataTable              schema     = await Schema(connection, token);
        await func(schema, token);
    }
    public async ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, string collectionName, CancellationToken token = default )
    {
        await using NpgsqlConnection connection = await ConnectAsync(token);
        using DataTable              schema     = await Schema(connection, collectionName, token);
        await func(schema, token);
    }
    public async ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, string collectionName, string?[] restrictionValues, CancellationToken token = default )
    {
        await using NpgsqlConnection connection = await ConnectAsync(token);
        using DataTable              schema     = await Schema(connection, collectionName, restrictionValues, token);
        await func(schema, token);
    }


    public async ValueTask<DataTable> Schema( NpgsqlConnection connection, CancellationToken token                                                                        = default ) => await connection.GetSchemaAsync(token);
    public async ValueTask<DataTable> Schema( NpgsqlConnection connection, string            collectionName, CancellationToken token                                      = default ) => await connection.GetSchemaAsync(collectionName, token);
    public async ValueTask<DataTable> Schema( NpgsqlConnection connection, string            collectionName, string?[]         restrictionValues, CancellationToken token = default ) => await connection.GetSchemaAsync(collectionName, restrictionValues, token);


    public async ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default )
    {
        await using NpgsqlConnection connection = await ConnectAsync(token);
        using DataTable              schema     = await Schema(connection, token);
        return await func(schema, token);
    }
    public async ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, string collectionName, CancellationToken token = default )
    {
        await using NpgsqlConnection connection = await ConnectAsync(token);
        using DataTable              schema     = await Schema(connection, collectionName, token);
        return await func(schema, token);
    }
    public async ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, string collectionName, string?[] restrictionValues, CancellationToken token = default )
    {
        await using NpgsqlConnection connection = await ConnectAsync(token);
        using DataTable              schema     = await Schema(connection, collectionName, restrictionValues, token);
        return await func(schema, token);
    }


    public virtual async ValueTask<string> ServerVersion( CancellationToken token = default )
    {
        await using NpgsqlConnection connection = await ConnectAsync(token);
        return connection.ServerVersion;
    }
}
