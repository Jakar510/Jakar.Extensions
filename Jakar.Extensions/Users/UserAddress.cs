// Jakar.Extensions :: Jakar.Extensions
// 09/29/2023  10:20 PM

namespace Jakar.Extensions;


public interface IAddress<out TID> : IUniqueID<TID>, IJsonModel
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



public interface IAddress<TSelf, TID> : IAddress<TID>, IParsable<TSelf>, IJsonModel<TSelf>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TSelf : class, IAddress<TSelf, TID>
{
    public abstract static TSelf Create( Match         match );
    public abstract static TSelf Create( IAddress<TID> address );
    public abstract static TSelf Create( string        line1, string line2, string city, string stateOrProvince, string postalCode, string country, TID id = default );
}



[Serializable]
public abstract class UserAddress<TSelf, TID> : BaseClass<TSelf>, IAddress<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TSelf : UserAddress<TSelf, TID>, IAddress<TSelf, TID>, IJsonModel<TSelf>, IEqualComparable<TSelf>
{
    private bool    __isPrimary;
    private string  __city            = EMPTY;
    private string  __country         = EMPTY;
    private string  __line1           = EMPTY;
    private string  __line2           = EMPTY;
    private string  __postalCode      = EMPTY;
    private string  __stateOrProvince = EMPTY;
    private string? __address;
    private TID     __id;


    [Required] [StringLength(ADDRESS)] public string? Address { get => __address ??= ToString(); set => SetProperty(ref __address, value); }

    [Required] [StringLength(CITY)] public string City
    {
        get => __city;
        set
        {
            if ( SetProperty(ref __city, value) ) { Address = null; }
        }
    }

    [Required] [StringLength(COUNTRY)] public string Country
    {
        get => __country;
        set
        {
            if ( SetProperty(ref __country, value) ) { Address = null; }
        }
    }


    public TID  ID        { get => __id;        init => SetProperty(ref __id,       value); }
    public bool IsPrimary { get => __isPrimary; set => SetProperty(ref __isPrimary, value); }


    [JsonIgnore] public bool IsValidAddress
    {
        get
        {
            ReadOnlySpan<char> address = Address;
            if ( address.IsEmpty ) { return false; }

            Span<char> span = stackalloc char[address.Length];
            address.CopyTo(span);

            for ( int i = 0; i < span.Length; i++ )
            {
                if ( !char.IsLetterOrDigit(span[i]) || char.IsPunctuation(span[i]) ) { span[i] = ' '; }
            }

            return span.IsNullOrWhiteSpace();
        }
    }

    [Required] [StringLength(LINE1)] public string Line1
    {
        get => __line1;
        set
        {
            if ( SetProperty(ref __line1, value) ) { Address = null; }
        }
    }

    [StringLength(LINE2)] public string Line2
    {
        get => __line2;
        set
        {
            if ( SetProperty(ref __line2, value) ) { Address = null; }
        }
    }

    [Required] [StringLength(POSTAL_CODE)] public string PostalCode
    {
        get => __postalCode;
        set
        {
            if ( SetProperty(ref __postalCode, value) ) { Address = null; }
        }
    }

    [Required] [StringLength(STATE_OR_PROVINCE)] public string StateOrProvince
    {
        get => __stateOrProvince;
        set
        {
            if ( SetProperty(ref __stateOrProvince, value) ) { Address = null; }
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
    protected UserAddress( Match match ) : this(match.Groups["StreetName"].Value, match.Groups["Apt"].Value, match.Groups["City"].Value, match.Groups["State"].Value, match.Groups["ZipCode"].Value, match.Groups["Country"].Value) { }
    protected UserAddress( string line1, string line2, string city, string stateOrProvince, string postalCode, string country, TID id = default )
    {
        ID         = id;
        Line1      = line1;
        Line2      = line2;
        City       = city;
        PostalCode = postalCode;
        Country    = country;
    }


    public override string ToString() => string.IsNullOrWhiteSpace(Line2)
                                             ? $"{Line1}. {City}, {StateOrProvince}. {Country}. {PostalCode}"
                                             : $"{Line1} {Line2}. {City}, {StateOrProvince}. {Country}. {PostalCode}";


    public override int CompareTo( TSelf? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int countryComparison = string.Compare(__country, other.__country, StringComparison.Ordinal);
        if ( countryComparison != 0 ) { return countryComparison; }

        int stateOrProvinceComparison = string.Compare(__stateOrProvince, other.__stateOrProvince, StringComparison.Ordinal);
        if ( stateOrProvinceComparison != 0 ) { return stateOrProvinceComparison; }

        int cityComparison = string.Compare(__city, other.__city, StringComparison.Ordinal);
        if ( cityComparison != 0 ) { return cityComparison; }

        int postalCodeComparison = string.Compare(__postalCode, other.__postalCode, StringComparison.Ordinal);
        if ( postalCodeComparison != 0 ) { return postalCodeComparison; }

        int line1Comparison = string.Compare(__line1, other.__line1, StringComparison.Ordinal);
        if ( line1Comparison != 0 ) { return line1Comparison; }

        int line2Comparison = string.Compare(__line2, other.__line2, StringComparison.Ordinal);
        if ( line2Comparison != 0 ) { return line2Comparison; }

        return 0;
    }
    public override bool Equals( TSelf? other )
    {
        if ( other is null ) { return false; }

        return ReferenceEquals(this, other) || ( string.Equals(Line1, other.Line1, StringComparison.Ordinal) && string.Equals(Line2, other.Line2, StringComparison.Ordinal) && string.Equals(City, other.City, StringComparison.Ordinal) && string.Equals(PostalCode, other.PostalCode, StringComparison.Ordinal) && string.Equals(StateOrProvince, other.StateOrProvince, StringComparison.Ordinal) );
    }
}
