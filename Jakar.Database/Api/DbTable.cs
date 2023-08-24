// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:54 PM


namespace Jakar.Database;
// TrueLogic :: TrueLogic.Common
// 04/11/2023  11:33 AM



[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord> : ObservableClass, IConnectableDb, IAsyncDisposable where TRecord : TableRecord<TRecord>
{
    protected readonly IConnectableDb                 _database;
    protected readonly TypePropertiesCache.Properties _propertiesCache;


    [SuppressMessage( "ReSharper", "ReturnTypeCanBeEnumerable.Global" )] public static TRecord[]           Empty       => Array.Empty<TRecord>();
    public                                                                             IEnumerable<string> ColumnNames => Descriptors.Select( x => x.ColumnName );

    public virtual string CreatedBy => Instance switch
                                       {
                                           DbInstance.Postgres => $@"""{nameof(TableRecord<TRecord>.CreatedBy)}""",
                                           DbInstance.MsSql    => nameof(TableRecord<TRecord>.CreatedBy),
                                           _                   => throw new OutOfRangeException( nameof(Instance), Instance ),
                                       };

    public string CurrentSchema => _database.CurrentSchema;

    public virtual string DateCreated => Instance switch
                                         {
                                             DbInstance.Postgres => $@"""{nameof(TableRecord<TRecord>.DateCreated)}""",
                                             DbInstance.MsSql    => nameof(TableRecord<TRecord>.DateCreated),
                                             _                   => throw new OutOfRangeException( nameof(Instance), Instance ),
                                         };

    public virtual IEnumerable<Descriptor> Descriptors => _propertiesCache.GetValues( this );

    public virtual string ID_ColumnName => Instance switch
                                           {
                                               DbInstance.Postgres => $@"""{nameof(TableRecord<TRecord>.ID)}""",
                                               DbInstance.MsSql    => nameof(TableRecord<TRecord>.ID),
                                               _                   => throw new OutOfRangeException( nameof(Instance), Instance ),
                                           };

    public DbInstance          Instance       => _database.Instance;
    public int                 CommandTimeout => _database.CommandTimeout;
    public IEnumerable<string> KeyValuePairs  => Descriptors.Select( x => x.KeyValuePair );

    public virtual string LastModified => Instance switch
                                          {
                                              DbInstance.Postgres => $@"""{nameof(TableRecord<TRecord>.LastModified)}""",
                                              DbInstance.MsSql    => nameof(TableRecord<TRecord>.LastModified),
                                              _                   => throw new OutOfRangeException( nameof(Instance), Instance ),
                                          };

    public virtual string OwnerUserID => Instance switch
                                         {
                                             DbInstance.Postgres => $@"""{nameof(TableRecord<TRecord>.OwnerUserID)}""",
                                             DbInstance.MsSql    => nameof(TableRecord<TRecord>.OwnerUserID),
                                             _                   => throw new OutOfRangeException( nameof(Instance), Instance ),
                                         };

    public string RandomMethod
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        get => Instance switch
               {
                   DbInstance.MsSql    => "NEWID()",
                   DbInstance.Postgres => "RANDOM()",
                   _                   => throw new OutOfRangeException( nameof(Instance), Instance ),
               };
    }
    public RecordGenerator<TRecord> Records => new(this);
    public virtual string SchemaTableName
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )] get => $"{CurrentSchema}.{TableName}";
    }
    public virtual string TableName
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        get => Instance switch
               {
                   DbInstance.Postgres => $"\"{typeof(TRecord).GetTableName()}\"",
                   DbInstance.MsSql    => typeof(TRecord).GetTableName(),
                   _                   => typeof(TRecord).GetTableName(),
               };
    }
    public IEnumerable<string> VariableNames => Descriptors.Select( x => x.VariableName );


    public DbTable( IConnectableDb database )
    {
        _database        = database;
        _propertiesCache = TypePropertiesCache.Current[typeof(TRecord)];
    }


    protected virtual CommandDefinition GetCommandDefinition( string sql, DynamicParameters? parameters, DbTransaction? transaction, CancellationToken token, CommandType? commandType = default, CommandFlags flags = CommandFlags.None ) =>
        new(sql, parameters, transaction, CommandTimeout, commandType, flags, token);


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Descriptor GetDescriptor( string columnName ) => _propertiesCache.Get( this, columnName );
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public string KeyValuePair( string columnName ) => GetDescriptor( columnName )
       .KeyValuePair;


    public ValueTask<IEnumerable<TRecord>> All( CancellationToken token = default ) => this.Call( All, token );
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<IEnumerable<TRecord>> All( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName}";

        if ( token.IsCancellationRequested ) { return Empty; }


        try
        {
            var command = new CommandDefinition( sql, default, transaction, default, default, CommandFlags.None, token );

            IEnumerable<TRecord> records = await connection.QueryAsync<TRecord>( sql, default, transaction );
            return records.GetArray();
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
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
            using SqlMapper.GridReader reader = await connection.QueryMultipleAsync( sql, parameters, transaction );
            return await func( reader, token );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    public ValueTask<TResult> Call<TResult>( string sql, DynamicParameters? parameters, Func<DbDataReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default ) =>
        this.TryCall( Call, sql, parameters, func, token );
    public virtual async ValueTask<TResult> Call<TResult>( DbConnection                                              connection,
                                                           DbTransaction                                             transaction,
                                                           string                                                    sql,
                                                           DynamicParameters?                                        parameters,
                                                           Func<DbDataReader, CancellationToken, ValueTask<TResult>> func,
                                                           CancellationToken                                         token = default
    )
    {
        try
        {
            await using DbDataReader reader = await connection.ExecuteReaderAsync( sql, parameters, transaction );
            return await func( reader, token );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
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


    public virtual ValueTask DisposeAsync() => default;
    public DbConnection Connect() => _database.Connect();
    public ValueTask<DbConnection> ConnectAsync( CancellationToken token = default ) => _database.ConnectAsync( token );
}
