﻿namespace Jakar.Database;


public static class TableExtensions
{
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static IDictionary<string, JToken?>? GetAdditionalData( this DbDataReader reader ) =>
        reader.GetFieldValue<string?>( nameof(JsonModels.IJsonModel.AdditionalData) )?.FromJson<Dictionary<string, JToken?>>();


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static string GetTableName( this object obj ) => obj.GetType().GetTableName();


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static string GetTableName<TRecord>()
        where TRecord : class => typeof(TRecord).GetTableName();
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static string GetTableName( this Type type )
    {
        string name = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>()?.Name ?? type.Name;

        // name = name.ToSnakeCase(CultureInfo.InvariantCulture)
        return name;
    }
}