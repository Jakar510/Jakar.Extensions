// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:08 PM


namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public IAsyncEnumerable<TRecord> Where( bool           matchAll,   DynamicParameters  parameters, [EnumeratorCancellation] CancellationToken token = default ) => this.Call( Where, matchAll,   parameters, token );
    public IAsyncEnumerable<TRecord> Where( string         sql,        DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default ) => this.Call( Where, sql,        parameters, token );
    public IAsyncEnumerable<TRecord> Where<TValue>( string columnName, TValue?            value,      [EnumeratorCancellation] CancellationToken token = default ) => this.Call( Where, columnName, value,      token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand               command     = TRecord.SQL.Get(matchAll, parameters);
        await using DbDataReader reader      = await _database.ExecuteReaderAsync( connection, transaction, command, token );
        await foreach ( TRecord record in TRecord.CreateAsync( reader, token ) ) { yield return record; }
    }


    public virtual async IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction? transaction, SqlCommand sql, [EnumeratorCancellation] CancellationToken token = default )
    {
        await using DbDataReader reader = await _database.ExecuteReaderAsync( connection, transaction, sql, token );
        await foreach ( TRecord record in TRecord.CreateAsync( reader, token ) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TRecord> Where( SqlCommand.Definition definition, [EnumeratorCancellation] CancellationToken token = default )
    {
        await using DbDataReader reader = await _database.ExecuteReaderAsync( definition );
        await foreach ( TRecord record in TRecord.CreateAsync( reader, token ) ) { yield return record; }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual IAsyncEnumerable<TRecord> Where<TValue>( DbConnection connection, DbTransaction? transaction, string columnName, TValue? value, [EnumeratorCancellation] CancellationToken token = default )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(value), value );

        SqlCommand sql = new($"SELECT * FROM {TRecord.TableName} WHERE {columnName} = @{nameof(value)};", parameters);
        return Where( connection, transaction, sql, token );
    }
}
