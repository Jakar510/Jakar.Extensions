namespace Jakar.Extensions;


/// <summary> See <see cref="Format"/> for formatting details. </summary>
[Serializable, JsonConverter( typeof(AppVersionJsonNetConverter) ), JsonObject]
public sealed class AppVersion : IComparable, IComparable<AppVersion>, IFuzzyEquals<AppVersion>, IReadOnlyCollection<int>, ICloneable, ISpanFormattable, ISpanParsable<AppVersion>
{
    private const          char                       SEPARATOR = '.';
    private                string?                    _string;
    public static readonly AppVersion                 Default = new();
    public static          Equalizer<AppVersion>      Equalizer             { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Equalizer<AppVersion>.Default; }
    public static          Sorter<AppVersion>         Sorter                { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Sorter<AppVersion>.Default; }
    public static          FuzzyEqualizer<AppVersion> FuzzyEqualityComparer { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => FuzzyEqualizer<AppVersion>.Default; }
    public                 Format                     Scheme                { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public                 int                        Major                 { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public                 int?                       Minor                 { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public                 int?                       Maintenance           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public                 int?                       MajorRevision         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public                 int?                       MinorRevision         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public                 int?                       Build                 { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public                 AppVersionFlags            Flags                 { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    [JsonIgnore] public    int                        Length                { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Flags.Length + 65; }


    static AppVersion() => JsonNet.Serializer.Converters.Add( AppVersionJsonNetConverter.Instance );
    public AppVersion() : this( 0, null, null, null, null, null, AppVersionFlags.Stable ) { }
    public AppVersion( int major ) : this( major, null, null, null, null, null, AppVersionFlags.Stable ) { }
    public AppVersion( int major, int  minor ) : this( major, minor, null, null, null, null, AppVersionFlags.Stable ) { }
    public AppVersion( int major, int  minor, int  build ) : this( major, minor, null, null, null, build, AppVersionFlags.Stable ) { }
    public AppVersion( int major, int  minor, int  maintenance, int  build ) : this( major, minor, maintenance, null, null, build, AppVersionFlags.Stable ) { }
    public AppVersion( int major, int  minor, int  maintenance, int  majorRevision, int  build ) : this( major, minor, maintenance, majorRevision, null, build, AppVersionFlags.Stable ) { }
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
    public AppVersion( AppVersionFlags flags, params ReadOnlySpan<int> span )
    {
        Flags  = flags;
        Scheme = (Format)span.Length;

        switch ( Scheme )
        {
            case Format.Singular:
                Major = span[0];
                return;

            case Format.Minimal:
                Major = span[0];
                Minor = span[1];
                return;

            case Format.Typical:
                Major = span[0];
                Minor = span[1];
                Build = span[2];
                return;

            case Format.Detailed:
                Major       = span[0];
                Minor       = span[1];
                Maintenance = span[2];
                Build       = span[3];
                return;

            case Format.DetailedRevisions:
                Major         = span[0];
                Minor         = span[1];
                Maintenance   = span[2];
                MajorRevision = span[3];
                Build         = span[4];
                return;

            case Format.Complete:
                Major         = span[0];
                Minor         = span[1];
                Maintenance   = span[2];
                MajorRevision = span[3];
                MinorRevision = span[4];
                Build         = span[5];
                return;

            default: throw OutOfRangeException.Create( Scheme, "value doesn't have the correct length." );
        }
    }


    public static implicit operator AppVersion( string            value ) => Parse( value );
    public static implicit operator AppVersion( Span<int>         value ) => new(AppVersionFlags.Stable, value);
    public static implicit operator AppVersion( ReadOnlySpan<int> value ) => new(AppVersionFlags.Stable, value);
    public static implicit operator AppVersion( Version           value ) => new(value);

    // TODO: [GeneratedRegex(@"^(\d+\.\d+\.\d+\.\d+)$", OPTIONS, 200 )] private static partial Regex GetParserRegex();


    /// <summary> </summary>
    /// <param name="value"> </param>
    /// <param name="version"> </param>
    /// <returns> <see langword="true"/> Parse was successful. <br/> <see langword="false"/> otherwise. </returns>
    public static bool TryParse( ReadOnlySpan<char> value, [NotNullWhen( true )] out AppVersion? version ) => TryParse( value, CultureInfo.CurrentCulture, out version );
    public static bool TryParse( ReadOnlySpan<char> value, IFormatProvider? provider, [NotNullWhen( true )] out AppVersion? version )
    {
        if ( value.IsEmpty is false )
        {
            try
            {
                version = Parse( value, provider );
                return true;
            }
            catch ( FormatException e ) { e.WriteToDebug(); }
            catch ( ArgumentException e ) { e.WriteToDebug(); }
        }

        version = null;
        return false;
    }
    public static bool TryParse( string? s, IFormatProvider? provider, [NotNullWhen( true )] out AppVersion? result )
    {
        if ( string.IsNullOrEmpty( s ) )
        {
            result = null;
            return false;
        }

        result = Parse( s, provider );
        return true;
    }


    public static AppVersion Parse( string                    s, IFormatProvider? provider ) => Parse( s.AsSpan(), provider );
    public static AppVersion Parse( scoped ReadOnlySpan<char> value )                            => Parse( value,     CultureInfo.CurrentUICulture );
    public static AppVersion Parse( scoped ReadOnlySpan<char> value, IFormatProvider? provider ) => Parse( ref value, value, provider );
    private static AppVersion Parse( scoped ref ReadOnlySpan<char> value, scoped in ReadOnlySpan<char> original, IFormatProvider? provider )
    {
        try
        {
            if ( value.IsNullOrWhiteSpace() ) { throw new ArgumentNullException( nameof(value) ); }

            if ( int.TryParse( value, NumberStyles.Integer, provider, out int n ) ) { return new AppVersion( n ); }

            AppVersionFlags    flags      = AppVersionFlags.Parse( ref value );
            int                count      = value.Count( SEPARATOR );
            Span<int>          result     = stackalloc int[count + 1];
            ReadOnlySpan<char> separators = [SEPARATOR];
            int                i          = 0;

            foreach ( ReadOnlySpan<char> span in value.SplitOn( separators ) )
            {
                if ( span.Length > 0 ) { result[i++] = int.Parse( span, NumberStyles.Number, provider ); }
            }

            return new AppVersion( flags, result );
        }
        catch ( Exception e ) { throw new ArgumentException( $"Cannot convert '{original}' into {nameof(AppVersion)}", nameof(value), e ); }
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


    public override string ToString() => ToString( null, CultureInfo.CurrentCulture );
    public string ToString( string? format, IFormatProvider? formatProvider )
    {
        if ( string.IsNullOrEmpty( _string ) is false ) { return _string; }

        Span<char> span = stackalloc char[Length + 1];
        if ( TryFormat( span, out int charsWritten, format, formatProvider ) is false ) { throw new InvalidOperationException( "Conversion failed" ); }

        Span<char> result = span[..charsWritten];
        return _string = result.ToString();
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public bool TryFormat( Span<char> span, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null )
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
            _                        => throw new OutOfRangeException( Scheme )
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
    public bool                  IsValid         => ReferenceEquals( this, Default ) is false && this != Default;


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
    ///     <see langword="0"/> if the <paramref name="other"/> is equivalent. <br/> <see langword="NOT_FOUND"/> if the <paramref name="other"/> is smaller. <br/> <see langword="1"/> if the
    ///     <paramref
    ///         name="other"/>
    ///     is greater. <br/>
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
    ///             <span> If <see cref="Major"/> and <see cref="Minor"/> match </span> <br/>
    ///             <span>
    ///                 All other fields that are not <see langword="null"/> and are equal to or greater than <br/>
    ///                 <list type="bullet">
    ///                     <span>
    ///                         <see cref="Maintenance"/>
    ///                     </span>
    ///                     <br/>
    ///                     <span>
    ///                         <see cref="MajorRevision"/>
    ///                     </span>
    ///                     <br/>
    ///                     <span>
    ///                         <see cref="MinorRevision"/>
    ///                     </span>
    ///                     <br/>
    ///                     <span>
    ///                         <see cref="Build"/>
    ///                     </span>
    ///                     <br/>
    ///                 </list>
    ///             </span>
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

        bool result = Major.Equals( other.Major ) && Nullable.Compare( other.Minor, Minor ) == 0 && Nullable.Compare( other.Maintenance, Maintenance ) >= 0 && Nullable.Compare( other.MajorRevision, MajorRevision ) >= 0 && Nullable.Compare( other.MinorRevision, MinorRevision ) >= 0 && Nullable.Compare( other.Build, Build ) >= 0;

        return result;
    }


    public bool Equals( AppVersion? other )
    {
        if ( other is null ) { return false; }

        AssertFormat( other );

        return Major == other.Major && Nullable.Equals( Minor, other.Minor ) && Nullable.Equals( Maintenance, other.Maintenance ) && Nullable.Equals( MajorRevision, other.MajorRevision ) && Nullable.Equals( MinorRevision, other.MinorRevision ) && Nullable.Equals( Build, other.Build ) && Flags.Equals( other.Flags );
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
        Typical = 3,

        /// <summary> Major.Minor.Maintenance.Build </summary>
        Detailed = 4,

        /// <summary> Major.Minor.Maintenance.MajorRevision.Build </summary>
        DetailedRevisions = 5,

        /// <summary> Major.Minor.Maintenance.MajorRevision.MinorRevision.Build </summary>
        Complete = 6
    }



    public class AppVersionJsonNetConverter : JsonConverter<AppVersion>
    {
        public static   AppVersionJsonNetConverter Instance { get; } = new();
        public override bool                       CanRead  => true;
        public override bool                       CanWrite => true;


        public override AppVersion? ReadJson( JsonReader reader, Type objectType, AppVersion? existingValue, bool hasExistingValue, JsonSerializer serializer ) => reader.Value is string version
                                                                                                                                                                       ? Parse( version )
                                                                                                                                                                       : existingValue;


        public override void WriteJson( JsonWriter writer, AppVersion? value, JsonSerializer serializer )
        {
            if ( value is null )
            {
                writer.WriteNull();
                return;
            }

            JToken token = value.ToString();
            token.WriteTo( writer );
        }
    }
}
