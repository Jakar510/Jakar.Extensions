using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Jakar.Extensions;


public static class Comparers
{
    public static T? Min<T>( [NotNullIfNotNull( "left" )] this T? left, [NotNullIfNotNull( "right" )] T? right ) where T : struct, IComparable<T>
    {
        if ( left is null && right is null ) { return default; }

        return Nullable.Compare( left, right ) == -1
                   ? left ?? right
                   : right ?? left;
    }


    public static T? Max<T>( [NotNullIfNotNull( "left" )] this T? left, [NotNullIfNotNull( "right" )] T? right ) where T : struct, IComparable<T>
    {
        if ( left is null && right is null ) { return default; }

        return Nullable.Compare( left, right ) == 1
                   ? left ?? right
                   : right ?? left;
    }
}
