// TrueLogic :: TrueLogic.Common.Hosting
// 04/12/2023  11:34 AM

namespace Jakar.Database;


public sealed class RoleValidator : RoleValidator<RoleRecord>
{
    public override Task<IdentityResult> ValidateAsync( RoleManager<RoleRecord> manager, RoleRecord role )
    {
        IdentityResult result = string.IsNullOrWhiteSpace( role.Name )
                                    ? IdentityResult.Failed( new IdentityError
                                                             {
                                                                 Description = "Name of Role Invalid",
                                                                 Code        = nameof(RoleRecord.Name)
                                                             } )
                                    : IdentityResult.Success;

        return Task.FromResult( result );
    }
}
