namespace Jakar.Database;


public class DbTable<TRecord, TID> : ObservableClass, IDbTable<TRecord, TID> where TRecord : BaseTableRecord<TRecord, TID>
                                                                             where TID : IComparable<TID>, IEquatable<TID>
{
    private readonly IConnectableDb _database;
    public virtual   string         TableName { get; } = DbExtensions.GetTableName<TRecord>();


    public DbTable( IConnectableDb database ) => _database = database;
    public virtual ValueTask DisposeAsync() => default;


    public DbConnection Connect() => _database.Connect();
    public Task<DbConnection> ConnectAsync( CancellationToken token ) => _database.ConnectAsync(token);


    public async Task<string> ServerVersion( CancellationToken token )
    {
        await using DbConnection connection = await ConnectAsync(token);
        return connection.ServerVersion;
    }


    public async Task<DataTable> Schema( DbConnection connection, CancellationToken token ) => await connection.GetSchemaAsync(token);
    public async Task<DataTable> Schema( DbConnection connection, string            collectionName, CancellationToken token ) => await connection.GetSchemaAsync(collectionName,                                      token);
    public async Task<DataTable> Schema( DbConnection connection, string            collectionName, string?[]         restrictionValues, CancellationToken token ) => await connection.GetSchemaAsync(collectionName, restrictionValues, token);


    public async Task Schema( Func<DataTable, CancellationToken, Task> func, CancellationToken token )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, token);
        await func(schema, token);
    }
    public async Task Schema( Func<DataTable, CancellationToken, Task> func, string collectionName, CancellationToken token )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, collectionName, token);
        await func(schema, token);
    }
    public async Task Schema( Func<DataTable, CancellationToken, Task> func, string collectionName, string?[] restrictionValues, CancellationToken token )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, collectionName, restrictionValues, token);
        await func(schema, token);
    }


    public async Task<TResult> Schema<TResult>( Func<DataTable, CancellationToken, Task<TResult>> func, CancellationToken token )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, token);
        return await func(schema, token);
    }
    public async Task<TResult> Schema<TResult>( Func<DataTable, CancellationToken, Task<TResult>> func, string collectionName, CancellationToken token )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, collectionName, token);
        return await func(schema, token);
    }
    public async Task<TResult> Schema<TResult>( Func<DataTable, CancellationToken, Task<TResult>> func, string collectionName, string?[] restrictionValues, CancellationToken token )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, collectionName, restrictionValues, token);
        return await func(schema, token);
    }


    public async Task<long> Count( CancellationToken token ) => await this.Call(Count, token);
    public async Task<long> Count( DbConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        string sql = $"SELECT COUNT({nameof(IUniqueID<TID>.ID)}) FROM {TableName}";

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstOrDefaultAsync<long>(sql, default, transaction);
    }


    public async Task<TRecord> First( CancellationToken token ) => await this.Call(First, token);
    public async Task<TRecord> First( DbConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        string sql = $"SELECT * FROM {TableName} ORDER BY {nameof(IUniqueID<TID>.ID)} ASC LIMIT 1";

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstAsync<TRecord>(sql, default, transaction);
    }


    public async Task<TRecord?> FirstOrDefault( CancellationToken token ) => await this.Call(FirstOrDefault, token);
    public async Task<TRecord?> FirstOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        string sql = $"SELECT * FROM {TableName} ORDER BY {nameof(IUniqueID<TID>.ID)} ASC LIMIT 1";

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstOrDefaultAsync<TRecord>(sql, default, transaction);
    }


    public async Task<TRecord> Last( CancellationToken token ) => await this.Call(Last, token);
    public async Task<TRecord> Last( DbConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        string sql = $"SELECT * FROM {TableName} ORDER BY {nameof(IUniqueID<TID>.ID)} DESC";

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstAsync<TRecord>(sql, default, transaction);
    }


    public async Task<TRecord?> LastOrDefault( CancellationToken token ) => await this.Call(LastOrDefault, token);
    public async Task<TRecord?> LastOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        token.ThrowIfCancellationRequested();
        string sql = $"SELECT * FROM {TableName} ORDER BY {nameof(IUniqueID<TID>.ID)} DESC";

        return await connection.QueryFirstOrDefaultAsync<TRecord>(sql, default, transaction);
    }


    public async Task<TRecord?> Single( TID id, CancellationToken token ) => await this.Call(Single, id, token);
    public async Task<TRecord?> Single( DbConnection connection, DbTransaction? transaction, TID id, CancellationToken token )
    {
        var parameters = new DynamicParameters();
        parameters.Add(nameof(IUniqueID<TID>.ID), id);

        string sql = $"SELECT * FROM {TableName} where {nameof(IUniqueID<TID>.ID)} = @{nameof(IUniqueID<TID>.ID)}";

        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sql, parameters, transaction);
        return items.Single();
    }


    public async Task<TRecord?> Single( StringBuilder sb, DynamicParameters? parameters, CancellationToken token ) => await this.Call(Single, sb, parameters, token);
    public async Task<TRecord?> Single( DbConnection connection, DbTransaction? transaction, StringBuilder sb, DynamicParameters? parameters, CancellationToken token )
    {
        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sb.ToString(), parameters, transaction);
        return items.Single();
    }


    public async Task<TRecord?> SingleOrDefault( TID id, CancellationToken token ) => await this.Call(SingleOrDefault, id, token);
    public async Task<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, TID id, CancellationToken token )
    {
        var parameters = new DynamicParameters();
        parameters.Add(nameof(IUniqueID<TID>.ID), id);

        string sql = $"SELECT * FROM {TableName} where {nameof(IUniqueID<TID>.ID)} = @{nameof(IUniqueID<TID>.ID)}";

        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sql, parameters, transaction);
        return items.SingleOrDefault();
    }


    public async Task<TRecord?> SingleOrDefault( StringBuilder sb, DynamicParameters? parameters, CancellationToken token ) => await this.Call(SingleOrDefault, sb, parameters, token);
    public async Task<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, StringBuilder sb, DynamicParameters? parameters, CancellationToken token )
    {
        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sb.ToString(), parameters, transaction);
        return items.SingleOrDefault();
    }


    public async Task<List<TRecord>> All( CancellationToken token ) => await this.Call(All, token);
    public async Task<List<TRecord>> All( DbConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        string sql = $"SELECT * FROM {TableName}";

        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sql, default, transaction);
        return items.ToList();
    }


    public async Task<TResult> Call<TResult>( StringBuilder sb, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, Task<TResult>> func, CancellationToken token ) => await this.Call(Call, sb, parameters, func, token);
    public async Task<TResult> Call<TResult>( DbConnection connection, DbTransaction? transaction, StringBuilder sb, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, Task<TResult>> func, CancellationToken token )
    {
        token.ThrowIfCancellationRequested();
        using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(sb.ToString(), parameters, transaction);
        return await func(reader, token);
    }


    public async Task<TResult> Call<TResult>( StringBuilder sb, DynamicParameters? parameters, Func<DbDataReader, CancellationToken, Task<TResult>> func, CancellationToken token ) => await this.Call(Call, sb, parameters, func, token);
    public async Task<TResult> Call<TResult>( DbConnection connection, DbTransaction? transaction, StringBuilder sb, DynamicParameters? parameters, Func<DbDataReader, CancellationToken, Task<TResult>> func, CancellationToken token )
    {
        token.ThrowIfCancellationRequested();
        await using DbDataReader reader = await connection.ExecuteReaderAsync(sb.ToString(), parameters, transaction);
        return await func(reader, token);
    }


    public async IAsyncEnumerable<TRecord> Where( StringBuilder sb, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token )
    {
        await foreach ( TRecord record in this.Call(Where, sb, parameters, token) ) { yield return record; }
    }
    public async IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction? transaction, StringBuilder sb, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token )
    {
        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sb.ToString(), parameters, transaction);

        foreach ( TRecord record in items ) { yield return record; }
    }


    public async Task<TRecord> Get( TID id, CancellationToken token ) => await this.Call(Get, id, token);
    public async IAsyncEnumerable<TRecord> Get( IEnumerable<TID> ids, [EnumeratorCancellation] CancellationToken token )
    {
        await foreach ( TRecord record in this.Call(Get, ids, token) ) { yield return record; }
    }
    public async IAsyncEnumerable<TRecord> Get( IAsyncEnumerable<TID> ids, [EnumeratorCancellation] CancellationToken token )
    {
        await foreach ( TRecord record in this.Call(Get, ids, token) ) { yield return record; }
    }
    public async Task<TRecord> Get( DbConnection connection, DbTransaction? transaction, TID id, CancellationToken token )
    {
        var parameters = new DynamicParameters();
        parameters.Add(nameof(IUniqueID<TID>.ID), id);

        string sql = $"SELECT * FROM {TableName} where {nameof(IUniqueID<TID>.ID)} = @{nameof(IUniqueID<TID>.ID)}";

        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sql, parameters, transaction);
        return items.SingleOrDefault() ?? throw new RecordNotFoundException();
    }
    public async IAsyncEnumerable<TRecord> Get( DbConnection connection, DbTransaction? transaction, IEnumerable<TID> ids, [EnumeratorCancellation] CancellationToken token )
    {
        var sb = new StringBuilder();

        if ( typeof(TID) == typeof(string) ) { sb.AppendJoin(',', ids.Select(x => $"'{x}'")); }
        else { sb.AppendJoin(',',                                 ids); }

        string sql = $"SELECT * FROM {TableName} where {nameof(IUniqueID<TID>.ID)} in {sb}";

        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sql, default, transaction);

        foreach ( TRecord record in items ) { yield return record; }
    }
    public async IAsyncEnumerable<TRecord> Get( DbConnection connection, DbTransaction? transaction, IAsyncEnumerable<TID> ids, [EnumeratorCancellation] CancellationToken token )
    {
        var values = new List<TID>();
        await foreach ( TID id in ids.WithCancellation(token) ) { values.Add(id); }

        await foreach ( TRecord record in Get(connection, transaction, values, token) ) { yield return record; }
    }


    public async Task<TRecord> Insert( TRecord record, CancellationToken token ) => await this.Call(Insert, record, token);
    public async IAsyncEnumerable<TRecord> Insert( IEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token )
    {
        await foreach ( TRecord record in this.Call(Insert, records, token) ) { yield return record; }
    }
    public async IAsyncEnumerable<TRecord> Insert( IAsyncEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token )
    {
        await foreach ( TRecord record in this.Call(Insert, records, token) ) { yield return record; }
    }
    public async Task<TRecord> Insert( DbConnection connection, DbTransaction transaction, TRecord record, CancellationToken token )
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
    public async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, IEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token )
    {
        foreach ( TRecord record in records ) { yield return await Insert(connection, transaction, record, token); }
    }
    public async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token )
    {
        await foreach ( TRecord record in records.WithCancellation(token) ) { yield return await Insert(connection, transaction, record, token); }
    }


    public async Task Update( TRecord                   record,  CancellationToken token ) => await this.Call(Update, record,  token);
    public async Task Update( IEnumerable<TRecord>      records, CancellationToken token ) => await this.Call(Update, records, token);
    public async Task Update( IAsyncEnumerable<TRecord> records, CancellationToken token ) => await this.Call(Update, records, token);
    public async Task Update( DbConnection connection, DbTransaction transaction, TRecord record, CancellationToken token )
    {
        var sb = new StringBuilder();

        sb.AppendJoin(',',
                      TypePropertiesCache.Current[typeof(TRecord)]
                                         .Where(x => !x.IsKey)
                                         .Select(x => x.UpdateName));

        string cmd        = $"UPDATE {TableName} SET {sb} where {nameof(IUniqueID<TID>.ID)} = @{nameof(IUniqueID<TID>.ID)};";
        var    parameters = new DynamicParameters(record);

        token.ThrowIfCancellationRequested();
        await connection.ExecuteScalarAsync(cmd, parameters, transaction);
    }
    public async Task Update( DbConnection connection, DbTransaction transaction, IEnumerable<TRecord> records, CancellationToken token )
    {
        foreach ( TRecord record in records ) { await Update(connection, transaction, record, token); }
    }
    public async Task Update( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TRecord> records, CancellationToken token )
    {
        await foreach ( TRecord record in records.WithCancellation(token) ) { await Update(connection, transaction, record, token); }
    }
}
