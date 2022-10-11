// Jakar.Extensions :: Jakar.Database
// 10/11/2022  12:10 AM

using Jakar.Database.Migrations;



namespace Jakar.Database.FluentMigrations;


public abstract class Create_UserRoleTable: Migration<UserRoleRecord>
{
    protected Create_UserRoleTable( string currentScheme ) : base( currentScheme ) { }


    public override void Down() => DeleteTable();
    public override void Up()
    {
        CheckSchema();
        ICreateTableWithColumnSyntax table = CreateTable();


        table.WithColumn( nameof(UserRoleRecord.ID) )
             .AsInt64()
             .NotNullable()
             .PrimaryKey()
             .Identity();
        
        table.WithColumn( nameof(UserRoleRecord.UserID) )
             .AsGuid()
             .NotNullable();

        table.WithColumn( nameof(UserRoleRecord.RoleID) )
             .AsInt64()
             .NotNullable();

        table.WithColumn( nameof(UserRoleRecord.DateCreated) )
             .AsDateTime2()
             .NotNullable();

        table.WithColumn( nameof(UserRoleRecord.CreatedBy) )
             .AsInt64()
             .Nullable();
    }
}
