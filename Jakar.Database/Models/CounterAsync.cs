// Jakar.Extensions :: Jakar.Database
// 09/15/2022  8:27 PM

namespace Jakar.Database;


[Serializable]
public record struct CounterAsync : IAsyncEnumerator<long>
{
    private long? _current;


    public long Current { get => _current ?? throw new InvalidOperationException( nameof(_current) ); private set => _current = value; }


    public CounterAsync() { }
    public void Reset() => _current = null;
    public ValueTask DisposeAsync()
    {
        _current = null;
        return ValueTask.CompletedTask;
    }


    public ValueTask<bool> MoveNextAsync() => new((++Current).IsValidID());
}
