using System.Buffers;
using System.Collections.Concurrent;
using System.Reflection.Emit;
using Jakar.Extensions.Collections;
using Jakar.Extensions.Strings;
using Microsoft.Toolkit.HighPerformance.Enumerables;
using Sigil;


#if NET6_0


// Jakar.Extensions :: Jakar.Extensions
// 06/09/2022  2:53 PM

#nullable enable
namespace Jakar.Mapper;


// A Pair holds a key and a value from a dictionary. It is used by the IEnumerable<T> implementation for both IDictionary<TKey, TValue> and IReadOnlyDictionary<TKey, TValue>.



public static class MContext
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, Func<object, object?>>> _contextMap = new();


    static MContext() => Register(Assembly.GetExecutingAssembly(), Assembly.GetCallingAssembly());
    public static void Register<T>() where T : class => Register(typeof(T));
    public static void Register( params Assembly[] assemblies ) => assemblies.ForEach(Register);
    public static void Register( Assembly          assembly ) => Register(assembly.GetTypes().Where(x => x.GetCustomAttribute<SerializableAttribute>() is not null));
    public static void Register( IEnumerable<Type> types ) => types.ForEach(Register);
    public static void Register( params Type[]     types ) => types.ForEach(Register);
    public static void Register( Type              type ) => EnsureRegistered(type);
    private static IReadOnlyDictionary<string, Func<object, object?>> EnsureRegistered( Type type )
    {
        if ( _contextMap.ContainsKey(type) ) { return _contextMap[type]; }

        return _contextMap[type] = Create(type);
    }
    private static IReadOnlyDictionary<string, Func<object, object?>> Create( Type type ) => Create(type.GetProperties(BindingFlags.Instance | BindingFlags.Public));
    private static IReadOnlyDictionary<string, Func<object, object?>> Create( params PropertyInfo[] properties )
    {
        var mapping = new Dictionary<string, Func<object, object?>>(properties.Length);

        foreach ( PropertyInfo property in properties )
        {
            if ( property.GetMethod is null ) { continue; }

            mapping[property.Name] = Create_GetMethod(property);
        }

        return mapping;
    }
    private static Func<object, object?> Create_GetMethod( PropertyInfo property )
    {
        Func<object, object?> method = Emit<Func<object, object?>>.NewDynamicMethod(property.Name).LoadArgument(0).CastClass(property.DeclaringType).Call(property.GetMethod).Return().CreateDelegate();

        return method;
    }


    public static MContext<T> GetContext<T>( T context ) where T : notnull => new(EnsureRegistered(typeof(T)), context);


    // public long AsLong( in ReadOnlySpan<char> key ) { throw new NotImplementedException(); }
}



public readonly ref struct MContext<T> where T : notnull
{
    private readonly T                                                  _context;
    private readonly IReadOnlyDictionary<string, Func<object, object?>> _mapping;


    public MContext( IReadOnlyDictionary<string, Func<object, object?>> mapping, T context )
    {
        _context = context;
        _mapping = mapping;
    }


    public object? GetValue( in string key ) => _mapping[key](_context);
    public bool TryGetValue( in string key, out object? result )
    {
        try
        {
            result = GetValue(key);
            return true;
        }
        catch ( Exception e )
        {
            e.WriteToConsole();
            e.WriteToDebug();
            result = default;
            return false;
        }
    }


    public object? GetValue( in ReadOnlySpan<char> key )
    {
        foreach ( ( string? name, Func<object, object?> func ) in _mapping )
        {
            if ( key.SequenceEqual(name) ) { return func(_context); }
        }

        throw new KeyNotFoundException(key.ToString());
    }
    public bool TryGetValue( in ReadOnlySpan<char> key, out object? result )
    {
        try
        {
            result = GetValue(key);
            return true;
        }
        catch ( Exception e )
        {
            e.WriteToConsole();
            e.WriteToDebug();
            result = default;
            return false;
        }
    }
}



#endif
