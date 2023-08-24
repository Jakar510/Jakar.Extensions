// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:08 PM


namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    private readonly ConcurrentDictionary<string, string> _where           = new();
    private readonly ConcurrentDictionary<int, string>    _whereParameters = new();


    private static int GetHash( DynamicParameters parameters ) => GetHash( parameters.ParameterNames );
    private static int GetHash<T>( IEnumerable<T> values )
    {
        var hash = new HashCode();
        foreach ( T value in values ) { hash.Add( value ); }

        return hash.ToHashCode();
    }


    public ValueTask<IEnumerable<TRecord>> Where( bool           matchAll,   DynamicParameters  parameters, CancellationToken token = default ) => this.Call( Where, matchAll,   parameters, token );
    public ValueTask<IEnumerable<TRecord>> Where( string         sql,        DynamicParameters? parameters, CancellationToken token = default ) => this.Call( Where, sql,        parameters, token );
    public ValueTask<IEnumerable<TRecord>> Where<TValue>( string columnName, TValue?            value,      CancellationToken token = default ) => this.Call( Where, columnName, value,      token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual ValueTask<IEnumerable<TRecord>> Where( DbConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        int     hash = GetHash( parameters );
        string? sql  = default;
        if ( hash > 0 && !_whereParameters.TryGetValue( hash, out sql ) ) { _whereParameters[hash] = sql = GetWhereSql( matchAll, parameters ); }

        sql ??= GetWhereSql( matchAll, parameters );
        return Where( connection, transaction, sql, parameters, token );
    }
    private string GetWhereSql( bool matchAll, DynamicParameters parameters )
    {
        return $"SELECT * FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                         ? "AND"
                                                                         : "OR",
                                                                     parameters.ParameterNames.Select( KeyValuePair ) )}";
    }


    public virtual async ValueTask<IEnumerable<TRecord>> Where( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested ) { return Empty; }

        try
        {
            CommandDefinition     command = GetCommandDefinition( sql, parameters, transaction, token );
            IEnumerable<TRecord?> records = await connection.QueryAsync<TRecord>( command );
            return records.WhereNotNull();
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual ValueTask<IEnumerable<TRecord>> Where<TValue>( DbConnection connection, DbTransaction? transaction, string columnName, TValue? value, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(value), value );

        if ( !_where.TryGetValue( columnName, out string? sql ) ) { _where[columnName] = sql = $"SELECT * FROM {SchemaTableName} WHERE {columnName} = @{nameof(value)}"; }

        return Where( connection, transaction, sql, parameters, token );
    }
}
