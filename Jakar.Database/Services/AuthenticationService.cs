#if DEBUG
using Microsoft.AspNetCore.Authorization;



namespace Jakar.Database;


public sealed class AuthorizationService : IAuthorizationService
{
    public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements) => null;
    public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName) => null;
}
#endif
