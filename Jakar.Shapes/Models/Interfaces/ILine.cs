// Jakar.Extensions :: Jakar.Shapes
// 07/12/2025  19:42

namespace Jakar.Shapes.Interfaces;


public interface ILine<TSelf> : IShape<TSelf>
    where TSelf : struct, ILine<TSelf>, IJsonModel<TSelf>
{
    ReadOnlyPoint End      { get; }
    bool          IsFinite { get; }
    double        Length   { get; }
    ReadOnlyPoint Start    { get; }


    [Pure] public abstract static TSelf Create( in ReadOnlyPoint start, in ReadOnlyPoint end, bool isFinite = true );
    [Pure] public                 TSelf Round();
    [Pure] public                 TSelf Floor();


    public abstract static TSelf operator &( TSelf self, ReadOnlyPoint  other );
    public abstract static TSelf operator &( TSelf self, ReadOnlyPointF other );
    public abstract static TSelf operator ^( TSelf self, ReadOnlyPoint  other );
    public abstract static TSelf operator ^( TSelf self, ReadOnlyPointF other );


    public static string ToString( ref readonly TSelf self, string? format )
    {
        switch ( format )
        {
            case "json":
            case "JSON":
            case "Json":
                return self.ToJson(TSelf.JsonTypeInfo);

            case ",":
                return $"{self.Start.ToString(format, null)},{self.End.ToString(format, null)}";

            case "-":
                return $"{self.Start.ToString(format, null)}-{self.End.ToString(format, null)}";

            case EMPTY:
            case null:
            default:
                return $"{typeof(TSelf).Name}<{nameof(Start)}: {self.Start}, {nameof(End)}: {self.End}, {nameof(IsFinite)}: {self.IsFinite}>";
        }
    }
}
