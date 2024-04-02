// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  22:4

using System.Security.Claims;



namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "InvertIf" )]
public static class UserDataExtensions
{
    public const ClaimType CLAIM_TYPES = ClaimType.UserID             | ClaimType.UserName           | ClaimType.GroupSid        | ClaimType.Role;
    public const ClaimType ADDRESS     = ClaimType.StreetAddressLine1 | ClaimType.StreetAddressLine2 | ClaimType.StateOrProvince | ClaimType.Country | ClaimType.PostalCode;


    [MethodImpl( MethodImplOptions.NoInlining )]
    public static Claim[] GetClaims<TID, TAddress, TGroupModel, TRoleModel>( this IUserData<TID, TAddress, TGroupModel, TRoleModel> data, ClaimType types = CLAIM_TYPES )
#if NET8_0
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

        
        if ( types.HasFlag( ClaimType.UserID ) ) { claims.Add( new Claim( ClaimType.UserID.ToClaimTypes(), data.UserID.ToString(), ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.UserName ) ) { claims.Add( new Claim( ClaimType.UserName.ToClaimTypes(), data.UserName, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.FirstName ) ) { claims.Add( new Claim( ClaimType.FirstName.ToClaimTypes(), data.FirstName, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.LastName ) ) { claims.Add( new Claim( ClaimType.LastName.ToClaimTypes(), data.LastName, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.FullName ) ) { claims.Add( new Claim( ClaimType.FullName.ToClaimTypes(), data.FullName, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.Gender ) ) { claims.Add( new Claim( ClaimType.Gender.ToClaimTypes(), data.Gender, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.SubscriptionExpiration ) ) { claims.Add( new Claim( ClaimType.SubscriptionExpiration.ToClaimTypes(), data.SubscriptionExpires?.ToString() ?? string.Empty, ClaimValueTypes.DateTime ) ); }

        if ( types.HasFlag( ClaimType.Expired ) ) { claims.Add( new Claim( ClaimType.Expired.ToClaimTypes(), (data.SubscriptionExpires > DateTimeOffset.UtcNow).ToString(), ClaimValueTypes.Boolean ) ); }

        if ( types.HasFlag( ClaimType.Email ) ) { claims.Add( new Claim( ClaimType.Email.ToClaimTypes(), data.Email, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.MobilePhone ) ) { claims.Add( new Claim( ClaimType.MobilePhone.ToClaimTypes(), data.PhoneNumber, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.WebSite ) ) { claims.Add( new Claim( ClaimType.WebSite.ToClaimTypes(), data.Website, ClaimValueTypes.String ) ); }


        if ( (types & ADDRESS) != 0 )
        {
            foreach ( TAddress address in data.Addresses )
            {
                if ( types.HasFlag( ClaimType.StreetAddressLine1 ) ) { claims.Add( new Claim( ClaimType.StreetAddressLine1.ToClaimTypes(), address.Line1, ClaimValueTypes.String ) ); }

                if ( types.HasFlag( ClaimType.StreetAddressLine2 ) ) { claims.Add( new Claim( ClaimType.StreetAddressLine2.ToClaimTypes(), address.Line2, ClaimValueTypes.String ) ); }

                if ( types.HasFlag( ClaimType.StateOrProvince ) ) { claims.Add( new Claim( ClaimType.StateOrProvince.ToClaimTypes(), address.StateOrProvince, ClaimValueTypes.String ) ); }

                if ( types.HasFlag( ClaimType.Country ) ) { claims.Add( new Claim( ClaimType.Country.ToClaimTypes(), address.Country, ClaimValueTypes.String ) ); }

                if ( types.HasFlag( ClaimType.PostalCode ) ) { claims.Add( new Claim( ClaimType.PostalCode.ToClaimTypes(), address.PostalCode, ClaimValueTypes.String ) ); }
            }
        }


        if ( types.HasFlag( ClaimType.GroupSid ) )
        {
            foreach ( TGroupModel record in data.Groups ) { claims.Add( new Claim( ClaimType.GroupSid.ToClaimTypes(), record.NameOfGroup, ClaimValueTypes.String ) ); }
        }


        if ( types.HasFlag( ClaimType.Role ) )
        {
            foreach ( TRoleModel record in data.Roles.AsSpan() ) { claims.Add( new Claim( ClaimType.Role.ToClaimTypes(), record.NameOfRole, ClaimValueTypes.String ) ); }
        }

        return [.. claims.Span];
    }
}
