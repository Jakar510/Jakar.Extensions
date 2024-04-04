// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  22:4

using System.Security.Claims;



namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "InvertIf" )]
public static class UserDataExtensions
{
    public const ClaimType ADDRESS     = ClaimType.StreetAddressLine1 | ClaimType.StreetAddressLine2 | ClaimType.StateOrProvince | ClaimType.Country | ClaimType.PostalCode;
    public const ClaimType CLAIM_TYPES = ClaimType.UserID             | ClaimType.UserName           | ClaimType.GroupSid        | ClaimType.Role;


    [MethodImpl( MethodImplOptions.NoInlining )]
    public static Claim[] GetClaims<TID, TAddress, TGroupModel, TRoleModel>( this IUserData<TID, TAddress, TGroupModel, TRoleModel> data, ClaimType types = CLAIM_TYPES )
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
    #endif
        where TGroupModel : IGroupModel<TID>
        where TRoleModel : IRoleModel<TID>
        where TAddress : IAddress<TID>
    {
        using Buffer<Claim> claims = new(20 + data.Groups.Count + data.Roles.Count + data.Addresses.Count * 5);

        types |= ClaimType.UserID | ClaimType.UserName;
        claims.Add( new Claim( ClaimType.UserID.ToClaimTypes(),   data.UserID.ToString(), ClaimValueTypes.String ) );
        claims.Add( new Claim( ClaimType.UserName.ToClaimTypes(), data.UserName,          ClaimValueTypes.String ) );

        if ( HasFlag( types, ClaimType.FirstName ) ) { claims.Add( new Claim( ClaimType.FirstName.ToClaimTypes(), data.FirstName, ClaimValueTypes.String ) ); }

        if ( HasFlag( types, ClaimType.LastName ) ) { claims.Add( new Claim( ClaimType.LastName.ToClaimTypes(), data.LastName, ClaimValueTypes.String ) ); }

        if ( HasFlag( types, ClaimType.FullName ) ) { claims.Add( new Claim( ClaimType.FullName.ToClaimTypes(), data.FullName, ClaimValueTypes.String ) ); }

        if ( HasFlag( types, ClaimType.Gender ) ) { claims.Add( new Claim( ClaimType.Gender.ToClaimTypes(), data.Gender, ClaimValueTypes.String ) ); }

        if ( HasFlag( types, ClaimType.SubscriptionExpiration ) ) { claims.Add( new Claim( ClaimType.SubscriptionExpiration.ToClaimTypes(), data.SubscriptionExpires?.ToString() ?? string.Empty, ClaimValueTypes.DateTime ) ); }

        if ( HasFlag( types, ClaimType.Expired ) ) { claims.Add( new Claim( ClaimType.Expired.ToClaimTypes(), (data.SubscriptionExpires > DateTimeOffset.UtcNow).ToString(), ClaimValueTypes.Boolean ) ); }

        if ( HasFlag( types, ClaimType.Email ) ) { claims.Add( new Claim( ClaimType.Email.ToClaimTypes(), data.Email, ClaimValueTypes.Email ) ); }

        if ( HasFlag( types, ClaimType.MobilePhone ) ) { claims.Add( new Claim( ClaimType.MobilePhone.ToClaimTypes(), data.PhoneNumber, ClaimValueTypes.String ) ); }

        if ( HasFlag( types, ClaimType.WebSite ) ) { claims.Add( new Claim( ClaimType.WebSite.ToClaimTypes(), data.Website, ClaimValueTypes.String ) ); }


        if ( HasFlag( types, ADDRESS ) )
        {
            foreach ( TAddress address in data.Addresses )
            {
                if ( HasFlag( types, ClaimType.StreetAddressLine1 ) ) { claims.Add( new Claim( ClaimType.StreetAddressLine1.ToClaimTypes(), address.Line1, ClaimValueTypes.String ) ); }

                if ( HasFlag( types, ClaimType.StreetAddressLine2 ) ) { claims.Add( new Claim( ClaimType.StreetAddressLine2.ToClaimTypes(), address.Line2, ClaimValueTypes.String ) ); }

                if ( HasFlag( types, ClaimType.StateOrProvince ) ) { claims.Add( new Claim( ClaimType.StateOrProvince.ToClaimTypes(), address.StateOrProvince, ClaimValueTypes.String ) ); }

                if ( HasFlag( types, ClaimType.Country ) ) { claims.Add( new Claim( ClaimType.Country.ToClaimTypes(), address.Country, ClaimValueTypes.String ) ); }

                if ( HasFlag( types, ClaimType.PostalCode ) ) { claims.Add( new Claim( ClaimType.PostalCode.ToClaimTypes(), address.PostalCode, ClaimValueTypes.String ) ); }
            }
        }


        if ( HasFlag( types, ClaimType.GroupSid ) )
        {
            foreach ( TGroupModel record in data.Groups ) { claims.Add( new Claim( ClaimType.GroupSid.ToClaimTypes(), record.NameOfGroup, ClaimValueTypes.String ) ); }
        }


        if ( HasFlag( types, ClaimType.Role ) )
        {
            foreach ( TRoleModel record in data.Roles.AsSpan() ) { claims.Add( new Claim( ClaimType.Role.ToClaimTypes(), record.NameOfRole, ClaimValueTypes.String ) ); }
        }

        return [.. claims.Span];
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static DateTimeOffset GetExpires( this IUserData user, scoped in TimeSpan offset ) => user.GetExpires( DateTimeOffset.UtcNow, offset );
    public static DateTimeOffset GetExpires( this IUserData user, scoped in DateTimeOffset now, scoped in TimeSpan offset )
    {
        DateTimeOffset date = now + offset;
        if ( user.SubscriptionExpires is null ) { return date; }

        DateTimeOffset expires = user.SubscriptionExpires.Value;

        return date > expires
                   ? expires
                   : date;
    }
}
