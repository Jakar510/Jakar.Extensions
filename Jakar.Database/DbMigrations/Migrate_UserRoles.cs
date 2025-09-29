// Jakar.Extensions :: Jakar.Database
// 10/11/2022  12:10 AM

namespace Jakar.Database.DbMigrations;


public abstract class Migrate_UserRoles : Migration<UserRoleRecord, UserRecord, RoleRecord>
{
    protected Migrate_UserRoles() : base() { }
}
