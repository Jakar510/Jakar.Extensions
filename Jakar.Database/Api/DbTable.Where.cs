// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:08 PM


namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ) ]
public partial class DbTable<TRecord>
{
    protected static int GetHash( DynamicParameters parameters ) => GetHash( parameters.ParameterNames );
    protected static int GetHash<T>( IEnumerable<T> values )
    {
        var hash = new HashCode();
        foreach ( T value in values ) { hash.Add( value ); }

        return hash.ToHashCode();
    }


    public IAsyncEnumerable<TRecord> Where( bool           matchAll,   DynamicParameters  parameters, [ EnumeratorCancellation ] CancellationToken token = default ) => this.Call( Where, matchAll,   parameters, token );
    public IAsyncEnumerable<TRecord> Where( string         sql,        DynamicParameters? parameters, [ EnumeratorCancellation ] CancellationToken token = default ) => this.Call( Where, sql,        parameters, token );
    public IAsyncEnumerable<TRecord> Where<TValue>( string columnName, TValue?            value,      [ EnumeratorCancellation ] CancellationToken token = default ) => this.Call( Where, columnName, value,      token );


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        int     hash = GetHash( parameters );
        string? sql  = default;
        if ( hash != 0 && !_whereParameters.TryGetValue( hash, out sql ) ) { _whereParameters[hash] = sql = GetWhereSql( matchAll, parameters ); }

        sql ??= GetWhereSql( matchAll, parameters );
        return Where( connection, transaction, new SqlCommand( sql, parameters ), token );
    }

    [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
    private string GetWhereSql( bool matchAll, DynamicParameters parameters )
    {
        using var buffer = new ValueStringBuilder( parameters.ParameterNames.Sum( x => x.Length ) * 2 );

        buffer.AppendJoin( matchAll
                               ? "AND"
                               : "OR",
                           parameters.ParameterNames.Select( KeyValuePair ) );

        return $"SELECT * FROM {SchemaTableName} WHERE {buffer.Span}";
    }


    public virtual async IAsyncEnumerable<TRecord> Where( DbConnection connection, DbTransaction? transaction, SqlCommand sql, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        await using DbDataReader reader = await _database.ExecuteReaderAsync( connection, transaction, sql, token );
        await foreach ( TRecord record in TRecord.CreateAsync( reader, token ) ) { yield return record; }
    }


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public virtual IAsyncEnumerable<TRecord> Where<TValue>( DbConnection connection, DbTransaction? transaction, string columnName, TValue? value, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(value), value );

        if ( !_where.TryGetValue( columnName, out string? sql ) ) { _where[columnName] = sql = $"SELECT * FROM {SchemaTableName} WHERE {columnName} = @{nameof(value)}"; }

        return Where( connection, transaction, new SqlCommand( sql, parameters ), token );
    }
}
