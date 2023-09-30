#nullable enable
namespace Jakar.Extensions;


public static partial class TypeExtensions
{
    public static bool IsEqualType( this         Type   value, Type other )                      => value           == other;
    public static bool IsEqualType<TValue>( this TValue value, Type other ) where TValue : class => value.GetType() == other;


    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    public static bool IsInitOnly( this PropertyInfo propertyInfo ) => propertyInfo.IsInitOnly( typeof(IsExternalInit) );

    public static bool IsInitOnly( this PropertyInfo propertyInfo, Type isExternalInit )
    {
        MethodInfo? setMethod = propertyInfo.SetMethod;
        if ( setMethod is null ) { return false; }

        if ( setMethod.ReturnParameter is null ) { throw new NullReferenceException( nameof(setMethod.ReturnParameter) ); }

        return setMethod.ReturnParameter.GetRequiredCustomModifiers()
                        .Contains( isExternalInit );
    }

    public static bool IsOneOfType( this Type value, params Type[] items ) => items.Any( value.IsEqualType );


    public static bool IsOneOfType<TValue>( this TValue value, params Type[] items ) where TValue : class => items.Any( value.IsEqualType );

    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    public static object? Construct( this Type target, params Type[] args )
    {
        Type type = target.MakeGenericType( args );

        return Activator.CreateInstance( type );
    }
}
