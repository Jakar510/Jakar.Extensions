// Jakar.Extensions :: Jakar.Shapes
// 07/12/2025  19:42

namespace Jakar.Shapes.Interfaces;


public interface ILine<TSelf, TPoint> : IShape<TSelf>
    where TSelf : struct, ILine<TSelf, TPoint>, IJsonModel<TSelf>
    where TPoint : struct, IPoint<TPoint>
{
    TPoint End      { get; }
    bool   IsFinite { get; }
    double Length   { get; }
    TPoint Start    { get; }


    [Pure] public abstract static TSelf Create( in TPoint start, in TPoint end, bool isFinite = true );


    public abstract static TSelf operator &( TSelf self, TPoint other );
    public abstract static TSelf operator ^( TSelf self, TPoint other );


}



public interface ILine<TSelf> : ILine<TSelf, ReadOnlyPoint>
    where TSelf : struct, ILine<TSelf>, IJsonModel<TSelf>
{
    public abstract static TSelf operator &( TSelf self, ReadOnlyPointF other );
    public abstract static TSelf operator ^( TSelf self, ReadOnlyPointF other );
}
