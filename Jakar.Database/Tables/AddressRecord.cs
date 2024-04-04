// Jakar.Extensions :: Jakar.Database
// 09/29/2023  9:25 PM

namespace Jakar.Database;


[Table( TABLE_NAME )]
public sealed record AddressRecord( [property: ProtectedPersonalData] string Line1,
                                    [property: ProtectedPersonalData] string Line2,
                                    [property: ProtectedPersonalData] string City,
                                    [property: ProtectedPersonalData] string StateOrProvince,
                                    [property: ProtectedPersonalData] string Country,
                                    [property: ProtectedPersonalData] string PostalCode,
                                    [property: ProtectedPersonalData] string Address,
                                    bool                                     IsPrimary,
                                    IDictionary<string, JToken?>?            AdditionalData,
                                    RecordID<AddressRecord>                  ID,
                                    RecordID<UserRecord>?                    CreatedBy,
                                    Guid?                                    OwnerUserID,
                                    DateTimeOffset                           DateCreated,
                                    DateTimeOffset?                          LastModified = default ) : OwnedTableRecord<AddressRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IAddress<Guid>, IDbReaderMapping<AddressRecord>
{
    public const  string                        TABLE_NAME = "Address";
    public static string                        TableName      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }
    public        IDictionary<string, JToken?>? AdditionalData { get; set; } = AdditionalData;


    [Pure]
    public override DynamicParameters ToDynamicParameters()
    {
        var parameters = base.ToDynamicParameters();
        parameters.Add( nameof(Line1),           Line1 );
        parameters.Add( nameof(Line2),           Line2 );
        parameters.Add( nameof(City),            City );
        parameters.Add( nameof(PostalCode),      PostalCode );
        parameters.Add( nameof(StateOrProvince), StateOrProvince );
        parameters.Add( nameof(Country),         Country );
        parameters.Add( nameof(Address),         Address );
        return parameters;
    }

    [Pure]
    public static AddressRecord Create( DbDataReader reader )
    {
        string                        line1           = reader.GetFieldValue<string>( nameof(Line1) );
        string                        line2           = reader.GetFieldValue<string>( nameof(Line2) );
        string                        city            = reader.GetFieldValue<string>( nameof(City) );
        string                        stateOrProvince = reader.GetFieldValue<string>( nameof(StateOrProvince) );
        string                        country         = reader.GetFieldValue<string>( nameof(Country) );
        string                        postalCode      = reader.GetFieldValue<string>( nameof(PostalCode) );
        string                        address         = reader.GetFieldValue<string>( nameof(Address) );
        IDictionary<string, JToken?>? additionalData  = reader.GetAdditionalData();
        bool                          isPrimary       = reader.GetFieldValue<bool>( nameof(IsPrimary) );
        RecordID<AddressRecord>       id              = RecordID<AddressRecord>.ID( reader );
        RecordID<UserRecord>?         createdBy       = RecordID<UserRecord>.CreatedBy( reader );
        var                           ownerUserID     = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        var                           dateCreated     = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var                           lastModified    = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );

        var record = new AddressRecord( line1,
                                        line2,
                                        city,
                                        stateOrProvince,
                                        country,
                                        postalCode,
                                        address,
                                        isPrimary,
                                        additionalData,
                                        id,
                                        createdBy,
                                        ownerUserID,
                                        dateCreated,
                                        lastModified );

        record.Validate();
        return record;
    }
    [Pure]
    public static async IAsyncEnumerable<AddressRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    [Pure]
    public static async ValueTask<AddressRecord?> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, ReadOnlyMemory<Claim> claims, ClaimType types, CancellationToken token )
    {
        DynamicParameters parameters = new();
        if ( HasFlag( types, ClaimType.StreetAddressLine1 ) ) { parameters.Add( nameof(Line1), claims.Span.Single( static x => x.Type == ClaimType.StreetAddressLine1.ToClaimTypes() ).Value ); }

        if ( HasFlag( types, ClaimType.StreetAddressLine2 ) ) { parameters.Add( nameof(Line2), claims.Span.Single( static x => x.Type == ClaimType.StreetAddressLine2.ToClaimTypes() ).Value ); }

        if ( HasFlag( types, ClaimType.StateOrProvince ) ) { parameters.Add( nameof(StateOrProvince), claims.Span.Single( static x => x.Type == ClaimType.StateOrProvince.ToClaimTypes() ).Value ); }

        if ( HasFlag( types, ClaimType.Country ) ) { parameters.Add( nameof(Country), claims.Span.Single( static x => x.Type == ClaimType.Country.ToClaimTypes() ).Value ); }

        if ( HasFlag( types, ClaimType.PostalCode ) ) { parameters.Add( nameof(PostalCode), claims.Span.Single( static x => x.Type == ClaimType.PostalCode.ToClaimTypes() ).Value ); }

        return await db.Addresses.Get( connection, transaction, true, parameters, token );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        static bool HasFlag( ClaimType value, ClaimType flag ) => (value & flag) != 0;
    }
    [Pure]
    public static async IAsyncEnumerable<AddressRecord> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, Claim claim, [EnumeratorCancellation] CancellationToken token )
    {
        DynamicParameters parameters = new();

        switch ( claim.Type )
        {
            case ClaimTypes.StreetAddress:
                parameters.Add( nameof(Line1), claim.Value );
                break;

            case ClaimTypes.Locality:
                parameters.Add( nameof(Line2), claim.Value );
                break;

            case ClaimTypes.StateOrProvince:
                parameters.Add( nameof(StateOrProvince), claim.Value );
                break;

            case ClaimTypes.Country:
                parameters.Add( nameof(Country), claim.Value );
                break;

            case ClaimTypes.PostalCode:
                parameters.Add( nameof(PostalCode), claim.Value );
                break;
        }

        await foreach ( AddressRecord record in db.Addresses.Where( connection, transaction, true, parameters, token ) ) { yield return record; }
    }


    [Pure]
    public IEnumerable<Claim> GetUserClaims( ClaimType types )
    {
        if ( HasFlag( types, ClaimType.StreetAddressLine1 ) ) { yield return new Claim( ClaimType.StreetAddressLine1.ToClaimTypes(), Line1, ClaimValueTypes.String ); }

        if ( HasFlag( types, ClaimType.StreetAddressLine2 ) ) { yield return new Claim( ClaimType.StreetAddressLine2.ToClaimTypes(), Line2, ClaimValueTypes.String ); }

        if ( HasFlag( types, ClaimType.StateOrProvince ) ) { yield return new Claim( ClaimType.StateOrProvince.ToClaimTypes(), StateOrProvince, ClaimValueTypes.String ); }

        if ( HasFlag( types, ClaimType.Country ) ) { yield return new Claim( ClaimType.Country.ToClaimTypes(), Country, ClaimValueTypes.String ); }

        if ( HasFlag( types, ClaimType.PostalCode ) ) { yield return new Claim( ClaimType.PostalCode.ToClaimTypes(), PostalCode, ClaimValueTypes.String ); }

        yield break;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        static bool HasFlag( ClaimType value, ClaimType flag ) => (value & flag) != 0;
    }


    public UserAddress<Guid> ToAddressModel() => UserAddress<Guid>.Create( this );
    public TAddress ToAddressModel<TAddress>()
        where TAddress : IAddress<TAddress, Guid> => TAddress.Create( this );


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
    public bool Equals( IAddress<Guid>? other )
    {
        if ( other is null ) { return false; }

        return Line1 == other.Line1 && Line2 == other.Line2 && City == other.City && PostalCode == other.PostalCode && StateOrProvince == other.StateOrProvince && Country == other.Country && Address == other.Address;
    }
    public bool Equals( AddressRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return base.Equals( other ) && Line1 == other.Line1 && Line2 == other.Line2 && City == other.City && PostalCode == other.PostalCode && StateOrProvince == other.StateOrProvince && Country == other.Country && Address == other.Address;
    }
    public override int GetHashCode() => HashCode.Combine( base.GetHashCode(), Line1, Line2, City, PostalCode, StateOrProvince, Country, Address );
}
