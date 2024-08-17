// Jakar.Extensions :: Jakar.Database
// 10/11/2022  12:10 AM

namespace Jakar.Database.DbMigrations;


public abstract class Migrate_UserRoles : Migration<UserRoleRecord>
{
    protected Migrate_UserRoles() : base() { }
    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(UserRoleRecord.KeyID) ).AsGuid().NotNullable().ForeignKey( $"{nameof(UserRoleRecord.KeyID)}.{nameof(UserRecord.ID)}", UserRecord.TableName, nameof(UserRecord.ID) );

        table.WithColumn( nameof(UserRoleRecord.ValueID) ).AsGuid().NotNullable().ForeignKey( $"{nameof(UserRoleRecord.ValueID)}.{nameof(RoleRecord.ID)}", RoleRecord.TableName, nameof(RoleRecord.ID) );

        return table;
    }
}
