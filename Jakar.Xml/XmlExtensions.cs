#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Jakar.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace Jakar.Xml;


[SuppressMessage( "ReSharper", "ParameterTypeCanBeEnumerable.Global" )]
public static class XmlExtensions
{
    public static ICollection<long> GetMappedIDs( this string xml, out IDictionary<string, string>? attributes ) => xml.FromXml<List<long>>( out attributes );

    public static ICollection<TValue> ToList<TValue>( this XmlDocument document, out IDictionary<string, string>? attributes ) where TValue : IConvertible
    {
        var results = new List<TValue>();

        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != Constants.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        // attributes = root.GetAttributes();
        attributes = null;

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.InnerText is null ) { continue; }

            results.Add( node.InnerText.ConvertTo<TValue>() );
        }

        return results;
    }


    // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------


    // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------


    public static IDictionary ToDictionary( this XmlNode root, out IDictionary<string, string>? attributes )
    {
        if ( root.Name != Constants.DICTIONARY ) { throw new FormatException( nameof(root.Name) ); }

        // attributes = root.GetAttributes();
        attributes = null;
        if ( attributes is null ) { throw new NullReferenceException( nameof(attributes) ); }

        Type keyType   = Xmlizer.nameToType[attributes[Constants.KEY]];
        Type valueType = Xmlizer.nameToType[attributes[Constants.VALUE]];

        Type target  = typeof(Dictionary<,>).MakeGenericType( keyType, valueType );
        var  results = (IDictionary)Activator.CreateInstance( target );

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.Name is null ) { continue; }

            if ( node.Name != Constants.KEY_VALUE_PAIR ) { throw new SerializationException( nameof(node.Name) ); }

            string  key   = string.Empty;
            object? value = default;

            for ( int c = 0; c < node.ChildNodes.Count; c++ )
            {
                XmlNode? child = node.ChildNodes[c];
                if ( child?.Name is null ) { throw new NullReferenceException( nameof(child.InnerText) ); }

                switch ( child.Name )
                {
                    case Constants.KEY:
                        key = child.InnerText;
                        break;

                    case Constants.VALUE:
                        value = child.InnerText.ConvertTo( valueType );
                        break;
                }
            }

            if ( string.IsNullOrWhiteSpace( key ) ) { throw new NullReferenceException( nameof(key) ); }

            results[key] = value;
        }

        return results;
    }


    // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------


    // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------


    public static string GetNameSpaceUri( this Type type, PropertyInfo info ) => type.GetNameSpaceUri( info.Name );
    public static string GetNameSpaceUri( this Type type, FieldInfo    info ) => type.GetNameSpaceUri( info.Name );
    public static string GetNameSpaceUri( this Type type, string       nameSpace ) => type.GetTypeName() + Constants.Dividers.NS + nameSpace;


    public static string PrettyXml( this XmlDocument document, XmlWriterSettings? settings = default )
    {
        settings ??= new XmlWriterSettings
                     {
                         Encoding            = Encoding.Unicode,
                         Indent              = true,
                         NewLineOnAttributes = true,
                         OmitXmlDeclaration  = true,
                         IndentChars         = new string( ' ', 4 ),
                     };

        var builder = new StringBuilder();
        var writer  = XmlWriter.Create( builder, settings );
        document.Save( writer );
        return builder.ToString();
    }
    public static string SetMappedIDs<T>( this IEnumerable<IEnumerable<T>> items ) where T : IDataBaseID => items.Consolidate()
                                                                                                                 .SetMappedIDs();
    public static string SetMappedIDs<T>( this IEnumerable<T> items ) where T : IDataBaseID => items.Select( item => item.ID )
                                                                                                    .SetMappedIDs<T>();

    public static string SetMappedIDs<T>( this IEnumerable<long> listOfIds ) => listOfIds.ToXml( new Dictionary<string, string>
                                                                                                 {
                                                                                                     [Constants.GROUP] = typeof(T).GetTableName(),
                                                                                                 } );


    public static string ToXml( this JObject item )
    {
        XmlDocument? node = JsonConvert.DeserializeXmlNode( item.ToJson(), nameof(item), true, true );

        string? result = node?.PrettyXml();
        return result ?? throw new InvalidOperationException();
    }

    public static string ToXml( this IEnumerable<JObject> item )
    {
        XmlDocument? node = JsonConvert.DeserializeXmlNode( item.ToJson(), nameof(item), true, true );

        string? result = node?.PrettyXml();
        return result ?? throw new InvalidOperationException();
    }
    public static TResult? DeserializeXml<TResult>( this string xml )
    {
        if ( string.IsNullOrWhiteSpace( xml ) ) { return default; }

        var doc = new XmlDocument();
        doc.LoadXml( xml );

        string json = JsonConvert.SerializeXmlNode( doc );
        return json.FromJson<TResult>();
    }

    public static XmlDocument ToRawXml( this string xml )
    {
        var document = new XmlDocument();
        document.LoadXml( xml );

        return document;
    }
}
