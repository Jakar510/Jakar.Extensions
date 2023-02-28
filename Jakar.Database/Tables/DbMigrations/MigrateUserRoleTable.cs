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
             .AsString( 256 )
             .NotNullable();

        table.WithColumn( nameof(UserRoleRecord.ValueID) )
             .AsString( 256 )
             .NotNullable();

        return table;
    }
}
