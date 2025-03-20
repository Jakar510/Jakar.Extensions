// Jakar.Extensions :: Jakar.Extensions
// 11/22/2023  9:56 AM

namespace Jakar.Extensions;


[method: MethodImpl( MethodImplOptions.AggressiveInlining )]
public ref struct EnumerateEnumerator<T>( scoped ref readonly ReadOnlySpan<T> span )
{
    private readonly ReadOnlySpan<T> _buffer = span;
    private          int             _index  = 0;

    public (int Index, T Value) Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; private set; } = default;

    public EnumerateEnumerator( int startIndex, scoped ref readonly ReadOnlySpan<T> span ) : this( in span ) { _index = startIndex; }

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly EnumerateEnumerator<T> GetEnumerator() => this;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool MoveNext()
    {
        int index = int.CreateChecked( _index );

        if ( index >= _buffer.Length )
        {
            _index = 0;
            return false;
        }

        Current = (_index, _buffer[index]);
        _index++;
        return true;
    }
}
