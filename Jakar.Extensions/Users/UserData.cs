// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  22:4

using System.Security.Claims;



namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "InvertIf")]
public static class UserData
{
    public const ClaimType ADDRESS     = ClaimType.StreetAddressLine1 | ClaimType.StreetAddressLine2 | ClaimType.StateOrProvince | ClaimType.Country | ClaimType.PostalCode;
    public const ClaimType CLAIM_TYPES = ClaimType.UserID             | ClaimType.UserName           | ClaimType.Group           | ClaimType.Role;


    public static Claim[] GetClaims<TID>( this IUserData<TID> model, in ClaimType types = CLAIM_TYPES, in string? issuer = null )
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    {
        using IMemoryOwner<Claim> claims = MemoryPool<Claim>.Shared.Rent(20);
        int                       size   = 0;
        Span<Claim>               span   = claims.Memory.Span;


        span[size++] = ClaimType.UserID.ToClaim(model.UserID.ToString(), issuer);

        if ( HasFlag(types, ClaimType.UserName) ) { span[size++] = ClaimType.UserName.ToClaim(model.UserName, issuer); }

        if ( HasFlag(types, ClaimType.FirstName) ) { span[size++] = ClaimType.FirstName.ToClaim(model.FirstName, issuer); }

        if ( HasFlag(types, ClaimType.LastName) ) { span[size++] = ClaimType.LastName.ToClaim(model.LastName, issuer); }

        if ( HasFlag(types, ClaimType.FullName) ) { span[size++] = ClaimType.FullName.ToClaim(model.FullName, issuer); }

        if ( HasFlag(types, ClaimType.Gender) ) { span[size++] = ClaimType.Gender.ToClaim(model.Gender, issuer); }

        if ( HasFlag(types, ClaimType.SubscriptionExpiration) ) { span[size++] = ClaimType.SubscriptionExpiration.ToClaim(model.SubscriptionExpires, issuer); }

        if ( HasFlag(types, ClaimType.Expired) ) { span[size++] = ClaimType.Expired.ToClaim(model.SubscriptionExpires > DateTimeOffset.UtcNow, issuer); }

        if ( HasFlag(types, ClaimType.Email) ) { span[size++] = ClaimType.Email.ToClaim(model.Email, issuer); }

        if ( HasFlag(types, ClaimType.MobilePhone) ) { span[size++] = ClaimType.MobilePhone.ToClaim(model.PhoneNumber, issuer); }

        if ( HasFlag(types, ClaimType.WebSite) ) { span[size++] = ClaimType.WebSite.ToClaim(model.Website, issuer); }

        return [.. span];
    }

    public static Claim[] GetClaims<TID, TAddress, TGroupModel, TRoleModel>( this IUserData<TID, TAddress, TGroupModel, TRoleModel> model, in ClaimType types = CLAIM_TYPES, in string? issuer = null )
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
        where TGroupModel : IGroupModel<TID>, IEquatable<TGroupModel>
        where TRoleModel : IRoleModel<TID>, IEquatable<TRoleModel>
        where TAddress : IAddress<TID>, IEquatable<TAddress>
    {
        using IMemoryOwner<Claim> claims = MemoryPool<Claim>.Shared.Rent(20 + model.Groups.Count + model.Roles.Count + model.Addresses.Count * 5);
        int                       size   = 0;
        Span<Claim>               span   = claims.Memory.Span;


        span[size++] = ClaimType.UserID.ToClaim(model.UserID.ToString(), issuer);

        if ( HasFlag(types, ClaimType.UserName) ) { span[size++] = ClaimType.UserName.ToClaim(model.UserName, issuer); }

        if ( HasFlag(types, ClaimType.FirstName) ) { span[size++] = ClaimType.FirstName.ToClaim(model.FirstName, issuer); }

        if ( HasFlag(types, ClaimType.LastName) ) { span[size++] = ClaimType.LastName.ToClaim(model.LastName, issuer); }

        if ( HasFlag(types, ClaimType.FullName) ) { span[size++] = ClaimType.FullName.ToClaim(model.FullName, issuer); }

        if ( HasFlag(types, ClaimType.Gender) ) { span[size++] = ClaimType.Gender.ToClaim(model.Gender, issuer); }

        if ( HasFlag(types, ClaimType.SubscriptionExpiration) ) { span[size++] = ClaimType.SubscriptionExpiration.ToClaim(model.SubscriptionExpires, issuer); }

        if ( HasFlag(types, ClaimType.Expired) ) { span[size++] = ClaimType.Expired.ToClaim(model.SubscriptionExpires > DateTimeOffset.UtcNow, issuer); }

        if ( HasFlag(types, ClaimType.Email) ) { span[size++] = ClaimType.Email.ToClaim(model.Email, issuer); }

        if ( HasFlag(types, ClaimType.MobilePhone) ) { span[size++] = ClaimType.MobilePhone.ToClaim(model.PhoneNumber, issuer); }

        if ( HasFlag(types, ClaimType.WebSite) ) { span[size++] = ClaimType.WebSite.ToClaim(model.Website, issuer); }


        if ( HasFlag(types, ADDRESS) )
        {
            foreach ( TAddress address in model.Addresses )
            {
                if ( HasFlag(types, ClaimType.StreetAddressLine1) ) { span[size++] = ClaimType.StreetAddressLine1.ToClaim(address.Line1, issuer); }

                if ( HasFlag(types, ClaimType.StreetAddressLine2) ) { span[size++] = ClaimType.StreetAddressLine2.ToClaim(address.Line2, issuer); }

                if ( HasFlag(types, ClaimType.StateOrProvince) ) { span[size++] = ClaimType.StateOrProvince.ToClaim(address.StateOrProvince, issuer); }

                if ( HasFlag(types, ClaimType.Country) ) { span[size++] = ClaimType.Country.ToClaim(address.Country, issuer); }

                if ( HasFlag(types, ClaimType.PostalCode) ) { span[size++] = ClaimType.PostalCode.ToClaim(address.PostalCode, issuer); }
            }
        }


        if ( HasFlag(types, ClaimType.Group) )
        {
            foreach ( TGroupModel record in model.Groups.AsSpan() ) { span[size++] = ClaimType.Group.ToClaim(record.NameOfGroup, issuer); }
        }


        if ( HasFlag(types, ClaimType.Role) )
        {
            foreach ( TRoleModel record in model.Roles.AsSpan() ) { span[size++] = ClaimType.Role.ToClaim(record.NameOfRole, issuer); }
        }

        return [.. span];
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static DateTimeOffset GetExpires( this IUserData user, scoped in TimeSpan offset ) => user.GetExpires(DateTimeOffset.UtcNow, offset);
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
