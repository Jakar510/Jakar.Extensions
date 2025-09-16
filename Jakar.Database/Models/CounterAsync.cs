// Jakar.Extensions :: Jakar.Database
// 09/15/2022  8:27 PM

namespace Jakar.Database;


[Serializable]
public record struct CounterAsync : IAsyncEnumerator<long>
{
    private long? __current;


    public long Current { get => __current ?? throw new InvalidOperationException( nameof(__current) ); private set => __current = value; }


    public CounterAsync() { }
    public void Reset() => __current = null;
    public ValueTask DisposeAsync()
    {
        __current = null;
        return ValueTask.CompletedTask;
    }


    public ValueTask<bool> MoveNextAsync() => new((++Current).IsValidID());
}
