// Jakar.Extensions :: Jakar.Database
// 08/17/2022  8:48 PM

namespace Jakar.Database.Caches;


public sealed class TypePropertiesCache : ConcurrentDictionary<Type, IReadOnlyList<Descriptor>>
{
    public static TypePropertiesCache Current { get; } = new();


    public new IReadOnlyList<Descriptor> this[Type type]
    {
        get => Register( type );
        private set => base[type] = value;
    }


    static TypePropertiesCache() => Current.Register( Assembly.GetCallingAssembly() );
    internal TypePropertiesCache() { }

    public void Register(Assembly assembly) => Register( assembly.DefinedTypes.Where( x => x.GetCustomAttribute<SerializableAttribute>() is not null ) );
    public void Register(IEnumerable<Type> types)
    {
        foreach (Type type in types) { Register( type ); }
    }
    public void Register(params Type[] types)
    {
        foreach (Type type in types) { Register( type ); }
    }
    public IReadOnlyList<Descriptor> Register(Type type)
    {
        if (ContainsKey( type )) { return base[type]; }

        var properties = type.GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetField )
                                            .ToList();

        var results = new List<Descriptor>( properties.Count );
        results.AddRange( properties.Select( property => (Descriptor)property ) );

        this[type] = results;
        return results;
    }
}
