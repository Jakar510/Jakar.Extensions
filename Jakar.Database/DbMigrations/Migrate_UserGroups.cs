// Jakar.AppLogger :: Jakar.Database
// 10/26/2022  10:54 AM

namespace Jakar.Database.DbMigrations;


public abstract class Migrate_UserGroups : Migration<UserGroupRecord, UserRecord, GroupRecord>
{
    protected Migrate_UserGroups() : base() { }
}
