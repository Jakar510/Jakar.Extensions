// Jakar.Extensions :: Jakar.Extensions.Blazor
// 09/26/2022  10:14 AM

namespace Jakar.Extensions.Blazor;


public interface IModelState
{
    [CascadingParameter( Name = ModelStateDictionaryCascadingValueSource.KEY )] public ModelStateDictionary ModelState { get; set; }
}
