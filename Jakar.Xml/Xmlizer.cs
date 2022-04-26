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



namespace Jakar.Xml;


public static class Xmlizer
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

    public static T FromXml<T>( this    string xml, out IDictionary<string, string>? attributes ) => Xmlizer<T>.Deserialize(xml, out attributes);
    public static string ToXml<T>( this T      obj, in  IDictionary<string, string>? attributes = default ) => Xmlizer<T>.Serialize(obj, attributes);
}



/// <summary>
/// A PREDICTABLE (de)serializer for any given object.
/// <para>
///	Primary goals are to be easily human readable, predictable and consistent.
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class Xmlizer<T>
{
    public static T Deserialize( string xml, out IDictionary<string, string>? attributes ) => new Xmlizer<T>().FromXml(xml, out attributes);

    public static string Serialize( T obj, in IDictionary<string, string>? attributes = default ) => new Xmlizer<T>(attributes).ToXml(obj);


    private readonly Type                         _objectType;
    private readonly XmlDocument                  _document;
    private readonly IDictionary<string, string>? _attributes;


    internal Xmlizer( in IDictionary<string, string>? attributes = default )
    {
        _objectType = typeof(T);
        _attributes = attributes;
        _document   = new XmlDocument();
    }


    private void SetAttributes( in XmlNode node, in IDictionary<string, string>? attributes, in string? nameSpace = default )
    {
        if ( attributes is null ) { return; }

        foreach ( ( string key, string value ) in attributes ) { AddAttribute(node, key, value, nameSpace); }
    }
    private void AddAttribute( in XmlNode node, in string key, in string value, in string? nameSpace = default )
    {
        if ( node.Attributes is null ) { throw new NullReferenceException(nameof(node.Attributes)); }

        XmlAttribute attributeItem = _document.CreateAttribute(key, nameSpace);
        attributeItem.InnerText = value;
        node.Attributes.Append(attributeItem);
    }
    private void UpdateAttributes( in XmlNode node, in Type type )
    {
        if ( node.Attributes is null ) { throw new NullReferenceException(nameof(node.Attributes)); }

        if ( type.IsAnyBuiltInType() ) { return; }

        if ( type.IsGenericType )
        {
            if ( type.IsKeyValuePair(out Type? keyPairType, out Type? valuePairType) )
            {
                AddAttribute(node, Constants.KEY,   keyPairType.GetTypeName(true));
                AddAttribute(node, Constants.VALUE, valuePairType.GetTypeName(true));
            }

            if ( type.IsDictionary(out Type? keyType, out Type? valueType) )
            {
                AddAttribute(node, Constants.KEY,   keyType.GetTypeName(true));
                AddAttribute(node, Constants.VALUE, valueType.GetTypeName(true));
            }

            else if ( type.IsCollection(out Type? itemType) )
            {
                AddAttribute(node, Constants.TYPE, type.GetTypeName(true));
                AddAttribute(node, Constants.ITEM, itemType.GetTypeName(true));
            }
        }
        else { AddAttribute(node, Constants.TYPE, type.GetTypeName(true)); }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="FormatException"></exception>
    /// <returns></returns>
    private string ToXml( T obj )
    {
        if ( obj is null ) { throw new NullReferenceException(nameof(obj)); }

        Type type = obj.GetType();

        AddNode(null, obj, type, type.GetTypeName(true));

        return _document.PrettyXml();
    }
    private XmlNode WrapNode( in XmlNode parent, in string name )
    {
        XmlNode child = _document.CreateNode(XmlNodeType.Element, name, null);
        parent.AppendChild(child);
        return child;
    }
    private void AddNode( XmlNode parent, in DictionaryEntry pair )
    {
        XmlNode child = WrapNode(parent, Constants.KEY_VALUE_PAIR);

        AddNode(WrapNode(child, Constants.KEY), pair.Key, pair.Key.GetType(), null);

        AddNode(WrapNode(child, Constants.VALUE), pair.Value, pair.Value?.GetType(), null);
    }
    private void AddNode( in XmlNode? parent, object? value, in Type? type, in string? nameSpace )
    {
        XmlNode child = _document.CreateElement(type?.GetNodeName() ?? Constants.NULL);

        UpdateAttributes(child, type);
        if ( nameSpace is not null ) { AddAttribute(child, Constants.XMLS, nameSpace); }

        parent?.AppendChild(child);

        if ( parent is null )
        {
            SetAttributes(child, _attributes);
            _document.AppendChild(child);
        }

        if ( value is null ) { child.InnerText = Constants.NULL; }

        else if ( value.GetType().IsAnyBuiltInType() ) { child.InnerText = value.ToString(); }

        else if ( value.GetType().IsDictionaryEntry() )
        {
            if ( value is not DictionaryEntry pair ) { throw new ExpectedValueTypeException(nameof(value), value, typeof(DictionaryEntry)); }

            AddNode(child, pair);
        }

        else if ( type.IsArray )
        {
            if ( value is not IList array ) { throw new ExpectedValueTypeException(nameof(value), value, typeof(Array)); }

            foreach ( var item in array ) { AddNode(child, item, item.GetType(), null); }
        }

        else if ( type.IsDictionary() )
        {
            if ( value is not IDictionary dict ) { throw new ExpectedValueTypeException(nameof(value), value, typeof(IDictionary)); }

            foreach ( DictionaryEntry pair in dict ) { AddNode(child, pair); }
        }

        else if ( type.IsCollection() )
        {
            if ( value is not IEnumerable list ) { throw new ExpectedValueTypeException(nameof(value), value, typeof(ICollection)); }

            foreach ( var item in list ) { AddNode(child, item, item.GetType(), null); }
        }

        else
        {
            IReadOnlyList<PropertyInfo> pi = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty);

            if ( pi.Count > 0 )
            {
                XmlNode properties = _document.CreateElement(Constants.PROPERTIES);
                AddAttribute(properties, Constants.TYPE, type.GetTypeName(true));
                child.AppendChild(properties);

                foreach ( PropertyInfo propertyInfo in pi )
                {
                    object? obj = propertyInfo.GetValue(value);

                    AddNode(properties, obj, propertyInfo.PropertyType, type.GetNameSpaceUri(propertyInfo));
                }
            }


            IReadOnlyList<FieldInfo> fi = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField);

            if ( fi.Count > 0 )
            {
                XmlNode fields = _document.CreateElement(Constants.FIELDS);
                AddAttribute(fields, Constants.TYPE, type.GetTypeName(true));
                child.AppendChild(fields);

                foreach ( FieldInfo fieldInfo in fi )
                {
                    object? obj = fieldInfo.GetValue(value);

                    AddNode(fields, obj, fieldInfo.FieldType, type.GetNameSpaceUri(fieldInfo));
                }
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="xml"></param>
    /// <param name="attributes"></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="FormatException"></exception>
    /// <returns></returns>
    private T FromXml( string xml, out IDictionary<string, string>? attributes )
    {
        _document.LoadXml(xml);
        T result = Activator.CreateInstance<T>() ?? throw new NullReferenceException(nameof(Activator.CreateInstance));

        object obj = result;
        Update(ref obj, _document.ChildNodes[0]);

        attributes = null;
        return result;
    }
    private void Update( ref object obj, in XNode node )
    {
        if ( node.Name.SequenceEqual(Constants.GROUP) ) { PopulateList(ref obj, node); }
        else if ( node.Name.SequenceEqual(Constants.DICTIONARY) ) { PopulateDictionary(ref obj, node); }
        else if ( node.Name.SequenceEqual(Constants.GROUP) ) { PopulateArray(ref obj, node); }
        else
        {
            if ( _objectType.IsClass ) { }
            else if ( _objectType.IsValueType )
            {
                if ( _objectType.IsNullableType() ) { }
            }
        }
    }
    private void PopulateArray( ref object obj, XmlNode parent )
    {
        if ( obj is not IList array ) { throw new ExpectedValueTypeException(nameof(obj), obj, typeof(IList)); }

        Type TValue = typeof(bool); // TODO

        for ( var i = 0; i < parent.ChildNodes.Count; i++ )
        {
            XmlNode? node = parent.ChildNodes[i];
            if ( node?.InnerText is null ) { continue; }

            array.Add(node.InnerText.ConvertTo(TValue));
        }
    }
    private void PopulateList( ref object obj, in XNode parent )
    {
        if ( obj is not IList list ) { throw new ExpectedValueTypeException(nameof(obj), obj, typeof(IList)); }

        Type TValue = typeof(bool); // TODO

        for ( var i = 0; i < parent.ChildNodes.Count; i++ )
        {
            XmlNode? node = parent.ChildNodes[i];
            if ( node?.InnerText is null ) { continue; }

            list.Add(node.InnerText.ConvertTo(TValue));
        }
    }
    private void PopulateDictionary( ref object obj, in XNode parent )
    {
        if ( obj is not IDictionary dict ) { throw new ExpectedValueTypeException(nameof(obj), obj, typeof(IDictionary)); }

        if ( _attributes is null ) { throw new NullReferenceException(nameof(_attributes)); }

        Type keyType   = Xmlizer.nameToType[_attributes[Constants.KEY]];
        Type valueType = Xmlizer.nameToType[_attributes[Constants.VALUE]];


        for ( var i = 0; i < parent.ChildNodes.Count; i++ )
        {
            XmlNode? node = parent.ChildNodes[i];
            if ( node?.Name is null ) { continue; }

            if ( node.Name != Constants.KEY_VALUE_PAIR ) { throw new FormatException(nameof(node.Name)); }


            object? key   = default;
            object? value = default;

            for ( var c = 0; c < node.ChildNodes.Count; c++ )
            {
                XmlNode? child = node.ChildNodes[c];
                if ( child?.Name is null ) { throw new NullReferenceException(nameof(child.InnerText)); }

                switch ( child.Name )
                {
                    case Constants.KEY:
                        if ( keyType.IsGenericType && keyType.HasInterface<IConvertible>() ) { key = child.InnerText.ConvertTo(keyType); }

                        else { key = child.InnerText; }

                        break;

                    case Constants.VALUE:
                        if ( valueType.IsGenericType && valueType.HasInterface<IConvertible>() ) { value = child.InnerText.ConvertTo(valueType); }

                        break;
                }
            }

            // ReSharper disable once ConvertSwitchStatementToSwitchExpression
            switch ( key )
            {
                case null: { throw new FormatException($"Key is not found at {parent.Name}"); }

                case string s when string.IsNullOrWhiteSpace(s): { throw new NullReferenceException(nameof(key)); }

                default:
                    dict[key] = value;
                    break;
            }
        }
    }
}
