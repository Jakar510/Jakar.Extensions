// Jakar.Extensions :: Jakar.Database
// 10/11/2022  12:10 AM

namespace Jakar.Database.Migrations;


public abstract class MigrateUserRoleTable : Migration<UserRoleRecord>
{
    protected MigrateUserRoleTable() : base() { }


    public override void Down() => DeleteTable();
    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        var table = base.CreateTable();

        table.WithColumn( nameof(UserRoleRecord.RoleID) )
             .AsInt64()
             .NotNullable();

        return table;
    }
}