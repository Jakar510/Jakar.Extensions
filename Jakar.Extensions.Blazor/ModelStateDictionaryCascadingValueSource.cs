// Jakar.Extensions :: Jakar.Extensions.Blazor
// 04/25/2024  11:04

namespace Jakar.Extensions.Blazor;


public sealed class ModelStateDictionaryCascadingValueSource( ModelStateDictionary value, bool isFixed ) : CascadingValueSource<ModelStateDictionary>( KEY, value, isFixed )
{
    public const string KEY = "MODEL_STATE";

    public ModelStateDictionaryCascadingValueSource() : this( new ModelStateDictionary(), false ) { }
}
