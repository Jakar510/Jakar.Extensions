﻿namespace Jakar.Database;


public class DateOnlyHandler : SqlConverter<DateOnlyHandler, DateOnly>
{
    public static DateOnly Get( in DateTime       value ) => DateOnly.FromDateTime( value );
    public static DateOnly Get( in DateTimeOffset value ) => Get( value.DateTime );
    public override DateOnly Parse( object value ) =>
        value switch
        {
            DateTime item                                                               => Get( item ),
            DateTimeOffset item                                                         => Get( item ),
            DateOnly item                                                               => item,
            string item when DateOnly.TryParse( item, out DateOnly offset )             => offset,
            string item when DateTime.TryParse( item, out DateTime offset )             => Get( offset ),
            string item when DateTimeOffset.TryParse( item, out DateTimeOffset offset ) => Get( offset ),
            _                                                                           => throw new ExpectedValueTypeException( nameof(value), value, typeof(DateOnly), typeof(string) )
        };
    public override void SetValue( IDbDataParameter parameter, DateOnly value )
    {
        parameter.Value  = DateTime.SpecifyKind( value.ToDateTime( TimeOnly.MinValue ), DateTimeKind.Utc );
        parameter.DbType = DbType.DateTime2;
    }
}
