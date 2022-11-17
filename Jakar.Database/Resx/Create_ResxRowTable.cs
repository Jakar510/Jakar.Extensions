// Jakar.Extensions :: Jakar.Database.FluentMigrations
// 10/07/2022  11:04 PM

using Jakar.Database.Migrations;



namespace Jakar.Database.Resx;


public abstract class MigrateResxRowTable : Migration<ResxRowTable>
{
    protected MigrateResxRowTable() : base() { }


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(ResxRowTable.Key) )
             .AsString( int.MaxValue )
             .NotNullable();

        table.WithColumn( nameof(ResxRowTable.Neutral) )
             .AsString( int.MaxValue )
             .NotNullable();

        table.WithColumn( nameof(ResxRowTable.English) )
             .AsString( int.MaxValue )
             .Nullable();

        table.WithColumn( nameof(ResxRowTable.Spanish) )
             .AsString( int.MaxValue )
             .Nullable();

        table.WithColumn( nameof(ResxRowTable.French) )
             .AsString( int.MaxValue )
             .Nullable();

        table.WithColumn( nameof(ResxRowTable.Swedish) )
             .AsString( int.MaxValue )
             .Nullable();

        table.WithColumn( nameof(ResxRowTable.German) )
             .AsString( int.MaxValue )
             .Nullable();

        table.WithColumn( nameof(ResxRowTable.Chinese) )
             .AsString( int.MaxValue )
             .Nullable();

        table.WithColumn( nameof(ResxRowTable.Polish) )
             .AsString( int.MaxValue )
             .Nullable();

        table.WithColumn( nameof(ResxRowTable.Thai) )
             .AsString( int.MaxValue )
             .Nullable();

        table.WithColumn( nameof(ResxRowTable.Japanese) )
             .AsString( int.MaxValue )
             .Nullable();

        table.WithColumn( nameof(ResxRowTable.Czech) )
             .AsString( int.MaxValue )
             .Nullable();

        table.WithColumn( nameof(ResxRowTable.Portuguese) )
             .AsString( int.MaxValue )
             .Nullable();

        table.WithColumn( nameof(ResxRowTable.Dutch) )
             .AsString( int.MaxValue )
             .Nullable();

        table.WithColumn( nameof(ResxRowTable.Korean) )
             .AsString( int.MaxValue )
             .Nullable();

        table.WithColumn( nameof(ResxRowTable.Arabic) )
             .AsString( int.MaxValue )
             .Nullable();

        return table;
    }


    public override void Down() => DeleteTable();
}
