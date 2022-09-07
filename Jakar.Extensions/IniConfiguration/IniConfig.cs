#nullable enable
namespace Jakar.Extensions;


public partial class IniConfig : ConcurrentDictionary<string, IniConfig.Section>
{
    public new Section this[ string sectionName ]
    {
        get => GetOrAdd(sectionName);
        set => base[sectionName] = value;
    }
    public IniConfig() : this(StringComparer.OrdinalIgnoreCase) { }
    public IniConfig( IEqualityComparer<string>                  comparer ) : base(comparer) { }
    public IniConfig( IDictionary<string, Section>               dictionary ) : this(dictionary, StringComparer.OrdinalIgnoreCase) { }
    public IniConfig( IDictionary<string, Section>               dictionary, IEqualityComparer<string> comparer ) : base(dictionary, comparer) { }
    public IniConfig( IEnumerable<KeyValuePair<string, Section>> collection ) : this(collection, StringComparer.OrdinalIgnoreCase) { }
    public IniConfig( IEnumerable<KeyValuePair<string, Section>> collection, IEqualityComparer<string> comparer ) : base(collection, comparer) { }


    public Task WriteToFile( LocalFile file ) => file.WriteAsync(ToString());


    [SuppressMessage("ReSharper", "UseDeconstruction", Justification = "Support NetFramework")]
    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach ( KeyValuePair<string, Section> pair in this )
        {
            builder.Append("[ ");
            builder.Append(pair.Key);
            builder.Append(" ]");
            builder.AppendLine();

            Section dictionary = pair.Value;
            int     longest    = dictionary.Keys.Max(item => item.Length);

            foreach ( KeyValuePair<string, string> setting in dictionary )
            {
                builder.Append(setting.Key.PadRight(longest));
                builder.Append(" = ");
                builder.Append(setting.Value);
                builder.AppendLine();
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }


    public static Task<IniConfig?> ReadFromFileAsync( LocalFile file ) => ReadFromFileAsync<IniConfig>(file);
    public static IniConfig? ReadFromFile( LocalFile            file ) => ReadFromFile<IniConfig>(file);


    public static T? ReadFromFile<T>( LocalFile file ) where T : IniConfig, new()
    {
        ReadOnlySpan<char> content = file.Read().AsSpan();
        return From<T>(content);
    }
    public static async Task<T?> ReadFromFileAsync<T>( LocalFile file ) where T : IniConfig, new()
    {
        string content = await file.ReadAsync().AsString();
        return From<T>(content);
    }


    // public static IniConfig? From( in string content )
    // {
    //     if ( string.IsNullOrWhiteSpace(content) ) { return null; }
    //
    //     var data = new IniConfig();
    //
    //     var section = string.Empty;
    //
    //     foreach ( string rawLine in content.Split('\n') )
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
    //         if ( data[section].ContainsKey(key) ) { throw new FormatException(@$"Duplicate key ""{key}"" in ""{section}"""); }
    //
    //         data[section][key] = value;
    //     }
    //
    //     return data;
    // }

    public static T? From<T>( in string content ) where T : IniConfig, new() => From<T>(content.AsSpan());

    public static T? From<T>( in ReadOnlySpan<char> content ) where T : IniConfig, new()
    {
        if ( content.IsEmpty ) { return default; }

        var data = new T();

        var section = string.Empty;

        foreach ( LineSplitEntry<char> rawLine in content.SplitOn('\n') )
        {
            ReadOnlySpan<char> line = rawLine.Value.Trim();

            // Ignore blank lines
            if ( line.IsNullOrWhiteSpace() ) { continue; }

            switch ( line[0] )
            {
                // Ignore comments
                case ';':
                case '#':
                case '/':
                    continue;

                // [Section:header]
                case '[' when line[^1] == ']':
                    ReadOnlySpan<char> sectionSpan = line.Slice(1, line.Length - 2).Trim(); // remove the brackets and whitespace
                    if ( sectionSpan.IsNullOrWhiteSpace() ) { throw new FormatException("section title cannot be empty or whitespace."); }

                    section       = sectionSpan.ToString();
                    data[section] = new Section();
                    continue;
            }

            // key = value OR "value"
            int separator = line.IndexOf('=');

            if ( separator < 0 ) { throw new FormatException($@"Line doesn't contain an equals sign. ""{line.ToString()}"" "); }


            ReadOnlySpan<char> keySpan   = line[..separator].Trim();
            ReadOnlySpan<char> valueSpan = line[( separator + 1 )..].Trim();

            // Remove quotes
            if ( valueSpan.Length > 1 && valueSpan[0] == '"' && valueSpan[^1] == '"' ) { valueSpan = valueSpan.Slice(1, valueSpan.Length - 2); }

            var key   = keySpan.ToString();
            var value = valueSpan.ToString();
            if ( data[section].ContainsKey(key) ) { throw new FormatException(@$"Duplicate key ""{key}"" in ""{section}"""); }

            data[section][key] = value;
        }

        return data;
    }


    /// <summary>
    ///     Gets the
    ///     <see cref = "Section" />
    ///     with the
    ///     <paramref name = "sectionName" />
    ///     . If it doesn't exist, it is created, then returned.
    /// </summary>
    /// <param name = "sectionName" > Section Name </param>
    /// <returns>
    ///     <see cref = "Section" />
    /// </returns>
    public Section GetOrAdd( in string sectionName )
    {
        if ( !ContainsKey(sectionName) ) { Add(sectionName); }

        return base[sectionName];
    }

    /// <summary>
    ///     Gets the
    ///     <see cref = "Section" />
    ///     with the
    ///     <paramref name = "sectionName" />
    ///     , using the
    ///     <paramref name = "comparison" />
    ///     rules.
    /// </summary>
    /// <param name = "sectionName" > </param>
    /// <param name = "comparison" > </param>
    /// <returns> </returns>
    /// <exception cref = "KeyNotFoundException" > </exception>
    private Section Get( in string sectionName, in StringComparison comparison )
    {
        foreach ( string key in Keys )
        {
            if ( string.Compare(key, sectionName, comparison) == 0 ) { return base[key]; }
        }

        throw new KeyNotFoundException(sectionName);
    }

    public void Add( in string                     section ) => Add(section, new Section());
    public void Add( KeyValuePair<string, Section> pair ) => Add(pair.Key,   pair.Value);
    public void Add( in string                     section, Section value ) => this[section] = value;
}
