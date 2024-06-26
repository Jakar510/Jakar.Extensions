﻿// Jakar.AppLogger :: Jakar.Database
// 10/26/2022  10:54 AM

namespace Jakar.Database.DbMigrations;


public abstract class Migrate_UserGroups : Migration<UserGroupRecord>
{
    protected Migrate_UserGroups() : base() { }
    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(UserGroupRecord.KeyID) ).AsGuid().NotNullable();

        table.WithColumn( nameof(UserGroupRecord.ValueID) ).AsGuid().NotNullable();

        return table;
    }
}
