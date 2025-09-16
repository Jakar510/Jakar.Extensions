// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  15:2

using System.Security.Claims;
using ZLinq;



namespace Jakar.Extensions;


[Flags]
public enum ClaimType : ulong
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
    [Display( Description = ClaimTypes.GroupSid )]        Group                  = 1 << 18,
    [Display( Description = ClaimTypes.Role )]            Role                   = 1 << 19,
    All                                                                          = ~0ul
}



public static class Claims
{
    public const string    ALL      = "http://schemas.microsoft.com/ws/2008/06/identity/claims/all";
    public const ClaimType DEFAULTS = ClaimType.UserID | ClaimType.UserName | ClaimType.Role | ClaimType.Group;


    public static bool TryParse( this ClaimsPrincipal principal, out Guid userID )
    {
        Claim? claim = principal.Claims.FirstOrDefault( CheckUserID );

        if ( Guid.TryParse( claim?.Value, out Guid id ) )
        {
            userID = id;
            return true;
        }

        userID = Guid.Empty;
        return false;
        static bool CheckUserID( Claim claim ) => claim.IsUserID();
    }
    public static bool TryParse( this ClaimsPrincipal principal, [NotNullWhen( true )] out Guid? userID )
    {
        Claim? claim = principal.Claims.FirstOrDefault( CheckUserID );

        if ( Guid.TryParse( claim?.Value, out Guid id ) )
        {
            userID = id;
            return true;
        }

        userID = null;
        return false;
        static bool CheckUserID( Claim claim ) => claim.IsUserID();
    }
    public static bool TryParse( this ClaimsPrincipal principal, out Guid userID, out string userName ) => TryParse( principal.Claims.ToArray(), out userID, out userName );
    public static bool TryParse( this ReadOnlySpan<Claim> claims, out Guid userID, out string userName )
    {
        userName = claims.FirstOrDefault( IsUserName )?.Value ?? string.Empty;

        if ( Guid.TryParse( claims.FirstOrDefault( IsUserID )?.Value, out Guid id ) )
        {
            userID = id;
            return true;
        }

        userID = Guid.Empty;
        return false;
    }
    public static bool TryParse( this ClaimsPrincipal principal, [NotNullWhen( true )] out Guid? userID, out string userName ) => TryParse( principal.Claims.ToArray(), out userID, out userName );
    public static bool TryParse( this ReadOnlySpan<Claim> claims, [NotNullWhen( true )] out Guid? userID, out string userName )
    {
        userName = claims.FirstOrDefault( IsUserName )?.Value ?? string.Empty;

        if ( Guid.TryParse( claims.FirstOrDefault( IsUserID )?.Value, out Guid id ) )
        {
            userID = id;
            return true;
        }

        userID = null;
        return false;
    }
    public static bool TryParse( this ClaimsPrincipal principal, out Guid userID, out string userName, out Claim[] roles, out Claim[] groups ) => TryParse( principal.Claims.ToArray(), out userID, out userName, out roles, out groups );
    public static bool TryParse( this ReadOnlySpan<Claim> claims, out Guid userID, out string userName, out Claim[] roles, out Claim[] groups )
    {
        roles    = claims.AsValueEnumerable().Where( CheckRole ).ToArray();
        groups   = claims.AsValueEnumerable().Where( CheckGroup ).ToArray();
        userName = claims.FirstOrDefault( IsUserName )?.Value ?? string.Empty;

        if ( Guid.TryParse( claims.FirstOrDefault( IsUserID )?.Value, out Guid id ) )
        {
            userID = id;
            return true;
        }

        userID = Guid.Empty;
        return false;
    }
    public static bool TryParse( this ClaimsPrincipal principal, [NotNullWhen( true )] out Guid? userID, out string userName, out Claim[] roles, out Claim[] groups ) => TryParse( principal.Claims.ToArray(), out userID, out userName, out roles, out groups );
    public static bool TryParse( this ReadOnlySpan<Claim> claims, [NotNullWhen( true )] out Guid? userID, out string userName, out Claim[] roles, out Claim[] groups )
    {
        roles    = claims.AsValueEnumerable().Where( CheckRole ).ToArray();
        groups   = claims.AsValueEnumerable().Where( CheckGroup ).ToArray();
        userName = claims.FirstOrDefault( IsUserName )?.Value ?? string.Empty;

        if ( Guid.TryParse( claims.FirstOrDefault( IsUserID )?.Value, out Guid id ) )
        {
            userID = id;
            return true;
        }

        userID = null;
        return false;
    }
    public static bool CheckRole( Claim  claim ) => claim.IsRole();
    public static bool CheckGroup( Claim claim ) => claim.IsGroup();


    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsAuthorized( this             IEnumerable<Claim> claims,    Guid userID ) => userID != Guid.Empty && !string.Equals( claims.FirstOrDefault( static x => x.IsUserID() )?.Value, userID.ToString(), StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsAuthorized( this             ClaimsPrincipal    principal, Guid userID ) => principal.Claims.IsAuthorized( userID );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsAuthorized( this             ClaimsIdentity     principal, Guid userID ) => principal.Claims.IsAuthorized( userID );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsUserID( this                 Claim              claim )                                              => string.Equals( claim.Type, ClaimType.UserID.ToClaimTypes(),                 StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsUserName( this               Claim              claim )                                              => string.Equals( claim.Type, ClaimType.UserName.ToClaimTypes(),               StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsFirstName( this              Claim              claim )                                              => string.Equals( claim.Type, ClaimType.FirstName.ToClaimTypes(),              StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsLastName( this               Claim              claim )                                              => string.Equals( claim.Type, ClaimType.LastName.ToClaimTypes(),               StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsFullName( this               Claim              claim )                                              => string.Equals( claim.Type, ClaimType.FullName.ToClaimTypes(),               StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsGender( this                 Claim              claim )                                              => string.Equals( claim.Type, ClaimType.Gender.ToClaimTypes(),                 StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsSubscriptionExpiration( this Claim              claim )                                              => string.Equals( claim.Type, ClaimType.SubscriptionExpiration.ToClaimTypes(), StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsExpired( this                Claim              claim )                                              => string.Equals( claim.Type, ClaimType.Expired.ToClaimTypes(),                StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsEmail( this                  Claim              claim )                                              => string.Equals( claim.Type, ClaimType.Email.ToClaimTypes(),                  StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsMobilePhone( this            Claim              claim )                                              => string.Equals( claim.Type, ClaimType.MobilePhone.ToClaimTypes(),            StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsStreetAddressLine1( this     Claim              claim )                                              => string.Equals( claim.Type, ClaimType.StreetAddressLine1.ToClaimTypes(),     StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsStreetAddressLine2( this     Claim              claim )                                              => string.Equals( claim.Type, ClaimType.StreetAddressLine2.ToClaimTypes(),     StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsStateOrProvince( this        Claim              claim )                                              => string.Equals( claim.Type, ClaimType.StateOrProvince.ToClaimTypes(),        StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsCountry( this                Claim              claim )                                              => string.Equals( claim.Type, ClaimType.Country.ToClaimTypes(),                StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsPostalCode( this             Claim              claim )                                              => string.Equals( claim.Type, ClaimType.PostalCode.ToClaimTypes(),             StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsWebSite( this                Claim              claim )                                              => string.Equals( claim.Type, ClaimType.WebSite.ToClaimTypes(),                StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsGroup( this                  Claim              claim )                                              => string.Equals( claim.Type, ClaimType.Group.ToClaimTypes(),                  StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool  IsRole( this                   Claim              claim )                                              => string.Equals( claim.Type, ClaimType.Role.ToClaimTypes(),                   StringComparison.Ordinal );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, DateOnly        value, string? issuer = null ) => new(type.ToClaimTypes(), value.ToString( CultureInfo.CurrentCulture ), ClaimValueTypes.Date, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, DateOnly?       value, string? issuer = null ) => new(type.ToClaimTypes(), value?.ToString( CultureInfo.CurrentCulture ) ?? string.Empty, ClaimValueTypes.Date, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, TimeOnly        value, string? issuer = null ) => new(type.ToClaimTypes(), value.ToString(), ClaimValueTypes.Time, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, TimeOnly?       value, string? issuer = null ) => new(type.ToClaimTypes(), value?.ToString() ?? string.Empty, ClaimValueTypes.Time, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, string?         value, string? issuer = null ) => new(type.ToClaimTypes(), value             ?? string.Empty, ClaimValueTypes.String, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, DateTime        value, string? issuer = null ) => new(type.ToClaimTypes(), value.ToString( CultureInfo.CurrentCulture ), ClaimValueTypes.DateTime, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, DateTime?       value, string? issuer = null ) => new(type.ToClaimTypes(), value?.ToString( CultureInfo.CurrentCulture ) ?? string.Empty, ClaimValueTypes.DateTime, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, DateTimeOffset  value, string? issuer = null ) => new(type.ToClaimTypes(), value.ToString( CultureInfo.CurrentCulture ), ClaimValueTypes.DateTime, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, DateTimeOffset? value, string? issuer = null ) => new(type.ToClaimTypes(), value?.ToString( CultureInfo.CurrentCulture ) ?? string.Empty, ClaimValueTypes.DateTime, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, TimeSpan        value, string? issuer = null ) => new(type.ToClaimTypes(), value.ToString(), ClaimValueTypes.DaytimeDuration, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, TimeSpan?       value, string? issuer = null ) => new(type.ToClaimTypes(), value?.ToString() ?? string.Empty, ClaimValueTypes.DaytimeDuration, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, double          value, string? issuer = null ) => new(type.ToClaimTypes(), value.ToString( CultureInfo.CurrentCulture ), ClaimValueTypes.Double, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, double?         value, string? issuer = null ) => new(type.ToClaimTypes(), value?.ToString( CultureInfo.CurrentCulture ) ?? string.Empty, ClaimValueTypes.Double, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, int             value, string? issuer = null ) => new(type.ToClaimTypes(), value.ToString(), ClaimValueTypes.Integer32, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, int?            value, string? issuer = null ) => new(type.ToClaimTypes(), value?.ToString() ?? string.Empty, ClaimValueTypes.Integer32, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, long            value, string? issuer = null ) => new(type.ToClaimTypes(), value.ToString(), ClaimValueTypes.Integer64, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, long?           value, string? issuer = null ) => new(type.ToClaimTypes(), value?.ToString() ?? string.Empty, ClaimValueTypes.Integer64, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, uint            value, string? issuer = null ) => new(type.ToClaimTypes(), value.ToString(), ClaimValueTypes.UInteger32, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, uint?           value, string? issuer = null ) => new(type.ToClaimTypes(), value?.ToString() ?? string.Empty, ClaimValueTypes.UInteger32, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, ulong           value, string? issuer = null ) => new(type.ToClaimTypes(), value.ToString(), ClaimValueTypes.UInteger64, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, ulong?          value, string? issuer = null ) => new(type.ToClaimTypes(), value?.ToString() ?? string.Empty, ClaimValueTypes.UInteger64, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, bool            value, string? issuer = null ) => new(type.ToClaimTypes(), value.ToString(), ClaimValueTypes.Boolean, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, bool?           value, string? issuer = null ) => new(type.ToClaimTypes(), value?.ToString() ?? string.Empty, ClaimValueTypes.Boolean, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, Email           value, string? issuer = null ) => new(type.ToClaimTypes(), value.ToString(), ClaimValueTypes.Email, issuer);
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static Claim ToClaim( this                  ClaimType          type, Email?          value, string? issuer = null ) => new(type.ToClaimTypes(), value?.ToString() ?? string.Empty, ClaimValueTypes.Email, issuer);


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool HasFlag<TValue>( in TValue value, in TValue flag )
        where TValue : struct, Enum => (value.AsULong() & flag.AsULong()) != 0;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool HasFlag( in ClaimType value, in ClaimType flag ) => (value & flag) != 0;


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
                                                                    ClaimType.Group                  => ClaimTypes.GroupSid,
                                                                    ClaimType.Role                   => ClaimTypes.Role,
                                                                    ClaimType.All                    => ALL,
                                                                    _                                => throw new OutOfRangeException( type )
                                                                };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ClaimType? FromClaimTypes( this string type ) => type switch
                                                                   {
                                                                       EMPTY                      => ClaimType.None,
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
                                                                       ClaimTypes.GroupSid        => ClaimType.Group,
                                                                       ClaimTypes.Role            => ClaimType.Role,
                                                                       ALL                        => ClaimType.All,
                                                                       _                          => null
                                                                   };
}
