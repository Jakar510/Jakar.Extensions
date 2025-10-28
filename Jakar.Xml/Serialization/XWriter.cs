namespace Jakar.Xml.Serialization;


[SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable")]
public ref struct XWriter( bool shouldIndent )
{
    public const      string        NULL         = "null";
    private readonly  StringBuilder __sb         = new(); // TODO: System.Text.ValueStringBuilder
    internal readonly bool          shouldIndent = shouldIndent;
    internal          int           IndentLevel  = 0;


    public XWriter() : this(true) { }


    public XArray  AddArray( ReadOnlySpan<char>  name ) => new(name, this);
    public XObject AddObject( ReadOnlySpan<char> name ) => new(name, this);


    public XWriter Add( XObject parent, ReadOnlySpan<char> key, IEnumerable<IXmlizer> enumerable )
    {
        using XArray node = parent.AddArray(key);
        return Add(node, enumerable);
    }
    public XWriter Add( XArray parent, IEnumerable<IXmlizer> enumerable )
    {
        foreach ( IXmlizer item in enumerable )
        {
            ReadOnlySpan<char> name = item.Name;
            using XObject      node = parent.AddObject(name);
            item.Serialize(node);
        }

        return this;
    }
    public XWriter Add( XObject parent, ReadOnlySpan<char> key, IDictionary dictionary )
    {
        using XObject node = parent.AddObject(key);

        foreach ( DictionaryEntry pair in dictionary ) { node.Add(pair); }

        return this;
    }
    public XWriter Add( XObject parent, ReadOnlySpan<char> key, IDictionary<string, IXmlizer> dictionary )
    {
        using XObject node = parent.AddObject(key);

        foreach ( ( string? k, IXmlizer? value ) in dictionary )
        {
            using XObject item = node.AddObject(k);
            value.Serialize(item);
        }

        return this;
    }


    public void StartBlock( ReadOnlySpan<char> name )
    {
        __sb.Append('<')
            .Append(name)
            .Append('>');

        if ( shouldIndent )
        {
            __sb.Append('\n');
            IndentLevel += 1;
        }
    }
    public void StartBlock( ReadOnlySpan<char> name, XAttributeBuilder builder )
    {
        __sb.Append('<')
            .Append(name)
            .Append(' ')
            .Append(builder.sb)
            .Append('>');

        if ( shouldIndent )
        {
            __sb.Append('\n');
            IndentLevel += 1;
        }
    }
    public void FinishBlock( ReadOnlySpan<char> name )
    {
        __sb.Append("</")
            .Append(name)
            .Append('>');

        if ( shouldIndent )
        {
            __sb.Append('\n');
            IndentLevel -= 1;
        }
    }


    public XWriter Indent( ReadOnlySpan<char> key )
    {
        if ( shouldIndent )
        {
            // throw new InvalidOperationException($"{nameof(Indent)} should not be used  this context"); 
            __sb.Append('\t', IndentLevel);
        }

        __sb.Append('<')
            .Append(key)
            .Append('>');

        return this;
    }
    public XWriter Next( ReadOnlySpan<char> key )
    {
        __sb.Append("</")
            .Append(key)
            .Append('>');

        if ( shouldIndent ) { __sb.Append('\n'); }

        return this;
    }


    public XWriter Null()
    {
        __sb.Append(NULL);
        return this;
    }
    public XWriter Append( string value ) => Append(value.AsSpan());
    public XWriter Append( ReadOnlySpan<char> value )
    {
        __sb.Append(value);
        return this;
    }
    public XWriter Append( char value )
    {
        __sb.Append(value);
        return this;
    }


    public XWriter Append( short value )
    {
        __sb.Append(value);
        return this;
    }
    public XWriter Append( ushort value )
    {
        __sb.Append(value);
        return this;
    }
    public XWriter Append( int value )
    {
        __sb.Append(value);
        return this;
    }
    public XWriter Append( uint value )
    {
        __sb.Append(value);
        return this;
    }
    public XWriter Append( long value )
    {
        __sb.Append(value);
        return this;
    }
    public XWriter Append( ulong value )
    {
        __sb.Append(value);
        return this;
    }
    public XWriter Append( float value )
    {
        __sb.Append(value);
        return this;
    }
    public XWriter Append( double value )
    {
        __sb.Append(value);
        return this;
    }
    public XWriter Append( decimal value )
    {
        __sb.Append(value);
        return this;
    }


    public XWriter Append( ISpanFormattable value, ReadOnlySpan<char> format, CultureInfo culture, int bufferSize )
    {
        Span<char> buffer = stackalloc char[bufferSize];

        if ( !value.TryFormat(buffer, out int charsWritten, format, culture) ) { throw new InvalidOperationException($"Can't format value: '{value}'"); }

        __sb.Append(buffer[..charsWritten]);
        return this;
    }
    public XWriter Append<TValue>( TValue? value, ReadOnlySpan<char> format, CultureInfo culture, int bufferSize )
        where TValue : struct, ISpanFormattable
    {
        if ( value.HasValue )
        {
            Span<char> buffer = stackalloc char[bufferSize];

            if ( !value.Value.TryFormat(buffer, out int charsWritten, format, culture) ) { throw new InvalidOperationException($"Can't format value: '{value}'"); }

            __sb.Append(buffer[..charsWritten]);
        }
        else { __sb.Append(NULL); }

        return this;
    }


    public override string ToString() => __sb.ToString();
    public          void   Dispose()  => __sb.Clear();
}
