// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:06 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask Delete( TRecord                             record,   CancellationToken token                               = default ) => this.TryCall( Delete, record,   token );
    public ValueTask Delete( IEnumerable<TRecord>                records,  CancellationToken token                               = default ) => this.TryCall( Delete, records,  token );
    public ValueTask Delete( IAsyncEnumerable<TRecord>           records,  CancellationToken token                               = default ) => this.TryCall( Delete, records,  token );
    public ValueTask Delete( RecordID<TRecord>                   id,       CancellationToken token                               = default ) => this.TryCall( Delete, id,       token );
    public ValueTask Delete( IEnumerable<RecordID<TRecord>>      ids,      CancellationToken token                               = default ) => this.TryCall( Delete, ids,      token );
    public ValueTask Delete( IAsyncEnumerable<RecordID<TRecord>> ids,      CancellationToken token                               = default ) => this.TryCall( Delete, ids,      token );
    public ValueTask Delete( bool                                matchAll, DynamicParameters parameters, CancellationToken token = default ) => this.TryCall( Delete, matchAll, parameters, token );


    public virtual ValueTask Delete( DbConnection connection, DbTransaction transaction, TRecord              record,  CancellationToken token = default ) => Delete( connection, transaction, record.ID,                          token );
    public virtual ValueTask Delete( DbConnection connection, DbTransaction transaction, IEnumerable<TRecord> records, CancellationToken token = default ) => Delete( connection, transaction, records.Select( static x => x.ID ), token );
    public async ValueTask Delete( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TRecord> records, CancellationToken token = default )
    {
        HashSet<TRecord> ids = await records.ToHashSet( token );
        await Delete( connection, transaction, ids, token );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<RecordID<TRecord>> ids, CancellationToken token = default )
    {
        HashSet<RecordID<TRecord>> records = await ids.ToHashSet( token );
        await Delete( connection, transaction, records, token );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, RecordID<TRecord> id, CancellationToken token = default )
    {
        SqlCommand        sql     = TRecord.SQL.DeleteID( in id );
        CommandDefinition command = _database.GetCommand( in sql, transaction, token );
        await connection.ExecuteScalarAsync( command );
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, IEnumerable<RecordID<TRecord>> ids, CancellationToken token = default )
    {
        SqlCommand sql = TRecord.SQL.Delete( ids );

        try
        {
            CommandDefinition command = _database.GetCommand( in sql, transaction, token );
            await connection.ExecuteScalarAsync( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public async ValueTask Delete( DbConnection connection, DbTransaction transaction, bool matchAll, DynamicParameters parameters, CancellationToken token )
    {
        SqlCommand        sql     = TRecord.SQL.Delete( matchAll, parameters );
        CommandDefinition command = _database.GetCommand( in sql, transaction, token );
        await connection.ExecuteScalarAsync( command );
    }
}
