// Jakar.Extensions :: Jakar.Shapes
// 09/26/2025  16:57

using System.Collections;



namespace Jakar.Shapes.Interfaces;


public interface IStructuralComparable<TSelf> : IComparable<TSelf>, IStructuralComparable
{
    int CompareTo( TSelf? other, IComparer<TSelf> comparer );
}
