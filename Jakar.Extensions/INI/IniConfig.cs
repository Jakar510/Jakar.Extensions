namespace Jakar.Extensions;


public sealed partial class IniConfig( IEqualityComparer<string> comparer, int capacity = DEFAULT_CAPACITY ) : IReadOnlyDictionary<string, IniConfig.Section>, ISpanParsable<IniConfig>, ISpanFormattable
{
    private readonly ConcurrentDictionary<string, Section> _dictionary = new(Environment.ProcessorCount, capacity, comparer);
    private readonly IEqualityComparer<string>             _comparer   = comparer;

    public int  Count      => _dictionary.Count;
    public bool IsReadOnly => ((ICollection<KeyValuePair<string, Section>>)_dictionary).IsReadOnly;


    public Section this[ string sectionName ] { get => GetOrAdd( sectionName ); set => _dictionary[sectionName] = value; }
    public IEnumerable<string> Keys => _dictionary.Keys;
    public int Length
    {
        get
        {
            int values = _dictionary.Values.Sum( x => x.Length );
            int result = values + _dictionary.Keys.Count;
            return result;
        }
    }
    public ConcurrentDictionary<string, Section>.AlternateLookup<ReadOnlySpan<char>> Lookup => _dictionary.GetAlternateLookup<ReadOnlySpan<char>>();
    public IEnumerable<Section>                                                      Values => _dictionary.Values;


    public IniConfig() : this( StringComparer.OrdinalIgnoreCase ) { }
    public IniConfig( IDictionary<string, Section>               dictionary ) : this( dictionary, StringComparer.OrdinalIgnoreCase ) { }
    public IniConfig( IEnumerable<KeyValuePair<string, Section>> collection ) : this( collection, StringComparer.OrdinalIgnoreCase ) { }
    public IniConfig( IEnumerable<KeyValuePair<string, Section>> collection, IEqualityComparer<string> comparer ) : this( comparer )
    {
        foreach ( (string? key, Section? value) in collection ) { _dictionary[key] = value; }
    }
    public IniConfig( IDictionary<string, Section> dictionary, IEqualityComparer<string> comparer ) : this( comparer, dictionary.Count )
    {
        foreach ( (string? key, Section? value) in dictionary ) { _dictionary[key] = value; }
    }
    public IniConfig( IEnumerable<Section> sections ) : this()
    {
        foreach ( Section section in sections ) { Add( section ); }
    }
    public IniConfig( params ReadOnlySpan<Section> sections ) : this()
    {
        foreach ( Section section in sections ) { Add( section ); }
    }


    public static IniConfig ReadFromFile( LocalFile file, IFormatProvider? provider = null, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span    = parent.SubSpan();
        string              content = file.Read().AsString( in span );
        return Parse( content, provider );
    }
    public static async ValueTask<IniConfig> ReadFromFileAsync( LocalFile file, IFormatProvider? provider = null, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span    = parent.SubSpan();
        string              content = await file.ReadAsync().AsString( span, token );
        return Parse( content, provider, in span );
    }

    public static IniConfig Parse( scoped in ReadOnlySpan<char> span, in TelemetrySpan parent = default ) => Parse( span, CultureInfo.InvariantCulture, parent );
    public static IniConfig Parse( scoped in ReadOnlySpan<char> span, IFormatProvider? provider, in TelemetrySpan parent = default )
    {
        using TelemetrySpan telemetrySpan = parent.SubSpan();
        IniConfig           config        = new();

        // $"-- {nameof(IniConfig)}.{nameof(Refresh)}.{nameof(content)} --\n{content.ToString()}".WriteToConsole();
        if ( span.IsEmpty ) { return config; }


        string section = string.Empty;

        foreach ( ReadOnlySpan<char> rawLine in span.SplitOn() )
        {
            ReadOnlySpan<char> line = rawLine.Trim();
            if ( line.IsNullOrWhiteSpace() ) { continue; }


            Debug.Assert( line.Contains( '\n', '\r' ) is false );

            switch ( line[0] )
            {
                // Ignore comments
                case ';':
                case '#':
                case '/':
                    continue;

                // [Section:header]
                case '[' when line[^1] == ']':
                    ReadOnlySpan<char> sectionSpan = line[1..^1];
                    sectionSpan = sectionSpan.Trim(); // remove the brackets and whitespace

                    if ( sectionSpan.IsNullOrWhiteSpace() ) { throw new FormatException( "section title cannot be empty or whitespace." ); }

                    section = sectionSpan.ToString();
                    config.Add( new Section( section ) );
                    continue;
            }

            line = line.Trim();
            if ( line.IsNullOrWhiteSpace() ) { continue; }


            int separator = line.IndexOf( '=' ); // key = value OR "value"
            if ( separator < 0 ) { continue; }

            ReadOnlySpan<char> keySpan = line[..separator];
            string             key     = keySpan.Trim().ToString();

            ReadOnlySpan<char> valueSpan = line[(separator + 1)..];
            valueSpan = valueSpan.Trim();
            valueSpan = valueSpan.Trim( '"' ); // Remove quotes;

            string value = valueSpan.ToString();

            Debug.Assert( !string.IsNullOrEmpty( section ) );

            if ( config[section].ContainsKey( key ) ) { throw new FormatException( $"Duplicate key '{key}':  '{section}'" ); }

            config[section][key] = value;
        }

        return config;
    }
    public static bool TryParse( scoped in ReadOnlySpan<char> span, IFormatProvider? provider, [NotNullWhen( true )] out IniConfig? result )
    {
        try
        {
            result = Parse( span, provider );
            return true;
        }
        catch ( Exception )
        {
            result = null;
            return false;
        }
    }


    static IniConfig IParsable<IniConfig>.    Parse( string?               s,    IFormatProvider? provider )                                              => Parse( s, provider );
    static bool IParsable<IniConfig>.         TryParse( string?            s,    IFormatProvider? provider, [NotNullWhen( true )] out IniConfig? result ) => TryParse( s, provider, out result );
    static IniConfig ISpanParsable<IniConfig>.Parse( ReadOnlySpan<char>    span, IFormatProvider? provider )                                              => Parse( span, provider );
    static bool ISpanParsable<IniConfig>.     TryParse( ReadOnlySpan<char> span, IFormatProvider? provider, [NotNullWhen( true )] out IniConfig? result ) => TryParse( span, provider, out result );


    /// <summary> Gets the <see cref="Section"/> with the <paramref name="sectionName"/> . If it doesn't exist, it is created, then returned. </summary>
    /// <param name="sectionName"> Section AppName </param>
    /// <returns>
    ///     <see cref="Section"/>
    /// </returns>
    public Section GetOrAdd( string sectionName )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace( sectionName );

        foreach ( string key in _dictionary.Keys )
        {
            if ( _comparer.Equals( key, sectionName ) ) { return _dictionary[key]; }
        }

        return _dictionary[sectionName] = new Section( sectionName );
    }


    public IniConfig Add( params ReadOnlySpan<Section> values )
    {
        foreach ( Section section in values ) { Add( section ); }

        return this;
    }
    public IniConfig Add( IEnumerable<Section> values )
    {
        foreach ( Section section in values ) { Add( section ); }

        return this;
    }
    public IniConfig Add( Section value )
    {
        this[value.Name] = value;
        return this;
    }
    public IniConfig Add( KeyValuePair<string, Section> pair )
    {
        _dictionary.Add( pair );
        return this;
    }
    public IniConfig Add( string key, Section value )
    {
        _dictionary[key] = value;
        return this;
    }
    public bool Contains( KeyValuePair<string, Section> pair )                  => _dictionary.Contains( pair );
    public void CopyTo( KeyValuePair<string, Section>[] array, int arrayIndex ) => ((ICollection<KeyValuePair<string, Section>>)_dictionary).CopyTo( array, arrayIndex );
    public bool Remove( KeyValuePair<string, Section>   pair )                      => _dictionary.TryRemove( pair );
    public bool Remove( string                          key )                       => _dictionary.TryRemove( key, out _ );
    public bool ContainsKey( string                     key )                       => _dictionary.ContainsKey( key );
    public void Clear()                                                             => _dictionary.Clear();
    public bool TryGetValue( string key, [NotNullWhen( true )] out Section? value ) => _dictionary.TryGetValue( key, out value );


    IEnumerator IEnumerable.                                   GetEnumerator() => GetEnumerator();
    public          IEnumerator<KeyValuePair<string, Section>> GetEnumerator() => _dictionary.GetEnumerator();
    public override string                                     ToString()      => ToString( null, CultureInfo.InvariantCulture );
    public string ToString( string? format, IFormatProvider? formatProvider )
    {
        Span<char> span = stackalloc char[Length + 10];
        if ( TryFormat( span, out int charsWritten, format, formatProvider ) is false ) { throw new InvalidOperationException( "Cannot convert to string" ); }

        ReadOnlySpan<char> result = span[..charsWritten];
        return result.ToString();
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        Debug.Assert( destination.Length >= Length );
        charsWritten = 0;

        foreach ( (string key, Section section) in this )
        {
            Span<char> span = destination[charsWritten..];
            if ( section.TryFormat( span, out int sectionCharsWritten, format, provider ) ) { charsWritten += sectionCharsWritten; }

            destination[charsWritten++] = '\n';
        }

        return true;
    }


    public ValueTask WriteToFile( LocalFile file ) => file.WriteAsync( ToString() );
    public ValueTask WriteToFile( Stream stream, CancellationToken token = default )
    {
        ReadOnlyMemory<byte> buffer = Encoding.Default.GetBytes( ToString() );
        return stream.WriteAsync( buffer, token );
    }
    public async ValueTask WriteToFile( StringWriter writer ) => await writer.WriteAsync( ToString() );
}
