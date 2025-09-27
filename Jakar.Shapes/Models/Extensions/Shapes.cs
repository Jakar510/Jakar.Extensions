// Jakar.Extensions :: Jakar.Extensions
// 07/11/2025  15:58

namespace Jakar.Shapes;


public delegate TValue RefSelect<TValue>( ref readonly TValue value );



public delegate TOutput RefSelect<TInput, out TOutput>( ref readonly TInput value );



public static class Shapes
{
    public static ReadOnlyLine RadiusLine<TCircle>( this TCircle self, in Radians radians )
        where TCircle : struct, ICircle<TCircle> => new(self.Center, new ReadOnlyPoint(self.Center.X + self.Radius * Math.Cos(radians.Value), self.Center.Y + self.Radius * Math.Sin(radians.Value)));
    public static CalculatedLine RadiusCalculatedLine<TCircle>( this TCircle self, in Radians radians )
        where TCircle : struct, ICircle<TCircle>
    {
        ReadOnlyPoint center = self.Center;
        ReadOnlyPoint end   = new(self.Center.X + self.Radius * Math.Cos(radians.Value), self.Center.Y + self.Radius * Math.Sin(radians.Value));

        double dx = end.X - center.X;
        double dy = end.Y - center.Y;

        if ( Math.Abs(dx) < double.Epsilon ) { return CalculatedLine.Create(x => self.Center.X); }

        double m = dy / dx;
        double b = center.Y - m * center.X;

        return CalculatedLine.Create(x => m * x + b);
    }


    public static ReadOnlyLine DiameterLine<TCircle>( this TCircle self, in Radians radians )
        where TCircle : struct, ICircle<TCircle>
    {
        ReadOnlyPoint center = self.Center;
        ReadOnlyPoint start  = new(center.X - self.Radius * Math.Cos(radians.Value), center.Y - self.Radius * Math.Sin(radians.Value));
        ReadOnlyPoint end    = new(center.X + self.Radius * Math.Cos(radians.Value), center.Y + self.Radius * Math.Sin(radians.Value));
        return new ReadOnlyLine(start, end);
    }
    public static CalculatedLine DiameterCalculatedLine<TCircle>( this TCircle self, in Radians radians )
        where TCircle : struct, ICircle<TCircle>
    {
        ReadOnlyPoint center = self.Center;
        ReadOnlyPoint start  = new(center.X - self.Radius * Math.Cos(radians.Value), center.Y - self.Radius * Math.Sin(radians.Value));
        ReadOnlyPoint end    = new(center.X + self.Radius * Math.Cos(radians.Value), center.Y + self.Radius * Math.Sin(radians.Value));

        double dx = end.X - start.X;
        double dy = end.Y - start.Y;

        if ( Math.Abs(dx) < double.Epsilon ) { return CalculatedLine.Create(x => start.X); }

        double m = dy / dx;
        double b = start.Y - m * start.X;

        return CalculatedLine.Create(x => m * x + b);
    }


    public static TValue[]? Create<TValue>( this TValue[]? self, RefSelect<TValue> func )
    {
        ReadOnlySpan<TValue> span = self;
        if ( span.IsEmpty ) { return null; }

        TValue[] buffer = GC.AllocateUninitializedArray<TValue>(span.Length);
        int      index  = 0;

        foreach ( ref readonly TValue value in span ) { buffer[index++] = func(in value); }

        return buffer;
    }
    public static TOutput[]? Create<TInput, TOutput>( this TInput[]? self, RefSelect<TInput, TOutput> func )
    {
        ReadOnlySpan<TInput> span = self;
        if ( span.IsEmpty ) { return null; }

        TOutput[] buffer = GC.AllocateUninitializedArray<TOutput>(span.Length);
        int       index  = 0;

        foreach ( ref readonly TInput value in span ) { buffer[index++] = func(in value); }

        return buffer;
    }


    public static bool IsAtLeast<TRectangle, TSize>( this TRectangle self, in TSize other )
        where TRectangle : IRectangle<TRectangle>
        where TSize : ISize<TSize> => other.Width <= self.Width && other.Height <= self.Height;
    public static bool Contains<TRectangle, TPoint>( this TRectangle self, in TPoint other )
        where TRectangle : IRectangle<TRectangle>
        where TPoint : IPoint<TPoint> => other.X >= self.X && other.X < self.Right && other.Y >= self.Y && other.Y < self.Bottom;


    public static bool Contains<TRectangle, TPoint>( this TRectangle self, params ReadOnlySpan<TPoint> points )
        where TRectangle : IRectangle<TRectangle>
        where TPoint : IPoint<TPoint>
    {
        foreach ( ref readonly TPoint point in points )
        {
            if ( self.Contains(in point) ) { return true; }
        }

        return false;
    }


    public static bool ContainsAny<TRectangle, TPoint>( this TRectangle self, params ReadOnlySpan<TPoint> others )
        where TRectangle : IRectangle<TRectangle>
        where TPoint : IPoint<TPoint>
    {
        foreach ( ref readonly TPoint point in others )
        {
            if ( self.Contains(in point) ) { return true; }
        }

        return false;
    }
    public static bool ContainsAll<TRectangle, TPoint>( this TRectangle self, params ReadOnlySpan<TPoint> others )
        where TRectangle : IRectangle<TRectangle>
        where TPoint : IPoint<TPoint>
    {
        foreach ( ref readonly TPoint point in others )
        {
            if ( !self.Contains(in point) ) { return false; }
        }

        return true;
    }


    [Pure] public static TRectangle Union<TRectangle, TOther>( this TRectangle self, in TOther other )
        where TRectangle : IRectangle<TRectangle>
        where TOther : IRectangle<TOther>
    {
        double x      = Math.Min(self.X, other.X);
        double y      = Math.Min(self.Y, other.Y);
        double width  = Math.Max(self.X + self.Width, other.X + other.Width) - self.X;
        double height = Math.Max(self.Y + self.Height, other.Y + other.Height) - self.Y;

        return width < 0 || height < 0
                   ? TRectangle.Zero
                   : TRectangle.Create(x, y, width, height);
    }


    [Pure] public static TRectangle Intersection<TRectangle, TOther>( this TRectangle self, in TOther other )
        where TRectangle : IRectangle<TRectangle>
        where TOther : IRectangle<TOther>
    {
        double x      = Math.Max(self.X, other.X);
        double y      = Math.Max(self.Y, other.Y);
        double width  = Math.Min(self.X + self.Width,  other.X + other.Width)  - x;
        double height = Math.Min(self.Y + self.Height, other.Y + other.Height) - y;

        return width < 0 || height < 0
                   ? TRectangle.Zero
                   : TRectangle.Create(x, y, width, height);
    }


    public static bool IntersectsWith<TRectangle, TOther>( this TRectangle self, in TOther other )
        where TRectangle : IRectangle<TRectangle>
        where TOther : IRectangle<TOther> => !( self.Left >= other.Right || self.Right <= other.Left || self.Top >= other.Bottom || self.Bottom <= other.Top );


    public static bool DoesLineIntersect<TRectangle, TPoint>( this TRectangle self, in TPoint source, in TPoint target )
        where TRectangle : IRectangle<TRectangle>
        where TPoint : IPoint<TPoint>
    {
        double               t0          = 0.0;
        double               t1          = 1.0;
        double               dx          = target.X - source.X;
        double               dy          = target.Y - source.Y;
        ReadOnlySpan<double> boundariesX = [self.X, self.X + self.Width];
        ReadOnlySpan<double> boundariesY = [self.Y, self.Y + self.Height];

        for ( int i = 0; i < 2; i++ )
        {
            double pX = i == 0
                            ? -dx
                            : dx;

            double pY = i == 0
                            ? -dy
                            : dy;

            for ( int j = 0; j < 2; j++ )
            {
                double qX = j == 0
                                ? source.X       - boundariesX[i]
                                : boundariesX[i] - source.X;

                double qY = j == 0
                                ? source.Y       - boundariesY[i]
                                : boundariesY[i] - source.Y;

                if ( pX == 0 && qX < 0 ) { return false; } // Line is parallel to the self's horizontal edge and outside of it

                if ( pY == 0 && qY < 0 ) { return false; } // Line is parallel to the self's vertical edge and outside of it

                double rX = pX != 0
                                ? qX / pX
                                : double.MaxValue;

                double rY = pY != 0
                                ? qY / pY
                                : double.MaxValue;

                if ( pX < 0 ) { t0 = Math.Max(t0, rX); }
                else { t1          = Math.Min(t1, rX); }

                if ( pY < 0 ) { t0 = Math.Max(t0, rY); }
                else { t1          = Math.Min(t1, rY); }

                if ( t0 > t1 ) { return false; }
            }
        }

        return true;
    }
}
