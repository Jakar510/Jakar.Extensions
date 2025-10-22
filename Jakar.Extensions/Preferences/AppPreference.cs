namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible")]
public static class AppPreference
{
    private static readonly Lock             __lock = new();
    private static          IAppPreferences? __source;


    public static IAppPreferences Source
    {
        get
        {
            lock ( __lock ) { return __source ??= File.Create(); }
        }
        set
        {
            lock ( __lock ) { __source = value; }
        }
    }


    public static bool? GetBool( this string value ) => value switch
                                                        {
                                                            null  => null,
                                                            NULL  => null,
                                                            TRUE  => true,
                                                            FALSE => false,
                                                            _     => string.Equals(TRUE, value, StringComparison.InvariantCultureIgnoreCase)
                                                        };


    public static string GetString( this bool value ) => value
                                                             ? TRUE
                                                             : FALSE;


    public static string GetString( this bool? value ) => value.HasValue
                                                              ? value.Value.GetString()
                                                              : NULL;


    public static string GetPreference( this string sharedName, string key, string defaultValue = EMPTY, string? alternateKey = null ) => Source.Get(key, sharedName, defaultValue, alternateKey);
    public static TValue GetPreference<TValue>( this string sharedName, string key, TValue defaultValue, in string? alternateKey = null )
        where TValue : IParsable<TValue>, IFormattable
    {
        string value = Source.Get(key, sharedName, defaultValue.ToString(null, CultureInfo.CurrentCulture), alternateKey);

        return TValue.TryParse(value, CultureInfo.CurrentCulture, out TValue? result)
                   ? result
                   : defaultValue;
    }


    public static bool GetPreference( this string sharedName, string key, bool defaultValue, string? alternateKey = null )
    {
        string result = sharedName.GetPreference(key, defaultValue.GetString(), alternateKey);
        return result.GetBool() is true;
    }
    public static bool? GetPreference( this string sharedName, string key, bool? defaultValue, string? alternateKey = null )
    {
        string result = sharedName.GetPreference(key, defaultValue.GetString(), alternateKey);
        return result.GetBool();
    }
    public static Uri GetPreference( this string sharedName, string key, in Uri defaultValue, string? alternateKey = null )
    {
        string value = sharedName.GetPreference(key, defaultValue.OriginalString, alternateKey);

        try
        {
            return Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out Uri? host)
                       ? host
                       : defaultValue;
        }
        catch ( Exception e )
        {
            Debug.WriteLine(e);
            sharedName.RemovePreference(key, alternateKey);
            return defaultValue;
        }
    }


    public static void SetPreference( this string sharedName, string key, bool?   value ) => sharedName.SetPreference(key, value.GetString());
    public static void SetPreference( this string sharedName, string key, bool    value ) => sharedName.SetPreference(key, value.GetString());
    public static void SetPreference( this string sharedName, string key, Uri     value ) => sharedName.SetPreference(key, value.ToString());
    public static void SetPreference( this string sharedName, string key, string? value ) => Source.Set(key, value ?? EMPTY, sharedName);


    public static void SetPreference<TValue>( this string sharedName, string key, TValue value )
        where TValue : IParsable<TValue>, IFormattable => Source.Set(key, value, sharedName);


    public static void RemovePreference( this string sharedName, string key, string? alternateKey = null ) => Source.Remove(key, sharedName, alternateKey);



    public sealed class File( LocalFile file ) : IAppPreferences // TODO: Add watcher to update if file changes
    {
        public const     string    DEFAULT_FILE_NAME = "Settings.ini";
        private readonly IniConfig __config          = IniConfig.ReadFromFile(file);
        private readonly LocalFile __file            = file;


        public static File      Create()                           => Create(LocalDirectory.CurrentDirectory);
        public static File      Create( LocalDirectory directory ) => Create(directory.Join(DEFAULT_FILE_NAME));
        public static File      Create( LocalFile      file )      => new(file);
        public async  ValueTask DisposeAsync()                     => await SaveAsync();
        private async Task      SaveAsync()                        => await __config.WriteToFile(__file);


        public bool ContainsKey( string key, string sharedName ) => __config[sharedName]
           .ContainsKey(key);
        public void Remove( string key, string sharedName, string? alternateKey = null )
        {
            __config[sharedName]
               .TryRemove(key, out _);

            if ( alternateKey is not null )
            {
                __config[sharedName]
                   .TryRemove(alternateKey, out _);
            }

            _ = SaveAsync();
        }
        public void Clear()
        {
            __config.Clear();
            _ = SaveAsync();
        }
        public void Clear( string sharedName )
        {
            __config[sharedName]
               .Clear();

            _ = SaveAsync();
        }


        public void Set( string key, string value, string sharedName )
        {
            __config[sharedName][key] = value;
            _                         = SaveAsync();
        }
        public void Set( string key, Uri   value, string sharedName ) => Set(key, value.ToString(),  sharedName);
        public void Set( string key, bool  value, string sharedName ) => Set(key, value.GetString(), sharedName);
        public void Set( string key, bool? value, string sharedName ) => Set(key, value.GetString(), sharedName);
        public void Set<TValue>( string key, TValue value, string sharedName )
            where TValue : IParsable<TValue>, IFormattable
        {
            __config[sharedName][key] = value.ToString(null, CultureInfo.CurrentCulture);
            _                         = SaveAsync();
        }


        public TValue Get<TValue>( string key, TValue defaultValue, string sharedName, string? alternateKey = null )
            where TValue : IParsable<TValue>, IFormattable
        {
            string value = Get(key, sharedName, defaultValue.ToString(null, CultureInfo.CurrentCulture), alternateKey);

            return TValue.TryParse(value, CultureInfo.CurrentCulture, out TValue? result)
                       ? result
                       : defaultValue;
        }
        public Uri Get( string key, Uri defaultValue, string sharedName, string? alternateKey = null ) => Uri.TryCreate(Get(key, sharedName), UriKind.RelativeOrAbsolute, out Uri? result)
                                                                                                              ? result
                                                                                                              : defaultValue;
        public bool Get( string key, bool defaultValue, string sharedName, string? alternateKey = null )
        {
            string result = Get(key, sharedName, EMPTY, alternateKey);
            return result.GetBool() ?? defaultValue;
        }
        public bool? Get( string key, bool? defaultValue, string sharedName, string? alternateKey = null )
        {
            string result = Get(key, sharedName, EMPTY, alternateKey);
            return result.GetBool() ?? defaultValue;
        }
        public string Get( string key, string sharedName, string defaultValue = EMPTY, string? alternateKey = null )
        {
            IniConfig.Section section = __config[sharedName];

            string? result = alternateKey is not null && section.TryGetValue(alternateKey, out string? value)
                                 ? value
                                 : section[key];

            return result ?? defaultValue;
        }
    }
}
