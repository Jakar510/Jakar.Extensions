// Jakar.Extensions :: Jakar.SettingsView.Blazor
// 08/14/2024  22:08

using System.Globalization;
using Jakar.SettingsView.Abstractions;



namespace Jakar.SettingsView.Blazor.Controls;


public abstract class ValueCellBase<T> : DescriptionCellBase, ISvCellValue<T>
    where T : IEquatable<T>, IComparable<T>
{
    protected static readonly T                          _default = default!;
    public static             IEqualityComparer<T>       Equalizer           { get; set; } = EqualityComparer<T>.Default;
    public static             IComparer<T>               Sorter              { get; set; } = Comparer<T>.Default;
    [Parameter] public        string?                    Hint                { get; set; }
    [Parameter] public        EventCallback<string?>     HintChanged         { get; set; }
    [Parameter] public        Expression<Func<string?>>? HintExpression      { get; set; }
    [Parameter] public        T?                         Value               { get; set; }
    [Parameter] public        EventCallback<T?>          ValueChanged        { get; set; }
    [Parameter] public        Expression<Func<T?>>?      ValueExpression     { get; set; }
    [Parameter] public        string?                    ValueText           { get; set; }
    [Parameter] public        EventCallback<string?>     ValueTextChanged    { get; set; }
    [Parameter] public        Expression<Func<string?>>? ValueTextExpression { get; set; }
    [Parameter] public        string?                    ValueDisplayFormat  { get; set; }
    [Parameter] public        T?                         Max                 { get; set; }
    [Parameter] public        T?                         Min                 { get; set; }
    public virtual            bool                       IsValid             => Value is not null || Equalizer.Equals( _default, Value ) is false;


    public async ValueTask SetValue( T? value )
    {
        if ( EqualityComparer<T>.Default.Equals( Value, value ) ) { return; }

        Value = value;
        await ValueChanged.InvokeAsync( value );

        await SetValue( value is IFormattable formattable
                            ? formattable.ToString( ValueDisplayFormat, Section.Sv.CurrentCulture )
                            : value?.ToString() );
    }
    public async ValueTask SetValue( string? value )
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
