// Jakar.Extensions :: Jakar.Extensions
// 09/26/2025  16:38

namespace Jakar.Extensions;


public interface IStructuralComparable<TSelf> : IComparable<TSelf>, IStructuralComparable
{
    int CompareTo( TSelf? other, IComparer<TSelf> comparer );
}
