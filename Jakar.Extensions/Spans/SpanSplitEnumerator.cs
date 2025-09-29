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
    private readonly ReadOnlySpan<TValue> __separators;
    private readonly ReadOnlySpan<TValue> __originalString;
    private          ReadOnlySpan<TValue> __span;


    public LineSplitEntry<TValue> Current { get; private set; }


    public SpanSplitEnumerator( ReadOnlySpan<TValue> span, params TValue[] separators )
    {
        if ( separators.Length == 0 ) { throw new ArgumentException( $"{nameof(separators)} cannot be empty" ); }

        __originalString = span;
        __span           = span;
        __separators     = separators;
        Current         = default;
    }


    public readonly override string                      ToString()      => $"{nameof(LineSplitEntry<TValue>)}({nameof(Current)}: '{Current.ToString()}', {nameof(__originalString)}: '{__originalString.ToString()}')";
    public readonly          SpanSplitEnumerator<TValue> GetEnumerator() => this;
    public                   void                        Reset()         => __span = __originalString;


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public bool MoveNext()
    {
        ReadOnlySpan<TValue> span = __span;

        if ( span.IsEmpty )
        {
            Current = default;
            return false;
        }


        int start;
        int index = start = span.IndexOfAny( __separators );

        if ( index < 0 ) // The string doesn't contain the separators
        {
            __span   = default; // The remaining string is an empty string
            Current = new LineSplitEntry<TValue>( span, default );
            return true;
        }

        while ( index < span.Length - 1 && __separators.Contains( span[index + 1] ) ) { index++; }

        Current = new LineSplitEntry<TValue>( span[..start], span.Slice( start, Math.Max( index - start, 1 ) ) );
        __span   = span[(index + 1)..];
        return true;
    }
}
