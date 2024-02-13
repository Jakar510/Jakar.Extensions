// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public struct LockerEnumerator<TValue>( ILockedCollection<TValue> collection ) : IEnumerator<TValue>, IEnumerable<TValue>
{
    private const    int                                START_INDEX = -1;
    private readonly ILockedCollection<TValue> _collection = collection;
    private          int                                _index   = START_INDEX;
    private          TValue?                            _current = default;
    private          ReadOnlyMemory<TValue>             _cache   = default;
    public readonly  TValue                             Current        => _current ?? throw new NullReferenceException( nameof(_current) );
    readonly         object IEnumerator.                Current        => Current  ?? throw new NullReferenceException( nameof(_current) );
    internal         bool                               ShouldContinue => ++_index < _cache.Length;


    public   void                                    Dispose()       => this = default;
    readonly IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => this;
    readonly IEnumerator IEnumerable.                GetEnumerator() => this;


    public bool MoveNext()
    {
        if ( _collection is null ) { throw new ObjectDisposedException( nameof(LockerEnumerator<TValue>) ); }

        // ReSharper disable once InvertIf
        if ( _cache.IsEmpty )
        {
            _index = START_INDEX;
            _cache = _collection.Copy();
        }
        
        _index += 1;

        _current = _index < _cache.Span.Length
                       ? _cache.Span[_index]
                       : default;

        bool result = _index < _cache.Span.Length;
        if ( result is false ) { Reset(); }

        return result;
    }
    public void Reset()
    {
        if ( _collection is null ) { throw new ObjectDisposedException( nameof(LockerEnumerator<TValue>) ); }

        _cache   = default;
        _current = default;
        _index   = START_INDEX;
    }
}
