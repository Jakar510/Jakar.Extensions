// Jakar.Extensions :: Jakar.Database
// 01/30/2023  9:44 PM

namespace Jakar.Database.DbMigrations;


[Migration( CREATE_USER_LOGIN_INFO )]

// ReSharper disable once InconsistentNaming
public sealed class Create_UserLoginInfo : Migrate_UserLoginInfos
{
    public Create_UserLoginInfo() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
