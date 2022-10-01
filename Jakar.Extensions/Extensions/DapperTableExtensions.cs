#nullable enable
namespace Jakar.Extensions;


public static class DapperTableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetTableName( this object obj ) => obj.GetType()
                                                               .GetTableName();

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static string GetTableName<TRecord>() where TRecord : class => typeof(TRecord).GetTableName();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetTableName( this Type type ) => type.GetCustomAttribute<TableAttribute>()
                                                              ?.Name ?? type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>()
                                                                           ?.Name ?? type.Name;
}
