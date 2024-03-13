// Jakar.Extensions :: Jakar.Extensions
// 12/06/2023  11:46 AM

namespace Jakar.Extensions;


public record MsUserAddress : ObservableRecord, IAddress, IComparable<MsUserAddress>, IComparable, JsonModels.IJsonModel
{
    private bool                          _isPrimary;
    private Guid?                         _userID;
    private IDictionary<string, JToken?>? _additionalData;
    private string                        _city            = string.Empty;
    private string                        _country         = string.Empty;
    private string                        _line1           = string.Empty;
    private string                        _line2           = string.Empty;
    private string                        _postalCode      = string.Empty;
    private string                        _stateOrProvince = string.Empty;
    private string?                       _address;


    public static Sorter<MsUserAddress> Sorter => Sorter<MsUserAddress>.Default;


    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get => _additionalData; set => SetProperty( ref _additionalData, value ); }


    public string? Address { get => _address ??= ToString(); set => SetProperty( ref _address, value ); }


    public string City
    {
        get => _city;
        set
        {
            if ( SetProperty( ref _city, value ) ) { Address = null; }
        }
    }


    public string Country
    {
        get => _country;
        set
        {
            if ( SetProperty( ref _country, value ) ) { Address = null; }
        }
    }

    public bool IsPrimary { get => _isPrimary; set => SetProperty( ref _isPrimary, value ); }

    [JsonIgnore]
    public bool IsValidAddress
    {
        get
        {
            Span<char> span = stackalloc char[Address?.Length ?? 0];

            Address.AsSpan().CopyTo( span );

            for ( int i = 0; i < span.Length; i++ )
            {
                if ( char.IsLetterOrDigit( span[i] ) ) { continue; }

                span[i] = ' ';
            }

            return span.IsNullOrWhiteSpace();
        }
    }


    public string Line1
    {
        get => _line1;
        set
        {
            if ( SetProperty( ref _line1, value ) ) { Address = null; }
        }
    }


    public string Line2
    {
        get => _line2;
        set
        {
            if ( SetProperty( ref _line2, value ) ) { Address = null; }
        }
    }

    [Required]
    public string PostalCode
    {
        get => _postalCode;
        set
        {
            if ( SetProperty( ref _postalCode, value ) ) { Address = null; }
        }
    }


    public string StateOrProvince
    {
        get => _stateOrProvince;
        set
        {
            if ( SetProperty( ref _stateOrProvince, value ) ) { Address = null; }
        }
    }

    public Guid? UserID { get => _userID; set => SetProperty( ref _userID, value ); }


    public MsUserAddress() { }
    public MsUserAddress( IAddress address )
    {
        Address         = address.Address;
        Line1           = address.Line1;
        Line2           = address.Line2;
        City            = address.City;
        StateOrProvince = address.StateOrProvince;
        Country         = address.Country;
        PostalCode      = address.PostalCode;
        IsPrimary       = address.IsPrimary;
        UserID          = address.UserID;
    }


    public override string ToString() => string.IsNullOrWhiteSpace( Line2 )
                                             ? $"{Line1}. {City}, {StateOrProvince}. {Country}. {PostalCode}"
                                             : $"{Line1} {Line2}. {City}, {StateOrProvince}. {Country}. {PostalCode}";

    public int CompareTo( MsUserAddress? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int userIDCompare = Nullable.Compare( UserID, other.UserID );
        if ( userIDCompare != 0 ) { return userIDCompare; }

        int addressComparison = string.Compare( _address, other._address, StringComparison.Ordinal );
        if ( addressComparison != 0 ) { return addressComparison; }

        int cityComparison = string.Compare( _city, other._city, StringComparison.Ordinal );
        if ( cityComparison != 0 ) { return cityComparison; }

        int countryComparison = string.Compare( _country, other._country, StringComparison.Ordinal );
        if ( countryComparison != 0 ) { return countryComparison; }

        int line1Comparison = string.Compare( _line1, other._line1, StringComparison.Ordinal );
        if ( line1Comparison != 0 ) { return line1Comparison; }

        int line2Comparison = string.Compare( _line2, other._line2, StringComparison.Ordinal );
        if ( line2Comparison != 0 ) { return line2Comparison; }

        int postalCodeComparison = string.Compare( _postalCode, other._postalCode, StringComparison.Ordinal );
        if ( postalCodeComparison != 0 ) { return postalCodeComparison; }

        return string.Compare( _stateOrProvince, other._stateOrProvince, StringComparison.Ordinal );
    }
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        return other is MsUserAddress address
                   ? CompareTo( address )
                   : throw new ArgumentException( $"Object must be of type {nameof(MsUserAddress)}" );
    }


    public static bool operator <( MsUserAddress?  left, MsUserAddress? right ) => Sorter.Compare( left, right ) < 0;
    public static bool operator >( MsUserAddress?  left, MsUserAddress? right ) => Sorter.Compare( left, right ) > 0;
    public static bool operator <=( MsUserAddress? left, MsUserAddress? right ) => Sorter.Compare( left, right ) <= 0;
    public static bool operator >=( MsUserAddress? left, MsUserAddress? right ) => Sorter.Compare( left, right ) >= 0;
}
