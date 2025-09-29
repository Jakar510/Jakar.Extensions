// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/13/2024  20:08

using System.Globalization;



namespace Jakar.SettingsView.Maui.Sv.Base;


public abstract class ValueCellBase<TValue> : DescriptionCellBase, ISvCellValue<TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    public static readonly BindableProperty FormatProperty    = BindableProperty.Create( nameof(Format),    typeof(string), typeof(ValueCellBase<TValue>) );
    public static readonly BindableProperty HintProperty      = BindableProperty.Create( nameof(Hint),      typeof(string), typeof(ValueCellBase<TValue>) );
    public static readonly BindableProperty MaxProperty       = BindableProperty.Create( nameof(Value),     typeof(TValue),      typeof(ValueCellBase<TValue>) );
    public static readonly BindableProperty MinProperty       = BindableProperty.Create( nameof(Value),     typeof(TValue),      typeof(ValueCellBase<TValue>) );
    public static readonly BindableProperty ValueProperty     = BindableProperty.Create( nameof(Value),     typeof(TValue),      typeof(ValueCellBase<TValue>) );
    public static readonly BindableProperty ValueTextProperty = BindableProperty.Create( nameof(ValueText), typeof(string), typeof(ValueCellBase<TValue>) );


    public static  IEqualityComparer<TValue> Equalizer { get;                                        set; } = EqualityComparer<TValue>.Default;
    public static  IComparer<TValue>         Sorter    { get;                                        set; } = Comparer<TValue>.Default;
    public virtual string?                   Format    { get => (string?)GetValue( FormatProperty ); set => SetValue( FormatProperty, value ); }
    public virtual string?                   Hint      { get => (string?)GetValue( HintProperty );   set => SetValue( HintProperty,   value ); }
    public virtual bool                      IsValid   => Value is not null && !Equalizer.Equals( Value, default );
    public virtual TValue?                   Max       { get => (TValue?)GetValue( MaxProperty );         set => SetValue( MaxProperty,         value ); }
    public virtual TValue?                   Min       { get => (TValue?)GetValue( MinProperty );         set => SetValue( MinProperty,         value ); }
    public virtual TValue?                   Value     { get => (TValue?)GetValue( ValueProperty );       set => SetValue( ValueProperty,       value ); }
    public virtual string?                   ValueText { get => (string?)GetValue( DescriptionProperty ); set => SetValue( DescriptionProperty, value ); }


    public event EventHandler<ChangedEventArgs<TValue>>? TextChanged;


    public abstract ErrorOrResult<TValue> Save();

    public void SetValue( TValue? value )
    {
        if ( Equalizer.Equals( Value, value ) ) { return; }

        TValue? oldValue = Value;
        Value = value;

        ValueText = value is IFormattable formattable
                        ? formattable.ToString( Format, CultureInfo.CurrentUICulture )
                        : value?.ToString();

        TextChanged?.Invoke( this, new ChangedEventArgs<TValue>( oldValue, value ) );
    }
}
