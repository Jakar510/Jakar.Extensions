using FluentMigrator;
using FluentMigrator.Builders.Create.Table;



namespace Jakar.AppLogger.Portal.Data.DbMigrations;


[Migration( 106 )]

// ReSharper disable once InconsistentNaming
public sealed class Create_LogRecord : LoggerMigration<LogRecord>
{
    public Create_LogRecord() : base() { }


    public override void Down() => DeleteTable();
    public override void Up()
    {
        CheckSchema();
        ICreateTableWithColumnSyntax table = CreateTable();


        table.WithColumn( nameof(LogRecord.IsError) )
             .AsBoolean()
             .NotNullable();

        table.WithColumn( nameof(LogRecord.IsFatal) )
             .AsBoolean()
             .NotNullable();

        table.WithColumn( nameof(LogRecord.IsValid) )
             .AsBoolean()
             .NotNullable();

        table.WithColumn( nameof(LogRecord.AppStartTime) )
             .AsDateTime2()
             .NotNullable();

        table.WithColumn( nameof(LogRecord.TimeStamp) )
             .AsDateTime2()
             .NotNullable();

        table.WithColumn( nameof(LogRecord.Timestamp) )
             .AsDateTime2()
             .NotNullable();

        table.WithColumn( nameof(LogRecord.SessionID) )
             .AsGuid()
             .NotNullable();

        table.WithColumn( nameof(LogRecord.ScopeID) )
             .AsGuid()
             .NotNullable();

        table.WithColumn( nameof(LogRecord.EventID) )
             .AsInt32()
             .NotNullable();

        table.WithColumn( nameof(LogRecord.Level) )
             .AsInt64()
             .NotNullable();

        table.WithColumn( nameof(LogRecord.AppID) )
             .AsInt64()
             .NotNullable();

        table.WithColumn( nameof(LogRecord.DeviceID) )
             .AsInt64()
             .NotNullable();

        table.WithColumn( nameof(LogRecord.Message) )
             .AsString( int.MaxValue )
             .NotNullable();

        table.WithColumn( nameof(LogRecord.AppUserID) )
             .AsString( 1024 )
             .NotNullable();

        table.WithColumn( nameof(LogRecord.BuildID) )
             .AsString( 1024 )
             .NotNullable();

        table.WithColumn( nameof(LogRecord.EventName) )
             .AsString( 1024 )
             .NotNullable();

        table.WithColumn( nameof(LogRecord.ExceptionDetails) )
             .AsString( int.MaxValue )
             .NotNullable();

        table.WithColumn( nameof(LogRecord.StackTrace) )
             .AsString( int.MaxValue )
             .NotNullable();
    }
}
