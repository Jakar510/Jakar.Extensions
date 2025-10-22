namespace Jakar.Database;


// TODO: asp.net authorization dapper
public static partial class DbExtensions
{
    public static bool TryParse( this ClaimsPrincipal principal, out RecordID<UserRecord> userID )
    {
        Claim? claim = principal.Claims.FirstOrDefault(Claims.IsUserID);

        if ( Guid.TryParse(claim?.Value, out Guid id) )
        {
            userID = RecordID<UserRecord>.Create(id);
            return true;
        }

        userID = RecordID<UserRecord>.Empty;
        return false;
    }
    public static bool TryParse( this ClaimsPrincipal principal, [NotNullWhen(true)] out RecordID<UserRecord>? userID )
    {
        Claim? claim = principal.Claims.FirstOrDefault(Claims.IsUserID);

        if ( Guid.TryParse(claim?.Value, out Guid id) )
        {
            userID = RecordID<UserRecord>.Create(id);
            return true;
        }

        userID = null;
        return false;
    }
    public static bool TryParse( this ClaimsPrincipal principal, out RecordID<UserRecord> userID, out string userName ) => TryParse(principal.Claims.ToArray(), out userID, out userName);
    public static bool TryParse( this ReadOnlySpan<Claim> claims, out RecordID<UserRecord> userID, out string userName )
    {
        userName = claims.FirstOrDefault(static ( ref readonly Claim x ) => x.IsUserName())?.Value ?? EMPTY;

        if ( Guid.TryParse(claims.FirstOrDefault(static ( ref readonly Claim x ) => x.IsUserID())?.Value, out Guid id) )
        {
            userID = RecordID<UserRecord>.Create(id);
            return true;
        }

        userID = RecordID<UserRecord>.Empty;
        return false;
    }
    public static bool TryParse( this ClaimsPrincipal principal, [NotNullWhen(true)] out RecordID<UserRecord>? userID, out string userName ) => TryParse(principal.Claims.ToArray(), out userID, out userName);
    public static bool TryParse( this ReadOnlySpan<Claim> claims, [NotNullWhen(true)] out RecordID<UserRecord>? userID, out string userName )
    {
        userName = claims.FirstOrDefault(static ( ref readonly Claim x ) => x.IsUserName())?.Value ?? EMPTY;

        if ( Guid.TryParse(claims.FirstOrDefault(static ( ref readonly Claim x ) => x.IsUserID())?.Value, out Guid id) )
        {
            userID = RecordID<UserRecord>.Create(id);
            return true;
        }

        userID = null;
        return false;
    }
    public static bool TryParse( this ClaimsPrincipal principal, out RecordID<UserRecord> userID, out string userName, out Claim[] roles, out Claim[] groups ) => TryParse(principal.Claims.ToArray(), out userID, out userName, out roles, out groups);
    public static bool TryParse( this ReadOnlySpan<Claim> claims, out RecordID<UserRecord> userID, out string userName, out Claim[] roles, out Claim[] groups )
    {
        roles    = claims.AsValueEnumerable().Where(Claims.IsRole).ToArray();
        groups   = claims.AsValueEnumerable().Where(Claims.IsGroup).ToArray();
        userName = claims.FirstOrDefault(Claims.IsUserName)?.Value ?? EMPTY;

        if ( Guid.TryParse(claims.FirstOrDefault(Claims.IsUserID)?.Value, out Guid id) )
        {
            userID = RecordID<UserRecord>.Create(id);
            return true;
        }

        userID = RecordID<UserRecord>.Empty;
        return false;
    }
    public static bool TryParse( this ClaimsPrincipal principal, [NotNullWhen(true)] out RecordID<UserRecord>? userID, out string userName, out Claim[] roles, out Claim[] groups ) => TryParse(principal.Claims.ToArray(), out userID, out userName, out roles, out groups);
    public static bool TryParse( this ReadOnlySpan<Claim> claims, [NotNullWhen(true)] out RecordID<UserRecord>? userID, out string userName, out Claim[] roles, out Claim[] groups )
    {
        roles    = claims.AsValueEnumerable().Where(Claims.IsRole).ToArray();
        groups   = claims.AsValueEnumerable().Where(Claims.IsGroup).ToArray();
        userName = claims.FirstOrDefault(Claims.IsUserName)?.Value ?? EMPTY;

        if ( Guid.TryParse(claims.FirstOrDefault(Claims.IsUserID)?.Value, out Guid id) )
        {
            userID = RecordID<UserRecord>.Create(id);
            return true;
        }

        userID = null;
        return false;
    }


    public static IServiceCollection AddAuth<TValue>( this IServiceCollection services )
        where TValue : class, IAuthenticationService
    {
        services.AddHttpContextAccessor();
        services.AddTransient<IAuthenticationService, TValue>();
        return services;
    }
}
