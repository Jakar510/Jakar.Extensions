// Jakar.Extensions :: Jakar.Extensions
// 06/27/2025  18:22

namespace Jakar.Shapes.Interfaces;


public interface IMutableRectangle<TSelf> : IRectangle<TSelf>
    where TSelf : struct, IMutableRectangle<TSelf>
{
    public new double X      { get; set; }
    public new double Y      { get; set; }
    public new double Width  { get; set; }
    public new double Height { get; set; }


    public TSelf Reverse();
    public TSelf Round();
    public TSelf Floor();
    public void  Deconstruct( out ReadOnlyPoint  point, out MutableSize size );
    public void  Deconstruct( out ReadOnlyPointF point, out MutableSize size );


    public abstract static implicit operator ReadOnlyRectangleF( TSelf self );
    public abstract static implicit operator ReadOnlyRectangle( TSelf  rect );
    public abstract static implicit operator TSelf( Rectangle          rect );
    public abstract static implicit operator TSelf( RectangleF         rect );
    public abstract static implicit operator TSelf( ReadOnlySize       rect );
    public abstract static implicit operator TSelf( ReadOnlySizeF      rect );
    public abstract static implicit operator TSelf( MutableSize        rect );
    public abstract static implicit operator TSelf( ReadOnlyPoint      rect );
    public abstract static implicit operator TSelf( ReadOnlyPointF     rect );


    public abstract static TSelf operator +( TSelf self, TSelf                            other );
    public abstract static TSelf operator -( TSelf self, TSelf                            other );
    public abstract static TSelf operator *( TSelf self, TSelf                            other );
    public abstract static TSelf operator /( TSelf self, TSelf                            other );
    public abstract static TSelf operator +( TSelf self, ReadOnlyRectangle                other );
    public abstract static TSelf operator -( TSelf self, ReadOnlyRectangle                other );
    public abstract static TSelf operator *( TSelf self, ReadOnlyRectangle                other );
    public abstract static TSelf operator /( TSelf self, ReadOnlyRectangle                other );
    public abstract static TSelf operator +( TSelf self, ReadOnlyRectangleF               other );
    public abstract static TSelf operator -( TSelf self, ReadOnlyRectangleF               other );
    public abstract static TSelf operator *( TSelf self, ReadOnlyRectangleF               other );
    public abstract static TSelf operator /( TSelf self, ReadOnlyRectangleF               other );
    public abstract static TSelf operator +( TSelf self, ReadOnlySize                     other );
    public abstract static TSelf operator -( TSelf self, ReadOnlySize                     other );
    public abstract static TSelf operator *( TSelf self, ReadOnlySize                     other );
    public abstract static TSelf operator /( TSelf self, ReadOnlySize                     other );
    public abstract static TSelf operator +( TSelf self, ReadOnlySizeF                    other );
    public abstract static TSelf operator -( TSelf self, ReadOnlySizeF                    other );
    public abstract static TSelf operator *( TSelf self, ReadOnlySizeF                    other );
    public abstract static TSelf operator /( TSelf self, ReadOnlySizeF                    other );
    public abstract static TSelf operator +( TSelf self, MutableSize                      other );
    public abstract static TSelf operator -( TSelf self, MutableSize                      other );
    public abstract static TSelf operator *( TSelf self, MutableSize                      other );
    public abstract static TSelf operator /( TSelf self, MutableSize                      other );
    public abstract static TSelf operator +( TSelf self, ReadOnlyPoint                    other );
    public abstract static TSelf operator -( TSelf self, ReadOnlyPoint                    other );
    public abstract static TSelf operator *( TSelf self, ReadOnlyPoint                    other );
    public abstract static TSelf operator /( TSelf self, ReadOnlyPoint                    other );
    public abstract static TSelf operator +( TSelf self, ReadOnlyPointF                   other );
    public abstract static TSelf operator -( TSelf self, ReadOnlyPointF                   other );
    public abstract static TSelf operator *( TSelf self, ReadOnlyPointF                   other );
    public abstract static TSelf operator /( TSelf self, ReadOnlyPointF                   other );
    public abstract static TSelf operator +( TSelf self, PointF                           other );
    public abstract static TSelf operator -( TSelf self, PointF                           other );
    public abstract static TSelf operator *( TSelf self, PointF                           other );
    public abstract static TSelf operator /( TSelf self, PointF                           other );
    public abstract static TSelf operator +( TSelf self, Point                            other );
    public abstract static TSelf operator -( TSelf self, Point                            other );
    public abstract static TSelf operator *( TSelf self, Point                            other );
    public abstract static TSelf operator /( TSelf self, Point                            other );
    public abstract static TSelf operator &( TSelf self, ReadOnlyPointF                   other );
    public abstract static TSelf operator &( TSelf self, ReadOnlyPoint                    other );
    public abstract static TSelf operator &( TSelf self, PointF                           other );
    public abstract static TSelf operator &( TSelf self, Point                            other );
    public abstract static TSelf operator &( TSelf self, ReadOnlySize                     other );
    public abstract static TSelf operator &( TSelf self, ReadOnlySizeF                    other );
    public abstract static TSelf operator &( TSelf self, MutableSize                      other );
    public abstract static TSelf operator &( TSelf self, Size                             other );
    public abstract static TSelf operator &( TSelf self, SizeF                            other );
    public abstract static TSelf operator +( TSelf self, ReadOnlyThickness                margin );
    public abstract static TSelf operator -( TSelf self, ReadOnlyThickness                margin );
    public abstract static TSelf operator +( TSelf self, int                              value );
    public abstract static TSelf operator +( TSelf self, float                            value );
    public abstract static TSelf operator +( TSelf self, double                           value );
    public abstract static TSelf operator -( TSelf self, int                              value );
    public abstract static TSelf operator -( TSelf self, float                            value );
    public abstract static TSelf operator -( TSelf self, double                           value );
    public abstract static TSelf operator *( TSelf self, int                              value );
    public abstract static TSelf operator *( TSelf self, float                            value );
    public abstract static TSelf operator *( TSelf self, double                           value );
    public abstract static TSelf operator /( TSelf self, int                              value );
    public abstract static TSelf operator /( TSelf self, float                            value );
    public abstract static TSelf operator /( TSelf self, double                           value );
    public abstract static TSelf operator +( TSelf self, (int xOffset, int yOffset)       value );
    public abstract static TSelf operator +( TSelf self, (float xOffset, float yOffset)   value );
    public abstract static TSelf operator +( TSelf self, (double xOffset, double yOffset) value );
    public abstract static TSelf operator -( TSelf self, (int xOffset, int yOffset)       value );
    public abstract static TSelf operator -( TSelf self, (float xOffset, float yOffset)   value );
    public abstract static TSelf operator -( TSelf self, (double xOffset, double yOffset) value );
    public abstract static TSelf operator *( TSelf self, (int xOffset, int yOffset)       value );
    public abstract static TSelf operator *( TSelf self, (float xOffset, float yOffset)   value );
    public abstract static TSelf operator *( TSelf self, (double xOffset, double yOffset) value );
    public abstract static TSelf operator /( TSelf self, (int xOffset, int yOffset)       value );
    public abstract static TSelf operator /( TSelf self, (float xOffset, float yOffset)   value );
    public abstract static TSelf operator /( TSelf self, (double xOffset, double yOffset) value );
}
