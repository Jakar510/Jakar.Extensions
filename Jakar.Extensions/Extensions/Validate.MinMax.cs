namespace Jakar.Extensions;


public static partial class Validate
{
    extension<TValue>( [NotNullIfNotNull("left")] TValue? left )
        where TValue : struct, IComparable<TValue>
    {
        public TValue? Min( [NotNullIfNotNull("right")] TValue? right )
        {
            if ( left is null && right is null ) { return null; }

            return Nullable.Compare(left, right) == NOT_FOUND
                       ? left  ?? right
                       : right ?? left;
        }
        public TValue? Max( [NotNullIfNotNull("right")] TValue? right )
        {
            if ( left is null && right is null ) { return null; }

            return Nullable.Compare(left, right) == 1
                       ? left  ?? right
                       : right ?? left;
        }
    }
}
