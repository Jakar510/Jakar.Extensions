// Jakar.Extensions :: Jakar.Database
// 08/14/2022  10:46 PM

#pragma warning disable CS8424



namespace Jakar.Database;


[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
public interface IDbTable<TRecord, in TID> : IConnectableDb, IAsyncDisposable where TRecord : BaseTableRecord<TRecord, TID>
                                                                              where TID : IComparable<TID>, IEquatable<TID>
{
    public string TableName { get; }


    public Task<string> ServerVersion( CancellationToken token );


    public Task<DataTable> Schema( DbConnection connection, CancellationToken token );
    public Task<DataTable> Schema( DbConnection connection, string            collectionName, CancellationToken token );
    public Task<DataTable> Schema( DbConnection connection, string            collectionName, string?[]         restrictionValues, CancellationToken token );


    public Task Schema( Func<DataTable, CancellationToken, Task> func, CancellationToken token );
    public Task Schema( Func<DataTable, CancellationToken, Task> func, string            collectionName, CancellationToken token );
    public Task Schema( Func<DataTable, CancellationToken, Task> func, string            collectionName, string?[]         restrictionValues, CancellationToken token );


    public Task<TResult> Schema<TResult>( Func<DataTable, CancellationToken, Task<TResult>> func, CancellationToken token );
    public Task<TResult> Schema<TResult>( Func<DataTable, CancellationToken, Task<TResult>> func, string            collectionName, CancellationToken token );
    public Task<TResult> Schema<TResult>( Func<DataTable, CancellationToken, Task<TResult>> func, string            collectionName, string?[]         restrictionValues, CancellationToken token );


    public Task<long> Count( CancellationToken token );
    public Task<long> Count( DbConnection      connection, DbTransaction? transaction, CancellationToken token );


    public Task<TRecord> First( CancellationToken token );
    public Task<TRecord> First( DbConnection      connection, DbTransaction? transaction, CancellationToken token );


    public Task<TRecord?> FirstOrDefault( CancellationToken token );
    public Task<TRecord?> FirstOrDefault( DbConnection      connection, DbTransaction? transaction, CancellationToken token );


    public Task<TRecord> Last( CancellationToken token );
    public Task<TRecord> Last( DbConnection      connection, DbTransaction? transaction, CancellationToken token );


    public Task<TRecord?> LastOrDefault( CancellationToken token );
    public Task<TRecord?> LastOrDefault( DbConnection      connection, DbTransaction? transaction, CancellationToken token );


    public Task<TRecord?> Single( TID           id,         CancellationToken  token );
    public Task<TRecord?> Single( DbConnection  connection, DbTransaction?     transaction, TID               id, CancellationToken token );
    public Task<TRecord?> Single( StringBuilder sb,         DynamicParameters? parameters,  CancellationToken token );
    public Task<TRecord?> Single( DbConnection  connection, DbTransaction?     transaction, StringBuilder     sb, DynamicParameters? parameters, CancellationToken token );


    public Task<TRecord?> SingleOrDefault( TID           id,         CancellationToken  token );
    public Task<TRecord?> SingleOrDefault( DbConnection  connection, DbTransaction?     transaction, TID               id, CancellationToken token );
    public Task<TRecord?> SingleOrDefault( StringBuilder sb,         DynamicParameters? parameters,  CancellationToken token );
    public Task<TRecord?> SingleOrDefault( DbConnection  connection, DbTransaction?     transaction, StringBuilder     sb, DynamicParameters? parameters, CancellationToken token );


    public Task<List<TRecord>> All( CancellationToken token );
    public Task<List<TRecord>> All( DbConnection      connection, DbTransaction? transaction, CancellationToken token );


    public Task<TResult> Call<TResult>( StringBuilder sb,         DynamicParameters? parameters,  Func<SqlMapper.GridReader, CancellationToken, Task<TResult>> func, CancellationToken token );
    public Task<TResult> Call<TResult>( DbConnection  connection, DbTransaction?     transaction, StringBuilder sb, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, Task<TResult>> func, CancellationToken token );


    public Task<TResult> Call<TResult>( StringBuilder sb,         DynamicParameters? parameters,  Func<DbDataReader, CancellationToken, Task<TResult>> func, CancellationToken token );
    public Task<TResult> Call<TResult>( DbConnection  connection, DbTransaction?     transaction, StringBuilder sb, DynamicParameters? parameters, Func<DbDataReader, CancellationToken, Task<TResult>> func, CancellationToken token );


    public IAsyncEnumerable<TRecord> Where( StringBuilder sb,         DynamicParameters? parameters,  [EnumeratorCancellation] CancellationToken token );
    public IAsyncEnumerable<TRecord> Where( DbConnection  connection, DbTransaction?     transaction, StringBuilder                              sb, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token );


    public Task<TRecord> Get( TID                               id,         CancellationToken                          token );
    public IAsyncEnumerable<TRecord> Get( IEnumerable<TID>      ids,        [EnumeratorCancellation] CancellationToken token );
    public IAsyncEnumerable<TRecord> Get( IAsyncEnumerable<TID> ids,        [EnumeratorCancellation] CancellationToken token );
    public Task<TRecord> Get( DbConnection                      connection, DbTransaction?                             transaction, TID                   id,  CancellationToken                          token );
    public IAsyncEnumerable<TRecord> Get( DbConnection          connection, DbTransaction?                             transaction, IEnumerable<TID>      ids, [EnumeratorCancellation] CancellationToken token );
    public IAsyncEnumerable<TRecord> Get( DbConnection          connection, DbTransaction?                             transaction, IAsyncEnumerable<TID> ids, [EnumeratorCancellation] CancellationToken token );


    public Task<TRecord> Insert( TRecord                               record,     CancellationToken                          token );
    public IAsyncEnumerable<TRecord> Insert( IEnumerable<TRecord>      records,    [EnumeratorCancellation] CancellationToken token );
    public IAsyncEnumerable<TRecord> Insert( IAsyncEnumerable<TRecord> records,    [EnumeratorCancellation] CancellationToken token );
    public Task<TRecord> Insert( DbConnection                          connection, DbTransaction                              transaction, TRecord                   record,  CancellationToken                          token );
    public IAsyncEnumerable<TRecord> Insert( DbConnection              connection, DbTransaction                              transaction, IEnumerable<TRecord>      records, [EnumeratorCancellation] CancellationToken token );
    public IAsyncEnumerable<TRecord> Insert( DbConnection              connection, DbTransaction                              transaction, IAsyncEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token );


    public Task Update( TRecord                   record,     CancellationToken token );
    public Task Update( IEnumerable<TRecord>      records,    CancellationToken token );
    public Task Update( IAsyncEnumerable<TRecord> records,    CancellationToken token );
    public Task Update( DbConnection              connection, DbTransaction     transaction, TRecord                   record,  CancellationToken token );
    public Task Update( DbConnection              connection, DbTransaction     transaction, IEnumerable<TRecord>      records, CancellationToken token );
    public Task Update( DbConnection              connection, DbTransaction     transaction, IAsyncEnumerable<TRecord> records, CancellationToken token );
}
