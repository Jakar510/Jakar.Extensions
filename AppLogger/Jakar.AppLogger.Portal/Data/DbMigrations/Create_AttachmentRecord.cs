using FluentMigrator;
using FluentMigrator.Builders.Create.Table;



namespace Jakar.AppLogger.Portal.Data.DbMigrations;


[Migration( 104 )]
// ReSharper disable once InconsistentNaming
public sealed class Create_AttachmentRecord : LoggerMigration<AttachmentRecord>
{
    public Create_AttachmentRecord() : base() { }


    public override void Down() => DeleteTable();
    public override void Up()
    {
        CheckSchema();
        ICreateTableWithColumnSyntax table = CreateTable();
        
        table.WithColumn( nameof(AttachmentRecord.IsBinary) )
             .AsBoolean()
             .NotNullable();

        table.WithColumn( nameof(AttachmentRecord.SessionID) )
             .AsGuid()
             .NotNullable();

        table.WithColumn( nameof(AttachmentRecord.ScopeID) )
             .AsGuid()
             .Nullable();

        table.WithColumn( nameof(AttachmentRecord.AppID) )
             .AsInt64()
             .NotNullable();

        table.WithColumn( nameof(AttachmentRecord.DeviceID) )
             .AsInt64()
             .NotNullable();

        table.WithColumn( nameof(AttachmentRecord.Length) )
             .AsInt64()
             .NotNullable();

        table.WithColumn( nameof(AttachmentRecord.LogID) )
             .AsInt64()
             .NotNullable();

        table.WithColumn( nameof(AttachmentRecord.Content) )
             .AsString( 2 ^ 20 )
             .NotNullable();

        table.WithColumn( nameof(AttachmentRecord.Description) )
             .AsString( 1024 )
             .Nullable();

        table.WithColumn( nameof(AttachmentRecord.FileName) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(AttachmentRecord.Type) )
             .AsString( 256 )
             .Nullable();
    }
}