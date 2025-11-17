namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "StringLiteralTypo")]
public static class Languages
{
    public static readonly FrozenSet<SupportedLanguage> All = Enum.GetValues<SupportedLanguage>()
                                                                  .ToFrozenSet();
    public static readonly FrozenDictionary<string, SupportedLanguage> Values            = All.ToFrozenDictionary(ToStringFast, SelectSelf);
    public static readonly FrozenDictionary<SupportedLanguage, string> ReverseShortNames = All.ToFrozenDictionary(SelectSelf,   GetShortName);
    public static readonly FrozenDictionary<string, SupportedLanguage> ShortNames        = All.ToFrozenDictionary(GetShortName, SelectSelf);
    public static readonly FrozenDictionary<SupportedLanguage, string> ReverseNames      = All.ToFrozenDictionary(SelectSelf,   ToStringFast);
    public static readonly FrozenDictionary<string, SupportedLanguage> Names             = All.ToFrozenDictionary(GetName,      SelectSelf);


    private static TValue SelectSelf<TValue>( TValue v ) => v;


    extension( SupportedLanguage language )
    {
        public CultureInfo GetCultureInfo( CultureInfo defaultValue ) => language is SupportedLanguage.Unspecified || !All.Contains(language)
                                                                             ? defaultValue
                                                                             : new CultureInfo(language.GetShortName());
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
                                       SupportedLanguage.Unspecified => nameof(SupportedLanguage.Unspecified),
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
                                            SupportedLanguage.Unspecified => EMPTY,
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
        string name = culture.DisplayName;
        if ( string.IsNullOrEmpty(name) ) { return SupportedLanguage.Unspecified; }

        foreach ( ( string key, SupportedLanguage value ) in Values )
        {
            if ( name.Contains(key, StringComparison.OrdinalIgnoreCase) ) { return value; }
        }

        foreach ( ( string key, SupportedLanguage value ) in Names )
        {
            if ( name.Contains(key, StringComparison.OrdinalIgnoreCase) ) { return value; }
        }

        foreach ( ( string key, SupportedLanguage value ) in ShortNames )
        {
            if ( name.Contains(key, StringComparison.OrdinalIgnoreCase) ) { return value; }
        }

        return SupportedLanguage.Unspecified;
    }
    public static SupportedLanguage GetSupportedLanguage( this IFormatProvider culture )
    {
        if ( culture is CultureInfo info ) { return info.GetSupportedLanguage(); }

        string? name = culture.ToString();
        if ( string.IsNullOrEmpty(name) ) { return SupportedLanguage.Unspecified; }

        foreach ( ( string key, SupportedLanguage value ) in Values )
        {
            if ( name.Contains(key, StringComparison.OrdinalIgnoreCase) ) { return value; }
        }

        foreach ( ( string key, SupportedLanguage value ) in Names )
        {
            if ( name.Contains(key, StringComparison.OrdinalIgnoreCase) ) { return value; }
        }

        foreach ( ( string key, SupportedLanguage value ) in ShortNames )
        {
            if ( name.Contains(key, StringComparison.OrdinalIgnoreCase) ) { return value; }
        }

        return SupportedLanguage.Unspecified;
    }
}
