namespace Jakar.Extensions.Models.Users;


public interface IUserAddress
{
    string Line1      { get; }
    string Line2      { get; }
    string City       { get; }
    string State      { get; }
    string Country    { get; }
    string PostalCode { get; }
    bool   IsPrimary  { get; }
}



[Serializable]
public abstract class BaseUserAddress<T> : BaseCollections<T>, IUserAddress where T : BaseUserAddress<T>
{
    private string _city    = string.Empty;
    private string _country = string.Empty;
    private bool   _isPrimary;
    private string _line1      = string.Empty;
    private string _line2      = string.Empty;
    private string _postalCode = string.Empty;
    private string _state      = string.Empty;


    protected BaseUserAddress() { }

    protected BaseUserAddress( IUserAddress address )
    {
        Line1      = address.Line1;
        Line2      = address.Line2;
        City       = address.City;
        State      = address.State;
        Country    = address.Country;
        PostalCode = address.PostalCode;
    }


    // public AddressOptions ToAddressOptions() => new()
    //                                             {
    //                                                 City       = City,
    //                                                 Country    = Country,
    //                                                 Line1      = Line1,
    //                                                 Line2      = Line2,
    //                                                 PostalCode = PostalCode,
    //                                                 State      = State
    //                                             };
    //
    // public Address ToStripeAddress() => new()
    //                                     {
    //                                         Line1      = Line1,
    //                                         Line2      = Line2,
    //                                         City       = City,
    //                                         State      = State,
    //                                         Country    = Country,
    //                                         PostalCode = PostalCode
    //                                     };


    public override string ToString() => string.IsNullOrWhiteSpace(Line2)
                                             ? $"{Line1}. {City}, {State}. {Country}. {PostalCode}"
                                             : $"{Line1} {Line2}. {City}, {State}. {Country}. {PostalCode}";


    public override bool Equals( T? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return _line1 == other.Line1 && _line2 == other.Line2 && _city == other._city && _state == other._state && _country == other._country && _postalCode == other._postalCode;
    }

    public override int GetHashCode() => HashCode.Combine(_line1, _line2, _city, _state, _country, _postalCode);

    public override int CompareTo( T? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }


        int line1Compare = string.CompareOrdinal(_line1, other._line1);
        if ( line1Compare != 0 ) { return line1Compare; }


        int line2Compare = string.CompareOrdinal(_line2, other._line2);
        if ( line2Compare != 0 ) { return line2Compare; }


        int cityCompare = string.CompareOrdinal(_city, other._city);
        if ( cityCompare != 0 ) { return cityCompare; }


        int stateCompare = string.CompareOrdinal(_state, other._state);
        if ( stateCompare != 0 ) { return stateCompare; }


        int countryCompare = string.CompareOrdinal(_country, other._country);
        if ( countryCompare != 0 ) { return countryCompare; }


        return string.CompareOrdinal(_postalCode, other._postalCode);
    }


    public string Line1
    {
        get => _line1;
        set => SetProperty(ref _line1, value);
    }


    public string Line2
    {
        get => _line2;
        set => SetProperty(ref _line2, value);
    }


    public string City
    {
        get => _city;
        set => SetProperty(ref _city, value);
    }


    public string State
    {
        get => _state;
        set => SetProperty(ref _state, value);
    }


    public string Country
    {
        get => _country;
        set => SetProperty(ref _country, value);
    }


    public string PostalCode
    {
        get => _postalCode;
        set => SetProperty(ref _postalCode, value);
    }


    public bool IsPrimary
    {
        get => _isPrimary;
        set => SetProperty(ref _isPrimary, value);
    }
}
