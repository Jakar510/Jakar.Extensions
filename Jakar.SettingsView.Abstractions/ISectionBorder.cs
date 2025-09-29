// Jakar.Extensions :: Jakar.SettingsView.Abstractions
// 08/13/2024  20:08

using System.Windows.Input;



namespace Jakar.SettingsView.Abstractions;


public interface ISectionBorder : IDisposable
{
    public string  Title      { get; set; }
    public double  FontSize   { get; set; }
    public string? FontFamily { get; set; }
}



public interface ISectionBorder<TTextColor> : ISectionBorder
{
    public TTextColor TextColor { get; set; }
}



public interface ISectionBorder<TTextColor, TImageSource> : ISectionBorder<TTextColor>
{
    public TImageSource? Collapsed     { get; set; }
    public TImageSource? Expanded      { get; set; }
    public TImageSource? IconSource    { get; set; }
    public bool          IsExpanded    { get; set; }
    public ICommand?     TappedCommand { get; set; }
}
