// Jakar.Database ::  Jakar.Database 
// 04/12/2023  11:34 AM

namespace Jakar.Database;


public sealed class RoleValidator : RoleValidator<RoleRecord>
{
    public override Task<IdentityResult> ValidateAsync( RoleManager<RoleRecord> manager, RoleRecord role )
    {
        IdentityResult result = string.IsNullOrWhiteSpace( role.NameOfRole )
                                    ? IdentityResult.Failed( new IdentityError
                                                             {
                                                                 Description = "Name of Role Invalid",
                                                                 Code        = nameof(RoleRecord.NameOfRole)
                                                             } )
                                    : string.IsNullOrWhiteSpace( role.NormalizedName )
                                        ? IdentityResult.Failed( new IdentityError
                                                                 {
                                                                     Description = "NormalizedName of Role Invalid",
                                                                     Code        = nameof(RoleRecord.NormalizedName)
                                                                 } )
                                        : IdentityResult.Success;

        return Task.FromResult( result );
    }
}
