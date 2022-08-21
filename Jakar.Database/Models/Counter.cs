// Jakar.Extensions :: Jakar.Database
// 08/21/2022  9:25 AM

using System.Collections;



namespace Jakar.Database;


[Serializable]
public sealed class Counter<TID> : IEnumerator<TID> where TID : IComparable<TID>, IEquatable<TID>
{
    private readonly Func<TID?, TID> _adder;
    private          TID?            _current;


    public TID Current
    {
        get => _current ?? throw new InvalidOperationException(nameof(_current));
        private set => _current = value;
    }
    object IEnumerator.Current => Current;


    public Counter( Func<TID?, TID> adder ) => _adder = adder;


    public bool MoveNext()
    {
        Current = _adder(_current);
        return true;
    }
    public void Reset() => _current = default;
    public void Dispose() => _current = default;
}
