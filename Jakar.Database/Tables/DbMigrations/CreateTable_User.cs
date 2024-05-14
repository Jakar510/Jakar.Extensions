namespace Jakar.Database.DbMigrations;


[Migration( USERS )]

// ReSharper disable once InconsistentNaming
public sealed class CreateTable_User : Migrate_Users
{
    public CreateTable_User() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
