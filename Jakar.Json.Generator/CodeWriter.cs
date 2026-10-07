// Jakar.Json.Generator
// 10/07/2026

using System;
using System.Text;



namespace Jakar.Json.Generator;


/// <summary> A <see cref="StringBuilder"/> with indentation and braces, for emitting readable generated code. </summary>
internal sealed class CodeWriter
{
    private readonly StringBuilder __builder = new(8 * 1024);
    private          int           __indent;


    public CodeWriter Line( string text = "" )
    {
        if ( text.Length > 0 ) { __builder.Append(' ', __indent * 4); }

        __builder.Append(text).Append('\n');
        return this;
    }

    public CodeWriter Open( string header )
    {
        Line(header);
        Line("{");
        __indent++;
        return this;
    }

    public CodeWriter Close( string suffix = "" )
    {
        __indent--;
        Line("}" + suffix);
        return this;
    }

    public IDisposable Block( string header )
    {
        Open(header);
        return new Closer(this);
    }

    public override string ToString() => __builder.ToString();



    private sealed class Closer( CodeWriter writer ) : IDisposable
    {
        public void Dispose() => writer.Close();
    }
}
