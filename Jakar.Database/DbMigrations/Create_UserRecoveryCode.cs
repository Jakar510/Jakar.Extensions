// Jakar.Extensions :: Jakar.Database
// 04/11/2023  9:48 PM

namespace Jakar.Database.DbMigrations;


[Migration(CREATE_USER_RECOVERY_CODE)]
public sealed class Create_UserRecoveryCode : Migrate_UserRecoveryCodes
{
    public Create_UserRecoveryCode() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
