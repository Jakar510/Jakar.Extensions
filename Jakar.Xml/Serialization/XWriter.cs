#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;



namespace Jakar.Xml.Serialization;


[SuppressMessage( "ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable" )]
public ref struct XWriter
{
    public const      string        NULL = "null";
    private readonly  StringBuilder _sb  = new(); // TODO: System.Text.ValueStringBuilder
    internal readonly bool          shouldIndent;
    internal          int           indentLevel = default;


    public XWriter() : this( true ) { }
    public XWriter( bool shouldIndent ) => this.shouldIndent = shouldIndent;


    public XArray AddArray( in   ReadOnlySpan<char> name ) => new(name, ref this);
    public XObject AddObject( in ReadOnlySpan<char> name ) => new(name, ref this);

    public XWriter Add( in XObject parent, in ReadOnlySpan<char> key, in IEnumerable<IXmlizer> enumerable )
    {
        using XArray node = parent.AddArray( key );
        return Add( node, enumerable );
    }
    public XWriter Add( in XArray parent, in IEnumerable<IXmlizer> enumerable )
    {
        foreach (IXmlizer item in enumerable)
        {
            ReadOnlySpan<char> name = item.Name;
            using XObject      node = parent.AddObject( name );
            item.Serialize( node );
        }

        return this;
    }
    public XWriter Add( in XObject parent, in ReadOnlySpan<char> key, in IDictionary dictionary )
    {
        using XObject node = parent.AddObject( key );

        foreach (DictionaryEntry pair in dictionary) { node.Add( pair ); }

        return this;
    }
    public XWriter Add( in XObject parent, in ReadOnlySpan<char> key, in IDictionary<string, IXmlizer> dictionary )
    {
        using XObject node = parent.AddObject( key );

        foreach ((string? k, IXmlizer? value) in dictionary)
        {
            using XObject item = node.AddObject( k );
            value.Serialize( item );
        }

        return this;
    }


    public void StartBlock( in ReadOnlySpan<char> name )
    {
        _sb.Append( '<' )
           .Append( name )
           .Append( '>' );

        if (shouldIndent)
        {
            _sb.Append( '\n' );
            indentLevel += 1;
        }
    }
    public void StartBlock( in ReadOnlySpan<char> name, in XAttributeBuilder builder )
    {
        _sb.Append( '<' )
           .Append( name )
           .Append( ' ' )
           .Append( builder.sb )
           .Append( '>' );

        if (shouldIndent)
        {
            _sb.Append( '\n' );
            indentLevel += 1;
        }
    }
    public void FinishBlock( in ReadOnlySpan<char> name )
    {
        _sb.Append( "</" )
           .Append( name )
           .Append( '>' );

        if (shouldIndent)
        {
            _sb.Append( '\n' );
            indentLevel -= 1;
        }
    }


    public XWriter Indent( in ReadOnlySpan<char> key )
    {
        if (shouldIndent)
        {
            // throw new InvalidOperationException($"{nameof(Indent)} should not be used in this context"); 
            _sb.Append( '\t', indentLevel );
        }

        _sb.Append( '<' )
           .Append( key )
           .Append( '>' );

        return this;
    }
    public XWriter Next( in ReadOnlySpan<char> key )
    {
        _sb.Append( "</" )
           .Append( key )
           .Append( '>' );

        if (shouldIndent) { _sb.Append( '\n' ); }

        return this;
    }


    public XWriter Null()
    {
        _sb.Append( NULL );
        return this;
    }
    public XWriter Append( in string value ) => Append( value.AsSpan() );
    public XWriter Append( in ReadOnlySpan<char> value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( in char value )
    {
        _sb.Append( value );
        return this;
    }


    public XWriter Append( in short value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( in ushort value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( in int value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( in uint value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( in long value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( in ulong value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( in float value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( in double value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( in decimal value )
    {
        _sb.Append( value );
        return this;
    }


    public XWriter Append( in ISpanFormattable value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize )
    {
        Span<char> buffer = stackalloc char[bufferSize];

        if (!value.TryFormat( buffer, out int charsWritten, format, culture )) { throw new InvalidOperationException( $"Can't format value: '{value}'" ); }

        _sb.Append( buffer[..charsWritten] );
        return this;
    }
    public XWriter Append<T>( in T? value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : struct, ISpanFormattable
    {
        if (value.HasValue)
        {
            Span<char> buffer = stackalloc char[bufferSize];

            if (!value.Value.TryFormat( buffer, out int charsWritten, format, culture )) { throw new InvalidOperationException( $"Can't format value: '{value}'" ); }

            _sb.Append( buffer[..charsWritten] );
        }
        else { _sb.Append( NULL ); }

        return this;
    }


    public override string ToString() => _sb.ToString();
    public void Dispose() => _sb.Clear();
}
