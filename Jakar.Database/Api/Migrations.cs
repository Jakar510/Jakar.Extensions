using ZXing;



namespace Jakar.Database.DbMigrations;


public static class Migrations
{
    private const            string                                       MIGRATIONS = "/_migrations";
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
            foreach ( MigrationRecord record in pending.OrderBy(static x => x.MigrationID) ) { await record.Apply(db, token); }

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


    public static void TryUseMigrationsEndPoint( this WebApplication app, string endpoint = MIGRATIONS )
    {
        if ( app.Environment.IsDevelopment() ) { app.UseMigrationsEndPoint(endpoint); }
    }
    public static void UseMigrationsEndPoint( this WebApplication app, string endpoint = MIGRATIONS ) => app.MapGet(endpoint, GetMigrationsAndRenderHtml);
    private static async Task<ContentHttpResult> GetMigrationsAndRenderHtml( [FromServices] Database db, CancellationToken token )
    {
        ReadOnlySpan<MigrationRecord> records = await MigrationRecord.All(db, token);
        string                        html    = records.CreateHtml();
        return TypedResults.Content(html, "text/html", Encoding.UTF8);
    }


    public static string CreateHtml( this ReadOnlySpan<MigrationRecord> records )
    {
        StringWriter writer = new();
        writer.CreateHtml(records);
        return writer.ToString();
    }
    public static void CreateHtml( this TextWriter writer, ReadOnlySpan<MigrationRecord> records )
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
}
