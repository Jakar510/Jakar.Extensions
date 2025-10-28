// Jakar.Extensions :: Jakar.Xml
// 04/27/2022  11:38 AM

namespace Jakar.Xml.Serialization;


public readonly ref struct XAttributeBuilder
{
    internal readonly StringBuilder sb = new();

    public XAttributeBuilder() { }
    public XAttributeBuilder( Type               type ) : this(type.AssemblyQualifiedName ?? type.FullName ?? type.Name) { }
    public XAttributeBuilder( ReadOnlySpan<char> xmls ) => With(nameof(xmls), xmls);


    public XAttributeBuilder With( ReadOnlySpan<char> key, ReadOnlySpan<char> value )
    {
        sb.Append(' ')
          .Append(key)
          .Append('=')
          .Append('"')
          .Append(value)
          .Append('"');

        return this;
    }

    public override string ToString() => sb.ToString();
}
