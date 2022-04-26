// Jakar.Extensions :: Jakar.Json
// 04/26/2022  10:52 AM

using System.Text;
using Newtonsoft.Json;



namespace Jakar.Json.Serialization;


public ref struct JContext
{
    public const    char       INDENT = '\t';
    public readonly Formatting formatting;
    public          int        indentLevel = default;


    public JContext() : this(Formatting.None) { }
    public JContext( Formatting formatting ) => this.formatting = formatting;


    public void Increase() => indentLevel += 1;
    public void Decrease() => indentLevel -= 1;

    public bool Indent( ref StringBuilder sb )
    {
        if ( formatting is Formatting.None ) { return false; }

        for ( var i = 0; i < indentLevel; i++ ) { sb.Append(INDENT); }

        return true;
    }

    // public void Dispose() { }
}
