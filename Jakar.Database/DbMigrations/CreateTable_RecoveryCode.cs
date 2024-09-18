// Jakar.Extensions :: Jakar.Database
// 01/29/2023  1:51 PM

namespace Jakar.Database.DbMigrations;


[Migration( RECOVERY_CODE )]

// ReSharper disable once InconsistentNaming
public sealed class CreateTable_RecoveryCode : Migrate_RecoveryCodes
{
    public CreateTable_RecoveryCode() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
