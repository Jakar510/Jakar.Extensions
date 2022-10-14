// Jakar.Extensions :: Jakar.Database
// 10/14/2022  1:53 PM

using Jakar.Database.Migrations;



namespace Jakar.Database.FluentMigrations;


public abstract class Create_RoleTable: Migration<RoleRecord>
{
    protected Create_RoleTable( string currentScheme ) : base( currentScheme ) { }


    public override void Down() => DeleteTable();
    public override void Up()
    {
        CheckSchema();
        ICreateTableWithColumnSyntax table = CreateTable();


        table.WithColumn( nameof(RoleRecord.ID) )
             .AsInt64()
             .NotNullable()
             .PrimaryKey()
             .Identity();
        
        table.WithColumn( nameof(RoleRecord.UserID) )
             .AsGuid()
             .NotNullable();

        table.WithColumn( nameof(RoleRecord.Name) )
             .AsString(1024)
             .NotNullable();

        table.WithColumn( nameof(RoleRecord.NormalizedName) )
             .AsString(1024)
             .NotNullable();

        table.WithColumn( nameof(RoleRecord.ConcurrencyStamp) )
             .AsString(4096)
             .NotNullable();

        table.WithColumn( nameof(RoleRecord.DateCreated) )
             .AsDateTime2()
             .NotNullable();

        table.WithColumn( nameof(RoleRecord.CreatedBy) )
             .AsInt64()
             .Nullable();
    }
}
