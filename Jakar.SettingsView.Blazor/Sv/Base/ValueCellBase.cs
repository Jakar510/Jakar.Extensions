// Jakar.Extensions :: Jakar.SettingsView.Blazor
// 08/14/2024  22:08

using Jakar.SettingsView.Abstractions;



namespace Jakar.SettingsView.Blazor.Sv;


public abstract class ValueCellBase<TValue> : DescriptionCellBase, ISvCellValue<TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    protected static readonly TValue                     _default = default!;
    public static             IEqualityComparer<TValue>  Equalizer           { get; set; } = EqualityComparer<TValue>.Default;
    public static             IComparer<TValue>          Sorter              { get; set; } = Comparer<TValue>.Default;
    [Parameter] public        string?                    Hint                { get; set; }
    [Parameter] public        EventCallback<string?>     HintChanged         { get; set; }
    [Parameter] public        Expression<Func<string?>>? HintExpression      { get; set; }
    [Parameter] public        TValue?                    Value               { get; set; }
    [Parameter] public        EventCallback<TValue?>     ValueChanged        { get; set; }
    [Parameter] public        Expression<Func<TValue?>>? ValueExpression     { get; set; }
    [Parameter] public        string?                    ValueText           { get; set; }
    [Parameter] public        EventCallback<string?>     ValueTextChanged    { get; set; }
    [Parameter] public        Expression<Func<string?>>? ValueTextExpression { get; set; }
    [Parameter] public        string?                    ValueDisplayFormat  { get; set; }
    [Parameter] public        TValue?                    Max                 { get; set; }
    [Parameter] public        TValue?                    Min                 { get; set; }
    public virtual            bool                       IsValid             => Value is not null || !Equalizer.Equals( _default, Value );


    public event EventHandler<ChangedEventArgs<TValue>>? TextChanged;

    public async ValueTask SetValue( TValue? value )
    {
        if ( EqualityComparer<TValue>.Default.Equals( Value, value ) ) { return; }

        TextChanged?.Invoke( this, new ChangedEventArgs<TValue>( Value, value ) );
        Value = value;
        await ValueChanged.InvokeAsync( value );

        await SetValueString( value is IFormattable formattable
                                  ? formattable.ToString( ValueDisplayFormat, Section.Sv.CurrentCulture )
                                  : value?.ToString() );
    }
    public async ValueTask SetValueString( string? value )
    {
        if ( string.Equals( ValueText, value, StringComparison.Ordinal ) ) { return; }

        ValueText = value;
        await ValueTextChanged.InvokeAsync( value );
    }
    public async ValueTask SetHint( string? value )
    {
        if ( string.Equals( Hint, value, StringComparison.Ordinal ) ) { return; }

        Hint = value;
        await HintChanged.InvokeAsync( value );
    }
}
