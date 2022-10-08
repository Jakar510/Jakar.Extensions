namespace Jakar.Database.Migrations;


public sealed class Migrator
{
    private readonly Type[] _tables;
    public Migrator( params Assembly[] assemblies ) : this(assemblies.SelectMany(a => a.GetTypes()
                                                                                       .Where(x => x.GetCustomAttribute<DbMigrationAttribute>() is not null))
                                                                     .ToArray()) { }
    public Migrator( params Type[] tables ) => _tables = tables;
    public static async Task Migrate<TDatabase>( CancellationToken token = default )
    {
        var migrator = new Migrator(typeof(TDatabase).Assembly, Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly());
        await migrator.Start(token);
    }
    public async Task Start( CancellationToken token )
    {
        await ValueTask.CompletedTask;
        await ValueTask.CompletedTask;
    }
}
