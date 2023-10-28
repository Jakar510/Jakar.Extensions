// Jakar.Extensions :: Jakar.Xml
// 04/28/2022  1:13 PM

using System;
using System.Collections.Generic;
using Jakar.Xml.Serialization;



namespace Jakar.Xml;


public static partial class Xmlizer
{
    public static string Serialize<T>( T obj, in IDictionary<string, string>? attributes = default )
    {
        if ( obj is null ) { throw new NullReferenceException( nameof(obj) ); }

        Type type   = typeof(T);
        var  writer = new XWriter();

        return writer.ToString();
    }


    // private void Update( ref object obj, in XNode node )
    // {
    //     if ( node.Name.SequenceEqual(Constants.GROUP) ) { PopulateList(ref obj, node); }
    //     else if ( node.Name.SequenceEqual(Constants.DICTIONARY) ) { PopulateDictionary(ref obj, node); }
    //     else if ( node.Name.SequenceEqual(Constants.GROUP) ) { PopulateArray(ref obj, node); }
    //     else
    //     {
    //         if ( _objectType.IsClass ) { }
    //         else if ( _objectType.IsValueType )
    //         {
    //             if ( _objectType.IsNullableType() ) { }
    //         }
    //     }
    // }
    // private void PopulateArray( ref object obj, XmlNode parent )
    // {
    //     if ( obj is not IList array ) { throw new ExpectedValueTypeException(nameof(obj), obj, typeof(IList)); }
    //
    //     Type TValue = typeof(bool); // TODO
    //
    //     for ( var i = 0; i < parent.ChildNodes.Count; i++ )
    //     {
    //         XmlNode? node = parent.ChildNodes[i];
    //         if ( node?.InnerText is null ) { continue; }
    //
    //         array.Add(node.InnerText.ConvertTo(TValue));
    //     }
    // }
    // private void PopulateList( ref object obj, in XNode parent )
    // {
    //     if ( obj is not IList list ) { throw new ExpectedValueTypeException(nameof(obj), obj, typeof(IList)); }
    //
    //     Type TValue = typeof(bool); // TODO
    //
    //     for ( var i = 0; i < parent.ChildNodes.Count; i++ )
    //     {
    //         XmlNode? node = parent.ChildNodes[i];
    //         if ( node?.InnerText is null ) { continue; }
    //
    //         list.Add(node.InnerText.ConvertTo(TValue));
    //     }
    // }
    // private void PopulateDictionary( ref object obj, in XNode parent )
    // {
    //     if ( obj is not IDictionary dict ) { throw new ExpectedValueTypeException(nameof(obj), obj, typeof(IDictionary)); }
    //
    //     if ( _attributes is null ) { throw new NullReferenceException(nameof(_attributes)); }
    //
    //     Type keyType   = Xmlizer.nameToType[_attributes[Constants.KEY]];
    //     Type valueType = Xmlizer.nameToType[_attributes[Constants.VALUE]];
    //
    //
    //     for ( var i = 0; i < parent.ChildNodes.Count; i++ )
    //     {
    //         XmlNode? node = parent.ChildNodes[i];
    //         if ( node?.Name is null ) { continue; }
    //
    //         if ( node.Name != Constants.KEY_VALUE_PAIR ) { throw new FormatException(nameof(node.Name)); }
    //
    //
    //         object? key   = default;
    //         object? value = default;
    //
    //         for ( var c = 0; c < node.ChildNodes.Count; c++ )
    //         {
    //             XmlNode? child = node.ChildNodes[c];
    //             if ( child?.Name is null ) { throw new NullReferenceException(nameof(child.InnerText)); }
    //
    //             switch ( child.Name )
    //             {
    //                 case Constants.KEY:
    //                     if ( keyType.IsGenericType && keyType.HasInterface<IConvertible>() ) { key = child.InnerText.ConvertTo(keyType); }
    //
    //                     else { key = child.InnerText; }
    //
    //                     break;
    //
    //                 case Constants.VALUE:
    //                     if ( valueType.IsGenericType && valueType.HasInterface<IConvertible>() ) { value = child.InnerText.ConvertTo(valueType); }
    //
    //                     break;
    //             }
    //         }
    //
    //         // ReSharper disable once ConvertSwitchStatementToSwitchExpression
    //         switch ( key )
    //         {
    //             case null: { throw new FormatException($"Key is not found at {parent.Name}"); }
    //
    //             case string s when string.IsNullOrWhiteSpace(s): { throw new NullReferenceException(nameof(key)); }
    //
    //             default:
    //                 dict[key] = value;
    //                 break;
    //         }
    //     }
    // }
}
