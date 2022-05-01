using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Jakar.Extensions.Exceptions.General;
using Jakar.Extensions.Strings;
using Jakar.Extensions.Types;
using Jakar.Xml.Deserialization;
using Jakar.Xml.Serialization;



namespace Jakar.Xml;


public interface IXmlizer
{
    void Serialize( in XObject node );
    ReadOnlySpan<char> Name { get; set; }
}



/// <summary>
/// A PREDICTABLE (de)serializer for any given object.
/// <para>
///	Primary goals are to be PERFORMANT, easily human readable, predictable and consistent.
/// </para>
/// </summary>
public static partial class Xmlizer
{
    /// <summary>
    /// Maps <see cref="XmlNode.Name"/> to <see cref="Type"/>
    /// </summary>
    internal static readonly ConcurrentDictionary<string, Type> mapper = new();


    /// <summary>
    /// Maps <see cref="Type.FullName"/> to <see cref="Type"/>.
    /// <para>
    ///	Uses <see cref="NodeNames.GetTypeName"/> to get the <see cref="Type.FullName"/>
    /// </para>
    /// </summary>
    internal static readonly ConcurrentDictionary<string, Type> nameToType = new();


    /// <summary>
    /// Maps <see cref="Type.FullName"/> to <see cref="Type"/>.
    /// <para>
    ///	Uses <see cref="NodeNames.GetTypeName"/> to get the <see cref="Type.FullName"/>
    /// </para>
    /// </summary>
    internal static readonly ConcurrentDictionary<Type, Func<string, object>> parsers = new();


    /// <summary>
    /// register System built-ins
    /// </summary>
    static Xmlizer() => Register(typeof(bool?),
                                 typeof(byte?),
                                 typeof(sbyte?),
                                 typeof(Guid?),
                                 typeof(char?),
                                 typeof(DateTime?),
                                 typeof(int?),
                                 typeof(uint?),
                                 typeof(short?),
                                 typeof(ushort?),
                                 typeof(long?),
                                 typeof(ulong?),
                                 typeof(float?),
                                 typeof(double?),
                                 typeof(decimal?),
                                 typeof(TimeSpan?),
                                 typeof(bool),
                                 typeof(byte),
                                 typeof(sbyte),
                                 typeof(Guid),
                                 typeof(char),
                                 typeof(string),
                                 typeof(DateTime),
                                 typeof(int),
                                 typeof(uint),
                                 typeof(short),
                                 typeof(ushort),
                                 typeof(long),
                                 typeof(ulong),
                                 typeof(float),
                                 typeof(double),
                                 typeof(decimal),
                                 typeof(TimeSpan));


    // Register(typeof(IPAddress),
    // typeof(DnsEndPoint),
    // typeof(SocketAddress));
    public static void Register( Assembly assembly ) => Register(assembly.GetTypes());

    public static void Register<T>() => Register(typeof(T));

    public static void Register( params Type[] types )
    {
        foreach ( Type type in types ) { Register(type); }
    }

    public static void Register( Type type, string? nodeName = default )
    {
        if ( nameToType.Values.Contains(type) ) { return; }

        nameToType[type.GetTypeName(true)] = type;

        if ( type.HasInterface<IConvertible>() ) { parsers[type] = value => value.ConvertTo(type); }

        NodeNames.RegisterNodeName(type, nodeName);
    }


    public static T FromXml<T>( this    string xml, out IDictionary<string, string>? attributes ) => Deserialize<T>(xml, out attributes);
    public static string ToXml<T>( this T      obj, in  IDictionary<string, string>? attributes = default ) => Serialize(obj, attributes);
}
