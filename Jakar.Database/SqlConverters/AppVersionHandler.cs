// Jakar.Extensions :: Jakar.Database
// 08/30/2022  11:07 PM

namespace Jakar.Database;


public class AppVersionHandler : SqlConverter<AppVersionHandler, AppVersion>
{
    public override AppVersion Parse( object value ) =>
        value switch
        {
            AppVersion item                                                      => item,
            string item when AppVersion.TryParse( item, out AppVersion? offset ) => offset,
            _                                                                    => throw new ExpectedValueTypeException( nameof(value), value, typeof(DateOnly), typeof(string) ),
        };
    public override void SetValue( IDbDataParameter parameter, AppVersion value )
    {
        parameter.Value  = value.ToString();
        parameter.DbType = DbType.String;
    }
}
