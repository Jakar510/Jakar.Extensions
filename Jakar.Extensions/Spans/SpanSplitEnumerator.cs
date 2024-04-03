namespace Jakar.Extensions;


/// <summary>
///     <see cref="SpanSplitEnumerator{T}"/> is a struct so there is no allocation here.
///     <para> Must be a ref struct as it contains a <see cref="ReadOnlySpan{T}"/> </para>
///     <para>
///         <see href="https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm"/>
///     </para>
/// </summary>
public ref struct SpanSplitEnumerator<T>
    where T : unmanaged, IEquatable<T>
{
    private readonly ReadOnlySpan<T> _separators;
    private readonly ReadOnlySpan<T> _originalString;
    private          ReadOnlySpan<T> _span;


    public LineSplitEntry<T> Current { get; private set; }


    public SpanSplitEnumerator( scoped in ReadOnlySpan<T> span, scoped in ReadOnlySpan<T> separators )
    {
        if ( separators.IsEmpty ) { throw new ArgumentException( $"{nameof(separators)} cannot be empty" ); }

        _originalString = span;
        _span           = span;
        _separators     = separators;
        Current         = default;
    }


    public readonly override string                 ToString()      => $"{nameof(LineSplitEntry<T>)}({nameof(Current)}: '{Current.ToString()}', {nameof(_originalString)}: '{_originalString.ToString()}')";
    public readonly          SpanSplitEnumerator<T> GetEnumerator() => this;
    public                   void                   Reset()         => _span = _originalString;


#if NET6_0_OR_GREATER
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
#endif
    public bool MoveNext()
    {
        ReadOnlySpan<T> span = _span;

        if ( span.IsEmpty )
        {
            Current = default;
            return false;
        }


        int start;
        int index = start = span.IndexOfAny( _separators );

        if ( index < 0 ) // The string doesn't contain the separators
        {
            _span   = default; // The remaining string is an empty string
            Current = new LineSplitEntry<T>( span, default );
            return true;
        }

        while ( index < span.Length - 1 && _separators.Contains( span[index + 1] ) ) { index++; }

        Current = new LineSplitEntry<T>( span[..start], span.Slice( start, Math.Max( index - start, 1 ) ) );
        _span   = span[(index + 1)..];
        return true;
    }
}
