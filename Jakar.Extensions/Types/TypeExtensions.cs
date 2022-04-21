namespace Jakar.Extensions.Types;


public static partial class TypeExtensions
{
    public static bool IsEqualType( this Type value, Type other )
    {
        if ( value is null ) { throw new NullReferenceException(nameof(value)); }

        if ( other is null ) { throw new NullReferenceException(nameof(other)); }

        return value == other;
    }

    public static bool IsOneOfType( this Type value, params Type[] items ) => items.Any(value.IsEqualType);


    public static bool IsOneOfType<TValue>( this TValue value, params Type[] items ) where TValue : class => items.Any(value.IsEqualType);
    public static bool IsEqualType<TValue>( this TValue value, Type          other ) where TValue : class => value.GetType() == other;


    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    public static object Construct( this Type target, params Type[] args )
    {
        Type type = target.MakeGenericType(args);

        return Activator.CreateInstance(type);
    }


    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    public static bool IsInitOnly( this PropertyInfo propertyInfo ) => propertyInfo.IsInitOnly<IsExternalInit>();

    public static bool IsInitOnly<TIsExternalInit>( this PropertyInfo propertyInfo )
    {
        MethodInfo setMethod = propertyInfo.SetMethod;
        if ( setMethod is null ) { return false; }

        if ( setMethod.ReturnParameter is null ) { throw new NullReferenceException(nameof(setMethod.ReturnParameter)); }

        return setMethod.ReturnParameter.GetRequiredCustomModifiers().Contains(typeof(TIsExternalInit));
    }
}
