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


    public XArray AddArray( ReadOnlySpan<char>   name ) => new(name, ref this);
    public XObject AddObject( ReadOnlySpan<char> name ) => new(name, ref this);

    public XWriter Add( XObject parent, ReadOnlySpan<char> key, IEnumerable<IXmlizer> enumerable )
    {
        using XArray node = parent.AddArray( key );
        return Add( node, enumerable );
    }
    public XWriter Add( XArray parent, IEnumerable<IXmlizer> enumerable )
    {
        foreach ( IXmlizer item in enumerable )
        {
            ReadOnlySpan<char> name = item.Name;
            using XObject      node = parent.AddObject( name );
            item.Serialize( node );
        }

        return this;
    }
    public XWriter Add( XObject parent, ReadOnlySpan<char> key, IDictionary dictionary )
    {
        using XObject node = parent.AddObject( key );

        foreach ( DictionaryEntry pair in dictionary ) { node.Add( pair ); }

        return this;
    }
    public XWriter Add( XObject parent, ReadOnlySpan<char> key, IDictionary<string, IXmlizer> dictionary )
    {
        using XObject node = parent.AddObject( key );

        foreach ( (string? k, IXmlizer? value) in dictionary )
        {
            using XObject item = node.AddObject( k );
            value.Serialize( item );
        }

        return this;
    }


    public void StartBlock( ReadOnlySpan<char> name )
    {
        _sb.Append( '<' )
           .Append( name )
           .Append( '>' );

        if ( shouldIndent )
        {
            _sb.Append( '\n' );
            indentLevel += 1;
        }
    }
    public void StartBlock( ReadOnlySpan<char> name, XAttributeBuilder builder )
    {
        _sb.Append( '<' )
           .Append( name )
           .Append( ' ' )
           .Append( builder.sb )
           .Append( '>' );

        if ( shouldIndent )
        {
            _sb.Append( '\n' );
            indentLevel += 1;
        }
    }
    public void FinishBlock( ReadOnlySpan<char> name )
    {
        _sb.Append( "</" )
           .Append( name )
           .Append( '>' );

        if ( shouldIndent )
        {
            _sb.Append( '\n' );
            indentLevel -= 1;
        }
    }


    public XWriter Indent( ReadOnlySpan<char> key )
    {
        if ( shouldIndent )
        {
            // throw new InvalidOperationException($"{nameof(Indent)} should not be used  this context"); 
            _sb.Append( '\t', indentLevel );
        }

        _sb.Append( '<' )
           .Append( key )
           .Append( '>' );

        return this;
    }
    public XWriter Next( ReadOnlySpan<char> key )
    {
        _sb.Append( "</" )
           .Append( key )
           .Append( '>' );

        if ( shouldIndent ) { _sb.Append( '\n' ); }

        return this;
    }


    public XWriter Null()
    {
        _sb.Append( NULL );
        return this;
    }
    public XWriter Append( string value ) => Append( value.AsSpan() );
    public XWriter Append( ReadOnlySpan<char> value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( char value )
    {
        _sb.Append( value );
        return this;
    }


    public XWriter Append( short value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( ushort value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( int value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( uint value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( long value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( ulong value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( float value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( double value )
    {
        _sb.Append( value );
        return this;
    }
    public XWriter Append( decimal value )
    {
        _sb.Append( value );
        return this;
    }


    public XWriter Append( ISpanFormattable value, ReadOnlySpan<char> format, CultureInfo culture, int bufferSize )
    {
        Span<char> buffer = stackalloc char[bufferSize];

        if ( !value.TryFormat( buffer, out int charsWritten, format, culture ) ) { throw new InvalidOperationException( $"Can't format value: '{value}'" ); }

        _sb.Append( buffer[..charsWritten] );
        return this;
    }
    public XWriter Append<T>( T? value, ReadOnlySpan<char> format, CultureInfo culture, int bufferSize ) where T : struct, ISpanFormattable
    {
        if ( value.HasValue )
        {
            Span<char> buffer = stackalloc char[bufferSize];

            if ( !value.Value.TryFormat( buffer, out int charsWritten, format, culture ) ) { throw new InvalidOperationException( $"Can't format value: '{value}'" ); }

            _sb.Append( buffer[..charsWritten] );
        }
        else { _sb.Append( NULL ); }

        return this;
    }


    public override string ToString() => _sb.ToString();
    public void Dispose() => _sb.Clear();
}
