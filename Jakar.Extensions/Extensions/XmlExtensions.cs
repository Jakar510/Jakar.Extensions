using System.Xml;



namespace Jakar.Extensions;


[ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
public static class XmlNames
{
    public const string DICTIONARY        = "Dictionary";
    public const string ELEMENT           = "element";
    public const string ITEM              = "item";
    public const string KEY               = "key";
    public const string LIST              = "List";
    public const string PAIR              = "pair";
    public const string TARGET_TABLE_NAME = "TargetTableName";
    public const string VALUE             = "value";
}



[ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ), SuppressMessage( "ReSharper", "ParameterTypeCanBeEnumerable.Global" ) ]
public static class XmlExtensions
{
    private static string PrettyXml( this XmlDocument document )
    {
        var settings = new XmlWriterSettings
                       {
                           Encoding            = Encoding.Unicode,
                           Indent              = true,
                           NewLineOnAttributes = true,
                           OmitXmlDeclaration  = true,
                           IndentChars         = new string( ' ', 4 )
                       };

        var builder = new StringBuilder();
        var writer  = XmlWriter.Create( builder, settings );
        document.Save( writer );
        return builder.ToString();
    }


    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static string ToXml( this JObject item )
    {
        XmlDocument? node = JsonConvert.DeserializeXmlNode( item.ToJson(), nameof(item), true, true );

        string? result = node?.PrettyXml();
        return result ?? throw new InvalidOperationException();
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
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



    #region Mapping

    public static string SetMappedIDs<T>( this IEnumerable<IEnumerable<T>> items ) where T : IDataBaseID => items.Consolidate().SetMappedIDs();
    public static string SetMappedIDs<T>( this IEnumerable<T>              items ) where T : IDataBaseID => items.Select( item => item.ID ).SetMappedIDs<T>();

    public static string SetMappedIDs<T>( this IEnumerable<long> listOfIds ) =>
        listOfIds.ToList()
                 .ToXml( new Dictionary<string, string>
                         {
                             [XmlNames.TARGET_TABLE_NAME] = typeof(T).GetTableName()
                         } );

    public static List<long> GetMappedIDs( this string xml, out IDictionary<string, string>? attributes ) => xml.ToRawXml().ToLongList( out attributes );

    #endregion



    #region serialize

    #region lists

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this List<Guid?> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this List<Guid> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this List<bool> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this List<string> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this List<double> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this List<double?> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this List<long> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this List<long?> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this List<int> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this List<int?> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    private static string Serialize<TValue>( this IEnumerable<TValue> item, IDictionary<string, string>? attributes )
    {
        var          document  = new XmlDocument();
        XmlNode      node      = document.CreateNode( XmlNodeType.Element, XmlNames.ITEM, null );
        XmlAttribute attribute = document.CreateAttribute( nameof(Type), null );
        attribute.InnerText = XmlNames.LIST;
        node.Attributes?.Append( attribute );

        if ( attributes is not null )
        {
            foreach ( (string key, string value) in attributes )
            {
                XmlAttribute attributeItem = document.CreateAttribute( key, null );
                attributeItem.InnerText = value;
                node.Attributes?.Append( attributeItem );
            }
        }


        foreach ( TValue element in item )
        {
            XmlNode child = document.CreateNode( XmlNodeType.Element, XmlNames.ELEMENT, null );

            XmlNode? id = node.AppendChild( child );
            if ( id is null ) { throw new NullReferenceException( nameof(id) ); }

            id.InnerText = element?.ToString() ?? throw new SerializationException( nameof(element) );
        }

        document.AppendChild( node );
        return document.PrettyXml();
    }

    #endregion



    #region dictionary

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this Dictionary<string, Guid> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this Dictionary<string, Guid?> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this Dictionary<string, string?> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this Dictionary<string, double?> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this Dictionary<string, double> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this Dictionary<string, long?> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this Dictionary<string, long> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this Dictionary<string, int?> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this Dictionary<string, int> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this Dictionary<string, bool?> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ] public static string ToXml( this Dictionary<string, bool> item, in IDictionary<string, string>? attributes = default ) => item.Serialize( attributes );

    private static string Serialize<TValue>( this IDictionary<string, TValue> item, IDictionary<string, string>? attributes )
    {
        var          document  = new XmlDocument();
        XmlNode      node      = document.CreateNode( XmlNodeType.Element, XmlNames.ITEM, null );
        XmlAttribute attribute = document.CreateAttribute( nameof(Type), null );
        attribute.InnerText = XmlNames.DICTIONARY;
        node.Attributes?.Append( attribute );

        if ( attributes is not null )
        {
            foreach ( (string key, string value) in attributes )
            {
                XmlAttribute attributeItem = document.CreateAttribute( key, null );
                attributeItem.InnerText = value;
                node.Attributes?.Append( attributeItem );
            }
        }

        foreach ( KeyValuePair<string, TValue> pair in item )
        {
            XmlNode element = document.CreateNode( XmlNodeType.Element, XmlNames.PAIR,  null );
            XmlNode key     = document.CreateNode( XmlNodeType.Element, XmlNames.KEY,   null );
            XmlNode value   = document.CreateNode( XmlNodeType.Element, XmlNames.VALUE, null );

            XmlNode? keyId   = element.AppendChild( key );
            XmlNode? valueId = element.AppendChild( value );
            if ( keyId is null ) { throw new NullReferenceException( nameof(keyId) ); }

            if ( valueId is null ) { throw new NullReferenceException( nameof(valueId) ); }

            keyId.InnerText   = pair.Key               ?? throw new SerializationException( nameof(pair.Key) );
            valueId.InnerText = pair.Value?.ToString() ?? throw new SerializationException( nameof(pair.Value) );

            XmlNode? id = node.AppendChild( element );
            if ( id is null ) { throw new NullReferenceException( nameof(id) ); }
        }

        document.AppendChild( node );
        return document.PrettyXml();
    }

    #endregion

    #endregion



    #region deserialize

    #region dictionary

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static Dictionary<string, string> ToStringDictionary( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        var results = new Dictionary<string, string>();

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.Name is null ) { continue; }

            if ( node.Name != XmlNames.PAIR ) { throw new SerializationException( nameof(node.Name) ); }

            string key   = "";
            string value = "";

            for ( int c = 0; c < node.ChildNodes.Count; c++ )
            {
                XmlNode? child = node.ChildNodes[c];
                if ( child?.Name is null ) { throw new NullReferenceException( nameof(child.InnerText) ); }

                switch ( child.Name )
                {
                    case XmlNames.KEY:
                        key = child.InnerText;
                        break;

                    case XmlNames.VALUE:
                        value = child.InnerText;
                        break;
                }
            }

            if ( string.IsNullOrWhiteSpace( key ) ) { throw new NullReferenceException( nameof(key) ); }

            results[key] = value;
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static Dictionary<string, Guid> ToGuidDictionary( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        var results = new Dictionary<string, Guid>();

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.Name is null ) { continue; }

            if ( node.Name != XmlNames.PAIR ) { throw new SerializationException( nameof(node.Name) ); }

            string key   = "";
            Guid   value = default;

            for ( int c = 0; c < node.ChildNodes.Count; c++ )
            {
                XmlNode? child = node.ChildNodes[c];
                if ( child?.Name is null ) { throw new NullReferenceException( nameof(child.InnerText) ); }

                switch ( child.Name )
                {
                    case XmlNames.KEY:
                        key = child.InnerText;
                        break;

                    case XmlNames.VALUE when Guid.TryParse( child.InnerText, out Guid n ):
                        value = n;
                        break;
                }
            }

            if ( string.IsNullOrWhiteSpace( key ) ) { throw new NullReferenceException( nameof(key) ); }

            results[key] = value;
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static Dictionary<string, Guid?> ToNullableGuidDictionary( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        var results = new Dictionary<string, Guid?>();

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.Name is null ) { continue; }

            if ( node.Name != XmlNames.PAIR ) { throw new SerializationException( nameof(node.Name) ); }

            string key   = "";
            Guid?  value = default;

            for ( int c = 0; c < node.ChildNodes.Count; c++ )
            {
                XmlNode? child = node.ChildNodes[c];
                if ( child?.Name is null ) { throw new NullReferenceException( nameof(child.InnerText) ); }

                switch ( child.Name )
                {
                    case XmlNames.KEY:
                        key = child.InnerText;
                        break;

                    case XmlNames.VALUE when Guid.TryParse( child.InnerText, out Guid n ):
                        value = n;
                        break;
                }
            }

            if ( string.IsNullOrWhiteSpace( key ) ) { throw new NullReferenceException( nameof(key) ); }

            results[key] = value;
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static Dictionary<string, bool> ToBoolDictionary( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        var results = new Dictionary<string, bool>();

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.Name is null ) { continue; }

            if ( node.Name != XmlNames.PAIR ) { throw new SerializationException( nameof(node.Name) ); }

            string key   = "";
            bool   value = default;

            for ( int c = 0; c < node.ChildNodes.Count; c++ )
            {
                XmlNode? child = node.ChildNodes[c];
                if ( child?.Name is null ) { throw new NullReferenceException( nameof(child.InnerText) ); }

                switch ( child.Name )
                {
                    case XmlNames.KEY:
                        key = child.InnerText;
                        break;

                    case XmlNames.VALUE when bool.TryParse( child.InnerText, out bool n ):
                        value = n;
                        break;
                }
            }

            if ( string.IsNullOrWhiteSpace( key ) ) { throw new NullReferenceException( nameof(key) ); }

            results[key] = value;
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static Dictionary<string, bool?> ToNullableBoolDictionary( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        var results = new Dictionary<string, bool?>();

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.Name is null ) { continue; }

            if ( node.Name != XmlNames.PAIR ) { throw new SerializationException( nameof(node.Name) ); }

            string key   = "";
            bool?  value = default;

            for ( int c = 0; c < node.ChildNodes.Count; c++ )
            {
                XmlNode? child = node.ChildNodes[c];
                if ( child?.Name is null ) { throw new NullReferenceException( nameof(child.InnerText) ); }

                switch ( child.Name )
                {
                    case XmlNames.KEY:
                        key = child.InnerText;
                        break;

                    case XmlNames.VALUE when bool.TryParse( child.InnerText, out bool n ):
                        value = n;
                        break;
                }
            }

            if ( string.IsNullOrWhiteSpace( key ) ) { throw new NullReferenceException( nameof(key) ); }

            results[key] = value;
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static Dictionary<string, double> ToDoubleDictionary( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        var results = new Dictionary<string, double>();

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.Name is null ) { continue; }

            if ( node.Name != XmlNames.PAIR ) { throw new SerializationException( nameof(node.Name) ); }

            string key   = "";
            double value = default;

            for ( int c = 0; c < node.ChildNodes.Count; c++ )
            {
                XmlNode? child = node.ChildNodes[c];
                if ( child?.Name is null ) { throw new NullReferenceException( nameof(child.InnerText) ); }

                switch ( child.Name )
                {
                    case XmlNames.KEY:
                        key = child.InnerText;
                        break;

                    case XmlNames.VALUE when double.TryParse( child.InnerText, out double n ):
                        value = n;
                        break;
                }
            }

            if ( string.IsNullOrWhiteSpace( key ) ) { throw new NullReferenceException( nameof(key) ); }

            results[key] = value;
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static Dictionary<string, double?> ToNullableDoubleDictionary( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        var results = new Dictionary<string, double?>();

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.Name is null ) { continue; }

            if ( node.Name != XmlNames.PAIR ) { throw new SerializationException( nameof(node.Name) ); }

            string  key   = "";
            double? value = default;

            for ( int c = 0; c < node.ChildNodes.Count; c++ )
            {
                XmlNode? child = node.ChildNodes[c];
                if ( child?.Name is null ) { throw new NullReferenceException( nameof(child.InnerText) ); }

                switch ( child.Name )
                {
                    case XmlNames.KEY:
                        key = child.InnerText;
                        break;

                    case XmlNames.VALUE when double.TryParse( child.InnerText, out double n ):
                        value = n;
                        break;
                }
            }

            if ( string.IsNullOrWhiteSpace( key ) ) { throw new NullReferenceException( nameof(key) ); }

            results[key] = value;
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static Dictionary<string, long> ToLongDictionary( this XmlDocument document )
    {
        var results = new Dictionary<string, long>();

        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.Name is null ) { continue; }

            if ( node.Name != XmlNames.PAIR ) { throw new SerializationException( nameof(node.Name) ); }

            string key   = "";
            long   value = default;

            for ( int c = 0; c < node.ChildNodes.Count; c++ )
            {
                XmlNode? child = node.ChildNodes[c];
                if ( child?.Name is null ) { throw new NullReferenceException( nameof(child.InnerText) ); }

                switch ( child.Name )
                {
                    case XmlNames.KEY:
                        key = child.InnerText;
                        break;

                    case XmlNames.VALUE when long.TryParse( child.InnerText, out long n ):
                        value = n;
                        break;
                }
            }

            if ( string.IsNullOrWhiteSpace( key ) ) { throw new NullReferenceException( nameof(key) ); }

            results[key] = value;
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static Dictionary<string, long?> ToNullableLongDictionary( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        var results = new Dictionary<string, long?>();

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.Name is null ) { continue; }

            if ( node.Name != XmlNames.PAIR ) { throw new SerializationException( nameof(node.Name) ); }

            string key   = "";
            long?  value = default;

            for ( int c = 0; c < node.ChildNodes.Count; c++ )
            {
                XmlNode? child = node.ChildNodes[c];
                if ( child?.Name is null ) { throw new NullReferenceException( nameof(child.InnerText) ); }

                switch ( child.Name )
                {
                    case XmlNames.KEY:
                        key = child.InnerText;
                        break;

                    case XmlNames.VALUE when long.TryParse( child.InnerText, out long n ):
                        value = n;
                        break;
                }
            }

            if ( string.IsNullOrWhiteSpace( key ) ) { throw new NullReferenceException( nameof(key) ); }

            results[key] = value;
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static Dictionary<string, int> ToIntDictionary( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        var results = new Dictionary<string, int>();

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.Name is null ) { continue; }

            if ( node.Name != XmlNames.PAIR ) { throw new SerializationException( nameof(node.Name) ); }

            string key   = "";
            int    value = default;

            for ( int c = 0; c < node.ChildNodes.Count; c++ )
            {
                XmlNode? child = node.ChildNodes[c];
                if ( child?.Name is null ) { throw new NullReferenceException( nameof(child.InnerText) ); }

                switch ( child.Name )
                {
                    case XmlNames.KEY:
                        key = child.InnerText;
                        break;

                    case XmlNames.VALUE when int.TryParse( child.InnerText, out int n ):
                        value = n;
                        break;
                }
            }

            if ( string.IsNullOrWhiteSpace( key ) ) { throw new NullReferenceException( nameof(key) ); }

            results[key] = value;
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static Dictionary<string, int?> ToNullableIntDictionary( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        var results = new Dictionary<string, int?>();

        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.Name is null ) { continue; }

            if ( node.Name != XmlNames.PAIR ) { throw new SerializationException( nameof(node.Name) ); }

            string key   = "";
            int    value = default;

            for ( int c = 0; c < node.ChildNodes.Count; c++ )
            {
                XmlNode? child = node.ChildNodes[c];
                if ( child?.Name is null ) { throw new NullReferenceException( nameof(child.InnerText) ); }

                switch ( child.Name )
                {
                    case XmlNames.KEY:
                        key = child.InnerText;
                        break;

                    case XmlNames.VALUE when int.TryParse( child.InnerText, out int n ):
                        value = n;
                        break;
                }
            }

            if ( string.IsNullOrWhiteSpace( key ) ) { throw new NullReferenceException( nameof(key) ); }

            results[key] = value;
        }

        return results;
    }

    #endregion



    #region lists

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static List<string> ToStringList( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        var results = new List<string>();

        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.InnerText is null ) { continue; }

            results.Add( node.InnerText );
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static List<Guid> ToGuidList( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        var results = new List<Guid>();

        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.InnerText is null ) { continue; }

            if ( Guid.TryParse( node.InnerText, out Guid n ) ) { results.Add( n ); }
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static List<bool> ToBoolList( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        var results = new List<bool>();

        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.InnerText is null ) { continue; }

            if ( bool.TryParse( node.InnerText, out bool n ) ) { results.Add( n ); }
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static List<double> ToDoubleList( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        var results = new List<double>();

        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.InnerText is null ) { continue; }

            results.Add( double.TryParse( node.InnerText, out double n )
                             ? n
                             : double.NaN );
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static List<double?> ToNullableDoubleList( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        var results = new List<double?>();

        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.InnerText is null ) { continue; }

            if ( double.TryParse( node.InnerText, out double n ) ) { results.Add( n ); }
            else { results.Add( null ); }
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static List<long> ToLongList( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        var results = new List<long>();

        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.InnerText is null ) { continue; }

            if ( long.TryParse( node.InnerText, out long n ) ) { results.Add( n ); }
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static List<long?> ToNullableLongList( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        var results = new List<long?>();

        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.InnerText is null ) { continue; }

            if ( long.TryParse( node.InnerText, out long n ) ) { results.Add( n ); }
            else { results.Add( null ); }
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static List<int> ToIntList( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        var results = new List<int>();

        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.InnerText is null ) { continue; }

            if ( int.TryParse( node.InnerText, out int n ) ) { results.Add( n ); }
        }

        return results;
    }

    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static List<int?> ToNullableIntList( this XmlDocument document, out IDictionary<string, string>? attributes )
    {
        var results = new List<int?>();

        XmlNode? root = document.ChildNodes[0];

        if ( root?.Name != XmlNames.ITEM ) { throw new SerializationException( nameof(root.Name) ); }

        XmlAttributeCollection? attributesCollection = root.Attributes;

        if ( attributesCollection is not null )
        {
            attributes = new Dictionary<string, string>();

            for ( int i = 0; i < attributesCollection.Count; i++ )
            {
                XmlAttribute attribute = attributesCollection[i];
                attributes[attribute.Name] = attribute.InnerText;
            }
        }
        else { attributes = default; }


        for ( int i = 0; i < root.ChildNodes.Count; i++ )
        {
            XmlNode? node = root.ChildNodes[i];
            if ( node?.InnerText is null ) { continue; }

            if ( int.TryParse( node.InnerText, out int n ) ) { results.Add( n ); }
            else { results.Add( null ); }
        }

        return results;
    }

    #endregion



    [ Obsolete( "Will be removed in a future version, and moved to a dedicated nuget" ) ]
    public static XmlDocument ToRawXml( this string xml )
    {
        var document = new XmlDocument();
        document.LoadXml( xml );

        return document;
    }

    #endregion
}