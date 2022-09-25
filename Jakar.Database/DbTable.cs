namespace Jakar.Database;


public abstract class DbTable<TRecord, TID> : ObservableClass, IDbTable<TRecord, TID> where TRecord : BaseTableRecord<TRecord, TID>
                                                                                      where TID : struct, IComparable<TID>, IEquatable<TID>
{
    // ReSharper disable once StaticMemberInGenericType
    private static string? _defaultID;

    public static string DefaultID => _defaultID ??= typeof(TID) == typeof(Guid)
                                                         ? $"'{Guid.Empty}'"
                                                         : default(TID).ToString() ?? throw new InvalidOperationException();


    // ReSharper disable once InconsistentNaming
    public        IDGenerator       IDs       => new(this);
    public        RecordGenerator   Records   => new(this);
    public static string            TableName => BaseTableRecord<TRecord, TID>.TableName;
    string IDbTable<TRecord, TID>.  TableName => TableName;
    private readonly IConnectableDb _database;


    protected DbTable( IConnectableDb database ) => _database = database;
    public virtual ValueTask DisposeAsync() => default;


    public DbConnection Connect() => _database.Connect();
    public ValueTask<DbConnection> ConnectAsync( CancellationToken token = default ) => _database.ConnectAsync(token);


    public virtual async ValueTask<string> ServerVersion( CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync(token);
        return connection.ServerVersion;
    }


    public virtual async ValueTask<DataTable> Schema( DbConnection connection, CancellationToken token = default ) => await connection.GetSchemaAsync(token);
    public virtual async ValueTask<DataTable> Schema( DbConnection connection, string collectionName, CancellationToken token = default ) => await connection.GetSchemaAsync(collectionName, token);
    public virtual async ValueTask<DataTable> Schema( DbConnection connection, string collectionName, string?[] restrictionValues, CancellationToken token = default ) => await connection.GetSchemaAsync(collectionName, restrictionValues, token);


    public virtual async ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, token);
        await func(schema, token);
    }
    public virtual async ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, string collectionName, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, collectionName, token);
        await func(schema, token);
    }
    public virtual async ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, string collectionName, string?[] restrictionValues, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, collectionName, restrictionValues, token);
        await func(schema, token);
    }


    public virtual async ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, token);
        return await func(schema, token);
    }
    public virtual async ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, string collectionName, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, collectionName, token);
        return await func(schema, token);
    }
    public virtual async ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, string collectionName, string?[] restrictionValues, CancellationToken token = default )
    {
        await using DbConnection connection = await ConnectAsync(token);
        using DataTable          schema     = await Schema(connection, collectionName, restrictionValues, token);
        return await func(schema, token);
    }


    public virtual async ValueTask<long> Count( CancellationToken token = default ) => await this.Call(Count, token);
    public virtual async ValueTask<long> Count( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        var sql = $"SELECT COUNT({nameof(IUniqueID<TID>.ID)}) FROM {TableName}";

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstOrDefaultAsync<long>(sql, default, transaction);
    }


    public virtual async ValueTask<TRecord> Random( CancellationToken token = default ) => await this.Call(Random, token);
    public virtual async ValueTask<TRecord> Random( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {TableName} WHERE {nameof(IUniqueID<TID>.ID)} >= RAND() * ( SELECT MAX ({nameof(IUniqueID<TID>.ID)}) FROM table ) ORDER BY {nameof(IUniqueID<TID>.ID)} LIMIT 1";

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstAsync<TRecord>(sql, default, transaction);
    }


    public virtual IAsyncEnumerable<TRecord> Random( long count, CancellationToken token = default ) => this.Call(Random, count, token);
    public virtual async IAsyncEnumerable<TRecord> Random( DbConnection connection, DbTransaction? transaction, long count, [EnumeratorCancellation] CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {TableName} WHERE {nameof(IUniqueID<TID>.ID)} >= RAND() * ( SELECT MAX ({nameof(IUniqueID<TID>.ID)}) FROM table ) ORDER BY {nameof(IUniqueID<TID>.ID)} LIMIT {count}";

        token.ThrowIfCancellationRequested();
        using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(sql, default, transaction);
        IEnumerable<TRecord>       items  = await reader.ReadAsync<TRecord>(false);

        foreach ( TRecord record in items )
        {
            if ( token.IsCancellationRequested ) { yield break; }

            yield return record;
        }
    }


    public virtual async ValueTask<TRecord> First( CancellationToken token = default ) => await this.Call(First, token);
    public virtual async ValueTask<TRecord> First( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {TableName} ORDER BY {nameof(IUniqueID<TID>.ID)} ASC LIMIT 1";

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstAsync<TRecord>(sql, default, transaction);
    }


    public virtual async ValueTask<TRecord?> FirstOrDefault( CancellationToken token = default ) => await this.Call(FirstOrDefault, token);
    public virtual async ValueTask<TRecord?> FirstOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {TableName} ORDER BY {nameof(IUniqueID<TID>.ID)} ASC LIMIT 1";

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstOrDefaultAsync<TRecord>(sql, default, transaction);
    }


    public virtual async ValueTask<TRecord> Last( CancellationToken token = default ) => await this.Call(Last, token);
    public virtual async ValueTask<TRecord> Last( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {TableName} ORDER BY {nameof(IUniqueID<TID>.ID)} DESC LIMIT 1";

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstAsync<TRecord>(sql, default, transaction);
    }


    public virtual async ValueTask<TRecord?> LastOrDefault( CancellationToken token = default ) => await this.Call(LastOrDefault, token);
    public virtual async ValueTask<TRecord?> LastOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        token.ThrowIfCancellationRequested();
        string sql = $"SELECT * FROM {TableName} ORDER BY {nameof(IUniqueID<TID>.ID)} DESC LIMIT 1";

        return await connection.QueryFirstOrDefaultAsync<TRecord>(sql, default, transaction);
    }


    public virtual async ValueTask<TRecord?> Single( TID id, CancellationToken token = default ) => await this.Call(Single, id, token);
    public virtual async ValueTask<TRecord?> Single( DbConnection connection, DbTransaction? transaction, TID id, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add(nameof(IUniqueID<TID>.ID), id);

        string sql = $"SELECT * FROM {TableName} where {nameof(IUniqueID<TID>.ID)} = @{nameof(IUniqueID<TID>.ID)}";

        token.ThrowIfCancellationRequested();
        return await connection.QuerySingleAsync<TRecord>(sql, parameters, transaction);
    }


    public virtual async ValueTask<TRecord?> Single( string sql, DynamicParameters? parameters, CancellationToken token = default ) => await this.Call(Single, sql, parameters, token);
    public virtual async ValueTask<TRecord?> Single( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        token.ThrowIfCancellationRequested();
        return await connection.QuerySingleAsync<TRecord>(sql, parameters, transaction);
    }


    public virtual async ValueTask<TRecord?> SingleOrDefault( TID id, CancellationToken token = default ) => await this.Call(SingleOrDefault, id, token);
    public virtual async ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, TID id, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add(nameof(IUniqueID<TID>.ID), id);

        string sql = $"SELECT * FROM {TableName} where {nameof(IUniqueID<TID>.ID)} = @{nameof(IUniqueID<TID>.ID)}";

        token.ThrowIfCancellationRequested();
        return await connection.QuerySingleOrDefaultAsync<TRecord>(sql, parameters, transaction);
    }


    public virtual async ValueTask<TRecord?> SingleOrDefault( string sql, DynamicParameters? parameters, CancellationToken token = default ) => await this.Call(SingleOrDefault, sql, parameters, token);
    public virtual async ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        token.ThrowIfCancellationRequested();
        return await connection.QuerySingleOrDefaultAsync<TRecord>(sql, parameters, transaction);
    }


    public virtual async ValueTask<List<TRecord>> All( CancellationToken token = default ) => await this.Call(All, token);
    public virtual async ValueTask<List<TRecord>> All( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        var sql = $"SELECT * FROM {TableName}";

        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord> items = await connection.QueryAsync<TRecord>(sql, default, transaction);
        if ( items is List<TRecord> list ) { return list; }

        return items.ToList();
    }


    public virtual async ValueTask<TResult> Call<TResult>( string sql, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default ) =>
        await this.Call(Call, sql, parameters, func, token);
    public virtual async ValueTask<TResult> Call<TResult>( DbConnection                                                      connection,
                                                           DbTransaction?                                                    transaction,
                                                           string                                                            sql,
                                                           DynamicParameters?                                                parameters,
                                                           Func<SqlMapper.GridReader, CancellationToken, ValueTask<TResult>> func,
                                                           CancellationToken                                                 token = default
    )
    {
        token.ThrowIfCancellationRequested();
        using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(sql, parameters, transaction);
        return await func(reader, token);
    }


    public virtual async ValueTask<TResult> Call<TResult>( string sql, DynamicParameters? parameters, Func<DbDataReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default ) =>
        await this.Call(Call, sql, parameters, func, token);
    public virtual async ValueTask<TResult> Call<TResult>( DbConnection                                              connection,
                                                           DbTransaction?                                            transaction,
                                                           string                                                    sql,
                                                           DynamicParameters?                                        parameters,
                                                           Func<DbDataReader, CancellationToken, ValueTask<TResult>> func,
                                                           CancellationToken                                         token = default
    )
    {
        token.ThrowIfCancellationRequested();
        await using DbDataReader reader = await connection.ExecuteReaderAsync(sql, parameters, transaction);
        return await func(reader, token);
    }


    public virtual async IAsyncEnumerable<TRecord> Where( bool matchAll, DynamicParameters parameters, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in this.Call(Where, matchAll, parameters, token) ) { yield return record; }
    }
    public virtual IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        var sql = new StringBuilder($"SELECT * FROM {TableName} WHERE ").AppendJoin(matchAll
                                                                                        ? "AND"
                                                                                        : "OR",
                                                                                    parameters.ParameterNames.Select(x => $"{x} = @{x}"))
                                                                        .ToString();

        return Where(connection, transaction, sql, parameters, token);
    }


    public virtual async IAsyncEnumerable<TRecord> Where( string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in this.Call(Where, sql, parameters, token) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
    {
        token.ThrowIfCancellationRequested();
        using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(sql, parameters, transaction);
        IEnumerable<TRecord>       items  = await reader.ReadAsync<TRecord>(false);

        foreach ( TRecord record in items )
        {
            if ( token.IsCancellationRequested ) { yield break; }

            yield return record;
        }
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

        using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(sql, parameters, transaction);
        IEnumerable<TRecord>       items  = await reader.ReadAsync<TRecord>(false);

        foreach ( TRecord record in items )
        {
            if ( token.IsCancellationRequested ) { yield break; }

            yield return record;
        }
    }


    public virtual async ValueTask<TID> GetID<TValue>( string sql, DynamicParameters? parameters, CancellationToken token = default ) => await this.Call(GetID, sql, parameters, token);
    public virtual async ValueTask<TID> GetID<TValue>( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        token.ThrowIfCancellationRequested();
        return await connection.QuerySingleAsync<TID>(sql, parameters, transaction);
    }


    public virtual async ValueTask<TID> GetID<TValue>( string columnName, TValue value, CancellationToken token = default ) => await this.Call(GetID, columnName, value, token);
    public virtual async ValueTask<TID> GetID<TValue>( DbConnection connection, DbTransaction? transaction, string columnName, TValue value, CancellationToken token = default )
    {
        string sql        = $"SELECT {nameof(IUniqueID<TID>.ID)} FROM {TableName} WHERE {columnName} = @{nameof(value)}";
        var    parameters = new DynamicParameters();
        parameters.Add(nameof(value), value);

        return await connection.QuerySingleAsync<TID>(sql, parameters, transaction);
    }


    public async ValueTask<TRecord?> Get<TValue>( string columnName, TValue? value, CancellationToken token = default ) => await this.Call(Get, columnName, value, token);
    public async ValueTask<TRecord?> Get<TValue>( DbConnection connection, DbTransaction? transaction, string columnName, TValue? value, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add(nameof(value), value);

        string sql = $"SELECT * FROM {TableName} where {columnName} = @{nameof(value)}";

        token.ThrowIfCancellationRequested();
        IEnumerable<TRecord?> items = await connection.QueryAsync<TRecord>(sql, parameters, transaction);
        return items.SingleOrDefault();
    }


    public virtual async ValueTask<TRecord?> Get( TID id, CancellationToken token = default ) => await this.Call(Get, id, token);
    public virtual async IAsyncEnumerable<TRecord?> Get( IEnumerable<TID> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord? record in this.Call(Get, ids, token) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TRecord?> Get( IAsyncEnumerable<TID> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord? record in this.Call(Get, ids, token) ) { yield return record; }
    }
    public virtual async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, TID id, CancellationToken token = default ) => await Get(connection, transaction, nameof(IUniqueID<TID>.ID), id, token);
    public virtual async IAsyncEnumerable<TRecord?> Get( DbConnection connection, DbTransaction? transaction, IEnumerable<TID> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        var sb = new StringBuilder();

        if ( typeof(TID) == typeof(string) ) { sb.AppendJoin(',', ids.Select(x => $"'{x}'")); }
        else { sb.AppendJoin(',',                                 ids); }

        string sql = $"SELECT * FROM {TableName} where {nameof(IUniqueID<TID>.ID)} in {sb}";

        token.ThrowIfCancellationRequested();
        using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(sql, default, transaction);
        IEnumerable<TRecord>       items  = await reader.ReadAsync<TRecord>(false);

        foreach ( TRecord record in items )
        {
            if ( token.IsCancellationRequested ) { yield break; }

            yield return record;
        }
    }
    public virtual async IAsyncEnumerable<TRecord?> Get( DbConnection connection, DbTransaction? transaction, IAsyncEnumerable<TID> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        var values = new List<TID>();
        await foreach ( TID id in ids.WithCancellation(token) ) { values.Add(id); }

        await foreach ( TRecord? record in Get(connection, transaction, values, token) ) { yield return record; }
    }


    public virtual async ValueTask<TRecord> Insert( TRecord record, CancellationToken token = default ) => await this.Call(Insert, record, token);
    public virtual async IAsyncEnumerable<TRecord> Insert( IEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in this.Call(Insert, records, token) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TRecord> Insert( IAsyncEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in this.Call(Insert, records, token) ) { yield return record; }
    }
    public virtual async ValueTask<TRecord> Insert( DbConnection connection, DbTransaction transaction, TRecord record, CancellationToken token = default )
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


    public virtual async ValueTask Update( TRecord                   record,  CancellationToken token = default ) => await this.Call(Update, record,  token);
    public virtual async ValueTask Update( IEnumerable<TRecord>      records, CancellationToken token = default ) => await this.Call(Update, records, token);
    public virtual async ValueTask Update( IAsyncEnumerable<TRecord> records, CancellationToken token = default ) => await this.Call(Update, records, token);
    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, TRecord record, CancellationToken token = default )
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
    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, IEnumerable<TRecord> records, CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { await Update(connection, transaction, record, token); }
    }
    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, IAsyncEnumerable<TRecord> records, CancellationToken token = default )
    {
        await foreach ( TRecord record in records.WithCancellation(token) ) { await Update(connection, transaction, record, token); }
    }



    /// <summary> <see href="https://stackoverflow.com/a/15992856/9530917"/> </summary>
    public struct RecordGenerator : IAsyncEnumerator<TRecord?>
    {
        private readonly DbTable<TRecord, TID> _table;
        private          TRecord?              _current = default;
        private          TID                   _id      = default!;


        public TRecord Current
        {
            get => _current ?? throw new NullReferenceException(nameof(_current));
            set
            {
                _current = value;
                _id      = value.ID;
            }
        }


        public RecordGenerator( DbTable<TRecord, TID> table ) => _table = table;
        public ValueTask DisposeAsync()
        {
            _id      = default;
            _current = default;
            return ValueTask.CompletedTask;
        }


        private DynamicParameters GetParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(IUniqueID<TID>.ID), _id);
            return parameters;
        }
        public void Reset()
        {
            _id      = default;
            _current = default;
        }
        public async ValueTask<bool> MoveNextAsync( CancellationToken token = default )
        {
            await using DbConnection connection = await _table.ConnectAsync(token);
            return await MoveNextAsync(connection, default, token);
        }
        public async ValueTask<bool> MoveNextAsync( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
        {
            _current = default!;
            token.ThrowIfCancellationRequested();

            var record =
                await
                    connection.ExecuteScalarAsync<TRecord>(@$"select * from {TableName} where ( id = IFNULL((select min({nameof(IUniqueID<TID>.ID)}) from {TableName} where {nameof(IUniqueID<TID>.ID)} > @{nameof(IUniqueID<TID>.ID)}), {DefaultID}) )",
                                                           GetParameters(),
                                                           transaction);

            if ( record is null ) { return false; }

            Current = record;
            return true;
        }
        ValueTask<bool> IAsyncEnumerator<TRecord?>.MoveNextAsync() => MoveNextAsync();
    }



    /// <summary> <see href="https://stackoverflow.com/a/15992856/9530917"/> </summary>
    public struct IDGenerator : IAsyncEnumerator<TID>
    {
        private readonly DbTable<TRecord, TID> _table;
        public           TID                   Current { get; set; } = default;


        public IDGenerator( DbTable<TRecord, TID> table ) => _table = table;
        public ValueTask DisposeAsync()
        {
            Current = default;
            return ValueTask.CompletedTask;
        }


        private DynamicParameters GetParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(IUniqueID<TID>.ID), Current);
            return parameters;
        }
        public void Reset() => Current = default;
        public async ValueTask<bool> MoveNextAsync( CancellationToken token = default )
        {
            await using DbConnection connection = await _table.ConnectAsync(token);
            return await MoveNextAsync(connection, default, token);
        }
        public async ValueTask<bool> MoveNextAsync( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
        {
            Current = default;
            token.ThrowIfCancellationRequested();

            Current =
                await
                    connection.ExecuteScalarAsync<TID>(@$"select {nameof(IUniqueID<TID>.ID)} from {TableName} where ( id = IFNULL((select min({nameof(IUniqueID<TID>.ID)}) from {TableName} where {nameof(IUniqueID<TID>.ID)} > @{nameof(IUniqueID<TID>.ID)}), {DefaultID}) )",
                                                       GetParameters(),
                                                       transaction);

            return !Current.Equals(default);
        }
        ValueTask<bool> IAsyncEnumerator<TID>.MoveNextAsync() => MoveNextAsync();
    }
}
