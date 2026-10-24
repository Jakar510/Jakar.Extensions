// Jakar.Extensions :: Jakar.Extensions
// 4/2/2024  13:31

namespace Jakar.Extensions;


/// <summary>
///     The prerelease part of an <see cref="AppVersion"/>: a label (<c>alpha</c>, <c>beta</c>, <c>rc</c>, or any other) and an optional iteration, written SemVer style as
///     <c>-beta.1</c> (<c>10.1.0-beta.1</c>), or <c>-beta</c> when <see cref="Iteration"/> is 0. <see cref="Stable"/> (no label) is a release.
/// </summary>
/// <remarks>
///     <para> Parsing also accepts the older spellings: <c>-beta1</c>, <c>beta</c> / <c>rc</c> / <c>alpha</c> without the dash (<c>1.1rc</c>), and <c>a</c> / <c>b</c> for alpha / beta. </para>
///     <para> Ordering follows SemVer and NuGet: a prerelease comes before its release, labels compare alphabetically (<c>alpha</c> &lt; <c>beta</c> &lt; <c>rc</c>), then iterations numerically. </para>
/// </remarks>
[DefaultValue(nameof(Stable))]
public readonly struct AppVersionFlags( string flag, uint iteration ) : IEqualityOperators<AppVersionFlags>, IComparisonOperators<AppVersionFlags>, ISpanParsable<AppVersionFlags>, IFormattable
{
    private const           char               ITERATION_SEPARATOR = '.';
    private const           char               METADATA_SEPARATOR  = '+';
    private static readonly SearchValues<char> __letters           = SearchValues.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");


    public static readonly AppVersionFlags Stable    = new(EMPTY, 0);
    public readonly        string          Flag      = flag;
    public readonly        uint            Iteration = iteration;


    public bool IsEmpty    => string.IsNullOrWhiteSpace(Flag);
    public bool IsNotEmpty => !IsEmpty;
    public int  Length     => Flag.Length + 15;


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


    [MethodImpl(MethodImplOptions.AggressiveOptimization)] public bool TryFormat( Span<char> buffer, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null )
    {
        charsWritten = 0;
        if ( IsEmpty ) { return true; }

        // -label, then .iteration when there is one: too small a buffer is a false return, as ISpanFormattable expects.
        if ( buffer.Length < Flag.Length + 1 ) { return false; }

        buffer[0] = FLAG_SEPARATOR;
        Flag.CopyTo(buffer[1..]);
        int written = Flag.Length + 1;

        if ( Iteration > 0 )
        {
            if ( buffer.Length == written ) { return false; }

            buffer[written++] = ITERATION_SEPARATOR; // -beta.1
            if ( !Iteration.TryFormat(buffer[written..], out int intCharsWritten, default, CultureInfo.InvariantCulture) ) { return false; }

            written += intCharsWritten;
        }

        charsWritten = written;
        return true;
    }
    public static AppVersionFlags ReleaseCandidate( uint iteration = 0 ) => new(RC, iteration);
    public static AppVersionFlags Alpha( uint            iteration = 0 ) => new(ALPHA, iteration);
    public static AppVersionFlags Beta( uint             iteration = 0 ) => new(BETA, iteration);


    /// <summary>
    ///     Removes the prerelease part from the end of <paramref name="value"/> and returns it: everything after the first <c>-</c> (<c>10.1.0-beta.1</c>), or, without a dash, from the first
    ///     letter (<c>1.1rc</c>). SemVer build metadata (<c>+sha</c>) is dropped. <paramref name="value"/> keeps the numeric part; <see cref="Stable"/> when there's no prerelease part.
    /// </summary>
    /// <exception cref="FormatException"> The prerelease part isn't a label with an optional iteration (<c>beta</c>, <c>beta.1</c>, <c>beta1</c>). </exception>
    public static AppVersionFlags Parse( scoped ref ReadOnlySpan<char> value )
    {
        int metadata = value.IndexOf(METADATA_SEPARATOR);
        if ( metadata >= 0 ) { value = value[..metadata]; }

        int  end    = value.IndexOf(FLAG_SEPARATOR);
        bool dashed = end >= 0;
        if ( !dashed ) { end = value.IndexOfAny(__letters); } // 1.1rc, 1.1beta2

        if ( end < 0 ) { return Stable; }

        ReadOnlySpan<char> prerelease = value[( dashed ? end + 1 : end )..];
        value = value[..end];
        return ParsePrerelease(prerelease);
    }

    /// <summary> <c>beta</c>, <c>beta.1</c> or <c>beta1</c>: a label of letters, then an optional iteration. </summary>
    private static AppVersionFlags ParsePrerelease( ReadOnlySpan<char> prerelease )
    {
        int digits = prerelease.IndexOfAnyExcept(__letters);

        ReadOnlySpan<char> label = digits < 0
                                       ? prerelease
                                       : prerelease[..digits];

        ReadOnlySpan<char> number = digits < 0
                                        ? default
                                        : prerelease[digits..];

        if ( number.StartsWith(ITERATION_SEPARATOR) ) { number = number[1..]; }

        if ( label.IsEmpty ) { throw new FormatException($"'{prerelease}' has no prerelease label (e.g. beta.1)."); }

        uint iteration = 0;
        if ( !number.IsEmpty && !uint.TryParse(number, NumberStyles.None, CultureInfo.InvariantCulture, out iteration) ) { throw new FormatException($"'{prerelease}' isn't a prerelease label with an optional iteration (e.g. beta.1)."); }

        string flag = label switch
                      {
                          _ when label.Equals(ALPHA, StringComparison.OrdinalIgnoreCase) || label.Equals("a", StringComparison.OrdinalIgnoreCase) => ALPHA,
                          _ when label.Equals(BETA,  StringComparison.OrdinalIgnoreCase) || label.Equals("b", StringComparison.OrdinalIgnoreCase) => BETA,
                          _ when label.Equals(RC,    StringComparison.OrdinalIgnoreCase)                                                         => RC,
                          _                                                                                                                       => label.ToString()
                      };

        return new AppVersionFlags(flag, iteration);
    }
    public static AppVersionFlags Parse( string flag, IFormatProvider? provider )
    {
        ReadOnlySpan<char> span = flag;
        return Parse(ref span);
    }
    public static bool TryParse( string? flag, IFormatProvider? provider, out AppVersionFlags result )
    {
        if ( !string.IsNullOrEmpty(flag) ) { return TryParse(flag.AsSpan(), provider, out result); }

        result = default;
        return false;
    }
    public static AppVersionFlags Parse( ReadOnlySpan<char> flag, IFormatProvider? provider ) => Parse(ref flag);
    public static bool TryParse( ReadOnlySpan<char> flag, IFormatProvider? provider, out AppVersionFlags result )
    {
        try
        {
            result = Parse(flag, provider);
            return true;
        }
        catch ( FormatException )
        {
            result = default;
            return false;
        }
    }


    public int CompareTo( AppVersionFlags? other )
    {
        if ( other is null ) { return 1; }

        return CompareTo(other.Value);
    }
    /// <summary> SemVer / NuGet order: a release (<see cref="Stable"/>) follows every prerelease; labels compare alphabetically (<c>alpha</c> &lt; <c>beta</c> &lt; <c>rc</c>), then iterations numerically. </summary>
    public int CompareTo( AppVersionFlags other )
    {
        if ( IsEmpty ) { return other.IsEmpty ? 0 : 1; }

        if ( other.IsEmpty ) { return -1; }

        int labelComparison = string.Compare(Flag, other.Flag, StringComparison.OrdinalIgnoreCase);

        return labelComparison != 0
                   ? labelComparison
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
    public          bool Equals( AppVersionFlags             other ) => IsEmpty ? other.IsEmpty : string.Equals(Flag, other.Flag, StringComparison.OrdinalIgnoreCase) && Iteration.Equals(other.Iteration); // default and Stable are both a release
    public override int  GetHashCode()                               => HashCode.Combine(IsEmpty ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(Flag), Iteration); // matches Equals: case-insensitive label


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
