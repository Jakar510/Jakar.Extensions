// Jakar.Database ::  Jakar.Database 
// 02/17/2023  2:39 PM

namespace Jakar.Database;


[Serializable,Table( "UserGroups" )]
public sealed record UserGroupRecord : Mapping<UserGroupRecord, UserRecord, GroupRecord>, ICreateMapping<UserGroupRecord, UserRecord, GroupRecord>
{
    public UserGroupRecord() : base() { }
    public UserGroupRecord( UserRecord                                         owner, GroupRecord value ) : base( owner, value ) { }
    [RequiresPreviewFeatures] public static UserGroupRecord Create( UserRecord owner, GroupRecord value ) => new(owner, value);
}
