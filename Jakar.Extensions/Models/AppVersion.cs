﻿namespace Jakar.Extensions.Models;


/// <summary>
/// See <see cref="Format"/>  for formatting details.
/// </summary>
[Serializable]
[JsonObject]
[JsonConverter(typeof(AppVersionConverter))]
[SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
[SuppressMessage("ReSharper", "ParameterTypeCanBeEnumerable.Local")]
public readonly struct AppVersion : IComparable, IComparable<AppVersion>, IComparable<AppVersion?>, IEquatable<AppVersion?>, IEquatable<AppVersion>, IReadOnlyCollection<int>
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



    public int  Major         { get; init; }
    public int? Minor         { get; init; }
    public int? Maintenance   { get; init; }
    public int? MajorRevision { get; init; }
    public int? MinorRevision { get; init; }
    public int? Build         { get; init; }


    public AppVersion() : this(0) { }

    public AppVersion( Version version )
    {
        Major         = version.Major;
        Minor         = version.Minor;
        Maintenance   = version.Revision;
        Build         = version.Build;
        MinorRevision = version.MinorRevision;
        MajorRevision = version.MajorRevision;
    }

    public AppVersion( int major ) : this(major, null) { }
    public AppVersion( int major, int? minor ) : this(major, minor, null) { }
    public AppVersion( int major, int? minor, int? build ) : this(major, minor, null, build) { }

    public AppVersion( int major, int? minor, int? maintenance, int? build ) : this(major,
                                                                                    minor,
                                                                                    maintenance,
                                                                                    null,
                                                                                    build) { }

    public AppVersion( int major, int? minor, int? maintenance, int? majorRevision, int? build ) : this(major,
                                                                                                        minor,
                                                                                                        maintenance,
                                                                                                        majorRevision,
                                                                                                        null,
                                                                                                        build) { }

    public AppVersion( int major, int? minor, int? maintenance, int? majorRevision, int? minorRevision, int? build )
    {
        Major         = major;
        Minor         = minor;
        Maintenance   = maintenance;
        Build         = build;
        MajorRevision = majorRevision;
        MinorRevision = minorRevision;
    }

    public AppVersion( Span<int>         items ) : this((IReadOnlyList<int>)items.ToArray()) { }
    public AppVersion( ReadOnlySpan<int> items ) : this((IReadOnlyList<int>)items.ToArray()) { }

    public AppVersion( IReadOnlyList<int> items )
    {
        if ( items is null ) { throw new ArgumentNullException(nameof(items)); }

        Minor         = default;
        Maintenance   = default;
        Build         = default;
        MajorRevision = default;
        MinorRevision = default;


        switch ( items.Count )
        {
            case (int)Format.Singular:
                Major = items[0];
                return;

            case (int)Format.Minimal:
                Major = items[0];
                Minor = items[1];
                return;

            case (int)Format.Typical:
                Major = items[0];
                Minor = items[1];
                Build = items[2];
                return;

            case (int)Format.Detailed:
                Major       = items[0];
                Minor       = items[1];
                Maintenance = items[2];
                Build       = items[3];
                return;

            case (int)Format.DetailedRevisions:
                Major         = items[0];
                Minor         = items[1];
                Maintenance   = items[2];
                MajorRevision = items[3];
                Build         = items[4];
                return;

            case (int)Format.Complete:
                Major         = items[0];
                Minor         = items[1];
                Maintenance   = items[2];
                MajorRevision = items[3];
                MinorRevision = items[4];
                Build         = items[5];
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(items), @"value doesn't contain the correct amount of items.");
        }
    }


    public static implicit operator AppVersion( string            value ) => Parse(value);
    public static implicit operator AppVersion( Span<int>         value ) => new(value);
    public static implicit operator AppVersion( ReadOnlySpan<int> value ) => new(value);
    public static implicit operator AppVersion( Version           value ) => new(value);
    public static implicit operator AppVersion( List<int>         value ) => new(value);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="version"></param>
    /// <returns>
    /// <see langword="true"/> if <see cref="Parse"/> as successful.
    /// <br/>
    /// <see langword="false"/> otherwise.
    /// </returns>
    public static bool TryParse( string? value, [NotNullWhen(true)] out AppVersion? version )
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
        catch ( ArgumentNullException ) { }
        catch ( ArgumentOutOfRangeException ) { }

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
    public static AppVersion Parse( string value )
    {
        if ( string.IsNullOrWhiteSpace(value) ) { throw new ArgumentNullException(nameof(value)); }

        if ( !value.Contains(SEPARATOR) ) { throw new FormatException($"value \"{value}\" doesn't contain any periods."); }

        // StringSplitOptions.RemoveEmptyEntries
        IReadOnlyList<int> items = value.Split(SEPARATOR).Select(int.Parse).ToList();

        return new AppVersion(items);
    }


    // ---------------------------------------------------------------------------------------------------------------------------------


    public const char SEPARATOR = '.';

    public override string ToString()
    {
        string s = this.Aggregate(string.Empty, ( current, item ) => current + $"{item}{SEPARATOR}");

        return s.TrimEnd(SEPARATOR);
    }


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

    [JsonIgnore] public int Count => this.Count();


    public Format GetFormat() => (Format)Count;


    public static bool operator ==( in AppVersion left, in AppVersion right )
    {
        if ( left.GetFormat() != right.GetFormat() ) { return false; }

        return left.Equals(right);
    }

    public static bool operator !=( in AppVersion left, in AppVersion right )
    {
        if ( left.GetFormat() != right.GetFormat() ) { return false; }

        return !left.Equals(right);
    }


    public static bool operator >( in AppVersion left, in AppVersion right )
    {
        if ( left.GetFormat() != right.GetFormat() ) { return false; }

        return left.CompareTo(right) > 0;
    }

    public static bool operator >=( in AppVersion left, in AppVersion right )
    {
        if ( left.GetFormat() != right.GetFormat() ) { return false; }

        return left.CompareTo(right) >= 0;
    }


    public static bool operator <( in AppVersion left, in AppVersion right )
    {
        if ( left.GetFormat() != right.GetFormat() ) { return false; }

        return left.CompareTo(right) < 0;
    }

    public static bool operator <=( in AppVersion left, in AppVersion right )
    {
        if ( left.GetFormat() != right.GetFormat() ) { return false; }

        return left.CompareTo(right) <= 0;
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
        ToString().WriteToConsole();
        other.ToString().WriteToConsole();


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

    public bool Equals( AppVersion other ) => Major == other.Major &&
                                              Nullable.Equals(Minor, other.Minor) &&
                                              Nullable.Equals(Maintenance, other.Maintenance) &&
                                              Nullable.Equals(MajorRevision, other.MajorRevision) &&
                                              Nullable.Equals(MinorRevision, other.MinorRevision) &&
                                              Nullable.Equals(Build, other.Build);


    public override bool Equals( object obj ) => obj is AppVersion version && Equals(version);

    public override int GetHashCode() => HashCode.Combine(Major, Minor, Maintenance, MajorRevision, MinorRevision, Build);


    // ---------------------------------------------------------------------------------------------------------------------------------



    private sealed class RelationalComparer : IComparer<AppVersion?>, IComparer<AppVersion>, IComparer
    {
        public static RelationalComparer Instance { get; } = new();


        public int Compare( AppVersion? left, AppVersion? right ) => Nullable.Compare(left, right);
        public int Compare( AppVersion  left, AppVersion  right ) => left.CompareTo(right);


        public int Compare( object x, object y )
        {
            if ( x is not AppVersion left ) { throw new ExpectedValueTypeException(nameof(x), x, typeof(AppVersion)); }

            if ( y is not AppVersion right ) { throw new ExpectedValueTypeException(nameof(y), y, typeof(AppVersion)); }

            return left.CompareTo(right);
        }
    }



    public sealed class EqualityComparer : IEqualityComparer<AppVersion>, IEqualityComparer<AppVersion?>, IEqualityComparer
    {
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



    public sealed class FuzzyEqualityComparer : IEqualityComparer<AppVersion>, IEqualityComparer<AppVersion?>, IEqualityComparer
    {
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
