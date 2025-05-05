// Jakar.Extensions :: Jakar.Extensions
// 11/22/2023  9:56 AM

namespace Jakar.Extensions;


[method: MethodImpl( MethodImplOptions.AggressiveInlining )]
public ref struct EnumerateEnumerator<TValue>( ReadOnlySpan<TValue> span )
{
    private readonly ReadOnlySpan<TValue> _buffer = span;
    private          int                  _index  = 0;

    public (int Index, TValue Value) Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; private set; } = default;

    public EnumerateEnumerator( int startIndex, ReadOnlySpan<TValue> span ) : this( span ) => _index = startIndex;

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly EnumerateEnumerator<TValue> GetEnumerator() => this;


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
