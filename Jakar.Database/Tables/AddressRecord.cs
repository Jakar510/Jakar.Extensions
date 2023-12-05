// Jakar.Extensions :: Jakar.Database
// 09/29/2023  9:25 PM

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;



namespace Jakar.Database;


public sealed record AddressRecord( [ property: ProtectedPersonalData, MaxLength( 512 ) ]  string Line1,
                                    [ property: ProtectedPersonalData, MaxLength( 512 ) ]  string Line2,
                                    [ property: ProtectedPersonalData, MaxLength( 256 ) ]  string City,
                                    [ property: ProtectedPersonalData, MaxLength( 256 ) ]  string StateOrProvince,
                                    [ property: ProtectedPersonalData, MaxLength( 256 ) ]  string Country,
                                    [ property: ProtectedPersonalData, MaxLength( 256 ) ]  string PostalCode,
                                    [ property: ProtectedPersonalData, MaxLength( 4096 ) ] string Address,
                                    bool                                                          IsPrimary,
                                    IDictionary<string, JToken?>?                                 AdditionalData,
                                    RecordID<AddressRecord>                                       ID,
                                    RecordID<UserRecord>?                                         CreatedBy,
                                    Guid?                                                         OwnerUserID,
                                    DateTimeOffset                                                DateCreated,
                                    DateTimeOffset?                                               LastModified = default
) : OwnedTableRecord<AddressRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IAddress, IEquatable<IAddress>, IDbReaderMapping<AddressRecord>, IMsJsonContext<AddressRecord>
{
    public static string                 TableName      { get; } = typeof(AddressRecord).GetTableName();
    Guid? IAddress.                      UserID         => OwnerUserID;
    public IDictionary<string, JToken?>? AdditionalData { get; set; } = AdditionalData;


    [ Pure ]
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

    [ Pure ]
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
    [ Pure ]
    public static async IAsyncEnumerable<AddressRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    [ Pure ]
    public static async ValueTask<AddressRecord?> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, Claim[] claims, ClaimType types, CancellationToken token )
    {
        var parameters = new DynamicParameters();

        if ( types.HasFlag( ClaimType.StreetAddressLine1 ) ) { parameters.Add( nameof(Line1), claims.Single( x => x.Type == ClaimTypes.StreetAddress ).Value ); }

        if ( types.HasFlag( ClaimType.StreetAddressLine2 ) ) { parameters.Add( nameof(Line2), claims.Single( x => x.Type == ClaimTypes.Locality ).Value ); }

        if ( types.HasFlag( ClaimType.StateOrProvince ) ) { parameters.Add( nameof(StateOrProvince), claims.Single( x => x.Type == ClaimTypes.StateOrProvince ).Value ); }

        if ( types.HasFlag( ClaimType.Country ) ) { parameters.Add( nameof(Country), claims.Single( x => x.Type == ClaimTypes.Country ).Value ); }

        if ( types.HasFlag( ClaimType.PostalCode ) ) { parameters.Add( nameof(PostalCode), claims.Single( x => x.Type == ClaimTypes.PostalCode ).Value ); }

        return await db.Addresses.Get( connection, transaction, true, parameters, token );
    }
    [ Pure ]
    public static async IAsyncEnumerable<AddressRecord> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, Claim claim, [ EnumeratorCancellation ] CancellationToken token )
    {
        var parameters = new DynamicParameters();

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


    // TODO: public static async IAsyncEnumerable<Claim> GetUserClaims( DbConnection connection, DbTransaction? transaction, Database db, ClaimType types, RecordID<UserRecord> id, CancellationToken token ) { }
    [ Pure ]
    public IEnumerable<Claim> GetUserClaims( ClaimType types )
    {
        if ( types.HasFlag( ClaimType.StreetAddressLine1 ) ) { yield return new Claim( ClaimTypes.StreetAddress, Line1, ClaimValueTypes.String ); }

        if ( types.HasFlag( ClaimType.StreetAddressLine2 ) ) { yield return new Claim( ClaimTypes.Locality, Line2, ClaimValueTypes.String ); }

        if ( types.HasFlag( ClaimType.StateOrProvince ) ) { yield return new Claim( ClaimTypes.Country, StateOrProvince, ClaimValueTypes.String ); }

        if ( types.HasFlag( ClaimType.Country ) ) { yield return new Claim( ClaimTypes.Country, Country, ClaimValueTypes.String ); }

        if ( types.HasFlag( ClaimType.PostalCode ) ) { yield return new Claim( ClaimTypes.PostalCode, PostalCode, ClaimValueTypes.String ); }
    }


    [ Pure ]
    public AddressRecord WithUserData( IAddress value ) =>
        this with
        {
            Line1 = value.Line1,
            Line2 = value.Line2,
            City = value.City,
            StateOrProvince = value.StateOrProvince,
            Country = value.Country,
            PostalCode = value.PostalCode
        };
    public bool Equals( IAddress? other )
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


    [ Pure ]
    public static JsonSerializerOptions JsonOptions( bool formatted ) => new()
                                                                         {
                                                                             WriteIndented    = formatted,
                                                                             TypeInfoResolver = AddressRecordContext.Default,
                                                                         };
    [ Pure ] public static JsonTypeInfo<AddressRecord> JsonTypeInfo() => AddressRecordContext.Default.AddressRecord;
}



[ JsonSerializable( typeof(AddressRecord) ) ] public partial class AddressRecordContext : JsonSerializerContext { }
