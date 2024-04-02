// Jakar.Extensions :: Jakar.Extensions
// 4/2/2024  13:31

namespace Jakar.Extensions;


[DefaultValue( nameof(Stable) )]
public readonly record struct AppVersionFlags( string Flag, uint Iteration ) :
#if NET7_0_OR_GREATER
    IEquatable<AppVersionFlags?>,
    IComparable<AppVersionFlags>,
    IComparable<AppVersionFlags?>,
    ISpanParsable<AppVersionFlags>,
    IComparable,
    IFormattable
#else
        IEquatable<AppVersionFlags?>,
        IComparable<AppVersionFlags>,
        IComparable<AppVersionFlags?>,
        IComparable,
        IFormattable
#endif
{
    private const string ALPHA          = "alpha";
    private const string BETA           = "beta";
    private const string RC             = "rc";
    private const char   FLAG_SEPARATOR = '-';
    private const string STABLE         = "";


    public static AppVersionFlags Stable     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(STABLE, 0); }
    public        bool            IsEmpty    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => string.IsNullOrWhiteSpace( Flag ); }
    public        bool            IsNotEmpty { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => !IsEmpty; }
    public        int             Length     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Flag.Length + 15; }


    public override string ToString()                                                  => AsSpan().ToString();
    public          string ToString( string? format, IFormatProvider? formatProvider ) => AsSpan( format, formatProvider ).ToString();


    public ReadOnlySpan<char> AsSpan()                            => AsSpan( default, CultureInfo.CurrentCulture );
    public ReadOnlySpan<char> AsSpan( ReadOnlySpan<char> format ) => AsSpan( format,  CultureInfo.CurrentCulture );
    public ReadOnlySpan<char> AsSpan( ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        Span<char> buffer = AsyncLinq.GetArray<char>( Length );
        if ( !TryFormat( buffer, out int charsWritten, format, provider ) ) { throw new InvalidOperationException( "Conversion failed" ); }

        return buffer[..charsWritten];
    }


#if NET6_0_OR_GREATER
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
#endif
    public bool TryFormat( Span<char> span, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = default )
    {
        Debug.Assert( span.Length > Length );

        if ( IsEmpty )
        {
            charsWritten = 0;
            return true;
        }


        charsWritten         = 0;
        span[charsWritten++] = FLAG_SEPARATOR;
        foreach ( char t in Flag ) { span[charsWritten++] = t; }

        if ( Iteration > 0 && Iteration.TryFormat( span[charsWritten..], out int intCharsWritten, format, provider ) ) { charsWritten += intCharsWritten; }

        span.WriteToDebug();
        return true;
    }
    public static AppVersionFlags ReleaseCandidate( uint iteration = 0 ) => new(RC, iteration);
    public static AppVersionFlags Alpha( uint            iteration = 0 ) => new(ALPHA, iteration);
    public static AppVersionFlags Beta( uint             iteration = 0 ) => new(BETA, iteration);


    public static AppVersionFlags Parse( ref ReadOnlySpan<char> value, StringComparison comparison = StringComparison.OrdinalIgnoreCase )
    {
        if ( value.EndsWith( "-rc", comparison ) )
        {
            value = value[..value.IndexOf( "-rc", comparison )];
            return ReleaseCandidate();
        }

        if ( value.EndsWith( "rc", comparison ) )
        {
            value = value[..value.IndexOf( "rc", comparison )];
            return ReleaseCandidate();
        }

        if ( value.EndsWith( "-beta", comparison ) )
        {
            value = value[..value.IndexOf( "-beta", comparison )];
            return Beta();
        }

        if ( value.EndsWith( "-alpha", comparison ) )
        {
            value = value[..value.IndexOf( "-alpha", comparison )];
            return Alpha();
        }

        if ( value.EndsWith( "beta", comparison ) )
        {
            value = value[..value.IndexOf( "beta", comparison )];
            return Beta();
        }

        if ( value.EndsWith( "alpha", comparison ) )
        {
            value = value[..value.IndexOf( "alpha", comparison )];
            return Alpha();
        }

        if ( value.EndsWith( "-b", comparison ) )
        {
            value = value[..value.IndexOf( "-b", comparison )];
            return Beta();
        }

        if ( value.EndsWith( "b", comparison ) )
        {
            value = value[..value.IndexOf( "b", comparison )];
            return Beta();
        }

        if ( value.EndsWith( "-a", comparison ) )
        {
            value = value[..value.IndexOf( "-a", comparison )];
            return Alpha();
        }

        if ( value.EndsWith( "a", comparison ) )
        {
            value = value[..value.IndexOf( "a", comparison )];
            return Alpha();
        }

        if ( value.Contains( FLAG_SEPARATOR ) is false ) { return Stable; }

        int                index = value.IndexOf( FLAG_SEPARATOR );
        ReadOnlySpan<char> span  = value[(index - 1)..].Trim( FLAG_SEPARATOR );
        value = value[..index];

        int end = span.IndexOfAny( Randoms.Numeric.Span );

        return end < 0
                   ? new AppVersionFlags( span.ToString(),        0 )
                   : new AppVersionFlags( span[..end].ToString(), uint.Parse( span[end..] ) );
    }
    public static AppVersionFlags Parse( string s, IFormatProvider? provider )
    {
        ReadOnlySpan<char> span = s;
        return Parse( ref span );
    }
    public static bool TryParse( string? s, IFormatProvider? provider, out AppVersionFlags result )
    {
        if ( string.IsNullOrEmpty( s ) )
        {
            result = default;
            return false;
        }

        result = Parse( s, provider );
        return true;
    }
    public static AppVersionFlags Parse( ReadOnlySpan<char> s, IFormatProvider? provider ) => Parse( ref s );
    public static bool TryParse( ReadOnlySpan<char> s, IFormatProvider? provider, out AppVersionFlags result )
    {
        result = Parse( s, provider );
        return true;
    }


    public int CompareTo( AppVersionFlags? other )
    {
        if ( other is null ) { return 1; }

        return CompareTo( other.Value );
    }
    public int CompareTo( AppVersionFlags other )
    {
        int flagComparison = Flag switch
                             {
                                 STABLE => other.Flag switch
                                           {
                                               STABLE => 0,
                                               RC     => 1,
                                               ALPHA  => 1,
                                               BETA   => 1,
                                               _      => string.Compare( Flag, other.Flag, StringComparison.OrdinalIgnoreCase )
                                           },
                                 RC => other.Flag switch
                                       {
                                           STABLE => -1,
                                           RC     => 0,
                                           ALPHA  => 1,
                                           BETA   => 1,
                                           _      => string.Compare( Flag, other.Flag, StringComparison.OrdinalIgnoreCase )
                                       },
                                 ALPHA => other.Flag switch
                                          {
                                              STABLE => -1,
                                              RC     => -1,
                                              ALPHA  => 0,
                                              BETA   => 1,
                                              _      => string.Compare( Flag, other.Flag, StringComparison.OrdinalIgnoreCase )
                                          },
                                 BETA => other.Flag switch
                                         {
                                             STABLE => -1,
                                             RC     => -1,
                                             ALPHA  => -1,
                                             BETA   => 0,
                                             _      => string.Compare( Flag, other.Flag, StringComparison.OrdinalIgnoreCase )
                                         },
                                 _ => string.Compare( Flag, other.Flag, StringComparison.Ordinal )
                             };


        return flagComparison != 0
                   ? flagComparison
                   : Iteration.CompareTo( other.Iteration );
    }
    public int CompareTo( object? other ) => other switch
                                             {
                                                 null                  => 1,
                                                 AppVersionFlags flags => CompareTo( flags ),
                                                 _                     => throw new ArgumentException( $"Object must be of type {nameof(AppVersionFlags)}" )
                                             };
    public bool Equals( AppVersionFlags? other )
    {
        if ( other is null ) { return false; }

        return Equals( other.Value );
    }
    public          bool Equals( AppVersionFlags other ) => string.Equals( Flag, other.Flag, StringComparison.OrdinalIgnoreCase ) && Iteration.Equals( other.Iteration );
    public override int  GetHashCode()                   => Flag.GetHashCode();


    public static bool operator <( AppVersionFlags   left, AppVersionFlags  right ) => left.CompareTo( right )                                     < 0;
    public static bool operator >( AppVersionFlags   left, AppVersionFlags  right ) => left.CompareTo( right )                                     > 0;
    public static bool operator <=( AppVersionFlags  left, AppVersionFlags  right ) => left.CompareTo( right )                                     <= 0;
    public static bool operator >=( AppVersionFlags  left, AppVersionFlags  right ) => left.CompareTo( right )                                     >= 0;
    public static bool operator <( AppVersionFlags?  left, AppVersionFlags? right ) => ValueSorter<AppVersionFlags>.Default.Compare( left, right ) < 0;
    public static bool operator >( AppVersionFlags?  left, AppVersionFlags? right ) => ValueSorter<AppVersionFlags>.Default.Compare( left, right ) > 0;
    public static bool operator <=( AppVersionFlags? left, AppVersionFlags? right ) => ValueSorter<AppVersionFlags>.Default.Compare( left, right ) <= 0;
    public static bool operator >=( AppVersionFlags? left, AppVersionFlags? right ) => ValueSorter<AppVersionFlags>.Default.Compare( left, right ) >= 0;
    public static bool operator ==( AppVersionFlags? left, AppVersionFlags? right ) => ValueEqualizer<AppVersionFlags>.Default.Equals( left, right );
    public static bool operator !=( AppVersionFlags? left, AppVersionFlags? right ) => ValueEqualizer<AppVersionFlags>.Default.Equals( left, right ) is false;
}
