// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:06 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TClass>
{
    public ValueTask Delete( TClass                             record,   CancellationToken token                               = default ) => this.TryCall( Delete, record,   token );
    public ValueTask Delete( IEnumerable<TClass>                records,  CancellationToken token                               = default ) => this.TryCall( Delete, records,  token );
    public ValueTask Delete( IAsyncEnumerable<TClass>           records,  CancellationToken token                               = default ) => this.TryCall( Delete, records,  token );
    public ValueTask Delete( RecordID<TClass>                   id,       CancellationToken token                               = default ) => this.TryCall( Delete, id,       token );
    public ValueTask Delete( IEnumerable<RecordID<TClass>>      ids,      CancellationToken token                               = default ) => this.TryCall( Delete, ids,      token );
    public ValueTask Delete( IAsyncEnumerable<RecordID<TClass>> ids,      CancellationToken token                               = default ) => this.TryCall( Delete, ids,      token );
    public ValueTask Delete( bool                                matchAll, DynamicParameters parameters, CancellationToken token = default ) => this.TryCall( Delete, matchAll, parameters, token );


    public virtual ValueTask Delete( DbConnection connection, DbTransaction transaction, TClass              record,  CancellationToken token = default ) => Delete( connection, transaction, record.ID,                          token );
    public virtual ValueTask Delete( DbConnection connection, DbTransaction transaction, IEnumerable<TClass> records, CancellationToken token = default ) => Delete( connection, transaction, records.Select( static x => x.ID ), token );
    public async ValueTask Delete( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<TClass> records, CancellationToken token = default )
    {
        HashSet<TClass> ids = await records.ToHashSet( token );
        await Delete( connection, transaction, ids, token );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, IAsyncEnumerable<RecordID<TClass>> ids, CancellationToken token = default )
    {
        HashSet<RecordID<TClass>> records = await ids.ToHashSet( token );
        await Delete( connection, transaction, records, token );
    }
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, RecordID<TClass> id, CancellationToken token = default )
    {
        SqlCommand        sql     = TClass.SQL.GetDeleteID( in id );
        CommandDefinition command = _database.GetCommand( in sql, transaction, token );
        await connection.ExecuteScalarAsync( command );
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask Delete( DbConnection connection, DbTransaction transaction, IEnumerable<RecordID<TClass>> ids, CancellationToken token = default )
    {
        SqlCommand sql = TClass.SQL.GetDelete( ids );

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
        SqlCommand        sql     = TClass.SQL.GetDelete( matchAll, parameters );
        CommandDefinition command = _database.GetCommand( in sql, transaction, token );
        await connection.ExecuteScalarAsync( command );
    }
}
