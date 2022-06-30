#nullable enable
namespace Jakar.Extensions;


public static class DapperTableExtensions
{
    public static string GetTableName(this object obj) => obj.GetType().GetTableName();

    public static string GetTableName(this Type classType)
    {
        string table = classType.GetCustomAttribute<TableAttribute>()?.Name ?? classType.Name;
        return table;
    }
}
