// Jakar.Extensions :: Jakar.Xml
// 09/25/2025  21:43

using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Xml;
using Jakar.Extensions;
using ValueOf;



namespace Jakar.Xml;


public static class JsonXmlConverter
{
    public static XmlDocument JsonObjectToXml( this IEnumerable<JsonObject> values, string? objectName = null, [CallerArgumentExpression(nameof(values))] string rootName = BaseClass.EMPTY )
    {
        XmlDocument doc  = new();
        XmlElement  root = doc.CreateElement(rootName);
        doc.AppendChild(root);

        foreach ( JsonObject json in values ) { doc.AppendJsonNode(root, json, objectName); }

        return doc;
    }
    public static XmlDocument JsonObjectToXml( this JsonObject value, [CallerArgumentExpression(nameof(value))] string rootName = BaseClass.EMPTY )
    {
        XmlDocument doc = new();
        doc.JsonObjectToXml(value, rootName);
        return doc;
    }
    public static XmlDocument JsonObjectToXml( this XmlDocument doc, JsonObject value, [CallerArgumentExpression(nameof(value))] string rootName = BaseClass.EMPTY )
    {
        XmlElement root = doc.CreateElement(rootName);
        doc.AppendChild(root);
        doc.AppendJsonNode(root, value);
        return doc;
    }
    public static void AppendJsonNode( this XmlDocument doc, XmlElement parent, JsonNode? node, string? name = null )
    {
        switch ( node )
        {
            case JsonObject obj:
            {
                XmlElement element = name != null
                                         ? doc.CreateElement(name)
                                         : parent;

                if ( name != null ) { parent.AppendChild(element); }

                foreach ( KeyValuePair<string, JsonNode?> kvp in obj ) { AppendJsonNode(doc, element, kvp.Value, kvp.Key); }

                break;
            }

            case JsonArray array:
            {
                foreach ( JsonNode? item in array ) { AppendJsonNode(doc, parent, item, name ?? Constants.ITEM); }

                break;
            }

            case JsonValue value:
            {
                XmlElement element = doc.CreateElement(name ?? Constants.VALUE);
                element.InnerText = value.ToJsonString().Trim('"'); // strips quotes for strings
                parent.AppendChild(element);
                break;
            }

            case null:
            {
                XmlElement element = doc.CreateElement(name ?? Constants.NULL);
                element.IsEmpty = true;
                parent.AppendChild(element);
                break;
            }
        }
    }


    public static JsonNode    ToJson( this XmlDocument           value, [CallerArgumentExpression(nameof(value))] string rootName = BaseClass.EMPTY ) { return null; }
    public static XmlDocument ToXml( this  JsonNode              value, [CallerArgumentExpression(nameof(value))] string rootName = BaseClass.EMPTY ) { return null; }
    public static XmlDocument ToXml( this  IEnumerable<JsonNode> values, [CallerArgumentExpression(nameof(values))] string rootName = BaseClass.EMPTY ) { return null; }
}
