namespace Jakar.SettingsView.Maui.Cells;


public interface ICellData : INotifyPropertyChanged
{
    public WidgetType Type        { get; }
    public bool       IsEnabled   { get; set; }
    public bool       IsVisible   { get; set; }
    public string?    ValueText   { get; set; }
    public string?    Title       { get; set; }
    public string?    Description { get; set; }
}



public interface ICellData<T> : IValidateCell<T>, ICellData
    where T : IEquatable<T>, IComparable<T>
{
    public bool IsValid { get; }
    public T    Value   { get; }
}



public interface IValidateCell<T>
    where T : IEquatable<T>, IComparable<T>
{
    public T? Max { get; }
    public T? Min { get; }

    public ErrorOrResult<T> Save();
}
