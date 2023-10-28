using FluentMigrator;
using FluentMigrator.Builders.Create.Table;



namespace Jakar.AppLogger.Portal.Data.DbMigrations;


[ Migration( 104 ) ]

// ReSharper disable once InconsistentNaming
public sealed class Create_AttachmentRecord : LoggerMigration<LoggerAttachmentRecord>
{
    public Create_AttachmentRecord() : base() { }


    public override void Down() => DeleteTable();
    public override void Up()
    {
        ICreateTableWithColumnSyntax table = CreateTable();

        table.WithColumn( nameof(LoggerAttachmentRecord.IsBinary) ).AsBoolean().NotNullable();

        table.WithColumn( nameof(LoggerAttachmentRecord.SessionID) ).AsGuid().NotNullable();

        table.WithColumn( nameof(LoggerAttachmentRecord.AppID) ).AsInt64().NotNullable();

        table.WithColumn( nameof(LoggerAttachmentRecord.DeviceID) ).AsInt64().NotNullable();

        table.WithColumn( nameof(LoggerAttachmentRecord.Length) ).AsInt64().NotNullable();

        table.WithColumn( nameof(LoggerAttachmentRecord.LogID) ).AsInt64().NotNullable();

        table.WithColumn( nameof(LoggerAttachmentRecord.Content) ).AsString( 2 ^ 20 ).NotNullable();

        table.WithColumn( nameof(LoggerAttachmentRecord.Description) ).AsString( 1024 ).Nullable();

        table.WithColumn( nameof(LoggerAttachmentRecord.FileName) ).AsString( 256 ).Nullable();

        table.WithColumn( nameof(LoggerAttachmentRecord.Type) ).AsString( 256 ).Nullable();
    }
}
