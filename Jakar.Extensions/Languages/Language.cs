namespace Jakar.Extensions.Languages;


[Serializable]
public readonly struct Language : IDataBaseID, IComparable<Language>, IEquatable<Language>, IComparable<Language?>, IEquatable<Language?>, IComparable
{
    public       string            DisplayName { get; init; } = string.Empty;
    public       string            ShortName   { get; init; } = string.Empty;
    public       SupportedLanguage Version     { get; init; } = SupportedLanguage.English;
    [Key] public long              ID          { get; init; } = SupportedLanguage.English.ToLong();


    public Language() { }
    public Language( SupportedLanguage language ) : this(language.GetName(), language.GetShortName(), language) { }
    public Language( string name, string shortName, SupportedLanguage language )
    {
        DisplayName = name;
        ShortName   = shortName;
        Version     = language;
        ID          = language.ToLong();
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
        if ( ReferenceEquals(null, obj) ) return 1;
        if ( ReferenceEquals(this, obj) ) return 0;

        return obj is Language other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(Language)}");
    }


    public static bool operator <( Language?  left, Language? right ) => RelationalComparer.Instance.Compare(left, right) < 0;
    public static bool operator >( Language?  left, Language? right ) => RelationalComparer.Instance.Compare(left, right) > 0;
    public static bool operator <=( Language? left, Language? right ) => RelationalComparer.Instance.Compare(left, right) <= 0;
    public static bool operator >=( Language? left, Language? right ) => RelationalComparer.Instance.Compare(left, right) >= 0;


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



    private sealed class RelationalComparer : IComparer<Language?>, IComparer<Language>, IComparer
    {
        public static RelationalComparer Instance { get; } = new();


        public int Compare( Language? left, Language? right ) => Nullable.Compare(left, right);

        public int Compare( Language left, Language right ) => left.CompareTo(right);


        public int Compare( object x, object y )
        {
            if ( x is not Language left ) { throw new ExpectedValueTypeException(nameof(x), x, typeof(Language)); }

            if ( y is not Language right ) { throw new ExpectedValueTypeException(nameof(y), y, typeof(Language)); }

            return left.CompareTo(right);
        }
    }



    public sealed class EqualityComparer : IEqualityComparer<Language?>, IEqualityComparer<Language>, IEqualityComparer
    {
        public static EqualityComparer Instance { get; } = new();


        public bool Equals( Language? left, Language? right ) => Nullable.Equals(left, right);
        public bool Equals( Language  left, Language  right ) => left.Equals(right);


        public int GetHashCode( Language  obj ) => obj.GetHashCode();
        public int GetHashCode( Language? obj ) => obj.GetHashCode();


        bool IEqualityComparer.Equals( object x, object y )
        {
            if ( x is not Language left ) { throw new ExpectedValueTypeException(nameof(x), x, typeof(Language)); }

            if ( y is not Language right ) { throw new ExpectedValueTypeException(nameof(y), y, typeof(Language)); }

            return left.Equals(right);
        }

        int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();
    }
}
