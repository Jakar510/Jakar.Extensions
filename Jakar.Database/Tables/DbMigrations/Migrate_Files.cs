// Jakar.Extensions :: Jakar.Database
// 04/10/2024  19:04

using MimeTypes = Jakar.Extensions.MimeTypes;



namespace Jakar.Database.DbMigrations;


public abstract class Migrate_Files : Migration<FileRecord>
{
    protected Migrate_Files() : base() { }


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(FileRecord.FileName) ).AsString( UNICODE_CAPACITY ).NotNullable();

        table.WithColumn( nameof(FileRecord.FileDescription) ).AsString( UNICODE_CAPACITY ).NotNullable();

        table.WithColumn( nameof(FileRecord.FileType) ).AsString( UNICODE_CAPACITY ).NotNullable();

        table.WithColumn( nameof(FileRecord.FileSize) ).AsInt64().NotNullable();

        table.WithColumn( nameof(FileRecord.Hash) ).AsString( UNICODE_CAPACITY ).NotNullable();

        table.WithColumn( nameof(FileRecord.MimeType) ).AsString( MimeTypes.Names.Values.Max( static x => x.Length ) ).NotNullable();

        table.WithColumn( nameof(FileRecord.Payload) ).AsString( BINARY_CAPACITY ).NotNullable();

        return table;
    }
}
