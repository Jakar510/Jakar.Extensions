// Jakar.AppLogger :: Jakar.Database
// 10/26/2022  10:54 AM

namespace Jakar.Database.Migrations;


public abstract class MigrateUserGroupRecord : Migration<UserGroupRecord>
{
    protected MigrateUserGroupRecord() : base() { }
    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(UserGroupRecord.KeyID) )
             .AsString( 256 )
             .NotNullable();

        table.WithColumn( nameof(UserGroupRecord.ValueID) )
             .AsString( 256 )
             .NotNullable();

        return table;
    }
}
