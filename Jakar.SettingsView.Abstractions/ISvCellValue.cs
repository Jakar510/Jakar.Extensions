namespace Jakar.SettingsView.Abstractions;


public interface ISvCellValue : ISvCellHint
{
    public string? ValueText { get; set; }
}



public interface ISvCellValue<T> : ISvCellValue, IValidator
    where T : IEquatable<T>, IComparable<T>
{
    public T? Max   { get; set; }
    public T? Min   { get; set; }
    public T? Value { get; set; }


    public abstract static IComparer<T>         Sorter    { get; }
    public abstract static IEqualityComparer<T> Equalizer { get; }
}
