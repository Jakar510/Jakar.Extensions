// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:54 PM


namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf> : IConnectableDb
    where TSelf : class, ITableRecord<TSelf>
{
    protected readonly FusionCache        _cache;
    protected readonly IConnectableDbRoot _database;


    public static TSelf[]                  Empty                     => [];
    public static ImmutableArray<TSelf>    EmptyArray                => [];
    public static FrozenSet<TSelf>         Set                       => FrozenSet<TSelf>.Empty;
    public        FusionCacheEntryOptions? Options                   { get; set; }
    public        RecordGenerator<TSelf>   Records                   => new(this);
    public        IsolationLevel           TransactionIsolationLevel => _database.TransactionIsolationLevel;


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
    public virtual async IAsyncEnumerable<TSelf> All( NpgsqlConnection connection, NpgsqlTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand<TSelf>            command = SqlCommand<TSelf>.GetAll();
        await using NpgsqlCommand    cmd     = command.ToCommand(connection, transaction);
        await using NpgsqlDataReader reader  = await cmd.ExecuteReaderAsync(token);
        await foreach ( TSelf record in reader.CreateAsync<TSelf>(token) ) { yield return record; }
    }


    public ValueTask<TResult> Call<TResult>( string sql, PostgresParameters parameters, Func<SqlMapper.GridReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default ) => this.TryCall(Call, sql, parameters, func, token);
    public virtual async ValueTask<TResult> Call<TResult>( NpgsqlConnection connection, NpgsqlTransaction transaction, string sql, PostgresParameters parameters, Func<SqlMapper.GridReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default )
    {
        try
        {
            await using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(sql, parameters, transaction);
            return await func(reader, token);
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(sql, parameters, e); }
    }


    public ValueTask<TResult> Call<TResult>( SqlCommand<TSelf> sql, Func<DbDataReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default ) => this.TryCall(Call, sql, func, token);
    public virtual async ValueTask<TResult> Call<TResult>( NpgsqlConnection connection, NpgsqlTransaction transaction, SqlCommand<TSelf> command, Func<DbDataReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default )
    {
        try
        {
            await using NpgsqlCommand    cmd    = command.ToCommand(connection, transaction);
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(token);
            return await func(reader, token);
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(command.SQL, command.Parameters, e); }
    }


    public async ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, CancellationToken token = default )
    {
        await using NpgsqlConnection connection = await ConnectAsync(token);
        using DataTable              schema     = await Schema(connection, token);
        await func(schema, token);
    }
    public async ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, PostgresCollectionType collectionName, CancellationToken token = default )
    {
        await using NpgsqlConnection connection = await ConnectAsync(token);
        using DataTable              schema     = await Schema(connection, collectionName, token);
        await func(schema, token);
    }
    public async ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, PostgresCollectionType collectionName, string?[] restrictionValues, CancellationToken token = default )
    {
        await using NpgsqlConnection connection = await ConnectAsync(token);
        using DataTable              schema     = await Schema(connection, collectionName, restrictionValues, token);
        await func(schema, token);
    }


    public async ValueTask<DataTable> Schema( NpgsqlConnection connection, CancellationToken      token                                                                        = default ) => await connection.GetSchemaAsync(token);
    public async ValueTask<DataTable> Schema( NpgsqlConnection connection, PostgresCollectionType collectionName, CancellationToken token                                      = default ) => await connection.GetSchemaAsync(GetCollectionTypeName(collectionName), token);
    public async ValueTask<DataTable> Schema( NpgsqlConnection connection, PostgresCollectionType collectionName, string?[]         restrictionValues, CancellationToken token = default ) => await connection.GetSchemaAsync(GetCollectionTypeName(collectionName), restrictionValues, token);


    public async ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default )
    {
        await using NpgsqlConnection connection = await ConnectAsync(token);
        using DataTable              schema     = await Schema(connection, token);
        return await func(schema, token);
    }
    public async ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, PostgresCollectionType collectionName, CancellationToken token = default )
    {
        await using NpgsqlConnection connection = await ConnectAsync(token);
        using DataTable              schema     = await Schema(connection, collectionName, token);
        return await func(schema, token);
    }
    public async ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, PostgresCollectionType collectionName, string?[] restrictionValues, CancellationToken token = default )
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

    protected static string GetCollectionTypeName( PostgresCollectionType type )
    {
        return type switch
               {
                   PostgresCollectionType.METADATA_COLLECTIONS    => @"METADATACOLLECTIONS",
                   PostgresCollectionType.RESTRICTIONS            => @"RESTRICTIONS",
                   PostgresCollectionType.DATA_SOURCE_INFORMATION => @"DATASOURCEINFORMATION",
                   PostgresCollectionType.DATA_TYPES              => @"DATATYPES",
                   PostgresCollectionType.RESERVED_WORDS          => @"RESERVEDWORDS",
                   PostgresCollectionType.DATABASES               => @"DATABASES",
                   PostgresCollectionType.SCHEMATA                => @"SCHEMATA",
                   PostgresCollectionType.TABLES                  => @"TABLES",
                   PostgresCollectionType.COLUMNS                 => @"COLUMNS",
                   PostgresCollectionType.VIEWS                   => @"VIEWS",
                   PostgresCollectionType.MATERIALIZED_VIEWS      => @"MATERIALIZEDVIEWS",
                   PostgresCollectionType.USERS                   => @"USERS",
                   PostgresCollectionType.INDEXES                 => @"INDEXES",
                   PostgresCollectionType.INDEX_COLUMNS           => @"INDEXCOLUMNS",
                   PostgresCollectionType.CONSTRAINTS             => @"CONSTRAINTS",
                   PostgresCollectionType.PRIMARY_KEY             => @"PRIMARYKEY",
                   PostgresCollectionType.UNIQUE_KEYS             => @"UNIQUEKEYS",
                   PostgresCollectionType.FOREIGN_KEYS            => @"FOREIGNKEYS",
                   PostgresCollectionType.CONSTRAINT_COLUMNS      => @"CONSTRAINTCOLUMNS",
                   _                                              => throw new NotImplementedException()
               };
    }
}



public enum PostgresCollectionType
{
    METADATA_COLLECTIONS,
    RESTRICTIONS,
    DATA_SOURCE_INFORMATION,
    DATA_TYPES,
    RESERVED_WORDS,

// custom collections for npgsql
    DATABASES,
    SCHEMATA,
    TABLES,
    COLUMNS,
    VIEWS,
    MATERIALIZED_VIEWS,
    USERS,
    INDEXES,
    INDEX_COLUMNS,
    CONSTRAINTS,
    PRIMARY_KEY,
    UNIQUE_KEYS,
    FOREIGN_KEYS,
    CONSTRAINT_COLUMNS
}
