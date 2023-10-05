// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:54 PM


namespace Jakar.Database;
// TrueLogic :: TrueLogic.Common
// 04/11/2023  11:33 AM



[ SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ) ]
public partial class DbTable<TRecord> : ObservableClass, IConnectableDb, IAsyncDisposable where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    protected static readonly TypePropertiesCache.Properties _propertiesCache = TypePropertiesCache.Current.Register<TRecord>();
    protected const           char                           QUOTE            = '"';
    protected readonly        IConnectableDbRoot             _database;


    [ SuppressMessage( "ReSharper", "ReturnTypeCanBeEnumerable.Global" ) ] public static ImmutableArray<TRecord> Empty          => ImmutableArray<TRecord>.Empty;
    [ SuppressMessage( "ReSharper", "ReturnTypeCanBeEnumerable.Global" ) ] public static ImmutableList<TRecord>  EmptyList      => ImmutableList<TRecord>.Empty;
    public                                                                               IEnumerable<string>     ColumnNames    => Descriptors.Select( x => x.ColumnName );
    public                                                                               int                     CommandTimeout => _database.CommandTimeout;

    public virtual string CreatedBy => Instance switch
                                       {
                                           DbInstance.Postgres => $"{QUOTE}{nameof(IOwnedTableRecord.CreatedBy)}{QUOTE}",
                                           DbInstance.MsSql    => nameof(IOwnedTableRecord.CreatedBy),
                                           _                   => throw new OutOfRangeException( nameof(Instance), Instance )
                                       };

    public string CurrentSchema => _database.CurrentSchema;

    public virtual string DateCreated => Instance switch
                                         {
                                             DbInstance.Postgres => $@"""{nameof(TableRecord<TRecord>.DateCreated)}""",
                                             DbInstance.MsSql    => nameof(TableRecord<TRecord>.DateCreated),
                                             _                   => throw new OutOfRangeException( nameof(Instance), Instance )
                                         };

    public virtual IEnumerable<Descriptor> Descriptors => _propertiesCache.GetValues( this );

    public virtual string ID_ColumnName => Instance switch
                                           {
                                               DbInstance.Postgres => $@"""{nameof(TableRecord<TRecord>.ID)}""",
                                               DbInstance.MsSql    => nameof(TableRecord<TRecord>.ID),
                                               _                   => throw new OutOfRangeException( nameof(Instance), Instance )
                                           };

    public DbInstance          Instance      => _database.Instance;
    public IEnumerable<string> KeyValuePairs => Descriptors.Select( x => x.KeyValuePair );

    public virtual string LastModified => Instance switch
                                          {
                                              DbInstance.Postgres => $@"""{nameof(TableRecord<TRecord>.LastModified)}""",
                                              DbInstance.MsSql    => nameof(TableRecord<TRecord>.LastModified),
                                              _                   => throw new OutOfRangeException( nameof(Instance), Instance )
                                          };

    public virtual string OwnerUserID => Instance switch
                                         {
                                             DbInstance.Postgres => $"{QUOTE}{nameof(IOwnedTableRecord.OwnerUserID)}{QUOTE}",
                                             DbInstance.MsSql    => nameof(IOwnedTableRecord.OwnerUserID),
                                             _                   => throw new OutOfRangeException( nameof(Instance), Instance )
                                         };

    public string RandomMethod
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
        get => Instance switch
               {
                   DbInstance.MsSql    => "NEWID()",
                   DbInstance.Postgres => "RANDOM()",
                   _                   => throw new OutOfRangeException( nameof(Instance), Instance )
               };
    }
    public RecordGenerator<TRecord> Records => new(this);
    public virtual string SchemaTableName
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ] get => $"{CurrentSchema}.{TableName}";
    }
    public virtual string TableName
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
        get => Instance switch
               {
                   DbInstance.Postgres => $"{QUOTE}{TRecord.TableName}{QUOTE}",
                   DbInstance.MsSql    => TRecord.TableName,
                   _                   => TRecord.TableName
               };
    }
    public IEnumerable<string> VariableNames => Descriptors.Select( x => x.VariableName );


    public DbTable( IConnectableDbRoot database )
    {
        _database = database;

        if ( TRecord.TableName != typeof(TRecord).GetTableName() ) { throw new InvalidOperationException( $"{TRecord.TableName} != {typeof(TRecord).GetTableName()}" ); }
    }


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public Descriptor GetDescriptor( string columnName ) => _propertiesCache.Get( this, columnName );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public string KeyValuePair( string columnName ) => GetDescriptor( columnName )
       .KeyValuePair;


    public IAsyncEnumerable<TRecord> All( CancellationToken token = default ) => this.Call( All, token );
    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual async IAsyncEnumerable<TRecord> All( DbConnection connection, DbTransaction? transaction, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName}";

        if ( token.IsCancellationRequested ) { yield break; }

        DbDataReader reader;

        try
        {
            var command = new CommandDefinition( sql, default, transaction, default, default, CommandFlags.None, token );
            reader = await connection.ExecuteReaderAsync( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }

        await using ( reader )
        {
            await foreach ( TRecord record in TRecord.CreateAsync( reader, token ) ) { yield return record; }
        }
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
            CommandDefinition        command = _database.GetCommandDefinition( sql, parameters, transaction, token );
            await using DbDataReader reader  = await connection.ExecuteReaderAsync( command );
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


    public virtual ValueTask               DisposeAsync()                                    => default;
    public         DbConnection            Connect()                                         => _database.Connect();
    public         ValueTask<DbConnection> ConnectAsync( CancellationToken token = default ) => _database.ConnectAsync( token );
}
