﻿using System;
using System.ComponentModel.DataAnnotations.Schema;



namespace Jakar.Xml;


public static class TableExtensions
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string GetTableName( this object obj ) => obj.GetType().GetTableName();


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetTableName<TRecord>()
        where TRecord : class => typeof(TRecord).GetTableName();
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetTableName( this Type type )
    {
        string name = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;

        // name = name.ToSnakeCase(CultureInfo.InvariantCulture)
        return name;
    }
}
