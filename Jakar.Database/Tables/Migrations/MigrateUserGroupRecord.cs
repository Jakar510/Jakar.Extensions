// Jakar.AppLogger :: Jakar.Database
// 10/26/2022  10:54 AM

namespace Jakar.Database.Migrations;


public abstract class MigrateUserGroupRecord: Migration<UserGroupRecord>
{
    protected MigrateUserGroupRecord() : base() { }


    public override void Down() => DeleteTable();
    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        var table = base.CreateTable();

        table.WithColumn( nameof(UserGroupRecord.GroupID) )
             .AsInt64()
             .NotNullable();

        return table;
    }
}
