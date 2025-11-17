namespace Jakar.Extensions;


public sealed partial class IniConfig : IReadOnlyDictionary<string, IniConfig.Section>, ISpanParsable<IniConfig>, ISpanFormattable
{
    private readonly ConcurrentDictionary<string, Section> __dictionary;


    public int  Count      => __dictionary.Count;
    public bool IsReadOnly => ( (ICollection<KeyValuePair<string, Section>>)__dictionary ).IsReadOnly;
    public Section this[ string sectionName ] { get => GetOrAdd(sectionName); set => __dictionary[sectionName] = value; }
    public IEnumerable<string> Keys => __dictionary.Keys;
    public int Length
    {
        get
        {
            int values = __dictionary.Values.Sum(static x => x.Length + 1);
            int result = values + __dictionary.Keys.Sum(static x => x.Length + 3);
            return result;
        }
    }
    public ConcurrentDictionary<string, Section>.AlternateLookup<ReadOnlySpan<char>> Lookup => __dictionary.GetAlternateLookup<ReadOnlySpan<char>>();
    public IEnumerable<Section>                                                      Values => __dictionary.Values;


    public IniConfig() : this(DEFAULT_CAPACITY) { }
    public IniConfig( int                                                capacity ) => __dictionary = new ConcurrentDictionary<string, Section>(Environment.ProcessorCount, capacity, StringComparer.OrdinalIgnoreCase);
    public IniConfig( IDictionary<string, Section>                       sections ) : this(sections.Count) { Add(sections); }
    public IniConfig( IEnumerable<Section>                               sections ) : this(DEFAULT_CAPACITY) { Add(sections); }
    public IniConfig( IEnumerable<KeyValuePair<string, Section>>         sections ) : this(DEFAULT_CAPACITY) { Add(sections); }
    public IniConfig( params ReadOnlySpan<KeyValuePair<string, Section>> sections ) : this(sections.Length) { Add(sections); }
    public IniConfig( params ReadOnlySpan<Section>                       sections ) : this() { Add(sections); }


    public static IniConfig ReadFromFile( LocalFile file, IFormatProvider? provider = null )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        string content = file.Read()
                             .AsString();

        return Parse(content, provider);
    }
    public static async ValueTask<IniConfig> ReadFromFileAsync( LocalFile file, IFormatProvider? provider = null, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        string content = await file.ReadAsync()
                                   .AsString(token);

        return Parse(content, provider);
    }


    public static IniConfig Parse( scoped in ReadOnlySpan<char> span ) => Parse(in span, CultureInfo.InvariantCulture);
    public static IniConfig Parse( scoped in ReadOnlySpan<char> span, IFormatProvider? provider )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        IniConfig           config        = new();

        // $"-- {nameof(IniConfig)}.{nameof(Refresh)}.{nameof(content)} --\n{content.ToString()}".WriteToConsole();
        if ( span.IsEmpty ) { return config; }


        string section = EMPTY;

        foreach ( ReadOnlySpan<char> rawLine in span.SplitOn() )
        {
            ReadOnlySpan<char> line = rawLine.Trim();
            if ( line.IsNullOrWhiteSpace() ) { continue; }


            Debug.Assert(line.ContainsNone('\n', '\r'));

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

                    if ( sectionSpan.IsNullOrWhiteSpace() ) { throw new FormatException("section title cannot be empty or whitespace."); }

                    section = sectionSpan.ToString();
                    config.Add(new Section(section));
                    continue;
            }

            line = line.Trim();
            if ( line.IsNullOrWhiteSpace() ) { continue; }


            int separator = line.IndexOf('='); // key = value OR "value"
            if ( separator < 0 ) { continue; }

            ReadOnlySpan<char> keySpan = line[..separator];

            string key = keySpan.Trim()
                                .ToString();

            ReadOnlySpan<char> valueSpan = line[( separator + 1 )..];
            valueSpan = valueSpan.Trim();
            valueSpan = valueSpan.Trim('"'); // Remove quotes;

            string value = valueSpan.ToString();

            Debug.Assert(!string.IsNullOrEmpty(section));

            if ( config[section]
               .ContainsKey(key) ) { throw new FormatException($"Duplicate key '{key}':  '{section}'"); }

            config[section][key] = value;
        }

        return config;
    }
    public static bool TryParse( scoped in ReadOnlySpan<char> span, IFormatProvider? provider, [NotNullWhen(true)] out IniConfig? result )
    {
        try
        {
            result = Parse(span, provider);
            return true;
        }
        catch ( Exception )
        {
            result = null;
            return false;
        }
    }


    static IniConfig IParsable<IniConfig>.    Parse( string?               s,    IFormatProvider? provider )                                            => Parse(s, provider);
    static bool IParsable<IniConfig>.         TryParse( string?            s,    IFormatProvider? provider, [NotNullWhen(true)] out IniConfig? result ) => TryParse(s, provider, out result);
    static IniConfig ISpanParsable<IniConfig>.Parse( ReadOnlySpan<char>    span, IFormatProvider? provider )                                            => Parse(span, provider);
    static bool ISpanParsable<IniConfig>.     TryParse( ReadOnlySpan<char> span, IFormatProvider? provider, [NotNullWhen(true)] out IniConfig? result ) => TryParse(span, provider, out result);


    /// <summary> Gets the <see cref="Section"/> with the <paramref name="sectionName"/> . If it doesn't exist, it is created, then returned. </summary>
    /// <param name="sectionName"> Section Name </param>
    /// <returns>
    ///     <see cref="Section"/>
    /// </returns>
    public Section GetOrAdd( string sectionName )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);
        return __dictionary.GetOrAdd(sectionName, Section.Create);
    }


    public IniConfig Add( string key, Section value )
    {
        __dictionary[key] = value;
        return this;
    }
    public IniConfig Add( IEnumerable<KeyValuePair<string, Section>> values )
    {
        foreach ( KeyValuePair<string, Section> section in values ) { Add(section); }

        return this;
    }
    public IniConfig Add( params ReadOnlySpan<KeyValuePair<string, Section>> values )
    {
        foreach ( KeyValuePair<string, Section> section in values ) { Add(section); }

        return this;
    }
    public IniConfig Add( KeyValuePair<string, Section> pair )
    {
        __dictionary.Add(pair);
        return this;
    }
    public IniConfig Add( IEnumerable<Section> values )
    {
        foreach ( Section section in values ) { Add(section); }

        return this;
    }
    public IniConfig Add( params ReadOnlySpan<Section> values )
    {
        foreach ( Section section in values ) { Add(section); }

        return this;
    }
    public IniConfig Add( Section value )
    {
        this[value.Name] = value;
        return this;
    }
    public bool Contains( KeyValuePair<string, Section> pair )                  => __dictionary.Contains(pair);
    public void CopyTo( KeyValuePair<string, Section>[] array, int arrayIndex ) => ( (ICollection<KeyValuePair<string, Section>>)__dictionary ).CopyTo(array, arrayIndex);
    public bool Remove( KeyValuePair<string, Section>   pair )                    => __dictionary.TryRemove(pair);
    public bool ContainsKey( Section                    pair )                    => ContainsKey(pair.Name);
    public bool Remove( Section                         pair )                    => Remove(pair.Name);
    public bool Remove( string                          key )                     => __dictionary.TryRemove(key, out _);
    public bool ContainsKey( string                     key )                     => __dictionary.ContainsKey(key);
    public void Clear()                                                           => __dictionary.Clear();
    public bool TryGetValue( string key, [NotNullWhen(true)] out Section? value ) => __dictionary.TryGetValue(key, out value);


    IEnumerator IEnumerable.                                   GetEnumerator() => GetEnumerator();
    public          IEnumerator<KeyValuePair<string, Section>> GetEnumerator() => __dictionary.GetEnumerator();
    public override string                                     ToString()      => ToString(null, CultureInfo.InvariantCulture);
    public string ToString( string? format, IFormatProvider? formatProvider )
    {
        Span<char> span = stackalloc char[Length + 10];
        if ( !TryFormat(span, out int charsWritten, format, formatProvider) ) { throw new InvalidOperationException("Cannot convert to string"); }

        ReadOnlySpan<char> result = span[..charsWritten];
        return result.ToString();
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)] public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        Debug.Assert(destination.Length >= Length);
        charsWritten = 0;

        foreach ( ( string key, Section section ) in __dictionary )
        {
            Span<char> span = destination[charsWritten..];

            if ( !section.TryFormat(span, out int sectionCharsWritten, format, provider) ) { throw new InvalidOperationException($"Cannot convert section '{section.Name}' to string"); }

            charsWritten                += sectionCharsWritten;
            destination[charsWritten++] =  '\n';
        }

        return true;
    }


    public ValueTask WriteToFile( LocalFile file ) => file.WriteAsync(ToString());
    public ValueTask WriteToFile( Stream stream, CancellationToken token = default )
    {
        ReadOnlyMemory<byte> buffer = Encoding.Default.GetBytes(ToString());
        return stream.WriteAsync(buffer, token);
    }
    public async ValueTask WriteToFile( StringWriter writer ) => await writer.WriteAsync(ToString());
}
