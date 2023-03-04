// Jakar.Extensions :: Jakar.Database
// 10/11/2022  12:10 AM

namespace Jakar.Database.DbMigrations;


public abstract class MigrateUserRoleTable : Migration<UserRoleRecord>
{
    protected MigrateUserRoleTable() : base() { }
    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(UserRoleRecord.KeyID) )
             .AsGuid()
             .NotNullable();

        table.WithColumn( nameof(UserRoleRecord.ValueID) )
             .AsGuid()
             .NotNullable();

        return table;
    }
}
