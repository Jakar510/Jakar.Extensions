﻿// Jakar.Extensions :: Jakar.Extensions
// 09/29/2023  10:20 PM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "UnusedMemberInSuper.Global" )]
public interface IAddress<out TID> : IUniqueID<TID>
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
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



public class UserAddress<TID> : ObservableClass, IAddress<TID>, IComparable<UserAddress<TID>>, IComparable, JsonModels.IJsonModel
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
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


    public static Sorter<UserAddress<TID>> Sorter { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Sorter<UserAddress<TID>>.Default; }


    [JsonExtensionData]                       public IDictionary<string, JToken?>? AdditionalData { get => _additionalData;         set => SetProperty( ref _additionalData, value ); }
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

    public TID  ID        { get;               init; }
    public bool IsPrimary { get => _isPrimary; set => SetProperty( ref _isPrimary, value ); }

    [JsonIgnore]
    public bool IsValidAddress
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        get
        {
            Span<char> span = [..Address];

            for ( int i = 0; i < span.Length; i++ )
            {
                if ( char.IsLetterOrDigit( span[i] ) ) { continue; }

                span[i] = ' ';
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


    public UserAddress() { }
    public UserAddress( IAddress<TID> address )
    {
        Address         = address.Address;
        Line1           = address.Line1;
        Line2           = address.Line2;
        City            = address.City;
        StateOrProvince = address.StateOrProvince;
        Country         = address.Country;
        PostalCode      = address.PostalCode;
        IsPrimary       = address.IsPrimary;
        ID              = address.ID;
    }
    public static UserAddress<TID> Create( IAddress<TID> address ) => new(address);


    public override string ToString() => string.IsNullOrWhiteSpace( Line2 )
                                             ? $"{Line1}. {City}, {StateOrProvince}. {Country}. {PostalCode}"
                                             : $"{Line1} {Line2}. {City}, {StateOrProvince}. {Country}. {PostalCode}";

    public int CompareTo( UserAddress<TID>? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

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

        return other is UserAddress<TID> address
                   ? CompareTo( address )
                   : throw new ArgumentException( $"Object must be of type {nameof(UserAddress<TID>)}" );
    }
}
