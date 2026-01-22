namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible")]
public static class AppPreference
{
    private static readonly Lock __lock = new();


    public static IAppPreferences Source
    {
        get
        {
            lock ( __lock ) { return field ??= File.Create(); }
        }
        set
        {
            lock ( __lock ) { field = value; }
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



    extension( string sharedName )
    {
        public string GetPreference( string key, string defaultValue = EMPTY, params ReadOnlySpan<string> alternateKeys ) => Source.Get(key, sharedName, defaultValue, alternateKeys);
        public TValue GetPreference<TValue>( string key, TValue defaultValue, params ReadOnlySpan<string> alternateKeys )
            where TValue : IParsable<TValue>, IFormattable
        {
            string value = Source.Get(key, sharedName, defaultValue.ToString(null, CultureInfo.CurrentCulture), alternateKeys);

            return TValue.TryParse(value, CultureInfo.CurrentCulture, out TValue? result)
                       ? result
                       : defaultValue;
        }
        public bool GetPreference( string key, bool defaultValue, params ReadOnlySpan<string> alternateKeys )
        {
            string result = sharedName.GetPreference(key, defaultValue.GetString(), alternateKeys);
            return result.GetBool() is true;
        }
        public bool? GetPreference( string key, bool? defaultValue, params ReadOnlySpan<string> alternateKeys )
        {
            string result = sharedName.GetPreference(key, defaultValue.GetString(), alternateKeys);
            return result.GetBool();
        }
        public Uri GetPreference( string key, in Uri defaultValue, params ReadOnlySpan<string> alternateKeys )
        {
            string value = sharedName.GetPreference(key, defaultValue.OriginalString, alternateKeys);

            try
            {
                return Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out Uri? host)
                           ? host
                           : defaultValue;
            }
            catch ( Exception e )
            {
                Debug.WriteLine(e);
                sharedName.RemovePreference(key, alternateKeys);
                return defaultValue;
            }
        }
        public void SetPreference( string key, bool?   value ) => sharedName.SetPreference(key, value.GetString());
        public void SetPreference( string key, bool    value ) => sharedName.SetPreference(key, value.GetString());
        public void SetPreference( string key, Uri     value ) => sharedName.SetPreference(key, value.ToString());
        public void SetPreference( string key, string? value ) => Source.Set(key, value ?? EMPTY, sharedName);
        public void SetPreference<TValue>( string key, TValue value )
            where TValue : IParsable<TValue>, IFormattable => Source.Set(key, value, sharedName);
        public void RemovePreference( string key, params ReadOnlySpan<string> alternateKeys ) => Source.Remove(key, sharedName, alternateKeys);
    }



    public sealed class File( LocalFile file ) : IAppPreferences // TODO: Add watcher to update if file changes
    {
        public const     string    DEFAULT_FILE_NAME = "Settings.ini";
        private readonly IniConfig __config          = IniConfig.ReadFromFile(file);
        private readonly LocalFile __file            = file;


        public static File Create()                           => Create(LocalDirectory.CurrentDirectory);
        public static File Create( LocalDirectory directory ) => Create(directory.Join(DEFAULT_FILE_NAME));
        public static File Create( LocalFile      file )      => new(file);
        public async ValueTask DisposeAsync() => await SaveAsync()
                                                    .ConfigureAwait(false);
        private async Task SaveAsync() => await __config.WriteToFile(__file)
                                                        .ConfigureAwait(false);


        public bool ContainsKey( string key, string sharedName ) => __config[sharedName]
           .ContainsKey(key);
        public void Remove( string key, string sharedName, params ReadOnlySpan<string> alternateKeys )
        {
            __config[sharedName]
               .TryRemove(key, out _);

            foreach ( string alternateKey in alternateKeys )
            {
                if ( !string.IsNullOrWhiteSpace(alternateKey) )
                {
                    __config[sharedName]
                       .TryRemove(alternateKey, out _);
                }
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


        public TValue Get<TValue>( string key, TValue defaultValue, string sharedName, params ReadOnlySpan<string> alternateKeys )
            where TValue : IParsable<TValue>, IFormattable
        {
            string value = Get(key, sharedName, defaultValue.ToString(null, CultureInfo.CurrentCulture), alternateKeys);

            return TValue.TryParse(value, CultureInfo.CurrentCulture, out TValue? result)
                       ? result
                       : defaultValue;
        }
        public Uri Get( string key, Uri defaultValue, string sharedName, params ReadOnlySpan<string> alternateKeys ) => Uri.TryCreate(Get(key, sharedName, EMPTY, alternateKeys), UriKind.RelativeOrAbsolute, out Uri? result)
                                                                                                                            ? result
                                                                                                                            : defaultValue;
        public bool Get( string key, bool defaultValue, string sharedName, params ReadOnlySpan<string> alternateKeys )
        {
            string result = Get(key, sharedName, EMPTY, alternateKeys);
            return result.GetBool() ?? defaultValue;
        }
        public bool? Get( string key, bool? defaultValue, string sharedName, params ReadOnlySpan<string> alternateKeys )
        {
            string result = Get(key, sharedName, EMPTY, alternateKeys);
            return result.GetBool() ?? defaultValue;
        }
        public string Get( string key, string sharedName, string defaultValue = EMPTY, params ReadOnlySpan<string> alternateKeys )
        {
            IniConfig.Section section = __config[sharedName];
            string?           result  = TryGet(sharedName, alternateKeys) ?? section[key];
            return result ?? defaultValue;
        }
        private string? TryGet( string sharedName, params ReadOnlySpan<string> alternateKeys )
        {
            IniConfig.Section section = __config[sharedName];

            string? result = null;

            foreach ( string alternateKey in alternateKeys )
            {
                if ( !string.IsNullOrWhiteSpace(alternateKey) && section.TryGetValue(alternateKey, out string? value) ) { result = value; }
            }

            return result;
        }
    }
}
