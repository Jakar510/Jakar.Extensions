// Jakar.Extensions :: Jakar.Database
// 04/10/2024  19:04

namespace Jakar.Database.DbMigrations;


[Migration( CREATE_FILES )]
public sealed class Create_Files : Migrate_Files
{
    public override void Up()   => CreateTable();
    public override void Down() => Delete.Table( FileRecord.TableName );
}
