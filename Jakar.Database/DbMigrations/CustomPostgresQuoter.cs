using FluentMigrator.Builder.Create.Index;
using FluentMigrator.Model;
using FluentMigrator.Postgres;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Generic;
using FluentMigrator.Runner.Generators.Postgres;
using FluentMigrator.Runner.Processors.Postgres;



namespace Jakar.Database.DbMigrations;


public class CustomPostgresQuoter( PostgresOptions options, IOptions<DbOptions> db ) : PostgresQuoter( options )
{
    private readonly   DbOptions _options = db.Value;
    public override    string    QuoteSchemaName( string   schemaName )                      => _options.CurrentSchema;
    public override    string    Quote( string             name )                            => name;
    public override    string    QuoteSequenceName( string sequenceName, string schemaName ) => base.QuoteSequenceName( sequenceName, _options.CurrentSchema );
    public new         string    UnQuoteSchemaName( string quoted ) => _options.CurrentSchema;
    protected override bool      ShouldQuote( string       name )   => false;
}



public class CustomPostgresGenerator : PostgresGenerator
{
    public override string AddColumn    => "ALTER TABLE {0} ADD {1};";
    public override string AlterColumn  => "ALTER TABLE {0} {1};";
    public override string CreateTable  => "CREATE TABLE {0} ({1})";
    public override string DropColumn   => "ALTER TABLE {0} DROP COLUMN {1};";
    public override string DropTable    => "DROP TABLE {0};";
    public override string RenameColumn => "ALTER TABLE {0} RENAME COLUMN {1} TO {2};";


    public CustomPostgresGenerator( CustomPostgresQuoter    quoter ) : this( quoter, new OptionsWrapper<GeneratorOptions>( new GeneratorOptions() ) ) { }
    public CustomPostgresGenerator( CustomPostgresQuoter    quoter, IOptions<GeneratorOptions> generatorOptions ) : base( quoter, generatorOptions ) { }
    protected CustomPostgresGenerator( CustomPostgresQuoter quoter, IOptions<GeneratorOptions> generatorOptions, ITypeMap                   typeMap ) : base( quoter, generatorOptions, typeMap ) { }
    protected CustomPostgresGenerator( IColumn              column, CustomPostgresQuoter       quoter,           IOptions<GeneratorOptions> generatorOptions ) : base( column, quoter, generatorOptions ) { }
}
