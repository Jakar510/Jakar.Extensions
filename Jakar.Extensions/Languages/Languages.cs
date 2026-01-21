using System.Linq;



namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "StringLiteralTypo")]
public static class Languages
{
    public static readonly ImmutableArray<SupportedLanguage> All = Enum.GetValues<SupportedLanguage>()
                                                                       .AsImmutableArray();
    public static readonly FrozenDictionary<string, SupportedLanguage> FromStrings = CreateFromStrings();
    public static readonly FrozenDictionary<SupportedLanguage, Language> Cache = Enum.GetValues<SupportedLanguage>()
                                                                                     .ToFrozenDictionary(static x => x, static x => new Language(CultureInfo.GetCultureInfo(x.GetShortName())));
    public static readonly FrozenDictionary<SupportedLanguage, CultureInfo> CultureCache = All.ToFrozenDictionary(static x => x, static x => CultureInfo.GetCultureInfo(x.GetShortName()));
    public static readonly FrozenDictionary<CultureInfo, SupportedLanguage> Cultures = Enum.GetValues<SupportedLanguage>()
                                                                                           .ToFrozenDictionary(static x => CultureInfo.GetCultureInfo(x.GetShortName()), static x => x);


    static FrozenDictionary<string, SupportedLanguage> CreateFromStrings()
    {
        Dictionary<string, SupportedLanguage> dictionary = new(StringComparer.OrdinalIgnoreCase);

        foreach ( SupportedLanguage language in Enum.GetValues<SupportedLanguage>() )
        {
            string value = language.ToStringFast();
            string two   = language.GetShortName();
            string name  = language.GetName();

            dictionary[value] = language;
            dictionary[two]   = language;
            dictionary[name]  = language;

            CultureInfo culture = CultureInfo.GetCultureInfo(language.GetShortName());
            dictionary[culture.Name]                           = language;
            dictionary[culture.DisplayName]                    = language;
            dictionary[culture.TwoLetterISOLanguageName]       = language;
            dictionary[culture.ThreeLetterISOLanguageName]     = language;
            dictionary[culture.ThreeLetterWindowsLanguageName] = language;
            dictionary[culture.ToString()]                     = language;
        }

        return dictionary.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }



    extension( SupportedLanguage language )
    {
        public CultureInfo AsCultureInfo()                           => language.AsCultureInfo(CultureInfo.InvariantCulture);
        public CultureInfo AsCultureInfo( CultureInfo defaultValue ) => CultureCache.GetValueOrDefault(language, defaultValue);


        public string GetName() => language switch
                                   {
                                       SupportedLanguage.English     => "English",
                                       SupportedLanguage.Spanish     => "Español - Spanish",
                                       SupportedLanguage.French      => "Français - French",
                                       SupportedLanguage.Swedish     => "svenska - Swedish",
                                       SupportedLanguage.German      => "Deutsche - German",
                                       SupportedLanguage.Chinese     => "中文 - Chinese (simplified)",
                                       SupportedLanguage.Polish      => "Polskie - Polish",
                                       SupportedLanguage.Thai        => "ไทย - Thai",
                                       SupportedLanguage.Japanese    => "日本語 - Japanese",
                                       SupportedLanguage.Czech       => "čeština - Czech",
                                       SupportedLanguage.Portuguese  => "Português - Portuguese",
                                       SupportedLanguage.Dutch       => "Nederlands - Dutch",
                                       SupportedLanguage.Korean      => "한국어 - Korean",
                                       SupportedLanguage.Arabic      => "عربى - Arabic",
                                       SupportedLanguage.Unspecified => CultureInfo.InvariantCulture.ToString(),
                                       _                             => EMPTY
                                   };


        public string GetShortName() => language switch
                                        {
                                            SupportedLanguage.English     => "en",
                                            SupportedLanguage.Spanish     => "es",
                                            SupportedLanguage.French      => "fr",
                                            SupportedLanguage.Swedish     => "sv",
                                            SupportedLanguage.German      => "de",
                                            SupportedLanguage.Chinese     => "zh-Hans",
                                            SupportedLanguage.Polish      => "pl",
                                            SupportedLanguage.Thai        => "th",
                                            SupportedLanguage.Japanese    => "ja",
                                            SupportedLanguage.Czech       => "cs",
                                            SupportedLanguage.Portuguese  => "pt",
                                            SupportedLanguage.Dutch       => "nl",
                                            SupportedLanguage.Korean      => "ko",
                                            SupportedLanguage.Arabic      => "ar",
                                            SupportedLanguage.Unspecified => CultureInfo.InvariantCulture.TwoLetterISOLanguageName,
                                            _                             => throw new OutOfRangeException(language)
                                        };


        public string ToStringFast() => language switch
                                        {
                                            SupportedLanguage.Unspecified => nameof(SupportedLanguage.Unspecified),
                                            SupportedLanguage.English     => nameof(SupportedLanguage.English),
                                            SupportedLanguage.Spanish     => nameof(SupportedLanguage.Spanish),
                                            SupportedLanguage.French      => nameof(SupportedLanguage.French),
                                            SupportedLanguage.Swedish     => nameof(SupportedLanguage.Swedish),
                                            SupportedLanguage.German      => nameof(SupportedLanguage.German),
                                            SupportedLanguage.Chinese     => nameof(SupportedLanguage.Chinese),
                                            SupportedLanguage.Polish      => nameof(SupportedLanguage.Polish),
                                            SupportedLanguage.Thai        => nameof(SupportedLanguage.Thai),
                                            SupportedLanguage.Japanese    => nameof(SupportedLanguage.Japanese),
                                            SupportedLanguage.Czech       => nameof(SupportedLanguage.Czech),
                                            SupportedLanguage.Portuguese  => nameof(SupportedLanguage.Portuguese),
                                            SupportedLanguage.Dutch       => nameof(SupportedLanguage.Dutch),
                                            SupportedLanguage.Korean      => nameof(SupportedLanguage.Korean),
                                            SupportedLanguage.Arabic      => nameof(SupportedLanguage.Arabic),
                                            _                             => throw new OutOfRangeException(language)
                                        };
    }



    public static string? ToStringFast( this SupportedLanguage? language ) => language?.ToStringFast();


    public static SupportedLanguage GetSupportedLanguage( this CultureInfo culture )
    {
        if ( CultureInfo.InvariantCulture.Equals(culture) ) { return SupportedLanguage.Unspecified; }

        // ReSharper disable DuplicatedSequentialIfBodies
        if ( FromStrings.TryGetValue(culture.ThreeLetterISOLanguageName, out var language) ) { return language; }

        if ( FromStrings.TryGetValue(culture.ThreeLetterWindowsLanguageName, out language) ) { return language; }

        if ( FromStrings.TryGetValue(culture.TwoLetterISOLanguageName, out language) ) { return language; }

        if ( FromStrings.TryGetValue(culture.DisplayName, out language) ) { return language; }

        if ( FromStrings.TryGetValue(culture.Name, out language) ) { return language; }

        // ReSharper restore DuplicatedSequentialIfBodies
        return FromStrings.GetValueOrDefault(culture.ToString(), SupportedLanguage.Unspecified);
    }
    public static SupportedLanguage GetSupportedLanguage( this IFormatProvider culture )
    {
        if ( culture is CultureInfo info ) { return info.GetSupportedLanguage(); }

        string? name = culture.ToString();

        return string.IsNullOrEmpty(name)
                   ? SupportedLanguage.Unspecified
                   : FromStrings.GetValueOrDefault(name, SupportedLanguage.Unspecified);
    }
}
