namespace Jakar.Database;


public class DateTimeHandler : SqlConverter<DateTimeHandler, DateTime>
{
    public override DateTime Parse( object value ) =>
        value switch
        {
            DateTime item                                                   => DateTime.SpecifyKind( item,   DateTimeKind.Utc ),
            string item when DateTime.TryParse( item, out DateTime offset ) => DateTime.SpecifyKind( offset, DateTimeKind.Utc ),
            _                                                               => throw new ExpectedValueTypeException( nameof(value), value, [typeof(DateTime), typeof(string)] )
        };
    public override void SetValue( IDbDataParameter parameter, DateTime value )
    {
        parameter.Value  = DateTime.SpecifyKind( value, DateTimeKind.Utc );
        parameter.DbType = DbType.DateTime2;
    }
}
