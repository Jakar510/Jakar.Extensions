// Jakar.Extensions :: Jakar.Database
// 08/18/2022  10:35 PM

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;



namespace Jakar.Database;


public sealed class RequireMfa : IAuthorizationRequirement
{
    public static RequireMfa Instance { get; } = new();
}
