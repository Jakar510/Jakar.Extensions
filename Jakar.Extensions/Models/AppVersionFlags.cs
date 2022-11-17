﻿// Jakar.Extensions :: Jakar.Extensions
// 10/20/2022  2:37 PM

namespace Jakar.Extensions;


public readonly struct AppVersionFlags : IEquatable<AppVersionFlags>, IEquatable<AppVersionFlags?>, IComparable<AppVersionFlags>, IComparable<AppVersionFlags?>, IComparable, IFormattable
{
    internal const char   SEPARATOR = '-';
    private const  string STABLE    = "";
    private const  string RC        = "rc";
    private const  string ALPHA     = "alpha";
    private const  string BETA      = "beta";


    public   string Flag       { get; init; }
    public   int    Iteration  { get; init; }
    internal int    Length     => Flag.Length + 15;
    public   bool   IsEmpty    => string.IsNullOrWhiteSpace( Flag );
    public   bool   IsNotEmpty => !IsEmpty;


    public AppVersionFlags( string flag, int iteration )
    {
        Flag      = flag;
        Iteration = iteration;
    }


    // public override string ToString()
    // {
    //     return Iteration > 0
    //                ? $"-{Flag}{Iteration}"
    //                : Flag;
    // }
    public override string ToString() => AsSpan()
       .ToString();
    public string ToString( string? format, IFormatProvider? formatProvider ) => AsSpan( format, formatProvider )
       .ToString();


    public ReadOnlySpan<char> AsSpan() => AsSpan( default,                           CultureInfo.CurrentCulture );
    public ReadOnlySpan<char> AsSpan( ReadOnlySpan<char> format ) => AsSpan( format, CultureInfo.CurrentCulture );
    public ReadOnlySpan<char> AsSpan( ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        Span<char> buffer = stackalloc char[Length];

        if ( TryFormat( buffer, out int charsWritten, format, provider ) )
        {
            buffer = buffer[..charsWritten];
            return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), buffer.Length );
        }

        throw new InvalidOperationException( "Conversion failed" );
    }


    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = default )
    {
        if ( destination.IsEmpty || destination.Length < Length ) { throw new ArgumentException( $"{nameof(destination)} is too small" ); }

        if ( IsEmpty )
        {
            charsWritten = 0;
            return true;
        }


        destination[0] = SEPARATOR;
        for ( int i = 0; i < Flag.Length; i++ ) { destination[i + 1] = Flag[i]; }


        charsWritten = Flag.Length + 1;

        if ( Iteration > 0 && Iteration.TryFormat( destination[charsWritten..], out int intCharsWritten, format, provider ) ) { charsWritten += intCharsWritten; }

        return true;
    }


    public static AppVersionFlags Default => Stable;
    public static AppVersionFlags Stable  => new(STABLE, 0);
    public static AppVersionFlags ReleaseCandidate( int iteration = 0 ) => new(RC, iteration);
    public static AppVersionFlags Alpha( int            iteration = 0 ) => new(ALPHA, iteration);
    public static AppVersionFlags Beta( int             iteration = 0 ) => new(BETA, iteration);


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

        if ( value.Contains( SEPARATOR ) )
        {
            int index = value.IndexOf( SEPARATOR );
            value = value[..index];

            ReadOnlySpan<char> span = value[(index - 1)..]
               .Trim( SEPARATOR );

            int end = span.IndexOfAny( Randoms.Numeric );

            return end == -1
                       ? new AppVersionFlags( span.ToString(), 0 )
                       : new AppVersionFlags( span[..end]
                                                 .ToString(),
                                              int.Parse( span[end..] ) );
        }


        return Stable;
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
                                               _      => string.Compare( Flag, other.Flag, StringComparison.OrdinalIgnoreCase ),
                                           },
                                 RC => other.Flag switch
                                       {
                                           STABLE => -1,
                                           RC     => 0,
                                           ALPHA  => 1,
                                           BETA   => 1,
                                           _      => string.Compare( Flag, other.Flag, StringComparison.OrdinalIgnoreCase ),
                                       },
                                 ALPHA => other.Flag switch
                                          {
                                              STABLE => -1,
                                              RC     => -1,
                                              ALPHA  => 0,
                                              BETA   => 1,
                                              _      => string.Compare( Flag, other.Flag, StringComparison.OrdinalIgnoreCase ),
                                          },
                                 BETA => other.Flag switch
                                         {
                                             STABLE => -1,
                                             RC     => -1,
                                             ALPHA  => -1,
                                             BETA   => 0,
                                             _      => string.Compare( Flag, other.Flag, StringComparison.OrdinalIgnoreCase ),
                                         },
                                 _ => string.Compare( Flag, other.Flag, StringComparison.Ordinal ),
                             };


        return flagComparison != 0
                   ? flagComparison
                   : Iteration.CompareTo( other.Iteration );
    }
    public int CompareTo( object? other ) => other switch
                                             {
                                                 null                  => 1,
                                                 AppVersionFlags flags => CompareTo( flags ),
                                                 _                     => throw new ArgumentException( $"Object must be of type {nameof(AppVersionFlags)}" ),
                                             };
    public bool Equals( AppVersionFlags? other )
    {
        if ( other is null ) { return false; }

        return Equals( other.Value );
    }
    public bool Equals( AppVersionFlags  other ) => string.Equals( Flag, other.Flag, StringComparison.OrdinalIgnoreCase ) && Iteration.Equals( other.Iteration );
    public override bool Equals( object? obj ) => obj is AppVersionFlags other && Equals( other );
    public override int GetHashCode() => Flag.GetHashCode();


    public static bool operator <( AppVersionFlags  left, AppVersionFlags right ) => left.CompareTo( right ) < 0;
    public static bool operator >( AppVersionFlags  left, AppVersionFlags right ) => left.CompareTo( right ) > 0;
    public static bool operator <=( AppVersionFlags left, AppVersionFlags right ) => left.CompareTo( right ) <= 0;
    public static bool operator >=( AppVersionFlags left, AppVersionFlags right ) => left.CompareTo( right ) >= 0;
    public static bool operator ==( AppVersionFlags left, AppVersionFlags right ) => left.Equals( right );
    public static bool operator !=( AppVersionFlags left, AppVersionFlags right ) => !left.Equals( right );

    public static bool operator <( AppVersionFlags?  left, AppVersionFlags? right ) => ValueSorter<AppVersionFlags>.Instance.Compare( left, right ) < 0;
    public static bool operator >( AppVersionFlags?  left, AppVersionFlags? right ) => ValueSorter<AppVersionFlags>.Instance.Compare( left, right ) > 0;
    public static bool operator <=( AppVersionFlags? left, AppVersionFlags? right ) => ValueSorter<AppVersionFlags>.Instance.Compare( left, right ) <= 0;
    public static bool operator >=( AppVersionFlags? left, AppVersionFlags? right ) => ValueSorter<AppVersionFlags>.Instance.Compare( left, right ) >= 0;
    public static bool operator ==( AppVersionFlags? left, AppVersionFlags? right ) => ValueEqualizer<AppVersionFlags>.Instance.Equals( left, right );
    public static bool operator !=( AppVersionFlags? left, AppVersionFlags? right ) => !ValueEqualizer<AppVersionFlags>.Instance.Equals( left, right );
}
