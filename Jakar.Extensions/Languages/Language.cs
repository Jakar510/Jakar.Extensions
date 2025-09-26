using ZLinq;
using ZLinq.Linq;



namespace Jakar.Extensions;


[Serializable][DebuggerDisplay(nameof(DisplayName))]
public sealed class Language : BaseClass, IEqualComparable<Language>
{
    private readonly CultureInfo __culture;


    public string             DisplayName      { get; init; }
    public string             EnglishName      { get; init; }
    public bool               IsNeutralCulture { get; init; }
    public string             Name             { get; init; }
    public string             ThreeLetterISO   { get; init; }
    public string             TwoLetterISO     { get; init; }
    public SupportedLanguage? Version          { get; init; }


    public Language( CultureInfo culture, SupportedLanguage? version = null )
    {
        __culture        = culture;
        Name             = culture.Name;
        EnglishName      = culture.EnglishName;
        TwoLetterISO     = culture.TwoLetterISOLanguageName;
        ThreeLetterISO   = culture.ThreeLetterISOLanguageName;
        IsNeutralCulture = culture.IsNeutralCulture;
        Version          = version ??= culture.GetSupportedLanguage();
        DisplayName      = version?.GetName() ?? culture.DisplayName;
    }
    public Language( SupportedLanguage language ) : this(language.GetCultureInfo(CultureInfo.InvariantCulture), language) { }


    public static implicit operator Language( CultureInfo       value ) => new(value);
    public static implicit operator Language( SupportedLanguage value ) => new(value);
    public static implicit operator CultureInfo( Language       value ) => value.__culture;


    public          CultureInfo GetCulture()                  => __culture;
    public override string      ToString()                    => DisplayName;
    private static  Language    Create( CultureInfo culture ) => new(culture);


    public int CompareTo( object? value ) => value switch
                                             {
                                                 null           => 1,
                                                 Language other => CompareTo(other),
                                                 _              => throw new ExpectedValueTypeException(nameof(value), value, typeof(Language))
                                             };
    public int CompareTo( Language? other )
    {
        if ( other is null ) { return -1; }

        int displayNameComparison = string.Compare(DisplayName, other.DisplayName, StringComparison.Ordinal);
        if ( displayNameComparison != 0 ) { return displayNameComparison; }

        int shortNameComparison = string.Compare(Name, other.Name, StringComparison.Ordinal);
        if ( shortNameComparison != 0 ) { return shortNameComparison; }

        return Nullable.Compare(Version, other.Version);
    }
    public bool Equals( Language? other )
    {
        if ( other is null ) { return false; }

        return DisplayName == other.DisplayName && Name == other.Name && Version == other.Version;
    }
    public override bool Equals( object? obj ) => obj is Language language && Equals(language);
    public override int  GetHashCode()         => HashCode.Combine(DisplayName, Name, Version);


    public static bool operator ==( Language? left, Language? right ) => EqualityComparer<Language>.Default.Equals(left, right);
    public static bool operator !=( Language? left, Language? right ) => !EqualityComparer<Language>.Default.Equals(left, right);
    public static bool operator >( Language   left, Language  right ) => Comparer<Language>.Default.Compare(left, right) > 0;
    public static bool operator >=( Language  left, Language  right ) => Comparer<Language>.Default.Compare(left, right) >= 0;
    public static bool operator <( Language   left, Language  right ) => Comparer<Language>.Default.Compare(left, right) < 0;
    public static bool operator <=( Language  left, Language  right ) => Comparer<Language>.Default.Compare(left, right) <= 0;



    [Serializable]
    public class Items : List<Language>
    {
        public Items() : base(DEFAULT_CAPACITY) { }
        public Items( int                   capacity ) : base(capacity) { }
        public Items( IEnumerable<Language> items ) : base(items) => Sort(Comparer<Language>.Default);
        public Items( in ValueEnumerable<ArraySelect<CultureInfo, Language>, Language> enumerable ) : this(enumerable.TryGetNonEnumeratedCount(out int count)
                                                                                                               ? count
                                                                                                               : DEFAULT_CAPACITY)
        {
            foreach ( Language language in enumerable ) { Add(language); }
        }
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

    public static Items NeutralCultures  => new(CultureInfo.GetCultures(CultureTypes.NeutralCultures).AsValueEnumerable().Select(Create));
    public static Items SpecificCultures => new(CultureInfo.GetCultures(CultureTypes.SpecificCultures).AsValueEnumerable().Select(Create));
    public static Items All              => new(CultureInfo.GetCultures(CultureTypes.AllCultures).AsValueEnumerable().Select(Create));

    public static LanguageCollection Supported { get; } =
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



[Serializable]
public class LanguageCollection : ObservableCollection<LanguageCollection, Language>, ICollectionAlerts<LanguageCollection, Language>
{
    public static JsonSerializerContext              JsonContext   => JakarExtensionsContext.Default;
    public static JsonTypeInfo<LanguageCollection>   JsonTypeInfo  => JakarExtensionsContext.Default.LanguageCollection;
    public static JsonTypeInfo<LanguageCollection[]> JsonArrayInfo => JakarExtensionsContext.Default.LanguageCollectionArray;


    public LanguageCollection() : base(DEFAULT_CAPACITY) { }
    public LanguageCollection( int                           capacity ) : base(capacity) { }
    public LanguageCollection( IEnumerable<Language>         items ) : base(items) { }
    public LanguageCollection( params ReadOnlySpan<Language> items ) : base(items) { }
    public LanguageCollection( in ValueEnumerable<ArraySelect<CultureInfo, Language>, Language> enumerable ) : this(enumerable.TryGetNonEnumeratedCount(out int count)
                                                                                                                        ? count
                                                                                                                        : DEFAULT_CAPACITY)
    {
        foreach ( Language language in enumerable ) { Add(language); }
    }
    public static implicit operator LanguageCollection( List<Language>           values ) => new(values);
    public static implicit operator LanguageCollection( HashSet<Language>        values ) => new(values);
    public static implicit operator LanguageCollection( ConcurrentBag<Language>  values ) => new(values);
    public static implicit operator LanguageCollection( Collection<Language>     values ) => new(values);
    public static implicit operator LanguageCollection( Language[]               values ) => new(values.AsSpan());
    public static implicit operator LanguageCollection( ImmutableArray<Language> values ) => new(values.AsSpan());
    public static implicit operator LanguageCollection( ReadOnlyMemory<Language> values ) => new(values.Span);
    public static implicit operator LanguageCollection( ReadOnlySpan<Language>   values ) => new(values);

    
    public static bool operator ==( LanguageCollection? left, LanguageCollection? right ) => EqualityComparer<LanguageCollection>.Default.Equals(left, right);
    public static bool operator !=( LanguageCollection? left, LanguageCollection? right ) => !EqualityComparer<LanguageCollection>.Default.Equals(left, right);
    public static bool operator >( LanguageCollection   left, LanguageCollection  right ) => Comparer<LanguageCollection>.Default.Compare(left, right) > 0;
    public static bool operator >=( LanguageCollection  left, LanguageCollection  right ) => Comparer<LanguageCollection>.Default.Compare(left, right) >= 0;
    public static bool operator <( LanguageCollection   left, LanguageCollection  right ) => Comparer<LanguageCollection>.Default.Compare(left, right) < 0;
    public static bool operator <=( LanguageCollection  left, LanguageCollection  right ) => Comparer<LanguageCollection>.Default.Compare(left, right) <= 0;
}
