namespace Jakar.Database.DbMigrations;


[Migration( 2 )]
// ReSharper disable once InconsistentNaming
public sealed class CreateTable_Role : MigrateRoleTable
{
    public CreateTable_Role() : base() { }
    public override void Up()
    {
        CheckSchema();
        CreateTable();
    }
    public override void Down() => DeleteTable();

    
}