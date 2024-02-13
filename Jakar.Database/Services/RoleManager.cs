// Jakar.Database ::  Jakar.Database 
// 04/11/2023  7:56 PM

namespace Jakar.Database;


public sealed class RoleManager( RoleStore store, IEnumerable<IRoleValidator<RoleRecord>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager> logger ) : RoleManager<RoleRecord>( store, roleValidators, keyNormalizer, errors, logger );
