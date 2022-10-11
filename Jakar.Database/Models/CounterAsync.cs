// Jakar.Extensions :: Jakar.Database
// 09/15/2022  8:27 PM

namespace Jakar.Database;


[Serializable]
public sealed class CounterAsync<TID> : IAsyncEnumerator<TID> where TID : struct, IComparable<TID>, IEquatable<TID>
{
    private readonly Func<TID?, TID> _adder;
    private          TID?            _current;


    public TID Current
    {
        get => _current ?? throw new InvalidOperationException( nameof(_current) );
        private set => _current = value;
    }


    public CounterAsync( Func<TID?, TID> adder ) => _adder = adder;
    public void Reset() => _current = default;
    public ValueTask DisposeAsync()
    {
        _current = default;
        return ValueTask.CompletedTask;
    }


    public ValueTask<bool> MoveNextAsync()
    {
        Current = _adder( _current );
        return true.ValueTaskFromResult();
    }
}
