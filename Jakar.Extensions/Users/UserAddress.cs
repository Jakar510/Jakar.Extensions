// Jakar.Extensions :: Jakar.Extensions
// 09/29/2023  10:20 PM

namespace Jakar.Extensions;


public interface IAddress<out TID> : IUniqueID<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public            string? Address         { get; }
    public            string  City            { get; }
    public            string  Country         { get; }
    public            bool    IsPrimary       { get; }
    public            string  Line1           { get; }
    public            string  Line2           { get; }
    [Required] public string  PostalCode      { get; }
    public            string  StateOrProvince { get; }
}



public interface IAddress<TClass, TID> : IAddress<TID>, IParsable<TClass>, IEqualComparable<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : class, IAddress<TClass, TID>
{
    public abstract static TClass Create( Match         match );
    public abstract static TClass Create( IAddress<TID> address );
    public abstract static TClass Create( string        line1, string line2, string city, string postalCode, string country );
}



[Serializable]
public abstract class UserAddress<TClass, TID> : ObservableClass<TClass>, IAddress<TID>, JsonModels.IJsonModel
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : UserAddress<TClass, TID>, IAddress<TClass, TID>, IEqualComparable<TClass>

{
    private bool                          _isPrimary;
    private IDictionary<string, JToken?>? _additionalData;
    private string                        _city            = string.Empty;
    private string                        _country         = string.Empty;
    private string                        _line1           = string.Empty;
    private string                        _line2           = string.Empty;
    private string                        _postalCode      = string.Empty;
    private string                        _stateOrProvince = string.Empty;
    private string?                       _address;
    private TID                           _id;


    [JsonExtensionData]                public IDictionary<string, JToken?>? AdditionalData { get => _additionalData;         set => SetProperty( ref _additionalData, value ); }
    [StringLength( UNICODE_CAPACITY )] public string?                       Address        { get => _address ??= ToString(); set => SetProperty( ref _address,        value ); }


    [StringLength( UNICODE_CAPACITY )]
    public string City
    {
        get => _city;
        set
        {
            if ( SetProperty( ref _city, value ) ) { Address = null; }
        }
    }


    [StringLength( UNICODE_CAPACITY )]
    public string Country
    {
        get => _country;
        set
        {
            if ( SetProperty( ref _country, value ) ) { Address = null; }
        }
    }


    public TID  ID        { get => _id;        init => SetProperty( ref _id,       value ); }
    public bool IsPrimary { get => _isPrimary; set => SetProperty( ref _isPrimary, value ); }


    [JsonIgnore]
    public bool IsValidAddress
    {
        get
        {
            ReadOnlySpan<char> address = Address;
            if ( address.IsEmpty ) { return false; }

            Span<char> span = stackalloc char[address.Length];
            address.CopyTo( span );

            for ( int i = 0; i < span.Length; i++ )
            {
                if ( char.IsLetterOrDigit( span[i] ) is false || char.IsPunctuation( span[i] ) ) { span[i] = ' '; }
            }

            return span.IsNullOrWhiteSpace();
        }
    }


    [StringLength( UNICODE_CAPACITY )]
    public string Line1
    {
        get => _line1;
        set
        {
            if ( SetProperty( ref _line1, value ) ) { Address = null; }
        }
    }


    [StringLength( UNICODE_CAPACITY )]
    public string Line2
    {
        get => _line2;
        set
        {
            if ( SetProperty( ref _line2, value ) ) { Address = null; }
        }
    }


    [Required, StringLength( UNICODE_CAPACITY )]
    public string PostalCode
    {
        get => _postalCode;
        set
        {
            if ( SetProperty( ref _postalCode, value ) ) { Address = null; }
        }
    }


    [StringLength( UNICODE_CAPACITY )]
    public string StateOrProvince
    {
        get => _stateOrProvince;
        set
        {
            if ( SetProperty( ref _stateOrProvince, value ) ) { Address = null; }
        }
    }


    protected UserAddress() { }
    protected UserAddress( IAddress<TID> address ) : base()
    {
        ID              = address.ID;
        Address         = address.Address;
        Line1           = address.Line1;
        Line2           = address.Line2;
        City            = address.City;
        StateOrProvince = address.StateOrProvince;
        Country         = address.Country;
        PostalCode      = address.PostalCode;
        IsPrimary       = address.IsPrimary;
    }
    protected UserAddress( Match match ) : this( match.Groups["StreetName"].Value, match.Groups["Apt"].Value, match.Groups["City"].Value, match.Groups["ZipCode"].Value, match.Groups["Country"].Value ) { }
    protected UserAddress( string line1, string line2, string city, string postalCode, string country, TID id = default )
    {
        ID         = id;
        Line1      = line1;
        Line2      = line2;
        City       = city;
        PostalCode = postalCode;
        Country    = country;
    }


    public override string ToString() => string.IsNullOrWhiteSpace( Line2 )
                                             ? $"{Line1}. {City}, {StateOrProvince}. {Country}. {PostalCode}"
                                             : $"{Line1} {Line2}. {City}, {StateOrProvince}. {Country}. {PostalCode}";


    public override int CompareTo( TClass? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int countryComparison = string.Compare( _country, other._country, StringComparison.Ordinal );
        if ( countryComparison != 0 ) { return countryComparison; }

        int stateOrProvinceComparison = string.Compare( _stateOrProvince, other._stateOrProvince, StringComparison.Ordinal );
        if ( stateOrProvinceComparison != 0 ) { return stateOrProvinceComparison; }

        int cityComparison = string.Compare( _city, other._city, StringComparison.Ordinal );
        if ( cityComparison != 0 ) { return cityComparison; }

        int postalCodeComparison = string.Compare( _postalCode, other._postalCode, StringComparison.Ordinal );
        if ( postalCodeComparison != 0 ) { return postalCodeComparison; }

        int line1Comparison = string.Compare( _line1, other._line1, StringComparison.Ordinal );
        if ( line1Comparison != 0 ) { return line1Comparison; }

        int line2Comparison = string.Compare( _line2, other._line2, StringComparison.Ordinal );
        if ( line2Comparison != 0 ) { return line2Comparison; }

        return 0;
    }
    public override bool Equals( TClass? other )
    {
        if ( other is null ) { return false; }

        return ReferenceEquals( this, other ) || string.Equals( Line1, other.Line1, StringComparison.Ordinal ) && string.Equals( Line2, other.Line2, StringComparison.Ordinal ) && string.Equals( City, other.City, StringComparison.Ordinal ) && string.Equals( PostalCode, other.PostalCode, StringComparison.Ordinal ) && string.Equals( StateOrProvince, other.StateOrProvince, StringComparison.Ordinal );
    }
}



[Serializable]
public sealed class UserAddress<TID> : UserAddress<UserAddress<TID>, TID>, IAddress<UserAddress<TID>, TID>, IEqualComparable<UserAddress<TID>>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public UserAddress() { }
    public UserAddress( Match                            match ) : base( match ) { }
    public UserAddress( IAddress<TID>                    address ) : base( address ) { }
    public UserAddress( string                           line1, string line2, string city, string postalCode, string country ) : base( line1, line2, city, postalCode, country ) { }
    public static UserAddress<TID> Create( Match         match )                                                               => new(match);
    public static UserAddress<TID> Create( IAddress<TID> address )                                                             => new(address);
    public static UserAddress<TID> Create( string        line1, string line2, string city, string postalCode, string country ) => new(line1, line2, city, postalCode, country);
    public new static UserAddress<TID> Parse( string value, IFormatProvider? provider )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        Match               match         = Validate.Re.Address.Match( value );
        return new UserAddress<TID>( match );
    }
    public new static bool TryParse( string? value, IFormatProvider? provider, [NotNullWhen( true )] out UserAddress<TID>? result )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        try
        {
            result = string.IsNullOrWhiteSpace( value ) is false
                         ? Parse( value, provider )
                         : null;

            return result is not null;
        }
        catch ( Exception e )
        {
            telemetrySpan.AddException( e );
            result = null;
            return false;
        }
    }


    public override bool Equals( object? other )                                        => other is UserAddress<TID> x && Equals( x );
    public override int  GetHashCode()                                                  => HashCode.Combine( Line1, Line2, City, PostalCode, Country, ID );
    public static   bool operator ==( UserAddress<TID>? left, UserAddress<TID>? right ) => Equalizer.Equals( left, right );
    public static   bool operator !=( UserAddress<TID>? left, UserAddress<TID>? right ) => Equalizer.Equals( left, right ) is false;
    public static   bool operator >( UserAddress<TID>   left, UserAddress<TID>  right ) => Sorter.GreaterThan( left, right );
    public static   bool operator >=( UserAddress<TID>  left, UserAddress<TID>  right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static   bool operator <( UserAddress<TID>   left, UserAddress<TID>  right ) => Sorter.LessThan( left, right );
    public static   bool operator <=( UserAddress<TID>  left, UserAddress<TID>  right ) => Sorter.LessThanOrEqualTo( left, right );
}
