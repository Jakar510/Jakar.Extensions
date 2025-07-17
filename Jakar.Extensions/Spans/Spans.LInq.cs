// Jakar.Extensions :: Jakar.Extensions
// 08/26/2023  12:06 PM

namespace Jakar.Extensions;


public delegate TOutput RefSelect<TInput, out TOutput>( in TInput value );



[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public static partial class Spans
{
    [Pure]
    public static TNext[]? Select<TValue, TNext>( this scoped ref readonly ReadOnlySpan<TValue> span, RefConvert<TValue, TNext> func )
        where TNext : IEquatable<TNext>
    {
        if ( span.IsEmpty ) { return null; }

        TNext[] buffer = GC.AllocateUninitializedArray<TNext>(span.Length);
        int     index  = 0;

        foreach ( ref readonly TValue value in span ) { buffer[index++] = func(in value); }

        return buffer;
    }


    [Pure]
    public static ReadOnlySpan<TValue> Join<TValue>( this scoped ReadOnlySpan<TValue> first, params ReadOnlySpan<TValue> last )
    {
        int          size   = first.Length;
        TValue[]     buffer = GC.AllocateUninitializedArray<TValue>(size + last.Length);
        Span<TValue> result = buffer;
        first.CopyTo(result[..size]);
        last.CopyTo(result[size..]);
        return buffer;
    }


    [Pure]
    public static ReadOnlySpan<TValue> Replace<TValue>( this scoped ReadOnlySpan<TValue> value, scoped ReadOnlySpan<TValue> oldValue, scoped ReadOnlySpan<TValue> newValue )
        where TValue : unmanaged, IEquatable<TValue>
    {
        Buffer<TValue> buffer = new(value.Length + value.Count(oldValue) * Math.Abs(newValue.Length - oldValue.Length) + 1);

        try
        {
            value.Replace(oldValue, newValue, ref buffer);
            return buffer.ToArray();
        }
        finally { buffer.Dispose(); }
    }
    public static void Replace<TValue>( this scoped ReadOnlySpan<TValue> source, scoped ReadOnlySpan<TValue> oldValue, scoped ReadOnlySpan<TValue> newValue, scoped ref Buffer<TValue> buffer )
        where TValue : unmanaged, IEquatable<TValue>
    {
        if ( source.ContainsExact(oldValue) is false )
        {
            source.CopyTo(buffer.Next);
            return;
        }

        int sourceIndex = 0;

        while ( sourceIndex < source.Length )
        {
            if ( source[sourceIndex..].StartsWith(oldValue) )
            {
                // buffer = buffer.EnsureCapacity(newValue.Length);
                buffer.Add(newValue);
                sourceIndex += oldValue.Length;
            }
            else { buffer.Add(source[sourceIndex++]); }
        }
    }


    [Pure]
    public static ReadOnlySpan<TValue> Remove<TValue>( this scoped ReadOnlySpan<TValue> span, TValue value )
        where TValue : IEquatable<TValue>
    {
        TValue[] buffer = GC.AllocateUninitializedArray<TValue>(span.Length);
        int      index  = 0;

        foreach ( ref readonly TValue equatable in span )
        {
            if ( equatable.Equals(value) is false ) { buffer[index++] = equatable; }
        }

        return new ReadOnlySpan<TValue>(buffer, 0, index);
    }


    [Pure]
    public static ReadOnlySpan<TValue> Remove<TValue>( this scoped ReadOnlySpan<TValue> span, params ReadOnlySpan<TValue> values )
        where TValue : IEquatable<TValue>
    {
        TValue[] buffer = GC.AllocateUninitializedArray<TValue>(span.Length);
        int      index  = 0;

        foreach ( ref readonly TValue equatable in span )
        {
            if ( equatable.IsOneOf(values) is false ) { buffer[index++] = equatable; }
        }

        return new ReadOnlySpan<TValue>(buffer, 0, index);
    }


    [Pure]
    public static TValue First<TValue>( this scoped ref readonly Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return First(in span, selector);
    }
    [Pure]
    public static TValue First<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( ref readonly TValue value in values )
        {
            if ( selector(value) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure]
    public static TValue? FirstOrDefault<TValue>( this scoped ref readonly Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return FirstOrDefault(in span, selector);
    }
    [Pure]
    public static TValue? FirstOrDefault<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( ref readonly TValue value in values )
        {
            if ( selector(value) ) { return value; }
        }

        return default;
    }


    [Pure]
    public static TValue First<TValue>( this scoped ref readonly Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.First(selector);
    }
    [Pure]
    public static TValue First<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( ref readonly TValue value in values )
        {
            if ( selector(in value) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure]
    public static TValue? FirstOrDefault<TValue>( this scoped ref readonly Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.FirstOrDefault(selector);
    }
    [Pure]
    public static TValue? FirstOrDefault<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( ref readonly TValue value in values )
        {
            if ( selector(in value) ) { return value; }
        }

        return default;
    }


    [Pure]
    public static TValue Last<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, Func<TValue, bool> predicate )
    {
        for ( int index = span.Length - 1; index >= 0; --index )
        {
            if ( predicate(span[index]) ) { return span[index]; }
        }

        throw new NotFoundException();
    }
    [Pure]
    public static TValue? LastOrDefault<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, Func<TValue, bool> predicate )
    {
        for ( int index = span.Length - 1; index >= 0; --index )
        {
            if ( predicate(span[index]) ) { return span[index]; }
        }

        return default;
    }


    [Pure]
    public static TValue Last<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, RefCheck<TValue> predicate )
    {
        for ( int index = span.Length - 1; index >= 0; --index )
        {
            if ( predicate(in span[index]) ) { return span[index]; }
        }

        throw new NotFoundException();
    }
    [Pure]
    public static TValue? LastOrDefault<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, RefCheck<TValue> predicate )
    {
        for ( int index = span.Length - 1; index >= 0; --index )
        {
            if ( predicate(in span[index]) ) { return span[index]; }
        }

        return default;
    }


    [Pure]
    public static bool All<TValue>( this scoped ref readonly Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.All(selector);
    }
    [Pure]
    public static bool All<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( ref readonly TValue value in values )
        {
            if ( selector(value) is false ) { return false; }
        }

        return true;
    }


    [Pure]
    public static bool All<TValue>( this scoped ref readonly Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.All(selector);
    }
    [Pure]
    public static bool All<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( ref readonly TValue value in values )
        {
            if ( selector(in value) is false ) { return false; }
        }

        return true;
    }


    [Pure]
    public static bool Any<TValue>( this scoped ref readonly Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.Any(selector);
    }
    [Pure]
    public static bool Any<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( ref readonly TValue value in values )
        {
            if ( selector(in value) ) { return true; }
        }

        return false;
    }


    [Pure]
    public static TValue Single<TValue>( this scoped ref readonly Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.Single(selector);
    }
    [Pure]
    public static TValue Single<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( ref readonly TValue value in values )
        {
            if ( selector(in value) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure]
    public static TValue? SingleOrDefault<TValue>( this scoped ref readonly Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.SingleOrDefault(selector);
    }
    [Pure]
    public static TValue? SingleOrDefault<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( ref readonly TValue value in values )
        {
            if ( selector(in value) ) { return value; }
        }

        return default;
    }


    [Pure]
    public static TValue Single<TValue>( this scoped ref readonly Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return Single(in span, selector);
    }
    [Pure]
    public static TValue Single<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( ref readonly TValue value in values )
        {
            if ( selector(value) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure]
    public static TValue? SingleOrDefault<TValue>( this scoped ref readonly Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return SingleOrDefault(in span, selector);
    }
    [Pure]
    public static TValue? SingleOrDefault<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( ref readonly TValue value in values )
        {
            if ( selector(value) ) { return value; }
        }

        return default;
    }


    [Pure]
    public static int Count<TValue>( this scoped ref readonly Span<TValue> span, TValue value )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> temp = span;
        return temp.Count(value);
    }
    [Pure]
    public static int Count<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, TValue value )
        where TValue : IEquatable<TValue>
    {
        int result = 0;

        foreach ( ref readonly TValue v in span )
        {
            if ( v.Equals(value) ) { result++; }
        }

        return result;
    }
    [Pure]
    public static int Count<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, RefCheck<TValue> check )
        where TValue : IEquatable<TValue>
    {
        int result = 0;

        foreach ( ref readonly TValue v in span )
        {
            if ( check(in v) ) { result++; }
        }

        return result;
    }
}
