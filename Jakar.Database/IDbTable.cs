﻿// Jakar.Extensions :: Jakar.Database
// 08/14/2022  10:46 PM

using System.Diagnostics.CodeAnalysis;



namespace Jakar.Database;


[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
public interface IDbTable<TRecord, in TID> : IAsyncDisposable where TRecord : BaseTableRecord<TRecord, TID>
                                                              where TID : IComparable<TID>, IEquatable<TID>
{
    public string TableName { get; }


    public Task<string> ServerVersion( CancellationToken      token );
    public Task<DbConnection> ConnectAsync( CancellationToken token );


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


    public Task<TRecord?> Single( TID          id,         CancellationToken  token );
    public Task<TRecord?> Single( DbConnection  connection, DbTransaction?     transaction, TID              id, CancellationToken token );
    public Task<TRecord?> Single( StringBuilder sb,         DynamicParameters? parameters,  CancellationToken token );
    public Task<TRecord?> Single( DbConnection  connection, DbTransaction?     transaction, StringBuilder     sb, DynamicParameters? parameters, CancellationToken token );


    public Task<TRecord?> SingleOrDefault( TID          id,         CancellationToken  token );
    public Task<TRecord?> SingleOrDefault( DbConnection  connection, DbTransaction?     transaction, TID              id, CancellationToken token );
    public Task<TRecord?> SingleOrDefault( StringBuilder sb,         DynamicParameters? parameters,  CancellationToken token );
    public Task<TRecord?> SingleOrDefault( DbConnection  connection, DbTransaction?     transaction, StringBuilder     sb, DynamicParameters? parameters, CancellationToken token );


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


    public Task<TRecord> Get( TID                                     id,         CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Get( IEnumerable<TID>      ids,        CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Get( IAsyncEnumerable<TID> ids,        CancellationToken token );
    public Task<TRecord> Get( DbConnection                             connection, DbTransaction?    transaction, TID                   id,  CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Get( DbConnection           connection, DbTransaction?    transaction, IEnumerable<TID>      ids, CancellationToken token );
    public Task<IAsyncEnumerable<TRecord>> Get( DbConnection           connection, DbTransaction?    transaction, IAsyncEnumerable<TID> ids, CancellationToken token );


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
