namespace Jakar.Database.DbMigrations;


[Migration( ROLES )]

// ReSharper disable once InconsistentNaming
public sealed class CreateTable_Role : Migrate_Roles
{
    public CreateTable_Role() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
