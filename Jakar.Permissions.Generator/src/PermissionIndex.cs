namespace Jakar.Permissions.Generator;


/// <summary> Represents a permission identifier with a stable integer index. </summary>
public readonly record struct PermissionIndex( int Value, string Description )
{
    public readonly int    Value       = Value;
    public readonly string Description = Description;
}
