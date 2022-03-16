// unset

using System;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;





namespace Jakar.Extensions.Xamarin.Forms.iOS.Extensions;


public static class DrawingExtensions
{
    public static CGContext Context => UIGraphics.GetCurrentContext();

    public static void CenterCircle( this CGRect   bounds,
                                     in   nfloat   factor,
                                     in   bool     isChecked,
                                     in   CGColor? on,
                                     in   CGColor? off
    ) =>
        bounds.CenterCircle(factor,
                            isChecked,
                            on,
                            off,
                            1
                           );

    public static void CenterCircle( this CGRect   bounds,
                                     in   nfloat   factor,
                                     in   bool     isChecked,
                                     in   CGColor? on,
                                     in   CGColor? off,
                                     in   nfloat   alpha
    )
    {
        nfloat width  = bounds.Width * factor;
        nfloat height = bounds.Height * factor;
        nfloat x      = bounds.X + bounds.Width - width;
        nfloat y      = bounds.Y + bounds.Height - height;

        Context.CenterCircle(x,
                             y,
                             width,
                             height,
                             isChecked,
                             on,
                             off,
                             alpha
                            );
    }

    public static void CenterCircle( this CGContext con,
                                     in   nfloat    x,
                                     in   nfloat    y,
                                     in   nfloat    width,
                                     in   nfloat    height,
                                     in   bool      isChecked,
                                     in   CGColor?  on,
                                     in   CGColor?  off,
                                     in   nfloat    alpha
    ) =>
        con.CenterCircle(new CGRect(x, y, width, height),
                         isChecked,
                         on,
                         off,
                         alpha
                        );

    public static void CenterCircle( this CGContext con,
                                     CGRect         rect,
                                     in bool        isChecked,
                                     in CGColor?    on,
                                     in CGColor?    off,
                                     in nfloat      alpha
    )
    {
        con.SetAlpha(alpha);

        if ( isChecked )
        {
            CGColor color = on ?? Color.Accent.ToCGColor();
            con.DrawCircle(rect, 5, color);
            con.StrokePath();

            con.DrawCircle(rect, 8, color);
            con.FillPath();
        }
        else
        {
            con.DrawCircle(rect, 5, off ?? Color.Default.ToCGColor());
            con.StrokePath();
        }
    }

    public static void DrawCircle( this CGContext con, in CGRect bounds, in nfloat padding, in CGColor color ) =>
        con.DrawCircle(color,
                       padding,
                       padding,
                       bounds.Width - 2 * padding,
                       bounds.Height - 2 * padding
                      );

    public static void DrawCircle( this CGContext con,
                                   in   CGColor   color,
                                   in   nfloat    x,
                                   in   nfloat    y,
                                   in   nfloat    width,
                                   in   nfloat    height
    ) =>
        con.DrawCircle(color, new CGRect(x, y, width, height));

    public static void DrawCircle( this CGContext con, in CGColor color, in CGRect bounds )
    {
        con.SetStrokeColor(color);
        con.AddEllipseInRect(bounds);
    }
}
