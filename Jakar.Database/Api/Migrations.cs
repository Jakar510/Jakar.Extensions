namespace Jakar.Database.DbMigrations;


public static class Migrations
{
    internal static readonly ConcurrentDictionary<ulong, MigrationRecord> migrations = new();
    public static void AddMigrations<TSelf>()
        where TSelf : ITableRecord<TSelf>
    {
        foreach ( MigrationRecord record in MigrationRecord.Migrations )
        {
            if ( migrations.TryGetValue(record.MigrationID, out _) ) { throw new InvalidOperationException($"Duplicate migration version {record.MigrationID} for {typeof(TSelf).Name}"); }

            migrations[record.MigrationID] = record;
        }
    }
    public static void UseMigrationsEndPoint( this WebApplication app )
    {
        // if ( app.Environment.IsDevelopment() ) { app.UseMigrationsEndPoint(); }

    }
    public static async ValueTask ApplyMigrations( this WebApplication app, CancellationToken token = default )
    {
        await using AsyncServiceScope scope   = app.Services.CreateAsyncScope();
        Database                      db      = scope.ServiceProvider.GetRequiredService<Database>();
        MigrationRecord[]             applied = await MigrationRecord.All(db, token);


        await using NpgsqlConnection  connection  = await db.ConnectAsync(token);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(token);
        HashSet<MigrationRecord>      pending     = [..MigrationRecord.Migrations];
        pending.ExceptWith(applied);


        try
        {
            foreach ( MigrationRecord record in pending ) { await record.Apply(db, token); }

            await transaction.CommitAsync(token);
        }
        catch ( Exception e )
        {
            await transaction.RollbackAsync(token);
            throw new SqlException("", e);
        }
    }


    public static DbPropType ToDbPropertyType( this DbType type ) => type switch
                                                                     {
                                                                         DbType.AnsiString            => DbPropType.String,
                                                                         DbType.Binary                => DbPropType.Binary,
                                                                         DbType.Byte                  => DbPropType.Byte,
                                                                         DbType.Boolean               => DbPropType.Boolean,
                                                                         DbType.Currency              => DbPropType.Decimal,
                                                                         DbType.Date                  => DbPropType.Date,
                                                                         DbType.Decimal               => DbPropType.Decimal,
                                                                         DbType.Double                => DbPropType.Double,
                                                                         DbType.Guid                  => DbPropType.Guid,
                                                                         DbType.Int16                 => DbPropType.Int16,
                                                                         DbType.Int32                 => DbPropType.Int32,
                                                                         DbType.Int64                 => DbPropType.Int64,
                                                                         DbType.SByte                 => DbPropType.SByte,
                                                                         DbType.Single                => DbPropType.Double,
                                                                         DbType.String                => DbPropType.String,
                                                                         DbType.StringFixedLength     => DbPropType.String,
                                                                         DbType.Time                  => DbPropType.Time,
                                                                         DbType.UInt16                => DbPropType.UInt16,
                                                                         DbType.UInt32                => DbPropType.UInt32,
                                                                         DbType.UInt64                => DbPropType.UInt64,
                                                                         DbType.VarNumeric            => DbPropType.Decimal,
                                                                         DbType.Xml                   => DbPropType.Xml,
                                                                         DbType.AnsiStringFixedLength => DbPropType.String,
                                                                         DbType.DateTime              => DbPropType.DateTime,
                                                                         DbType.DateTime2             => DbPropType.DateTime,
                                                                         DbType.DateTimeOffset        => DbPropType.DateTimeOffset,
                                                                         _                            => throw new OutOfRangeException(type)
                                                                     };
    public static DbPropType? ToDbPropertyType( this DbType? type ) => type?.ToDbPropertyType();
}
