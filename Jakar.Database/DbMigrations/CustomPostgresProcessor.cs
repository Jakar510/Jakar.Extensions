// Jakar.Extensions :: Jakar.Database
// 10/08/2023  11:43 PM

using FluentMigrator.Expressions;
using FluentMigrator.Runner.Helpers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.Postgres;



namespace Jakar.Database.DbMigrations;


public class CustomPostgresProcessor( IOptions<DbOptions>                db,
                                      PostgresDbFactory                  factory,
                                      CustomPostgresGenerator            generator,
                                      ILogger<PostgresProcessor>         logger,
                                      IOptionsSnapshot<ProcessorOptions> options,
                                      IConnectionStringAccessor          connectionStringAccessor,
                                      PostgresOptions                    pgOptions
) : GenericProcessorBase( () => factory.Factory, generator, logger, options.Value, connectionStringAccessor )
{
    private readonly CustomPostgresQuoter _quoter = new(pgOptions, db);


    public override string DatabaseType => "Postgres";

    public override IList<string> DatabaseTypeAliases { get; } = new List<string>
                                                                 {
                                                                     @"PostgreSQL"
                                                                 };

    public override void Execute( string template, params object[] args ) => Process( string.Format( template, args ) );

    public override bool SchemaExists( string schemaName ) => Exists( "select * from information_schema.schemata where schema_name = '{0}'", FormatToSafeSchemaName( schemaName ) );
    public override bool TableExists( string schemaName, string tableName ) => Exists( "select * from information_schema.tables where table_schema = '{0}' and table_name = '{1}'", FormatToSafeSchemaName( schemaName ), FormatToSafeName( tableName ) );

    public override bool ColumnExists( string schemaName, string tableName, string columnName ) => Exists( "select * from information_schema.columns where table_schema = '{0}' and table_name = '{1}' and column_name = '{2}'",
                                                                                                           FormatToSafeSchemaName( schemaName ),
                                                                                                           FormatToSafeName( tableName ),
                                                                                                           FormatToSafeName( columnName ) );

    public override bool ConstraintExists( string schemaName, string tableName, string constraintName ) =>
        Exists( "select * from information_schema.table_constraints where constraint_catalog = current_catalog and table_schema = '{0}' and table_name = '{1}' and constraint_name = '{2}'",
                FormatToSafeSchemaName( schemaName ),
                FormatToSafeName( tableName ),
                FormatToSafeName( constraintName ) );

    public override bool IndexExists( string schemaName, string tableName, string indexName ) => Exists( @"select * from pg_catalog.pg_indexes where schemaname='{0}' and tablename = '{1}' and indexname = '{2}'",
                                                                                                         FormatToSafeSchemaName( schemaName ),
                                                                                                         FormatToSafeName( tableName ),
                                                                                                         FormatToSafeName( indexName ) );

    public override bool SequenceExists( string schemaName, string sequenceName ) => Exists( "select * from information_schema.sequences where sequence_catalog = current_catalog and sequence_schema ='{0}' and sequence_name = '{1}'",
                                                                                             FormatToSafeSchemaName( schemaName ),
                                                                                             FormatToSafeName( sequenceName ) );

    public override DataSet ReadTableData( string schemaName, string tableName ) => Read( "SELECT * FROM {0}", _quoter.QuoteTableName( tableName, schemaName ) );

    public override bool DefaultValueExists( string schemaName, string tableName, string columnName, object defaultValue )
    {
        string defaultValueAsString = $"%{FormatHelper.FormatSqlEscape( defaultValue.ToString() )}%";

        return Exists( "select * from information_schema.columns where table_schema = '{0}' and table_name = '{1}' and column_name = '{2}' and column_default like '{3}'",
                       FormatToSafeSchemaName( schemaName ),
                       FormatToSafeName( tableName ),
                       FormatToSafeName( columnName ),
                       defaultValueAsString );
    }

    public override DataSet Read( string template, params object[] args )
    {
        EnsureConnectionIsOpen();

        using IDbCommand? command = CreateCommand( string.Format( template, args ) );
        using IDataReader reader  = command.ExecuteReader();
        return reader.ReadDataSet();
    }

    public override bool Exists( string template, params object[] args )
    {
        EnsureConnectionIsOpen();

        using IDbCommand? command = CreateCommand( string.Format( template, args ) );
        using IDataReader reader  = command.ExecuteReader();
        return reader.Read();
    }

    protected override void Process( string sql )
    {
        Logger.LogSql( sql );

        if ( Options.PreviewOnly || string.IsNullOrEmpty( sql ) ) { return; }

        EnsureConnectionIsOpen();

        using IDbCommand? command = CreateCommand( sql );

        try { command.ExecuteNonQuery(); }
        catch ( Exception ex ) { ReThrowWithSql( ex, sql ); }
    }

    public override void Process( PerformDBOperationExpression expression )
    {
        Logger.LogSay( "Performing DB Operation" );

        if ( Options.PreviewOnly ) { return; }

        EnsureConnectionIsOpen();

        expression.Operation?.Invoke( Connection, Transaction );
    }

    private string FormatToSafeSchemaName( string schemaName ) => FormatHelper.FormatSqlEscape( _quoter.UnQuoteSchemaName( schemaName ) );
    private string FormatToSafeName( string       sqlName )    => FormatHelper.FormatSqlEscape( _quoter.UnQuote( sqlName ) );
}
