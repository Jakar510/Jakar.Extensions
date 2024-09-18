// Jakar.Extensions :: Jakar.Database
// 01/30/2023  9:44 PM

namespace Jakar.Database.DbMigrations;


[Migration( USER_LOGIN_INFO )]

// ReSharper disable once InconsistentNaming
public sealed class CreateTable_UserLoginInfo : Migrate_UserLoginInfos
{
    public CreateTable_UserLoginInfo() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
