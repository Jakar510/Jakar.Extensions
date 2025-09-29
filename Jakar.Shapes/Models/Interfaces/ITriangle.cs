// Jakar.Extensions :: Jakar.Shapes
// 07/14/2025  10:32

namespace Jakar.Shapes.Interfaces;


public interface ITriangle<TSelf> : IShape<TSelf>, IShapeLocation
    where TSelf : struct, ITriangle<TSelf>
{
   ReadOnlyPoint A { get; }
   ReadOnlyPoint B { get; }
   ReadOnlyPoint C { get; }


    public abstract static TSelf Create( ReadOnlyPoint a, ReadOnlyPoint b, ReadOnlyPoint c );
}
