﻿// Jakar.Database ::  Jakar.Database 
// 02/17/2023  2:40 PM

namespace Jakar.Database;


[Serializable,Table( "UserRoles" )]
public sealed record UserRoleRecord : Mapping<UserRoleRecord, UserRecord, RoleRecord>, ICreateMapping<UserRoleRecord, UserRecord, RoleRecord>
{
    public UserRoleRecord() : base() { }
    public UserRoleRecord( UserRecord                                         owner, RoleRecord value ) : base( owner, value ) { }
    [RequiresPreviewFeatures] public static UserRoleRecord Create( UserRecord owner, RoleRecord value ) => new(owner, value);
}
