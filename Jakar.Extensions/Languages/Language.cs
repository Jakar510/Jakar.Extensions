namespace Jakar.Extensions.Languages;


[Serializable]
public readonly struct Language : IDataBaseID, IComparable<Language>, IEquatable<Language>, IComparable<Language?>, IEquatable<Language?>, IComparable
{
    public       string            DisplayName { get; init; } = string.Empty;
    public       string            ShortName   { get; init; } = string.Empty;
    public       SupportedLanguage Version     { get; init; } = SupportedLanguage.English;
    [Key] public long              ID          { get; init; } = SupportedLanguage.English.AsLong();


    public Language() { }
    public Language( SupportedLanguage language ) : this(language.GetName(), language.GetShortName(), language) { }
    public Language( string name, string shortName, SupportedLanguage language )
    {
        DisplayName = name;
        ShortName   = shortName;
        Version     = language;
        ID          = language.AsLong();
    }


    public static implicit operator Language( SupportedLanguage language ) => new(language);


    public CultureInfo ToCultureInfo() => new(ShortName);


    public bool Equals( Language? other )
    {
        if ( other is null ) { return false; }

        return Equals(other.Value);
    }
    public bool Equals( Language other ) => DisplayName == other.DisplayName &&
                                            ShortName == other.ShortName &&
                                            Version == other.Version &&
                                            ID == other.ID;
    public override bool Equals( object? obj ) => obj is Language language && Equals(language);
    public override int  GetHashCode()         => HashCode.Combine(DisplayName, ShortName, Version, ID);


    public int CompareTo( Language? other )
    {
        if ( other is null ) { return -1; }

        return CompareTo(other.Value);
    }
    public int CompareTo( Language other )
    {
        int displayNameComparison = string.Compare(DisplayName, other.DisplayName, StringComparison.Ordinal);
        if ( displayNameComparison != 0 ) { return displayNameComparison; }

        int shortNameComparison = string.Compare(ShortName, other.ShortName, StringComparison.Ordinal);
        if ( shortNameComparison != 0 ) { return shortNameComparison; }

        int versionComparison = Version.CompareTo(other.Version);
        if ( versionComparison != 0 ) { return versionComparison; }

        return ID.CompareTo(other.ID);
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is Language other
                   ? CompareTo(other)
                   : throw new ExpectedValueTypeException(nameof(obj), obj, typeof(Language));
    }


    public static bool operator <( Language?  left, Language? right ) => Sorter.Instance.Compare(left, right) < 0;
    public static bool operator >( Language?  left, Language? right ) => Sorter.Instance.Compare(left, right) > 0;
    public static bool operator <=( Language? left, Language? right ) => Sorter.Instance.Compare(left, right) <= 0;
    public static bool operator >=( Language? left, Language? right ) => Sorter.Instance.Compare(left, right) >= 0;


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



    public sealed class Equalizer : ValueEqualizer<Language> { }



    public sealed class Sorter : ValueSorter<Language> { }
}
