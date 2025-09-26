using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml;
using Jakar.Extensions;



namespace Jakar.Xml;


[SuppressMessage("ReSharper", "ParameterTypeCanBeEnumerable.Global")]
public static class XmlExtensions
{
    public static ICollection<long> GetMappedIDs( this string xml, out IDictionary<string, string>? attributes ) => xml.FromXml<List<long>>(out attributes);

    public static ICollection<TValue> ToList<TValue>( this XmlDocument document, out IDictionary<string, string>? attributes )
        where TValue : IConvertible
    {
        List<TValue> results = new();

        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != Constants.ITEM ) { throw new SerializationException(nameof(root.Name)); }

        // attributes = root.GetAttributes();
        attributes = null;

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.InnerText is null ) { continue; }

            results.Add(node.InnerText.ConvertTo<TValue>());
        }

        return results;
    }


    // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------


    // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------


    public static IDictionary ToDictionary( this XmlNode root, out IDictionary<string, string>? attributes )
    {
        if ( root.Name != Constants.DICTIONARY ) { throw new FormatException(nameof(root.Name)); }

        // attributes = root.GetAttributes();
        attributes = null;
        if ( attributes is null ) { throw new NullReferenceException(nameof(attributes)); }

        Type keyType   = Xmlizer.NameToType[attributes[Constants.KEY]];
        Type valueType = Xmlizer.NameToType[attributes[Constants.VALUE]];

        Type        target  = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        IDictionary results = (IDictionary?)Activator.CreateInstance(target) ?? throw new InvalidOperationException($"Cannot Create instance of '{target.Name}'");

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.Name is null ) { continue; }

            if ( node.Name != Constants.KEY_VALUE_PAIR ) { throw new SerializationException(nameof(node.Name)); }

            string  key   = string.Empty;
            object? value = null;

            for ( int c = 0; c < node.ChildNodes.Count; c++ )
            {
                XmlNode? child = node.ChildNodes[c];
                if ( child?.Name is null ) { throw new NullReferenceException(nameof(child.InnerText)); }

                switch ( child.Name )
                {
                    case Constants.KEY:
                        key = child.InnerText;
                        break;

                    case Constants.VALUE:
                        value = child.InnerText.ConvertTo(valueType);
                        break;
                }
            }

            if ( string.IsNullOrWhiteSpace(key) ) { throw new NullReferenceException(nameof(key)); }

            results[key] = value;
        }

        return results;
    }


    // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------


    // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------


    public static string GetNameSpaceUri( this Type type, PropertyInfo info )      => type.GetNameSpaceUri(info.Name);
    public static string GetNameSpaceUri( this Type type, FieldInfo    info )      => type.GetNameSpaceUri(info.Name);
    public static string GetNameSpaceUri( this Type type, string       nameSpace ) => type.GetTypeName() + Constants.Dividers.NS + nameSpace;


    public static string PrettyXml( this XmlDocument document, XmlWriterSettings? settings = null )
    {
        settings ??= new XmlWriterSettings
                     {
                         Encoding            = Encoding.Unicode,
                         Indent              = true,
                         NewLineOnAttributes = true,
                         OmitXmlDeclaration  = true,
                         IndentChars         = new string(' ', 4)
                     };

        StringBuilder builder = new();
        XmlWriter     writer  = XmlWriter.Create(builder, settings);
        document.Save(writer);
        return builder.ToString();
    }
    public static string SetMappedIDs<TValue>( this IEnumerable<IEnumerable<TValue>> items )
        where TValue : IUniqueID<long> => items.Consolidate().SetMappedIDs();
    public static string SetMappedIDs<TValue>( this IEnumerable<TValue> items )
        where TValue : IUniqueID<long> => items.Select(item => item.ID).SetMappedIDs<TValue>();

    public static string SetMappedIDs<TValue>( this IEnumerable<long> listOfIds ) => listOfIds.ToXml(new Dictionary<string, string> { [Constants.GROUP] = typeof(TValue).GetTableName() });


    public static string ToXml( this JsonObject value )
    {
        XmlDocument? node = JsonXmlConverter.ToXml(value);

        string? result = node?.PrettyXml();
        return result ?? throw new InvalidOperationException();
    }

    public static string ToXml( this IEnumerable<JsonObject> item )
    {
        XmlDocument? node = JsonXmlConverter.ToXml(item);

        string? result = node?.PrettyXml();
        return result ?? throw new InvalidOperationException();
    }
    public static TResult? DeserializeXml<TResult>( this string xml )
        where TResult : IJsonModel<TResult>
    {
        if ( string.IsNullOrWhiteSpace(xml) ) { return default; }

        XmlDocument doc = new();
        doc.LoadXml(xml);

        JsonNode json = doc.ToJson();
        return json.ToObject(TResult.JsonTypeInfo);
    }


    public static XmlDocument ToRawXml( this string xml )
    {
        XmlDocument document = new();
        document.LoadXml(xml);

        return document;
    }
}
