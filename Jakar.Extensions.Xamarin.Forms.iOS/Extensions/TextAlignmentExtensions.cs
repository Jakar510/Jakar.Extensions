using UIKit;
using Xamarin.Forms;





namespace Jakar.Extensions.Xamarin.Forms.iOS.Extensions;


[Foundation.Preserve(AllMembers = true)]
public static class TextAlignmentExtensions
{
    // ReSharper disable once InconsistentNaming
    public static UITextAlignment ToUITextAlignment( this TextAlignment forms )
    {
        return forms switch
               {
                   TextAlignment.Start  => UITextAlignment.Left,
                   TextAlignment.Center => UITextAlignment.Center,
                   TextAlignment.End    => UITextAlignment.Right,
                   _                    => UITextAlignment.Right
               };
    }
}
