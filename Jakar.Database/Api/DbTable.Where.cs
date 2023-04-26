// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:08 PM

namespace Jakar.Database;


public partial class DbTable<TRecord>
{
    public ValueTask<TRecord[]> Where( bool           matchAll,   DynamicParameters  parameters, CancellationToken token = default ) => this.Call( Where, matchAll,   parameters, token );
    public ValueTask<TRecord[]> Where( string         sql,        DynamicParameters? parameters, CancellationToken token = default ) => this.Call( Where, sql,        parameters, token );
    public ValueTask<TRecord[]> Where<TValue>( string columnName, TValue?            value,      CancellationToken token = default ) => this.Call( Where, columnName, value,      token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual ValueTask<TRecord[]> Where( DbConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                               ? "AND"
                                                                               : "OR",
                                                                           parameters.ParameterNames.Select( KeyValuePair ) )}";

        return Where( connection, transaction, sql, parameters, token );
    }


    public virtual async ValueTask<TRecord[]> Where( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested ) { return Empty; }

        try
        {
            IEnumerable<TRecord> records = await connection.QueryAsync<TRecord>( sql, parameters, transaction );

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            var results = new List<TRecord>( records.Where( x => x is not null ) );
            return results.GetArray();
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual ValueTask<TRecord[]> Where<TValue>( DbConnection connection, DbTransaction? transaction, string columnName, TValue? value, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} WHERE {columnName} = @{nameof(value)}";

        var parameters = new DynamicParameters();
        parameters.Add( nameof(value), value );

        return Where( connection, transaction, sql, parameters, token );
    }
}
