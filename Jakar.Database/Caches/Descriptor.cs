// Jakar.Extensions :: Jakar.Database
// 08/17/2022  8:48 PM

namespace Jakar.Database.Caches;


public sealed record Descriptor
{
    public bool                 IsKey        { get;  }
    public Func<object, object> GetValue     { get;  }
    public string               ColumnName   { get;  }
    public string               KeyValuePair { get;  }
    public string               Name         { get;  }
    public string               VariableName { get;  }


    private Descriptor( PropertyInfo property, string name, string columnName, string variableName, string keyValuePair )
    {
        ArgumentNullException.ThrowIfNull( property.GetMethod );
        ArgumentNullException.ThrowIfNull( property.DeclaringType );

        Name         = name;
        ColumnName   = columnName;
        VariableName = variableName;
        KeyValuePair = keyValuePair;
        IsKey        = IsDbKey( property );


        Emit<Func<object, object>>? emit = Emit<Func<object, object>>.NewDynamicMethod( GetType(), "GetTablePropertyValue" )
                                                                     .LoadArgument( 0 )
                                                                     .CastClass( property.DeclaringType )
                                                                     .Call( property.GetMethod );

        if ( property.PropertyType.IsValueType ) { emit = emit.Box( property.PropertyType ); }

        GetValue = emit.Return()
                       .CreateDelegate();
    }
    private static bool IsDbKey( MemberInfo property ) => property.GetCustomAttribute<KeyAttribute>() is not null || property.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() is not null;

    
    public static Descriptor Create( PropertyInfo property ) => Create( property, property.Name );
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public static Descriptor Create( PropertyInfo property, in string name ) =>
        new(property, name, $" {name} ", $" @{name} ", $" {name} = @{name} ");


    public static Descriptor MsSql( PropertyInfo property ) => MsSql( property, property.Name );
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public static Descriptor MsSql( PropertyInfo property, in string name ) =>
        new(property, name, $" {name} ", $" @{name} ", $" {name} = @{name} ");


    public static Descriptor Postgres( PropertyInfo property ) => Postgres( property, property.Name );
    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public static Descriptor Postgres( PropertyInfo property, in string name ) =>
        new(property, name, $" \"{name}\" ", $" @{name} ", $" \"{name}\" = @{name} ");
}
