using System.Runtime.CompilerServices;
using Dapper.Contrib.Extensions;



namespace Jakar.SqlBuilder;


public static class TableExtensions
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string GetTableName<TRecord>( this TRecord _ ) => typeof(TRecord).GetTableName();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string GetTableName<TRecord>()                 => typeof(TRecord).GetTableName();


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetTableName( this Type type )
    {
        string name = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>()?.Name ?? type.Name;

        // name = name.ToSnakeCase(CultureInfo.InvariantCulture)
        return name;
    }
}
