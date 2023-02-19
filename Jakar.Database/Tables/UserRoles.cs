// TrueLogic :: TrueLogic.Common.Hosting
// 02/17/2023  2:40 PM

namespace Jakar.Database;


[Serializable]
[Table( "UserRoles" )]
public sealed record UserRoles : Mapping<UserRoles, RoleRecord>, ICreateMapping<UserRoles, RoleRecord>
{
    public UserRoles() : base() { }
    public UserRoles( UserRecord                                         owner, RoleRecord value ) : base( owner, value ) { }
    [RequiresPreviewFeatures] public static UserRoles Create( UserRecord owner, RoleRecord value ) => new(owner, value);
}
