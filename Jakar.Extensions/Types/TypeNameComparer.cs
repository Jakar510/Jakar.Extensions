namespace Jakar.Extensions;


public sealed class TypeNameComparer : IComparer<Type>
{
    public static readonly TypeNameComparer Instance = new();

    /// <summary>
    ///     <para>
    ///         Comparing by <see cref="Type.AssemblyQualifiedName"/> would give the same result, but it performs a hidden concatenation (and therefore string allocation) of <see cref="Type.FullName"/> and
    ///         <see
    ///             cref="Type.Assembly"/>
    ///         with its
    ///         <see
    ///             cref="Assembly.FullName"/>
    ///         .
    ///     </para>
    ///     We can avoid this overhead by comparing the two properties separately.
    /// </summary>
    /// <param name="x"> </param>
    /// <param name="y"> </param>
    /// <returns> </returns>
    public int Compare( Type? x, Type? y )
    {
        int fullNameComparison = string.CompareOrdinal(x?.FullName, y?.FullName);
        if ( fullNameComparison != 0 ) { return fullNameComparison; }

        int nameComparison = string.CompareOrdinal(x?.Name, y?.Name);
        if ( nameComparison != 0 ) { return nameComparison; }

        return string.CompareOrdinal(x?.Assembly.FullName, y?.Assembly.FullName);
    }
}
