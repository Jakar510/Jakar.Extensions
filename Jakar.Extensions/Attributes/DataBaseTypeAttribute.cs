#nullable enable
namespace Jakar.Extensions;


[AttributeUsage( AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct )]
public sealed class DataBaseTypeAttribute : Attribute
{
    public DbType Type { get; init; }

    public DataBaseTypeAttribute( DbType type ) => Type = type;
}
