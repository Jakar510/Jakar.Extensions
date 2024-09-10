namespace Jakar.Extensions;


public static partial class TypeExtensions
{
    public static bool IsInitOnly( this PropertyInfo propertyInfo ) => propertyInfo.IsInitOnly( typeof(IsExternalInit) );

    public static bool IsInitOnly( this PropertyInfo propertyInfo, Type isExternalInit )
    {
        MethodInfo? setMethod = propertyInfo.SetMethod;
        if ( setMethod is null ) { return false; }

        if ( setMethod.ReturnParameter is null ) { throw new NullReferenceException( nameof(setMethod.ReturnParameter) ); }

        return setMethod.ReturnParameter.GetRequiredCustomModifiers().Contains( isExternalInit );
    }

    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqualType( this    Type value, Type other ) => value                           == other;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqualType<T>( this T    value, Type other ) => (value?.GetType() ?? typeof(T)) == other;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqualType<T>( this Type other ) => typeof(T) == other;


    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsOneOfType( this    Type value, scoped in ReadOnlySpan<Type> types ) => types.Any( value.IsEqualType );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsOneOfType<T>( this T    value, scoped in ReadOnlySpan<Type> types ) => (value?.GetType() ?? typeof(T)).IsOneOfType( types );


    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    [RequiresDynamicCode( nameof(Construct) )]
    public static object? Construct( this Type target, params Type[] args )
    {
        Type type = target.MakeGenericType( args );

        return Activator.CreateInstance( type );
    }
}
