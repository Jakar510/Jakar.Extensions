// Jakar.Extensions :: Jakar.Xml
// 04/28/2022  1:14 PM

using System;
using System.Collections.Generic;
using Jakar.Xml.Deserialization;



namespace Jakar.Xml;


public static partial class Xmlizer
{
    public static T Deserialize<T>( string xml, out IDictionary<string, string>? attributes ) where T : new()
    {
        // Activator.CreateInstance<T>() ?? throw new NullReferenceException(nameof(Activator.CreateInstance));

        var document = new XDocument( xml );
        var result   = new T();

        object obj  = result;
        Type   type = typeof(T);


        attributes = null;
        return result;
    }


    // private XmlNode WrapNode( in XmlNode parent, in string name )
    // {
    //     XmlNode child = _deserializer.CreateNode(XmlNodeType.Element, name, null);
    //     parent.AppendChild(child);
    //     return child;
    // }
    // private void AddNode( XmlNode parent, in DictionaryEntry pair )
    // {
    //     XmlNode child = WrapNode(parent, Constants.KEY_VALUE_PAIR);
    //
    //     AddNode(WrapNode(child, Constants.KEY), pair.Key, pair.Key.GetType(), null);
    //
    //     AddNode(WrapNode(child, Constants.VALUE), pair.Value, pair.Value?.GetType(), null);
    // }
    // private void AddNode( in XmlNode? parent, object? value, in Type? type, in string? nameSpace )
    // {
    //     XmlNode child = _deserializer.CreateElement(type?.GetNodeName() ?? Constants.NULL);
    //
    //     UpdateAttributes(child, type);
    //     if ( nameSpace is not null ) { AddAttribute(child, Constants.XMLS_TAG, nameSpace); }
    //
    //     parent?.AppendChild(child);
    //
    //     if ( parent is null )
    //     {
    //         SetAttributes(child, _attributes);
    //         _deserializer.AppendChild(child);
    //     }
    //
    //     if ( value is null ) { child.InnerText = Constants.NULL; }
    //
    //     else if ( value.GetType().IsAnyBuiltInType() ) { child.InnerText = value.ToString(); }
    //
    //     else if ( value.GetType().IsDictionaryEntry() )
    //     {
    //         if ( value is not DictionaryEntry pair ) { throw new ExpectedValueTypeException(nameof(value), value, typeof(DictionaryEntry)); }
    //
    //         AddNode(child, pair);
    //     }
    //
    //     else if ( type.IsArray )
    //     {
    //         if ( value is not IList array ) { throw new ExpectedValueTypeException(nameof(value), value, typeof(Array)); }
    //
    //         foreach ( var item in array ) { AddNode(child, item, item.GetType(), null); }
    //     }
    //
    //     else if ( type.IsDictionary() )
    //     {
    //         if ( value is not IDictionary dict ) { throw new ExpectedValueTypeException(nameof(value), value, typeof(IDictionary)); }
    //
    //         foreach ( DictionaryEntry pair in dict ) { AddNode(child, pair); }
    //     }
    //
    //     else if ( type.IsCollection() )
    //     {
    //         if ( value is not IEnumerable list ) { throw new ExpectedValueTypeException(nameof(value), value, typeof(ICollection)); }
    //
    //         foreach ( var item in list ) { AddNode(child, item, item.GetType(), null); }
    //     }
    //
    //     else
    //     {
    //         IReadOnlyList<PropertyInfo> pi = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty);
    //
    //         if ( pi.Count > 0 )
    //         {
    //             XmlNode properties = _deserializer.CreateElement(Constants.PROPERTIES);
    //             AddAttribute(properties, Constants.TYPE, type.GetTypeName(true));
    //             child.AppendChild(properties);
    //
    //             foreach ( PropertyInfo propertyInfo in pi )
    //             {
    //                 object? obj = propertyInfo.GetValue(value);
    //
    //                 AddNode(properties, obj, propertyInfo.PropertyType, type.GetNameSpaceUri(propertyInfo));
    //             }
    //         }
    //
    //
    //         IReadOnlyList<FieldInfo> fi = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField);
    //
    //         if ( fi.Count > 0 )
    //         {
    //             XmlNode fields = _deserializer.CreateElement(Constants.FIELDS);
    //             AddAttribute(fields, Constants.TYPE, type.GetTypeName(true));
    //             child.AppendChild(fields);
    //
    //             foreach ( FieldInfo fieldInfo in fi )
    //             {
    //                 object? obj = fieldInfo.GetValue(value);
    //
    //                 AddNode(fields, obj, fieldInfo.FieldType, type.GetNameSpaceUri(fieldInfo));
    //             }
    //         }
    //     }
    // }
}
