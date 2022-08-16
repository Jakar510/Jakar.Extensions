using System.Data.SqlTypes;
using System.Reflection;
using Dapper.Contrib.Extensions;



namespace Jakar.Database;


public record DbTable<TRecord, TID> : BaseRecord, IDbTable<TRecord, TID> where TRecord : BaseTableRecord<TRecord, TID>
                                                                         where TID : IComparable<TID>, IEquatable<TID>
{
    private readonly Database<TID> _database;
    public virtual   string        TableName { get; } = Database<TID>.GetTableName<TRecord>();


    public DbTable( Database<TID> database ) => _database = database;
    public virtual ValueTask DisposeAsync() => default;


    public async Task<string> ServerVersion( CancellationToken token )
    {
        await using DbConnection connection = await ConnectAsync(token);
        return connection.ServerVersion;
    }
    public Task<DbConnection> ConnectAsync( CancellationToken token ) => _database.ConnectAsync(token);


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


    public async Task<long> Count( CancellationToken token ) => await _database.Call(Count, token);
    public async Task<long> Count( DbConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        var sb = new StringBuilder();
        sb.Append("Select ");
        sb.Append(nameof(IUniqueID<TID>.ID));

        var parameters = new DynamicParameters();

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstOrDefaultAsync<long>(sb.ToString(), default, transaction);
    }


    public async Task<TRecord> First( CancellationToken token ) => await _database.Call(First, token);
    public async Task<TRecord> First( DbConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        var sb         = new StringBuilder();
        var parameters = new DynamicParameters();

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstAsync<TRecord>(sb.ToString(), default, transaction);
    }


    public async Task<TRecord?> FirstOrDefault( CancellationToken token ) => await _database.Call(FirstOrDefault, token);
    public async Task<TRecord?> FirstOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        var sb         = new StringBuilder();
        var parameters = new DynamicParameters();

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstOrDefaultAsync<TRecord>(sb.ToString(), default, transaction);
    }


    public async Task<TRecord> Last( CancellationToken token ) => await _database.Call(Last, token);
    public async Task<TRecord> Last( DbConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        var sb         = new StringBuilder();
        var parameters = new DynamicParameters();

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstAsync<TRecord>(sb.ToString(), default, transaction);
    }


    public async Task<TRecord?> LastOrDefault( CancellationToken token ) => await _database.Call(LastOrDefault, token);
    public async Task<TRecord?> LastOrDefault( DbConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        var sb         = new StringBuilder();
        var parameters = new DynamicParameters();

        token.ThrowIfCancellationRequested();
        return await connection.QueryFirstOrDefaultAsync<TRecord>(sb.ToString(), default, transaction);
    }


    public async Task<TRecord?> Single( TID         id,         CancellationToken token ) => await _database.Call(Single, id, token);
    public async Task<TRecord?> Single( DbConnection connection, DbTransaction?    transaction, TID id, CancellationToken token ) => null;


    public async Task<TRecord?> Single( StringBuilder sb,         DynamicParameters? parameters,  CancellationToken token ) => await _database.Call(Single, sb, parameters, token);
    public async Task<TRecord?> Single( DbConnection  connection, DbTransaction?     transaction, StringBuilder     sb, DynamicParameters? parameters, CancellationToken token ) => null;


    public async Task<TRecord?> SingleOrDefault( TID         id,         CancellationToken token ) => await _database.Call(SingleOrDefault, id, token);
    public async Task<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction?    transaction, TID id, CancellationToken token ) => null;


    public async Task<TRecord?> SingleOrDefault( StringBuilder sb,         DynamicParameters? parameters,  CancellationToken token ) => await _database.Call(SingleOrDefault, sb, parameters, token);
    public async Task<TRecord?> SingleOrDefault( DbConnection  connection, DbTransaction?     transaction, StringBuilder     sb, DynamicParameters? parameters, CancellationToken token ) => null;


    public async Task<List<TRecord>> All( CancellationToken token ) => await _database.Call(All, token);
    public async Task<List<TRecord>> All( DbConnection      connection, DbTransaction? transaction, CancellationToken token ) => null;


    public async Task<TResult> Call<TResult>( StringBuilder sb, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, Task<TResult>> func, CancellationToken token ) => await _database.Call(Call, sb, parameters, func, token);
    public async Task<TResult> Call<TResult>( DbConnection connection, DbTransaction? transaction, StringBuilder sb, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, Task<TResult>> func, CancellationToken token ) =>
        default;


    public async Task<TResult> Call<TResult>( StringBuilder sb,         DynamicParameters? parameters,  Func<DataTable, CancellationToken, Task<TResult>> func, CancellationToken token ) => await _database.Call(Call, sb, parameters, func, token);
    public async Task<TResult> Call<TResult>( DbConnection  connection, DbTransaction?     transaction, StringBuilder sb, DynamicParameters? parameters, Func<DataTable, CancellationToken, Task<TResult>> func, CancellationToken token ) => default;


    public async Task<TResult> Call<TResult>( StringBuilder sb,         DynamicParameters? parameters,  Func<DbDataReader, CancellationToken, Task<TResult>> func, CancellationToken token ) => await _database.Call(Call, sb, parameters, func, token);
    public async Task<TResult> Call<TResult>( DbConnection  connection, DbTransaction?     transaction, StringBuilder sb, DynamicParameters? parameters, Func<DbDataReader, CancellationToken, Task<TResult>> func, CancellationToken token ) => default;


    public async Task<IAsyncEnumerable<TRecord>> Where( StringBuilder sb,         DynamicParameters? parameters,  CancellationToken token ) => await _database.Call(Where, sb, parameters, token);
    public async Task<IAsyncEnumerable<TRecord>> Where( DbConnection  connection, DbTransaction?     transaction, StringBuilder     sb, DynamicParameters? parameters, CancellationToken token ) => null;


    public async Task<TRecord> Get( TID                                     id,         CancellationToken token ) => await _database.Call(Get, id,  token);
    public async Task<IAsyncEnumerable<TRecord>> Get( IEnumerable<TID>      ids,        CancellationToken token ) => await _database.Call(Get, ids, token);
    public async Task<IAsyncEnumerable<TRecord>> Get( IAsyncEnumerable<TID> ids,        CancellationToken token ) => await _database.Call(Get, ids, token);
    public async Task<TRecord> Get( DbConnection                             connection, DbTransaction?    transaction, TID                   id,  CancellationToken token ) => null;
    public async Task<IAsyncEnumerable<TRecord>> Get( DbConnection           connection, DbTransaction?    transaction, IEnumerable<TID>      ids, CancellationToken token ) => null;
    public async Task<IAsyncEnumerable<TRecord>> Get( DbConnection           connection, DbTransaction?    transaction, IAsyncEnumerable<TID> ids, CancellationToken token ) => null;


    public async Task<TRecord> Insert( TRecord                                     record,     CancellationToken token ) => await _database.Call(Insert, record,  token);
    public async Task<IAsyncEnumerable<TRecord>> Insert( IEnumerable<TRecord>      records,    CancellationToken token ) => await _database.Call(Insert, records, token);
    public async Task<IAsyncEnumerable<TRecord>> Insert( IAsyncEnumerable<TRecord> records,    CancellationToken token ) => await _database.Call(Insert, records, token);
    public async Task<TRecord> Insert( DbConnection                                connection, DbTransaction     transaction, TRecord                   record,  CancellationToken token ) => null;
    public async Task<IAsyncEnumerable<TRecord>> Insert( DbConnection              connection, DbTransaction     transaction, IEnumerable<TRecord>      records, CancellationToken token ) => null;
    public async Task<IAsyncEnumerable<TRecord>> Insert( DbConnection              connection, DbTransaction     transaction, IAsyncEnumerable<TRecord> records, CancellationToken token ) => null;


    public async Task<TRecord> Update( TRecord                                     record,     CancellationToken token ) => await _database.Call(Update, record,  token);
    public async Task<IAsyncEnumerable<TRecord>> Update( IEnumerable<TRecord>      records,    CancellationToken token ) => await _database.Call(Update, records, token);
    public async Task<IAsyncEnumerable<TRecord>> Update( IAsyncEnumerable<TRecord> records,    CancellationToken token ) => await _database.Call(Update, records, token);
    public async Task<TRecord> Update( DbConnection                                connection, DbTransaction     transaction, TRecord                   record,  CancellationToken token ) => null;
    public async Task<IAsyncEnumerable<TRecord>> Update( DbConnection              connection, DbTransaction     transaction, IEnumerable<TRecord>      records, CancellationToken token ) => null;
    public async Task<IAsyncEnumerable<TRecord>> Update( DbConnection              connection, DbTransaction     transaction, IAsyncEnumerable<TRecord> records, CancellationToken token ) => null;
}
