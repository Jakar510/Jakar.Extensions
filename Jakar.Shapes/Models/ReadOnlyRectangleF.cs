// TrueLogic :: TrueLogic.Common.Maui
// 01/20/2025  08:01

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;



namespace Jakar.Shapes;


[StructLayout(LayoutKind.Sequential)][DefaultValue(nameof(Zero))]
[method: JsonConstructor]
public readonly struct ReadOnlyRectangleF( float x, float y, float width, float height ) : IRectangle<ReadOnlyRectangleF>
{
    public static readonly ReadOnlyRectangleF Invalid = new(float.NaN, float.NaN, float.NaN, float.NaN);
    public static readonly ReadOnlyRectangleF Zero    = new(0, 0, 0, 0);
    public static readonly ReadOnlyRectangleF One     = 1;
    public readonly        float              X       = x;
    public readonly        float              Y       = y;
    public readonly        float              Width   = width;
    public readonly        float              Height  = height;


    static ref readonly ReadOnlyRectangleF IShape<ReadOnlyRectangleF>.Zero     => ref Zero;
    static ref readonly ReadOnlyRectangleF IShape<ReadOnlyRectangleF>.Invalid  => ref Invalid;
    static ref readonly ReadOnlyRectangleF IShape<ReadOnlyRectangleF>.One      => ref One;
    public              bool                                          IsEmpty  => double.IsNaN(Bottom) || double.IsNaN(Top) || double.IsNegative(Left) || double.IsNaN(Left) || double.IsNaN(Right) || double.IsNegative(Right);
    double IShapeLocation.                                            X        => X;
    double IShapeLocation.                                            Y        => Y;
    double IShapeSize.                                                Width    => Width;
    double IShapeSize.                                                Height   => Height;
    public bool                                                       IsNaN    => double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Width) || double.IsNaN(Height);
    public bool                                                       IsValid  => !IsNaN && X >= 0 && Y >= 0 && Width >= 0 && Height >= 0;
    public ReadOnlyPointF                                             Center   => new(Right / 2, Bottom / 2);
    public ReadOnlyPointF                                             Location => new(X, Y);
    public ReadOnlySizeF                                              Size     => new(Width, Height);
    public float                                                      Bottom   => Y + Height;
    public float                                                      Left     => X;
    public float                                                      Right    => X + Width;
    public float                                                      Top      => Y;
    ReadOnlyPoint IRectangle<ReadOnlyRectangleF>.                     Center   => new(Right / 2, Bottom / 2);
    ReadOnlyPoint IRectangle<ReadOnlyRectangleF>.                     Location => new(X, Y);
    ReadOnlySize IRectangle<ReadOnlyRectangleF>.                      Size     => new(Width, Height);
    double IRectangle<ReadOnlyRectangleF>.                            Bottom   => Y + Height;
    double IRectangle<ReadOnlyRectangleF>.                            Left     => X;
    double IRectangle<ReadOnlyRectangleF>.                            Right    => X + Width;
    double IRectangle<ReadOnlyRectangleF>.                            Top      => Y;


    public static implicit operator Rectangle( ReadOnlyRectangleF  self )  => new((int)self.X.Round(), (int)self.Y.Round(), (int)self.Width.Round(), (int)self.Height.Round());
    public static implicit operator RectangleF( ReadOnlyRectangleF self )  => new(self.X, self.Y, self.Width, self.Height);
    public static implicit operator ReadOnlyRectangleF( Rectangle  self )  => new(self.X, self.Y, self.Width, self.Height);
    public static implicit operator ReadOnlyRectangleF( RectangleF self )  => new(self.X, self.Y, self.Width, self.Height);
    public static implicit operator ReadOnlyRectangleF( int        value ) => new(value, value, value, value);
    public static implicit operator ReadOnlyRectangleF( long       value ) => new(value, value, value, value);
    public static implicit operator ReadOnlyRectangleF( float      value ) => new(value, value, value, value);
    public static implicit operator ReadOnlyRectangleF( double     value ) => value.AsFloat();


    public void Deconstruct( out double x, out double y )
    {
        x = X;
        y = Y;
    }


    [Pure]
    public static ReadOnlySize AddMargin( in ReadOnlySize value, in ReadOnlyThickness margin )
    {
        ReadOnlySize result = new(value.Width + margin.HorizontalThickness, value.Height + margin.VerticalThickness);
        Debug.Assert(result >= value);
        return result;
    }


    [Pure]
    public static ReadOnlyRectangleF Create<T>( ref readonly T rect )
        where T : IRectangle<T>
    {
        return new ReadOnlyRectangleF(rect.X.AsFloat(), rect.Y.AsFloat(), rect.Width.AsFloat(), rect.Height.AsFloat());
    }
    [Pure] public static ReadOnlyRectangleF Create( params ReadOnlySpan<ReadOnlyPoint>  points )                                 => MutableRectangle.Create(points);
    [Pure] public static ReadOnlyRectangleF Create( params ReadOnlySpan<ReadOnlyPointF> points )                                 => MutableRectangle.Create(points);
    [Pure] public static ReadOnlyRectangleF Create( in     ReadOnlyPoint                point,   in ReadOnlySize   size )        => new(point.X.AsFloat(), point.Y.AsFloat(), size.Width.AsFloat(), size.Height.AsFloat());
    [Pure] public static ReadOnlyRectangleF Create( in     ReadOnlyPointF               point,   in ReadOnlySizeF  size )        => new(point.X, point.Y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangleF Create( in     ReadOnlyPoint                topLeft, in ReadOnlyPoint  bottomRight ) => new(topLeft.X.AsFloat(), topLeft.Y.AsFloat(), ( bottomRight.X - topLeft.X ).AsFloat(), ( bottomRight.Y - topLeft.Y ).AsFloat());
    [Pure] public static ReadOnlyRectangleF Create( in     ReadOnlyPointF               topLeft, in ReadOnlyPointF bottomRight ) => new(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

    [Pure]
    public static ReadOnlyRectangleF Create( in ReadOnlyRectangle self, in ReadOnlyThickness padding )
    {
        float x      = (float)( self.Left   + padding.Left );
        float y      = (float)( self.Top    + padding.Top );
        float width  = (float)( self.Width  - padding.HorizontalThickness );
        float height = (float)( self.Height - padding.VerticalThickness );
        return new ReadOnlyRectangleF(x, y, width, height);
    }

    [Pure]
    public static ReadOnlyRectangleF Create( in ReadOnlyRectangleF self, in ReadOnlyThickness padding )
    {
        float x      = (float)( self.Left   + padding.HorizontalThickness );
        float y      = (float)( self.Top    + padding.VerticalThickness );
        float width  = (float)( self.Width  - padding.HorizontalThickness );
        float height = (float)( self.Height - padding.VerticalThickness );
        return new ReadOnlyRectangleF(x, y, width, height);
    }


    [Pure] public static ReadOnlyRectangleF Create( double x, double y, in ReadOnlySize  size )                 => new(x.AsFloat(), y.AsFloat(), size.Width.AsFloat(), size.Height.AsFloat());
    [Pure] public static ReadOnlyRectangleF Create( double x, double y, double           width, double height ) => new(x.AsFloat(), y.AsFloat(), width.AsFloat(), height.AsFloat());
    [Pure] public static ReadOnlyRectangleF Create( float  x, float  y, in ReadOnlySizeF size )                => new(x, y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangleF Create( float  x, float  y, float            width, float height ) => new(x, y, width, height);


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        return other is ReadOnlyRectangleF x
                   ? CompareTo(x)
                   : throw new ExpectedValueTypeException(nameof(other), other, typeof(ReadOnlyRectangleF));
    }
    public int CompareTo( ReadOnlyRectangleF other )
    {
        int xComparison = X.CompareTo(other.X);
        if ( xComparison != 0 ) { return xComparison; }

        int yComparison = Y.CompareTo(other.Y);
        if ( yComparison != 0 ) { return yComparison; }

        int widthComparison = Width.CompareTo(other.Width);
        if ( widthComparison != 0 ) { return widthComparison; }

        return Height.CompareTo(other.Height);
    }
    public          bool   Equals( ReadOnlyRectangleF other )                          => X.Equals(other.X)             && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);
    public override bool   Equals( object?            other )                          => other is ReadOnlyRectangleF x && Equals(x);
    public override int    GetHashCode()                                               => HashCode.Combine(X, Y, Width, Height);
    public override string ToString()                                                  => ToString(null, null);
    public          string ToString( string? format, IFormatProvider? formatProvider ) => IRectangle<ReadOnlyRectangleF>.ToString(in this, format);


    public void Deconstruct( out float x, out float y, out float width, out float height )
    {
        x      = X;
        y      = Y;
        width  = Width;
        height = Height;
    }
    public void Deconstruct( out double x, out double y, out double width, out double height )
    {
        x      = X;
        y      = Y;
        width  = Width;
        height = Height;
    }
    public void Deconstruct( out ReadOnlyPoint point, out ReadOnlySize size )
    {
        point = Location;
        size  = Size;
    }
    public void Deconstruct( out ReadOnlyPointF point, out ReadOnlySizeF size )
    {
        point = Location;
        size  = Size;
    }


    public static bool operator ==( ReadOnlyRectangleF?             left, ReadOnlyRectangleF?              right ) => Nullable.Equals(left, right);
    public static bool operator !=( ReadOnlyRectangleF?             left, ReadOnlyRectangleF?              right ) => !Nullable.Equals(left, right);
    public static bool operator ==( ReadOnlyRectangleF              left, ReadOnlyRectangleF               right ) => EqualityComparer<ReadOnlyRectangleF>.Default.Equals(left, right);
    public static bool operator !=( ReadOnlyRectangleF              left, ReadOnlyRectangleF               right ) => !EqualityComparer<ReadOnlyRectangleF>.Default.Equals(left, right);
    public static bool operator >( ReadOnlyRectangleF               left, ReadOnlyRectangleF               right ) => Comparer<ReadOnlyRectangleF>.Default.Compare(left, right) > 0;
    public static bool operator >=( ReadOnlyRectangleF              left, ReadOnlyRectangleF               right ) => Comparer<ReadOnlyRectangleF>.Default.Compare(left, right) >= 0;
    public static bool operator <( ReadOnlyRectangleF               left, ReadOnlyRectangleF               right ) => Comparer<ReadOnlyRectangleF>.Default.Compare(left, right) < 0;
    public static bool operator <=( ReadOnlyRectangleF              left, ReadOnlyRectangleF               right ) => Comparer<ReadOnlyRectangleF>.Default.Compare(left, right) <= 0;
    public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF self, int                              value ) => new(self.X, self.Y, self.Width + value, self.Height + value);
    public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF self, float                            value ) => new(self.X, self.Y, self.Width + value, self.Height + value);
    public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF self, double                           value ) => new(self.X, self.Y, (float)( self.Width + value ), (float)( self.Height + value ));
    public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF self, int                              value ) => new(self.X, self.Y, self.Width - value, self.Height - value);
    public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF self, float                            value ) => new(self.X, self.Y, self.Width - value, self.Height - value);
    public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF self, double                           value ) => new(self.X, self.Y, (float)( self.Width - value ), (float)( self.Height - value ));
    public static ReadOnlyRectangleF operator *( ReadOnlyRectangleF self, int                              value ) => new(self.X, self.Y, self.Width * value, self.Height * value);
    public static ReadOnlyRectangleF operator *( ReadOnlyRectangleF self, float                            value ) => new(self.X, self.Y, self.Width * value, self.Height * value);
    public static ReadOnlyRectangleF operator *( ReadOnlyRectangleF self, double                           value ) => new(self.X, self.Y, (float)( self.Width * value ), (float)( self.Height * value ));
    public static ReadOnlyRectangleF operator /( ReadOnlyRectangleF self, int                              value ) => new(self.X, self.Y, self.Width / value, self.Height / value);
    public static ReadOnlyRectangleF operator /( ReadOnlyRectangleF self, float                            value ) => new(self.X, self.Y, self.Width / value, self.Height / value);
    public static ReadOnlyRectangleF operator /( ReadOnlyRectangleF self, double                           value ) => new(self.X, self.Y, (float)( self.Width / value ), (float)( self.Height / value ));
    public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF self, (int xOffset, int yOffset)       value ) => new(self.X + value.xOffset, self.Y + value.yOffset, self.Width, self.Height);
    public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF self, (float xOffset, float yOffset)   value ) => new(self.X + value.xOffset, self.Y + value.yOffset, self.Width, self.Height);
    public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF self, (double xOffset, double yOffset) value ) => new((float)( self.X + value.xOffset ), (float)( self.Y + value.yOffset ), self.Width, self.Height);
    public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF self, (int xOffset, int yOffset)       value ) => new(self.X - value.xOffset, self.Y - value.yOffset, self.Width, self.Height);
    public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF self, (float xOffset, float yOffset)   value ) => new(self.X - value.xOffset, self.Y - value.yOffset, self.Width, self.Height);
    public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF self, (double xOffset, double yOffset) value ) => new((float)( self.X - value.xOffset ), (float)( self.Y - value.yOffset ), self.Width, self.Height);
    public static ReadOnlyRectangleF operator *( ReadOnlyRectangleF self, (int xOffset, int yOffset)       value ) => new(self.X * value.xOffset, self.Y * value.yOffset, self.Width, self.Height);
    public static ReadOnlyRectangleF operator *( ReadOnlyRectangleF self, (float xOffset, float yOffset)   value ) => new(self.X * value.xOffset, self.Y * value.yOffset, self.Width, self.Height);
    public static ReadOnlyRectangleF operator *( ReadOnlyRectangleF self, (double xOffset, double yOffset) value ) => new((float)( self.X * value.xOffset ), (float)( self.Y * value.yOffset ), self.Width, self.Height);
    public static ReadOnlyRectangleF operator /( ReadOnlyRectangleF self, (int xOffset, int yOffset)       value ) => new(self.X / value.xOffset, self.Y / value.yOffset, self.Width, self.Height);
    public static ReadOnlyRectangleF operator /( ReadOnlyRectangleF self, (float xOffset, float yOffset)   value ) => new(self.X / value.xOffset, self.Y / value.yOffset, self.Width, self.Height);
    public static ReadOnlyRectangleF operator /( ReadOnlyRectangleF self, (double xOffset, double yOffset) value ) => new((float)( self.X / value.xOffset ), (float)( self.Y / value.yOffset ), self.Width, self.Height);
    public static JsonSerializerContext              JsonContext   => JakarShapesContext.Default;
    public static JsonTypeInfo<ReadOnlyRectangleF>   JsonTypeInfo  => JakarShapesContext.Default.ReadOnlyRectangleF;
    public static JsonTypeInfo<ReadOnlyRectangleF[]> JsonArrayInfo => JakarShapesContext.Default.ReadOnlyRectangleFArray;
    public static bool TryFromJson( string? json, out ReadOnlyRectangleF result )
    {
        try
        {
            if ( string.IsNullOrWhiteSpace(json) )
            {
                result = Invalid;
                return false;
            }

            result = FromJson(json);
            return true;
        }
        catch ( Exception e ) { SelfLogger.WriteLine("{Exception}", e.ToString()); }

        result = Invalid;
        return false;
    }
    public static ReadOnlyRectangleF FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));
}
