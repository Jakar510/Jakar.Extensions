// Jakar.Extensions :: Jakar.Extensions
// 06/09/2022  11:05 PM

namespace Jakar.Extensions;


/// <summary> A <see langword="ref"/> <see langword="struct"/> that enumerates the items in a given <see cref="Span{T}"/> instance. </summary>
/// <typeparam name="T"> The type of items to enumerate. </typeparam>
/// <remarks> Initializes a new instance of the <see cref="SpanEnumerator{T}"/> struct. </remarks>
/// <param name="span"> The source <see cref="Span{T}"/> instance. </param>
[EditorBrowsable( EditorBrowsableState.Never )]
[method: MethodImpl( MethodImplOptions.AggressiveInlining )]
public ref struct SpanEnumerator<T>( scoped in ReadOnlySpan<T> span )
{
    private readonly ReadOnlySpan<T> _span  = span;
    private          int             _index = -1;


    public readonly Item Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(_span, _index); }

    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly SpanEnumerator<T> GetEnumerator() => this;
    [MethodImpl(       MethodImplOptions.AggressiveInlining )] public          bool              MoveNext()      => ++_index < _span.Length;



    /// <summary> An item from a source <see cref="Span{T}"/> instance. </summary>
    /// <remarks> Initializes a new instance of the <see cref="Item"/> struct. </remarks>
    /// <param name="span"> The source <see cref="Span{T}"/> instance. </param>
    /// <param name="index"> The current index within <paramref name="span"/> . </param>
    [EditorBrowsable( EditorBrowsableState.Never )]
    [method: MethodImpl( MethodImplOptions.AggressiveInlining )]
    public readonly ref struct Item( scoped in ReadOnlySpan<T> span, int index )
    {
        private readonly ReadOnlySpan<T> _span = span;

        public ref readonly T   Value { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _span[Index]; }
        public              int Index { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = index;

        public void Deconstruct( out T line, out int separator )
        {
            line      = Value;
            separator = Index;
        }
    }
}
