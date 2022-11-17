// Jakar.Extensions :: Jakar.Database
// 08/17/2022  8:48 PM

namespace Jakar.Database.Caches;


[SuppressMessage( "ReSharper", "ReturnTypeCanBeEnumerable.Global" )]
public sealed class TypePropertiesCache : ConcurrentDictionary<Type, ConcurrentDictionary<DbInstance, IReadOnlyList<Descriptor>>>
{
    public static TypePropertiesCache Current { get; } = new();


    static TypePropertiesCache() => Current.Register( Assembly.GetCallingAssembly() );
    internal TypePropertiesCache() { }
    [Pure]
    public Descriptor Get( Type type, in DbInstance instance, string columnName ) => Get( type, instance )
       .First( x => x.ColumnName == columnName );

    [Pure]
    public IReadOnlyList<Descriptor> Get( Type type, in DbInstance instance )
    {
        if ( !ContainsKey( type ) ) { Register( type ); }

        return this[type][instance];
    }


    public void Register( Assembly assembly ) => Register( assembly.DefinedTypes.Where( x => x.GetCustomAttribute<SerializableAttribute>() is not null ) );
    public void Register( IEnumerable<Type> types )
    {
        foreach ( Type type in types ) { Register( type ); }
    }
    public void Register( params Type[] types )
    {
        foreach ( Type type in types ) { Register( type ); }
    }
    public void Register( Type type )
    {
        if ( ContainsKey( type ) ) { return; }


        PropertyInfo[] properties = type.GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetField )
                                        .ToArray();

        this[type] = new ConcurrentDictionary<DbInstance, IReadOnlyList<Descriptor>>
                     {
                         [DbInstance.Postgres] = properties.Select( property => new PostgresDescriptor( property ) )
                                                           .ToArray(),
                         [DbInstance.MsSql] = properties.Select( property => new MsSqlDescriptor( property ) )
                                                        .ToArray(),
                     };
    }
}
