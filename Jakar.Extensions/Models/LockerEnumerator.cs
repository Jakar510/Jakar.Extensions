// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public struct LockerEnumerator<TValue, TList> : IEnumerator<TValue>, IEnumerable<TValue>
    where TList : ILockedCollection<TValue>
{
    private const    int                    START_INDEX = -1;
    private readonly TList                  _collection;
    private          int                    _index   = START_INDEX;
    private          TValue?                _current = default;
    private          ReadOnlyMemory<TValue> _cache   = default;
    public readonly  TValue                 Current        => _current ?? throw new NullReferenceException( nameof(_current) );
    readonly         object IEnumerator.    Current        => Current  ?? throw new NullReferenceException( nameof(_current) );
    internal         bool                   ShouldContinue => ++_index < _cache.Length;


    public LockerEnumerator( TList collection ) => _collection = collection;
    public   void                                    Dispose()       => this = default;
    readonly IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => this;
    readonly IEnumerator IEnumerable.                GetEnumerator() => this;


    public bool MoveNext()
    {
        if ( _collection is null ) { throw new ObjectDisposedException( nameof(LockerEnumerator<TValue, TList>) ); }

        // ReSharper disable once InvertIf
        if ( _cache.IsEmpty )
        {
            _index = START_INDEX;
            _cache = _collection.Copy();
        }

        bool result = ILockedCollection<TValue>.MoveNext( ref _index, _cache.Span, out _current );
        if ( result is false ) { Reset(); }

        return result;
    }
    public void Reset()
    {
        if ( _collection is null ) { throw new ObjectDisposedException( nameof(LockerEnumerator<TValue, TList>) ); }
        
        _cache   = default;
        _current = default;
        Interlocked.Exchange( ref _index, START_INDEX );
    }
}
