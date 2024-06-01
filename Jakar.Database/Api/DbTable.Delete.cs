// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:06 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask Delete( Activity? activity, TRecord                             record,   CancellationToken token                               = default ) => this.TryCall( Delete, activity, record,   token );
    public ValueTask Delete( Activity? activity, IEnumerable<TRecord>                records,  CancellationToken token                               = default ) => this.TryCall( Delete, activity, records,  token );
    public ValueTask Delete( Activity? activity, IAsyncEnumerable<TRecord>           records,  CancellationToken token                               = default ) => this.TryCall( Delete, activity, records,  token );
    public ValueTask Delete( Activity? activity, RecordID<TRecord>                   id,       CancellationToken token                               = default ) => this.TryCall( Delete, activity, id,       token );
    public ValueTask Delete( Activity? activity, IEnumerable<RecordID<TRecord>>      ids,      CancellationToken token                               = default ) => this.TryCall( Delete, activity, ids,      token );
    public ValueTask Delete( Activity? activity, IAsyncEnumerable<RecordID<TRecord>> ids,      CancellationToken token                               = default ) => this.TryCall( Delete, activity, ids,      token );
    public ValueTask Delete( Activity? activity, bool                                matchAll, DynamicParameters parameters, CancellationToken token = default ) => this.TryCall( Delete, activity, matchAll, parameters, token );


    public virtual ValueTask Delete( DbConnection connection, DbTransaction transaction, Activity? activity, TRecord              record,  CancellationToken token = default ) => Delete( connection, transaction, activity, record.ID,                          token );
    public virtual ValueTask Delete( DbConnection connection, DbTransaction transaction, Activity? activity, IEnumerable<TRecord> records, CancellationToken token = default ) => Delete( connection, transaction, activity, records.Select( static x => x.ID ), token );
    public async ValueTask Delete( DbConnection connection, DbTransaction transaction, Activity? activity, IAsyncEnumerable<TRecord> records, CancellationToken token = default )
    {
        HashSet<TRecord> ids = await records.ToHashSet( token );
        await Delete( connection, transaction, activity, ids, token );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, Activity? activity, IAsyncEnumerable<RecordID<TRecord>> ids, CancellationToken token = default )
    {
        HashSet<RecordID<TRecord>> records = await ids.ToHashSet( token );
        await Delete( connection, transaction, activity, records, token );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, Activity? activity, RecordID<TRecord> id, CancellationToken token = default )
    {
        SqlCommand        sql     = _sqlCache.Delete( id );
        CommandDefinition command = _database.GetCommand( activity, sql, transaction, token );
        await connection.ExecuteScalarAsync( command );
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, Activity? activity, IEnumerable<RecordID<TRecord>> ids, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Delete( ids );

        try
        {
            CommandDefinition command = _database.GetCommand( activity, sql, transaction, token );
            await connection.ExecuteScalarAsync( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public async ValueTask Delete( DbConnection connection, DbTransaction transaction, Activity? activity, bool matchAll, DynamicParameters parameters, CancellationToken token )
    {
        SqlCommand        sql     = _sqlCache.Delete( matchAll, parameters );
        CommandDefinition command = _database.GetCommand( activity, sql, transaction, token );
        await connection.ExecuteScalarAsync( command );
    }
}
