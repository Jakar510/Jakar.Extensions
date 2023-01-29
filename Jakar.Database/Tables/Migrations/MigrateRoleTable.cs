// Jakar.Extensions :: Jakar.Database
// 10/14/2022  1:53 PM

namespace Jakar.Database.Migrations;


public abstract class MigrateRoleTable : Migration<RoleRecord>
{
    protected MigrateRoleTable() : base() { }


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(RoleRecord.Name) )
             .AsString( 1024 )
             .NotNullable();

        table.WithColumn( nameof(RoleRecord.NormalizedName) )
             .AsString( 1024 )
             .NotNullable();

        table.WithColumn( nameof(RoleRecord.ConcurrencyStamp) )
             .AsString( 4096 )
             .NotNullable();

        return table;
    }
}
