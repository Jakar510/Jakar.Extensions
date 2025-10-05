// Jakar.Extensions :: Jakar.Database
// 01/01/2025  14:01

namespace Jakar.Database;


public sealed record Descriptor( string Name, bool IsKey, string ColumnName, string VariableName, string KeyValuePair, Func<object, object> GetValue )
{
    private static Func<object, object> GetTablePropertyValue( PropertyInfo property )
    {
        ArgumentNullException.ThrowIfNull(property.GetMethod);
        ArgumentNullException.ThrowIfNull(property.DeclaringType);

        Emit<Func<object, object>>? emit = Emit<Func<object, object>>.NewDynamicMethod(property.DeclaringType, nameof(GetTablePropertyValue))
                                                                     .LoadArgument(0)
                                                                     .CastClass(property.DeclaringType)
                                                                     .Call(property.GetMethod);

        if ( property.PropertyType.IsValueType ) { emit = emit.Box(property.PropertyType); }

        return emit.Return()
                   .CreateDelegate();
    }
    private static bool IsDbKey( MemberInfo property ) => property.GetCustomAttribute<KeyAttribute>() is not null || property.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() is not null;


    public static FrozenDictionary<string, Descriptor> CreateMapping( Type type )
    {
        const BindingFlags ATTRIBUTES = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty;

        PropertyInfo[] properties = type.GetProperties(ATTRIBUTES)
                                        .Where(static x => !x.HasAttribute<DbIgnoreAttribute>())
                                        .ToArray();

        Debug.Assert(properties.Length > 0);
        Debug.Assert(properties.Any(IsDbKey));

        return properties.ToFrozenDictionary(static x => x.Name, Create)
                         .ToFrozenDictionary(StringComparer.Ordinal);
    }
    public static FrozenDictionary<string, Descriptor> CreateMapping<TSelf>()
        where TSelf : class, ITableRecord<TSelf> => CreateMapping(typeof(TSelf));
    public static Descriptor Create( PropertyInfo property ) => Create(property, property.Name.ToSnakeCase());
    public static Descriptor Create( PropertyInfo property, in string name )
    {
        if ( !string.Equals(name.ToSnakeCase(), name) ) { throw new ArgumentException($"The name '{name}' is not in snake case."); }

        return new Descriptor(name, IsDbKey(property), $" {name} ", $" @{name} ", $" {name} = @{name} ", GetTablePropertyValue(property));
    }


    public static string GetColumnName( Descriptor   x ) => x.ColumnName;
    public static string GetVariableName( Descriptor x ) => x.VariableName;
    public static string GetKeyValuePair( Descriptor x ) => x.KeyValuePair;
}
