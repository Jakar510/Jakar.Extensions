// Jakar.Extensions :: Jakar.Database
// 08/14/2022  10:46 PM

#pragma warning disable CS8424



namespace Jakar.Database;


[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
public interface IDbTable<TRecord, TID> : IConnectableDb, IAsyncDisposable where TRecord : BaseTableRecord<TRecord, TID>
                                                                           where TID : IComparable<TID>, IEquatable<TID>
{
    public string TableName { get; }


    public Task<string> ServerVersion( CancellationToken token = default );


    public Task<DataTable> Schema( DbConnection connection, CancellationToken token = default );
    public Task<DataTable> Schema( DbConnection connection, string            collectionName, CancellationToken token = default );
    public Task<DataTable> Schema( DbConnection connection, string            collectionName, string?[]         restrictionValues, CancellationToken token = default );


    public Task Schema( Func<DataTable, CancellationToken, Task> func, CancellationToken token = default );
    public Task Schema( Func<DataTable, CancellationToken, Task> func, string            collectionName, CancellationToken token = default );
    public Task Schema( Func<DataTable, CancellationToken, Task> func, string            collectionName, string?[]         restrictionValues, CancellationToken token = default );


    public Task<TResult> Schema<TResult>( Func<DataTable, CancellationToken, Task<TResult>> func, CancellationToken token = default );
    public Task<TResult> Schema<TResult>( Func<DataTable, CancellationToken, Task<TResult>> func, string            collectionName, CancellationToken token = default );
    public Task<TResult> Schema<TResult>( Func<DataTable, CancellationToken, Task<TResult>> func, string            collectionName, string?[]         restrictionValues, CancellationToken token = default );


    public Task<long> Count( CancellationToken token = default );
    public Task<long> Count( DbConnection      connection, DbTransaction? transaction, CancellationToken token = default );


    public Task<TRecord> First( CancellationToken token = default );
    public Task<TRecord> First( DbConnection      connection, DbTransaction? transaction, CancellationToken token = default );


    public Task<TRecord?> FirstOrDefault( CancellationToken token = default );
    public Task<TRecord?> FirstOrDefault( DbConnection      connection, DbTransaction? transaction, CancellationToken token = default );


    public Task<TRecord> Last( CancellationToken token = default );
    public Task<TRecord> Last( DbConnection      connection, DbTransaction? transaction, CancellationToken token = default );


    public Task<TRecord?> LastOrDefault( CancellationToken token = default );
    public Task<TRecord?> LastOrDefault( DbConnection      connection, DbTransaction? transaction, CancellationToken token = default );


    public Task<TRecord?> Single( TID          id,         CancellationToken  token );
    public Task<TRecord?> Single( DbConnection connection, DbTransaction?     transaction, TID               id, CancellationToken token = default );
    public Task<TRecord?> Single( string       sql,        DynamicParameters? parameters,  CancellationToken token = default );
    public Task<TRecord?> Single( DbConnection connection, DbTransaction?     transaction, string            sql, DynamicParameters? parameters, CancellationToken token = default );


    public Task<TRecord?> SingleOrDefault( TID          id,         CancellationToken  token );
    public Task<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction?     transaction, TID               id, CancellationToken token = default );
    public Task<TRecord?> SingleOrDefault( string       sql,        DynamicParameters? parameters,  CancellationToken token = default );
    public Task<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction?     transaction, string            sql, DynamicParameters? parameters, CancellationToken token = default );


    public Task<List<TRecord>> All( CancellationToken token = default );
    public Task<List<TRecord>> All( DbConnection      connection, DbTransaction? transaction, CancellationToken token = default );


    public Task<TResult> Call<TResult>( string       sql,        DynamicParameters? parameters,  Func<SqlMapper.GridReader, CancellationToken, Task<TResult>> func, CancellationToken token = default );
    public Task<TResult> Call<TResult>( DbConnection connection, DbTransaction?     transaction, string sql, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, Task<TResult>> func, CancellationToken token = default );


    public Task<TResult> Call<TResult>( string       sql,        DynamicParameters? parameters,  Func<DbDataReader, CancellationToken, Task<TResult>> func, CancellationToken token = default );
    public Task<TResult> Call<TResult>( DbConnection connection, DbTransaction?     transaction, string sql, DynamicParameters? parameters, Func<DbDataReader, CancellationToken, Task<TResult>> func, CancellationToken token = default );


    public IAsyncEnumerable<TRecord> Where( DynamicParameters parameters, [EnumeratorCancellation] CancellationToken token = default );
    public IAsyncEnumerable<TRecord> Where( DbConnection      connection, DbTransaction?                             transaction, DynamicParameters parameters, [EnumeratorCancellation] CancellationToken token = default );


    public IAsyncEnumerable<TRecord> Where( string       sql,        DynamicParameters? parameters,  [EnumeratorCancellation] CancellationToken token = default );
    public IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction?     transaction, string                                     sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default );


    public IAsyncEnumerable<TRecord> Where<TValue>( string       columnName, TValue?        value,       [EnumeratorCancellation] CancellationToken token = default );
    public IAsyncEnumerable<TRecord> Where<TValue>( DbConnection connection, DbTransaction? transaction, string                                     columnName, TValue? value, [EnumeratorCancellation] CancellationToken token = default );


    public Task<TID> GetID<TValue>( string       sql,        DynamicParameters? parameters,  CancellationToken token = default );
    public Task<TID> GetID<TValue>( DbConnection connection, DbTransaction?     transaction, string            sql, DynamicParameters? parameters, CancellationToken token = default );


    public Task<TID> GetID<TValue>( string       columnName, TValue?        value,       CancellationToken token = default );
    public Task<TID> GetID<TValue>( DbConnection connection, DbTransaction? transaction, string            columnName, TValue? value, CancellationToken token = default );


    public Task<TRecord> Get( TID                               id,         CancellationToken                          token );
    public IAsyncEnumerable<TRecord> Get( IEnumerable<TID>      ids,        [EnumeratorCancellation] CancellationToken token = default );
    public IAsyncEnumerable<TRecord> Get( IAsyncEnumerable<TID> ids,        [EnumeratorCancellation] CancellationToken token = default );
    public Task<TRecord> Get( DbConnection                      connection, DbTransaction?                             transaction, TID                   id,  CancellationToken                          token );
    public IAsyncEnumerable<TRecord> Get( DbConnection          connection, DbTransaction?                             transaction, IEnumerable<TID>      ids, [EnumeratorCancellation] CancellationToken token = default );
    public IAsyncEnumerable<TRecord> Get( DbConnection          connection, DbTransaction?                             transaction, IAsyncEnumerable<TID> ids, [EnumeratorCancellation] CancellationToken token = default );


    public Task<TRecord> Insert( TRecord                               record,     CancellationToken                          token );
    public IAsyncEnumerable<TRecord> Insert( IEnumerable<TRecord>      records,    [EnumeratorCancellation] CancellationToken token = default );
    public IAsyncEnumerable<TRecord> Insert( IAsyncEnumerable<TRecord> records,    [EnumeratorCancellation] CancellationToken token = default );
    public Task<TRecord> Insert( DbConnection                          connection, DbTransaction                              transaction, TRecord                   record,  CancellationToken                          token );
    public IAsyncEnumerable<TRecord> Insert( DbConnection              connection, DbTransaction                              transaction, IEnumerable<TRecord>      records, [EnumeratorCancellation] CancellationToken token = default );
    public IAsyncEnumerable<TRecord> Insert( DbConnection              connection, DbTransaction                              transaction, IAsyncEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default );


    public Task Update( TRecord                   record,     CancellationToken token = default );
    public Task Update( IEnumerable<TRecord>      records,    CancellationToken token = default );
    public Task Update( IAsyncEnumerable<TRecord> records,    CancellationToken token = default );
    public Task Update( DbConnection              connection, DbTransaction     transaction, TRecord                   record,  CancellationToken token = default );
    public Task Update( DbConnection              connection, DbTransaction     transaction, IEnumerable<TRecord>      records, CancellationToken token = default );
    public Task Update( DbConnection              connection, DbTransaction     transaction, IAsyncEnumerable<TRecord> records, CancellationToken token = default );
}
