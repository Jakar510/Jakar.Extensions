﻿// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:08 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask<IEnumerable<TRecord>> Where( bool           matchAll,   DynamicParameters  parameters, CancellationToken token = default ) => this.Call( Where, matchAll,   parameters, token );
    public ValueTask<IEnumerable<TRecord>> Where( string         sql,        DynamicParameters? parameters, CancellationToken token = default ) => this.Call( Where, sql,        parameters, token );
    public ValueTask<IEnumerable<TRecord>> Where<TValue>( string columnName, TValue?            value,      CancellationToken token = default ) => this.Call( Where, columnName, value,      token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual ValueTask<IEnumerable<TRecord>> Where( DbConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                               ? "AND"
                                                                               : "OR",
                                                                           parameters.ParameterNames.Select( KeyValuePair ) )}";

        return Where( connection, transaction, sql, parameters, token );
    }


    public virtual async ValueTask<IEnumerable<TRecord>> Where( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested ) { return Empty; }

        try
        {
            IEnumerable<TRecord?> records = await connection.QueryAsync<TRecord>( sql, parameters, transaction );
            return records.WhereNotNull();
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual ValueTask<IEnumerable<TRecord>> Where<TValue>( DbConnection connection, DbTransaction? transaction, string columnName, TValue? value, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} WHERE {columnName} = @{nameof(value)}";

        var parameters = new DynamicParameters();
        parameters.Add( nameof(value), value );

        return Where( connection, transaction, sql, parameters, token );
    }
}
