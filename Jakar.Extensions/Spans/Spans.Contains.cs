// Jakar.Extensions :: Jakar.Extensions
// 06/10/2022  10:17 AM

namespace Jakar.Extensions;


public static partial class Spans
{
    // https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7/#:~:text=also%20add%20a-,Vector256,-%3CT%3E
    public static bool Contains<TValue>( scoped in ReadOnlySpan<TValue> span, TValue value )
        where TValue : IEquatable<TValue>
    {
        if ( Vector.IsHardwareAccelerated && Vector<TValue>.IsSupported && span.Length >= Vector<TValue>.Count )
        {
            Vector<TValue> source = Vector.Create(span);
            return Vector.EqualsAll(source, Vector.Create(value));
        }

        return span.Contains(value);
    }


    public static bool Contains( scoped in Span<char>         span, params ReadOnlySpan<char> value ) => span.Contains(value, StringComparison.Ordinal);
    public static bool Contains( scoped in ReadOnlySpan<char> span, params ReadOnlySpan<char> value ) => span.Contains(value, StringComparison.Ordinal);



    extension<TValue>( scoped in Span<TValue> self )
        where TValue : IEquatable<TValue>
    {
        public bool ContainsExact( params ReadOnlySpan<TValue> value )
        {
            if ( value.Length > self.Length ) { return false; }

            if ( value.Length == self.Length ) { return self.SequenceEqual(value); }

            for ( int i = 0; i < self.Length || i + value.Length < self.Length; i++ )
            {
                if ( self.Slice(i, value.Length)
                         .SequenceEqual(value) ) { return true; }
            }

            return false;
        }

        public bool ContainsAll( params ReadOnlySpan<TValue> values )
        {
            /*
            if ( Vector.IsHardwareAccelerated && span.Length >= Vector<TValue>.Count )
            {
                Vector<TValue> source = Vector.Create( span );
                if ( values.Length >= Vector<TValue>.Count ) { return Vector.EqualsAll( source, Vector.Create( values ) ); }

                using LinkSpan<Vector<TValue>> vectors = values.GetVectors();

                foreach ( Vector<TValue> vector in vectors.ReadOnlySpan )
                {
                    if ( Vector.EqualsAll( source, vector ) ) { return true; }
                }
            }
            */

            foreach ( TValue c in values )
            {
                if ( !self.Contains(c) ) { return false; }
            }

            return true;
        }

        public bool ContainsAny( params ReadOnlySpan<TValue> values )
        {
            /*
            if ( Vector.IsHardwareAccelerated && span.Length >= Vector<TValue>.Count )
            {
                Vector<TValue> source = Vector.Create( span );
                if ( values.Length >= Vector<TValue>.Count ) { return Vector.EqualsAny( source, Vector.Create( values ) ); }

                using LinkSpan<Vector<TValue>> vectors = values.GetVectors();

                foreach ( Vector<TValue> vector in vectors.ReadOnlySpan )
                {
                    if ( Vector.EqualsAny( source, vector ) ) { return true; }
                }
            }
            */

            foreach ( TValue c in values )
            {
                if ( self.Contains(c) ) { return true; }
            }

            return false;
        }

        public bool ContainsNone( params ReadOnlySpan<TValue> values )
        {
            /*
            if ( Vector.IsHardwareAccelerated && span.Length >= Vector<TValue>.Count )
            {
                Vector<TValue> source = Vector.Create( span );
                if ( values.Length >= Vector<TValue>.Count ) { return Vector.EqualsAny( source, Vector.Create( values ) ); }

                using LinkSpan<Vector<TValue>> vectors = values.GetVectors();

                foreach ( Vector<TValue> vector in vectors.ReadOnlySpan )
                {
                    if ( Vector.EqualsAny( source, vector ) ) { return false; }
                }
            }
            */

            foreach ( TValue c in values )
            {
                if ( self.Contains(c) ) { return false; }
            }

            return true;
        }


        public bool EndsWith( TValue value ) => !self.IsEmpty &&
                                                self[^1]
                                                   .Equals(value);
        public bool EndsWith( params ReadOnlySpan<TValue> value )
        {
            if ( self.IsEmpty ) { return false; }

            if ( self.Length < value.Length ) { return false; }

            ReadOnlySpan<TValue> temp = self.Slice(self.Length - value.Length, value.Length);

            for ( int i = 0; i < value.Length; i++ )
            {
                if ( !temp[i]
                        .Equals(value[i]) ) { return false; }
            }

            return true;
        }


        public bool StartsWith( TValue value ) => !self.IsEmpty &&
                                                  self[0]
                                                     .Equals(value);
        public bool StartsWith( params ReadOnlySpan<TValue> value )
        {
            if ( self.IsEmpty ) { return false; }

            if ( self.Length < value.Length ) { return false; }

            ReadOnlySpan<TValue> temp = self[..value.Length];

            for ( int i = 0; i < value.Length; i++ )
            {
                if ( !temp[i]
                        .Equals(value[i]) ) { return false; }
            }

            return true;
        }
    }



    extension<TValue>( scoped in ReadOnlySpan<TValue> self )
        where TValue : IEquatable<TValue>
    {
        public bool ContainsExact( params ReadOnlySpan<TValue> value )
        {
            if ( value.Length > self.Length ) { return false; }

            if ( value.Length == self.Length ) { return self.SequenceEqual(value); }

            for ( int i = 0; i < self.Length || i + value.Length < self.Length; i++ )
            {
                if ( self.Slice(i, value.Length)
                         .SequenceEqual(value) ) { return true; }
            }

            return false;
        }

        public bool ContainsAll( params ReadOnlySpan<TValue> values )
        {
            /*
            if ( Vector.IsHardwareAccelerated && span.Length >= Vector<TValue>.Count )
            {
                Vector<TValue> source = Vector.Create( span );
                if ( values.Length >= Vector<TValue>.Count ) { return Vector.EqualsAll( source, Vector.Create( values ) ); }

                using LinkSpan<Vector<TValue>> vectors = values.GetVectors();

                foreach ( Vector<TValue> vector in vectors.ReadOnlySpan )
                {
                    if ( Vector.EqualsAll( source, vector ) ) { return true; }
                }
            }
            */

            foreach ( TValue c in values )
            {
                if ( !self.Contains(c) ) { return false; }
            }

            return true;
        }

        public bool ContainsAny( params ReadOnlySpan<TValue> values )
        {
            /*
            if ( Vector.IsHardwareAccelerated && span.Length >= Vector<TValue>.Count )
            {
                Vector<TValue> source = Vector.Create( span );
                if ( values.Length >= Vector<TValue>.Count ) { return Vector.EqualsAny( source, Vector.Create( values ) ); }

                using LinkSpan<Vector<TValue>> vectors = values.GetVectors();

                foreach ( Vector<TValue> vector in vectors.ReadOnlySpan )
                {
                    if ( Vector.EqualsAny( source, vector ) ) { return true; }
                }
            }
            */

            foreach ( TValue c in values )
            {
                if ( self.Contains(c) ) { return true; }
            }

            return false;
        }

        public bool ContainsNone( params ReadOnlySpan<TValue> values )
        {
            /*
            if ( Vector.IsHardwareAccelerated && span.Length >= Vector<TValue>.Count )
            {
                Vector<TValue> source = Vector.Create( span );
                if ( values.Length >= Vector<TValue>.Count ) { return Vector.EqualsAny( source, Vector.Create( values ) ); }

                using LinkSpan<Vector<TValue>> vectors = values.GetVectors();

                foreach ( Vector<TValue> vector in vectors.ReadOnlySpan )
                {
                    if ( Vector.EqualsAny( source, vector ) ) { return false; }
                }
            }
            */

            foreach ( TValue c in values )
            {
                if ( self.Contains(c) ) { return false; }
            }

            return true;
        }


        public bool EndsWith( TValue value ) => !self.IsEmpty &&
                                                self[^1]
                                                   .Equals(value);
        public bool EndsWith( params ReadOnlySpan<TValue> value )
        {
            if ( self.IsEmpty ) { return false; }

            if ( self.Length < value.Length ) { return false; }

            ReadOnlySpan<TValue> temp = self.Slice(self.Length - value.Length, value.Length);

            for ( int i = 0; i < value.Length; i++ )
            {
                if ( !temp[i]
                        .Equals(value[i]) ) { return false; }
            }

            return true;
        }


        public bool StartsWith( TValue value ) => !self.IsEmpty &&
                                                  self[0]
                                                     .Equals(value);
        public bool StartsWith( params ReadOnlySpan<TValue> value )
        {
            if ( self.IsEmpty ) { return false; }

            if ( self.Length < value.Length ) { return false; }

            ReadOnlySpan<TValue> temp = self[..value.Length];

            for ( int i = 0; i < value.Length; i++ )
            {
                if ( !temp[i]
                        .Equals(value[i]) ) { return false; }
            }

            return true;
        }


        [Pure] [MustDisposeResource] public ReadOnlySpan<Vector<TValue>> GetVectors()
        {
            Span<Vector<TValue>> vectors = GC.AllocateUninitializedArray<Vector<TValue>>(self.Length);
            for ( int i = 0; i < vectors.Length; i++ ) { vectors[i] = Vector.Create(self[i]); }

            return vectors;
        }
    }
}
