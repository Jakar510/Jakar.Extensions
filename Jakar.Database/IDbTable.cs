// Jakar.Extensions :: Jakar.Database
// 08/14/2022  10:46 PM

namespace Jakar.Database;


public interface IDbTable<TRecord> : IAsyncDisposable where TRecord : BaseTableRecord<TRecord>
{
    public Task<string> ServerVersion();
    public Task<DbConnection> ConnectAsync();


    public Task<DataTable> Schema( DbConnection                                             connection, CancellationToken token );
    public Task Schema( Func<DataTable, CancellationToken, Task>                            func,       CancellationToken token );
    public Task<TResult> Schema<TResult>( Func<DataTable, CancellationToken, Task<TResult>> func,       CancellationToken token );


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


    public Task<TRecord> Single( CancellationToken           token );
    public Task<TRecord> Single( DbConnection                connection, DbTransaction? transaction, CancellationToken token );
    public Task<TRecord?> SingleOrDefault( CancellationToken token );
    public Task<TRecord?> SingleOrDefault( DbConnection      connection, DbTransaction? transaction, CancellationToken token );


    public Task<List<TRecord>> All( CancellationToken token );
    public Task<List<TRecord>> All( DbConnection      connection, DbTransaction? transaction, CancellationToken token );


    public Task<TResult> Call<TResult>( StringBuilder sb,         DynamicParameters? parameters,  Func<SqlMapper.GridReader, CancellationToken, Task<TResult>> func, CancellationToken token );
    public Task<TResult> Call<TResult>( DbConnection  connection, DbTransaction?     transaction, StringBuilder sb, DynamicParameters? parameters, Func<SqlMapper.GridReader, CancellationToken, Task<TResult>> func, CancellationToken token );


    public Task<TResult> Call<TResult>( StringBuilder sb,         DynamicParameters? parameters,  Func<DataTable, CancellationToken, Task<TResult>> func, CancellationToken token );
    public Task<TResult> Call<TResult>( DbConnection  connection, DbTransaction?     transaction, StringBuilder sb, DynamicParameters? parameters, Func<DataTable, CancellationToken, Task<TResult>> func, CancellationToken token );


    public Task<TResult> Call<TResult>( StringBuilder sb,         DynamicParameters? parameters,  Func<DbDataReader, CancellationToken, Task<TResult>> func, CancellationToken token );
    public Task<TResult> Call<TResult>( DbConnection  connection, DbTransaction?     transaction, StringBuilder sb, DynamicParameters? parameters, Func<DbDataReader, CancellationToken, Task<TResult>> func, CancellationToken token );


    public Task<IAsyncEnumerable<TRecord>> Where( StringBuilder sb,         DynamicParameters? parameters,  CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Where( DbConnection  connection, DbTransaction?     transaction, StringBuilder     sb, DynamicParameters? parameters, CancellationToken token );


    public Task<IAsyncEnumerable<TRecord>> WhereAsync( StringBuilder sb,         DynamicParameters? parameters,  CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> WhereAsync( DbConnection  connection, DbTransaction?     transaction, StringBuilder     sb, DynamicParameters? parameters, CancellationToken token );


    public Task<TRecord> Get( long                                     id,         CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Get( IEnumerable<long>      id,         CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Get( IAsyncEnumerable<long> id,         CancellationToken token );
    public Task<TRecord> Get( DbConnection                             connection, DbTransaction?    transaction, long                   id, CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Get( DbConnection           connection, DbTransaction?    transaction, IEnumerable<long>      id, CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Get( DbConnection           connection, DbTransaction?    transaction, IAsyncEnumerable<long> id, CancellationToken token );


    public Task<TRecord> Insert( TRecord                                     record,     CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Insert( IEnumerable<TRecord>      records,    CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Insert( IAsyncEnumerable<TRecord> records,    CancellationToken token );
    public Task<TRecord> Insert( DbConnection                                connection, DbTransaction     transaction, TRecord                   record,  CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Insert( DbConnection              connection, DbTransaction     transaction, IEnumerable<TRecord>      records, CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Insert( DbConnection              connection, DbTransaction     transaction, IAsyncEnumerable<TRecord> records, CancellationToken token );


    public Task<TRecord> Update( TRecord                                     record,     CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Update( IEnumerable<TRecord>      records,    CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Update( IAsyncEnumerable<TRecord> records,    CancellationToken token );
    public Task<TRecord> Update( DbConnection                                connection, DbTransaction     transaction, TRecord                   record,  CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Update( DbConnection              connection, DbTransaction     transaction, IEnumerable<TRecord>      records, CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Update( DbConnection              connection, DbTransaction     transaction, IAsyncEnumerable<TRecord> records, CancellationToken token );
}
