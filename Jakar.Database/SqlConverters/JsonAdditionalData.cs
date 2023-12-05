// Jakar.Extensions :: Jakar.Database
// 09/09/2023  4:23 PM

namespace Jakar.Database;


public sealed class JsonAdditionalData : SqlConverter<JsonAdditionalData, IDictionary<string, JsonElement>?>
{
    public override void                          SetValue( IDbDataParameter parameter, IDictionary<string, JsonElement>? value ) => parameter.Value = value?.ToJson();
    public override IDictionary<string, JsonElement>? Parse( object?             value ) => Parse( value?.ToString() );
    public static   IDictionary<string, JsonElement>? Parse( string?             value ) => value?.FromJson<Dictionary<string, JsonElement>>();
}
