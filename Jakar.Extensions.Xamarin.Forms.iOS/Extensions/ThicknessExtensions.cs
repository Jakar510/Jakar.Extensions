#nullable enable
using UIKit;
using Xamarin.Forms;



namespace Jakar.Extensions.Xamarin.Forms.iOS;


[Foundation.Preserve(AllMembers = true)]
public static class ThicknessExtensions
{
    // ReSharper disable once InconsistentNaming
    public static UIEdgeInsets ToUIEdgeInsets( this Thickness forms ) => new(forms.Top.ToNFloat(), forms.Left.ToNFloat(), forms.Bottom.ToNFloat(), forms.Right.ToNFloat());
}
