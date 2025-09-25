// Jakar.Extensions :: Jakar.Database
// 09/09/2023  4:23 PM

using Newtonsoft.Json.Linq;



namespace Jakar.Database;


public sealed class JsonAdditionalData : JsonSqlHandler<JsonAdditionalData, JsonObject?>
{
    public override JsonTypeInfo<JsonObject?> TypeInfo                                                  => JakarExtensionsContext.Default.JsonObject;
    public override void                      SetValue( IDbDataParameter parameter, JsonObject? value ) => parameter.Value = value?.ToJson();
    public override JsonObject?               Parse( object?             value ) => Parse(value as string);
    public static   JsonObject?               Parse( string?             value ) => value?.GetAdditionalData();
}
