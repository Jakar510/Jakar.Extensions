// TrueLogic :: TrueLogic.Common.Hosting
// 02/17/2023  2:39 PM

namespace Jakar.Database;


[Serializable]
[Table( "UserGroups" )]
public sealed record UserGroups : Mapping<UserGroups, GroupRecord>, ICreateMapping<UserGroups, GroupRecord>
{
    public UserGroups() : base() { }
    public UserGroups( UserRecord               owner, GroupRecord value ) : base( owner, value ) { }
    [RequiresPreviewFeatures]  public static UserGroups Create( UserRecord owner, GroupRecord value ) => new(owner, value);
}
