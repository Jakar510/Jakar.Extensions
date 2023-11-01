﻿namespace Jakar.Extensions;


/// <summary> See <see cref="Format"/> for formatting details. </summary>
[ Serializable, JsonConverter( typeof(AppVersionConverter) ) ]

// AppVersionNullableConverter
public sealed class AppVersion : IComparable,
                                 IComparable<AppVersion>,
                                 IFuzzyEquals<AppVersion>,
                                 IReadOnlyCollection<int>,
                                 ICloneable,
                             #if NETSTANDARD2_1
                                 IFormattable
#else
                                 ISpanFormattable
#endif
#if NET7_0_OR_GREATER
                                 ,
                                 IParsable<AppVersion>,
                                 ISpanParsable<AppVersion>
#endif
{
    public static AppVersion      Default       { get; } = new();
    public        Format          Scheme        { get; init; }
    public        int             Major         { get; init; }
    public        int?            Minor         { get; init; }
    public        int?            Maintenance   { get; init; }
    public        int?            MajorRevision { get; init; }
    public        int?            MinorRevision { get; init; }
    public        int?            Build         { get; init; }
    public        AppVersionFlags Flags         { get; init; }
    public        int             Length        => Flags.Length + 65;


    public AppVersion() : this( 0, default, default, default, default, default, AppVersionFlags.Stable ) { }
    public AppVersion( int major ) : this( major, default, default, default, default, default, AppVersionFlags.Stable ) { }
    public AppVersion( int major, int  minor ) : this( major, minor, default, default, default, default, AppVersionFlags.Stable ) { }
    public AppVersion( int major, int  minor, int  build ) : this( major, minor, default, default, default, build, AppVersionFlags.Stable ) { }
    public AppVersion( int major, int  minor, int  maintenance, int  build ) : this( major, minor, maintenance, default, default, build, AppVersionFlags.Stable ) { }
    public AppVersion( int major, int  minor, int  maintenance, int  majorRevision, int  build ) : this( major, minor, maintenance, majorRevision, default, build, AppVersionFlags.Stable ) { }
    public AppVersion( int major, int? minor, int? maintenance, int? majorRevision, int? minorRevision, int? build ) : this( major, minor, maintenance, majorRevision, minorRevision, build, AppVersionFlags.Stable ) { }
    public AppVersion( int major, int? minor, int? maintenance, int? majorRevision, int? minorRevision, int? build, AppVersionFlags flags )
    {
        Major         = major;
        Minor         = minor;
        Maintenance   = maintenance;
        Build         = build;
        MajorRevision = majorRevision;
        MinorRevision = minorRevision;
        Scheme        = GetFormat( Minor, Maintenance, MajorRevision, MinorRevision, Build );
        Flags         = flags;
    }
    public AppVersion( Version version )
    {
        Major         = version.Major;
        Minor         = version.Minor;
        Maintenance   = version.Revision;
        Build         = version.Build;
        MinorRevision = version.MinorRevision;
        MajorRevision = version.MajorRevision;
        Scheme        = GetFormat( Minor, Maintenance, MajorRevision, MinorRevision, Build );
        Flags         = AppVersionFlags.Stable;
    }
    public AppVersion( ReadOnlySpan<int> items, AppVersionFlags flags = default )
    {
        Flags  = flags;
        Scheme = (Format)items.Length;

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

            default: throw new OutOfRangeException( nameof(Scheme), Scheme, @"value doesn't contain the correct amount of items." );
        }
    }


#if NETSTANDARD2_1
    public AppVersion( List<int> items, AppVersionFlags flags = default )
    {
        if ( items is null ) { throw new ArgumentNullException( nameof(items) ); }

        Flags  = flags;
        Scheme = (Format)items.Count;

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

            default: throw new ArgumentOutOfRangeException( nameof(items), items.Count, @"value doesn't contain the correct amount of items." );
        }
    }

#else
    public AppVersion( List<int> items, AppVersionFlags flags = default ) : this( CollectionsMarshal.AsSpan( items ), flags ) { }

#endif


    public static implicit operator AppVersion( string            value ) => Parse( value );
    public static implicit operator AppVersion( Span<int>         value ) => new(value);
    public static implicit operator AppVersion( ReadOnlySpan<int> value ) => new(value);
    public static implicit operator AppVersion( Version           value ) => new(value);


    /// <summary> </summary>
    /// <param name="value"> </param>
    /// <param name="version"> </param>
    /// <returns> <see langword="true"/> Parse was successful. <br/> <see langword="false"/> otherwise. </returns>
    public static bool TryParse( ReadOnlySpan<char> value, [ NotNullWhen( true ) ] out AppVersion? version ) => TryParse( value, CultureInfo.CurrentCulture, out version );
    public static bool TryParse( ReadOnlySpan<char> value, IFormatProvider? provider, [ NotNullWhen( true ) ] out AppVersion? version )
    {
        if ( !value.IsEmpty )
        {
            try
            {
                version = Parse( value, provider );
                return true;
            }
            catch ( FormatException e ) { e.WriteToDebug(); }
            catch ( ArgumentException e ) { e.WriteToDebug(); }
        }

        version = default;
        return false;
    }
    public static AppVersion Parse( ReadOnlySpan<char> value )                            => Parse( value, CultureInfo.CurrentCulture );
    public static AppVersion Parse( ReadOnlySpan<char> value, IFormatProvider? provider ) => Parse( value, value, provider );
    private static AppVersion Parse( ReadOnlySpan<char> value, in ReadOnlySpan<char> original, IFormatProvider? provider )
    {
        try
        {
            if ( value.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof(value) ); }

            value.WriteToDebug();
            AppVersionFlags flags = AppVersionFlags.Parse( ref value );
            value.WriteToDebug();

            int count = value.Count( SEPARATOR );


            int        i          = 0;
            Span<int>  result     = stackalloc int[count + 1];
            Span<char> separators = stackalloc char[1];
            separators[0] = SEPARATOR;

            foreach ( ReadOnlySpan<char> item in value.SplitOn( separators ) )
            {
                if ( item.IsEmpty ) { continue; }

                Debug.Assert( !item.Contains( SEPARATOR ) );

                if ( int.TryParse( item, NumberStyles.Number, provider, out int n ) ) { result[i++] = n; }
                else
                {
                #if NETSTANDARD2_1
                    throw new FormatException( $"Cannot convert '{item.ToString()}' to an int." );
                #else
                    throw new FormatException( $"Cannot convert '{item}' to an int." );
                #endif
                }
            }

            return new AppVersion( result, flags );
        }
        catch ( Exception e ) { throw new ArgumentException( $"Cannot convert '{original.ToString()}' into {nameof(AppVersion)}", nameof(value), e ); }
    }
    public static AppVersion Parse( string s, IFormatProvider? provider ) => Parse( s.AsSpan() );
    public static bool TryParse( string? s, IFormatProvider? provider, [ NotNullWhen( true ) ] out AppVersion? result )
    {
        if ( string.IsNullOrEmpty( s ) )
        {
            result = default;
            return false;
        }

        result = Parse( s, provider );
        return true;
    }


    public static AppVersion FromAssembly<T>()                     => FromAssembly( typeof(T).Assembly );
    public static AppVersion FromAssembly( Type         type )     => FromAssembly( type.Assembly );
    public static AppVersion FromAssembly( Assembly     assembly ) => FromAssembly( assembly.GetName() );
    public static AppVersion FromAssembly( AssemblyName assembly ) => assembly.Version ?? throw new NullReferenceException( nameof(assembly.Version) );


    private static Format GetFormat( int? minor, int? maintenance, int? majorRevision, int? minorRevision, int? build ) =>
        minor.HasValue && maintenance.HasValue && majorRevision.HasValue && minorRevision.HasValue && build.HasValue
            ? Format.Complete
            : minor.HasValue && maintenance.HasValue && majorRevision.HasValue && build.HasValue
                ? Format.DetailedRevisions
                : minor.HasValue && maintenance.HasValue && build.HasValue
                    ? Format.Detailed
                    : minor.HasValue && build.HasValue
                        ? Format.Typical
                        : minor.HasValue
                            ? Format.Minimal
                            : Format.Singular;


    // ---------------------------------------------------------------------------------------------------------------------------------


    internal const char SEPARATOR = '.';


    public override string ToString() => ToString( default, CultureInfo.CurrentCulture );
    public string ToString( string? format, IFormatProvider? formatProvider )
    {
        Span<char> span = stackalloc char[Length + 1];
        if ( !TryFormat( span, out int charsWritten, format, formatProvider ) ) { throw new InvalidOperationException( "Conversion failed" ); }

        Span<char> result = span[..charsWritten];
        return result.ToString();
    }


#if NET6_0_OR_GREATER
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
#endif
    public bool TryFormat( Span<char> span, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = default )
    {
        Debug.Assert( span.Length > Length );
        charsWritten = 0;
        Span<char> numberBuffer = stackalloc char[20];

        foreach ( int value in this )
        {
            if ( value.TryFormat( numberBuffer, out int intCharsWritten, format, provider ) )
            {
                foreach ( char c in numberBuffer[..intCharsWritten] ) { span[charsWritten++] = c; }

                span[charsWritten++] = SEPARATOR;
            }
            else { throw new InvalidOperationException( "Conversion failed" ); }
        }


        Span<char> flags = span[--charsWritten..];
        if ( Flags.IsNotEmpty && Flags.TryFormat( flags, out int flagsCharsWritten, format, provider ) ) { charsWritten += flagsCharsWritten; }

        return true;
    }


    object ICloneable.Clone() => Clone();
    public AppVersion Clone() => new(Major, Minor, Maintenance, MajorRevision, MinorRevision, Build, Flags);


    /// <summary>
    ///     If the <see cref="Scheme"/> is any of [ <see cref="Format.Singular"/> , <see cref="Format.DetailedRevisions"/> , <see cref="Format.Complete"/> ], will throw
    ///     <see
    ///         cref="InvalidOperationException"/>
    /// </summary>
    /// <returns> </returns>
    /// <exception cref="InvalidOperationException"> </exception>
    public Version ToVersion() =>
        Scheme switch
        {
            Format.Singular          => new Version( Major, 0 ),
            Format.Minimal           => new Version( Major, Minor ?? 0 ),
            Format.Typical           => new Version( Major, Minor ?? 0, Build       ?? 0 ),
            Format.Detailed          => new Version( Major, Minor ?? 0, Maintenance ?? 0, Build ?? 0 ),
            Format.DetailedRevisions => new Version( Major, Minor ?? 0, Maintenance ?? 0, Build ?? 0 ),
            Format.Complete          => new Version( Major, Minor ?? 0, Maintenance ?? 0, Build ?? 0 ),
            _                        => throw new OutOfRangeException( nameof(Scheme), Scheme )
        };


    public IEnumerator<int> GetEnumerator()
    {
        yield return Major;

        if ( Minor.HasValue ) { yield return Minor.Value; }

        if ( Maintenance.HasValue ) { yield return Maintenance.Value; }

        if ( MajorRevision.HasValue ) { yield return MajorRevision.Value; }

        if ( MinorRevision.HasValue ) { yield return MinorRevision.Value; }

        if ( Build.HasValue ) { yield return Build.Value; }
    }
    IEnumerator IEnumerable.     GetEnumerator() => GetEnumerator();
    int IReadOnlyCollection<int>.Count           => Scheme.AsInt();


    // public static bool operator ==( AppVersion  left, AppVersion  right ) => Equalizer.Instance.Equals( left, right );
    // public static bool operator !=( AppVersion  left, AppVersion  right ) => !Equalizer.Instance.Equals( left, right );
    // public static bool operator >( AppVersion   left, AppVersion  right ) => Sorter.Instance.Compare( left, right ) > 0;
    // public static bool operator >=( AppVersion  left, AppVersion  right ) => Sorter.Instance.Compare( left, right ) >= 0;
    // public static bool operator <( AppVersion   left, AppVersion  right ) => Sorter.Instance.Compare( left, right ) < 0;
    // public static bool operator <=( AppVersion  left, AppVersion  right ) => Sorter.Instance.Compare( left, right ) <= 0;
    public static bool operator ==( AppVersion? left, AppVersion? right ) => Equalizer.Equals( left, right );
    public static bool operator !=( AppVersion? left, AppVersion? right ) => !Equalizer.Equals( left, right );
    public static bool operator >( AppVersion?  left, AppVersion? right ) => Sorter.Compare( left, right ) > 0;
    public static bool operator >=( AppVersion? left, AppVersion? right ) => Sorter.Compare( left, right ) >= 0;
    public static bool operator <( AppVersion?  left, AppVersion? right ) => Sorter.Compare( left, right ) < 0;
    public static bool operator <=( AppVersion? left, AppVersion? right ) => Sorter.Compare( left, right ) <= 0;


    private void AssertFormat( AppVersion other )
    {
        if ( Scheme == 0 || Scheme == other.Scheme ) { return; }

        throw new FormatException( $"{nameof(other)}.{nameof(Scheme)} is '{other.Scheme}' and expected '{Scheme}'" );
    }


    /// <summary> Compares two <see cref="AppVersion"/> instances </summary>
    /// <param name="other"> </param>
    /// <returns>
    ///     <see langword="0"/> if the <paramref name="other"/> is equivalent. <br/> <see langword="-1"/> if the <paramref name="other"/> is smaller. <br/> <see langword="1"/> if the <paramref name="other"/> is greater. <br/>
    ///     <see
    ///         langword="-2"/>
    ///     if the
    ///     <paramref
    ///         name="other"/>
    ///     is not an instance of
    ///     <see
    ///         cref="AppVersion"/>
    ///     . <br/>
    /// </returns>
    /// <exception cref="ArgumentException"> If something goes wrong, and the <paramref name="other"/> is not comparable. </exception>
    public int CompareTo( object? other ) =>
        other switch
        {
            Version version    => CompareTo( version ),
            AppVersion version => CompareTo( version ),
            _                  => throw new ExpectedValueTypeException( nameof(other), other, typeof(Version), typeof(AppVersion) )
        };


    public int CompareTo( AppVersion? other )
    {
        if ( other is null ) { return 1; }

        AssertFormat( other );

        int majorComparison = Major.CompareTo( other.Major );
        if ( majorComparison != 0 ) { return majorComparison; }

        int minorComparison = Nullable.Compare( Minor, other.Minor );
        if ( minorComparison != 0 ) { return minorComparison; }

        int maintenanceComparison = Nullable.Compare( Maintenance, other.Maintenance );
        if ( maintenanceComparison != 0 ) { return maintenanceComparison; }

        int majorRevisionComparison = Nullable.Compare( MajorRevision, other.MajorRevision );
        if ( majorRevisionComparison != 0 ) { return majorRevisionComparison; }

        int minorRevisionComparison = Nullable.Compare( MinorRevision, other.MinorRevision );
        if ( minorRevisionComparison != 0 ) { return minorRevisionComparison; }

        int buildComparison = Nullable.Compare( Build, other.Build );
        if ( buildComparison != 0 ) { return buildComparison; }

        return Flags.CompareTo( other.Flags );
    }


    /// <summary> Compares two <see cref="AppVersion"/> instances for relative equality. </summary>
    /// <param name="other"> </param>
    /// <returns>
    ///     <para>
    ///         returns <see langword="true"/> if all of the following is true:
    ///         <list type="number">
    ///             <item> If <see cref="Major"/> and <see cref="Minor"/> match </item> <br/>
    ///             <item>
    ///                 All other fields that are not <see langword="null"/> and are equal to or greater than <br/>
    ///                 <list type="bullet">
    ///                     <item> <see cref="Maintenance"/> </item> <br/>
    ///                     <item>
    ///                         <see
    ///                             cref="MajorRevision"/>
    ///                     </item>
    ///                     <br/>
    ///                     <item>
    ///                         <see
    ///                             cref="MinorRevision"/>
    ///                     </item>
    ///                     <br/>
    ///                     <item>
    ///                         <see
    ///                             cref="Build"/>
    ///                     </item>
    ///                     <br/>
    ///                 </list>
    ///             </item>
    ///             <br/>
    ///         </list>
    ///     </para>
    ///     <para> otherwise <see langword="false"/> </para>
    /// </returns>
    /// <exception cref="ArgumentException"> If something goes wrong, and the <paramref name="other"/> is not comparable. </exception>
    public bool FuzzyEquals( AppVersion? other )
    {
        if ( other is null ) { return false; }

        AssertFormat( other );

        bool result = Major.Equals( other.Major )                                 &&
                      Nullable.Compare( other.Minor,         Minor )         == 0 &&
                      Nullable.Compare( other.Maintenance,   Maintenance )   >= 0 &&
                      Nullable.Compare( other.MajorRevision, MajorRevision ) >= 0 &&
                      Nullable.Compare( other.MinorRevision, MinorRevision ) >= 0 &&
                      Nullable.Compare( other.Build,         Build )         >= 0;

        return result;
    }


    public bool Equals( AppVersion? other )
    {
        if ( other is null ) { return false; }

        AssertFormat( other );

        return Major == other.Major                                  &&
               Nullable.Equals( Minor,         other.Minor )         &&
               Nullable.Equals( Maintenance,   other.Maintenance )   &&
               Nullable.Equals( MajorRevision, other.MajorRevision ) &&
               Nullable.Equals( MinorRevision, other.MinorRevision ) &&
               Nullable.Equals( Build,         other.Build )         &&
               Flags.Equals( other.Flags );
    }
    public override bool Equals( object? obj ) => obj is AppVersion version && Equals( version );
    public override int  GetHashCode()         => HashCode.Combine( Scheme, Major, Minor, Maintenance, MajorRevision, MinorRevision, Build, Flags );


    // ---------------------------------------------------------------------------------------------------------------------------------



    public enum Format
    {
        /// <summary> Major </summary>
        Singular = 1,

        /// <summary> Major.Minor </summary>
        Minimal = 2,

        /// <summary> Major.Minor.Build </summary>
        Typical,

        /// <summary> Major.Minor.Maintenance.Build </summary>
        Detailed,

        /// <summary> Major.Minor.Maintenance.MajorRevision.Build </summary>
        DetailedRevisions,

        /// <summary> Major.Minor.Maintenance.MajorRevision.MinorRevision.Build </summary>
        Complete
    }



    public static Equalizer<AppVersion>      Equalizer             => Equalizer<AppVersion>.Default;
    public static Sorter<AppVersion>         Sorter                => Sorter<AppVersion>.Default;
    public static FuzzyEqualizer<AppVersion> FuzzyEqualityComparer => FuzzyEqualizer<AppVersion>.Default;
}
