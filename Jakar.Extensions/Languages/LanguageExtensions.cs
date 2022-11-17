#nullable enable
namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "StringLiteralTypo" )]
public static class LanguageExtensions
{
    public static string GetName( this SupportedLanguage language ) =>
        language switch
        {
            SupportedLanguage.English    => "English",
            SupportedLanguage.Spanish    => "Español - Spanish",
            SupportedLanguage.French     => "Français - French",
            SupportedLanguage.Swedish    => "svenska - Swedish",
            SupportedLanguage.German     => "Deutsche - German",
            SupportedLanguage.Chinese    => "中文 - Chinese (simplified)",
            SupportedLanguage.Polish     => "Polskie - Polish",
            SupportedLanguage.Thai       => "ไทย - Thai",
            SupportedLanguage.Japanese   => "日本語 - Japanese",
            SupportedLanguage.Czech      => "čeština - Czech",
            SupportedLanguage.Portuguese => "Português - Portuguese",
            SupportedLanguage.Dutch      => "Nederlands - Dutch",
            SupportedLanguage.Korean     => "한국어 - Korean",
            SupportedLanguage.Arabic     => "عربى - Arabic",
            _                            => throw new ArgumentOutOfRangeException( nameof(language), language, null ),
        };
    public static string GetShortName( this SupportedLanguage language ) =>
        language switch
        {
            SupportedLanguage.English    => "en",
            SupportedLanguage.Spanish    => "es",
            SupportedLanguage.French     => "fr",
            SupportedLanguage.Swedish    => "sv",
            SupportedLanguage.German     => "de",
            SupportedLanguage.Chinese    => "zh-Hans",
            SupportedLanguage.Polish     => "pl",
            SupportedLanguage.Thai       => "th",
            SupportedLanguage.Japanese   => "ja",
            SupportedLanguage.Czech      => "cs",
            SupportedLanguage.Portuguese => "pt",
            SupportedLanguage.Dutch      => "nl",
            SupportedLanguage.Korean     => "ko",
            SupportedLanguage.Arabic     => "ar",
            _                            => throw new ArgumentOutOfRangeException( nameof(language), language, null ),
        };
}
