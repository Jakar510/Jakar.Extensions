// Jakar.Extensions :: Jakar.Extensions
// 06/09/2022  11:05 PM

namespace Jakar.Extensions;


/// <summary>
///     A
///     <see langword = "ref" />
///     <see langword = "struct" />
///     that enumerates the items in a given
///     <see cref = "Span{T}" />
///     instance.
/// </summary>
/// <typeparam name = "T" > The type of items to enumerate. </typeparam>
[EditorBrowsable( EditorBrowsableState.Never )]
public ref struct SpanEnumerator<T>
{
    private readonly ReadOnlySpan<T> _span;
    private          int             _index;


    public readonly Item Current
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(_span, _index);
    }


    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref = "SpanEnumerator{T}" />
    ///     struct.
    /// </summary>
    /// <param name = "span" >
    ///     The source
    ///     <see cref = "Span{T}" />
    ///     instance.
    /// </param>
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public SpanEnumerator( ReadOnlySpan<T> span )
    {
        _span  = span;
        _index = -1;
    }


    [Pure] [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly SpanEnumerator<T> GetEnumerator() => this;

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool MoveNext() => ++_index < _span.Length;



    /// <summary>
    ///     An item from a source
    ///     <see cref = "Span{T}" />
    ///     instance.
    /// </summary>
    [EditorBrowsable( EditorBrowsableState.Never )]
    public readonly ref struct Item
    {
        private readonly ReadOnlySpan<T> _span;
        private readonly int             _index;


        public ref T Value
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref Unsafe.Add( ref MemoryMarshal.GetReference( _span ), (nint)(uint)_index );
        }

        public int Index
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _index;
        }


        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref = "Item" />
        ///     struct.
        /// </summary>
        /// <param name = "span" >
        ///     The source
        ///     <see cref = "Span{T}" />
        ///     instance.
        /// </param>
        /// <param name = "index" >
        ///     The current index within
        ///     <paramref name = "span" />
        ///     .
        /// </param>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public Item( ReadOnlySpan<T> span, int index )
        {
            _span  = span;
            _index = index;
        }


        public void Deconstruct( out T line, out int separator )
        {
            line      = Value;
            separator = Index;
        }
    }
}
