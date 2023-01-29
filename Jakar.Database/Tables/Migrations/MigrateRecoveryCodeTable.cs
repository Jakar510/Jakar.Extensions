﻿// Jakar.Extensions :: Jakar.Database
// 01/29/2023  1:49 PM

namespace Jakar.Database.Migrations;


public abstract class MigrateRecoveryCodeTable : Migration<RecoveryCodeRecord>
{
    protected MigrateRecoveryCodeTable() : base() { }


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(RecoveryCodeRecord.Value) )
             .AsString( 10240 )
             .NotNullable();

        return table;
    }
}
