// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/26/2022  2:52 PM

namespace Jakar.AppLogger.Portal.Data;


public sealed record Notification( string Message, string Type ) : IComparable<Notification>, IComparable
{
    public static Sorter<Notification>    Sorter    => Sorter<Notification>.Default;
    public static Equalizer<Notification> Equalizer => Equalizer<Notification>.Default;


    public int CompareTo( Notification? other )
    {
        if ( ReferenceEquals( this, other ) ) { return 0; }

        if ( ReferenceEquals( null, other ) ) { return 1; }

        int typeComparison = string.Compare( Type, other.Type, StringComparison.Ordinal );
        if ( typeComparison != 0 ) { return typeComparison; }

        return string.Compare( Message, other.Message, StringComparison.Ordinal );
    }
    public int CompareTo( object? obj )
    {
        if ( ReferenceEquals( null, obj ) ) { return 1; }

        if ( ReferenceEquals( this, obj ) ) { return 0; }

        return obj is Notification other
                   ? CompareTo( other )
                   : throw new ArgumentException( $"Object must be of type {nameof(Notification)}" );
    }


    public static bool operator <( Notification?  left, Notification? right ) => Comparer<Notification>.Default.Compare( left, right ) < 0;
    public static bool operator >( Notification?  left, Notification? right ) => Comparer<Notification>.Default.Compare( left, right ) > 0;
    public static bool operator <=( Notification? left, Notification? right ) => Comparer<Notification>.Default.Compare( left, right ) <= 0;
    public static bool operator >=( Notification? left, Notification? right ) => Comparer<Notification>.Default.Compare( left, right ) >= 0;
}
