namespace Jakar.Extensions.Languages;


[Serializable]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class Language : IDataBaseID, IComparable<Language>, IEquatable<Language>, ICloneable
{
    public              string            DisplayName { get; init; }
    public              string            ShortName   { get; init; }
    public              SupportedLanguage Version     { get; init; }
    public virtual      long              ID          { get; init; }
    [JsonIgnore] public CultureInfo       Info        => new(ShortName);


    public Language() : this(string.Empty, string.Empty, SupportedLanguage.English) { }
    public Language( string name, string shortName, SupportedLanguage language ) : this(name, shortName, language, language.ToLong()) { }

    public Language( string name, string shortName, SupportedLanguage language, long id )
    {
        DisplayName = name;
        ShortName   = shortName;
        Version     = language;
        ID          = id;
    }

    public object Clone() => new Language(DisplayName, ShortName, Version, ID);


#region Implementation of IComparable<in Language>

    /// <summary>
    /// Returns 0 if equal, -1 if not.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo( Language other ) => Equals(other)
                                                  ? 0
                                                  : -1;

#endregion


#region Implementation of IEquatable<Language>

    public bool Equals( Language other ) => DisplayName == other.DisplayName &&
                                            ShortName == other.ShortName &&
                                            Version == other.Version &&
                                            ID == other.ID;


    public override bool Equals( object obj )
    {
        if ( obj is Language language ) { return Equals(language); }

        return false;
    }


    public override int GetHashCode() => HashCode.Combine(DisplayName, ShortName, (int)Version, ID);

#endregion


    public static Language Arabic     => new("عربى - Arabic", "ar", SupportedLanguage.Arabic);
    public static Language Chinese    => new("中文 - Chinese (simplified)", "zh-Hans", SupportedLanguage.Chinese);
    public static Language Czech      => new("čeština - Czech", "cs", SupportedLanguage.Czech);
    public static Language Dutch      => new("Nederlands - Dutch", "nl", SupportedLanguage.Dutch);
    public static Language English    => new("English", "en", SupportedLanguage.English);
    public static Language French     => new("Français - French", "fr", SupportedLanguage.French);
    public static Language German     => new("Deutsche - German", "de", SupportedLanguage.German);
    public static Language Japanese   => new("日本語 - Japanese", "ja", SupportedLanguage.Japanese);
    public static Language Korean     => new("한국어 - Korean", "ko", SupportedLanguage.Korean);
    public static Language Polish     => new("Polskie - Polish", "pl", SupportedLanguage.Polish);
    public static Language Portuguese => new("Português - Portuguese", "pt", SupportedLanguage.Portuguese);
    public static Language Spanish    => new("Español - Spanish", "es", SupportedLanguage.Spanish);
    public static Language Swedish    => new("svenska - Swedish", "sv", SupportedLanguage.Swedish);
    public static Language Thai       => new("ไทย - Thai", "th", SupportedLanguage.Thai);


    public static Items All => Items.Default();



    [Serializable]
    public class Collection : ObservableCollection<Language>
    {
        public Collection() : base() { }
        public Collection( IEnumerable<Language> items ) : base(items) { }


        public static Collection Default() => new(Language.Items.Default());
    }



    [Serializable]
    public class Items : List<Language>
    {
        public Items() : base() { }
        public Items( int                   capacity ) : base(capacity) { }
        public Items( IEnumerable<Language> items ) : base(items) { }


        public static Items Default() => new(14)
                                         {
                                             Arabic,
                                             Chinese,
                                             Czech,
                                             Dutch,
                                             English,
                                             French,
                                             German,
                                             Japanese,
                                             Korean,
                                             Polish,
                                             Portuguese,
                                             Spanish,
                                             Swedish,
                                             Thai
                                         };
    }
}
