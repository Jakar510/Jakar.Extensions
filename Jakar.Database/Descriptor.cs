// Jakar.Extensions :: Jakar.Database
// 08/17/2022  8:48 PM

namespace Jakar.Database;


public sealed record Descriptor
{
    public string Name         { get; init; }
    public string ColumnName   { get; init; }
    public string VariableName { get; init; }
    public bool   IsKey        { get; init; }


    public Descriptor( MemberInfo property )
    {
        Name         = property.Name;
        ColumnName   = $" {Name} ";
        VariableName = $" @{Name} ";
        IsKey        = property.GetCustomAttribute<Dapper.Contrib.Extensions.KeyAttribute>() is not null || property.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() is not null;
    }


    public static implicit operator Descriptor( MemberInfo property ) => new(property);
}
