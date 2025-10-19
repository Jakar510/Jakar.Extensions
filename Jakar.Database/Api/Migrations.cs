namespace Jakar.Database;


public static class Migrations
{
    private const            string       MIGRATIONS = "/_migrations";
    public static readonly   IdGenerator  Ids        = new();
    internal static readonly RecordValues Records    = new();


    public static bool HasFlagValue( this ColumnOptions type, ColumnOptions flag ) => ( type & flag ) != 0;


    public static string CreateTableSql<TSelf>()
        where TSelf : class, ITableRecord<TSelf> => SqlTableBuilder<TSelf>.Default.Build();


    public static IEnumerable<MigrationRecord> BuiltIns( IdGenerator ids )
    {
        yield return MigrationRecord.CreateTable(ids.Current);

        yield return MigrationRecord.SetLastModified(ids.Current);

        yield return MigrationRecord.FromEnum<MimeType>(ids.Current);
        yield return MigrationRecord.FromEnum<SupportedLanguage>(ids.Current);
        yield return MigrationRecord.FromEnum<SubscriptionStatus>(ids.Current);
        yield return MigrationRecord.FromEnum<DeviceCategory>(ids.Current);
        yield return MigrationRecord.FromEnum<DevicePlatform>(ids.Current);
        yield return MigrationRecord.FromEnum<DeviceTypes>(ids.Current);
        yield return MigrationRecord.FromEnum<DistanceUnit>(ids.Current);
        yield return MigrationRecord.FromEnum<ProgrammingLanguage>(ids.Current);
        yield return MigrationRecord.FromEnum<Status>(ids.Current);

        yield return FileRecord.CreateTable(ids.Current);

        yield return UserRecord.CreateTable(ids.Current);

        yield return RecoveryCodeRecord.CreateTable(ids.Current);
        yield return UserRecoveryCodeRecord.CreateTable(ids.Current);

        yield return RoleRecord.CreateTable(ids.Current);
        yield return UserRoleRecord.CreateTable(ids.Current);

        yield return GroupRecord.CreateTable(ids.Current);
        yield return UserGroupRecord.CreateTable(ids.Current);

        yield return AddressRecord.CreateTable(ids.Current);
        yield return UserAddressRecord.CreateTable(ids.Current);
    }


    public static async ValueTask ApplyMigrations( this WebApplication app, CancellationToken token = default )
    {
        await using AsyncServiceScope scope       = app.Services.CreateAsyncScope();
        Database                      db          = scope.ServiceProvider.GetRequiredService<Database>();
        MigrationRecord[]             applied     = await Migrations.All(db, token);
        await using NpgsqlConnection  connection  = await db.ConnectAsync(token);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(token);
        HashSet<MigrationRecord>      pending     = [..Records.Values];
        pending.ExceptWith(applied);


        try
        {
            foreach ( MigrationRecord record in pending.OrderBy(static x => x.MigrationID) ) { await record.Apply(db, token); }

            await transaction.CommitAsync(token);
        }
        catch ( Exception e )
        {
            await transaction.RollbackAsync(token);
            throw new InvalidOperationException("Failed to apply Records", e);
        }
    }


    public static async Task RunWithMigrationsAsync( this WebApplication app, string[]? urls = null, string endpoint = MIGRATIONS, CancellationToken token = default )
    {
        app.TryUseMigrationsEndPoint(endpoint);


        try
        {
            await app.ApplyMigrations(token);
            if ( urls is not null ) { app.UseUrls(urls); }

            await app.StartAsync(token)
                     .ConfigureAwait(false);

            await app.WaitForShutdownAsync(token)
                     .ConfigureAwait(false);
        }
        finally
        {
            await app.DisposeAsync()
                     .ConfigureAwait(false);
        }
    }
    public static void TryUseMigrationsEndPoint( this WebApplication app, string endpoint = MIGRATIONS )
    {
        if ( app.Environment.IsDevelopment() ) { app.UseMigrationsEndPoint(endpoint); }
    }
    public static void UseMigrationsEndPoint( this WebApplication app, string endpoint = MIGRATIONS ) => app.MapGet(endpoint, GetMigrationsAndRenderHtml);
    private static async Task<ContentHttpResult> GetMigrationsAndRenderHtml( [FromServices] Database db, CancellationToken token )
    {
        ReadOnlySpan<MigrationRecord> records = await Migrations.All(db, token);
        string                        html    = records.CreateHtml();
        return TypedResults.Content(html, "text/html", Encoding.UTF8);
    }


    public static string CreateHtml( this ReadOnlySpan<MigrationRecord> records )
    {
        StringWriter writer = new();
        writer.CreateHtml(records);
        return writer.ToString();
    }
    public static void CreateHtml( this TextWriter writer, params ReadOnlySpan<MigrationRecord> records )
    {
        writer.WriteLine("<!DOCTYPE html>");
        writer.WriteLine("<html lang=\"en\">");
        writer.WriteLine("  <head>");
        writer.WriteLine("    <meta charset=\"UTF-8\"/>");
        writer.WriteLine("    <title>Migration Records</title>");
        writer.WriteLine("    <style>");
        writer.WriteLine("      body { font-family: system-ui, sans-serif; background: #fafafa; color: #222; margin: 2em; }");
        writer.WriteLine("      h2 { border-bottom: 2px solid #ccc; padding-bottom: 0.25em; }");
        writer.WriteLine("      table { border-collapse: collapse; width: 100%; margin-top: 1em; }");
        writer.WriteLine("      th, td { border: 1px solid #ddd; padding: 6px 10px; }");
        writer.WriteLine("      th { background-color: #f4f4f4; text-align: left; }");
        writer.WriteLine("      tr:nth-child(even) { background-color: #f9f9f9; }");
        writer.WriteLine("      tr:hover { background-color: #f1f1f1; }");
        writer.WriteLine("    </style>");
        writer.WriteLine("  </head>");
        writer.WriteLine("  <body>");
        writer.WriteLine("    <h2>Migration Records</h2>");
        writer.WriteLine("    <table>");
        writer.WriteLine("      <thead>");
        writer.WriteLine("        <tr>");
        writer.WriteLine("          <th>ID</th>");
        writer.WriteLine("          <th>Table</th>");
        writer.WriteLine("          <th>Description</th>");
        writer.WriteLine("          <th>Applied On</th>");
        writer.WriteLine("        </tr>");
        writer.WriteLine("      </thead>");
        writer.WriteLine("      <tbody>");

        foreach ( ref readonly MigrationRecord migration in records )
        {
            writer.Write("        <tr>");
            writer.Write("<td>");
            writer.Write(migration.MigrationID);
            writer.Write("</td><td>");
            writer.HtmlEncode(migration.TableID);
            writer.Write("</td><td>");
            writer.HtmlEncode(migration.Description);
            writer.Write("</td><td>");
            writer.Write(migration.AppliedOn.ToString("u"));
            writer.WriteLine("</td></tr>");
        }

        writer.WriteLine("      </tbody>");
        writer.WriteLine("    </table>");
        writer.WriteLine("  </body>");
        writer.WriteLine("</html>");
    }
    private static void HtmlEncode( this TextWriter writer, string? value )
    {
        if ( string.IsNullOrEmpty(value) ) return;

        ReadOnlySpan<char> span  = value.AsSpan();
        int                start = 0;

        for ( int i = 0; i < span.Length; i++ )
        {
            string? entity = span[i] switch
                             {
                                 '&'  => "&amp;",
                                 '<'  => "&lt;",
                                 '>'  => "&gt;",
                                 '"'  => "&quot;",
                                 '\'' => "&#39;",
                                 _    => null
                             };

            if ( entity is not null )
            {
                if ( i > start ) writer.Write(span.Slice(start, i - start));
                writer.Write(entity);
                start = i + 1;
            }
        }

        if ( start < span.Length ) writer.Write(span.Slice(start));
    }


    public static PostgresType ToDbPropertyType( this DbType type ) => type switch
                                                                       {
                                                                           DbType.AnsiString            => PostgresType.String,
                                                                           DbType.Binary                => PostgresType.Binary,
                                                                           DbType.Byte                  => PostgresType.Byte,
                                                                           DbType.Boolean               => PostgresType.Boolean,
                                                                           DbType.Currency              => PostgresType.Decimal,
                                                                           DbType.Date                  => PostgresType.Date,
                                                                           DbType.Decimal               => PostgresType.Decimal,
                                                                           DbType.Double                => PostgresType.Double,
                                                                           DbType.Guid                  => PostgresType.Guid,
                                                                           DbType.Int16                 => PostgresType.Short,
                                                                           DbType.Int32                 => PostgresType.Int,
                                                                           DbType.Int64                 => PostgresType.Long,
                                                                           DbType.SByte                 => PostgresType.SByte,
                                                                           DbType.Single                => PostgresType.Double,
                                                                           DbType.String                => PostgresType.String,
                                                                           DbType.StringFixedLength     => PostgresType.String,
                                                                           DbType.Time                  => PostgresType.Time,
                                                                           DbType.UInt16                => PostgresType.UShort,
                                                                           DbType.UInt32                => PostgresType.UInt,
                                                                           DbType.UInt64                => PostgresType.Long,
                                                                           DbType.VarNumeric            => PostgresType.Decimal,
                                                                           DbType.Xml                   => PostgresType.Xml,
                                                                           DbType.AnsiStringFixedLength => PostgresType.String,
                                                                           DbType.DateTime              => PostgresType.DateTime,
                                                                           DbType.DateTime2             => PostgresType.DateTime,
                                                                           DbType.DateTimeOffset        => PostgresType.DateTimeOffset,
                                                                           DbType.Object                => PostgresType.Json,
                                                                           _                            => throw new OutOfRangeException(type)
                                                                       };
    public static PostgresType? ToDbPropertyType( this DbType? type ) => type?.ToDbPropertyType();


    public static async ValueTask<MigrationRecord[]> All( Database db, CancellationToken token )
    {
        await using NpgsqlConnection connection = await db.ConnectAsync(token);
        CommandDefinition            command    = new(MigrationRecord.SelectSql, null, null, null, null, CommandFlags.Buffered, token);

        // await using DbDataReader     reader     = await connection.ExecuteReaderAsync(command);

        await using NpgsqlCommand cmd = new(null, connection);
        cmd.Connection  = connection;
        cmd.CommandText = command.CommandText;
        NpgsqlParameter parameter = cmd.CreateParameter();
        parameter.NpgsqlDbType = NpgsqlDbType.Text;

        await using NpgsqlDataReader reader  = await cmd.ExecuteReaderAsync(token);
        MigrationRecord[]            records = await reader.CreateAsync<MigrationRecord>(Records.Count, token);
        return records;
    }
    public static async ValueTask Apply( this MigrationRecord record, Database db, CancellationToken token )
    {
        await using NpgsqlConnection  connection  = await db.ConnectAsync(token);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(token);

        try
        {
            await record.Apply(connection, transaction, token);
            await transaction.CommitAsync(token);
        }
        catch ( Exception e )
        {
            await transaction.RollbackAsync(token);
            throw new SqlException(MigrationRecord.ApplySql, e);
        }
    }
    public static async ValueTask Apply( this MigrationRecord record, NpgsqlConnection connection, NpgsqlTransaction transaction, CancellationToken token )
    {
        PostgresParameters parameters = new();
        parameters.Add(nameof(MigrationRecord.MigrationID),    record.MigrationID);
        parameters.Add(nameof(MigrationRecord.Description),    record.Description);
        parameters.Add(nameof(MigrationRecord.TableID),        record.TableID);
        parameters.Add(nameof(MigrationRecord.AppliedOn),      record.AppliedOn);
        parameters.Add(nameof(MigrationRecord.AdditionalData), record.AdditionalData);

        CommandDefinition command = new(MigrationRecord.ApplySql, parameters, transaction, null, null, CommandFlags.Buffered, token);
        await connection.ExecuteAsync(command);
    }



    public sealed class IdGenerator
    {
        private ulong __value;
        public  ulong Current => ++__value;
        internal IdGenerator() { }
    }



    public sealed class RecordValues : IReadOnlyDictionary<ulong, MigrationRecord>
    {
        private readonly ConcurrentDictionary<ulong, MigrationRecord> __records = new();
        public           int                                          Count => __records.Count;

        public MigrationRecord this[ ulong key ] => __records[key];
        public IEnumerable<ulong>           Keys   => __records.Keys;
        public IEnumerable<MigrationRecord> Values => __records.Values;
        public RecordValues()
        {
            foreach ( MigrationRecord record in Migrations.BuiltIns(Ids) ) { Add(record); }
        }


        public void Add( MigrationRecord record )
        {
            if ( record.MigrationID == 0 ) { throw new ArgumentOutOfRangeException(nameof(record), "MigrationID cannot be 0"); }

            if ( !__records.TryAdd(record.MigrationID, record) ) { throw new InvalidOperationException($"A record with the MigrationID {record.MigrationID} already exists"); }
        }

        public IEnumerator<KeyValuePair<ulong, MigrationRecord>> GetEnumerator()                                                            => __records.GetEnumerator();
        IEnumerator IEnumerable.                                 GetEnumerator()                                                            => ( (IEnumerable)__records ).GetEnumerator();
        public bool                                              ContainsKey( ulong key )                                                   => __records.ContainsKey(key);
        public bool                                              TryGetValue( ulong key, [MaybeNullWhen(false)] out MigrationRecord value ) => __records.TryGetValue(key, out value);
    }
}
