namespace Jakar.Xml;


public static class NodeNames
{
    /// <summary> Maps <see cref="Type.FullName"/> to <see cref="Type"/> .
    ///     <para> Uses <see cref="RegisterNodeName"/> to register the name. </para>
    /// </summary>
    private static readonly ConcurrentDictionary<string, Type> __nodeNameToType = new();

    /// <summary> Maps <see cref="Type.FullName"/> to <see cref="Type"/> .
    ///     <para> Uses <see cref="RegisterNodeName"/> to register the name. </para>
    /// </summary>
    private static readonly ConcurrentDictionary<Type, string> __typeToNodeName = new();
    private static bool GetName( Type type, [NotNullWhen(true)] out string? nodeName ) => __typeToNodeName.TryGetValue(type, out nodeName);


    private static bool GetType( string nodeName, [NotNullWhen(true)] out Type? type ) => __nodeNameToType.TryGetValue(nodeName, out type);

    public static string GetNodeName( this Type type, in bool useFullName = false )
    {
        if ( GetName(type, out string? nodeName) ) { return nodeName; }

        ReadOnlySpan<char> name = ( useFullName
                                        ? type.FullName
                                        : type.Name ).AsSpan();


        if ( type.IsArray ) { return Constants.GROUP; }

        nodeName = name.GetXmlName();
        RegisterNodeName(type, nodeName);
        return nodeName;
    }

    public static string GetTypeName( this Type type, in bool useFullName = false )
    {
        ReadOnlySpan<char> name = ( useFullName
                                        ? type.FullName
                                        : type.Name ).AsSpan();

        if ( type.IsArray ) { return name.GetXmlName('[', ']'); }

        return name.GetXmlName();
    }


    public static string GetXmlName( this ReadOnlySpan<char> name, params char[] trimmedChars )
    {
        name = name.TrimEnd(trimmedChars);
        int index = name.IndexOf(Constants.Dividers.TYPES);

        if ( index < 0 ) { return name.ToString(); }

        return name[..index]
           .ToString();
    }

    private static void AddOrUpdate( Type type, string nodeName )
    {
        Type addValue( string s ) => type;

        Type updateValue( string s, Type t ) => type;

        __nodeNameToType.AddOrUpdate(nodeName, addValue, updateValue);
    }

    private static void AddOrUpdate( string nodeName, Type type )
    {
        string addValue( Type t ) => nodeName;

        string updateValue( Type t, string s ) => nodeName;

        __typeToNodeName.AddOrUpdate(type, addValue, updateValue);
    }


    public static void RegisterNodeName( Type type, string? nodeName = null )
    {
        ReadOnlySpan<char> name = nodeName ??
                                  ( type.IsArray
                                        ? Constants.GROUP
                                        : type.Name );


        string result = name.GetXmlName();

        AddOrUpdate(type,   result);
        AddOrUpdate(result, type);
    }


    public static XmlNodeAttribute GetXmlNodeAttribute( this Type type ) => type.GetCustomAttribute<XmlNodeAttribute>(false) ?? XmlNodeAttribute.Default(type);
}
