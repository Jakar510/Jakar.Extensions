// Jakar.Extensions :: Jakar.Database
// 08/30/2022  11:01 PM

namespace Jakar.Database;


public class TimeOnlyHandler : SqlConverter<TimeOnlyHandler, TimeOnly>
{
    public static TimeOnly Get( in TimeSpan       value ) => new(value.Ticks);
    public static TimeOnly Get( in DateTime       value ) => Get( value.TimeOfDay );
    public static TimeOnly Get( in DateTimeOffset value ) => Get( value.TimeOfDay );
    public override TimeOnly Parse( object value ) =>
        value switch
        {
            DateTime item                                                               => Get( item ),
            DateTimeOffset item                                                         => Get( item ),
            TimeSpan item                                                               => Get( item ),
            string item when TimeOnly.TryParse( item, out TimeOnly offset )             => offset,
            string item when DateTime.TryParse( item, out DateTime offset )             => Get( offset ),
            string item when DateTimeOffset.TryParse( item, out DateTimeOffset offset ) => Get( offset ),
            _                                                                           => throw new ExpectedValueTypeException( nameof(value), value, [typeof(DateOnly), typeof(string)] )
        };
    public override void SetValue( IDbDataParameter parameter, TimeOnly value )
    {
        parameter.Value  = value.ToTimeSpan();
        parameter.DbType = DbType.Time;
    }
}
