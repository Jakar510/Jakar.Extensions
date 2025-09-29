// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  14:02

namespace Jakar.Shapes.Interfaces;


public interface ISize<TSelf> : IShape<TSelf>, IShapeSize
    where TSelf : struct, ISize<TSelf>
{
    [Pure] public abstract static TSelf Create( double width, double height );
}
