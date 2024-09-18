namespace Jakar.Database.DbMigrations;


[Migration( CREATE_ROLES )]

// ReSharper disable once InconsistentNaming
public sealed class Create_Role : Migrate_Roles
{
    public Create_Role() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
