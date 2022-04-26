// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  10:53 AM

using System;
using System.Text;
using Newtonsoft.Json;



namespace Jakar.Xml.Serialization;


public ref struct XContext
{
    public readonly Formatting         formatting;
    public readonly ReadOnlySpan<char> indent;
    public          int                indentLevel = default;


    public XContext( Formatting formatting, params char[] indent )
    {
        this.formatting = formatting;
        this.indent     = indent;
    }
    public XContext( Formatting formatting, ReadOnlySpan<char> indent )
    {
        this.formatting = formatting;
        this.indent     = indent;
    }


    public void Increase() => indentLevel += 1;
    public void Decrease() => indentLevel -= 1;


    public bool Indent( in StringBuilder sb )
    {
        if ( formatting is Formatting.None ) { return false; }

        for ( var i = 0; i < indentLevel; i++ ) { sb.Append(indent); }

        return true;
    }

    // public void Dispose() { }
}
