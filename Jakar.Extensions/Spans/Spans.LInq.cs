// Jakar.Extensions :: Jakar.Extensions
// 08/26/2023  12:06 PM

namespace Jakar.Extensions;


public delegate TOutput RefSelect<TInput, out TOutput>( in TInput value );



[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public static partial class Spans
{
    [Pure] public static TNext[]? Select<TValue, TNext>( this scoped in ReadOnlySpan<TValue> span, RefConvert<TValue, TNext> func )
        where TNext : IEquatable<TNext>
    {
        if ( span.IsEmpty ) { return null; }

        TNext[] buffer = GC.AllocateUninitializedArray<TNext>(span.Length);
        int     index  = 0;

        foreach ( ref readonly TValue value in span ) { buffer[index++] = func(in value); }

        return buffer;
    }


    [Pure] public static ReadOnlySpan<TValue> Join<TValue>( this scoped ReadOnlySpan<TValue> first, params ReadOnlySpan<TValue> last )
    {
        int          size   = first.Length;
        TValue[]     buffer = GC.AllocateUninitializedArray<TValue>(size + last.Length);
        Span<TValue> result = buffer;
        first.CopyTo(result[..size]);
        last.CopyTo(result[size..]);
        return buffer;
    }


    extension<TValue>( scoped ReadOnlySpan<TValue> value )
        where TValue : unmanaged, IEquatable<TValue>
    {
        [Pure] public ReadOnlySpan<TValue> Replace( scoped ReadOnlySpan<TValue> oldValue, scoped ReadOnlySpan<TValue> newValue )
        {
            Buffer<TValue> buffer = new(value.Length + value.Count(oldValue) * Math.Abs(newValue.Length - oldValue.Length) + 1);

            try
            {
                value.Replace(oldValue, newValue, ref buffer);
                return buffer.ToArray();
            }
            finally { buffer.Dispose(); }
        }
        public void Replace( scoped ReadOnlySpan<TValue> oldValue, scoped ReadOnlySpan<TValue> newValue, scoped ref Buffer<TValue> buffer )
        {
            if ( !value.ContainsExact(oldValue) )
            {
                value.CopyTo(buffer.Next);
                return;
            }

            int sourceIndex = 0;

            while ( sourceIndex < value.Length )
            {
                if ( value[sourceIndex..]
                   .StartsWith(oldValue) )
                {
                    // buffer = buffer.EnsureCapacity(newValue.Length);
                    buffer.Add(newValue);
                    sourceIndex += oldValue.Length;
                }
                else { buffer.Add(value[sourceIndex++]); }
            }
        }
    }



    extension<TValue>( scoped ReadOnlySpan<TValue> span )
        where TValue : IEquatable<TValue>
    {
        [Pure] public ReadOnlySpan<TValue> Remove( TValue value )
        {
            TValue[] buffer = GC.AllocateUninitializedArray<TValue>(span.Length);
            int      index  = 0;

            foreach ( ref readonly TValue equatable in span )

            {
                if ( !equatable.Equals(value) ) { buffer[index++] = equatable; }
            }

            return new ReadOnlySpan<TValue>(buffer, 0, index);
        }
        [Pure] public ReadOnlySpan<TValue> Remove( params ReadOnlySpan<TValue> values )
        {
            TValue[] buffer = GC.AllocateUninitializedArray<TValue>(span.Length);
            int      index  = 0;

            foreach ( ref readonly TValue equatable in span )

            {
                if ( !equatable.IsOneOf(values) ) { buffer[index++] = equatable; }
            }

            return new ReadOnlySpan<TValue>(buffer, 0, index);
        }
    }



    [Pure] public static TValue First<TValue>( this scoped in Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return First(in span, selector);
    }
    [Pure] public static TValue First<TValue>( this scoped in ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( ref readonly TValue value in values )

        {
            if ( selector(value) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure] public static TValue? FirstOrDefault<TValue>( this scoped in Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return FirstOrDefault(in span, selector);
    }
    [Pure] public static TValue? FirstOrDefault<TValue>( this scoped in ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( ref readonly TValue value in values )

        {
            if ( selector(value) ) { return value; }
        }

        return default;
    }


    [Pure] public static TValue First<TValue>( this scoped in Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.First(selector);
    }
    [Pure] public static TValue First<TValue>( this scoped in ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( ref readonly TValue value in values )

        {
            if ( selector(in value) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure] public static TValue? FirstOrDefault<TValue>( this scoped in Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.FirstOrDefault(selector);
    }
    extension<TValue>( scoped in ReadOnlySpan<TValue> values )
    {
        [Pure] public TValue? FirstOrDefault( RefCheck<TValue> selector )
        {
            foreach ( ref readonly TValue value in values )

            {
                if ( selector(in value) ) { return value; }
            }

            return default;
        }
        [Pure] public TValue Last( Func<TValue, bool> predicate )
        {
            for ( int index = values.Length - 1; index >= 0; --index )
            {
                if ( predicate(values[index]) ) { return values[index]; }
            }

            throw new NotFoundException();
        }
        [Pure] public TValue? LastOrDefault( Func<TValue, bool> predicate )
        {
            for ( int index = values.Length - 1; index >= 0; --index )
            {
                if ( predicate(values[index]) ) { return values[index]; }
            }

            return default;
        }
        [Pure] public TValue Last( RefCheck<TValue> predicate )
        {
            for ( int index = values.Length - 1; index >= 0; --index )
            {
                if ( predicate(in values[index]) ) { return values[index]; }
            }

            throw new NotFoundException();
        }
        [Pure] public TValue? LastOrDefault( RefCheck<TValue> predicate )
        {
            for ( int index = values.Length - 1; index >= 0; --index )
            {
                if ( predicate(in values[index]) ) { return values[index]; }
            }

            return default;
        }
    }



    [Pure] public static bool All<TValue>( this scoped in Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.All(selector);
    }
    [Pure] public static bool All<TValue>( this scoped in ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( ref readonly TValue value in values )

        {
            if ( !selector(value) ) { return false; }
        }

        return true;
    }


    [Pure] public static bool All<TValue>( this scoped in Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.All(selector);
    }
    [Pure] public static bool All<TValue>( this scoped in ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( ref readonly TValue value in values )

        {
            if ( !selector(in value) ) { return false; }
        }

        return true;
    }


    [Pure] public static bool Any<TValue>( this scoped in Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.Any(selector);
    }
    [Pure] public static bool Any<TValue>( this scoped in ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( ref readonly TValue value in values )

        {
            if ( selector(in value) ) { return true; }
        }

        return false;
    }


    [Pure] public static TValue Single<TValue>( this scoped in Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.Single(selector);
    }
    [Pure] public static TValue Single<TValue>( this scoped in ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( ref readonly TValue value in values )

        {
            if ( selector(in value) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure] public static TValue? SingleOrDefault<TValue>( this scoped in Span<TValue> values, RefCheck<TValue> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return span.SingleOrDefault(selector);
    }
    [Pure] public static TValue? SingleOrDefault<TValue>( this scoped in ReadOnlySpan<TValue> values, RefCheck<TValue> selector )
    {
        foreach ( ref readonly TValue value in values )

        {
            if ( selector(in value) ) { return value; }
        }

        return default;
    }


    [Pure] public static TValue Single<TValue>( this scoped in Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return Single(in span, selector);
    }
    [Pure] public static TValue Single<TValue>( this scoped in ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( ref readonly TValue value in values )

        {
            if ( selector(value) ) { return value; }
        }

        throw new NotFoundException();
    }
    [Pure] public static TValue? SingleOrDefault<TValue>( this scoped in Span<TValue> values, Func<TValue, bool> selector )
    {
        ReadOnlySpan<TValue> span = values;
        return SingleOrDefault(in span, selector);
    }
    [Pure] public static TValue? SingleOrDefault<TValue>( this scoped in ReadOnlySpan<TValue> values, Func<TValue, bool> selector )
    {
        foreach ( ref readonly TValue value in values )

        {
            if ( selector(value) ) { return value; }
        }

        return default;
    }


    [Pure] public static int Count<TValue>( this scoped in Span<TValue> span, TValue value )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> temp = span;
        return temp.Count(value);
    }
    extension<TValue>( scoped in ReadOnlySpan<TValue> span )
        where TValue : IEquatable<TValue>
    {
        [Pure] public int Count( TValue value )
        {
            int result = 0;

            foreach ( ref readonly TValue v in span )

            {
                if ( v.Equals(value) ) { result++; }
            }

            return result;
        }
        [Pure] public int Count( RefCheck<TValue> check )
        {
            int result = 0;

            foreach ( ref readonly TValue v in span )

            {
                if ( check(in v) ) { result++; }
            }

            return result;
        }
    }
}
