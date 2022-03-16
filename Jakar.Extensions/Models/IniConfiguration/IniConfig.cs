namespace Jakar.Extensions.Models.IniConfiguration;


public partial class IniConfig : ConcurrentDictionary<string, IniConfig.Section>
{
    public IniConfig() : this(StringComparer.OrdinalIgnoreCase) { }
    public IniConfig( IEqualityComparer<string>                  comparer ) : base(comparer) { }
    public IniConfig( IDictionary<string, Section>               dictionary ) : this(dictionary, StringComparer.OrdinalIgnoreCase) { }
    public IniConfig( IDictionary<string, Section>               dictionary, IEqualityComparer<string> comparer ) : base(dictionary, comparer) { }
    public IniConfig( IEnumerable<KeyValuePair<string, Section>> collection ) : this(collection, StringComparer.OrdinalIgnoreCase) { }
    public IniConfig( IEnumerable<KeyValuePair<string, Section>> collection, IEqualityComparer<string> comparer ) : base(collection, comparer) { }


    public Task WriteToFile( LocalFile file ) => file.WriteToFileAsync(ToString());


    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach ( ( string? section, Section dictionary ) in this )
        {
            builder.Append("[ ");
            builder.Append(section);
            builder.Append(" ]");
            builder.AppendLine();

            int longest = dictionary.Keys.Max(item => item.Length);

            foreach ( ( string? key, string? value ) in dictionary )
            {
                builder.Append(key.PadRight(longest));
                builder.Append(" = ");
                builder.Append(value);
                builder.AppendLine();
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }


    public static async Task<IniConfig?> ReadFromFile( LocalFile file )
    {
        string content = await file.ReadFromFileAsync();
        return FromString(content);
    }

    public static IniConfig? FromString( string content )
    {
        if ( string.IsNullOrWhiteSpace(content) ) { return null; }

        var data = new IniConfig();

        string section = string.Empty;

        foreach ( string rawLine in content.Split('\n') )
        {
            string line = rawLine.Trim();

            // Ignore blank lines
            if ( string.IsNullOrWhiteSpace(line) ) { continue; }

            switch ( line[0] )
            {
                // Ignore comments
                case ';':
                case '#':
                case '/':
                    continue;

                // [Section:header]
                case '[' when line[^1] == ']':
                    section = line.Substring(1, line.Length - 2).Trim(); // remove the brackets and whitespace
                    if ( string.IsNullOrWhiteSpace(section) ) { throw new FormatException("section title cannot be empty."); }

                    data[section] = new Section();
                    continue;
            }

            // key = value OR "value"
            int separator = line.IndexOf('=');
            if ( separator < 0 ) { throw new FormatException($@"Line doesn't contain an equals sign. ""{line}"" "); }

            string key   = line[..separator].Trim();
            string value = line[( separator + 1 )..].Trim();

            // Remove quotes
            if ( value.Length > 1 && value[0] == '"' && value[^1] == '"' ) { value = value.Substring(1, value.Length - 2); }

            if ( data[section].ContainsKey(key) ) { throw new FormatException(@$"Duplicate key ""{key}"" in ""{section}"""); }

            data[section][key] = value;
        }

        return data;
    }


    /// <summary>
    /// Gets the <see cref="Section"/> with the <paramref name="sectionName"/>. If it doesn't exist, it is created, then returned.
    /// </summary>
    /// <param name="sectionName">Section Name</param>
    /// <returns><see cref="Section"/></returns>
    public new Section this[ string sectionName ]
    {
        get
        {
            if ( !ContainsKey(sectionName) ) { Add(sectionName); }

            return base[sectionName];
        }
        set => base[sectionName] = value;
    }


    public void Add( in string section )                => Add(section, new Section());
    public void Add( in string section, Section value ) => this[section] = value;
}
