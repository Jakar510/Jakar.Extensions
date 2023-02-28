// Jakar.Extensions :: Jakar.Database
// 01/29/2023  1:51 PM

namespace Jakar.Database.DbMigrations;


[Migration( 6 )]

// ReSharper disable once InconsistentNaming
public sealed class CreateTable_RecoveryCode : MigrateRecoveryCodeTable
{
    public CreateTable_RecoveryCode() : base() { }
    public override void Up()
    {
        CheckSchema();
        CreateTable();
    }
    public override void Down() => DeleteTable();
    
}