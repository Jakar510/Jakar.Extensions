// Jakar.Extensions :: Jakar.Database
// 09/29/2023  9:25 PM

using System.Security.Claims;



namespace Jakar.Database;


public sealed record AddressRecord( string                  Line1,
                                    string                  Line2,
                                    string                  City,
                                    string                  StateOrProvince,
                                    string                  Country,
                                    string                  PostalCode,
                                    string                  Address,
                                    RecordID<AddressRecord> ID,
                                    RecordID<UserRecord>?   CreatedBy,
                                    Guid?                   OwnerUserID,
                                    DateTimeOffset          DateCreated,
                                    DateTimeOffset?         LastModified = default
) : OwnedTableRecord<AddressRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IEquatable<IAddress>, IDbReaderMapping<AddressRecord>
{
    [ ProtectedPersonalData, MaxLength( 512 ) ]  public string Line1           { get; set; } = Line1;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string Line2           { get; set; } = Line2;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string City            { get; set; } = City;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string PostalCode      { get; set; } = PostalCode;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string StateOrProvince { get; set; } = StateOrProvince;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string Country         { get; set; } = Country;
    [ ProtectedPersonalData, MaxLength( 4096 ) ] public string Address         { get; set; } = Address;


    // public AddressRecord( IUserData data ) { }


    public static AddressRecord Create( DbDataReader reader )
    {
        string line1           = reader.GetString( nameof(Line1) );
        string line2           = reader.GetString( nameof(Line2) );
        string city            = reader.GetString( nameof(City) );
        string stateOrProvince = reader.GetString( nameof(StateOrProvince) );
        string country         = reader.GetString( nameof(Country) );
        string postalCode      = reader.GetString( nameof(PostalCode) );
        string address         = reader.GetString( nameof(Address) );
        var    id              = RecordID<AddressRecord>.ID( reader );
        var    createdBy       = RecordID<UserRecord>.CreatedBy( reader );
        var    ownerUserID     = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        var    dateCreated     = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var    lastModified    = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );

        return new AddressRecord( line1,
                                  line2,
                                  city,
                                  stateOrProvince,
                                  country,
                                  postalCode,
                                  address,
                                  id,
                                  createdBy,
                                  ownerUserID,
                                  dateCreated,
                                  lastModified );
    }
    public static async IAsyncEnumerable<AddressRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    public static async ValueTask<AddressRecord?> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, Claim[] claims, ClaimType types, CancellationToken token )
    {
        var parameters = new DynamicParameters();

        if ( types.HasFlag( ClaimType.StreetAddressLine1 ) )
        {
            parameters.Add( nameof(Line1),
                            claims.Single( x => x.Type == ClaimTypes.StreetAddress )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.StreetAddressLine2 ) )
        {
            parameters.Add( nameof(Line2),
                            claims.Single( x => x.Type == ClaimTypes.Locality )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.StateOrProvince ) )
        {
            parameters.Add( nameof(StateOrProvince),
                            claims.Single( x => x.Type == ClaimTypes.StateOrProvince )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.Country ) )
        {
            parameters.Add( nameof(Country),
                            claims.Single( x => x.Type == ClaimTypes.Country )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.PostalCode ) )
        {
            parameters.Add( nameof(PostalCode),
                            claims.Single( x => x.Type == ClaimTypes.PostalCode )
                                  .Value );
        }

        return await db.Addresses.Get( connection, transaction, true, parameters, token );
    }
    public static async ValueTask<IEnumerable<AddressRecord>> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, Claim claim, CancellationToken token )
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


        return await db.Addresses.Where( connection, transaction, true, parameters, token );
    }


    // TODO: public static async IAsyncEnumerable<Claim> GetUserClaims( DbConnection connection, DbTransaction? transaction, Database db, ClaimType types, RecordID<UserRecord> id, CancellationToken token ) { }
    public IEnumerable<Claim> GetUserClaims( ClaimType types )
    {
        if ( types.HasFlag( ClaimType.StreetAddressLine1 ) ) { yield return new Claim( ClaimTypes.StreetAddress, Line1, ClaimValueTypes.String ); }

        if ( types.HasFlag( ClaimType.StreetAddressLine2 ) ) { yield return new Claim( ClaimTypes.Locality, Line2, ClaimValueTypes.String ); }

        if ( types.HasFlag( ClaimType.StateOrProvince ) ) { yield return new Claim( ClaimTypes.Country, StateOrProvince, ClaimValueTypes.String ); }

        if ( types.HasFlag( ClaimType.Country ) ) { yield return new Claim( ClaimTypes.Country, Country, ClaimValueTypes.String ); }

        if ( types.HasFlag( ClaimType.PostalCode ) ) { yield return new Claim( ClaimTypes.PostalCode, PostalCode, ClaimValueTypes.String ); }
    }


    public AddressRecord WithUserData( IAddress value )
    {
        return this with
               {
                   Line1 = value.Line1,
                   Line2 = value.Line2,
                   City = value.City,
                   StateOrProvince = value.StateOrProvince,
                   Country = value.Country,
                   PostalCode = value.PostalCode
               };
    }
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
}
