namespace Jakar.Extensions;


public static partial class Validate
{
    public static TValue? Min<TValue>( [NotNullIfNotNull( "left" )] this TValue? left, [NotNullIfNotNull( "right" )] TValue? right )
        where TValue : struct, IComparable<TValue>
    {
        if ( left is null && right is null ) { return null; }

        return Nullable.Compare( left, right ) == NOT_FOUND
                   ? left  ?? right
                   : right ?? left;
    }


    public static TValue? Max<TValue>( [NotNullIfNotNull( "left" )] this TValue? left, [NotNullIfNotNull( "right" )] TValue? right )
        where TValue : struct, IComparable<TValue>
    {
        if ( left is null && right is null ) { return null; }

        return Nullable.Compare( left, right ) == 1
                   ? left  ?? right
                   : right ?? left;
    }
}
