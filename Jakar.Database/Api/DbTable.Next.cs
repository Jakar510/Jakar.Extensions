﻿// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:16 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TRecord>
{
    public ValueTask<TRecord?> Next( RecordPair                 pair, CancellationToken token = default ) => this.Call( Next,   pair, token );
    public ValueTask<Guid?> NextID( Guid?                       id,   CancellationToken token = default ) => this.Call( NextID, id,   token );
    public ValueTask<RecordPair[]> SortedIDs( CancellationToken token = default ) => this.Call( SortedIDs, token );


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<TRecord?> Next( DbConnection connection, DbTransaction? transaction, RecordPair pair, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(RecordPair.ID),          pair.ID );
        parameters.Add( nameof(RecordPair.DateCreated), pair.DateCreated );

        string sql = @$"SELECT * FROM {SchemaTableName} WHERE ( id = IFNULL((SELECT MIN({ID_ColumnName}) FROM {SchemaTableName} WHERE {ID_ColumnName} > @{nameof(RecordPair.ID)}), 0) )";
        
        try
        {
            CommandDefinition command = GetCommandDefinition( sql, parameters, transaction, token );
            return await connection.ExecuteScalarAsync<TRecord>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<RecordPair[]> SortedIDs( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = @$"SELECT {ID_ColumnName}, {DateCreated} FROM {SchemaTableName} ORDER BY {DateCreated} DESC";
        
        try
        {
            CommandDefinition       command = GetCommandDefinition( sql, default, transaction, token );
            IEnumerable<RecordPair> pairs   = await connection.QueryAsync<RecordPair>( command );
            return pairs.GetArray();
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<Guid?> NextID( DbConnection connection, DbTransaction? transaction, Guid? id, CancellationToken token = default )
    {
        if ( id is null ) { return default; }
        
        var parameters = new DynamicParameters();
        parameters.Add( nameof(id), id );

        string sql = @$"SELECT {ID_ColumnName} FROM {SchemaTableName} WHERE ( id = IFNULL((SELECT MIN({ID_ColumnName}) FROM {SchemaTableName} WHERE {ID_ColumnName} > @{nameof(id)}), 0) )";

        try
        {
            CommandDefinition command = GetCommandDefinition( sql, parameters, transaction, token );
            return await connection.ExecuteScalarAsync<Guid>( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }
}
