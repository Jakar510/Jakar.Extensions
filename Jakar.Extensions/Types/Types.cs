namespace Jakar.Extensions;


public static partial class Types
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqualType( this         Type   value, Type other ) => value                                == other;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqualType<TValue>( this TValue value, Type other ) => (value?.GetType() ?? typeof(TValue)) == other;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsEqualType<TValue>( this Type   other ) => typeof(TValue) == other;


    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsOneOfType( this         Type   value, scoped in ReadOnlySpan<Type> types ) => types.Any( value.IsEqualType );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsOneOfType<TValue>( this TValue value, scoped in ReadOnlySpan<Type> types ) => (value?.GetType() ?? typeof(TValue)).IsOneOfType( types );


    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    [RequiresDynamicCode( "The native code for this instantiation might not be available at runtime." ), RequiresUnreferencedCode( "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met." )]
    public static object? Construct( [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicParameterlessConstructor )] this Type target, params Type[] args )
    {
        Type type = target.MakeGenericType( args );

        return Activator.CreateInstance( type );
    }
}
