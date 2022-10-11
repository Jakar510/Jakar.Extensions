// Jakar.Extensions :: Jakar.Xml
// 04/27/2022  11:38 AM

#nullable enable
using System;
using System.Text;



namespace Jakar.Xml.Serialization;


public readonly ref struct XAttributeBuilder
{
    internal readonly StringBuilder sb = new();

    public XAttributeBuilder() { }
    public XAttributeBuilder( in Type               type ) : this( type.AssemblyQualifiedName ?? type.FullName ?? type.Name ) { }
    public XAttributeBuilder( in ReadOnlySpan<char> xmls ) => With( nameof(xmls), xmls );


    public XAttributeBuilder With( in ReadOnlySpan<char> key, in ReadOnlySpan<char> value )
    {
        sb.Append( ' ' )
          .Append( key )
          .Append( '=' )
          .Append( '"' )
          .Append( value )
          .Append( '"' );

        return this;
    }

    public override string ToString() => sb.ToString();
}
