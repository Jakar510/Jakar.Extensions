// Jakar.Extensions :: Jakar.Database
// 09/29/2023  9:25 PM

namespace Jakar.Database;


[Table(TABLE_NAME)]
public sealed record AddressRecord( [property: ProtectedPersonalData] string  Line1,
                                    [property: ProtectedPersonalData] string  Line2,
                                    [property: ProtectedPersonalData] string  City,
                                    [property: ProtectedPersonalData] string  StateOrProvince,
                                    [property: ProtectedPersonalData] string  Country,
                                    [property: ProtectedPersonalData] string  PostalCode,
                                    [property: ProtectedPersonalData] string? Address,
                                    bool                                      IsPrimary,
                                    JsonObject?             AdditionalData,
                                    RecordID<AddressRecord>                   ID,
                                    RecordID<UserRecord>?                     CreatedBy,
                                    DateTimeOffset                            DateCreated,
                                    DateTimeOffset?                           LastModified = null ) : OwnedTableRecord<AddressRecord>(in CreatedBy, in ID, in DateCreated, in LastModified), IAddress<AddressRecord, Guid>, ITableRecord<AddressRecord>
{
    public const  string                        TABLE_NAME = "Address";
    public static string                        TableName      { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => TABLE_NAME; }
    public        JsonObject? AdditionalData { get; set; } = AdditionalData;

    public AddressRecord( IAddress<Guid> address ) : this(address.Line1,
                                                          address.Line2,
                                                          address.City,
                                                          address.StateOrProvince,
                                                          address.Country,
                                                          address.PostalCode,
                                                          address.Address ?? EMPTY,
                                                          address.IsPrimary,
                                                          null,
                                                          RecordID<AddressRecord>.Create(address.ID),
                                                          null,
                                                          DateTimeOffset.UtcNow) { }
    public AddressRecord( Match match ) : this(match.Groups["StreetName"].Value, match.Groups["Apt"].Value, match.Groups["City"].Value, match.Groups["State"].Value, match.Groups["ZipCode"].Value, match.Groups["Country"].Value) { }
    public AddressRecord( string line1, string line2, string city, string stateOrProvince, string postalCode, string country, Guid? id = null ) : this(line1,
                                                                                                                                                       line2,
                                                                                                                                                       city,
                                                                                                                                                       stateOrProvince,
                                                                                                                                                       postalCode,
                                                                                                                                                       country,
                                                                                                                                                       null,
                                                                                                                                                       true,
                                                                                                                                                       null,
                                                                                                                                                       RecordID<AddressRecord>.Create(id),
                                                                                                                                                       null,
                                                                                                                                                       DateTimeOffset.UtcNow) { }


    [Pure]
    public override DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = base.ToDynamicParameters();
        parameters.Add(nameof(Line1),           Line1);
        parameters.Add(nameof(Line2),           Line2);
        parameters.Add(nameof(City),            City);
        parameters.Add(nameof(PostalCode),      PostalCode);
        parameters.Add(nameof(StateOrProvince), StateOrProvince);
        parameters.Add(nameof(Country),         Country);
        parameters.Add(nameof(Address),         Address);
        return parameters;
    }


    [Pure] public static AddressRecord Create( Match          match )   => new(match);
    [Pure] public static AddressRecord Create( IAddress<Guid> address ) => new(address);
    [Pure]
    public static AddressRecord Create( string line1, string line2, string city, string stateOrProvince, string postalCode, string country, Guid id = default ) => new(line1,
                                                                                                                                                                       line2,
                                                                                                                                                                       city,
                                                                                                                                                                       stateOrProvince,
                                                                                                                                                                       postalCode,
                                                                                                                                                                       country,
                                                                                                                                                                       id.IsValidID()
                                                                                                                                                                           ? id
                                                                                                                                                                           : Guid.NewGuid());

    [Pure]
    public static AddressRecord Create( DbDataReader reader )
    {
        string                        line1           = reader.GetFieldValue<string>(nameof(Line1));
        string                        line2           = reader.GetFieldValue<string>(nameof(Line2));
        string                        city            = reader.GetFieldValue<string>(nameof(City));
        string                        stateOrProvince = reader.GetFieldValue<string>(nameof(StateOrProvince));
        string                        country         = reader.GetFieldValue<string>(nameof(Country));
        string                        postalCode      = reader.GetFieldValue<string>(nameof(PostalCode));
        string                        address         = reader.GetFieldValue<string>(nameof(Address));
        JsonObject? additionalData  = reader.GetAdditionalData();
        bool                          isPrimary       = reader.GetFieldValue<bool>(nameof(IsPrimary));
        RecordID<AddressRecord>       id              = RecordID<AddressRecord>.ID(reader);
        RecordID<UserRecord>?         ownerUserID     = RecordID<UserRecord>.CreatedBy(reader);
        DateTimeOffset                dateCreated     = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?               lastModified    = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));

        AddressRecord record = new(line1,
                                   line2,
                                   city,
                                   stateOrProvince,
                                   country,
                                   postalCode,
                                   address,
                                   isPrimary,
                                   additionalData,
                                   id,
                                   ownerUserID,
                                   dateCreated,
                                   lastModified);

        return record.Validate();
    }


    [Pure]
    public static async ValueTask<AddressRecord?> TryFromClaims( NpgsqlConnection connection, DbTransaction transaction, Database db, Claim[] claims, ClaimType types, CancellationToken token )
    {
        DynamicParameters   parameters = new();
        ReadOnlySpan<Claim> span       = claims;
        if ( hasFlag(types, ClaimType.StreetAddressLine1) ) { parameters.Add(nameof(Line1), span.Single(static ( ref readonly Claim x ) => x.Type == ClaimType.StreetAddressLine1.ToClaimTypes()).Value); }

        if ( hasFlag(types, ClaimType.StreetAddressLine2) ) { parameters.Add(nameof(Line2), span.Single(static ( ref readonly Claim x ) => x.Type == ClaimType.StreetAddressLine2.ToClaimTypes()).Value); }

        if ( hasFlag(types, ClaimType.StateOrProvince) ) { parameters.Add(nameof(StateOrProvince), span.Single(static ( ref readonly Claim x ) => x.Type == ClaimType.StateOrProvince.ToClaimTypes()).Value); }

        if ( hasFlag(types, ClaimType.Country) ) { parameters.Add(nameof(Country), span.Single(static ( ref readonly Claim x ) => x.Type == ClaimType.Country.ToClaimTypes()).Value); }

        if ( hasFlag(types, ClaimType.PostalCode) ) { parameters.Add(nameof(PostalCode), span.Single(static ( ref readonly Claim x ) => x.Type == ClaimType.PostalCode.ToClaimTypes()).Value); }

        return await db.Addresses.Get(connection, transaction, true, parameters, token);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool hasFlag( ClaimType value, ClaimType flag ) => ( value & flag ) != 0;
    }
    [Pure]
    public static async IAsyncEnumerable<AddressRecord> TryFromClaims( NpgsqlConnection connection, DbTransaction transaction, Database db, Claim claim, [EnumeratorCancellation] CancellationToken token )
    {
        DynamicParameters parameters = new();

        switch ( claim.Type )
        {
            case ClaimTypes.StreetAddress:
                parameters.Add(nameof(Line1), claim.Value);
                break;

            case ClaimTypes.Locality:
                parameters.Add(nameof(Line2), claim.Value);
                break;

            case ClaimTypes.StateOrProvince:
                parameters.Add(nameof(StateOrProvince), claim.Value);
                break;

            case ClaimTypes.Country:
                parameters.Add(nameof(Country), claim.Value);
                break;

            case ClaimTypes.PostalCode:
                parameters.Add(nameof(PostalCode), claim.Value);
                break;
        }

        await foreach ( AddressRecord record in db.Addresses.Where(connection, transaction, true, parameters, token) ) { yield return record; }
    }


    [Pure]
    public IEnumerable<Claim> GetUserClaims( ClaimType types )
    {
        if ( hasFlag(types, ClaimType.StreetAddressLine1) ) { yield return new Claim(ClaimType.StreetAddressLine1.ToClaimTypes(), Line1, ClaimValueTypes.String); }

        if ( hasFlag(types, ClaimType.StreetAddressLine2) ) { yield return new Claim(ClaimType.StreetAddressLine2.ToClaimTypes(), Line2, ClaimValueTypes.String); }

        if ( hasFlag(types, ClaimType.StateOrProvince) ) { yield return new Claim(ClaimType.StateOrProvince.ToClaimTypes(), StateOrProvince, ClaimValueTypes.String); }

        if ( hasFlag(types, ClaimType.Country) ) { yield return new Claim(ClaimType.Country.ToClaimTypes(), Country, ClaimValueTypes.String); }

        if ( hasFlag(types, ClaimType.PostalCode) ) { yield return new Claim(ClaimType.PostalCode.ToClaimTypes(), PostalCode, ClaimValueTypes.String); }

        yield break;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool hasFlag( ClaimType value, ClaimType flag ) => ( value & flag ) != 0;
    }


    public UserAddress<Guid> ToAddressModel() => UserAddress<Guid>.Create(this);
    public TAddress ToAddressModel<TAddress>()
        where TAddress : class, IAddress<TAddress, Guid> => TAddress.Create(this);


    [Pure]
    public AddressRecord WithUserData( IAddress<Guid> value ) =>
        this with
        {
            Line1 = value.Line1,
            Line2 = value.Line2,
            City = value.City,
            StateOrProvince = value.StateOrProvince,
            Country = value.Country,
            PostalCode = value.PostalCode
        };
    public override bool Equals( AddressRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return base.Equals(other) && Line1 == other.Line1 && Line2 == other.Line2 && City == other.City && PostalCode == other.PostalCode && StateOrProvince == other.StateOrProvince && Country == other.Country && Address == other.Address;
    }
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Line1, Line2, City, PostalCode, StateOrProvince, Country, Address);


    public static bool operator >( AddressRecord  left, AddressRecord right ) => left.CompareTo(right) > 0;
    public static bool operator >=( AddressRecord left, AddressRecord right ) => left.CompareTo(right) >= 0;
    public static bool operator <( AddressRecord  left, AddressRecord right ) => left.CompareTo(right) < 0;
    public static bool operator <=( AddressRecord left, AddressRecord right ) => left.CompareTo(right) <= 0;
}
