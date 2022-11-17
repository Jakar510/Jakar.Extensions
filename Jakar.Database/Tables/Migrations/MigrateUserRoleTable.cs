// Jakar.Extensions :: Jakar.Database
// 10/11/2022  12:10 AM

namespace Jakar.Database.Migrations;


public abstract class MigrateUserRoleTable : Migration<UserRoleRecord>
{
    protected MigrateUserRoleTable() : base() { }
    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(UserRoleRecord.RoleID) )
             .AsInt64()
             .NotNullable();

        return table;
    }


    public override void Down() => DeleteTable();
}
