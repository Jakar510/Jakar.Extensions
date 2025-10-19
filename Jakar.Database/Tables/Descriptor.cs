// Jakar.Extensions :: Jakar.Database
// 01/01/2025  14:01

namespace Jakar.Database;


public sealed record Descriptor( string Name, bool IsKey, string ColumnName, string VariableName, string KeyValuePair, Func<object, object> GetValue )
{
    public readonly string               Name         = Name;
    public readonly bool                 IsKey        = IsKey;
    public readonly string               ColumnName   = ColumnName;
    public readonly string               VariableName = VariableName;
    public readonly string               KeyValuePair = KeyValuePair;
    public readonly Func<object, object> GetValue     = GetValue;


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

        if ( properties.Length <= 0 ) { throw new InvalidOperationException($"Type '{typeof(TSelf)}' does not have any public instance properties that are not marked with the '{nameof(DbIgnoreAttribute)}' attribute."); }

        if ( !properties.Any(IsDbKey) ) { throw new InvalidOperationException($"Type '{typeof(TSelf)}' does not have a property with the '{nameof(KeyAttribute)}' or '{nameof(System.ComponentModel.DataAnnotations.KeyAttribute)}' attribute."); }

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



public sealed record Descriptor<TSelf>( string Name, bool IsKey, string ColumnName, string VariableName, string KeyValuePair, Func<TSelf, object?> GetValue )
    where TSelf : class, ITableRecord<TSelf>
{
    public readonly string               Name         = Name;
    public readonly bool                 IsKey        = IsKey;
    public readonly string               ColumnName   = ColumnName;
    public readonly string               VariableName = VariableName;
    public readonly string               KeyValuePair = KeyValuePair;
    public readonly Func<TSelf, object?> GetValue     = GetValue;


    private static bool IsDbKey( MemberInfo property ) => property.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() is not null;


    public static FrozenDictionary<string, Descriptor<TSelf>> CreateMapping()
    {
        const BindingFlags ATTRIBUTES = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty;

        PropertyInfo[] properties = typeof(TSelf).GetProperties(ATTRIBUTES)
                                                 .Where(static x => !x.HasAttribute<DbIgnoreAttribute>())
                                                 .ToArray();

        if ( properties.Length <= 0 ) { throw new InvalidOperationException($"Type '{typeof(TSelf)}' does not have any public instance properties that are not marked with the '{nameof(DbIgnoreAttribute)}' attribute."); }

        if ( !properties.Any(IsDbKey) ) { throw new InvalidOperationException($"Type '{typeof(TSelf)}' does not have a property with the '{nameof(System.ComponentModel.DataAnnotations.KeyAttribute)}' attribute."); }

        return properties.ToFrozenDictionary(static x => x.Name, Create);
    }
    public static Descriptor<TSelf> Create( PropertyInfo property )
    {
        string name = property.Name.ToSnakeCase();
        return new Descriptor<TSelf>(name, IsDbKey(property), $" {name} ", $" @{name} ", $" {name} = @{name} ", GetTablePropertyValue(property));
    }
    private static Func<TSelf, object?> GetTablePropertyValue( PropertyInfo property )
    {
        // Validate getter and declaring type once
        MethodInfo? getter = property.GetMethod;
        if ( getter is null ) { throw new InvalidOperationException($"Property '{property.Name}' does not have a getter."); }

        Type? declaringType = getter.DeclaringType;
        if ( declaringType is null ) { throw new InvalidOperationException($"Getter for property '{property.Name}' has no declaring type."); }

        // Clear, per-property name helps when inspecting emitted methods
        string methodName = string.Concat(declaringType.FullName, ".__get_", property.Name);

        // Create Emit for the delegate type we want: Func<TSelf, object?>
        Emit<Func<TSelf, object?>> emit = Emit<Func<TSelf, object?>>.NewDynamicMethod(declaringType, methodName);

        // Load the instance argument
        emit.LoadArgument(0);

        // Only cast when the getter's declaring type differs from TSelf
        if ( declaringType != typeof(TSelf) ) { emit.CastClass(declaringType); }

        // Call the getter
        emit.Call(getter);

        // Box value types (reference types require no boxing)
        if ( property.PropertyType.IsValueType ) { emit.Box(property.PropertyType); }

        // Return and create delegate
        emit.Return();
        return emit.CreateDelegate();
    }


    public static string GetColumnName( Descriptor<TSelf>   x ) => x.ColumnName;
    public static string GetVariableName( Descriptor<TSelf> x ) => x.VariableName;
    public static string GetKeyValuePair( Descriptor<TSelf> x ) => x.KeyValuePair;
}
