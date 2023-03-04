// Jakar.Extensions :: Jakar.Database
// 03/03/2023  6:12 PM

namespace Jakar.Database;


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
    [Display( Description = ClaimTypes.StateOrProvince )] State                  = 1 << 14,
    [Display( Description = ClaimTypes.Country )]         Country                = 1 << 15,
    [Display( Description = ClaimTypes.PostalCode )]      PostalCode             = 1 << 16,
    [Display( Description = ClaimTypes.Webpage )]         WebSite                = 1 << 17,
    [Display( Description = ClaimTypes.GroupSid )]        Groups                 = 1 << 18,
    [Display( Description = ClaimTypes.Role )]            Roles                  = 1 << 19,
    All                                                                          = ~0
}
