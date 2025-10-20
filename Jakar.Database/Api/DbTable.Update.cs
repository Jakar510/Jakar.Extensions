// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:09 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf>
{
    public ValueTask Update( TSelf                   record,  CancellationToken token = default ) => this.TryCall(Update, record,  token);
    public ValueTask Update( IEnumerable<TSelf>      records, CancellationToken token = default ) => this.TryCall(Update, records, token);
    public ValueTask Update( ImmutableArray<TSelf>   records, CancellationToken token = default ) => this.TryCall(Update, records, token);
    public ValueTask Update( IAsyncEnumerable<TSelf> records, CancellationToken token = default ) => this.TryCall(Update, records, token);


    public virtual async ValueTask Update( NpgsqlConnection connection, NpgsqlTransaction? transaction, ImmutableArray<TSelf> records, CancellationToken token = default )
    {
        foreach ( TSelf record in records ) { await Update(connection, transaction, record, token); }
    }
    public virtual async ValueTask Update( NpgsqlConnection connection, NpgsqlTransaction? transaction, IEnumerable<TSelf> records, CancellationToken token = default )
    {
        foreach ( TSelf record in records ) { await Update(connection, transaction, record, token); }
    }


    public virtual async ValueTask Update( NpgsqlConnection connection, NpgsqlTransaction? transaction, IAsyncEnumerable<TSelf> records, CancellationToken token = default )
    {
        await foreach ( TSelf record in records.WithCancellation(token) ) { await Update(connection, transaction, record, token); }
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)] public virtual async ValueTask Update( NpgsqlConnection connection, NpgsqlTransaction? transaction, TSelf record, CancellationToken token = default )
    {
        SqlCommand<TSelf> command = SqlCommand<TSelf>.GetUpdate(record);

        try
        {
            await using NpgsqlCommand cmd = command.ToCommand(connection, transaction);
            await cmd.ExecuteNonQueryAsync(token);
        }
        catch ( Exception e ) { throw new SqlException<TSelf>(command, e); }
    }
}
