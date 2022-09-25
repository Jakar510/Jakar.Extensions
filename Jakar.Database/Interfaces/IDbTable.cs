// Jakar.Extensions :: Jakar.Database
// 08/14/2022  10:46 PM

#pragma warning disable CS8424



namespace Jakar.Database;


[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
public interface IDbTable<TRecord> : IConnectableDb, IAsyncDisposable where TRecord : TableRecord<TRecord>
{
    public string TableName { get; }


    public ValueTask<string> ServerVersion( CancellationToken token = default );


    public ValueTask<DataTable> Schema( DbConnection connection, CancellationToken token                                                                        = default );
    public ValueTask<DataTable> Schema( DbConnection connection, string            collectionName, CancellationToken token                                      = default );
    public ValueTask<DataTable> Schema( DbConnection connection, string            collectionName, string?[]         restrictionValues, CancellationToken token = default );


    public ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, CancellationToken token                                                                        = default );
    public ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, string            collectionName, CancellationToken token                                      = default );
    public ValueTask Schema( Func<DataTable, CancellationToken, ValueTask> func, string            collectionName, string?[]         restrictionValues, CancellationToken token = default );


    public ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, CancellationToken token                                                                        = default );
    public ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, string            collectionName, CancellationToken token                                      = default );
    public ValueTask<TResult> Schema<TResult>( Func<DataTable, CancellationToken, ValueTask<TResult>> func, string            collectionName, string?[]         restrictionValues, CancellationToken token = default );


    public ValueTask<long> Count( CancellationToken token                                                           = default );
    public ValueTask<long> Count( DbConnection      connection, DbTransaction? transaction, CancellationToken token = default );


    public ValueTask<TRecord> First( CancellationToken token                                                           = default );
    public ValueTask<TRecord> First( DbConnection      connection, DbTransaction? transaction, CancellationToken token = default );


    public ValueTask<TRecord?> FirstOrDefault( CancellationToken token                                                           = default );
    public ValueTask<TRecord?> FirstOrDefault( DbConnection      connection, DbTransaction? transaction, CancellationToken token = default );


    public ValueTask<TRecord> Last( CancellationToken token                                                           = default );
    public ValueTask<TRecord> Last( DbConnection      connection, DbTransaction? transaction, CancellationToken token = default );


    public ValueTask<TRecord?> LastOrDefault( CancellationToken token                                                           = default );
    public ValueTask<TRecord?> LastOrDefault( DbConnection      connection, DbTransaction? transaction, CancellationToken token = default );


    public ValueTask<TRecord?> Single( long          id,         CancellationToken  token );
    public ValueTask<TRecord?> Single( DbConnection connection, DbTransaction?     transaction, long               id, CancellationToken token = default );
    public ValueTask<TRecord?> Single( string       sql,        DynamicParameters? parameters,  CancellationToken token                                                       = default );
    public ValueTask<TRecord?> Single( DbConnection connection, DbTransaction?     transaction, string            sql, DynamicParameters? parameters, CancellationToken token = default );


    public ValueTask<TRecord?> SingleOrDefault( long          id,         CancellationToken  token );
    public ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction?     transaction, long               id, CancellationToken token = default );
    public ValueTask<TRecord?> SingleOrDefault( string       sql,        DynamicParameters? parameters,  CancellationToken token                                                       = default );
    public ValueTask<TRecord?> SingleOrDefault( DbConnection connection, DbTransaction?     transaction, string            sql, DynamicParameters? parameters, CancellationToken token = default );


    public ValueTask<List<TRecord>> All( CancellationToken token                                                           = default );
    public ValueTask<List<TRecord>> All( DbConnection      connection, DbTransaction? transaction, CancellationToken token = default );


    public ValueTask<TResult> Call<TResult>( string       sql,        DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default );
    public ValueTask<TResult> Call<TResult>( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default );


    public ValueTask<TResult> Call<TResult>( string       sql,        DynamicParameters? parameters,  Func<DbDataReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default );
    public ValueTask<TResult> Call<TResult>( DbConnection connection, DbTransaction?     transaction, string sql, DynamicParameters? parameters, Func<DbDataReader, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default );


    public IAsyncEnumerable<TRecord> Where(bool matchAll, DynamicParameters parameters, [EnumeratorCancellation] CancellationToken token                                                                                                      = default );
    public IAsyncEnumerable<TRecord> Where( DbConnection      connection, DbTransaction?                             transaction, bool matchAll, DynamicParameters parameters, [EnumeratorCancellation] CancellationToken token = default );


    public IAsyncEnumerable<TRecord> Where( string       sql,        DynamicParameters? parameters,  [EnumeratorCancellation] CancellationToken token                                                                                = default );
    public IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction?     transaction, string                                     sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default );


    public IAsyncEnumerable<TRecord> Where<TValue>( string       columnName, TValue?        value,       [EnumeratorCancellation] CancellationToken token                                                                       = default );
    public IAsyncEnumerable<TRecord> Where<TValue>( DbConnection connection, DbTransaction? transaction, string                                     columnName, TValue? value, [EnumeratorCancellation] CancellationToken token = default );


    public ValueTask<long> GetID<TValue>( string       sql,        DynamicParameters? parameters,  CancellationToken token                                                       = default );
    public ValueTask<long> GetID<TValue>( DbConnection connection, DbTransaction?     transaction, string            sql, DynamicParameters? parameters, CancellationToken token = default );


    public ValueTask<long> GetID<TValue>( string       columnName, TValue         value,       CancellationToken token                                             = default );
    public ValueTask<long> GetID<TValue>( DbConnection connection, DbTransaction? transaction, string            columnName, TValue value, CancellationToken token = default );


    public ValueTask<TRecord?> Get<TValue>( string       columnName, TValue         value,       CancellationToken token                                             = default );
    public ValueTask<TRecord?> Get<TValue>( DbConnection connection, DbTransaction? transaction, string            columnName, TValue value, CancellationToken token = default );


    public ValueTask<TRecord?> Get( long                          id,         CancellationToken                          token );
    public IAsyncEnumerable<TRecord?> Get( IEnumerable<long>      ids,        [EnumeratorCancellation] CancellationToken token = default );
    public IAsyncEnumerable<TRecord?> Get( IAsyncEnumerable<long> ids,        [EnumeratorCancellation] CancellationToken token = default );
    public ValueTask<TRecord?> Get( DbConnection                 connection, DbTransaction?                             transaction, long                   id,  CancellationToken                          token );
    public IAsyncEnumerable<TRecord?> Get( DbConnection          connection, DbTransaction?                             transaction, IEnumerable<long>      ids, [EnumeratorCancellation] CancellationToken token = default );
    public IAsyncEnumerable<TRecord?> Get( DbConnection          connection, DbTransaction?                             transaction, IAsyncEnumerable<long> ids, [EnumeratorCancellation] CancellationToken token = default );


    public ValueTask<TRecord> Insert( TRecord                          record,     CancellationToken                          token );
    public IAsyncEnumerable<TRecord> Insert( IEnumerable<TRecord>      records,    [EnumeratorCancellation] CancellationToken token = default );
    public IAsyncEnumerable<TRecord> Insert( IAsyncEnumerable<TRecord> records,    [EnumeratorCancellation] CancellationToken token = default );
    public ValueTask<TRecord> Insert( DbConnection                     connection, DbTransaction                              transaction, TRecord                   record,  CancellationToken                          token );
    public IAsyncEnumerable<TRecord> Insert( DbConnection              connection, DbTransaction                              transaction, IEnumerable<TRecord>      records, [EnumeratorCancellation] CancellationToken token = default );
    public IAsyncEnumerable<TRecord> Insert( DbConnection              connection, DbTransaction                              transaction, IAsyncEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default );


    public ValueTask Update( TRecord                   record,     CancellationToken token                                                                   = default );
    public ValueTask Update( IEnumerable<TRecord>      records,    CancellationToken token                                                                   = default );
    public ValueTask Update( IAsyncEnumerable<TRecord> records,    CancellationToken token                                                                   = default );
    public ValueTask Update( DbConnection              connection, DbTransaction?    transaction, TRecord                   record,  CancellationToken token = default );
    public ValueTask Update( DbConnection              connection, DbTransaction?    transaction, IEnumerable<TRecord>      records, CancellationToken token = default );
    public ValueTask Update( DbConnection              connection, DbTransaction?    transaction, IAsyncEnumerable<TRecord> records, CancellationToken token = default );
}
