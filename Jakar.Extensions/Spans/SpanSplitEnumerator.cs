namespace Jakar.Extensions;


/// <summary>
///     <see cref="SpanSplitEnumerator{TValue}"/> is a struct so there is no allocation here.
///     <para> Must be a ref struct as it contains a <see cref="ReadOnlySpan{TValue}"/> </para>
///     <para>
///         <see href="https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm"/>
///     </para>
/// </summary>
public ref struct SpanSplitEnumerator<TValue>
    where TValue : unmanaged, IEquatable<TValue>
{
    private readonly ReadOnlySpan<TValue> _separators;
    private readonly ReadOnlySpan<TValue> _originalString;
    private          ReadOnlySpan<TValue> _span;


    public LineSplitEntry<TValue> Current { get; private set; }


    public SpanSplitEnumerator( scoped ref readonly ReadOnlySpan<TValue> span, scoped ref readonly ReadOnlySpan<TValue> separators )
    {
        if ( separators.IsEmpty ) { throw new ArgumentException( $"{nameof(separators)} cannot be empty" ); }

        _originalString = span;
        _span           = span;
        _separators     = separators;
        Current         = default;
    }


    public readonly override string                 ToString()      => $"{nameof(LineSplitEntry<TValue>)}({nameof(Current)}: '{Current.ToString()}', {nameof(_originalString)}: '{_originalString.ToString()}')";
    public readonly          SpanSplitEnumerator<TValue> GetEnumerator() => this;
    public                   void                   Reset()         => _span = _originalString;


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public bool MoveNext()
    {
        ReadOnlySpan<TValue> span = _span;

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
            Current = new LineSplitEntry<TValue>( span, default );
            return true;
        }

        while ( index < span.Length - 1 && _separators.Contains( span[index + 1] ) ) { index++; }

        Current = new LineSplitEntry<TValue>( span[..start], span.Slice( start, Math.Max( index - start, 1 ) ) );
        _span   = span[(index + 1)..];
        return true;
    }
}
