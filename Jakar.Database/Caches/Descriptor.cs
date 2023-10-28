// Jakar.Extensions :: Jakar.Database
// 08/17/2022  8:48 PM

namespace Jakar.Database.Caches;


public sealed record Descriptor( string Name, bool IsKey, string ColumnName, string VariableName, string KeyValuePair, Func<object, object> GetValue )
{
    public static Func<object, object> GetTablePropertyValue( PropertyInfo property )
    {
        ArgumentNullException.ThrowIfNull( property.GetMethod );
        ArgumentNullException.ThrowIfNull( property.DeclaringType );

        Emit<Func<object, object>>? emit = Emit<Func<object, object>>.NewDynamicMethod( property.DeclaringType, nameof(GetTablePropertyValue) ).LoadArgument( 0 ).CastClass( property.DeclaringType ).Call( property.GetMethod );

        if ( property.PropertyType.IsValueType ) { emit = emit.Box( property.PropertyType ); }

        return emit.Return().CreateDelegate();
    }
    public static bool IsDbKey( MemberInfo property ) => property.GetCustomAttribute<KeyAttribute>() is not null || property.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() is not null;


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ] public static Descriptor MsSql( PropertyInfo property ) => MsSql( property, property.Name );


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public static Descriptor MsSql( PropertyInfo property, in string name ) =>
        new(name, IsDbKey( property ), $" {name} ", $" @{name} ", $" {name} = @{name} ", GetTablePropertyValue( property ));


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ] public static Descriptor Postgres( PropertyInfo property ) => Postgres( property, property.Name );


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public static Descriptor Postgres( PropertyInfo property, in string name ) =>
        new(name, IsDbKey( property ), $" \"{name}\" ", $" @{name} ", $" \"{name}\" = @{name} ", GetTablePropertyValue( property ));
}
