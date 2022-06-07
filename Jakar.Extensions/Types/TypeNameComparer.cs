#nullable enable
namespace Jakar.Extensions.Types;


public sealed class TypeNameComparer : IComparer<Type>
{
    public static readonly TypeNameComparer instance = new();

    /// <summary>
    ///     <para>
    ///         Comparing by
    ///         <see cref = "Type.AssemblyQualifiedName" />
    ///         would give the same result,
    ///         but it performs a hidden concatenation (and therefore string allocation)
    ///         of
    ///         <see cref = "Type.FullName" />
    ///         and
    ///         <see cref = "Type.Assembly" />
    ///         with its
    ///         <see cref = "Assembly.FullName" />
    ///         .
    ///     </para>
    ///     We can avoid this overhead by comparing the two properties separately.
    /// </summary>
    /// <param name = "x" > </param>
    /// <param name = "y" > </param>
    /// <returns> </returns>
    public int Compare( Type x, Type y )
    {
        int result = string.CompareOrdinal(x.FullName, y.FullName);

        return result != 0
                   ? result
                   : string.CompareOrdinal(x.Assembly.FullName, y.Assembly.FullName);
    }
}
