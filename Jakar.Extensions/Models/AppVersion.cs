namespace Jakar.Extensions.Models;


/// <summary>
/// See <see cref="Format"/>  for formatting details.
/// </summary>
[Serializable]
[JsonConverter(typeof(AppVersionConverter))] // AppVersionNullableConverter
[SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
[SuppressMessage("ReSharper", "ParameterTypeCanBeEnumerable.Local")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
public readonly struct AppVersion : IComparable, IComparable<AppVersion>, IComparable<AppVersion?>, IEquatable<AppVersion?>, IEquatable<AppVersion>, IReadOnlyCollection<int>, ICloneable
{
    public enum Format
    {
        /// <summary>
        /// Major
        /// </summary>
        Singular = 1,

        /// <summary>
        /// Major.Minor
        /// </summary>
        Minimal = 2,

        /// <summary>
        /// Major.Minor.Build
        /// </summary>
        Typical,

        /// <summary>
        /// Major.Minor.Maintenance.Build
        /// </summary>
        Detailed,

        /// <summary>
        /// Major.Minor.Maintenance.MajorRevision.Build
        /// </summary>
        DetailedRevisions,

        /// <summary>
        /// Major.Minor.Maintenance.MajorRevision.MinorRevision.Build
        /// </summary>
        Complete
    }



    public enum Option
    {
        Stable,
        ReleaseCandidate,
        Alpha,
        Beta,
    }



    public readonly Format           Scheme;
    public readonly int              Major;
    public readonly int?             Minor         = default;
    public readonly int?             Maintenance   = default;
    public readonly int?             MajorRevision = default;
    public readonly int?             MinorRevision = default;
    public readonly int?             Build         = default;
    public readonly Option           Type;
    private const   StringComparison COMPARISON = StringComparison.OrdinalIgnoreCase;


    public AppVersion() : this(0, Option.Stable) { }
    public AppVersion( int major, Option type ) : this(major, null, type) { }
    public AppVersion( int major, int?   minor, Option type ) : this(major, minor, null, type) { }
    public AppVersion( int major, int?   minor, int?   build,       Option type ) : this(major, minor, null, build, type) { }
    public AppVersion( int major, int?   minor, int?   maintenance, int?   build,         Option type ) : this(major, minor, maintenance, null, build, type) { }
    public AppVersion( int major, int?   minor, int?   maintenance, int?   majorRevision, int?   build, Option type ) : this(major, minor, maintenance, majorRevision, null, build, type) { }

    public AppVersion( int major, int? minor, int? maintenance, int? majorRevision, int? minorRevision, int? build, Option type )
    {
        Major         = major;
        Minor         = minor;
        Maintenance   = maintenance;
        Build         = build;
        MajorRevision = majorRevision;
        MinorRevision = minorRevision;
        Scheme        = GetFormat(Minor, Maintenance, MajorRevision, MinorRevision, Build);
        Type          = type;
    }

    public AppVersion( Version version )
    {
        Major         = version.Major;
        Minor         = version.Minor;
        Maintenance   = version.Revision;
        Build         = version.Build;
        MinorRevision = version.MinorRevision;
        MajorRevision = version.MajorRevision;
        Scheme        = GetFormat(Minor, Maintenance, MajorRevision, MinorRevision, Build);
        Type          = Option.Stable;
    }

    public AppVersion( in Span<int>         items, Option options ) : this(items.ToArray(), options) { }
    public AppVersion( in ReadOnlySpan<int> items, Option options ) : this(items.ToArray(), options) { }

    public AppVersion( IReadOnlyList<int> items, Option options )
    {
        if ( items is null ) { throw new ArgumentNullException(nameof(items)); }

        Scheme = (Format)items.Count;
        Type   = options;

        switch ( Scheme )
        {
            case Format.Singular:
                Major = items[0];
                return;

            case Format.Minimal:
                Major = items[0];
                Minor = items[1];
                return;

            case Format.Typical:
                Major = items[0];
                Minor = items[1];
                Build = items[2];
                return;

            case Format.Detailed:
                Major       = items[0];
                Minor       = items[1];
                Maintenance = items[2];
                Build       = items[3];
                return;

            case Format.DetailedRevisions:
                Major         = items[0];
                Minor         = items[1];
                Maintenance   = items[2];
                MajorRevision = items[3];
                Build         = items[4];
                return;

            case Format.Complete:
                Major         = items[0];
                Minor         = items[1];
                Maintenance   = items[2];
                MajorRevision = items[3];
                MinorRevision = items[4];
                Build         = items[5];
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(items), items.Count, @"value doesn't contain the correct amount of items.");
        }
    }


    public static implicit operator AppVersion( string            value ) => Parse(value);
    public static implicit operator AppVersion( Span<int>         value ) => new(value, Option.Stable);
    public static implicit operator AppVersion( ReadOnlySpan<int> value ) => new(value, Option.Stable);
    public static implicit operator AppVersion( Version           value ) => new(value);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="version"></param>
    /// <returns>
    /// <see langword="true"/> if <see cref="Parse(string)"/> as successful.
    /// <br/>
    /// <see langword="false"/> otherwise.
    /// </returns>
    public static bool TryParse( in string? value, [NotNullWhen(true)] out AppVersion? version )
    {
        try
        {
            if ( !string.IsNullOrWhiteSpace(value) )
            {
                version = Parse(value);
                return true;
            }
        }
        catch ( FormatException ) { }
        catch ( ArgumentException ) { }

        version = default;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OverflowException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns><see cref="AppVersion"/></returns>
    public static AppVersion Parse( in string value )
    {
        if ( string.IsNullOrWhiteSpace(value) ) { throw new ArgumentNullException(nameof(value)); }

        // if ( int.TryParse(value.Trim(), out int n) ) { return new AppVersion(n); }
        try
        {
            Option options = Split(value, out List<string> source);
            var    result  = new List<int>(source.Count);

            foreach ( string item in source )
            {
                if ( int.TryParse(item, out int n) ) { result.Add(n); }
                else { throw new FormatException($"Cannot convert '{item}' to an int."); }
            }

            return new AppVersion(result, options);
        }
        catch ( Exception e ) { throw new ArgumentException($"Cannot convert '{value}' into {nameof(AppVersion)}", nameof(value), e); }
    }

    private static Option? Trim( ref string value, in string candidate, in Option option )
    {
        if ( value.EndsWith(candidate, COMPARISON) )
        {
            value = value.Remove(value.IndexOf(candidate, COMPARISON));
            return option;
        }

        return default;
    }

    private static Option Split( in string value, out List<string> source )
    {
        string temp = value;

        Option options = Trim(ref temp, "-rc", Option.ReleaseCandidate)
                      ?? Trim(ref temp, "rc", Option.ReleaseCandidate)
                      ?? Trim(ref temp, "-alpha", Option.Alpha)
                      ?? Trim(ref temp, "alpha", Option.Alpha)
                      ?? Trim(ref temp, "-beta", Option.Beta)
                      ?? Trim(ref temp, "beta", Option.Beta)
                      ?? Option.Stable;

        source = new List<string>(temp.Split(SEPARATOR));
        return options;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="version"></param>
    /// <returns>
    /// <see langword="true"/> if <see cref="Parse(ReadOnlySpan&lt;char&gt;)"/> as successful.
    /// <br/>
    /// <see langword="false"/> otherwise.
    /// </returns>
    public static bool TryParse( in ReadOnlySpan<char> value, [NotNullWhen(true)] out AppVersion? version )
    {
        try
        {
            if ( !value.IsEmpty )
            {
                version = Parse(value);
                return true;
            }
        }
        catch ( FormatException ) { }
        catch ( ArgumentException ) { }

        version = default;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OverflowException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns><see cref="AppVersion"/></returns>
    public static AppVersion Parse( ReadOnlySpan<char> value )
    {
        if ( value.IsEmpty ) { throw new ArgumentNullException(nameof(value)); }

        value.ToString().WriteToConsole();

        try
        {
            Option options;

            if ( value.EndsWith("-rc", COMPARISON) )
            {
                value   = value[..value.IndexOf("-rc", COMPARISON)];
                options = Option.ReleaseCandidate;
            }
            else if ( value.EndsWith("rc", COMPARISON) )
            {
                value   = value[..value.IndexOf("rc", COMPARISON)];
                options = Option.ReleaseCandidate;
            }
            else if ( value.EndsWith("-alpha", COMPARISON) )
            {
                value   = value[..value.IndexOf("-alpha", COMPARISON)];
                options = Option.Alpha;
            }
            else if ( value.EndsWith("alpha", COMPARISON) )
            {
                value   = value[..value.IndexOf("alpha", COMPARISON)];
                options = Option.Alpha;
            }
            else if ( value.EndsWith("-beta", COMPARISON) )
            {
                value   = value[..value.IndexOf("-beta", COMPARISON)];
                options = Option.Beta;
            }
            else if ( value.EndsWith("beta", COMPARISON) )
            {
                value   = value[..value.IndexOf("beta", COMPARISON)];
                options = Option.Beta;
            }
            else { options = Option.Stable; }

            value.ToString().WriteToConsole();

            var result = new List<int>();

            foreach ( ( ReadOnlySpan<char> item, _ ) in value.SplitOn(SEPARATOR) )
            {
                if ( int.TryParse(item, NumberStyles.Any, CultureInfo.CurrentCulture, out int n) ) { result.Add(n); }
                else { throw new FormatException($"Cannot convert '{item.ToString()}' to an int."); }
            }

            return new AppVersion(result, options);
        }
        catch ( Exception e ) { throw new ArgumentException($"Cannot convert '{value.ToString()}' into {nameof(AppVersion)}", nameof(value), e); }
    }


    private static Format GetFormat( in int? minor, in int? maintenance, in int? majorRevision, in int? minorRevision, in int? build )
    {
        if ( minor.HasValue && maintenance.HasValue && majorRevision.HasValue && minorRevision.HasValue && build.HasValue ) { return Format.Complete; }

        if ( minor.HasValue && maintenance.HasValue && majorRevision.HasValue && build.HasValue ) { return Format.DetailedRevisions; }

        if ( minor.HasValue && maintenance.HasValue && build.HasValue ) { return Format.Detailed; }

        if ( minor.HasValue && build.HasValue ) { return Format.Typical; }

        if ( minor.HasValue ) { return Format.Minimal; }

        return Format.Singular;
    }


    // ---------------------------------------------------------------------------------------------------------------------------------


    public const char SEPARATOR = '.';

    public override string ToString()
    {
        string s = this.Aggregate(string.Empty, ( current, item ) => current + $"{item}{SEPARATOR}").TrimEnd(SEPARATOR);

        string addOn = Type switch
                       {
                           Option.ReleaseCandidate => "-rc",
                           Option.Alpha            => "-alpha",
                           Option.Beta             => "-beta",
                           Option.Stable           => string.Empty,
                           _                       => throw new OutOfRangeException(nameof(Type), Type)
                       };

        return s + addOn;
    }

    public object  Clone()     => Parse(ToString());
    public Version ToVersion() => Version.Parse(ToString());


    public IReadOnlyList<int> ToList() => new List<int>(this);

    public IEnumerator<int> GetEnumerator()
    {
        yield return Major;

        if ( Minor.HasValue ) { yield return Minor.Value; }

        if ( Maintenance.HasValue ) { yield return Maintenance.Value; }

        if ( MajorRevision.HasValue ) { yield return MajorRevision.Value; }

        if ( MinorRevision.HasValue ) { yield return MinorRevision.Value; }

        if ( Build.HasValue ) { yield return Build.Value; }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    int IReadOnlyCollection<int>.Count => Scheme.ToInt();


    public static bool operator ==( in AppVersion left, in AppVersion right ) => EqualityComparer.Instance.Equals(left, right);
    public static bool operator !=( in AppVersion left, in AppVersion right ) => !EqualityComparer.Instance.Equals(left, right);
    public static bool operator >( in  AppVersion left, in AppVersion right ) => RelationalComparer.Instance.Compare(left, right) > 0;
    public static bool operator >=( in AppVersion left, in AppVersion right ) => RelationalComparer.Instance.Compare(left, right) >= 0;
    public static bool operator <( in  AppVersion left, in AppVersion right ) => RelationalComparer.Instance.Compare(left, right) < 0;
    public static bool operator <=( in AppVersion left, in AppVersion right ) => RelationalComparer.Instance.Compare(left, right) <= 0;

    [DoesNotReturn]
    private void RaiseFormatError( in AppVersion other ) => throw new FormatException($"expected {nameof(other)}.{nameof(Scheme)} is '{other.Scheme}' and got '{Scheme}'");


    /// <summary>
    /// Compares two <see cref="AppVersion"/> instances
    /// </summary>
    /// <param name="other"></param>
    /// <returns>
    /// <see langword="0"/> if the <paramref name="other"/> is equivalent.
    /// <br/>
    /// <see langword="-1"/>  if the <paramref name="other"/> is smaller.
    /// <br/>
    /// <see langword="1"/>  if the <paramref name="other"/> is greater.
    /// <br/>
    /// <see langword="-2"/>  if the <paramref name="other"/> is not an instance of <see cref="AppVersion"/>.
    /// <br/>
    /// </returns>
    /// <exception cref="ArgumentException">If something goes wrong, and the <paramref name="other"/> is not comparable.</exception>
    public int CompareTo( object? other )
    {
        return other switch
               {
                   Version version    => CompareTo(version),
                   AppVersion version => CompareTo(version),
                   _                  => throw new ExpectedValueTypeException(nameof(other), other, typeof(Version), typeof(AppVersion))
               };
    }


    /// <summary>
    /// Compares two <see cref="AppVersion"/> instances
    /// </summary>
    /// <param name="other"></param>
    /// <returns>
    /// <see langword="0"/> if the <paramref name="other"/> is equivalent.
    /// <br/>
    /// <see langword="-1"/>  if the <paramref name="other"/> is smaller.
    /// <br/>
    /// <see langword="1"/>  if the <paramref name="other"/> is greater.
    /// <br/>
    /// </returns>
    /// <exception cref="ArgumentException">If something goes wrong, and the <paramref name="other"/> is not comparable.</exception>
    public int CompareTo( AppVersion? other )
    {
        if ( other is null ) { return -1; }

        return CompareTo(other.Value);
    }

    public int CompareTo( AppVersion other )
    {
        if ( Scheme != other.Scheme ) { RaiseFormatError(other); }

        int majorComparison = Major.CompareTo(other.Major);
        if ( majorComparison != 0 ) { return majorComparison; }

        int minorComparison = Nullable.Compare(Minor, other.Minor);
        if ( minorComparison != 0 ) { return minorComparison; }

        int maintenanceComparison = Nullable.Compare(Maintenance, other.Maintenance);
        if ( maintenanceComparison != 0 ) { return maintenanceComparison; }

        int majorRevisionComparison = Nullable.Compare(MajorRevision, other.MajorRevision);
        if ( majorRevisionComparison != 0 ) { return majorRevisionComparison; }

        int minorRevisionComparison = Nullable.Compare(MinorRevision, other.MinorRevision);
        if ( minorRevisionComparison != 0 ) { return minorRevisionComparison; }

        return Nullable.Compare(Build, other.Build);
    }


    /// <summary>
    /// Compares two <see cref="AppVersion"/> instances for relative equality.
    /// </summary>
    /// <param name="other"></param>
    /// <returns>
    /// <para>
    ///  returns <see langword="true"/> if all of the following is true: 
    /// <list type="number">
    /// <item> If <see cref="Major"/> and <see cref="Minor"/> match </item><br/>
    /// <item>
    /// All other fields that are not <see langword="null"/> and are equal to or greater than <br/>
    /// <list type="bullet">
    /// <item><see cref="Maintenance"/></item><br/>
    /// <item><see cref="MajorRevision"/></item><br/>
    /// <item><see cref="MinorRevision"/></item><br/>
    /// <item><see cref="Build"/></item><br/>
    /// </list>
    /// </item>
    /// <br/>
    /// </list>
    /// </para>
    /// <para> otherwise <see langword="false"/> </para>
    /// </returns>
    /// <exception cref="ArgumentException">If something goes wrong, and the <paramref name="other"/> is not comparable.</exception>
    public bool FuzzyEquals( AppVersion? other )
    {
        if ( other is null ) { return false; }

        return FuzzyEquals(other.Value);
    }

    /// <summary>
    /// Compares two <see cref="AppVersion"/> instances for relative equality.
    /// </summary>
    /// <param name="other"></param>
    /// <returns>
    /// <para>
    ///  returns <see langword="true"/> if all of the following is true: 
    /// <list type="number">
    /// <item> If <see cref="Major"/> and <see cref="Minor"/> match </item><br/>
    /// <item>
    /// All other fields that are not <see langword="null"/> and are equal to or greater than <br/>
    /// <list type="bullet">
    /// <item><see cref="Maintenance"/></item><br/>
    /// <item><see cref="MajorRevision"/></item><br/>
    /// <item><see cref="MinorRevision"/></item><br/>
    /// <item><see cref="Build"/></item><br/>
    /// </list>
    /// </item>
    /// <br/>
    /// </list>
    /// </para>
    /// <para> otherwise <see langword="false"/> </para>
    /// </returns>
    /// <exception cref="ArgumentException">If something goes wrong, and the <paramref name="other"/> is not comparable.</exception>
    public bool FuzzyEquals( AppVersion other )
    {
        if ( Scheme != other.Scheme ) { RaiseFormatError(other); }

        bool result = Major.Equals(other.Major) &&
                      Nullable.Compare(other.Minor, Minor) == 0 &&
                      Nullable.Compare(other.Maintenance, Maintenance) >= 0 &&
                      Nullable.Compare(other.MajorRevision, MajorRevision) >= 0 &&
                      Nullable.Compare(other.MinorRevision, MinorRevision) >= 0 &&
                      Nullable.Compare(other.Build, Build) >= 0;

        return result;
    }


    public bool Equals( AppVersion? other )
    {
        if ( other is null ) { return false; }

        return Equals(other.Value);
    }

    public bool Equals( AppVersion other )
    {
        if ( Scheme != other.Scheme ) { RaiseFormatError(other); }

        return Major == other.Major &&
               Nullable.Equals(Minor, other.Minor) &&
               Nullable.Equals(Maintenance, other.Maintenance) &&
               Nullable.Equals(MajorRevision, other.MajorRevision) &&
               Nullable.Equals(MinorRevision, other.MinorRevision) &&
               Nullable.Equals(Build, other.Build);
    }


    public override bool Equals( object obj ) => obj is AppVersion version && Equals(version);

    public override int GetHashCode() => HashCode.Combine(Scheme, Major, Minor, Maintenance, MajorRevision, MinorRevision, Build);


    // ---------------------------------------------------------------------------------------------------------------------------------



    private sealed class RelationalComparer : IComparer<AppVersion?>, IComparer<AppVersion>, IComparer
    {
        public static RelationalComparer Instance { get; } = new();


        public int Compare( AppVersion? left, AppVersion? right ) => Nullable.Compare(left, right);

        public int Compare( AppVersion left, AppVersion right ) => left.CompareTo(right);


        public int Compare( object x, object y )
        {
            if ( x is not AppVersion left ) { throw new ExpectedValueTypeException(nameof(x), x, typeof(AppVersion)); }

            if ( y is not AppVersion right ) { throw new ExpectedValueTypeException(nameof(y), y, typeof(AppVersion)); }

            return left.CompareTo(right);
        }
    }



    public sealed class EqualityComparer : IEqualityComparer<AppVersion?>, IEqualityComparer<AppVersion>, IEqualityComparer
    {
        public static EqualityComparer Instance { get; } = new();


        public bool Equals( AppVersion? left, AppVersion? right ) => Nullable.Equals(left, right);
        public bool Equals( AppVersion  left, AppVersion  right ) => left.Equals(right);


        public int GetHashCode( AppVersion  obj ) => obj.GetHashCode();
        public int GetHashCode( AppVersion? obj ) => obj.GetHashCode();


        bool IEqualityComparer.Equals( object x, object y )
        {
            if ( x is not AppVersion left ) { throw new ExpectedValueTypeException(nameof(x), x, typeof(AppVersion)); }

            if ( y is not AppVersion right ) { throw new ExpectedValueTypeException(nameof(y), y, typeof(AppVersion)); }

            return left.Equals(right);
        }

        int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();
    }



    public sealed class FuzzyEqualityComparer : IEqualityComparer<AppVersion?>, IEqualityComparer<AppVersion>, IEqualityComparer
    {
        public static FuzzyEqualityComparer Instance { get; } = new();


        public bool Equals( AppVersion? left, AppVersion? right )
        {
            if ( left.HasValue && right.HasValue ) { left.Value.FuzzyEquals(right.Value); }

            if ( left is null && right is null ) { return true; }

            return false;
        }

        public bool Equals( AppVersion left, AppVersion right ) => left.FuzzyEquals(right);


        public int GetHashCode( AppVersion  obj ) => obj.GetHashCode();
        public int GetHashCode( AppVersion? obj ) => obj.GetHashCode();


        bool IEqualityComparer.Equals( object x, object y )
        {
            if ( x is not AppVersion left ) { throw new ExpectedValueTypeException(nameof(x), x, typeof(AppVersion)); }

            if ( y is not AppVersion right ) { throw new ExpectedValueTypeException(nameof(y), y, typeof(AppVersion)); }

            return left.Equals(right);
        }

        int IEqualityComparer.GetHashCode( object obj ) => obj.GetHashCode();
    }
}
