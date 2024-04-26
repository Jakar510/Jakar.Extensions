using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;



namespace Jakar.Extensions;


public sealed partial class IniConfig : ConcurrentDictionary<string, IniConfig.Section>
                                    #if NET7_0_OR_GREATER
                                        ,
                                        ISpanParsable<IniConfig>
                                    #endif
                                    #if NET6_0_OR_GREATER
                                        ,
                                        ISpanFormattable
#endif
{
    private readonly IEqualityComparer<string> _comparer;
    public new Section this[ string sectionName ] { get => GetOrAdd( sectionName ); set => base[sectionName] = value; }


    public int Length
    {
        get
        {
            int values = Values.Sum( x => x.Length );
            int result = values + Keys.Count;
            return result;
        }
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
        string content = file.Read().AsString();
        return Parse( content, provider );
    }
    public static async ValueTask<IniConfig> ReadFromFileAsync( LocalFile file, IFormatProvider? provider = default, CancellationToken token = default )
    {
        string content = await file.ReadAsync().AsString( token );
        return Parse( content, provider );
    }

    public static IniConfig Parse( scoped in ReadOnlySpan<char> span ) => Parse( span, CultureInfo.InvariantCulture );
    public static IniConfig Parse( scoped in ReadOnlySpan<char> span, IFormatProvider? provider )
    {
        IniConfig config = new();

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
                    ReadOnlySpan<char> sectionSpan = line[1..^1].Trim(); // remove the brackets and whitespace

                    if ( sectionSpan.IsNullOrWhiteSpace() ) { throw new FormatException( "section title cannot be empty or whitespace." ); }

                    section = sectionSpan.ToString();
                    config.Add( new Section( section ) );
                    continue;
            }


            if ( line.Trim().IsNullOrWhiteSpace() ) { continue; }


            int separator = line.IndexOf( '=' ); // key = value OR "value"
            if ( separator < 0 ) { continue; }


            string key = line[..separator].Trim().ToString();

            string value = line[(separator + 1)..]
                          .Trim()
                          .Trim( '"' ) // Remove quotes;
                          .ToString();

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
            result = default;
            return false;
        }
    }


#if NET7_0_OR_GREATER
    static IniConfig IParsable<IniConfig>.    Parse( string?               s,    IFormatProvider? provider )                                              => Parse( s, provider );
    static bool IParsable<IniConfig>.         TryParse( string?            s,    IFormatProvider? provider, [NotNullWhen( true )] out IniConfig? result ) => TryParse( s, provider, out result );
    static IniConfig ISpanParsable<IniConfig>.Parse( ReadOnlySpan<char>    span, IFormatProvider? provider )                                              => Parse( span, provider );
    static bool ISpanParsable<IniConfig>.     TryParse( ReadOnlySpan<char> span, IFormatProvider? provider, [NotNullWhen( true )] out IniConfig? result ) => TryParse( span, provider, out result );
#endif


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


    public IniConfig Add( scoped in ReadOnlySpan<Section> values )
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


    public override string ToString() => ToString( default, CultureInfo.InvariantCulture );
    public string ToString( string? format, IFormatProvider? formatProvider )
    {
        Span<char> span = stackalloc char[Length + 10];
        if ( TryFormat( span, out int charsWritten, format, formatProvider ) is false ) { throw new InvalidOperationException( "Cannot convert to string" ); }

        ReadOnlySpan<char> result = span[..charsWritten];
        return result.ToString();
    }


#if NET6_0_OR_GREATER
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
#endif
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
