using FluentMigrator;
using FluentMigrator.Builders.Create.Table;



namespace Jakar.AppLogger.Portal.Data.DbMigrations;


[Migration( 105 )]

// ReSharper disable once InconsistentNaming
public sealed class Create_DeviceRecord : LoggerMigration<DeviceRecord>
{
    public Create_DeviceRecord() : base() { }


    public override void Down() => DeleteTable();
    public override void Up()
    {
        CheckSchema();
        ICreateTableWithColumnSyntax table = CreateTable();


        table.WithColumn( nameof(DeviceRecord.AppVersion) )
             .AsString( 512 )
             .NotNullable();

        table.WithColumn( nameof(DeviceRecord.TimeZoneOffset) )
             .AsTime()
             .NotNullable();

        table.WithColumn( nameof(DeviceRecord.AppBuild) )
             .AsInt32()
             .NotNullable();

        table.WithColumn( nameof(DeviceRecord.OsApiLevel) )
             .AsInt32()
             .NotNullable();

        table.WithColumn( nameof(DeviceRecord.DeviceID) )
             .AsString( 4096 )
             .NotNullable();

        table.WithColumn( nameof(DeviceRecord.Locale) )
             .AsString( 256 )
             .NotNullable();

        table.WithColumn( nameof(DeviceRecord.SdkName) )
             .AsString( 256 )
             .NotNullable();

        table.WithColumn( nameof(DeviceRecord.SdkVersion) )
             .AsString( 256 )
             .NotNullable();

        table.WithColumn( nameof(DeviceRecord.AppNamespace) )
             .AsString( 4096 )
             .Nullable();

        table.WithColumn( nameof(DeviceRecord.OsName) )
             .AsString( 256 )
             .NotNullable();

        table.WithColumn( nameof(DeviceRecord.OsVersion) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(DeviceRecord.ProcessArchitecture) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(DeviceRecord.Model) )
             .AsString( 4096 )
             .Nullable();

        table.WithColumn( nameof(DeviceRecord.HwInfo) )
             .AsString( BaseRecord.MAX_STRING_SIZE )
             .Nullable();
    }
}
