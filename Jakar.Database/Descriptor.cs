// Jakar.Extensions :: Jakar.Database
// 08/17/2022  8:48 PM

namespace Jakar.Database;


public sealed record Descriptor
{
    public string               Name         { get; init; }
    public string               ColumnName   { get; init; }
    public string               VariableName { get; init; }
    public string               UpdateName   { get; init; }
    public bool                 IsKey        { get; init; }
    public Func<object, object> GetValue     { get; init; }


    public Descriptor( PropertyInfo property )
    {
        MethodInfo method = property.GetMethod ?? throw new ArgumentNullException(nameof(property), nameof(property.GetMethod));
        Name         = property.Name;
        ColumnName   = $" {Name} ";
        VariableName = $" @{Name} ";
        UpdateName   = $"Name = @{Name} ";
        IsKey        = property.GetCustomAttribute<Dapper.Contrib.Extensions.KeyAttribute>() is not null || property.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() is not null;


        GetValue = Emit<Func<object, object>>.NewDynamicMethod(typeof(Descriptor))
                                             .LoadArgument(0)
                                             .CastClass(property.DeclaringType)
                                             .Call(method)
                                             .Return()
                                             .CreateDelegate();
    }


    public static implicit operator Descriptor( PropertyInfo property ) => new(property);
}
