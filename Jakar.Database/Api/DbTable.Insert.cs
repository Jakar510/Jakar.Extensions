﻿// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:09 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public IAsyncEnumerable<TRecord> Insert( IEnumerable<TRecord>      records, CancellationToken token = default ) => this.TryCall( Insert, records, token );
    public IAsyncEnumerable<TRecord> Insert( IAsyncEnumerable<TRecord> records, CancellationToken token = default ) => this.TryCall( Insert, records, token );
    public ValueTask<TRecord> Insert( TRecord                          record,  CancellationToken token = default ) => this.TryCall( Insert, record,  token );
    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, IEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { yield return await Insert( connection, transaction, record, token ); }
    }
    public virtual async IAsyncEnumerable<TRecord> Insert( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TRecord> records, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TRecord record in records.WithCancellation( token ) ) { yield return await Insert( connection, transaction, record, token ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord> Insert( DbConnection connection, DbTransaction transaction, TRecord record, CancellationToken token = default )
    {
        string sql = $@"SET NOCOUNT ON INSERT INTO {SchemaTableName} ({string.Join( ',', ColumnNames )}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )});";

        var parameters = new DynamicParameters( record );

        try
        {
            CommandDefinition command = GetCommandDefinition( sql, parameters, transaction, token );
            var               id      = await connection.ExecuteScalarAsync<Guid>( command );
            return record.NewID( id );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }

    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> TryInsert( DbConnection connection, DbTransaction transaction, TRecord record, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        string sql = Instance switch
                     {
                         DbInstance.MsSql => $@"IF NOT EXISTS(SELECT * FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                                                                      ? "AND"
                                                                                                                      : "OR",
                                                                                                                  parameters.ParameterNames.Select( KeyValuePair ) )})
BEGIN
    SET NOCOUNT ON INSERT INTO {SchemaTableName} ({string.Join( ',', ColumnNames )}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
END

ELSE 
BEGIN 
    SELECT {ID_ColumnName} = NULL 
END",
                         DbInstance.Postgres => $@"IF NOT EXISTS(SELECT * FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                                                                         ? "AND"
                                                                                                                         : "OR",
                                                                                                                     parameters.ParameterNames.Select( KeyValuePair ) )})
BEGIN
    SET NOCOUNT ON INSERT INTO {SchemaTableName} ({string.Join( ',', ColumnNames )}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
END

ELSE 
BEGIN 
    SELECT {ID_ColumnName} = NULL 
END",
                         _ => throw new OutOfRangeException( nameof(Instance), Instance ),
                     };


        try
        {
            CommandDefinition command = GetCommandDefinition( sql, parameters, transaction, token );
            var               id      = await connection.ExecuteScalarAsync<Guid?>( command );

            return id.HasValue
                       ? record.NewID( id.Value )
                       : default;
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> InsertOrUpdate( DbConnection connection, DbTransaction transaction, TRecord record, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        string sql = Instance switch
                     {
                         DbInstance.MsSql => $@"IF NOT EXISTS(SELECT * FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                                                                      ? "AND"
                                                                                                                      : "OR",
                                                                                                                  parameters.ParameterNames.Select( KeyValuePair ) )})
BEGIN
    SET NOCOUNT ON INSERT INTO {SchemaTableName} ({string.Join( ',', ColumnNames )}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
END

ELSE 
BEGIN 
    UPDATE {SchemaTableName} SET {string.Join( ',', KeyValuePairs )} WHERE {ID_ColumnName} = @{string.Join( matchAll
                                                                                                                ? "AND"
                                                                                                                : "OR",
                                                                                                            parameters.ParameterNames.Select( KeyValuePair ) )};

    SELECT TOP 1 {ID_ColumnName} FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                                ? "AND"
                                                                                : "OR",
                                                                            parameters.ParameterNames.Select( KeyValuePair ) )} 
END",
                         DbInstance.Postgres => $@"IF NOT EXISTS(SELECT * FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                                                                         ? "AND"
                                                                                                                         : "OR",
                                                                                                                     parameters.ParameterNames.Select( KeyValuePair ) )})
BEGIN
    SET NOCOUNT ON INSERT INTO {SchemaTableName} ({string.Join( ',', ColumnNames )}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
END

ELSE 
BEGIN 
    UPDATE {SchemaTableName} SET {string.Join( ',', KeyValuePairs )} WHERE {ID_ColumnName} = @{string.Join( matchAll
                                                                                                                ? "AND"
                                                                                                                : "OR",
                                                                                                            parameters.ParameterNames.Select( KeyValuePair ) )};

    SELECT {ID_ColumnName} FROM {SchemaTableName} WHERE {string.Join( matchAll
                                                                          ? "AND"
                                                                          : "OR",
                                                                      parameters.ParameterNames.Select( KeyValuePair ) )} LIMIT 1
END",
                         _ => throw new OutOfRangeException( nameof(Instance), Instance ),
                     };


        try
        {
            CommandDefinition command = GetCommandDefinition( sql, parameters, transaction, token );
            var               id      = await connection.ExecuteScalarAsync<Guid?>( command );

            return id.HasValue
                       ? record.NewID( id.Value )
                       : default;
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }
}
