namespace Jakar.SettingsView.Abstractions;


public interface ISvCell : ISvCellTitle
{
    public bool       IsEnabled { get; set; }
    public bool       IsVisible { get; set; }
    public WidgetType Type      { get; }
}
