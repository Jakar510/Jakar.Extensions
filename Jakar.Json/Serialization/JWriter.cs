using System.Globalization;



namespace Jakar.Json.Serialization;


[SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable")]
public ref struct JWriter
{
    public const      string        NULL = "null";
    private readonly  StringBuilder _sb  = new();
    internal readonly bool          shouldIndent;
    private           int           _indentLevel = 0;


    public JWriter() : this(Formatting.None) { }
    public JWriter( Formatting formatting ) => shouldIndent = formatting is Formatting.None;


    public JArray AddArray() => new(ref this);
    public JObject AddObject() => new(ref this);
    public JWriter Add( in JObject parent, in ReadOnlySpan<char> key, in IEnumerable<IJsonizer> enumerable )
    {
        using JArray node = parent.AddArray(key);
        return Add(node, enumerable);
    }
    public JWriter Add( in JArray parent, in IEnumerable<IJsonizer> enumerable )
    {
        foreach ( IJsonizer item in enumerable )
        {
            using ( JObject node = parent.AddObject() ) { item.Serialize(node); }

            Next();
        }

        return this;
    }
    public JWriter Add( in JObject parent, in ReadOnlySpan<char> key, in IDictionary dictionary )
    {
        using JObject node = parent.AddObject(key);

        foreach ( DictionaryEntry pair in dictionary )
        {
            node.Add(pair);
            Next();
        }

        return this;
    }
    public JWriter Add( in JObject parent, in ReadOnlySpan<char> key, in IDictionary<string, IJsonizer> dictionary )
    {
        using JObject node = parent.AddObject(key);

        foreach ( ( string? k, IJsonizer? value ) in dictionary )
        {
            using ( JObject item = node.AddObject(k) ) { value.Serialize(item); }

            Next();
        }

        return this;
    }


    public void StartBlock( in char start )
    {
        _sb.Append(start);

        if ( shouldIndent )
        {
            _sb.Append('\n');
            _indentLevel += 1;
        }
    }
    public void FinishBlock( in char end )
    {
        _sb.Append(end);

        if ( shouldIndent )
        {
            _sb.Append('\n');
            _indentLevel -= 1;
        }
    }


    public JWriter Indent()
    {
        if ( shouldIndent )
        {
            // throw new InvalidOperationException($"{nameof(Indent)} should not be used in this context"); 
            _sb.Append('\t', _indentLevel);
        }

        return this;
    }
    public JWriter Next()
    {
        _sb.Append(',');
        if ( shouldIndent ) { _sb.Append('\n'); }

        return this;
    }


    public JWriter Null()
    {
        _sb.Append(NULL);
        return this;
    }
    public JWriter Append( in string value ) => Append(value.AsSpan());
    public JWriter Append( in ReadOnlySpan<char> value )
    {
        _sb.Append(value);
        return this;
    }
    public JWriter Append( in char value )
    {
        _sb.Append(value);
        return this;
    }


    public JWriter Append( in short value )
    {
        _sb.Append(value);
        return this;
    }
    public JWriter Append( in ushort value )
    {
        _sb.Append(value);
        return this;
    }
    public JWriter Append( in int value )
    {
        _sb.Append(value);
        return this;
    }
    public JWriter Append( in uint value )
    {
        _sb.Append(value);
        return this;
    }
    public JWriter Append( in long value )
    {
        _sb.Append(value);
        return this;
    }
    public JWriter Append( in ulong value )
    {
        _sb.Append(value);
        return this;
    }
    public JWriter Append( in float value )
    {
        _sb.Append(value);
        return this;
    }
    public JWriter Append( in double value )
    {
        _sb.Append(value);
        return this;
    }
    public JWriter Append( in decimal value )
    {
        _sb.Append(value);
        return this;
    }


    public JWriter Append( in ISpanFormattable value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize )
    {
        Span<char> buffer = stackalloc char[bufferSize];

        if ( !value.TryFormat(buffer, out int charsWritten, format, culture) ) { throw new InvalidOperationException($"Can't format value: '{value}'"); }

        _sb.Append(buffer[..charsWritten]);
        return this;
    }
    public JWriter Append<T>( in T? value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : struct, ISpanFormattable
    {
        if ( value.HasValue )
        {
            Span<char> buffer = stackalloc char[bufferSize];

            if ( !value.Value.TryFormat(buffer, out int charsWritten, format, culture) ) { throw new InvalidOperationException($"Can't format value: '{value}'"); }

            _sb.Append(buffer[..charsWritten]);
        }
        else { _sb.Append(NULL); }

        return this;
    }


    public override string ToString() => _sb.ToString();
    public void Dispose() => _sb.Clear();
}
