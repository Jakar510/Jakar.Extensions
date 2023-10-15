// Jakar.Extensions :: Jakar.Database
// 09/09/2023  4:23 PM

namespace Jakar.Database;


public sealed class DapperJsonExtensionData : SqlConverter<DapperJsonExtensionData, IDictionary<string, JToken?>?>
{
    public override void SetValue( IDbDataParameter              parameter, IDictionary<string, JToken?>? value ) { }
    public override IDictionary<string, JToken?>? Parse( object? value ) => Parse( value?.ToString() );
    public static IDictionary<string, JToken?>? Parse( string?   value ) => value?.FromJson<Dictionary<string, JToken?>>();
}
