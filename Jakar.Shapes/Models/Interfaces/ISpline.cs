// Jakar.Extensions :: Jakar.Shapes
// 09/26/2025  16:57

using ZLinq;
using ZLinq.Linq;



namespace Jakar.Shapes.Interfaces;


public interface ISpline<TSelf, TPoint> : IShape<TSelf>, IStructuralComparable<TSelf>, IValueEnumerable<FromArray<TPoint>, TPoint>
    where TSelf : struct, ISpline<TSelf>, IJsonModel<TSelf>
    where TPoint : IPoint<TPoint>
{
    public ref readonly TPoint this[ int   index ] { get; }
    public ref readonly TPoint this[ Index index ] { get; }
    public Spline this[ Range              index ] { [Pure] get; }
    public int Length { get; }


    [Pure] public abstract static TSelf Create( params TPoint[]? points );
    [Pure] public                 TSelf Round();
    [Pure] public                 TSelf Floor();
}



public interface ISpline<TSelf> : ISpline<TSelf, ReadOnlyPoint>
    where TSelf : struct, ISpline<TSelf>, IJsonModel<TSelf>
{
    public abstract static implicit operator TSelf( ReadOnlyPoint[]? points );
}
