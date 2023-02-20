﻿// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:54 PM


namespace Jakar.Database;


[SuppressMessage( "ReSharper", "InconsistentNaming" )]
public abstract class Constants<TRecord> : ObservableClass where TRecord : TableRecord<TRecord>
{
    public const           string ID                  = nameof(TableRecord<TRecord>.ID);
    public const           string POSTGRES_ID         = $@"""{ID}""";
    public static readonly string POSTGRES_TABLE_NAME = $"\"{typeof(TRecord).GetTableName()}\"";
    public static readonly string TABLE_NAME          = typeof(TRecord).GetTableName();
}



[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class DbTable<TRecord> : Constants<TRecord>, IConnectableDb, IAsyncDisposable where TRecord : TableRecord<TRecord>
{
    protected readonly IConnectableDb                 _database;
    protected readonly object?                        _nullParameters = null;
    protected readonly TypePropertiesCache.Properties _propertiesCache;


    internal static                                               TRecord[]                Empty         => Array.Empty<TRecord>();
    public                                                        DbInstance               Instance      => _database.Instance;
    [SuppressMessage( "ReSharper", "InconsistentNaming" )] public IDGenerator<TRecord>     IDs           => new(this);
    public                                                        RecordGenerator<TRecord> Records       => new(this);
    public                                                        string                   CurrentSchema => _database.CurrentSchema;

    protected internal virtual string IDKey => Instance switch
                                               {
                                                   DbInstance.Postgres => POSTGRES_ID,
                                                   DbInstance.MsSql    => ID,
                                                   _                   => throw new OutOfRangeException( nameof(Instance), Instance )
                                               };

    public virtual string SchemaTableName => $"{CurrentSchema}.{TableName}";

    public virtual string TableName => Instance switch
                                       {
                                           DbInstance.Postgres => POSTGRES_TABLE_NAME,
                                           DbInstance.MsSql    => TABLE_NAME,
                                           _                   => TABLE_NAME
                                       };


    protected internal IEnumerable<Descriptor> Descriptors   => _propertiesCache.NotKeys( this );
    protected internal IEnumerable<string>     KeyValuePairs => Descriptors.Select( x => x.KeyValuePair );
    protected internal IEnumerable<string>     ColumnNames   => Descriptors.Select( x => x.ColumnName );
    protected internal IEnumerable<string>     VariableNames => Descriptors.Select( x => x.VariableName );


    public DbTable( IConnectableDb database )
    {
        _database        = database;
        _propertiesCache = TypePropertiesCache.Current[typeof(TRecord)];
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected internal Descriptor GetDescriptor( string columnName ) => _propertiesCache[columnName, this];
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal string KeyValuePair( string columnName ) => GetDescriptor( columnName )
       .KeyValuePair;


    public ValueTask<TRecord[]> All( CancellationToken token = default ) => this.Call( All, token );
    public virtual async ValueTask<TRecord[]> All( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName}";

        if ( token.IsCancellationRequested ) { return Empty; }


        try
        {
            IEnumerable<TRecord> records = await connection.QueryAsync<TRecord>( sql, default, transaction );
            return GetArray( records );
        }
        catch ( Exception e ) { throw new SqlException( sql, _nullParameters, e ); }
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


    public ValueTask<long> Count( CancellationToken token = default ) => this.Call( Count, token );
    public virtual async ValueTask<long> Count( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT COUNT({IDKey}) FROM {SchemaTableName}";

        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.QueryFirstOrDefaultAsync<long>( sql, default, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, _nullParameters, e ); }
    }


    public ValueTask Delete( TRecord                   record,     CancellationToken token                                                              = default ) => this.TryCall( Delete, record,  token );
    public ValueTask Delete( IEnumerable<TRecord>      records,    CancellationToken token                                                              = default ) => this.TryCall( Delete, records, token );
    public ValueTask Delete( IAsyncEnumerable<TRecord> records,    CancellationToken token                                                              = default ) => this.TryCall( Delete, records, token );
    public virtual ValueTask Delete( DbConnection      connection, DbTransaction     transaction, TRecord              record,  CancellationToken token = default ) => Delete( connection, transaction, record.ID,                   token );
    public virtual ValueTask Delete( DbConnection      connection, DbTransaction     transaction, IEnumerable<TRecord> records, CancellationToken token = default ) => Delete( connection, transaction, records.Select( x => x.ID ), token );
    public async ValueTask Delete( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TRecord> records, CancellationToken token = default )
    {
        HashSet<TRecord> ids = await records.ToHashSet( token );
        await Delete( connection, transaction, ids, token );
    }


    public ValueTask Delete( string                   id,       CancellationToken token                               = default ) => this.TryCall( Delete, id,       token );
    public ValueTask Delete( IEnumerable<string>      ids,      CancellationToken token                               = default ) => this.TryCall( Delete, ids,      token );
    public ValueTask Delete( IAsyncEnumerable<string> ids,      CancellationToken token                               = default ) => this.TryCall( Delete, ids,      token );
    public ValueTask Delete( bool                     matchAll, DynamicParameters parameters, CancellationToken token = default ) => this.TryCall( Delete, matchAll, parameters, token );
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<string> ids, CancellationToken token = default )
    {
        HashSet<string> records = await ids.ToHashSet( token );

        await Delete( connection, transaction, records, token );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, string id, CancellationToken token = default )
    {
        string cmd = $"DELETE FROM {SchemaTableName} WHERE {IDKey} = {id};";

        if ( token.IsCancellationRequested ) { return; }

        await connection.ExecuteScalarAsync( cmd, default, transaction );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, IEnumerable<string> ids, CancellationToken token = default )
    {
        string sql = $"DELETE FROM {SchemaTableName} WHERE {IDKey} in ( {string.Join( ',', ids.Select( x => $"'{x}'" ) )} );";

        if ( token.IsCancellationRequested ) { return; }

        try { await connection.ExecuteScalarAsync( sql, default, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, _nullParameters, e ); }
    }
    public async ValueTask Delete( DbConnection connection, DbTransaction transaction, bool matchAll, DynamicParameters parameters, CancellationToken token )
    {
        string cmd = $"DELETE FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                             ? "AND"
                                                                             : "OR",
                                                                         parameters.ParameterNames.Select( KeyValuePair ) )};";

        if ( token.IsCancellationRequested ) { return; }

        await connection.ExecuteScalarAsync( cmd, parameters, transaction );
    }


    public ValueTask<bool> Exists( bool matchAll, DynamicParameters parameters, CancellationToken token ) => this.TryCall( Exists, matchAll, parameters, token );
    public virtual async ValueTask<bool> Exists( DbConnection connection, DbTransaction transaction, bool matchAll, DynamicParameters parameters, CancellationToken token )
    {
        string sql = $"SELECT TOP 1 {IDKey} FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                                           ? "AND"
                                                                                           : "OR",
                                                                                       parameters.ParameterNames.Select( KeyValuePair ) )}";

        token.ThrowIfCancellationRequested();

        try
        {
            IEnumerable<string> results = await connection.QueryAsync<string>( sql, parameters, transaction );
            return results.Any();
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    public ValueTask<TRecord?> First( CancellationToken token = default ) => this.Call( First, token );
    public virtual async ValueTask<TRecord?> First( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} ORDER BY {IDKey} ASC LIMIT 1";

        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.QueryFirstAsync<TRecord>( sql, default, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, _nullParameters, e ); }
    }


    public ValueTask<TRecord?> FirstOrDefault( CancellationToken token = default ) => this.Call( FirstOrDefault, token );
    public virtual async ValueTask<TRecord?> FirstOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} ORDER BY {IDKey} ASC LIMIT 1";

        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.QueryFirstOrDefaultAsync<TRecord>( sql, default, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, _nullParameters, e ); }
    }


    public ValueTask<TRecord?> Get( bool               matchAll,   DynamicParameters parameters, CancellationToken token = default ) => this.Call( Get, matchAll,   parameters, token );
    public ValueTask<TRecord?> Get( string             columnName, object?           value,      CancellationToken token = default ) => this.Call( Get, columnName, value,      token );
    public ValueTask<TRecord?> Get( string?            id,         CancellationToken token                                            = default ) => this.Call( Get, id, token );
    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction?    transaction, string? id, CancellationToken token = default ) => await Get( connection, transaction, ID, id, token );
    public virtual async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                               ? "AND"
                                                                               : "OR",
                                                                           parameters.ParameterNames.Select( x => GetDescriptor( x ).KeyValuePair ) )}";


        if ( token.IsCancellationRequested ) { return default; }

        IEnumerable<TRecord>? records;
        TRecord?              result = default;

        try { records = await connection.QueryAsync<TRecord>( sql, parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }

        foreach ( TRecord record in records )
        {
            if ( result is not null )
            {
                throw new SqlException( sql, parameters, "Multiple records found" )
                      {
                          MatchAll = matchAll
                      };
            }

            result = record;
        }

        return result;
    }
    public virtual async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, string columnName, object? value, CancellationToken token = default )
    {
        DynamicParameters parameters = GetParameters( value );

        string sql = $"SELECT * FROM {SchemaTableName} WHERE {columnName} = @{nameof(value)}";

        if ( token.IsCancellationRequested ) { return default; }

        try
        {
            IEnumerable<TRecord?> items = await connection.QueryAsync<TRecord>( sql, parameters, transaction );
            return items.SingleOrDefault();
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }
    public ValueTask<TRecord[]> Get( IEnumerable<string>      ids, CancellationToken token = default ) => this.Call( Get, ids, token );
    public ValueTask<TRecord[]> Get( IAsyncEnumerable<string> ids, CancellationToken token = default ) => this.Call( Get, ids, token );
    public virtual async ValueTask<TRecord[]> Get( DbConnection connection, DbTransaction? transaction, IAsyncEnumerable<string> ids, CancellationToken token = default )
    {
        HashSet<string> values = await ids.ToHashSet( token );
        return await Get( connection, transaction, values, token );
    }
    public virtual ValueTask<TRecord[]> Get( DbConnection connection, DbTransaction? transaction, IEnumerable<string> ids, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} WHERE {IDKey} in {string.Join( ',', ids.Select( x => $"'{x}'" ) )}";

        return Where( connection, transaction, sql, default, token );
    }


    public virtual T[] GetArray<T>( IEnumerable<T> enumerable ) => enumerable switch
                                                                   {
                                                                       List<T> list       => list.GetInternalArray(),
                                                                       Collection<T> list => list.GetInternalArray(),
                                                                       T[] array          => array,
                                                                       _                  => enumerable.ToArray()
                                                                   };


    public ValueTask<string?> GetID( string sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( GetID, sql, parameters, token );
    public async ValueTask<string?> GetID( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.QuerySingleAsync<string>( sql, parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }
    public ValueTask<string?> GetID( string columnName, object value, CancellationToken token = default ) => this.Call( GetID, columnName, value, token );
    public virtual async ValueTask<string?> GetID( DbConnection connection, DbTransaction? transaction, string columnName, object value, CancellationToken token = default )
    {
        string sql = $"SELECT {IDKey} FROM {SchemaTableName} WHERE {columnName} = @{nameof(value)}";
        return await GetID( connection, transaction, sql, GetParameters( value ), token );
    }


    protected static DynamicParameters GetParameters( object? value, object? template = default, [CallerArgumentExpression( "value" )] string? variableName = default )
    {
        ArgumentNullException.ThrowIfNull( variableName );
        var parameters = new DynamicParameters( template );
        parameters.Add( variableName, value );
        return parameters;
    }


    public IAsyncEnumerable<TRecord> Insert( IEnumerable<TRecord>      records, CancellationToken token = default ) => this.TryCall( Insert, records, token );
    public IAsyncEnumerable<TRecord> Insert( IAsyncEnumerable<TRecord> records, CancellationToken token = default ) => this.TryCall( Insert, records, token );
    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, IEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { yield return await Insert( connection, transaction, record, token ); }
    }
    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in records.WithCancellation( token ) ) { yield return await Insert( connection, transaction, record, token ); }
    }
    public ValueTask<TRecord> Insert( TRecord record, CancellationToken token = default ) => this.TryCall( Insert, record, token );
    public virtual async ValueTask<TRecord> Insert( DbConnection connection, DbTransaction transaction, TRecord record, CancellationToken token = default )
    {
        var sbColumnList = new StringBuilder();
        sbColumnList.AppendJoin( ',', ColumnNames );


        var sbParameterList = new StringBuilder();

        sbParameterList.AppendJoin( ',', VariableNames );


        string sql        = $"INSERT INTO {SchemaTableName} ({sbColumnList}) values ({sbParameterList});";
        var    parameters = new DynamicParameters( record );


        if ( token.IsCancellationRequested ) { return record; }

        try
        {
            string id = await connection.ExecuteScalarAsync<string>( sql, parameters, transaction );
            return record.NewID( id );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    public ValueTask<TRecord?> Last( CancellationToken token = default ) => this.Call( Last, token );
    public virtual async ValueTask<TRecord?> Last( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} ORDER BY {IDKey} DESC LIMIT 1";
        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.QueryFirstAsync<TRecord>( sql, default, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, _nullParameters, e ); }
    }


    public ValueTask<TRecord?> LastOrDefault( CancellationToken token = default ) => this.Call( LastOrDefault, token );
    public virtual async ValueTask<TRecord?> LastOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested ) { return default; }

        string sql = $"SELECT * FROM {SchemaTableName} ORDER BY {IDKey} DESC LIMIT 1";

        try { return await connection.QueryFirstOrDefaultAsync<TRecord>( sql, default, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, _nullParameters, e ); }
    }


    public ValueTask<TRecord?> Next( string? id, CancellationToken token = default ) => this.Call( Next, id, token );
    public virtual async ValueTask<TRecord?> Next( DbConnection connection, DbTransaction? transaction, string? id, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( ID, id );
        string sql = @$"SELECT * FROM {SchemaTableName} WHERE ( id = IFNULL((SELECT MIN({IDKey}) FROM {SchemaTableName} WHERE {IDKey} > @{ID}), 0) )";
        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.ExecuteScalarAsync<TRecord>( sql, parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    public ValueTask<string?> NextID( string? id, CancellationToken token = default ) => this.Call( NextID, id, token );
    public virtual async ValueTask<string?> NextID( DbConnection connection, DbTransaction? transaction, string? id, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( ID, id );

        string sql = @$"SELECT {IDKey} FROM {SchemaTableName} WHERE ( id = IFNULL((SELECT MIN({IDKey}) FROM {SchemaTableName} WHERE {IDKey} > @{ID}), 0) )";
        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.ExecuteScalarAsync<string>( sql, parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    public ValueTask<TRecord?> Random( CancellationToken token = default ) => this.Call( Random, token );
    public virtual async ValueTask<TRecord?> Random( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} WHERE {IDKey} >= RAND() * ( SELECT MAX ({IDKey}) FROM table ) ORDER BY {IDKey} LIMIT 1";

        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.QueryFirstAsync<TRecord>( sql, default, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, _nullParameters, e ); }
    }


    public ValueTask<TRecord?> Random( UserRecord user, CancellationToken token = default ) => this.Call( Random, user, token );
    public virtual async ValueTask<TRecord?> Random( DbConnection connection, DbTransaction? transaction, UserRecord user, CancellationToken token = default )
    {
        var param = new DynamicParameters();
        param.Add( nameof(TableRecord<TRecord>.UserID), user.UserID );

        string sql = $"SELECT * FROM {SchemaTableName} WHERE {IDKey} >= RAND() * ( SELECT MAX ({IDKey}) FROM table ) AND {nameof(TableRecord<TRecord>.UserID)} = @{nameof(TableRecord<TRecord>.UserID)} ORDER BY {IDKey} LIMIT 1";

        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.QueryFirstAsync<TRecord>( sql, param, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, _nullParameters, e ); }
    }


    public ValueTask<TRecord[]> Random( string count, CancellationToken token = default ) => this.Call( Random, count, token );
    public virtual ValueTask<TRecord[]> Random( DbConnection connection, DbTransaction? transaction, string count, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} WHERE {IDKey} >= RAND() * ( SELECT MAX ({IDKey}) FROM table ) ORDER BY {IDKey} LIMIT {count}";

        return Where( connection, transaction, sql, default, token );
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


    public ValueTask<TRecord?> Single( string id,  CancellationToken  token                               = default ) => this.Call( Single, id,  token );
    public ValueTask<TRecord?> Single( string sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( Single, sql, parameters, token );
    public virtual async ValueTask<TRecord?> Single( DbConnection connection, DbTransaction? transaction, string id, CancellationToken token = default )
    {
        DynamicParameters parameters = GetParameters( id );

        string sql = $"SELECT * FROM {SchemaTableName} WHERE {IDKey} = @{nameof(id)}";

        if ( token.IsCancellationRequested ) { return default; }


        try { return await connection.QuerySingleAsync<TRecord>( sql, parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }
    public virtual async ValueTask<TRecord?> Single( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.QuerySingleAsync<TRecord>( sql, parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    public ValueTask<TRecord?> SingleOrDefault( string id,  CancellationToken  token                               = default ) => this.Call( SingleOrDefault, id,  token );
    public ValueTask<TRecord?> SingleOrDefault( string sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( SingleOrDefault, sql, parameters, token );
    public virtual async ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, string id, CancellationToken token = default )
    {
        DynamicParameters parameters = GetParameters( id );

        string sql = $"SELECT * FROM {SchemaTableName} WHERE {IDKey} = @{nameof(id)}";

        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.QuerySingleOrDefaultAsync<TRecord>( sql, parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }
    public virtual async ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.QuerySingleOrDefaultAsync<TRecord>( sql, parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    public ValueTask Update( TRecord                   record,  CancellationToken token = default ) => this.TryCall( Update, record,  token );
    public ValueTask Update( IEnumerable<TRecord>      records, CancellationToken token = default ) => this.TryCall( Update, records, token );
    public ValueTask Update( IAsyncEnumerable<TRecord> records, CancellationToken token = default ) => this.TryCall( Update, records, token );
    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, IEnumerable<TRecord> records, CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { await Update( connection, transaction, record, token ); }
    }
    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, IAsyncEnumerable<TRecord> records, CancellationToken token = default )
    {
        await foreach ( TRecord record in records.WithCancellation( token ) ) { await Update( connection, transaction, record, token ); }
    }
    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, TRecord record, CancellationToken token = default )
    {
        string            id         = record.ID;
        DynamicParameters parameters = GetParameters( id, record );
        string            sql        = $"UPDATE {SchemaTableName} SET {string.Join( ',', KeyValuePairs )} WHERE {IDKey} = @{nameof(id)};";

        if ( token.IsCancellationRequested ) { return; }

        try { await connection.ExecuteScalarAsync( sql, parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, record, e ); }
    }


    public ValueTask<TRecord[]> Where( bool matchAll, DynamicParameters parameters, CancellationToken token = default ) => this.Call( Where, matchAll, parameters, token );
    public ValueTask<TRecord[]> Where( string sql, DynamicParameters? parameters, CancellationToken token = default ) =>
        this.Call( Where, sql, parameters, token );
    public ValueTask<TRecord[]> Where<TValue>( string columnName, TValue? value, CancellationToken token = default ) => this.Call( Where, columnName, value, token );
    public virtual ValueTask<TRecord[]> Where( DbConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                               ? "AND"
                                                                               : "OR",
                                                                           parameters.ParameterNames.Select( KeyValuePair ) )}";

        return Where( connection, transaction, sql, parameters, token );
    }
    public virtual async ValueTask<TRecord[]> Where( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested ) { return Empty; }

        try
        {
            IEnumerable<TRecord> records = await connection.QueryAsync<TRecord>( sql, parameters, transaction );
            return GetArray( records );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }
    public virtual ValueTask<TRecord[]> Where<TValue>( DbConnection connection, DbTransaction? transaction, string columnName, TValue? value, CancellationToken token = default )
    {
        string sql        = $"SELECT * FROM {SchemaTableName} WHERE {columnName} = @{nameof(value)}";
        var    parameters = new DynamicParameters();
        parameters.Add( nameof(value), value );

        return Where( connection, transaction, sql, parameters, token );
    }


    public virtual ValueTask DisposeAsync() => default;
    public DbConnection Connect() => _database.Connect();
    public ValueTask<DbConnection> ConnectAsync( CancellationToken token = default ) => _database.ConnectAsync( token );
}
