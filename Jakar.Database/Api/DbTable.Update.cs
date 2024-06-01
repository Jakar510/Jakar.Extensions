// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:09 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public partial class DbTable<TRecord>
{
    public ValueTask Update( Activity? activity, TRecord                   record,  CancellationToken token = default ) => this.TryCall( Update, activity, record,  token );
    public ValueTask Update( Activity? activity, IEnumerable<TRecord>      records, CancellationToken token = default ) => this.TryCall( Update, activity, records, token );
    public ValueTask Update( Activity? activity, ImmutableArray<TRecord>   records, CancellationToken token = default ) => this.TryCall( Update, activity, records, token );
    public ValueTask Update( Activity? activity, IAsyncEnumerable<TRecord> records, CancellationToken token = default ) => this.TryCall( Update, activity, records, token );


    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, Activity? activity, ImmutableArray<TRecord> records, CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { await Update( connection, transaction, activity, record, token ); }
    }
    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, Activity? activity, IEnumerable<TRecord> records, CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { await Update( connection, transaction, activity, record, token ); }
    }


    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, Activity? activity, IAsyncEnumerable<TRecord> records, CancellationToken token = default )
    {
        await foreach ( TRecord record in records.WithCancellation( token ) ) { await Update( connection, transaction, activity, record, token ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, Activity? activity, TRecord record, CancellationToken token = default )
    {
        SqlCommand sql = _sqlCache.Update( record );

        try
        {
            CommandDefinition command = _database.GetCommand( activity, sql, transaction, token );
            await connection.ExecuteScalarAsync( command );
        }
        catch ( Exception e ) { throw new SqlException( sql, e ); }
    }
}
