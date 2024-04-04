// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  15:2

using System.Security.Claims;



namespace Jakar.Extensions;


[Flags]
public enum ClaimType : long
{
    None                                                                         = 1 << 0,
    [Display( Description = ClaimTypes.Sid )]             UserID                 = 1 << 1,
    [Display( Description = ClaimTypes.NameIdentifier )]  UserName               = 1 << 2,
    [Display( Description = ClaimTypes.GivenName )]       FirstName              = 1 << 3,
    [Display( Description = ClaimTypes.Surname )]         LastName               = 1 << 4,
    [Display( Description = ClaimTypes.Name )]            FullName               = 1 << 5,
    [Display( Description = ClaimTypes.Gender )]          Gender                 = 1 << 7,
    [Display( Description = ClaimTypes.Expiration )]      SubscriptionExpiration = 1 << 8,
    [Display( Description = ClaimTypes.Expired )]         Expired                = 1 << 9,
    [Display( Description = ClaimTypes.Email )]           Email                  = 1 << 10,
    [Display( Description = ClaimTypes.MobilePhone )]     MobilePhone            = 1 << 11,
    [Display( Description = ClaimTypes.StreetAddress )]   StreetAddressLine1     = 1 << 12,
    [Display( Description = ClaimTypes.Locality )]        StreetAddressLine2     = 1 << 13,
    [Display( Description = ClaimTypes.StateOrProvince )] StateOrProvince        = 1 << 14,
    [Display( Description = ClaimTypes.Country )]         Country                = 1 << 15,
    [Display( Description = ClaimTypes.PostalCode )]      PostalCode             = 1 << 16,
    [Display( Description = ClaimTypes.Webpage )]         WebSite                = 1 << 17,
    [Display( Description = ClaimTypes.GroupSid )]        GroupSid               = 1 << 18,
    [Display( Description = ClaimTypes.Role )]            Role                   = 1 << 19,
    All                                                                          = ~0
}



public static class ClaimTypeExtensions
{
    public const string ALL = "http://schemas.microsoft.com/ws/2008/06/identity/claims/all";


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool HasFlag( ClaimType value, ClaimType flag ) => (value & flag) != 0;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string ToClaimTypes( this ClaimType type ) => type switch
                                                                {
                                                                    ClaimType.None                   => string.Empty,
                                                                    ClaimType.UserID                 => ClaimTypes.Sid,
                                                                    ClaimType.UserName               => ClaimTypes.NameIdentifier,
                                                                    ClaimType.FirstName              => ClaimTypes.GivenName,
                                                                    ClaimType.LastName               => ClaimTypes.Surname,
                                                                    ClaimType.FullName               => ClaimTypes.Name,
                                                                    ClaimType.Gender                 => ClaimTypes.Gender,
                                                                    ClaimType.SubscriptionExpiration => ClaimTypes.Expiration,
                                                                    ClaimType.Expired                => ClaimTypes.Expired,
                                                                    ClaimType.Email                  => ClaimTypes.Email,
                                                                    ClaimType.MobilePhone            => ClaimTypes.MobilePhone,
                                                                    ClaimType.StreetAddressLine1     => ClaimTypes.StreetAddress,
                                                                    ClaimType.StreetAddressLine2     => ClaimTypes.Locality,
                                                                    ClaimType.StateOrProvince        => ClaimTypes.StateOrProvince,
                                                                    ClaimType.Country                => ClaimTypes.Country,
                                                                    ClaimType.PostalCode             => ClaimTypes.PostalCode,
                                                                    ClaimType.WebSite                => ClaimTypes.Webpage,
                                                                    ClaimType.GroupSid               => ClaimTypes.GroupSid,
                                                                    ClaimType.Role                   => ClaimTypes.Role,
                                                                    ClaimType.All                    => ALL,
                                                                    _                                => throw new OutOfRangeException( nameof(type), type )
                                                                };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ClaimType? FromClaimTypes( this string type ) => type switch
                                                                   {
                                                                       ""                         => ClaimType.None,
                                                                       ClaimTypes.Sid             => ClaimType.UserID,
                                                                       ClaimTypes.NameIdentifier  => ClaimType.UserName,
                                                                       ClaimTypes.GivenName       => ClaimType.FirstName,
                                                                       ClaimTypes.Surname         => ClaimType.LastName,
                                                                       ClaimTypes.Name            => ClaimType.FullName,
                                                                       ClaimTypes.Gender          => ClaimType.Gender,
                                                                       ClaimTypes.Expiration      => ClaimType.SubscriptionExpiration,
                                                                       ClaimTypes.Expired         => ClaimType.Expired,
                                                                       ClaimTypes.Email           => ClaimType.Email,
                                                                       ClaimTypes.MobilePhone     => ClaimType.MobilePhone,
                                                                       ClaimTypes.StreetAddress   => ClaimType.StreetAddressLine1,
                                                                       ClaimTypes.Locality        => ClaimType.StreetAddressLine2,
                                                                       ClaimTypes.StateOrProvince => ClaimType.StateOrProvince,
                                                                       ClaimTypes.Country         => ClaimType.Country,
                                                                       ClaimTypes.PostalCode      => ClaimType.PostalCode,
                                                                       ClaimTypes.Webpage         => ClaimType.WebSite,
                                                                       ClaimTypes.GroupSid        => ClaimType.GroupSid,
                                                                       ClaimTypes.Role            => ClaimType.Role,
                                                                       ALL                        => ClaimType.All,
                                                                       _                          => null
                                                                   };
}
