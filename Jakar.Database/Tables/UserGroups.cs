// TrueLogic :: TrueLogic.Common.Hosting
// 02/17/2023  2:39 PM

using System.Runtime.Versioning;

namespace Jakar.Database;


[Serializable]
[Table( "UserGroups" )]
public sealed record UserGroups : Mapping<UserGroups, UserRecord, GroupRecord>, ICreateMapping<UserGroups, UserRecord, GroupRecord>
{
    public UserGroups() : base() { }
    public UserGroups( UserRecord               owner, GroupRecord value ) : base( owner, value ) { }
    [RequiresPreviewFeatures]  public static UserGroups Create( UserRecord owner, GroupRecord value ) => new(owner, value);
}
