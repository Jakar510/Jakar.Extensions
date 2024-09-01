// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/13/2024  20:08

using System.Globalization;



namespace Jakar.SettingsView.Maui.Sv.Base;


public abstract class ValueCellBase<T> : DescriptionCellBase, ISvCellValue<T>
    where T : IEquatable<T>, IComparable<T>
{
    public static readonly BindableProperty FormatProperty    = BindableProperty.Create( nameof(Format),    typeof(string), typeof(ValueCellBase<T>) );
    public static readonly BindableProperty HintProperty      = BindableProperty.Create( nameof(Hint),      typeof(string), typeof(ValueCellBase<T>) );
    public static readonly BindableProperty MaxProperty       = BindableProperty.Create( nameof(Value),     typeof(T),      typeof(ValueCellBase<T>) );
    public static readonly BindableProperty MinProperty       = BindableProperty.Create( nameof(Value),     typeof(T),      typeof(ValueCellBase<T>) );
    public static readonly BindableProperty ValueProperty     = BindableProperty.Create( nameof(Value),     typeof(T),      typeof(ValueCellBase<T>) );
    public static readonly BindableProperty ValueTextProperty = BindableProperty.Create( nameof(ValueText), typeof(string), typeof(ValueCellBase<T>) );


    public static  IEqualityComparer<T> Equalizer { get;                                        set; } = EqualityComparer<T>.Default;
    public static  IComparer<T>         Sorter    { get;                                        set; } = Comparer<T>.Default;
    public virtual string?              Format    { get => (string?)GetValue( FormatProperty ); set => SetValue( FormatProperty, value ); }
    public virtual string?              Hint      { get => (string?)GetValue( HintProperty );   set => SetValue( HintProperty,   value ); }
    public virtual bool                 IsValid   => Value is not null && Equalizer.Equals( Value, default ) is false;
    public virtual T?                   Max       { get => (T?)GetValue( MaxProperty );              set => SetValue( MaxProperty,         value ); }
    public virtual T?                   Min       { get => (T?)GetValue( MinProperty );              set => SetValue( MinProperty,         value ); }
    public virtual T?                   Value     { get => (T?)GetValue( ValueProperty );            set => SetValue( ValueProperty,       value ); }
    public virtual string?              ValueText { get => (string?)GetValue( DescriptionProperty ); set => SetValue( DescriptionProperty, value ); }


    public event EventHandler<ChangedEventArgs<T>>? TextChanged;


    public abstract ErrorOrResult<T> Save();

    public void SetValue( T? value )
    {
        if ( Equalizer.Equals( Value, value ) ) { return; }

        T? oldValue = Value;
        Value = value;

        ValueText = value is IFormattable formattable
                        ? formattable.ToString( Format, CultureInfo.CurrentUICulture )
                        : value?.ToString();

        TextChanged?.Invoke( this, new ChangedEventArgs<T>( oldValue, value ) );
    }
}
