namespace Jakar.Xml;


public sealed class XmlNodeAttribute( string name, XmlInclude include = XmlInclude.All ) : Attribute
{
    public XmlInclude Include    { get; init; } = include;
    public string?    Name       { get; init; } = name;
    public bool       Properties { get; init; }


    public XmlNodeAttribute( Type type ) : this( type.Name ) { }

    public static XmlNodeAttribute Default( Type type ) => new(type.Name);

    internal BindingFlags FieldFlags()
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

        if ( ShouldIncludeFields() ) { flags |= BindingFlags.GetField | BindingFlags.SetField; }

        return flags;
    }


    internal BindingFlags PropFlags()
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        if ( ShouldIncludeProperties() ) { flags |= BindingFlags.GetProperty | BindingFlags.SetProperty; }

        return flags;
    }
    internal bool ShouldIncludeAttributes() => (Include & XmlInclude.Attributes) > 0;
    internal bool ShouldIncludeFields()     => (Include & XmlInclude.Fields)     > 0;


    internal bool ShouldIncludeProperties() => (Include & XmlInclude.Properties) > 0;
}
