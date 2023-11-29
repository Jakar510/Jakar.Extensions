using System.Collections.Frozen;


#pragma warning disable IDE0046



namespace Jakar.Extensions;


[ SuppressMessage( "ReSharper", "StringLiteralTypo" ) ]
public static class LanguageExtensions
{
#if NET8_0_OR_GREATER
    public static readonly FrozenDictionary<string, SupportedLanguage> Languages  = Enum.GetValues<SupportedLanguage>().ToFrozenDictionary( ToStringFast, ValueSelector );
    public static readonly FrozenDictionary<string, SupportedLanguage> ShortNames = Enum.GetValues<SupportedLanguage>().ToFrozenDictionary( GetShortName, ValueSelector );
    public static readonly FrozenDictionary<string, SupportedLanguage> Names      = Enum.GetValues<SupportedLanguage>().ToFrozenDictionary( GetName,      ValueSelector );
#elif NET6_0_OR_GREATER
    public  static readonly ImmutableDictionary<string, SupportedLanguage> Languages = Enum.GetValues<SupportedLanguage>().ToImmutableDictionary( ToStringFast, ValueSelector );
    public  static readonly ImmutableDictionary<string, SupportedLanguage> ShortNames = Enum.GetValues<SupportedLanguage>().ToImmutableDictionary( GetShortName, ValueSelector );
    public  static readonly ImmutableDictionary<string, SupportedLanguage> Names = Enum.GetValues<SupportedLanguage>().ToImmutableDictionary( GetName, ValueSelector );
#else
    public static readonly ImmutableDictionary<string, SupportedLanguage> Languages = Enum.GetValues( typeof(SupportedLanguage) ).Cast<SupportedLanguage>().ToImmutableDictionary( ToStringFast, ValueSelector );
    public static readonly ImmutableDictionary<string, SupportedLanguage> ShortNames = Enum.GetValues( typeof(SupportedLanguage) ).Cast<SupportedLanguage>().ToImmutableDictionary( GetShortName, ValueSelector );
    public static readonly ImmutableDictionary<string, SupportedLanguage> Names = Enum.GetValues( typeof(SupportedLanguage) ).Cast<SupportedLanguage>().ToImmutableDictionary( GetName,      ValueSelector );
#endif


    private static SupportedLanguage ValueSelector( SupportedLanguage v ) => v;


    public static string GetName( this SupportedLanguage language ) => language switch
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
                                                                           _                             => string.Empty
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
                                                                                _                             => throw new OutOfRangeException( nameof(language), language )
                                                                            };
    public static string ToStringFast( SupportedLanguage language ) => language switch
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
                                                                           _                             => throw new ArgumentOutOfRangeException( nameof(language), language, null )
                                                                       };


    public static SupportedLanguage? GetSupportedLanguage( this CultureInfo culture )
    {
        string name = culture.DisplayName;
        if ( string.IsNullOrEmpty( name ) ) { return null; }

        foreach ( (string? key, SupportedLanguage value) in Languages )
        {
            if ( name.Contains( key, StringComparison.OrdinalIgnoreCase ) ) { return value; }
        }

        foreach ( (string? key, SupportedLanguage value) in Names )
        {
            if ( name.Contains( key, StringComparison.OrdinalIgnoreCase ) ) { return value; }
        }

        foreach ( (string? key, SupportedLanguage value) in ShortNames )
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

        foreach ( (string? key, SupportedLanguage value) in Languages )
        {
            if ( name.Contains( key, StringComparison.OrdinalIgnoreCase ) ) { return value; }
        }

        foreach ( (string? key, SupportedLanguage value) in Names )
        {
            if ( name.Contains( key, StringComparison.OrdinalIgnoreCase ) ) { return value; }
        }

        foreach ( (string? key, SupportedLanguage value) in ShortNames )
        {
            if ( name.Contains( key, StringComparison.OrdinalIgnoreCase ) ) { return value; }
        }

        return null;
    }
}
