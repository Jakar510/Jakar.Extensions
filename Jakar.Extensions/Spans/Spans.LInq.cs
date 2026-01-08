// Jakar.Extensions :: Jakar.Extensions
// 08/26/2023  12:06 PM

using ZLinq;
using ZLinq.Linq;
using static Jakar.Extensions.Constants;



namespace Jakar.Extensions;


public delegate TOutput RefSelect<TInput, out TOutput>( in TInput value );



[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public static partial class Spans
{
    [Pure] public static ReadOnlySpan<TValue> Join<TValue>( this scoped ReadOnlySpan<TValue> first, params ReadOnlySpan<TValue> last )
    {
        int          size   = first.Length;
        TValue[]     buffer = GC.AllocateUninitializedArray<TValue>(size + last.Length);
        Span<TValue> result = buffer;
        first.CopyTo(result[..size]);
        last.CopyTo(result[size..]);
        return buffer;
    }



    extension<TValue>( scoped in ReadOnlySpan<TValue> self )
        where TValue : unmanaged, IEquatable<TValue>
    {
        [Pure] public ReadOnlySpan<TValue> Replace( scoped ReadOnlySpan<TValue> oldValue, scoped ReadOnlySpan<TValue> newValue )
        {
            Buffer<TValue> buffer = new(self.Length + self.Count(oldValue) * Math.Abs(newValue.Length - oldValue.Length) + 1);

            try
            {
                self.Replace(oldValue, newValue, ref buffer);
                return buffer.ToArray();
            }
            finally { buffer.Dispose(); }
        }

        public void Replace( scoped ReadOnlySpan<TValue> oldValue, scoped ReadOnlySpan<TValue> newValue, scoped ref Buffer<TValue> buffer )
        {
            if ( !self.ContainsExact(oldValue) )
            {
                self.CopyTo(buffer.Next);
                return;
            }

            int sourceIndex = 0;

            while ( sourceIndex < self.Length )
            {
                if ( self[sourceIndex..]
                   .StartsWith(oldValue) )
                {
                    // buffer = buffer.EnsureCapacity(newValue.Length);
                    buffer.Add(newValue);
                    sourceIndex += oldValue.Length;
                }
                else { buffer.Add(self[sourceIndex++]); }
            }
        }

        [Pure] [MustDisposeResource] public ArrayBuffer<TValue> Remove( TValue value )
        {
            ArrayBuffer<TValue> buffer = new(self.Length);

            foreach ( ref readonly TValue x in self )
            {
                if ( !x.Equals(value) ) { buffer.Add(value); }
            }

            return buffer;
        }

        [Pure] [MustDisposeResource] public ArrayBuffer<TValue> Remove( params ReadOnlySpan<TValue> values )
        {
            ArrayBuffer<TValue> buffer = new(self.Length);

            foreach ( ref readonly TValue x in self )
            {
                if ( !x.IsOneOf(values) ) { buffer.Add(x); }
            }

            return buffer;
        }
    }



    extension<TValue>( scoped in Span<TValue> self )
    {
        [Pure] public bool All( Func<TValue, bool> selector )
        {
            ReadOnlySpan<TValue> span = self;
            return span.All(selector);
        }
        [Pure] public bool All( RefCheck<TValue> selector )
        {
            ReadOnlySpan<TValue> span = self;
            return span.All(selector);
        }
        [Pure] public bool Any( RefCheck<TValue> selector )
        {
            ReadOnlySpan<TValue> span = self;
            return span.Any(selector);
        }
        [Pure] public TValue Single( RefCheck<TValue> selector )
        {
            ReadOnlySpan<TValue> span = self;
            return span.Single(selector);
        }
        [Pure] public TValue? SingleOrDefault( RefCheck<TValue> selector )
        {
            ReadOnlySpan<TValue> span = self;
            return span.SingleOrDefault(selector);
        }
        [Pure] public TValue Single( Func<TValue, bool> selector )
        {
            ReadOnlySpan<TValue> span = self;
            return span.Single(selector);
        }
        [Pure] public TValue? SingleOrDefault( Func<TValue, bool> selector )
        {
            ReadOnlySpan<TValue> span = self;
            return span.SingleOrDefault(selector);
        }
    }



    extension<TValue>( scoped in Span<TValue> self )
    {
        [Pure] public TValue First( Func<TValue, bool> selector )
        {
            ReadOnlySpan<TValue> span = self;
            return span.First(selector);
        }
        [Pure] public TValue? FirstOrDefault( Func<TValue, bool> selector )
        {
            ReadOnlySpan<TValue> span = self;
            return span.FirstOrDefault(selector);
        }
        [Pure] public TValue First( RefCheck<TValue> selector )
        {
            ReadOnlySpan<TValue> span = self;
            return span.First(selector);
        }
        [Pure] public TValue? FirstOrDefault( RefCheck<TValue> selector )
        {
            ReadOnlySpan<TValue> span = self;
            return span.FirstOrDefault(selector);
        }
    }



    extension<TValue>( scoped in ReadOnlySpan<TValue> self )
    {
        [Pure] public TValue? FirstOrDefault( RefCheck<TValue> selector )
        {
            foreach ( ref readonly TValue value in self )
            {
                if ( selector(in value) ) { return value; }
            }

            return default;
        }

        [Pure] public TValue Last( Func<TValue, bool> predicate )
        {
            for ( int index = self.Length - 1; index >= 0; --index )
            {
                if ( predicate(self[index]) ) { return self[index]; }
            }

            throw new NotFoundException();
        }

        [Pure] public TValue? LastOrDefault( Func<TValue, bool> predicate )
        {
            for ( int index = self.Length - 1; index >= 0; --index )
            {
                if ( predicate(self[index]) ) { return self[index]; }
            }

            return default;
        }

        [Pure] public TValue Last( RefCheck<TValue> predicate )
        {
            for ( int index = self.Length - 1; index >= 0; --index )
            {
                if ( predicate(in self[index]) ) { return self[index]; }
            }

            throw new NotFoundException();
        }

        [Pure] public TValue? LastOrDefault( RefCheck<TValue> predicate )
        {
            for ( int index = self.Length - 1; index >= 0; --index )
            {
                if ( predicate(in self[index]) ) { return self[index]; }
            }

            return default;
        }

        [Pure] public TNext[]? Select<TNext>( RefConvert<TValue, TNext> func )
            where TNext : IEquatable<TNext>
        {
            if ( self.IsEmpty ) { return null; }

            TNext[] buffer = GC.AllocateUninitializedArray<TNext>(self.Length);
            int     index  = 0;

            foreach ( ref readonly TValue value in self ) { buffer[index++] = func(in value); }

            return buffer;
        }

        [Pure] public ValueEnumerable<Select<FromSpan<TValue>, TValue, TNext>, TNext> Select<TNext>( Func<TValue, TNext> func )
            where TNext : IEquatable<TNext> => self.AsValueEnumerable()
                                                   .Select(func);
        

        [Pure] public bool All( Func<TValue, bool> selector ) => self.AsValueEnumerable()
                                                                     .All(selector);

        [Pure] public bool All( RefCheck<TValue> selector )
        {
            foreach ( ref readonly TValue value in self )
            {
                if ( !selector(in value) ) { return false; }
            }

            return true;
        }

        [Pure] public bool Any( Func<TValue, bool> selector )
        {
            foreach ( ref readonly TValue value in self )
            {
                if ( selector(value) ) { return true; }
            }

            return false;
        }

        [Pure] public bool Any( RefCheck<TValue> selector )
        {
            foreach ( ref readonly TValue value in self )
            {
                if ( selector(in value) ) { return true; }
            }

            return false;
        }


        [Pure] public TValue First( Func<TValue, bool> selector ) => self.AsValueEnumerable()
                                                                         .First(selector);

        [Pure] public TValue? FirstOrDefault( Func<TValue, bool> selector ) => self.AsValueEnumerable()
                                                                                   .FirstOrDefault(selector);

        [Pure] public TValue First( RefCheck<TValue> selector )
        {
            foreach ( ref readonly TValue value in self )
            {
                if ( selector(in value) ) { return value; }
            }

            throw new NotFoundException();
        }


        [Pure] public TValue Single( RefCheck<TValue> selector )
        {
            foreach ( ref readonly TValue value in self )
            {
                if ( selector(in value) ) { return value; }
            }

            throw new NotFoundException();
        }

        [Pure] public TValue? SingleOrDefault( RefCheck<TValue> selector )
        {
            foreach ( ref readonly TValue value in self )
            {
                if ( selector(in value) ) { return value; }
            }

            return default;
        }

        [Pure] public TValue Single( Func<TValue, bool> selector ) => self.AsValueEnumerable()
                                                                          .Single(selector);

        [Pure] public TValue? SingleOrDefault( Func<TValue, bool> selector ) => self.AsValueEnumerable()
                                                                                    .SingleOrDefault(selector);
    }



    [Pure] public static int Count<TValue>( this scoped in Span<TValue> self, TValue value )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> span = self;
        return span.Count(value);
    }



    extension<TValue>( scoped in ReadOnlySpan<TValue> self )
        where TValue : IEquatable<TValue>
    {
        [Pure] public int Count( TValue value )
        {
            int result = 0;

            foreach ( ref readonly TValue v in self )
            {
                if ( v.Equals(value) ) { result++; }
            }

            return result;
        }
        [Pure] public int Count( RefCheck<TValue> check )
        {
            int result = 0;

            foreach ( ref readonly TValue v in self )
            {
                if ( check(in v) ) { result++; }
            }

            return result;
        }
    }
}
