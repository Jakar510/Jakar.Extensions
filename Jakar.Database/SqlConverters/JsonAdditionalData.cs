// Jakar.Extensions :: Jakar.Database
// 09/09/2023  4:23 PM

namespace Jakar.Database;


public sealed class JsonAdditionalData : SqlConverter<JsonAdditionalData, JsonObject?>
{
    public override void                          SetValue( IDbDataParameter parameter, JsonObject? value ) => parameter.Value = value?.ToJson();
    public override JsonObject? Parse( object?             value ) => Parse( value?.ToString() );
    public static   JsonObject? Parse( string?             value ) => value?.FromJson<JsonObject>();
}
