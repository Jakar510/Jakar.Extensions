#nullable enable
namespace Jakar.Extensions;


/// <summary>
///     See
///     <see cref = "Format" />
///     for formatting details.
/// </summary>
[Serializable]
[JsonConverter( typeof(AppVersionConverter) )] // AppVersionNullableConverter
[SuppressMessage( "ReSharper", "IntroduceOptionalParameters.Global" )]
[SuppressMessage( "ReSharper", "ParameterTypeCanBeEnumerable.Local" )]
[SuppressMessage( "ReSharper", "InconsistentNaming" )]
[SuppressMessage( "ReSharper", "RedundantDefaultMemberInitializer" )]
public readonly struct AppVersion : IComparable, IComparable<AppVersion>, IComparable<AppVersion?>, IFuzzyEquals<AppVersion?>, IFuzzyEquals<AppVersion>, IReadOnlyCollection<int>, ICloneable //TODO: ISpanFormattable
{
    public enum Format
    {
        /// <summary>
        ///     Major
        /// </summary>
        Singular = 1,

        /// <summary>
        ///     Major.Minor
        /// </summary>
        Minimal = 2,

        /// <summary>
        ///     Major.Minor.Build
        /// </summary>
        Typical,

        /// <summary>
        ///     Major.Minor.Maintenance.Build
        /// </summary>
        Detailed,

        /// <summary>
        ///     Major.Minor.Maintenance.MajorRevision.Build
        /// </summary>
        DetailedRevisions,

        /// <summary>
        ///     Major.Minor.Maintenance.MajorRevision.MinorRevision.Build
        /// </summary>
        Complete
    }



    public readonly Format           Scheme;
    public readonly int              Major;
    public readonly int?             Minor         = default;
    public readonly int?             Maintenance   = default;
    public readonly int?             MajorRevision = default;
    public readonly int?             MinorRevision = default;
    public readonly int?             Build         = default;
    private const   StringComparison COMPARISON    = StringComparison.OrdinalIgnoreCase;


    public AppVersion() : this( 0, null, null, null, null, null ) { }
    public AppVersion( int major ) : this( major, null, null, null, null, null ) { }
    public AppVersion( int major, int minor ) : this( major, minor, null, null, null, null ) { }
    public AppVersion( int major, int minor, int build ) : this( major, minor, null, null, null, build ) { }
    public AppVersion( int major, int minor, int maintenance, int build ) : this( major, minor, maintenance, null, null, build ) { }
    public AppVersion( int major, int minor, int maintenance, int majorRevision, int build ) : this( major, minor, maintenance, majorRevision, null, build ) { }

    public AppVersion( int major, int? minor, int? maintenance, int? majorRevision, int? minorRevision, int? build )
    {
        Major         = major;
        Minor         = minor;
        Maintenance   = maintenance;
        Build         = build;
        MajorRevision = majorRevision;
        MinorRevision = minorRevision;
        Scheme        = GetFormat( Minor, Maintenance, MajorRevision, MinorRevision, Build );
    }

    public AppVersion( in Version version )
    {
        Major         = version.Major;
        Minor         = version.Minor;
        Maintenance   = version.Revision;
        Build         = version.Build;
        MinorRevision = version.MinorRevision;
        MajorRevision = version.MajorRevision;
        Scheme        = GetFormat( Minor, Maintenance, MajorRevision, MinorRevision, Build );
    }

    public AppVersion( in Span<int>         items ) : this( items.ToArray() ) { }
    public AppVersion( in ReadOnlySpan<int> items ) : this( items.ToArray() ) { }

    public AppVersion( IReadOnlyList<int> items )
    {
        if (items is null) { throw new ArgumentNullException( nameof(items) ); }

        Scheme = (Format)items.Count;

        switch (Scheme)
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

            default: throw new ArgumentOutOfRangeException( nameof(items), items.Count, @"value doesn't contain the correct amount of items." );
        }
    }


    public static implicit operator AppVersion( string            value ) => Parse( value );
    public static implicit operator AppVersion( Span<int>         value ) => new(value);
    public static implicit operator AppVersion( ReadOnlySpan<int> value ) => new(value);
    public static implicit operator AppVersion( Version           value ) => new(value);


    /// <summary>
    /// 
    /// </summary>
    /// <param name = "value" > </param>
    /// <param name = "version" > </param>
    /// <returns>
    ///     <see langword = "true" />
    ///     if
    ///     <see cref = "Parse(string)" />
    ///     as successful.
    ///     <br />
    ///     <see langword = "false" />
    ///     otherwise.
    /// </returns>
    public static bool TryParse( in string? value, [NotNullWhen( true )] out AppVersion? version )
    {
        try
        {
            if (!string.IsNullOrWhiteSpace( value ))
            {
                version = Parse( value );
                return true;
            }
        }
        catch (FormatException) { }
        catch (ArgumentException) { }

        version = default;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name = "value" > </param>
    /// <exception cref = "FormatException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "OverflowException" > </exception>
    /// <exception cref = "ArgumentOutOfRangeException" > </exception>
    /// <returns>
    ///     <see cref = "AppVersion" />
    /// </returns>
    public static AppVersion Parse( string value ) => Parse( value.AsSpan() );

    /// <summary>
    /// 
    /// </summary>
    /// <param name = "value" > </param>
    /// <param name = "version" > </param>
    /// <returns>
    ///     <see langword = "true" />
    ///     if
    ///     <see cref = "Parse(ReadOnlySpan&lt;char&gt;)" />
    ///     as successful.
    ///     <br />
    ///     <see langword = "false" />
    ///     otherwise.
    /// </returns>
    public static bool TryParse( in ReadOnlySpan<char> value, [NotNullWhen( true )] out AppVersion? version )
    {
        try
        {
            if (!value.IsEmpty)
            {
                version = Parse( value );
                return true;
            }
        }
        catch (FormatException) { }
        catch (ArgumentException) { }

        version = default;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name = "value" > </param>
    /// <exception cref = "FormatException" > </exception>
    /// <exception cref = "ArgumentNullException" > </exception>
    /// <exception cref = "OverflowException" > </exception>
    /// <exception cref = "ArgumentOutOfRangeException" > </exception>
    /// <returns>
    ///     <see cref = "AppVersion" />
    /// </returns>
    public static AppVersion Parse( ReadOnlySpan<char> value ) => Parse( value, value );
    private static AppVersion Parse( ReadOnlySpan<char> value, in ReadOnlySpan<char> original )
    {
        if (value.IsEmpty) { throw new ArgumentNullException( nameof(value) ); }

        value.ToString()
             .WriteToConsole();

        try
        {
            // Option options;
            //
            // if ( value.EndsWith("-rc", COMPARISON) )
            // {
            //     value   = value[..value.IndexOf("-rc", COMPARISON)];
            //     options = Option.RC;
            // }
            // else if ( value.EndsWith("rc", COMPARISON) )
            // {
            //     value   = value[..value.IndexOf("rc", COMPARISON)];
            //     options = Option.RC;
            // }
            // else if ( value.EndsWith("-beta", COMPARISON) )
            // {
            //     value   = value[..value.IndexOf("-beta", COMPARISON)];
            //     options = Option.Beta;
            // }
            // else if ( value.EndsWith("-alpha", COMPARISON) )
            // {
            //     value   = value[..value.IndexOf("-alpha", COMPARISON)];
            //     options = Option.Alpha;
            // }
            // else if ( value.EndsWith("beta", COMPARISON) )
            // {
            //     value   = value[..value.IndexOf("beta", COMPARISON)];
            //     options = Option.Beta;
            // }
            // else if ( value.EndsWith("alpha", COMPARISON) )
            // {
            //     value   = value[..value.IndexOf("alpha", COMPARISON)];
            //     options = Option.Alpha;
            // }
            // else if ( value.EndsWith("-b", COMPARISON) )
            // {
            //     value   = value[..value.IndexOf("-b", COMPARISON)];
            //     options = Option.Beta;
            // }
            // else if ( value.EndsWith("b", COMPARISON) )
            // {
            //     value   = value[..value.IndexOf("b", COMPARISON)];
            //     options = Option.Beta;
            // }
            // else if ( value.EndsWith("-a", COMPARISON) )
            // {
            //     value   = value[..value.IndexOf("-a", COMPARISON)];
            //     options = Option.Alpha;
            // }
            // else if ( value.EndsWith("a", COMPARISON) )
            // {
            //     value   = value[..value.IndexOf("a", COMPARISON)];
            //     options = Option.Alpha;
            // }
            // else { options = Option.Stable; }

            var result = new List<int>();

            foreach ((ReadOnlySpan<char> item, _) in value.SplitOn( SEPARATOR ))
            {
                if (int.TryParse( item, NumberStyles.Any, CultureInfo.CurrentCulture, out int n )) { result.Add( n ); }
                else { throw new FormatException( $"Cannot convert '{item.ToString()}' to an int." ); }
            }

            return new AppVersion( result );
        }
        catch (Exception e) { throw new ArgumentException( $"Cannot convert '{original.ToString()}' into {nameof(AppVersion)}", nameof(value), e ); }
    }


    private static Format GetFormat( in int? minor, in int? maintenance, in int? majorRevision, in int? minorRevision, in int? build )
    {
        if (minor.HasValue && maintenance.HasValue && majorRevision.HasValue && minorRevision.HasValue && build.HasValue) { return Format.Complete; }

        if (minor.HasValue && maintenance.HasValue && majorRevision.HasValue && build.HasValue) { return Format.DetailedRevisions; }

        if (minor.HasValue && maintenance.HasValue && build.HasValue) { return Format.Detailed; }

        if (minor.HasValue && build.HasValue) { return Format.Typical; }

        if (minor.HasValue) { return Format.Minimal; }

        return Format.Singular;
    }


    // ---------------------------------------------------------------------------------------------------------------------------------


    public const char SEPARATOR = '.';

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendJoin( SEPARATOR, this );
        return sb.ToString();
    }


    public ReadOnlySpan<char> AsSpan() => AsSpan( CultureInfo.CurrentCulture );
    public ReadOnlySpan<char> AsSpan( in CultureInfo culture )
    {
        Span<char> buffer = stackalloc char[150];

        if (TryFormat( buffer, out int size, default, culture ))
        {
            char[] result = new char[size];

            buffer[..size]
               .CopyTo( result );

            return result;
        }

        throw new InvalidOperationException( "Conversion failed" );
    }

    public bool TryFormat( Span<char> destination, out int size, ReadOnlySpan<char> format = default, IFormatProvider? provider = default )
    {
        size = 0;

        Span<char> numberBuffer = stackalloc char[20];

        foreach (int value in this)
        {
            if (value.TryFormat( numberBuffer, out int charsWritten, format, provider ))
            {
                for (int i = 0; i < charsWritten; i++) { destination[i + size] = numberBuffer[i]; }

                size                += charsWritten;
                destination[size++] =  SEPARATOR;
            }
            else { throw new InvalidOperationException( "Conversion failed" ); }
        }

        size--;
        return true;
    }


    object ICloneable.Clone() => Clone();
    public AppVersion Clone() => Parse( AsSpan() );


    /// <summary>
    ///     If the
    ///     <see cref = "Scheme" />
    ///     is any of [
    ///     <see cref = "Format.Singular" />
    ///     ,
    ///     <see cref = "Format.DetailedRevisions" />
    ///     ,
    ///     <see cref = "Format.Complete" />
    ///     ], will throw
    ///     <see cref = "InvalidOperationException" />
    /// </summary>
    /// <returns> </returns>
    /// <exception cref = "InvalidOperationException" > </exception>
    public Version ToVersion()
    {
        if (Scheme is Format.Singular or Format.DetailedRevisions or Format.Complete) { throw new InvalidOperationException( "Conversion is not possible" ); }

        return Version.Parse( AsSpan() );
    }


    public IEnumerator<int> GetEnumerator()
    {
        yield return Major;

        if (Minor.HasValue) { yield return Minor.Value; }

        if (Maintenance.HasValue) { yield return Maintenance.Value; }

        if (MajorRevision.HasValue) { yield return MajorRevision.Value; }

        if (MinorRevision.HasValue) { yield return MinorRevision.Value; }

        if (Build.HasValue) { yield return Build.Value; }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    int IReadOnlyCollection<int>.Count => Scheme.AsInt();


    public static bool operator ==( in AppVersion left, in AppVersion right ) => Equalizer.Instance.Equals( left, right );
    public static bool operator !=( in AppVersion left, in AppVersion right ) => !Equalizer.Instance.Equals( left, right );
    public static bool operator >( in  AppVersion left, in AppVersion right ) => Sorter.Instance.Compare( left, right ) > 0;
    public static bool operator >=( in AppVersion left, in AppVersion right ) => Sorter.Instance.Compare( left, right ) >= 0;
    public static bool operator <( in  AppVersion left, in AppVersion right ) => Sorter.Instance.Compare( left, right ) < 0;
    public static bool operator <=( in AppVersion left, in AppVersion right ) => Sorter.Instance.Compare( left, right ) <= 0;


    public static bool operator ==( in AppVersion? left, in AppVersion? right ) => Equalizer.Instance.Equals( left, right );
    public static bool operator !=( in AppVersion? left, in AppVersion? right ) => !Equalizer.Instance.Equals( left, right );
    public static bool operator >( in  AppVersion? left, in AppVersion? right ) => Sorter.Instance.Compare( left, right ) > 0;
    public static bool operator >=( in AppVersion? left, in AppVersion? right ) => Sorter.Instance.Compare( left, right ) >= 0;
    public static bool operator <( in  AppVersion? left, in AppVersion? right ) => Sorter.Instance.Compare( left, right ) < 0;
    public static bool operator <=( in AppVersion? left, in AppVersion? right ) => Sorter.Instance.Compare( left, right ) <= 0;


    private void AssertFormat( in AppVersion other )
    {
        if (Scheme == 0 || Scheme == other.Scheme) { return; }

        throw new FormatException( $"{nameof(other)}.{nameof(Scheme)} is '{other.Scheme}' and expected '{Scheme}'" );
    }


    /// <summary>
    ///     Compares two
    ///     <see cref = "AppVersion" />
    ///     instances
    /// </summary>
    /// <param name = "other" > </param>
    /// <returns>
    ///     <see langword = "0" />
    ///     if the
    ///     <paramref name = "other" />
    ///     is equivalent.
    ///     <br />
    ///     <see langword = "-1" />
    ///     if the
    ///     <paramref name = "other" />
    ///     is smaller.
    ///     <br />
    ///     <see langword = "1" />
    ///     if the
    ///     <paramref name = "other" />
    ///     is greater.
    ///     <br />
    ///     <see langword = "-2" />
    ///     if the
    ///     <paramref name = "other" />
    ///     is not an instance of
    ///     <see cref = "AppVersion" />
    ///     .
    ///     <br />
    /// </returns>
    /// <exception cref = "ArgumentException" >
    ///     If something goes wrong, and the
    ///     <paramref name = "other" />
    ///     is not comparable.
    /// </exception>
    public int CompareTo( object? other ) =>
        other switch
        {
            Version version    => CompareTo( version ),
            AppVersion version => CompareTo( version ),
            _                  => throw new ExpectedValueTypeException( nameof(other), other, typeof(Version), typeof(AppVersion) )
        };


    /// <summary>
    ///     Compares two
    ///     <see cref = "AppVersion" />
    ///     instances
    /// </summary>
    /// <param name = "other" > </param>
    /// <returns>
    ///     <see langword = "0" />
    ///     if the
    ///     <paramref name = "other" />
    ///     is equivalent.
    ///     <br />
    ///     <see langword = "-1" />
    ///     if the
    ///     <paramref name = "other" />
    ///     is smaller.
    ///     <br />
    ///     <see langword = "1" />
    ///     if the
    ///     <paramref name = "other" />
    ///     is greater.
    ///     <br />
    /// </returns>
    /// <exception cref = "ArgumentException" >
    ///     If something goes wrong, and the
    ///     <paramref name = "other" />
    ///     is not comparable.
    /// </exception>
    public int CompareTo( AppVersion? other )
    {
        if (other is null) { return -1; }

        return CompareTo( other.Value );
    }

    public int CompareTo( AppVersion other )
    {
        AssertFormat( other );

        int majorComparison = Major.CompareTo( other.Major );
        if (majorComparison != 0) { return majorComparison; }

        int minorComparison = Nullable.Compare( Minor, other.Minor );
        if (minorComparison != 0) { return minorComparison; }

        int maintenanceComparison = Nullable.Compare( Maintenance, other.Maintenance );
        if (maintenanceComparison != 0) { return maintenanceComparison; }

        int majorRevisionComparison = Nullable.Compare( MajorRevision, other.MajorRevision );
        if (majorRevisionComparison != 0) { return majorRevisionComparison; }

        int minorRevisionComparison = Nullable.Compare( MinorRevision, other.MinorRevision );
        if (minorRevisionComparison != 0) { return minorRevisionComparison; }

        return Nullable.Compare( Build, other.Build );
    }


    /// <summary>
    ///     Compares two
    ///     <see cref = "AppVersion" />
    ///     instances for relative equality.
    /// </summary>
    /// <param name = "other" > </param>
    /// <returns>
    ///     <para>
    ///         returns
    ///         <see langword = "true" />
    ///         if all of the following is true:
    ///         <list type = "number" >
    ///             <item>
    ///                 If
    ///                 <see cref = "Major" />
    ///                 and
    ///                 <see cref = "Minor" />
    ///                 match
    ///             </item>
    ///             <br />
    ///             <item>
    ///                 All other fields that are not
    ///                 <see langword = "null" />
    ///                 and are equal to or greater than
    ///                 <br />
    ///                 <list type = "bullet" >
    ///                     <item>
    ///                         <see cref = "Maintenance" />
    ///                     </item>
    ///                     <br />
    ///                     <item>
    ///                         <see cref = "MajorRevision" />
    ///                     </item>
    ///                     <br />
    ///                     <item>
    ///                         <see cref = "MinorRevision" />
    ///                     </item>
    ///                     <br />
    ///                     <item>
    ///                         <see cref = "Build" />
    ///                     </item>
    ///                     <br />
    ///                 </list>
    ///             </item>
    ///             <br />
    ///         </list>
    ///     </para>
    ///     <para>
    ///         otherwise
    ///         <see langword = "false" />
    ///     </para>
    /// </returns>
    /// <exception cref = "ArgumentException" >
    ///     If something goes wrong, and the
    ///     <paramref name = "other" />
    ///     is not comparable.
    /// </exception>
    public bool FuzzyEquals( AppVersion? other )
    {
        if (other is null) { return false; }

        return FuzzyEquals( other.Value );
    }

    /// <summary>
    ///     Compares two
    ///     <see cref = "AppVersion" />
    ///     instances for relative equality.
    /// </summary>
    /// <param name = "other" > </param>
    /// <returns>
    ///     <para>
    ///         returns
    ///         <see langword = "true" />
    ///         if all of the following is true:
    ///         <list type = "number" >
    ///             <item>
    ///                 If
    ///                 <see cref = "Major" />
    ///                 and
    ///                 <see cref = "Minor" />
    ///                 match
    ///             </item>
    ///             <br />
    ///             <item>
    ///                 All other fields that are not
    ///                 <see langword = "null" />
    ///                 and are equal to or greater than
    ///                 <br />
    ///                 <list type = "bullet" >
    ///                     <item>
    ///                         <see cref = "Maintenance" />
    ///                     </item>
    ///                     <br />
    ///                     <item>
    ///                         <see cref = "MajorRevision" />
    ///                     </item>
    ///                     <br />
    ///                     <item>
    ///                         <see cref = "MinorRevision" />
    ///                     </item>
    ///                     <br />
    ///                     <item>
    ///                         <see cref = "Build" />
    ///                     </item>
    ///                     <br />
    ///                 </list>
    ///             </item>
    ///             <br />
    ///         </list>
    ///     </para>
    ///     <para>
    ///         otherwise
    ///         <see langword = "false" />
    ///     </para>
    /// </returns>
    /// <exception cref = "ArgumentException" >
    ///     If something goes wrong, and the
    ///     <paramref name = "other" />
    ///     is not comparable.
    /// </exception>
    public bool FuzzyEquals( AppVersion other )
    {
        AssertFormat( other );

        bool result = Major.Equals( other.Major ) && Nullable.Compare( other.Minor, Minor ) == 0 && Nullable.Compare( other.Maintenance,   Maintenance ) >= 0 && Nullable.Compare( other.MajorRevision, MajorRevision ) >= 0 &&
                      Nullable.Compare( other.MinorRevision,                        MinorRevision ) >= 0 && Nullable.Compare( other.Build, Build ) >= 0;

        return result;
    }


    public bool Equals( AppVersion? other )
    {
        if (other is null) { return false; }

        return Equals( other.Value );
    }

    public bool Equals( AppVersion other )
    {
        AssertFormat( other );

        return Major == other.Major && Nullable.Equals( Minor, other.Minor ) && Nullable.Equals( Maintenance, other.Maintenance ) && Nullable.Equals( MajorRevision, other.MajorRevision ) && Nullable.Equals( MinorRevision, other.MinorRevision ) &&
               Nullable.Equals( Build,                         other.Build );
    }


    public override bool Equals( object? obj ) => obj is AppVersion version && Equals( version );

    public override int GetHashCode() => HashCode.Combine( Scheme, Major, Minor, Maintenance, MajorRevision, MinorRevision, Build );


    // ---------------------------------------------------------------------------------------------------------------------------------



    public sealed class Equalizer : ValueEqualizer<AppVersion> { }



    public sealed class Sorter : ValueSorter<AppVersion> { }



    public sealed class FuzzyEqualityComparer : ValueFuzzyEqualizer<AppVersion> { }
}
