// Jakar.Extensions :: Jakar.Database.FluentMigrations
// 10/07/2022  11:04 PM

namespace Jakar.Database.Resx;


public abstract class MigrateResxRowTable : Migration<ResxRowRecord>
{
    protected MigrateResxRowTable() : base() { }


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn(nameof(ResxRowRecord.Key)).AsString(int.MaxValue).NotNullable();

        table.WithColumn(nameof(ResxRowRecord.Neutral)).AsString(int.MaxValue).NotNullable();

        table.WithColumn(nameof(ResxRowRecord.English)).AsString(int.MaxValue).Nullable();

        table.WithColumn(nameof(ResxRowRecord.Spanish)).AsString(int.MaxValue).Nullable();

        table.WithColumn(nameof(ResxRowRecord.French)).AsString(int.MaxValue).Nullable();

        table.WithColumn(nameof(ResxRowRecord.Swedish)).AsString(int.MaxValue).Nullable();

        table.WithColumn(nameof(ResxRowRecord.German)).AsString(int.MaxValue).Nullable();

        table.WithColumn(nameof(ResxRowRecord.Chinese)).AsString(int.MaxValue).Nullable();

        table.WithColumn(nameof(ResxRowRecord.Polish)).AsString(int.MaxValue).Nullable();

        table.WithColumn(nameof(ResxRowRecord.Thai)).AsString(int.MaxValue).Nullable();

        table.WithColumn(nameof(ResxRowRecord.Japanese)).AsString(int.MaxValue).Nullable();

        table.WithColumn(nameof(ResxRowRecord.Czech)).AsString(int.MaxValue).Nullable();

        table.WithColumn(nameof(ResxRowRecord.Portuguese)).AsString(int.MaxValue).Nullable();

        table.WithColumn(nameof(ResxRowRecord.Dutch)).AsString(int.MaxValue).Nullable();

        table.WithColumn(nameof(ResxRowRecord.Korean)).AsString(int.MaxValue).Nullable();

        table.WithColumn(nameof(ResxRowRecord.Arabic)).AsString(int.MaxValue).Nullable();

        return table;
    }


    public override void Down() => DeleteTable();
}
