// Jakar.Extensions :: Jakar.Extensions
// 10/02/2026

namespace Jakar.Extensions;


public sealed partial class IniConfig
{
    /// <summary> Section name used by <see cref="FromJsonObject"/> for top-level values that aren't objects. </summary>
    public const string DEFAULT_SECTION = "Default";


    /// <summary>
    ///     <c> {"Section": {"key": "value"}} </c>. INI values are text: <c> true </c>/<c> false </c> become JSON booleans when <paramref name="inferBooleans"/> is set
    ///     (Jakar.Json won't read a string as a bool); everything else stays a string (the models read numbers from strings: <c> NumbersFromStrings = Allow </c>).
    /// </summary>
    public JObjectNode ToJsonObject( bool inferBooleans = true )
    {
        JObjectNode root = [];

        foreach ( ( string name, Section section ) in this )
        {
            JObjectNode values = [];

            foreach ( ( string key, string? value ) in section )
            {
                values[key] = value is null
                                  ? null
                                  : inferBooleans && bool.TryParse(value, out bool flag)
                                      ? flag
                                      : value;
            }

            root[name] = values;
        }

        return root;
    }


    /// <summary> The reverse of <see cref="ToJsonObject"/>: nested objects become sections, top-level scalars go into <see cref="DEFAULT_SECTION"/>; strings are written as-is, other values as JSON. </summary>
    public static IniConfig FromJsonObject( JObjectNode json )
    {
        ArgumentNullException.ThrowIfNull(json);
        IniConfig config = new();

        foreach ( ( string name, JNode? node ) in json )
        {
            if ( node is JObjectNode obj )
            {
                Section section = config[name];
                foreach ( ( string key, JNode? value ) in obj ) { section[key] = ToText(value); }
            }
            else { config[DEFAULT_SECTION][name] = ToText(node); }
        }

        return config;
    }


    private static string? ToText( JNode? node ) => node switch
                                                    {
                                                        null                                         => null,
                                                        JValueNode { Kind: JNodeKind.String } value => value.GetString(),
                                                        _                                            => node.ToJson(false)
                                                    };
}