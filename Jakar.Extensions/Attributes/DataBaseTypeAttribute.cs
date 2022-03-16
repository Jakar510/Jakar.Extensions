namespace Jakar.Extensions.Attributes;


[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public sealed class DataBaseTypeAttribute : Attribute
{
    public DbType Type { get; init; }

    public DataBaseTypeAttribute( DbType type ) => Type = type;
}
