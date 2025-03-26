// Jakar.Extensions :: Jakar.Extensions
// 03/26/2025  11:03

namespace Jakar.Extensions;


public sealed class TypeEqualityComparer : IEqualityComparer<Type>
{
    public static readonly TypeEqualityComparer Instance = new();
    public bool Equals( Type? x, Type? y )
    {
        if ( ReferenceEquals( x, y ) ) { return true; }

        if ( x is null || y is null ) { return false; }

        return string.Equals( x.Name, y.Name, StringComparison.Ordinal ) && string.Equals( x.AssemblyQualifiedName, y.AssemblyQualifiedName, StringComparison.Ordinal ) && string.Equals( x.FullName, y.FullName, StringComparison.Ordinal );
    }
    public int GetHashCode( Type obj ) => HashCode.Combine( obj.Name, obj.AssemblyQualifiedName, obj.FullName );
}
