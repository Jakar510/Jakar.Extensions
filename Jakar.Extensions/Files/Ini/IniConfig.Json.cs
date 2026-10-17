// Jakar.Extensions :: Jakar.Extensions
// 10/02/2026

namespace Jakar.Extensions;


public sealed partial class IniConfig
{
    /// <summary> Section name used by <see cref="FromJsonObject"/> for top-level values that aren't objects. </summary>
    public const string DEFAULT_SECTION = "Default";


    /// <summary>
    ///     <c> {"Section": {"key": "value"}} </c>. INI values are text: <c> true </c>/<c> false </c> become JSON booleans when <paramref name="inferBooleans"/> is set
    ///     (System.Text.Json won't read a string as a bool); everything else stays a string (numbers in strings are read with <see cref="JsonNumberHandling.AllowReadingFromString"/>).
    /// </summary>
    public JsonObject ToJsonObject( bool inferBooleans = true )
    {
        JsonObject root = new(Json.NodeOptions);

        foreach ( ( string name, Section section ) in this )
        {
            JsonObject values = new(Json.NodeOptions);

            foreach ( ( string key, string? value ) in section )
            {
                values[key] = value is null
                                  ? null
                                  : inferBooleans && bool.TryParse(value, out bool flag)
                                      ? JsonValue.Create(flag)
                                      : JsonValue.Create(value);
            }

            root[name] = values;
        }

        return root;
    }


    /// <summary> The reverse of <see cref="ToJsonObject"/>: nested objects become sections, top-level scalars go into <see cref="DEFAULT_SECTION"/>; strings are written as-is, other values as JSON. </summary>
    public static IniConfig FromJsonObject( JsonObject json )
    {
        ArgumentNullException.ThrowIfNull(json);
        IniConfig config = new();

        foreach ( ( string name, JsonNode? node ) in json )
        {
            if ( node is JsonObject obj )
            {
                Section section = config[name];
                foreach ( ( string key, JsonNode? value ) in obj ) { section[key] = ToText(value); }
            }
            else { config[DEFAULT_SECTION][name] = ToText(node); }
        }

        return config;
    }


    private static string? ToText( JsonNode? node ) => node switch
                                                       {
                                                           null                                                              => null,
                                                           JsonValue value when value.GetValueKind() == JsonValueKind.String => value.GetValue<string>(),
                                                           _                                                                 => node.ToJsonString()
                                                       };
}
