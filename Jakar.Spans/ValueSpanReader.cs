// Jakar.Spans
// 10/06/2026

namespace Jakar.Extensions;


/// <summary>
///     <para> A stack-only forward cursor over a <see cref="ReadOnlySpan{T}"/>: the reading counterpart of <see cref="ValueStringBuilder"/> / <see cref="ValueUtf8Builder"/>. One implementation serves UTF-16 (<see cref="char"/>) and UTF-8 (<see cref="byte"/>) text, or any other unmanaged element. </para>
///     <para> Nothing is allocated: reads return slices of the input, and the whole state is the span plus <see cref="Position"/>, so a checkpoint is just an <see cref="int"/> (<c>int mark = reader.Position; ...; reader.Rewind(mark);</c>). </para>
/// </summary>
/// <remarks>
///     <c>Try*</c> members never throw and never move the cursor when they return <see langword="false"/>.
///     <see cref="Peek()"/>, <see cref="Read()"/> and <see cref="Advance"/> throw when there isn't enough input, so a parser bug surfaces as an exception rather than as a silently wrong result.
/// </remarks>
public ref struct ValueSpanReader<T>( ReadOnlySpan<T> span )
    where T : unmanaged, IEquatable<T>
{
    private readonly ReadOnlySpan<T> __span     = span;
    private          int             __position = 0;


    /// <summary> The whole input. </summary>
    public readonly ReadOnlySpan<T> Span           { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __span; }
    /// <summary> The input's length. </summary>
    public readonly int             Length         { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __span.Length; }
    /// <summary> The offset of the next element to read (0..<see cref="Length"/>). </summary>
    public readonly int             Position       { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __position; }
    /// <summary> The elements already read (<c>[0..Position)</c>). </summary>
    public readonly ReadOnlySpan<T> Consumed       { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __span[..__position]; }
    /// <summary> The elements not yet read (<c>[Position..Length)</c>). </summary>
    public readonly ReadOnlySpan<T> Remaining      { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __span[__position..]; }
    /// <summary> The number of elements not yet read. </summary>
    public readonly int             RemainingCount { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __span.Length - __position; }
    /// <summary> Whether every element has been read. </summary>
    public readonly bool            End            { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __position >= __span.Length; }


    // ─── Peek ────────────────────────────────────────────────────────────────

    /// <summary> The next element, without consuming it. </summary>
    /// <exception cref="InvalidOperationException"> The reader is at the <see cref="End"/>. </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public readonly T Peek()
    {
        int pos = __position;
        if ( (uint)pos >= (uint)__span.Length ) { ThrowEnd(); }

        return __span[pos];
    }
    /// <summary> The next element, without consuming it; <see langword="false"/> at the <see cref="End"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly bool TryPeek( out T value )
    {
        int pos = __position;

        if ( (uint)pos < (uint)__span.Length )
        {
            value = __span[pos];
            return true;
        }

        value = default;
        return false;
    }
    /// <summary> The element <paramref name="offset"/> places after <see cref="Position"/>, without consuming anything; <see langword="false"/> if the input is too short. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly bool TryPeek( int offset, out T value )
    {
        long index = (long)__position + offset;

        if ( offset >= 0 && index < __span.Length )
        {
            value = __span[(int)index];
            return true;
        }

        value = default;
        return false;
    }
    /// <summary> Whether the remaining input starts with <paramref name="value"/>. Doesn't consume anything. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public readonly bool IsNext( T                      value ) => (uint)__position < (uint)__span.Length && __span[__position].Equals(value);
    /// <inheritdoc cref="IsNext(T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [Pure] public readonly bool IsNext( scoped ReadOnlySpan<T> value ) => Remaining.StartsWith(value);


    // ─── Read ────────────────────────────────────────────────────────────────

    /// <summary> Consumes and returns the next element. </summary>
    /// <exception cref="InvalidOperationException"> The reader is at the <see cref="End"/>. </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public T Read()
    {
        int pos = __position;
        if ( (uint)pos >= (uint)__span.Length ) { ThrowEnd(); }

        __position = pos + 1;
        return __span[pos];
    }
    /// <summary> Consumes the next element; <see langword="false"/> at the <see cref="End"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool TryRead( out T value )
    {
        int pos = __position;

        if ( (uint)pos < (uint)__span.Length )
        {
            value      = __span[pos];
            __position = pos + 1;
            return true;
        }

        value = default;
        return false;
    }
    /// <summary> Consumes the next <paramref name="count"/> elements; <see langword="false"/> (nothing consumed) if fewer remain. </summary>
    public bool TryRead( int count, out ReadOnlySpan<T> values )
    {
        if ( (uint)count <= (uint)RemainingCount )
        {
            values     =  __span.Slice(__position, count);
            __position += count;
            return true;
        }

        values = default;
        return false;
    }
    /// <summary> Consumes <paramref name="expected"/> if it is the next element; otherwise consumes nothing. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool TryReadExact( T expected )
    {
        int pos = __position;

        if ( (uint)pos < (uint)__span.Length && __span[pos].Equals(expected) )
        {
            __position = pos + 1;
            return true;
        }

        return false;
    }
    /// <summary> Consumes <paramref name="expected"/> if the remaining input starts with it; otherwise consumes nothing. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool TryReadExact( scoped ReadOnlySpan<T> expected )
    {
        if ( !Remaining.StartsWith(expected) ) { return false; }

        __position += expected.Length;
        return true;
    }
    /// <summary> Consumes everything up to the first <paramref name="delimiter"/>, returned in <paramref name="values"/>; the delimiter itself is consumed too when <paramref name="advancePastDelimiter"/>. <see langword="false"/> (nothing consumed) if there is no delimiter. </summary>
    public bool TryReadTo( T delimiter, out ReadOnlySpan<T> values, bool advancePastDelimiter = true )
    {
        int index = Remaining.IndexOf(delimiter);

        if ( index < 0 )
        {
            values = default;
            return false;
        }

        values = __span.Slice(__position, index);

        __position += advancePastDelimiter
                          ? index + 1
                          : index;

        return true;
    }
    /// <summary> Consumes everything up to the first element in <paramref name="delimiters"/>, returned in <paramref name="values"/>; the delimiter is <b> not </b> consumed (read it to see which one it was). <see langword="false"/> (nothing consumed) if there is none. </summary>
    public bool TryReadToAny( SearchValues<T> delimiters, out ReadOnlySpan<T> values )
    {
        int index = Remaining.IndexOfAny(delimiters);

        if ( index < 0 )
        {
            values = default;
            return false;
        }

        values     =  __span.Slice(__position, index);
        __position += index;
        return true;
    }


    // ─── Search / skip ───────────────────────────────────────────────────────

    /// <summary> The offset (relative to <see cref="Position"/>) of the next <paramref name="value"/>, or -1. Doesn't consume anything. </summary>
    [Pure] public readonly int IndexOf( T                        value )  => Remaining.IndexOf(value);
    /// <summary> The offset (relative to <see cref="Position"/>) of the next element in <paramref name="values"/>, or -1. Doesn't consume anything. </summary>
    [Pure] public readonly int IndexOfAny( SearchValues<T>       values ) => Remaining.IndexOfAny(values);
    /// <summary> The offset (relative to <see cref="Position"/>) of the next element <b> not </b> in <paramref name="values"/>, or -1. Doesn't consume anything. </summary>
    [Pure] public readonly int IndexOfAnyExcept( SearchValues<T> values ) => Remaining.IndexOfAnyExcept(values);


    /// <summary> Consumes elements while they equal <paramref name="value"/>; returns how many. </summary>
    public int SkipWhile( T value )
    {
        int index = Remaining.IndexOfAnyExcept(value);
        return Skip(index);
    }
    /// <summary> Consumes elements while they are in <paramref name="values"/> (e.g. whitespace); returns how many. </summary>
    public int SkipWhile( SearchValues<T> values )
    {
        int index = Remaining.IndexOfAnyExcept(values);
        return Skip(index);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] private int Skip( int index )
    {
        int skipped = index < 0
                          ? RemainingCount
                          : index;

        __position += skipped;
        return skipped;
    }


    // ─── Move ────────────────────────────────────────────────────────────────

    /// <summary> Consumes <paramref name="count"/> elements. </summary>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="count"/> is negative or more than <see cref="RemainingCount"/>. </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Advance( int count )
    {
        if ( (uint)count > (uint)RemainingCount ) { ThrowCount(count); }

        __position += count;
    }
    /// <summary> Moves back to <paramref name="position"/>, a value of <see cref="Position"/> seen earlier (a checkpoint). </summary>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="position"/> is negative or ahead of <see cref="Position"/>. </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Rewind( int position )
    {
        if ( (uint)position > (uint)__position ) { ThrowPosition(position); }

        __position = position;
    }
    /// <summary> Moves back to the start of the input. </summary>
    public void Reset() => __position = 0;


    /// <summary> The input from <paramref name="start"/> up to (not including) <paramref name="end"/>; absolute offsets, independent of <see cref="Position"/>. Typically <c>reader.Slice(mark, reader.Position)</c>. </summary>
    /// <exception cref="ArgumentOutOfRangeException"> The range isn't within the input. </exception>
    [Pure] public readonly ReadOnlySpan<T> Slice( int start, int end ) => __span[start..end];


    // ─── Throw helpers ───────────────────────────────────────────────────────

    [DoesNotReturn] private static   void ThrowEnd()                    => throw new InvalidOperationException("The reader is at the end of its input.");
    [DoesNotReturn] private readonly void ThrowCount( int    count )    => throw new ArgumentOutOfRangeException(nameof(count),    count,    $"Must be between 0 and the remaining count ({RemainingCount}).");
    [DoesNotReturn] private readonly void ThrowPosition( int position ) => throw new ArgumentOutOfRangeException(nameof(position), position, $"Must be between 0 and the current position ({__position}).");
}
