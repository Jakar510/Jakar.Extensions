// Jakar.Extensions :: Jakar.Extensions
// 07/12/2025  14:39

namespace Jakar.Shapes.Interfaces;


public interface ICircle<TSelf> : IShape<TSelf>, IShapeLocation
    where TSelf : struct, ICircle<TSelf>, IJsonModel<TSelf>
{
    ReadOnlyPoint Center { get; }
    double        Radius { get; }


    [Pure] public abstract static TSelf Create( in ReadOnlyPoint center, double radius );
}
