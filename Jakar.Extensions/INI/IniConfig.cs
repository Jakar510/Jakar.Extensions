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
            int keys   = Keys.Sum( x => x.Length + OPEN.Length + CLOSE.Length );
            int values = Values.Sum( x => x.Length );
            int result = keys + values + Keys.Count;
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


    // public static IniConfig? From(  string content )
    // {
    //     if ( string.IsNullOrWhiteSpace(content) ) { return null; }
    //
    //     var data = new IniConfig();
    //
    //     var section = string.Empty;
    //
    //     foreach ( string rawLine  content.Split('\n') )
    //     {
    //         string line = rawLine.Trim();
    //
    //         // Ignore blank lines
    //         if ( string.IsNullOrWhiteSpace(line) ) { continue; }
    //
    //         switch ( line[0] )
    //         {
    //             // Ignore comments
    //             case ';':
    //             case '#':
    //             case '/':
    //                 continue;
    //
    //             // [Section:header]
    //             case '[' when line[^1] == ']':
    //                 section = line.Substring(1, line.Length - 2).Trim(); // remove the brackets and whitespace
    //                 if ( string.IsNullOrWhiteSpace(section) ) { throw new FormatException("section title cannot be empty."); }
    //
    //                 data[section] = new Section();
    //                 continue;
    //         }
    //
    //         // key = value OR "value"
    //         int separator = line.IndexOf('=');
    //         if ( separator < 0 ) { throw new FormatException($@"Line doesn't contain an equals sign. ""{line}"" "); }
    //
    //         string key   = line[..separator].Trim();
    //         string value = line[( separator + 1 )..].Trim();
    //
    //         // Remove quotes
    //         if ( value.Length > 1 && value[0] == '"' && value[^1] == '"' ) { value = value.Substring(1, value.Length - 2); }
    //
    //         if ( data[section].ContainsKey(key) ) { throw new FormatException(@$"Duplicate key ""{key}""  ""{section}"""); }
    //
    //         data[section][key] = value;
    //     }
    //
    //     return data;
    // }


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

        return base[sectionName] = new Section();
    }


    public IniConfig Refresh( string content ) => Refresh( content.AsSpan() );
    public IniConfig Refresh( in ReadOnlySpan<char> content )
    {
        // $"-- {nameof(IniConfig)}.{nameof(Refresh)}.{nameof(content)} --\n{content.ToString()}".WriteToConsole();
        if ( content.IsEmpty ) { return this; }


        string section = string.Empty;

        foreach ( ReadOnlySpan<char> rawLine in content.SplitOn() )
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

                    section       = sectionSpan.ToString();
                    this[section] = new Section();
                    continue;
            }

            // key = value OR "value"
            int separator = line.IndexOf( '=' );
            if ( separator < 0 ) { continue; }


            ReadOnlySpan<char> keySpan = line[..separator]
               .Trim();

            ReadOnlySpan<char> valueSpan = line[(separator + 1)..]
               .Trim();

            // Remove quotes
            if ( valueSpan.Length > 1 && valueSpan[0] == '"' && valueSpan[^1] == '"' ) { valueSpan = valueSpan.Slice( 1, valueSpan.Length - 2 ); }

            string key   = keySpan.ToString();
            string value = valueSpan.ToString();

            Debug.Assert( !string.IsNullOrEmpty( section ) );

            if ( this[section]
               .ContainsKey( key ) ) { throw new FormatException( @$"Duplicate key '{key}':  '{section}'" ); }

            this[section][key] = value;
        }

        return this;
    }


    public static IniConfig Parse( string s ) => Parse( s,                                     CultureInfo.InvariantCulture );
    public static IniConfig Parse( string s, IFormatProvider? provider ) => Parse( s.AsSpan(), provider );
    public static bool TryParse( string?  s, IFormatProvider? provider, [NotNullWhen( true )] out IniConfig? result ) => TryParse( s.AsSpan(), provider, out result );

    public static IniConfig Parse( ReadOnlySpan<char> span, IFormatProvider? provider )
    {
        var ini = new IniConfig();
        return ini.Refresh( span );
    }
    public static bool TryParse( ReadOnlySpan<char> span, IFormatProvider? provider, [NotNullWhen( true )] out IniConfig? result )
    {
        result = null;
        return false;
    }

    public override string ToString() => ToString( default, CultureInfo.InvariantCulture );
    public string ToString( string? format, IFormatProvider? formatProvider )
    {
        Span<char> span = stackalloc char[Length + 1];

        if ( TryFormat( span, out int charsWritten, format, formatProvider ) )
        {
            return span[..charsWritten]
               .ToString();
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
            foreach ( char t in OPEN ) { destination[charsWritten++] = t; }

            foreach ( char t in key ) { destination[charsWritten++] = t; }

            foreach ( char t in CLOSE ) { destination[charsWritten++] = t; }

            destination[charsWritten++] = '\n';

            Span<char> span = destination[charsWritten..];
            if ( section.TryFormat( span, out int sectionCharsWritten, format, provider ) ) { charsWritten += sectionCharsWritten; }

            destination[charsWritten++] = '\n';
        }

        return true;
    }


    public ValueTask WriteToFile( LocalFile file ) => file.WriteAsync( ToString() );


    public void Add( string                        section ) => Add( section, new Section() );
    public void Add( KeyValuePair<string, Section> pair ) => Add( pair.Key,   pair.Value );
    public void Add( string                        section, Section value ) => this[section] = value;
}
