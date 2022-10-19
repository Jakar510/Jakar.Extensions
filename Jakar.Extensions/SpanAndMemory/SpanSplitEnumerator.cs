#nullable enable
namespace Jakar.Extensions;


/// <summary>
///     <see cref = "SpanSplitEnumerator{T}" />
///     is a struct so there is no allocation here.
///     <para>
///         Must be a ref struct as it contains a
///         <see cref = "ReadOnlySpan{T}" />
///     </para>
///     <para>
///         <see href = "https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm" />
///     </para>
/// </summary>
public ref struct SpanSplitEnumerator<T> where T : IEquatable<T>
{
    private readonly ReadOnlySpan<T> _originalString;
    private          ReadOnlySpan<T> _span;
    private readonly T[]             _separators;

    public SpanSplitEnumerator( ReadOnlySpan<T> span, params T[] separators )
    {
        if ( separators.IsEmpty() ) { throw new ArgumentException( $"{nameof(separators)} cannot be empty" ); }

        _originalString = span;
        _span           = span;
        _separators     = separators;
        Current         = default;
    }


    public SpanSplitEnumerator<T> GetEnumerator() => this;

    public void Reset() => _span = _originalString;

    public bool MoveNext()
    {
        ReadOnlySpan<T> span = _span;

        // Reach the end of the string
        if ( span.Length == 0 ) { return false; }

        int index = span.IndexOfAny( _separators );

        if ( index == -1 ) // The string doesn't contain the separators
        {
            _span   = ReadOnlySpan<T>.Empty; // The remaining string is an empty string
            Current = new LineSplitEntry<T>( span, ReadOnlySpan<T>.Empty );
            return true;
        }

        for ( int i = 0; i < _separators.Length; i++ )
        {
            T c = _separators[i];

            if ( index < span.Length - 1 && span[index]
                    .Equals( c ) )
            {
                // Try to consume the char associated to the next char
                T next = span[index + 1];

                if ( i + 1 >= _separators.Length ) { continue; }

                if ( next.Equals( _separators[i + 1] ) )
                {
                    Current = new LineSplitEntry<T>( span[..index], span.Slice( index, 2 ) );
                    _span   = span[(index + 2)..];
                    return true;
                }
            }
        }

        Current = new LineSplitEntry<T>( span[..index], span.Slice( index, 1 ) );
        _span   = span[(index + 1)..];
        return true;
    }

    public LineSplitEntry<T> Current { get; private set; }
}
