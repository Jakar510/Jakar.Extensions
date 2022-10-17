// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:54 PM

namespace Jakar.Database;
#nullable enable
#pragma warning disable CS8424 // The EnumeratorCancellationAttribute will have no effect. The attribute is only effective on a parameter of type CancellationToken in an async-iterator method returning IAsyncEnumerable



[SuppressMessage( "ReSharper", "InconsistentNaming" )]
public class DbTableBase<TRecord> : ObservableClass, IConnectableDb<TRecord>, IAsyncDisposable where TRecord : TableRecord<TRecord>

{
    protected readonly IConnectableDb           _database;
    internal static    List<TRecord>            Empty         { get; } = new();
    public             IDGenerator<TRecord>     IDs           => new(this);
    public             RecordGenerator<TRecord> Records       => new(this);
    public             string                   CurrentSchema => _database.CurrentSchema;
    public virtual     string                   TableName     { get; } = typeof(TRecord).GetTableName();


    public DbTableBase( IConnectableDb database ) => _database = database;


    public ValueTask<List<TRecord>> All( CancellationToken token = default ) => this.Call( All, token );
    public virtual async ValueTask<List<TRecord>> All( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {TableName}";

        if (token.IsCancellationRequested) { return Empty; }

        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>( sql, default, transaction );
        if (items is List<TRecord> list) { return list; }

        return items.ToList();
    }


    public ValueTask<TResult> Call<TResult>( string sql, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default ) =>
        this.TryCall( Call, sql, parameters, func, token );
    public async ValueTask<TResult> Call<TResult>( DbConnection                                                      connection,
                                                   DbTransaction                                                     transaction,
                                                   string                                                            sql,
                                                   DynamicParameters?                                                parameters,
                                                   Func<SqlMapper.GridReader, CancellationToken, ValueTask<TResult>> func,
                                                   CancellationToken                                                 token = default
    )
    {
        using SqlMapper.GridReader reader = await connection.QueryMultipleAsync( sql, parameters, transaction );
        return await func( reader, token );
    }


    public ValueTask<TResult> Call<TResult>( string sql, DynamicParameters? parameters, Func<DbDataReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default ) =>
        this.TryCall( Call, sql, parameters, func, token );
    public async ValueTask<TResult> Call<TResult>( DbConnection connection, DbTransaction transaction, string sql, DynamicParameters? parameters, Func<DbDataReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default )
    {
        await using DbDataReader reader = await connection.ExecuteReaderAsync( sql, parameters, transaction );
        return await func( reader, token );
    }


    public ValueTask<long> Count( CancellationToken token = default ) => this.Call( Count, token );
    public virtual async ValueTask<long> Count( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT COUNT({nameof(IDataBaseID.ID)}) FROM {TableName}";

        if (token.IsCancellationRequested) { return default; }

        return await connection.QueryFirstOrDefaultAsync<long>( sql, default, transaction );
    }


    public ValueTask Delete( TRecord                   record,     CancellationToken token                                                              = default ) => this.TryCall( Delete, record,  token );
    public ValueTask Delete( IEnumerable<TRecord>      records,    CancellationToken token                                                              = default ) => this.TryCall( Delete, records, token );
    public ValueTask Delete( IAsyncEnumerable<TRecord> records,    CancellationToken token                                                              = default ) => this.TryCall( Delete, records, token );
    public ValueTask Delete( DbConnection              connection, DbTransaction     transaction, TRecord              record,  CancellationToken token = default ) => Delete( connection, transaction, record.ID,                   token );
    public ValueTask Delete( DbConnection              connection, DbTransaction     transaction, IEnumerable<TRecord> records, CancellationToken token = default ) => Delete( connection, transaction, records.Select( x => x.ID ), token );
    public async ValueTask Delete( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TRecord> records, CancellationToken token = default )
    {
        var ids = new List<long>();
        await foreach (TRecord record in records.WithCancellation( token )) { ids.Add( record.ID ); }

        await Delete( connection, transaction, ids, token );
    }


    public ValueTask Delete( long                   id,  CancellationToken token = default ) => this.TryCall( Delete, id,  token );
    public ValueTask Delete( IEnumerable<long>      ids, CancellationToken token = default ) => this.TryCall( Delete, ids, token );
    public ValueTask Delete( IAsyncEnumerable<long> ids, CancellationToken token = default ) => this.TryCall( Delete, ids, token );
    public async ValueTask Delete( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<long> ids, CancellationToken token = default )
    {
        var records = new List<long>();
        await foreach (long id in ids.WithCancellation( token )) { records.Add( id ); }

        await Delete( connection, transaction, records, token );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, long id, CancellationToken token = default )
    {
        string cmd = $"DELETE FROM {CurrentSchema}.{TableName} WHERE {nameof(IDataBaseID.ID)} = {id};";

        if (token.IsCancellationRequested) { return; }

        await connection.ExecuteScalarAsync( cmd, default, transaction );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, IEnumerable<long> ids, CancellationToken token = default )
    {
        string cmd = $"DELETE FROM {CurrentSchema}.{TableName} WHERE {nameof(IDataBaseID.ID)} in ( {string.Join( ',', ids )} );";

        if (token.IsCancellationRequested) { return; }

        await connection.ExecuteScalarAsync( cmd, default, transaction );
    }


    public ValueTask<TRecord?> First( CancellationToken token = default ) => this.Call( First, token );
    public virtual async ValueTask<TRecord?> First( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {CurrentSchema}.{TableName} ORDER BY {nameof(IDataBaseID.ID)} ASC LIMIT 1";

        if (token.IsCancellationRequested) { return default; }

        return await connection.QueryFirstAsync<TRecord>( sql, default, transaction );
    }


    public ValueTask<TRecord?> FirstOrDefault( CancellationToken token = default ) => this.Call( FirstOrDefault, token );
    public virtual async ValueTask<TRecord?> FirstOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {CurrentSchema}.{TableName} ORDER BY {nameof(IDataBaseID.ID)} ASC LIMIT 1";

        if (token.IsCancellationRequested) { return default; }

        return await connection.QueryFirstOrDefaultAsync<TRecord>( sql, default, transaction );
    }
    public IAsyncEnumerable<TRecord?> Get( IEnumerable<long>      ids, CancellationToken token = default ) => this.Call( Get, ids, token );
    public IAsyncEnumerable<TRecord?> Get( IAsyncEnumerable<long> ids, CancellationToken token = default ) => this.Call( Get, ids, token );
    public async IAsyncEnumerable<TRecord?> Get( DbConnection connection, DbTransaction? transaction, IAsyncEnumerable<long> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        var values = new List<long>();
        await foreach (long id in ids.WithCancellation( token )) { values.Add( id ); }

        await foreach (TRecord? record in Get( connection, transaction, values, token )) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TRecord?> Get( DbConnection connection, DbTransaction? transaction, IEnumerable<long> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        var sb = new StringBuilder();

        if (typeof(long) == typeof(string)) { sb.AppendJoin( ',', ids.Select( x => $"'{x}'" ) ); }
        else { sb.AppendJoin( ',',                                ids ); }

        string sql = $"SELECT * FROM {CurrentSchema}.{TableName} WHERE {nameof(IDataBaseID.ID)} in {sb}";

        if (token.IsCancellationRequested) { yield break; }

        using SqlMapper.GridReader reader = await connection.QueryMultipleAsync( sql, default, transaction );
        IEnumerable<TRecord>       items  = await reader.ReadAsync<TRecord>( false );

        foreach (TRecord record in items)
        {
            if (token.IsCancellationRequested) { yield break; }

            yield return record;
        }
    }


    public ValueTask<TRecord?> Get( bool               matchAll,   DynamicParameters parameters, CancellationToken token = default ) => this.Call( Get, matchAll,   parameters, token );
    public ValueTask<TRecord?> Get<TValue>( string     columnName, TValue?           value,      CancellationToken token = default ) => this.Call( Get, columnName, value,      token );
    public ValueTask<TRecord?> Get( long               id,         CancellationToken token                                         = default ) => this.Call( Get, id, token );
    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction?    transaction, long id, CancellationToken token = default ) => await Get( connection, transaction, nameof(IDataBaseID.ID), id, token );
    public virtual async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        string sql = new StringBuilder( $"SELECT * FROM {CurrentSchema}.{TableName} WHERE " ).AppendJoin( matchAll
                                                                                                              ? "AND"
                                                                                                              : "OR",
                                                                                                          parameters.ParameterNames.Select( x => $"{x} = @{x}" ) )
                                                                                             .ToString();


        if (token.IsCancellationRequested) { return default; }

        IEnumerable<TRecord>? records = await connection.QueryAsync<TRecord>( sql, parameters, transaction );
        TRecord?              result  = default;
        if (token.IsCancellationRequested) { return default; }


        foreach (TRecord record in records)
        {
            if (result is not null)
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
    public virtual async ValueTask<TRecord?> Get<TValue>( DbConnection connection, DbTransaction? transaction, string columnName, TValue? value, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(value), value );

        string sql = $"SELECT * FROM {CurrentSchema}.{TableName} WHERE {columnName} = @{nameof(value)}";

        if (token.IsCancellationRequested) { return default; }

        IEnumerable<TRecord?> items = await connection.QueryAsync<TRecord>( sql, parameters, transaction );
        return items.SingleOrDefault();
    }


    public ValueTask<long> GetID( string sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( GetID, sql, parameters, token );
    public async ValueTask<long> GetID( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        if (token.IsCancellationRequested) { return default; }

        return await connection.QuerySingleAsync<long>( sql, parameters, transaction );
    }


    public ValueTask<long> GetID<TValue>( string columnName, TValue value, CancellationToken token = default ) => this.Call( GetID, columnName, value, token );
    public virtual async ValueTask<long> GetID<TValue>( DbConnection connection, DbTransaction? transaction, string columnName, TValue value, CancellationToken token = default )
    {
        string sql        = $"SELECT {nameof(IDataBaseID.ID)} FROM {CurrentSchema}.{TableName} WHERE {columnName} = @{nameof(value)}";
        var    parameters = new DynamicParameters();
        parameters.Add( nameof(value), value );
        token.ThrowIfCancellationRequested();
        return await connection.QuerySingleAsync<long>( sql, parameters, transaction );
    }
    public IAsyncEnumerable<TRecord> Insert( IEnumerable<TRecord>      records, CancellationToken token = default ) => this.TryCall( Insert, records, token );
    public IAsyncEnumerable<TRecord> Insert( IAsyncEnumerable<TRecord> records, CancellationToken token = default ) => this.TryCall( Insert, records, token );
    public async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, IEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach (TRecord record in records) { yield return await Insert( connection, transaction, record, token ); }
    }
    public async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach (TRecord record in records.WithCancellation( token )) { yield return await Insert( connection, transaction, record, token ); }
    }


    public ValueTask<TRecord> Insert( TRecord record, CancellationToken token = default ) => this.TryCall( Insert, record, token );
    public virtual async ValueTask<TRecord> Insert( DbConnection connection, DbTransaction transaction, TRecord record, CancellationToken token = default )
    {
        List<Descriptor> descriptors = TypePropertiesCache.Current[typeof(TRecord)]
                                                          .Where( x => !x.IsKey )
                                                          .ToList();


        var sbColumnList = new StringBuilder();
        sbColumnList.AppendJoin( ',', descriptors.Select( x => x.ColumnName ) );


        var sbParameterList = new StringBuilder();
        sbParameterList.AppendJoin( ',', descriptors.Select( x => x.VariableName ) );


        string cmd        = $"INSERT INTO {CurrentSchema}.{TableName} ({sbColumnList}) values ({sbParameterList});";
        var    parameters = new DynamicParameters( record );


        if (token.IsCancellationRequested) { return record; }

        long id = await connection.ExecuteScalarAsync<long>( cmd, parameters, transaction );
        return record.NewID( id );
    }


    public ValueTask<TRecord?> Last( CancellationToken token = default ) => this.Call( Last, token );
    public virtual async ValueTask<TRecord?> Last( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {CurrentSchema}.{TableName} ORDER BY {nameof(IDataBaseID.ID)} DESC LIMIT 1";

        if (token.IsCancellationRequested) { return default; }

        return await connection.QueryFirstAsync<TRecord>( sql, default, transaction );
    }


    public ValueTask<TRecord?> LastOrDefault( CancellationToken token = default ) => this.Call( LastOrDefault, token );
    public virtual async ValueTask<TRecord?> LastOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        if (token.IsCancellationRequested) { return default; }

        string sql = $"SELECT * FROM {CurrentSchema}.{TableName} ORDER BY {nameof(IDataBaseID.ID)} DESC LIMIT 1";

        return await connection.QueryFirstOrDefaultAsync<TRecord>( sql, default, transaction );
    }


    public ValueTask<TRecord?> Next( long id, CancellationToken token = default ) => this.Call( Next, id, token );
    public virtual async ValueTask<TRecord?> Next( DbConnection connection, DbTransaction? transaction, long id, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(IDataBaseID.ID), id );
        string sql = @$"SELECT * FROM {CurrentSchema}.{TableName} WHERE ( id = IFNULL((SELECT MIN({nameof(IDataBaseID.ID)}) FROM {CurrentSchema}.{TableName} WHERE {nameof(IDataBaseID.ID)} > @{nameof(IDataBaseID.ID)}), 0) )";
        if (token.IsCancellationRequested) { return default; }

        return await connection.ExecuteScalarAsync<TRecord>( sql, parameters, transaction );
    }


    public ValueTask<long> NextID( long id, CancellationToken token = default ) => this.Call( NextID, id, token );
    public virtual async ValueTask<long> NextID( DbConnection connection, DbTransaction? transaction, long id, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(IDataBaseID.ID), id );
        string sql = @$"SELECT {nameof(IDataBaseID.ID)} FROM {CurrentSchema}.{TableName} WHERE ( id = IFNULL((SELECT MIN({nameof(IDataBaseID.ID)}) FROM {CurrentSchema}.{TableName} WHERE {nameof(IDataBaseID.ID)} > @{nameof(IDataBaseID.ID)}), 0) )";
        if (token.IsCancellationRequested) { return default; }

        return await connection.ExecuteScalarAsync<long>( sql, parameters, transaction );
    }


    public IAsyncEnumerable<TRecord> Random( long count, CancellationToken token = default ) => this.Call( Random, count, token );
    public virtual async IAsyncEnumerable<TRecord> Random( DbConnection connection, DbTransaction? transaction, long count, [EnumeratorCancellation] CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {CurrentSchema}.{TableName} WHERE {nameof(IDataBaseID.ID)} >= RAND() * ( SELECT MAX ({nameof(IDataBaseID.ID)}) FROM table ) ORDER BY {nameof(IDataBaseID.ID)} LIMIT {count}";

        if (token.IsCancellationRequested) { yield break; }

        using SqlMapper.GridReader reader = await connection.QueryMultipleAsync( sql, default, transaction );
        IEnumerable<TRecord>       items  = await reader.ReadAsync<TRecord>( false );

        foreach (TRecord record in items)
        {
            if (token.IsCancellationRequested) { yield break; }

            yield return record;
        }
    }
    public ValueTask<TRecord?> Random( CancellationToken token                         = default ) => this.Call( Random, token );
    public ValueTask<TRecord?> Random( UserRecord        user, CancellationToken token = default ) => this.Call( Random, user, token );
    public virtual async ValueTask<TRecord?> Random( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {CurrentSchema}.{TableName} WHERE {nameof(IDataBaseID.ID)} >= RAND() * ( SELECT MAX ({nameof(IDataBaseID.ID)}) FROM table ) ORDER BY {nameof(IDataBaseID.ID)} LIMIT 1";

        if (token.IsCancellationRequested) { return default; }

        return await connection.QueryFirstAsync<TRecord>( sql, default, transaction );
    }
    public virtual async ValueTask<TRecord?> Random( DbConnection connection, DbTransaction? transaction, UserRecord user, CancellationToken token = default )
    {
        var param = new DynamicParameters();
        param.Add( nameof(TableRecord<TRecord>.UserID), user.UserID );

        string sql =
            $"SELECT * FROM {CurrentSchema}.{TableName} WHERE {nameof(IDataBaseID.ID)} >= RAND() * ( SELECT MAX ({nameof(IDataBaseID.ID)}) FROM table ) AND {nameof(TableRecord<TRecord>.UserID)} = @{nameof(TableRecord<TRecord>.UserID)} ORDER BY {nameof(IDataBaseID.ID)} LIMIT 1";

        if (token.IsCancellationRequested) { return default; }

        return await connection.QueryFirstAsync<TRecord>( sql, param, transaction );
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


    public async ValueTask<string> ServerVersion( CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync( token );
        return connection.ServerVersion;
    }


    public ValueTask<TRecord?> Single( long id, CancellationToken token = default ) => this.Call( Single, id, token );


    public ValueTask<TRecord?> Single( string sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( Single, sql, parameters, token );
    public virtual async ValueTask<TRecord?> Single( DbConnection connection, DbTransaction? transaction, long id, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(IDataBaseID.ID), id );

        string sql = $"SELECT * FROM {CurrentSchema}.{TableName} WHERE {nameof(IDataBaseID.ID)} = @{nameof(IDataBaseID.ID)}";

        if (token.IsCancellationRequested) { return default; }

        return await connection.QuerySingleAsync<TRecord>( sql, parameters, transaction );
    }
    public virtual async ValueTask<TRecord?> Single( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        if (token.IsCancellationRequested) { return default; }

        return await connection.QuerySingleAsync<TRecord>( sql, parameters, transaction );
    }


    public ValueTask<TRecord?> SingleOrDefault( long   id,  CancellationToken  token                               = default ) => this.Call( SingleOrDefault, id,  token );
    public ValueTask<TRecord?> SingleOrDefault( string sql, DynamicParameters? parameters, CancellationToken token = default ) => this.Call( SingleOrDefault, sql, parameters, token );
    public virtual async ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, long id, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(IDataBaseID.ID), id );

        string sql = $"SELECT * FROM {CurrentSchema}.{TableName} WHERE {nameof(IDataBaseID.ID)} = @{nameof(IDataBaseID.ID)}";

        if (token.IsCancellationRequested) { return default; }

        return await connection.QuerySingleOrDefaultAsync<TRecord>( sql, parameters, transaction );
    }
    public virtual async ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        if (token.IsCancellationRequested) { return default; }

        return await connection.QuerySingleOrDefaultAsync<TRecord>( sql, parameters, transaction );
    }


    public ValueTask Update( TRecord                   record,  CancellationToken token = default ) => this.TryCall( Update, record,  token );
    public ValueTask Update( IEnumerable<TRecord>      records, CancellationToken token = default ) => this.TryCall( Update, records, token );
    public ValueTask Update( IAsyncEnumerable<TRecord> records, CancellationToken token = default ) => this.TryCall( Update, records, token );
    public async ValueTask Update( DbConnection connection, DbTransaction? transaction, IEnumerable<TRecord> records, CancellationToken token = default )
    {
        foreach (TRecord record in records) { await Update( connection, transaction, record, token ); }
    }
    public async ValueTask Update( DbConnection connection, DbTransaction? transaction, IAsyncEnumerable<TRecord> records, CancellationToken token = default )
    {
        await foreach (TRecord record in records.WithCancellation( token )) { await Update( connection, transaction, record, token ); }
    }
    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, TRecord record, CancellationToken token = default )
    {
        string cmd        = $"UPDATE {CurrentSchema}.{TableName} SET {string.Join( ',', TypePropertiesCache.Current[typeof(TRecord)].Where( x => !x.IsKey ).Select( x => x.UpdateName ) )} WHERE {nameof(IDataBaseID.ID)} = @{nameof(IDataBaseID.ID)};";
        var    parameters = new DynamicParameters( record );

        if (token.IsCancellationRequested) { return; }

        await connection.ExecuteScalarAsync( cmd, parameters, transaction );
    }


    public IAsyncEnumerable<TRecord> Where( bool           matchAll,   DynamicParameters  parameters, CancellationToken token = default ) => this.Call( Where, matchAll,   parameters, token );
    public IAsyncEnumerable<TRecord> Where( string         sql,        DynamicParameters? parameters, CancellationToken token = default ) => this.Call( Where, sql,        parameters, token );
    public IAsyncEnumerable<TRecord> Where<TValue>( string columnName, TValue?            value,      CancellationToken token = default ) => this.Call( Where, columnName, value,      token );
    public virtual IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        string sql = new StringBuilder( $"SELECT * FROM {CurrentSchema}.{TableName} WHERE " ).AppendJoin( matchAll
                                                                                                              ? "AND"
                                                                                                              : "OR",
                                                                                                          parameters.ParameterNames.Select( x => $"{x} = @{x}" ) )
                                                                                             .ToString();

        return Where( connection, transaction, sql, parameters, token );
    }
    public virtual async IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
    {
        if (token.IsCancellationRequested) { yield break; }

        using SqlMapper.GridReader reader = await connection.QueryMultipleAsync( sql, parameters, transaction );
        IEnumerable<TRecord>       items  = await reader.ReadAsync<TRecord>( false );

        foreach (TRecord record in items)
        {
            if (token.IsCancellationRequested) { yield break; }

            yield return record;
        }
    }
    public virtual async IAsyncEnumerable<TRecord> Where<TValue>( DbConnection connection, DbTransaction? transaction, string columnName, TValue? value, [EnumeratorCancellation] CancellationToken token = default )
    {
        string sql        = $"SELECT * FROM {CurrentSchema}.{TableName} WHERE {columnName} = @{nameof(value)}";
        var    parameters = new DynamicParameters();
        parameters.Add( nameof(value), value );

        using SqlMapper.GridReader reader = await connection.QueryMultipleAsync( sql, parameters, transaction );
        IEnumerable<TRecord>       items  = await reader.ReadAsync<TRecord>( false );

        foreach (TRecord record in items)
        {
            if (token.IsCancellationRequested) { yield break; }

            yield return record;
        }
    }
    public ValueTask DisposeAsync() => default;


    public DbConnection Connect() => _database.Connect();
    public ValueTask<DbConnection> ConnectAsync( CancellationToken token = default ) => _database.ConnectAsync( token );
}
