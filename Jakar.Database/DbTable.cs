namespace Jakar.Database;


public abstract partial class DbTable<TRecord, TID> : ObservableClass, IDbTable<TRecord, TID> where TRecord : BaseTableRecord<TRecord, TID>
                                                                                              where TID : IComparable<TID>, IEquatable<TID>
{
    private readonly IConnectableDb _database;
    public virtual   string         TableName { get; } = DbExtensions.GetTableName<TRecord>();


    protected DbTable( IConnectableDb database ) => _database = database;
    public virtual ValueTask DisposeAsync() => default;


    public DbConnection Connect() => _database.Connect();
    public Task<DbConnection> ConnectAsync( CancellationToken token = default ) => _database.ConnectAsync(token);


    public virtual async Task<string> ServerVersion( CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync(token);
        return connection.ServerVersion;
    }


    public virtual async Task<DataTable> Schema( DbConnection connection, CancellationToken token = default ) => await connection.GetSchemaAsync(token);
    public virtual async Task<DataTable> Schema( DbConnection connection, string            collectionName, CancellationToken token = default ) => await connection.GetSchemaAsync(collectionName,                                      token);
    public virtual async Task<DataTable> Schema( DbConnection connection, string            collectionName, string?[]         restrictionValues, CancellationToken token = default ) => await connection.GetSchemaAsync(collectionName, restrictionValues, token);


    public virtual async Task Schema( Func<DataTable, CancellationToken, Task> func, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, token);
        await func(schema, token);
    }
    public virtual async Task Schema( Func<DataTable, CancellationToken, Task> func, string collectionName, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, collectionName, token);
        await func(schema, token);
    }
    public virtual async Task Schema( Func<DataTable, CancellationToken, Task> func, string collectionName, string?[] restrictionValues, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, collectionName, restrictionValues, token);
        await func(schema, token);
    }


    public virtual async Task<TResult> Schema<TResult>( Func<DataTable, CancellationToken, Task<TResult>> func, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, token);
        return await func(schema, token);
    }
    public virtual async Task<TResult> Schema<TResult>( Func<DataTable, CancellationToken, Task<TResult>> func, string collectionName, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, collectionName, token);
        return await func(schema, token);
    }
    public virtual async Task<TResult> Schema<TResult>( Func<DataTable, CancellationToken, Task<TResult>> func, string collectionName, string?[] restrictionValues, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, collectionName, restrictionValues, token);
        return await func(schema, token);
    }


    public virtual async Task<long> Count( CancellationToken token = default ) => await this.Call(Count, token);
    public virtual async Task<long> Count( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT COUNT({nameof(IUniqueID<TID>.ID)}) FROM {TableName}";

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstOrDefaultAsync<long>(sql, default, transaction);
    }


    public virtual async Task<TRecord> First( CancellationToken token = default ) => await this.Call(First, token);
    public virtual async Task<TRecord> First( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {TableName} ORDER BY {nameof(IUniqueID<TID>.ID)} ASC LIMIT 1";

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstAsync<TRecord>(sql, default, transaction);
    }


    public virtual async Task<TRecord?> FirstOrDefault( CancellationToken token = default ) => await this.Call(FirstOrDefault, token);
    public virtual async Task<TRecord?> FirstOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {TableName} ORDER BY {nameof(IUniqueID<TID>.ID)} ASC LIMIT 1";

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstOrDefaultAsync<TRecord>(sql, default, transaction);
    }


    public virtual async Task<TRecord> Last( CancellationToken token = default ) => await this.Call(Last, token);
    public virtual async Task<TRecord> Last( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {TableName} ORDER BY {nameof(IUniqueID<TID>.ID)} DESC";

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstAsync<TRecord>(sql, default, transaction);
    }


    public virtual async Task<TRecord?> LastOrDefault( CancellationToken token = default ) => await this.Call(LastOrDefault, token);
    public virtual async Task<TRecord?> LastOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        token.ThrowIfCancellationRequested();
        string sql = $"SELECT * FROM {TableName} ORDER BY {nameof(IUniqueID<TID>.ID)} DESC";

        return await connection.QueryFirstOrDefaultAsync<TRecord>(sql, default, transaction);
    }


    public virtual async Task<TRecord?> Single( TID id, CancellationToken token = default ) => await this.Call(Single, id, token);
    public virtual async Task<TRecord?> Single( DbConnection connection, DbTransaction? transaction, TID id, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add(nameof(IUniqueID<TID>.ID), id);

        string sql = $"SELECT * FROM {TableName} where {nameof(IUniqueID<TID>.ID)} = @{nameof(IUniqueID<TID>.ID)}";

        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sql, parameters, transaction);
        return items.Single();
    }


    public virtual async Task<TRecord?> Single( string sql, DynamicParameters? parameters, CancellationToken token = default ) => await this.Call(Single, sql, parameters, token);
    public virtual async Task<TRecord?> Single( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sql, parameters, transaction);
        return items.Single();
    }


    public virtual async Task<TRecord?> SingleOrDefault( TID id, CancellationToken token = default ) => await this.Call(SingleOrDefault, id, token);
    public virtual async Task<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, TID id, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add(nameof(IUniqueID<TID>.ID), id);

        string sql = $"SELECT * FROM {TableName} where {nameof(IUniqueID<TID>.ID)} = @{nameof(IUniqueID<TID>.ID)}";

        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sql, parameters, transaction);
        return items.SingleOrDefault();
    }


    public virtual async Task<TRecord?> SingleOrDefault( string sql, DynamicParameters? parameters, CancellationToken token = default ) => await this.Call(SingleOrDefault, sql, parameters, token);
    public virtual async Task<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sql, parameters, transaction);
        return items.SingleOrDefault();
    }


    public virtual async Task<List<TRecord>> All( CancellationToken token = default ) => await this.Call(All, token);
    public virtual async Task<List<TRecord>> All( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {TableName}";

        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sql, default, transaction);
        return items.ToList();
    }


    public virtual async Task<TResult> Call<TResult>( string sql, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, Task<TResult>> func, CancellationToken token = default ) => await this.Call(Call, sql, parameters, func, token);
    public virtual async Task<TResult> Call<TResult>( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, Task<TResult>> func, CancellationToken token = default )
    {
        token.ThrowIfCancellationRequested();
        using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(sql, parameters, transaction);
        return await func(reader, token);
    }


    public virtual async Task<TResult> Call<TResult>( string sql, DynamicParameters? parameters, Func<DbDataReader, CancellationToken, Task<TResult>> func, CancellationToken token = default ) => await this.Call(Call, sql, parameters, func, token);
    public virtual async Task<TResult> Call<TResult>( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, Func<DbDataReader, CancellationToken, Task<TResult>> func, CancellationToken token = default )
    {
        token.ThrowIfCancellationRequested();
        await using DbDataReader reader = await connection.ExecuteReaderAsync(sql, parameters, transaction);
        return await func(reader, token);
    }


    public virtual async IAsyncEnumerable<TRecord> Where( DynamicParameters parameters, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in this.Call(Where, parameters, token) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction? transaction, DynamicParameters parameters, [EnumeratorCancellation] CancellationToken token = default )
    {
        var sql = new StringBuilder($"SELECT * FROM {TableName} WHERE ").AppendJoin(',', parameters.ParameterNames.Select(x => $" @{x}"))
                                                                        .ToString();

        IEnumerable<TRecord> records = await connection.QueryAsync<TRecord>(sql, parameters, transaction);

        foreach ( TRecord record in records ) { yield return record; }
    }


    public virtual async IAsyncEnumerable<TRecord> Where( string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in this.Call(Where, sql, parameters, token) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
    {
        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sql, parameters, transaction);

        foreach ( TRecord record in items ) { yield return record; }
    }


    public virtual async IAsyncEnumerable<TRecord> Where<TValue>( string columnName, TValue? value, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in this.Call(Where, columnName, value, token) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TRecord> Where<TValue>( DbConnection connection, DbTransaction? transaction, string columnName, TValue? value, [EnumeratorCancellation] CancellationToken token = default )
    {
        string sql        = $"SELECT * FROM {TableName} WHERE {columnName} = @{nameof(value)}";
        var    parameters = new DynamicParameters();
        parameters.Add(nameof(value), value);

        IEnumerable<TRecord> records = await connection.QueryAsync<TRecord>(sql, parameters, transaction);

        foreach ( TRecord record in records ) { yield return record; }
    }


    public virtual async Task<TID> GetID<TValue>( string sql, DynamicParameters? parameters, CancellationToken token = default ) => await this.Call(GetID, sql, parameters, token);
    public virtual async Task<TID> GetID<TValue>( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        token.ThrowIfCancellationRequested();
        return await connection.QuerySingleAsync<TID>(sql, parameters, transaction);
    }


    public virtual async Task<TID> GetID<TValue>( string columnName, TValue? value, CancellationToken token = default ) => await this.Call(GetID, columnName, value, token);
    public virtual async Task<TID> GetID<TValue>( DbConnection connection, DbTransaction? transaction, string columnName, TValue? value, CancellationToken token = default )
    {
        string sql        = $"SELECT {nameof(IUniqueID<TID>.ID)} FROM {TableName} WHERE {columnName} = @{nameof(value)}";
        var    parameters = new DynamicParameters();
        parameters.Add(nameof(value), value);

        return await connection.QuerySingleAsync<TID>(sql, parameters, transaction);
    }


    public virtual async Task<TRecord> Get( TID id, CancellationToken token = default ) => await this.Call(Get, id, token);
    public virtual async IAsyncEnumerable<TRecord> Get( IEnumerable<TID> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in this.Call(Get, ids, token) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TRecord> Get( IAsyncEnumerable<TID> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in this.Call(Get, ids, token) ) { yield return record; }
    }
    public virtual async Task<TRecord> Get( DbConnection connection, DbTransaction? transaction, TID id, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add(nameof(IUniqueID<TID>.ID), id);

        string sql = $"SELECT * FROM {TableName} where {nameof(IUniqueID<TID>.ID)} = @{nameof(IUniqueID<TID>.ID)}";

        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sql, parameters, transaction);
        return items.SingleOrDefault() ?? throw new RecordNotFoundException();
    }
    public virtual async IAsyncEnumerable<TRecord> Get( DbConnection connection, DbTransaction? transaction, IEnumerable<TID> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        var sb = new StringBuilder();

        if ( typeof(TID) == typeof(string) ) { sb.AppendJoin(',', ids.Select(x => $"'{x}'")); }
        else { sb.AppendJoin(',',                                 ids); }

        string sql = $"SELECT * FROM {TableName} where {nameof(IUniqueID<TID>.ID)} in {sb}";

        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sql, default, transaction);

        foreach ( TRecord record in items ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TRecord> Get( DbConnection connection, DbTransaction? transaction, IAsyncEnumerable<TID> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        var values = new List<TID>();
        await foreach ( TID id in ids.WithCancellation(token) ) { values.Add(id); }

        await foreach ( TRecord record in Get(connection, transaction, values, token) ) { yield return record; }
    }


    public virtual async Task<TRecord> Insert( TRecord record, CancellationToken token = default ) => await this.Call(Insert, record, token);
    public virtual async IAsyncEnumerable<TRecord> Insert( IEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in this.Call(Insert, records, token) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TRecord> Insert( IAsyncEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in this.Call(Insert, records, token) ) { yield return record; }
    }
    public virtual async Task<TRecord> Insert( DbConnection connection, DbTransaction transaction, TRecord record, CancellationToken token = default )
    {
        List<Descriptor> descriptors = TypePropertiesCache.Current[typeof(TRecord)]
                                                          .Where(x => !x.IsKey)
                                                          .ToList();


        var sbColumnList = new StringBuilder();
        sbColumnList.AppendJoin(',', descriptors.Select(x => x.ColumnName));


        var sbParameterList = new StringBuilder();
        sbParameterList.AppendJoin(',', descriptors.Select(x => x.VariableName));


        string cmd        = $"INSERT INTO {TableName} ({sbColumnList}) values ({sbParameterList});";
        var    parameters = new DynamicParameters(record);


        token.ThrowIfCancellationRequested();
        var id = await connection.ExecuteScalarAsync<TID>(cmd, parameters, transaction);
        return record.NewID(id);
    }
    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, IEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { yield return await Insert(connection, transaction, record, token); }
    }
    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in records.WithCancellation(token) ) { yield return await Insert(connection, transaction, record, token); }
    }


    public virtual async Task Update( TRecord                   record,  CancellationToken token = default ) => await this.Call(Update, record,  token);
    public virtual async Task Update( IEnumerable<TRecord>      records, CancellationToken token = default ) => await this.Call(Update, records, token);
    public virtual async Task Update( IAsyncEnumerable<TRecord> records, CancellationToken token = default ) => await this.Call(Update, records, token);
    public virtual async Task Update( DbConnection connection, DbTransaction transaction, TRecord record, CancellationToken token = default )
    {
        var sql = new StringBuilder();

        sql.AppendJoin(',',
                       TypePropertiesCache.Current[typeof(TRecord)]
                                          .Where(x => !x.IsKey)
                                          .Select(x => x.UpdateName));

        string cmd        = $"UPDATE {TableName} SET {sql} where {nameof(IUniqueID<TID>.ID)} = @{nameof(IUniqueID<TID>.ID)};";
        var    parameters = new DynamicParameters(record);

        token.ThrowIfCancellationRequested();
        await connection.ExecuteScalarAsync(cmd, parameters, transaction);
    }
    public virtual async Task Update( DbConnection connection, DbTransaction transaction, IEnumerable<TRecord> records, CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { await Update(connection, transaction, record, token); }
    }
    public virtual async Task Update( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TRecord> records, CancellationToken token = default )
    {
        await foreach ( TRecord record in records.WithCancellation(token) ) { await Update(connection, transaction, record, token); }
    }
}
