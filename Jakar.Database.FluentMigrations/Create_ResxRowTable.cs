// // Jakar.Extensions :: Jakar.Database.FluentMigrations
// // 10/07/2022  11:04 PM
//
// namespace Jakar.Database.FluentMigrations;
//
//
// [Migration( 1 )]
// public class Create_ResxRowTable : Migration<ResxRowTable>
// {
//     public Create_ResxRowTable() : this( "dbo" ) { }
//     public Create_ResxRowTable( string currentScheme ) : base( currentScheme ) { }
//
//
//     public override void Down() => DeleteTable();
//     public override void Up()
//     {
//         CheckSchema();
//         ICreateTableWithColumnSyntax table = CreateTable();
//
//
//         table.WithColumn( nameof(ResxRowTable.ID) )
//              .AsInt64()
//              .NotNullable()
//              .PrimaryKey()
//              .Identity();
//     }
// }
