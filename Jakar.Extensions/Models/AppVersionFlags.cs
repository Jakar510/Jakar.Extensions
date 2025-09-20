// Jakar.Extensions :: Jakar.Extensions
// 4/2/2024  13:31

namespace Jakar.Extensions;


[DefaultValue(nameof(Stable))]
public readonly struct AppVersionFlags( string flag, uint iteration ) : IEqualityOperators<AppVersionFlags>, IComparisonOperators<AppVersionFlags>, ISpanParsable<AppVersionFlags>, IFormattable
{
    private const          string          ALPHA          = "alpha";
    private const          string          BETA           = "beta";
    private const          char            FLAG_SEPARATOR = '-';
    private const          string          RC             = "rc";
    private const          string          STABLE         = "";
    public static readonly AppVersionFlags Stable         = new(STABLE, 0);
    public readonly        string          Flag           = flag;
    public readonly        uint            Iteration      = iteration;


    public bool IsEmpty    { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => string.IsNullOrWhiteSpace(Flag); }
    public bool IsNotEmpty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => !IsEmpty; }
    public int  Length     { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Flag.Length + 15; }


    public override string ToString()                                                  => AsSpan().ToString();
    public          string ToString( string? format, IFormatProvider? formatProvider ) => AsSpan(format, formatProvider).ToString();


    public ReadOnlySpan<char> AsSpan()                            => AsSpan(default, CultureInfo.CurrentCulture);
    public ReadOnlySpan<char> AsSpan( ReadOnlySpan<char> format ) => AsSpan(format,  CultureInfo.CurrentCulture);
    public ReadOnlySpan<char> AsSpan( ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        Span<char> buffer = GC.AllocateUninitializedArray<char>(Length);
        if ( !TryFormat(buffer, out int charsWritten, format, provider) ) { throw new InvalidOperationException("Conversion failed"); }

        return buffer[..charsWritten];
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public bool TryFormat( Span<char> buffer, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null )
    {
        Debug.Assert(buffer.Length > Length);

        if ( IsEmpty )
        {
            charsWritten = 0;
            return true;
        }


        charsWritten           = 0;
        buffer[charsWritten++] = FLAG_SEPARATOR;
        foreach ( char t in Flag ) { buffer[charsWritten++] = t; }

        if ( Iteration > 0 && Iteration.TryFormat(buffer[charsWritten..], out int intCharsWritten, format, provider) ) { charsWritten += intCharsWritten; }

        buffer.WriteToDebug();
        return true;
    }
    public static AppVersionFlags ReleaseCandidate( uint iteration = 0 ) => new(RC, iteration);
    public static AppVersionFlags Alpha( uint            iteration = 0 ) => new(ALPHA, iteration);
    public static AppVersionFlags Beta( uint             iteration = 0 ) => new(BETA, iteration);


    public static AppVersionFlags Parse( scoped ref ReadOnlySpan<char> value )
    {
        if ( value.EndsWith("-rc", StringComparison.OrdinalIgnoreCase) )
        {
            value = value[..value.IndexOf("-rc", StringComparison.OrdinalIgnoreCase)];
            return ReleaseCandidate();
        }

        if ( value.EndsWith("rc", StringComparison.OrdinalIgnoreCase) )
        {
            value = value[..value.IndexOf("rc", StringComparison.OrdinalIgnoreCase)];
            return ReleaseCandidate();
        }

        if ( value.EndsWith("-beta", StringComparison.OrdinalIgnoreCase) )
        {
            value = value[..value.IndexOf("-beta", StringComparison.OrdinalIgnoreCase)];
            return Beta();
        }

        if ( value.EndsWith("-alpha", StringComparison.OrdinalIgnoreCase) )
        {
            value = value[..value.IndexOf("-alpha", StringComparison.OrdinalIgnoreCase)];
            return Alpha();
        }

        if ( value.EndsWith("beta", StringComparison.OrdinalIgnoreCase) )
        {
            value = value[..value.IndexOf("beta", StringComparison.OrdinalIgnoreCase)];
            return Beta();
        }

        if ( value.EndsWith("alpha", StringComparison.OrdinalIgnoreCase) )
        {
            value = value[..value.IndexOf("alpha", StringComparison.OrdinalIgnoreCase)];
            return Alpha();
        }

        if ( value.EndsWith("-b", StringComparison.OrdinalIgnoreCase) )
        {
            value = value[..value.IndexOf("-b", StringComparison.OrdinalIgnoreCase)];
            return Beta();
        }

        if ( value.EndsWith("b", StringComparison.OrdinalIgnoreCase) )
        {
            value = value[..value.IndexOf("b", StringComparison.OrdinalIgnoreCase)];
            return Beta();
        }

        if ( value.EndsWith("-a", StringComparison.OrdinalIgnoreCase) )
        {
            value = value[..value.IndexOf("-a", StringComparison.OrdinalIgnoreCase)];
            return Alpha();
        }

        if ( value.EndsWith("a", StringComparison.OrdinalIgnoreCase) )
        {
            value = value[..value.IndexOf("a", StringComparison.OrdinalIgnoreCase)];
            return Alpha();
        }

        if ( !value.Contains(FLAG_SEPARATOR) ) { return Stable; }

        int                index = value.IndexOf(FLAG_SEPARATOR);
        ReadOnlySpan<char> flag  = value[( index - 1 )..];
        flag  = flag.Trim(FLAG_SEPARATOR);
        value = value[..index];

        int end = flag.IndexOfAny(Randoms.Numeric);

        return end < 0
                   ? new AppVersionFlags(flag.ToString(),        0)
                   : new AppVersionFlags(flag[..end].ToString(), uint.Parse(flag[end..]));
    }
    public static AppVersionFlags Parse( string flag, IFormatProvider? provider )
    {
        ReadOnlySpan<char> span = flag;
        return Parse(ref span);
    }
    public static bool TryParse( string? flag, IFormatProvider? provider, out AppVersionFlags result )
    {
        if ( string.IsNullOrEmpty(flag) )
        {
            result = default;
            return false;
        }

        result = Parse(flag, provider);
        return true;
    }
    public static AppVersionFlags Parse( ReadOnlySpan<char> flag, IFormatProvider? provider ) => Parse(ref flag);
    public static bool TryParse( ReadOnlySpan<char> flag, IFormatProvider? provider, out AppVersionFlags result )
    {
        result = Parse(flag, provider);
        return true;
    }


    public int CompareTo( AppVersionFlags? other )
    {
        if ( other is null ) { return 1; }

        return CompareTo(other.Value);
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
                                               _      => string.Compare(Flag, other.Flag, StringComparison.OrdinalIgnoreCase)
                                           },
                                 RC => other.Flag switch
                                       {
                                           STABLE => -1,
                                           RC     => 0,
                                           ALPHA  => 1,
                                           BETA   => 1,
                                           _      => string.Compare(Flag, other.Flag, StringComparison.OrdinalIgnoreCase)
                                       },
                                 ALPHA => other.Flag switch
                                          {
                                              STABLE => -1,
                                              RC     => -1,
                                              ALPHA  => 0,
                                              BETA   => 1,
                                              _      => string.Compare(Flag, other.Flag, StringComparison.OrdinalIgnoreCase)
                                          },
                                 BETA => other.Flag switch
                                         {
                                             STABLE => -1,
                                             RC     => -1,
                                             ALPHA  => -1,
                                             BETA   => 0,
                                             _      => string.Compare(Flag, other.Flag, StringComparison.OrdinalIgnoreCase)
                                         },
                                 _ => string.Compare(Flag, other.Flag, StringComparison.Ordinal)
                             };


        return flagComparison != 0
                   ? flagComparison
                   : Iteration.CompareTo(other.Iteration);
    }
    public int CompareTo( object? other ) => other switch
                                             {
                                                 null                  => 1,
                                                 AppVersionFlags flags => CompareTo(flags),
                                                 _                     => throw new ArgumentException($"Object must be of type {nameof(AppVersionFlags)}")
                                             };
    public bool Equals( AppVersionFlags? other )
    {
        if ( other is null ) { return false; }

        AppVersionFlags flags = other.Value;
        return Equals(flags);
    }
    public override bool Equals( [NotNullWhen(true)] object? other ) => other is AppVersionFlags flags                                      && Equals(flags);
    public          bool Equals( AppVersionFlags             other ) => string.Equals(Flag, other.Flag, StringComparison.OrdinalIgnoreCase) && Iteration.Equals(other.Iteration);
    public override int  GetHashCode()                               => Flag.GetHashCode();


    public static bool operator ==( AppVersionFlags? left, AppVersionFlags? right ) => Nullable.Equals(left, right);
    public static bool operator !=( AppVersionFlags? left, AppVersionFlags? right ) => !Nullable.Equals(left, right);
    public static bool operator ==( AppVersionFlags  left, AppVersionFlags  right ) => left.Equals(right);
    public static bool operator !=( AppVersionFlags  left, AppVersionFlags  right ) => !left.Equals(right);
    public static bool operator <( AppVersionFlags   left, AppVersionFlags  right ) => Comparer<AppVersionFlags>.Default.Compare(left, right) < 0;
    public static bool operator >( AppVersionFlags   left, AppVersionFlags  right ) => Comparer<AppVersionFlags>.Default.Compare(left, right) > 0;
    public static bool operator <=( AppVersionFlags  left, AppVersionFlags  right ) => Comparer<AppVersionFlags>.Default.Compare(left, right) <= 0;
    public static bool operator >=( AppVersionFlags  left, AppVersionFlags  right ) => Comparer<AppVersionFlags>.Default.Compare(left, right) >= 0;
    public static bool operator <( AppVersionFlags?  left, AppVersionFlags? right ) => Nullable.Compare(left, right)                          < 0;
    public static bool operator >( AppVersionFlags?  left, AppVersionFlags? right ) => Nullable.Compare(left, right)                          > 0;
    public static bool operator <=( AppVersionFlags? left, AppVersionFlags? right ) => Nullable.Compare(left, right)                          <= 0;
    public static bool operator >=( AppVersionFlags? left, AppVersionFlags? right ) => Nullable.Compare(left, right)                          >= 0;
}
