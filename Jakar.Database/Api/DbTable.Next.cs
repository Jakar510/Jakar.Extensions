// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:16 PM

namespace Jakar.Database;


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
        if ( token.IsCancellationRequested ) { return default; }

        try { return await connection.ExecuteScalarAsync<TRecord>( sql, parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<RecordPair[]> SortedIDs( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        string sql = @$"SELECT {ID_ColumnName}, {DateCreated} FROM {SchemaTableName} ORDER BY {DateCreated} DESC";


        try
        {
            if ( token.IsCancellationRequested ) { return Array.Empty<RecordPair>(); }

            IEnumerable<RecordPair> pairs = await connection.QueryAsync<RecordPair>( sql, default, transaction );
            return pairs.GetArray();
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask<Guid?> NextID( DbConnection connection, DbTransaction? transaction, Guid? id, CancellationToken token = default )
    {
        if ( id is null ) { return default; }

        if ( token.IsCancellationRequested ) { return default; }

        var parameters = new DynamicParameters();
        parameters.Add( nameof(id), id );

        string sql = @$"SELECT {ID_ColumnName} FROM {SchemaTableName} WHERE ( id = IFNULL((SELECT MIN({ID_ColumnName}) FROM {SchemaTableName} WHERE {ID_ColumnName} > @{nameof(id)}), 0) )";

        try { return await connection.ExecuteScalarAsync<Guid>( sql, parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, parameters, e ); }
    }
}
