// Jakar.Extensions :: Jakar.Shapes
// 09/29/2025  10:07

using ZLinq;



namespace Jakar.Shapes;


public static class Splines
{
    public static ReadOnlyPoint Center<TSpline>( this TSpline self )
        where TSpline : struct, ISpline<TSpline> => new(self.AsValueEnumerable().Sum(static x => x.X) / self.Length, self.AsValueEnumerable().Sum(static x => x.Y) / self.Length);
    public static TSpline Abs<TSpline>( this TSpline self )
        where TSpline : struct, ISpline<TSpline> => TSpline.Create(self.AsValueEnumerable().Select(static x => x.Abs()).ToArray());
    public static bool IsFinite<TSpline>( this TSpline self )
        where TSpline : struct, ISpline<TSpline> => self.AsValueEnumerable().All(static x => x.IsFinite());
    public static bool IsInfinity<TSpline>( this TSpline self )
        where TSpline : struct, ISpline<TSpline> => self.AsValueEnumerable().Any(static x => x.IsInfinity());
    public static bool IsInteger<TSpline>( this TSpline self )
        where TSpline : struct, ISpline<TSpline> => self.AsValueEnumerable().All(static x => x.IsInteger());
    public static bool IsNaN<TSpline>( this TSpline self )
        where TSpline : struct, ISpline<TSpline> => self.AsValueEnumerable().Any(static x => x.IsNaN());
    public static bool IsNegative<TSpline>( this TSpline self )
        where TSpline : struct, ISpline<TSpline> => self.AsValueEnumerable().Any(static x => x.IsNegative());
    public static bool IsValid<TSpline>( this TSpline self )
        where TSpline : struct, ISpline<TSpline> => self.AsValueEnumerable().All(static x => x.IsValid());
    public static bool IsPositive<TSpline>( this TSpline self )
        where TSpline : struct, ISpline<TSpline> => self.AsValueEnumerable().Any(static x => x.IsNaN());
    public static bool IsZero<TSpline>( this TSpline self )
        where TSpline : struct, ISpline<TSpline> => self.AsValueEnumerable().Any(static x => x.IsZero());
}
