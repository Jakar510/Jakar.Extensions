// Jakar.Extensions :: Jakar.Extensions.Blazor
// 09/26/2022  10:26 AM

namespace Jakar.Extensions.Blazor;


public sealed class ModelStateProvider : CascadingValue<ModelStateDictionary>
{
    public ModelStateProvider()
    {
        Value = new ModelStateDictionary();
        Name  = nameof(ModelStateDictionary);
    }
}
