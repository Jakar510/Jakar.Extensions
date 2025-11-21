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



    // ReSharper disable once MoveToExtensionBlock
    // ReSharper disable once ConvertToExtensionBlock
    public static bool IsOneOf( this Type self, params ReadOnlySpan<Type> types ) => types.Any(self.IsEqualType);


    // ReSharper disable once MoveToExtensionBlock
    // ReSharper disable once ConvertToExtensionBlock
    public static bool IsGenericType( this Type self, params ReadOnlySpan<Type> types ) => self.IsAnyBuiltInType() || self.IsAssignableTo(typeof(JToken)) || self.IsOneOf(types);



    extension( Type self )
    {
        public bool IsBuiltInNullableType() => self.IsOneOf(typeof(bool?),
                                                            typeof(byte?),
                                                            typeof(sbyte?),
                                                            typeof(Guid?),
                                                            typeof(char?),
                                                            typeof(DateTime?),
                                                            typeof(int?),
                                                            typeof(uint?),
                                                            typeof(short?),
                                                            typeof(ushort?),
                                                            typeof(long?),
                                                            typeof(ulong?),
                                                            typeof(float?),
                                                            typeof(double?),
                                                            typeof(decimal?),
                                                            typeof(TimeSpan?));


        public bool IsBuiltInType() => self.IsOneOf(typeof(bool),
                                                    typeof(byte),
                                                    typeof(sbyte),
                                                    typeof(Guid),
                                                    typeof(char),
                                                    typeof(string),
                                                    typeof(DateTime),
                                                    typeof(int),
                                                    typeof(uint),
                                                    typeof(short),
                                                    typeof(ushort),
                                                    typeof(long),
                                                    typeof(ulong),
                                                    typeof(float),
                                                    typeof(double),
                                                    typeof(decimal),
                                                    typeof(TimeSpan));


        public bool IsEqualType( Type           other ) => self           == other;
        public bool IsEqualType<TValue>( TValue _ )     => typeof(TValue) == self;
        public bool IsEqualType<TValue>()               => typeof(TValue) == self;
        public bool IsNullableType()                    => self.IsGenericType && self.GetGenericTypeDefinition() == typeof(Nullable<>);
        public bool IsAnyBuiltInType()                  => self.IsGenericType && ( self.IsBuiltInType() || self.IsBuiltInNullableType() );
    }



    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")] [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public static object? Construct( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] this Type target, params Type[] args )
    {
        Type type = target.MakeGenericType(args);
        return Activator.CreateInstance(type);
    }
}
