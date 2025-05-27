namespace Jakar.Extensions;


[Serializable, DebuggerDisplay( nameof(DisplayName) )]
public sealed class Language : BaseClass, IComparisonOperators<Language>
{
    private readonly CultureInfo _culture;


    public static Equalizer<Language> Equalizer        => Equalizer<Language>.Default;
    public static Sorter<Language>    Sorter           => Sorter<Language>.Default;
    public        string              DisplayName      { get; init; }
    public        string              EnglishName      { get; init; }
    public        bool                IsNeutralCulture { get; init; }
    public        string              Name             { get; init; }
    public        string              ThreeLetterISO   { get; init; }
    public        string              TwoLetterISO     { get; init; }
    public        SupportedLanguage?  Version          { get; init; }


    public Language( CultureInfo culture, SupportedLanguage? version = null )
    {
        _culture         = culture;
        Name             = culture.Name;
        EnglishName      = culture.EnglishName;
        TwoLetterISO     = culture.TwoLetterISOLanguageName;
        ThreeLetterISO   = culture.ThreeLetterISOLanguageName;
        IsNeutralCulture = culture.IsNeutralCulture;
        Version          = version ??= culture.GetSupportedLanguage();
        DisplayName      = version?.GetName() ?? culture.DisplayName;
    }
    public Language( SupportedLanguage language ) : this( language.GetCultureInfo( CultureInfo.InvariantCulture ), language ) { }


    public static implicit operator Language( CultureInfo       value ) => new(value);
    public static implicit operator Language( SupportedLanguage value ) => new(value);
    public static implicit operator CultureInfo( Language       value ) => value._culture;


    public          CultureInfo GetCulture() => _culture;
    public override string      ToString()   => DisplayName;


    public int CompareTo( object? value ) => value switch
                                             {
                                                 null           => 1,
                                                 Language other => CompareTo( other ),
                                                 _              => throw new ExpectedValueTypeException( nameof(value), value, typeof(Language) )
                                             };
    public int CompareTo( Language? other )
    {
        if ( other is null ) { return -1; }

        int displayNameComparison = string.Compare( DisplayName, other.DisplayName, StringComparison.Ordinal );
        if ( displayNameComparison != 0 ) { return displayNameComparison; }

        int shortNameComparison = string.Compare( Name, other.Name, StringComparison.Ordinal );
        if ( shortNameComparison != 0 ) { return shortNameComparison; }

        return Nullable.Compare( Version, other.Version );
    }
    public bool Equals( Language? other )
    {
        if ( other is null ) { return false; }

        return DisplayName == other.DisplayName && Name == other.Name && Version == other.Version;
    }
    public override bool Equals( object? obj ) => obj is Language language && Equals( language );
    public override int  GetHashCode()         => HashCode.Combine( DisplayName, Name, Version );


    public static bool operator ==( Language? left, Language? right ) => Equalizer.Equals( left, right );
    public static bool operator !=( Language? left, Language? right ) => Equalizer.Equals( left, right );
    public static bool operator >( Language?  left, Language? right ) => Sorter.Compare( left, right ) > 0;
    public static bool operator >=( Language? left, Language? right ) => Sorter.Compare( left, right ) >= 0;
    public static bool operator <( Language?  left, Language? right ) => Sorter.Compare( left, right ) < 0;
    public static bool operator <=( Language? left, Language? right ) => Sorter.Compare( left, right ) <= 0;



    [Serializable]
    public class Collection : ObservableCollection<Language>
    {
        public Collection() : base() { }
        public Collection( IEnumerable<Language> items ) : base( items ) { }
    }



    [Serializable]
    public class Items : List<Language>
    {
        public Items() : base() { }
        public Items( int                   capacity ) : base( capacity ) { }
        public Items( IEnumerable<Language> items ) : base( items ) => Sort( Sorter );
    }



    #region Instances

    public static Language Arabic     { get; } = new(SupportedLanguage.Arabic);
    public static Language Chinese    { get; } = new(SupportedLanguage.Chinese);
    public static Language Czech      { get; } = new(SupportedLanguage.Czech);
    public static Language Dutch      { get; } = new(SupportedLanguage.Dutch);
    public static Language English    { get; } = new(SupportedLanguage.English);
    public static Language French     { get; } = new(SupportedLanguage.French);
    public static Language German     { get; } = new(SupportedLanguage.German);
    public static Language Japanese   { get; } = new(SupportedLanguage.Japanese);
    public static Language Korean     { get; } = new(SupportedLanguage.Korean);
    public static Language Polish     { get; } = new(SupportedLanguage.Polish);
    public static Language Portuguese { get; } = new(SupportedLanguage.Portuguese);
    public static Language Spanish    { get; } = new(SupportedLanguage.Spanish);
    public static Language Swedish    { get; } = new(SupportedLanguage.Swedish);
    public static Language Thai       { get; } = new(SupportedLanguage.Thai);

    #endregion



    #region Lists

    public static Items NeutralCultures => new(CultureInfo.GetCultures( CultureTypes.NeutralCultures ).Select( culture => new Language( culture ) ));

    public static Items SpecificCultures => new(CultureInfo.GetCultures( CultureTypes.SpecificCultures ).Select( culture => new Language( culture ) ));

    public static Items All => new(CultureInfo.GetCultures( CultureTypes.AllCultures ).Select( culture => new Language( culture ) ));

    public static Collection Supported { get; } =
        [
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
        ];

    #endregion
}
