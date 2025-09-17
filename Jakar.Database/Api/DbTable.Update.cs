// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:09 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TClass>
{
    public ValueTask Update( TClass                   record,  CancellationToken token = default ) => this.TryCall(Update, record,  token);
    public ValueTask Update( IEnumerable<TClass>      records, CancellationToken token = default ) => this.TryCall(Update, records, token);
    public ValueTask Update( ImmutableArray<TClass>   records, CancellationToken token = default ) => this.TryCall(Update, records, token);
    public ValueTask Update( IAsyncEnumerable<TClass> records, CancellationToken token = default ) => this.TryCall(Update, records, token);


    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, ImmutableArray<TClass> records, CancellationToken token = default )
    {
        foreach ( TClass record in records ) { await Update(connection, transaction, record, token); }
    }
    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, IEnumerable<TClass> records, CancellationToken token = default )
    {
        foreach ( TClass record in records ) { await Update(connection, transaction, record, token); }
    }


    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, IAsyncEnumerable<TClass> records, CancellationToken token = default )
    {
        await foreach ( TClass record in records.WithCancellation(token) ) { await Update(connection, transaction, record, token); }
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, TClass record, CancellationToken token = default )
    {
        SqlCommand sql = TClass.SQL.GetUpdate(record);

        try
        {
            CommandDefinition command = _database.GetCommand(in sql, transaction, token);
            await connection.ExecuteScalarAsync(command);
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }
}
