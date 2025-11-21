namespace Jakar.Extensions;


public static partial class Validate
{
    extension<TValue>( [NotNullIfNotNull("self")] TValue? self )
        where TValue : struct, IComparable<TValue>
    {
        public TValue? Min( [NotNullIfNotNull("other")] TValue? other )
        {
            if ( self is null && other is null ) { return null; }

            return Nullable.Compare(self, other) == NOT_FOUND
                       ? self  ?? other
                       : other ?? self;
        }
        public TValue? Max( [NotNullIfNotNull("other")] TValue? other )
        {
            if ( self is null && other is null ) { return null; }

            return Nullable.Compare(self, other) == 1
                       ? self  ?? other
                       : other ?? self;
        }
    }
}
