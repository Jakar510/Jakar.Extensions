// Jakar.Extensions :: Jakar.Extensions
// 06/27/2025  18:22

namespace Jakar.Extensions;


public interface IMutableRectangle<TSelf> : IRectangle<TSelf>
    where TSelf : struct, IMutableRectangle<TSelf>
{
    public new double X      { get; set; }
    public new double Y      { get; set; }
    public new double Width  { get; set; }
    public new double Height { get; set; }


    TSelf Union( in TSelf other );
    TSelf Round();
    TSelf Intersection( in      TSelf                        other );
    bool  IsAtLeast( in         ReadOnlySize                 other );
    bool  IsAtLeast( in         ReadOnlySizeF                other );
    bool  Contains( in          ReadOnlyPoint                other );
    bool  Contains( in          ReadOnlyPointF               other );
    bool  Contains( in          TSelf                        other );
    bool  Contains( params      ReadOnlySpan<ReadOnlyPoint>  points );
    bool  Contains( params      ReadOnlySpan<ReadOnlyPointF> points );
    bool  ContainsAny( params   ReadOnlySpan<ReadOnlyPoint>  others );
    bool  ContainsAll( params   ReadOnlySpan<ReadOnlyPoint>  others );
    bool  IntersectsWith( in    TSelf                        other );
    bool  DoesLineIntersect( in ReadOnlyPoint                source, in  ReadOnlyPoint  target );
    bool  DoesLineIntersect( in ReadOnlyPointF               source, in  ReadOnlyPointF target );
    void  Deconstruct( out      ReadOnlyPointF               point,  out ReadOnlySizeF  size );


    public abstract static TSelf operator +( TSelf rectangle, TSelf                            other );
    public abstract static TSelf operator -( TSelf rectangle, TSelf                            other );
    public abstract static TSelf operator *( TSelf rectangle, TSelf                            other );
    public abstract static TSelf operator /( TSelf rectangle, TSelf                            other );
    public abstract static TSelf operator +( TSelf rectangle, ReadOnlyRectangle                other );
    public abstract static TSelf operator -( TSelf rectangle, ReadOnlyRectangle                other );
    public abstract static TSelf operator *( TSelf rectangle, ReadOnlyRectangle                other );
    public abstract static TSelf operator /( TSelf rectangle, ReadOnlyRectangle                other );
    public abstract static TSelf operator +( TSelf rectangle, ReadOnlyRectangleF               other );
    public abstract static TSelf operator -( TSelf rectangle, ReadOnlyRectangleF               other );
    public abstract static TSelf operator *( TSelf rectangle, ReadOnlyRectangleF               other );
    public abstract static TSelf operator /( TSelf rectangle, ReadOnlyRectangleF               other );
    public abstract static TSelf operator +( TSelf rectangle, ReadOnlySize                     other );
    public abstract static TSelf operator -( TSelf rectangle, ReadOnlySize                     other );
    public abstract static TSelf operator *( TSelf rectangle, ReadOnlySize                     other );
    public abstract static TSelf operator /( TSelf rectangle, ReadOnlySize                     other );
    public abstract static TSelf operator +( TSelf rectangle, ReadOnlySizeF                    other );
    public abstract static TSelf operator -( TSelf rectangle, ReadOnlySizeF                    other );
    public abstract static TSelf operator *( TSelf rectangle, ReadOnlySizeF                    other );
    public abstract static TSelf operator /( TSelf rectangle, ReadOnlySizeF                    other );
    public abstract static TSelf operator +( TSelf rectangle, ReadOnlyPoint                    other );
    public abstract static TSelf operator -( TSelf rectangle, ReadOnlyPoint                    other );
    public abstract static TSelf operator *( TSelf rectangle, ReadOnlyPoint                    other );
    public abstract static TSelf operator /( TSelf rectangle, ReadOnlyPoint                    other );
    public abstract static TSelf operator +( TSelf rectangle, ReadOnlyPointF                   other );
    public abstract static TSelf operator -( TSelf rectangle, ReadOnlyPointF                   other );
    public abstract static TSelf operator *( TSelf rectangle, ReadOnlyPointF                   other );
    public abstract static TSelf operator /( TSelf rectangle, ReadOnlyPointF                   other );
    public abstract static TSelf operator +( TSelf rectangle, PointF                           other );
    public abstract static TSelf operator -( TSelf rectangle, PointF                           other );
    public abstract static TSelf operator *( TSelf rectangle, PointF                           other );
    public abstract static TSelf operator /( TSelf rectangle, PointF                           other );
    public abstract static TSelf operator +( TSelf rectangle, Point                            other );
    public abstract static TSelf operator -( TSelf rectangle, Point                            other );
    public abstract static TSelf operator *( TSelf rectangle, Point                            other );
    public abstract static TSelf operator /( TSelf rectangle, Point                            other );
    public abstract static TSelf operator &( TSelf rectangle, ReadOnlyPointF                   other );
    public abstract static TSelf operator &( TSelf rectangle, ReadOnlyPoint                    other );
    public abstract static TSelf operator &( TSelf rectangle, PointF                           other );
    public abstract static TSelf operator &( TSelf rectangle, Point                            other );
    public abstract static TSelf operator &( TSelf rectangle, ReadOnlySize                     other );
    public abstract static TSelf operator &( TSelf rectangle, ReadOnlySizeF                    other );
    public abstract static TSelf operator &( TSelf rectangle, Size                             other );
    public abstract static TSelf operator &( TSelf rectangle, SizeF                            other );
    public abstract static TSelf operator +( TSelf rectangle, ReadOnlyThickness                margin );
    public abstract static TSelf operator -( TSelf rectangle, ReadOnlyThickness                margin );
    public abstract static TSelf operator +( TSelf rectangle, int                              value );
    public abstract static TSelf operator +( TSelf rectangle, float                            value );
    public abstract static TSelf operator +( TSelf rectangle, double                           value );
    public abstract static TSelf operator -( TSelf rectangle, int                              value );
    public abstract static TSelf operator -( TSelf rectangle, float                            value );
    public abstract static TSelf operator -( TSelf rectangle, double                           value );
    public abstract static TSelf operator *( TSelf rectangle, int                              value );
    public abstract static TSelf operator *( TSelf rectangle, float                            value );
    public abstract static TSelf operator *( TSelf rectangle, double                           value );
    public abstract static TSelf operator /( TSelf rectangle, int                              value );
    public abstract static TSelf operator /( TSelf rectangle, float                            value );
    public abstract static TSelf operator /( TSelf rectangle, double                           value );
    public abstract static TSelf operator +( TSelf rectangle, (int xOffset, int yOffset)       value );
    public abstract static TSelf operator +( TSelf rectangle, (float xOffset, float yOffset)   value );
    public abstract static TSelf operator +( TSelf rectangle, (double xOffset, double yOffset) value );
    public abstract static TSelf operator -( TSelf rectangle, (int xOffset, int yOffset)       value );
    public abstract static TSelf operator -( TSelf rectangle, (float xOffset, float yOffset)   value );
    public abstract static TSelf operator -( TSelf rectangle, (double xOffset, double yOffset) value );
    public abstract static TSelf operator *( TSelf rectangle, (int xOffset, int yOffset)       value );
    public abstract static TSelf operator *( TSelf rectangle, (float xOffset, float yOffset)   value );
    public abstract static TSelf operator *( TSelf rectangle, (double xOffset, double yOffset) value );
    public abstract static TSelf operator /( TSelf rectangle, (int xOffset, int yOffset)       value );
    public abstract static TSelf operator /( TSelf rectangle, (float xOffset, float yOffset)   value );
    public abstract static TSelf operator /( TSelf rectangle, (double xOffset, double yOffset) value );
}
