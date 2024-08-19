// Jakar.Extensions :: Jakar.SettingsView.Blazor
// 08/14/2024  22:08

using System.ComponentModel;
using Jakar.SettingsView.Abstractions;



namespace Jakar.SettingsView.Blazor.Controls;


public abstract class DescriptionCellBase : CellBase, ISvCellDescription
{
    [Parameter] public string?                    Description           { get; set; }
    [Parameter] public EventCallback<string?>     DescriptionChanged    { get; set; }
    [Parameter] public Expression<Func<string?>>? DescriptionExpression { get; set; }
    public abstract    WidgetType                 Type                  { get; }


    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        await SetDescription( Description );
    }
    public async ValueTask SetDescription( string? value )
    {
        if ( string.Equals( Description, value, StringComparison.Ordinal ) ) { return; }

        Description = value;
        await DescriptionChanged.InvokeAsync( value );
    }
}
