namespace Jakar.Database;


public class DateTimeOffsetHandler : SqlConverter<DateTimeOffsetHandler, DateTimeOffset>
{
    public override DateTimeOffset Parse( object value ) =>
        value switch
        {
            DateTime item                                                               => DateTime.SpecifyKind( item, DateTimeKind.Utc ),
            DateTimeOffset item                                                         => item,
            string item when DateTimeOffset.TryParse( item, out DateTimeOffset offset ) => offset,
            string item when DateTime.TryParse( item, out DateTime offset )             => offset,
            _                                                                           => throw new ExpectedValueTypeException( nameof(value), value, typeof(DateTime), typeof(DateTimeOffset), typeof(string))
        };
    public override void SetValue( IDbDataParameter parameter, DateTimeOffset value )
    {
        parameter.Value  = value;
        parameter.DbType = DbType.DateTimeOffset;
    }
}
