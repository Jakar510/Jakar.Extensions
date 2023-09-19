#nullable enable
#pragma warning disable IDE0046



namespace Jakar.Extensions;


[ SuppressMessage( "ReSharper", "StringLiteralTypo" ) ]
public static class LanguageExtensions
{
    private static readonly IReadOnlyDictionary<string, SupportedLanguage> _languages = Enum.GetValues( typeof(SupportedLanguage) )
                                                                                            .Cast<SupportedLanguage>()
                                                                                            .ToDictionary( k => k.ToString(), v => v );


    public static string? GetName( this SupportedLanguage language ) => language switch
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
                                                                            SupportedLanguage.Unspecified => null,
                                                                            _                             => null,
                                                                        };


    public static string GetShortName( this SupportedLanguage language ) => language switch
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
                                                                                SupportedLanguage.Unspecified => throw new OutOfRangeException( nameof(language), language ),
                                                                                _                             => throw new OutOfRangeException( nameof(language), language ),
                                                                            };


    public static SupportedLanguage? GetSupportedLanguage( this CultureInfo culture )
    {
        string name = culture.DisplayName;

        foreach ( (string? key, SupportedLanguage value) in _languages )
        {
            if ( name.Contains( key, StringComparison.OrdinalIgnoreCase ) ) { return value; }
        }

        return null;
    }
    public static SupportedLanguage? GetSupportedLanguage( this IFormatProvider culture )
    {
        if ( culture is CultureInfo info ) { return info.GetSupportedLanguage(); }

        string? name = culture.ToString();
        if ( string.IsNullOrEmpty( name ) ) { return null; }

        foreach ( (string? key, SupportedLanguage value) in _languages )
        {
            if ( name.Contains( key, StringComparison.OrdinalIgnoreCase ) ) { return value; }
        }

        return null;
    }
}
