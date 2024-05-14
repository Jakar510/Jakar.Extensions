// Jakar.Extensions :: Jakar.Database
// 09/09/2023  4:23 PM

namespace Jakar.Database;


public sealed class JsonAdditionalData : JsonSqlHandler<JsonAdditionalData, IDictionary<string, JToken?>?>
{
    public override void                          SetValue( IDbDataParameter parameter, IDictionary<string, JToken?>? value ) => parameter.Value = value?.ToJson();
    public override IDictionary<string, JToken?>? Parse( object?             value ) => Parse( value as string );
    public static   IDictionary<string, JToken?>? Parse( string?             value ) => value?.FromJson<IDictionary<string, JToken?>>();
}
