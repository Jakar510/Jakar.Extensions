﻿// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:07 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask<long> Count( CancellationToken                    token = default ) => this.Call( Count, token );
    public ValueTask<bool> Exists( bool                                matchAll,   DynamicParameters  parameters, CancellationToken token ) => this.TryCall( Exists, matchAll, parameters, token );
    public ValueTask<Guid?> GetID( string                              sql,        DynamicParameters? parameters, CancellationToken token = default ) => this.Call( GetID, sql,        parameters, token );
    public ValueTask<Guid?> GetID( string                              columnName, object             value,      CancellationToken token = default ) => this.Call( GetID, columnName, value,      token );
    public ValueTask<IEnumerable<TRecord>> Get( IEnumerable<Guid>      ids,        CancellationToken  token                               = default ) => this.Call( Get, ids,        token );
    public ValueTask<IEnumerable<TRecord>> Get( IAsyncEnumerable<Guid> ids,        CancellationToken  token                               = default ) => this.Call( Get, ids,        token );
    public ValueTask<TRecord?> Get( bool                               matchAll,   DynamicParameters  parameters, CancellationToken token = default ) => this.Call( Get, matchAll,   parameters, token );
    public ValueTask<TRecord?> Get( string                             columnName, object?            value,      CancellationToken token = default ) => this.Call( Get, columnName, value,      token );
    public ValueTask<TRecord?> Get( Guid                               id,         CancellationToken  token = default ) => this.Call( Get, id, token );
    public ValueTask<TRecord?> Get( Guid?                              id,         CancellationToken  token = default ) => this.Call( Get, id, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<long> Count( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested ) { return default; }

        string sql = $"SELECT COUNT({ID_ColumnName}) FROM {SchemaTableName}";

        try { return await connection.QueryFirstAsync<long>( sql, default, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }

    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<bool> Exists( DbConnection connection, DbTransaction transaction, bool matchAll, DynamicParameters parameters, CancellationToken token )
    {
        string sql = Instance switch
                     {
                         DbInstance.MsSql => $"SELECT TOP 1 {ID_ColumnName} FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                                                                ? "AND"
                                                                                                                : "OR",
                                                                                                            parameters.ParameterNames.Select( KeyValuePair ) )}",
                         DbInstance.Postgres => $"SELECT {ID_ColumnName} FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                                                             ? "AND"
                                                                                                             : "OR",
                                                                                                         parameters.ParameterNames.Select( KeyValuePair ) )} LIMIT 1",
                         _ => throw new OutOfRangeException( nameof(Instance), Instance ),
                     };


        token.ThrowIfCancellationRequested();

        try
        {
            IEnumerable<string> results = await connection.QueryAsync<string>( sql, parameters, transaction );
            return results.Any();
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }

    public async ValueTask<Guid?> GetID( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.QuerySingleAsync<Guid?>( sql, parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }

    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<Guid?> GetID( DbConnection connection, DbTransaction? transaction, string columnName, object value, CancellationToken token = default )
    {
        string sql = $"SELECT {ID_ColumnName} FROM {SchemaTableName} WHERE {columnName} = @{nameof(value)}";
        return await GetID( connection, transaction, sql, Database.GetParameters( value ), token );
    }


    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, Guid? id, CancellationToken token = default ) =>
        id.HasValue
            ? await Get( connection, transaction, ID_ColumnName, id.Value, token )
            : default;
    public async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, Guid id, CancellationToken token = default ) =>
        await Get( connection, transaction, ID_ColumnName, id, token );


    public virtual async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, string columnName, object? value, CancellationToken token = default ) =>
        await Get( connection, transaction, true, Database.GetParameters( value, default, columnName ), token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> Get( DbConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested ) { return default; }

        string sql = $"SELECT * FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                               ? "AND"
                                                                               : "OR",
                                                                           parameters.ParameterNames.Select( x => GetDescriptor( x ).KeyValuePair ) )}";


        try
        {
            IEnumerable<TRecord?> results = await connection.QueryAsync<TRecord>( sql, parameters, transaction );
            IEnumerable<TRecord>  records = results.WhereNotNull();
            TRecord?              result  = default;
            
            // ReSharper disable once PossibleMultipleEnumeration
            foreach ( TRecord record in records )
            {
                // ReSharper disable once PossibleMultipleEnumeration
                if ( result is not null ) { throw new DuplicateRecordException( $"Record IDs: {string.Join( ',', records.Select( x => x.ID ) )}" ); }

                result = record;
            }

            return result;
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    public virtual async ValueTask<IEnumerable<TRecord>> Get( DbConnection connection, DbTransaction? transaction, IAsyncEnumerable<Guid> ids, CancellationToken token = default )
    {
        HashSet<Guid> set = await ids.ToHashSet( token );
        return await Get( connection, transaction, set, token );
    }
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual ValueTask<IEnumerable<TRecord>> Get( DbConnection connection, DbTransaction? transaction, IEnumerable<Guid> ids, CancellationToken token = default )
    {
        string sql = $"SELECT * FROM {SchemaTableName} WHERE {ID_ColumnName} in ( {string.Join( ',', ids.Select( x => $"'{x}'" ) )} )";
        return Where( connection, transaction, sql, default, token );
    }
}
