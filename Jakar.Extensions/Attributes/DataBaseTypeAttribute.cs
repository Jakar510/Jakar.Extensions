namespace Jakar.Extensions;


[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class DataBaseTypeAttribute( DbType type ) : Attribute
{
    public DbType Type { get; init; } = type;
}
