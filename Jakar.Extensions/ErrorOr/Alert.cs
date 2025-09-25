// Jakar.Extensions :: Jakar.Extensions
// 09/22/2025  15:07

namespace Jakar.Extensions;


[Serializable, DefaultValue(nameof(Empty))]
public sealed class Alert() : BaseClass<Alert>, IJsonModel<Alert>
{
    public static readonly Alert Empty = new(null);
    public                 bool  IsNotValid => !CheckIsValid(Title, Message);


    public static JsonSerializerContext JsonContext   => JakarExtensionsContext.Default;
    public static JsonTypeInfo<Alert>   JsonTypeInfo  => JakarExtensionsContext.Default.Alert;
    public static JsonTypeInfo<Alert[]> JsonArrayInfo => JakarExtensionsContext.Default.AlertArray;
    public        bool                  IsValid       => CheckIsValid(Title, Message);
    public        string?               Message       { get; init; }
    public        string?               Title         { get; init; }
    public        TimeSpan?             TTL           { get; init; }


    public Alert( string? title, string? message, double ttlSeconds ) : this(title, message, TimeSpan.FromSeconds(ttlSeconds)) { }
    public Alert( string? title, string? message = null, TimeSpan? ttl = null ) : this()
    {
        Title   = title;
        Message = message;
        TTL     = ttl;
    }
    public static bool CheckIsValid( string? title, string? message ) => !string.IsNullOrWhiteSpace(title) || !string.IsNullOrWhiteSpace(message);

    public static implicit operator Alert( string? value ) => new(value);


    [Pure] public Error ToError() => new(null, null, Title, Message, null, StringTags.Empty);


    public override int CompareTo( Alert? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( other is null ) { return 1; }

        int titleComparison = string.Compare(Title, other.Title, StringComparison.InvariantCultureIgnoreCase);
        if ( titleComparison != 0 ) { return titleComparison; }

        int messageComparison = string.Compare(Message, other.Message, StringComparison.InvariantCultureIgnoreCase);
        if ( messageComparison != 0 ) { return messageComparison; }

        return Nullable.Compare(TTL, other.TTL);
    }
    public override bool Equals( Alert? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return Title == other.Title && Message == other.Message && Nullable.Equals(TTL, other.TTL);
    }
    public override bool Equals( object? obj )                    => ReferenceEquals(this, obj) || obj is Alert other && Equals(other);
    public override int  GetHashCode()                            => HashCode.Combine(Message, Title, TTL);
    public static   bool operator ==( Alert? left, Alert? right ) => EqualityComparer<Alert>.Default.Equals(left, right);
    public static   bool operator !=( Alert? left, Alert? right ) => !EqualityComparer<Alert>.Default.Equals(left, right);
    public static   bool operator >( Alert   left, Alert  right ) => Comparer<Alert>.Default.Compare(left, right) > 0;
    public static   bool operator >=( Alert  left, Alert  right ) => Comparer<Alert>.Default.Compare(left, right) >= 0;
    public static   bool operator <( Alert   left, Alert  right ) => Comparer<Alert>.Default.Compare(left, right) < 0;
    public static   bool operator <=( Alert  left, Alert  right ) => Comparer<Alert>.Default.Compare(left, right) <= 0;
}
