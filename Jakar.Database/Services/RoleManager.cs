// Jakar.Database ::  Jakar.Database 
// 04/11/2023  7:56 PM

namespace Jakar.Database;


public sealed class RoleManager : RoleManager<RoleRecord>
{
    public RoleManager( IRoleStore<RoleRecord> store, IEnumerable<IRoleValidator<RoleRecord>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<RoleRecord>> logger ) :
        base( store, roleValidators, keyNormalizer, errors, logger ) { }
}
