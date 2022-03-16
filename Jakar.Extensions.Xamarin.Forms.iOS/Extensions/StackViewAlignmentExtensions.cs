using UIKit;
using Xamarin.Forms;





namespace Jakar.Extensions.Xamarin.Forms.iOS.Extensions;


[Foundation.Preserve(AllMembers = true)]
public static class StackViewAlignmentExtensions
{
    public static UIStackViewAlignment ToNativeVertical( this LayoutAlignment forms )
    {
        return forms switch
               {
                   LayoutAlignment.Start  => UIStackViewAlignment.Leading,
                   LayoutAlignment.Center => UIStackViewAlignment.Center,
                   LayoutAlignment.End    => UIStackViewAlignment.Trailing,
                   _                      => UIStackViewAlignment.Fill
               };
    }

    public static UIStackViewAlignment ToNativeHorizontal( this LayoutAlignment forms )
    {
        return forms switch
               {
                   LayoutAlignment.Start  => UIStackViewAlignment.Top,
                   LayoutAlignment.Center => UIStackViewAlignment.Center,
                   LayoutAlignment.End    => UIStackViewAlignment.Bottom,
                   _                      => UIStackViewAlignment.Fill
               };
    }
}
