using NoAlloq;



namespace Jakar.Database;


// TODO: asp.net authorization dapper
public static partial class DbExtensions
{
    public static bool TryParse( this ClaimsPrincipal principal, out RecordID<UserRecord> userID )
    {
        Claim? claim = principal.Claims.FirstOrDefault( Claims.IsUserID );

        if ( Guid.TryParse( claim?.Value, out Guid id ) )
        {
            userID = RecordID<UserRecord>.Create( id );
            return true;
        }

        userID = RecordID<UserRecord>.Empty;
        return false;
    }
    public static bool TryParse( this ClaimsPrincipal principal, [NotNullWhen( true )] out RecordID<UserRecord>? userID )
    {
        Claim? claim = principal.Claims.FirstOrDefault( Claims.IsUserID );

        if ( Guid.TryParse( claim?.Value, out Guid id ) )
        {
            userID = RecordID<UserRecord>.Create( id );
            return true;
        }

        userID = null;
        return false;
    }
    public static bool TryParse( this ClaimsPrincipal principal, out RecordID<UserRecord> userID, out string userName ) => TryParse( principal.Claims.ToArray(), out userID, out userName );
    public static bool TryParse( this ReadOnlySpan<Claim> claims, out RecordID<UserRecord> userID, out string userName )
    {
        userName = Spans.FirstOrDefault( claims, Claims.IsUserName )?.Value ?? string.Empty;

        if ( Guid.TryParse( Spans.FirstOrDefault( claims, Claims.IsUserID )?.Value, out Guid id ) )
        {
            userID = RecordID<UserRecord>.Create( id );
            return true;
        }

        userID = RecordID<UserRecord>.Empty;
        return false;
    }
    public static bool TryParse( this ClaimsPrincipal principal, [NotNullWhen( true )] out RecordID<UserRecord>? userID, out string userName ) => TryParse( principal.Claims.ToArray(), out userID, out userName );
    public static bool TryParse( this ReadOnlySpan<Claim> claims, [NotNullWhen( true )] out RecordID<UserRecord>? userID, out string userName )
    {
        userName = Spans.FirstOrDefault( claims, Claims.IsUserName )?.Value ?? string.Empty;

        if ( Guid.TryParse( Spans.FirstOrDefault( claims, Claims.IsUserID )?.Value, out Guid id ) )
        {
            userID = RecordID<UserRecord>.Create( id );
            return true;
        }

        userID = null;
        return false;
    }
    public static bool TryParse( this ClaimsPrincipal principal, out RecordID<UserRecord> userID, out string userName, out Claim[] roles, out Claim[] groups ) => TryParse( principal.Claims.ToArray(), out userID, out userName, out roles, out groups );
    public static bool TryParse( this ReadOnlySpan<Claim> claims, out RecordID<UserRecord> userID, out string userName, out Claim[] roles, out Claim[] groups )
    {
        roles    = claims.Where( Claims.IsRole ).ToArray();
        groups   = claims.Where( Claims.IsGroup ).ToArray();
        userName = Spans.FirstOrDefault( claims, Claims.IsUserName )?.Value ?? string.Empty;

        if ( Guid.TryParse( Spans.FirstOrDefault( claims, Claims.IsUserID )?.Value, out Guid id ) )
        {
            userID = RecordID<UserRecord>.Create( id );
            return true;
        }

        userID = RecordID<UserRecord>.Empty;
        return false;
    }
    public static bool TryParse( this ClaimsPrincipal principal, [NotNullWhen( true )] out RecordID<UserRecord>? userID, out string userName, out Claim[] roles, out Claim[] groups ) => TryParse( principal.Claims.ToArray(), out userID, out userName, out roles, out groups );
    public static bool TryParse( this ReadOnlySpan<Claim> claims, [NotNullWhen( true )] out RecordID<UserRecord>? userID, out string userName, out Claim[] roles, out Claim[] groups )
    {
        roles    = claims.Where( Claims.IsRole ).ToArray();
        groups   = claims.Where( Claims.IsGroup ).ToArray();
        userName = Spans.FirstOrDefault( claims, Claims.IsUserName )?.Value ?? string.Empty;

        if ( Guid.TryParse( Spans.FirstOrDefault( claims, Claims.IsUserID )?.Value, out Guid id ) )
        {
            userID = RecordID<UserRecord>.Create( id );
            return true;
        }

        userID = null;
        return false;
    }


    public static IServiceCollection AddAuth<T>( this IServiceCollection services )
        where T : class, IAuthenticationService
    {
        services.AddHttpContextAccessor();
        services.AddTransient<IAuthenticationService, T>();
        return services;
    }
}
