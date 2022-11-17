// Jakar.Extensions :: Jakar.Database
// 08/17/2022  8:48 PM

namespace Jakar.Database.Caches;


public abstract record Descriptor
{
    public bool                 IsKey        { get; init; }
    public Func<object, object> GetValue     { get; init; }
    public string               ColumnName   { get; init; }
    public string               KeyValuePair { get; init; }
    public string               Name         { get; init; }
    public string               VariableName { get; init; }


    protected Descriptor( PropertyInfo property, string name, string columnName, string variableName, string keyValuePair )
    {
        ArgumentNullException.ThrowIfNull( property.GetMethod );
        ArgumentNullException.ThrowIfNull( property.DeclaringType );

        Name         = name;
        ColumnName   = columnName;
        VariableName = variableName;
        KeyValuePair = keyValuePair;
        IsKey        = IsDbKey( property );


        Emit<Func<object, object>>? emit = Emit<Func<object, object>>.NewDynamicMethod( GetType() )
                                                                     .LoadArgument( 0 )
                                                                     .CastClass( property.DeclaringType )
                                                                     .Call( property.GetMethod );

        if ( property.PropertyType.IsValueType ) { emit = emit.Box( property.PropertyType ); }

        GetValue = emit.Return()
                       .CreateDelegate();
    }
    protected static bool IsDbKey( MemberInfo property ) => property.GetCustomAttribute<KeyAttribute>() is not null || property.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() is not null;
}



public sealed record MsSqlDescriptor : Descriptor
{
    public MsSqlDescriptor( PropertyInfo property ) : this( property, property.Name ) { }
    public MsSqlDescriptor( PropertyInfo property, string name ) : base( property, name, $" {name} ", $" @{name} ", $" {name} = @{name} " ) { }
}



public sealed record PostgresDescriptor : Descriptor
{
    public PostgresDescriptor( PropertyInfo property ) : this( property, property.Name ) { }
    public PostgresDescriptor( PropertyInfo property, string name ) : base( property, name, $" \"{name}\" ", $" @{name} ", $" \"{name}\" = @{name} " ) { }
}
