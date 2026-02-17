// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  22:4

using System.Security.Claims;



namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "InvertIf")]
public static class UserData
{
    public const ClaimType ADDRESS     = ClaimType.StreetAddressLine1 | ClaimType.StreetAddressLine2 | ClaimType.StateOrProvince | ClaimType.Country | ClaimType.PostalCode;
    public const ClaimType CLAIM_TYPES = ClaimType.UserID             | ClaimType.UserName           | ClaimType.Group           | ClaimType.Role;



    extension<TUser>( TUser self )
        where TUser : IUserData, IUserDetails
    {
        private void GetClaims( Span<Claim> span, ref int size, in ClaimType types = CLAIM_TYPES, in string? issuer = null )
        {
            if ( span.Length < 20 ) { throw new ArgumentException("Span must be at least 20 elements long.", nameof(span)); }

            span[size++] = ClaimType.UserID.ToClaim(self.UserID.ToString(), issuer);

            if ( HasFlag(types, ClaimType.UserName) ) { span[size++] = ClaimType.UserName.ToClaim(self.UserName, issuer); }

            if ( HasFlag(types, ClaimType.FirstName) ) { span[size++] = ClaimType.FirstName.ToClaim(self.FirstName, issuer); }

            if ( HasFlag(types, ClaimType.LastName) ) { span[size++] = ClaimType.LastName.ToClaim(self.LastName, issuer); }

            if ( HasFlag(types, ClaimType.FullName) ) { span[size++] = ClaimType.FullName.ToClaim(self.FullName, issuer); }

            if ( HasFlag(types, ClaimType.Gender) ) { span[size++] = ClaimType.Gender.ToClaim(self.Gender, issuer); }

            if ( HasFlag(types, ClaimType.SubscriptionExpiration) ) { span[size++] = ClaimType.SubscriptionExpiration.ToClaim(self.SubscriptionExpires, issuer); }

            if ( HasFlag(types, ClaimType.Expired) ) { span[size++] = ClaimType.Expired.ToClaim(self.SubscriptionExpires > DateTimeOffset.UtcNow, issuer); }

            if ( HasFlag(types, ClaimType.Email) ) { span[size++] = ClaimType.Email.ToClaim(self.Email, issuer); }

            if ( HasFlag(types, ClaimType.MobilePhone) ) { span[size++] = ClaimType.MobilePhone.ToClaim(self.PhoneNumber, issuer); }

            if ( HasFlag(types, ClaimType.WebSite) ) { span[size] = ClaimType.WebSite.ToClaim(self.Website, issuer); }
        }

        public Claim[] GetClaims( in ClaimType types = CLAIM_TYPES, in string? issuer = null )
        {
            using IMemoryOwner<Claim> claims = MemoryPool<Claim>.Shared.Rent(20);
            Span<Claim>               span   = claims.Memory.Span;
            int                       size   = 0;
            self.GetClaims(span, ref size, types, issuer);
            return [.. span];
        }
    }



    public static Claim[] GetClaims<TUser, TID, TAddress, TGroupModel, TRoleModel>( this TUser model, in ClaimType types = CLAIM_TYPES, in string? issuer = null )
        where TUser : IUserData<TID, TAddress, TGroupModel, TRoleModel>, IUserDetails
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
        where TGroupModel : IGroupModel<TID>, IEquatable<TGroupModel>
        where TRoleModel : IRoleModel<TID>, IEquatable<TRoleModel>
        where TAddress : IAddress<TID>, IEquatable<TAddress>
    {
        using ArrayBuffer<Claim> owner = new(20 + model.Groups.Count + model.Roles.Count + model.Addresses.Count * 5);
        int                      size  = 0;
        Span<Claim>              span  = owner.Span;
        model.GetClaims(span, ref size, types, issuer);

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



    extension( IUserData user )
    {
        public DateTimeOffset GetExpires( scoped in TimeSpan offset ) => user.GetExpires(DateTimeOffset.UtcNow, offset);

        public DateTimeOffset GetExpires( scoped in DateTimeOffset now, scoped in TimeSpan offset )
        {
            DateTimeOffset date = now + offset;
            if ( user.SubscriptionExpires is null ) { return date; }

            DateTimeOffset expires = user.SubscriptionExpires.Value;

            return date > expires
                       ? expires
                       : date;
        }
    }
}
