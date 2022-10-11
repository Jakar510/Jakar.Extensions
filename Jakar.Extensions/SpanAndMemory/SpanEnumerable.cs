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

    /// <summary>
    ///     Gets the duck-typed
    ///     <see cref = "IEnumerator{T}.Current" />
    ///     property.
    /// </summary>
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
    public SpanEnumerator( in ReadOnlySpan<T> span )
    {
        _span  = span;
        _index = -1;
    }


    /// <summary>
    ///     Implements the duck-typed
    ///     <see cref = "IEnumerable{T}.GetEnumerator" />
    ///     method.
    /// </summary>
    /// <returns>
    ///     An
    ///     <see cref = "SpanEnumerator{T}" />
    ///     instance targeting the current
    ///     <see cref = "Span{T}" />
    ///     value.
    /// </returns>
    [Pure]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public readonly SpanEnumerator<T> GetEnumerator() => this;

    /// <summary>
    ///     Implements the duck-typed
    ///     <see cref = "System.Collections.IEnumerator.MoveNext" />
    ///     method.
    /// </summary>
    /// <returns>
    ///     <see langword = "true" />
    ///     whether a new element is available,
    ///     <see langword = "false" />
    ///     otherwise
    /// </returns>
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool MoveNext() => ++_index < _span.Length;



    /// <summary>
    ///     An item from a source
    ///     <see cref = "Span{T}" />
    ///     instance.
    /// </summary>
    [EditorBrowsable( EditorBrowsableState.Never )]
    public readonly ref struct Item
    {
        /// <summary>
        ///     The source
        ///     <see cref = "Span{T}" />
        ///     instance.
        /// </summary>
        private readonly ReadOnlySpan<T> _span;

        /// <summary>
        ///     The current index within
        ///     <see cref = "_span" />
        ///     .
        /// </summary>
        private readonly int _index;

        /// <summary>
        ///     Gets the reference to the current value.
        /// </summary>
        public ref T Value
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get
            {
                ref T r0 = ref MemoryMarshal.GetReference( _span );
                ref T ri = ref Unsafe.Add( ref r0, (nint)(uint)_index );

                return ref ri;
            }
        }

        /// <summary>
        ///     Gets the current index.
        /// </summary>
        public int Index
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get
            {
            #if SPAN_RUNTIME_SUPPORT
                    return this.span.Length;
            #else
                return _index;
            #endif
            }
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
        public Item( in ReadOnlySpan<T> span, int index )
        {
            _span  = span;
            _index = index;
        }
    }
}
