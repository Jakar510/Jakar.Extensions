using ZLinq;



namespace Jakar.Extensions;


public static partial class Types
{
    extension( PropertyInfo propertyInfo )
    {
        public bool IsInitOnly() => propertyInfo.IsInitOnly(typeof(IsExternalInit));
        public bool IsInitOnly( Type isExternalInit )
        {
            MethodInfo? setMethod = propertyInfo.SetMethod;
            if ( setMethod is null ) { return false; }

            if ( setMethod.ReturnParameter is null ) { throw new NullReferenceException(nameof(setMethod.ReturnParameter)); }

            return setMethod.ReturnParameter.GetRequiredCustomModifiers()
                            .Contains(isExternalInit);
        }
    }



    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsEqualType( this         Type   value, Type other ) => value                                  == other;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsEqualType<TValue>( this TValue value, Type other ) => ( value?.GetType() ?? typeof(TValue) ) == other;
    extension( Type        other )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public        bool IsEqualType<TValue>() => typeof(TValue) == other;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsOneOfType( params ReadOnlySpan<Type> types ) => types.AsValueEnumerable()
                                                                                                                              .Any(other.IsEqualType);
    }


    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsOneOfType<TValue>( this TValue value, params ReadOnlySpan<Type> types ) => ( value?.GetType() ?? typeof(TValue) ).IsOneOfType(types);


    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")] [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public static object? Construct( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] this Type target, params Type[] args )
    {
        Type type = target.MakeGenericType(args);

        return Activator.CreateInstance(type);
    }
}
