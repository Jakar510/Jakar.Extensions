namespace Jakar.SettingsView.Abstractions;


public interface ISvCellValue : ISvCellHint
{
    public string? ValueText { get; set; }
}



public interface ISvCellValue<TValue> : ISvCellValue, IValidator
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    public abstract static IEqualityComparer<TValue> Equalizer { get; }
    public abstract static IComparer<TValue>         Sorter    { get; }
    public                 TValue?                   Max       { get; set; }
    public                 TValue?                   Min       { get; set; }
    public                 TValue?                   Value     { get; set; }


    public event EventHandler<ChangedEventArgs<TValue>>? TextChanged;
}