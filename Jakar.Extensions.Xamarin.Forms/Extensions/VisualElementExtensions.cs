


namespace Jakar.Extensions.Xamarin.Forms.Extensions;


public static class VisualElementExtensions
{
    public static void Show( this VisualElement element ) => element.IsVisible = true;
    public static void Hide( this VisualElement element ) => element.IsVisible = false;


    public static void SetBackgroundColor( this VisualElement element )              => element.SetBackgroundColor(Color.Transparent);
    public static void SetBackgroundColor( this VisualElement element, Color color ) => element.BackgroundColor = color;
}
