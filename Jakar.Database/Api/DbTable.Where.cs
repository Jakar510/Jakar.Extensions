// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:08 PM


namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public IAsyncEnumerable<TRecord> Where( Activity?         activity, bool   matchAll,   DynamicParameters  parameters, [EnumeratorCancellation] CancellationToken token = default ) => this.Call( Where, activity, matchAll,   parameters, token );
    public IAsyncEnumerable<TRecord> Where( Activity?         activity, string sql,        DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default ) => this.Call( Where, activity, sql,        parameters, token );
    public IAsyncEnumerable<TRecord> Where<TValue>( Activity? activity, string columnName, TValue?            value,      [EnumeratorCancellation] CancellationToken token = default ) => this.Call( Where, activity, columnName, value,      token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction? transaction, Activity? activity, bool matchAll, DynamicParameters parameters, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Where( matchAll, parameters );
        return Where( connection, transaction, activity, sql, token );
    }


    public virtual async IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction? transaction, Activity? activity, SqlCommand sql, [EnumeratorCancellation] CancellationToken token = default )
    {
        await using DbDataReader reader = await _database.ExecuteReaderAsync( connection, transaction, activity, sql, token );
        await foreach ( TRecord record in TRecord.CreateAsync( reader, token ) ) { yield return record; }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual IAsyncEnumerable<TRecord> Where<TValue>( DbConnection connection, DbTransaction? transaction, Activity? activity, string columnName, TValue? value, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Where( columnName, value );
        return Where( connection, transaction, activity, sql, token );
    }
}
