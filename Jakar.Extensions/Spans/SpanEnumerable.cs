// Jakar.Extensions :: Jakar.Extensions
// 06/09/2022  11:05 PM

namespace Jakar.Extensions;


/// <summary> A <see langword="ref"/> <see langword="struct"/> that enumerates the items in a given <see cref="Span{TValue}"/> instance. </summary>
/// <typeparam name="TValue"> The type of items to enumerate. </typeparam>
/// <remarks> Initializes a new instance of the <see cref="SpanEnumerator{TValue}"/> struct. </remarks>
/// <param name="span"> The source <see cref="Span{TValue}"/> instance. </param>
[EditorBrowsable( EditorBrowsableState.Never )]
[method: MethodImpl( MethodImplOptions.AggressiveInlining )]
public ref struct SpanEnumerator<TValue>( scoped in ReadOnlySpan<TValue> span )
{
    private readonly ReadOnlySpan<TValue> __span  = span;
    private          int                  __index = NOT_FOUND;


    public readonly Item Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(__span, __index); }

    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly SpanEnumerator<TValue> GetEnumerator() => this;
    [MethodImpl(       MethodImplOptions.AggressiveInlining )] public          bool                   MoveNext()      => ++__index < __span.Length;



    /// <summary> An item from a source <see cref="Span{TValue}"/> instance. </summary>
    /// <remarks> Initializes a new instance of the <see cref="Item"/> struct. </remarks>
    /// <param name="span"> The source <see cref="Span{TValue}"/> instance. </param>
    /// <param name="index"> The current index within <paramref name="span"/> . </param>
    [EditorBrowsable( EditorBrowsableState.Never )]
    [method: MethodImpl( MethodImplOptions.AggressiveInlining )]
    public readonly ref struct Item( scoped in ReadOnlySpan<TValue> span, int index )
    {
        private readonly ReadOnlySpan<TValue> __span = span;

        public ref readonly TValue Value { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref __span[Index]; }
        public              int    Index { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = index;

        public void Deconstruct( out TValue line, out int separator )
        {
            line      = Value;
            separator = Index;
        }
    }
}
