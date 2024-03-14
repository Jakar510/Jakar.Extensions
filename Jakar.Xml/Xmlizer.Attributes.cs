// Jakar.Extensions :: Jakar.Xml
// 04/28/2022  1:16 PM

namespace Jakar.Xml;


public static partial class Xmlizer
{
    // private void SetAttributes( in XmlNode node, in IDictionary<string, string>? attributes, in string? nameSpace = default )
    // {
    //     if ( attributes is null ) { return; }
    //
    //     foreach ( ( string key, string value ) in attributes ) { AddAttribute(node, key, value, nameSpace); }
    // }
    // private void AddAttribute( in XmlNode node, in string key, in string value, in string? nameSpace = default )
    // {
    //     if ( node.Attributes is null ) { throw new NullReferenceException(nameof(node.Attributes)); }
    //
    //     XmlAttribute attributeItem = _deserializer.CreateAttribute(key, nameSpace);
    //     attributeItem.InnerText = value;
    //     node.Attributes.Append(attributeItem);
    // }
    // private void UpdateAttributes( in XmlNode node, in Type? type )
    // {
    //     if ( node.Attributes is null ) { throw new NullReferenceException(nameof(node.Attributes)); }
    //
    //     if ( type is null )
    //     {
    //         return;
    //
    //         // throw new ArgumentNullException(nameof(type));
    //     }
    //
    //
    //     if ( type.IsAnyBuiltInType() ) { return; }
    //
    //     if ( type.IsGenericType )
    //     {
    //         if ( type.IsKeyValuePair(out Type? keyPairType, out Type? valuePairType) )
    //         {
    //             AddAttribute(node, Constants.KEY,   keyPairType.GetTypeName(true));
    //             AddAttribute(node, Constants.VALUE, valuePairType.GetTypeName(true));
    //         }
    //
    //         if ( type.IsDictionary(out Type? keyType, out Type? valueType) )
    //         {
    //             AddAttribute(node, Constants.KEY,   keyType.GetTypeName(true));
    //             AddAttribute(node, Constants.VALUE, valueType.GetTypeName(true));
    //         }
    //
    //         else if ( type.IsCollection(out Type? itemType) )
    //         {
    //             AddAttribute(node, Constants.TYPE, type.GetTypeName(true));
    //             AddAttribute(node, Constants.ITEM, itemType.GetTypeName(true));
    //         }
    //     }
    //     else { AddAttribute(node, Constants.TYPE, type.GetTypeName(true)); }
    // }
}
