#nullable enable
namespace Jakar.Extensions;


public sealed partial class IniConfig : ConcurrentDictionary<string, IniConfig.Section>
                                    #if NET6_0_OR_GREATER
                                        ,
                                        ISpanFormattable
#endif
#if NET7_0_OR_GREATER
                                        ,
                                        ISpanParsable<IniConfig>
#endif

{
    private readonly IEqualityComparer<string> _comparer;


    public int Length
    {
        get
        {
            int values = Values.Sum( x => x.Length );
            int result = values + Keys.Count;
            return result;
        }
    }
    public new Section this[ string sectionName ]
    {
        get => GetOrAdd( sectionName );
        set => base[sectionName] = value;
    }


    public IniConfig() : this( StringComparer.OrdinalIgnoreCase ) { }
    public IniConfig( IEqualityComparer<string>                  comparer ) : base( comparer ) => _comparer = comparer;
    public IniConfig( IDictionary<string, Section>               dictionary ) : this( dictionary, StringComparer.OrdinalIgnoreCase ) { }
    public IniConfig( IDictionary<string, Section>               dictionary, IEqualityComparer<string> comparer ) : base( dictionary, comparer ) => _comparer = comparer;
    public IniConfig( IEnumerable<KeyValuePair<string, Section>> collection ) : this( collection, StringComparer.OrdinalIgnoreCase ) { }
    public IniConfig( IEnumerable<KeyValuePair<string, Section>> collection, IEqualityComparer<string> comparer ) : base( collection, comparer ) => _comparer = comparer;
    public IniConfig( IEnumerable<Section> sections ) : this()
    {
        foreach ( Section section in sections ) { Add( section ); }
    }


    public static IniConfig ReadFromFile( LocalFile file, IFormatProvider? provider = default )
    {
        ReadOnlySpan<char> content = file.Read()
                                         .AsSpan();

        return Parse( content, provider );
    }
    public static async ValueTask<IniConfig> ReadFromFileAsync( LocalFile file, IFormatProvider? provider = default )
    {
        string content = await file.ReadAsync()
                                   .AsString();

        return Parse( content, provider );
    }


    /// <summary> Gets the <see cref="Section"/> with the <paramref name="sectionName"/> . If it doesn't exist, it is created, then returned. </summary>
    /// <param name="sectionName"> Section Name </param>
    /// <returns>
    ///     <see cref="Section"/>
    /// </returns>
    public Section GetOrAdd( string sectionName )
    {
        if ( string.IsNullOrEmpty( sectionName ) ) { throw new ArgumentNullException( nameof(sectionName) ); }

        foreach ( string key in Keys )
        {
            if ( _comparer.Equals( key, sectionName ) ) { return base[key]; }
        }

        return base[sectionName] = new Section( sectionName );
    }


    public static IniConfig Parse( string s ) => Parse( s, CultureInfo.InvariantCulture );
    public static IniConfig Parse( string? s, IFormatProvider? provider )
    {
        ReadOnlySpan<char> span = s;
        return Parse( span, provider );
    }
    public static bool TryParse( string? s, IFormatProvider? provider, [NotNullWhen( true )] out IniConfig? result )
    {
        ReadOnlySpan<char> span = s;
        return TryParse( span, provider, out result );
    }


    public static IniConfig Parse( ReadOnlySpan<char> span, IFormatProvider? provider )
    {
        var config = new IniConfig();

        // $"-- {nameof(IniConfig)}.{nameof(Refresh)}.{nameof(content)} --\n{content.ToString()}".WriteToConsole();
        if ( span.IsEmpty ) { return config; }


        string section = string.Empty;

        foreach ( ReadOnlySpan<char> rawLine in span.SplitOn() )
        {
            ReadOnlySpan<char> line = rawLine.Trim();
            if ( line.IsNullOrWhiteSpace() ) { continue; }


            Debug.Assert( !line.Contains( '\n' ) );

            switch ( line[0] )
            {
                // Ignore comments
                case ';':
                case '#':
                case '/':
                    continue;

                // [Section:header]
                case '[' when line[^1] == ']':
                    ReadOnlySpan<char> sectionSpan = line.Slice( 1, line.Length - 2 )
                                                         .Trim(); // remove the brackets and whitespace

                    if ( sectionSpan.IsNullOrWhiteSpace() ) { throw new FormatException( "section title cannot be empty or whitespace." ); }

                    section = sectionSpan.ToString();
                    config.Add( new Section( section ) );
                    continue;
            }


            if ( line.Trim()
                     .IsNullOrWhiteSpace() ) { continue; }


            int separator = line.IndexOf( '=' ); // key = value OR "value"
            if ( separator < 0 ) { continue; }


            string key = line[..separator]
                        .Trim()
                        .ToString();

            string value = line[(separator + 1)..]
                          .Trim()
                          .Trim( '"' ) // Remove quotes;
                          .ToString();

            Debug.Assert( !string.IsNullOrEmpty( section ) );

            if ( config[section]
               .ContainsKey( key ) ) { throw new FormatException( @$"Duplicate key '{key}':  '{section}'" ); }

            config[section][key] = value;
        }

        return config;
    }
    public static bool TryParse( ReadOnlySpan<char> span, IFormatProvider? provider, [NotNullWhen( true )] out IniConfig? result )
    {
        try
        {
            result = Parse( span, provider );
            return true;
        }
        catch ( Exception )
        {
            result = default;
            return false;
        }
    }


    public override string ToString() => ToString( default, CultureInfo.InvariantCulture );
    public string ToString( string? format, IFormatProvider? formatProvider )
    {
        Span<char> span = stackalloc char[Length + 10];

        if ( TryFormat( span, out int charsWritten, format, formatProvider ) )
        {
            ReadOnlySpan<char> result = span[..charsWritten];
            return result.ToString();
        }

        throw new InvalidOperationException( "Cannot convert to string" );
    }


#if NET6_0_OR_GREATER
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
#endif
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        Debug.Assert( destination.Length > Length );
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


    public void Add( string  section ) => Add( new Section( section ) );
    public void Add( Section value ) => this[value.Name] = value;
}
