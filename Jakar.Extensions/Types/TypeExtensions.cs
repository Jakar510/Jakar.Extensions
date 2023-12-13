using System;



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


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static bool IsEqualType( this Type value, Type other ) => value == other;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static bool IsEqualType<T>( this T value, Type other )
        where T : class => value.GetType() == other;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static bool IsEqualType<T>( this Type other )
        where T : class => typeof(T) == other;


    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static bool IsOneOfType( this Type value, params Type[]      types ) => value.IsOneOfType( types.AsSpan() );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static bool IsOneOfType( this Type value, ReadOnlySpan<Type> types ) => types.Any( value.IsEqualType );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static bool IsOneOfType<T>( this T value, params Type[] types )
        where T : class => types.Any( value.IsEqualType );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static bool IsOneOfType<T>( params Type[] types )
        where T : class => types.Any( typeof(T).IsEqualType );


    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


#if NET7_0_OR_GREATER
    [ RequiresDynamicCode( nameof(Construct) ) ]
#endif
    public static object? Construct( this Type target, params Type[] args )
    {
        Type type = target.MakeGenericType( args );

        return Activator.CreateInstance( type );
    }
}
