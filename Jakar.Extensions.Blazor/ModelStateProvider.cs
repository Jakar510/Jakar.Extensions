// Jakar.Extensions :: Jakar.Extensions.Blazor
// 09/26/2022  10:26 AM

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Jakar.Extensions.Blazor;


public sealed class ModelStateProvider : CascadingValue<ModelStateDictionary>
{
    public ModelStateProvider()
    {
        Value = new ModelStateDictionary();
        Name  = nameof(ModelStateDictionary);
    }
}
