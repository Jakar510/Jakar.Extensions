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



    extension( Type self )
    {
        public bool IsBuiltInNullableType()
        {
            if ( self.TryGetUnderlyingType(out Type? type) && type.IsBuiltInType() ) { return true; }

            return self.IsOneOf([
                                    typeof(bool?),
                                    typeof(byte?),
                                    typeof(sbyte?),
                                    typeof(Guid?),
                                    typeof(char?),
                                    typeof(DateOnly?),
                                    typeof(DateTime?),
                                    typeof(DateTimeOffset?),
                                    typeof(int?),
                                    typeof(uint?),
                                    typeof(short?),
                                    typeof(ushort?),
                                    typeof(long?),
                                    typeof(ulong?),
                                    typeof(float?),
                                    typeof(double?),
                                    typeof(decimal?),
                                    typeof(Index?),
                                    typeof(Range?),
                                    typeof(IntPtr?),
                                    typeof(UIntPtr?),
                                    typeof(TimeSpan?),
                                    typeof(TimeOnly?)
                                ]);
        }


        public bool IsBuiltInType()
        {
            if ( self is { IsGenericType: true, IsEnum: true } ) { return true; }

            return self.IsOneOf([
                                    typeof(bool),
                                    typeof(byte),
                                    typeof(sbyte),
                                    typeof(Guid),
                                    typeof(char),
                                    typeof(string),
                                    typeof(DateOnly),
                                    typeof(DateTime),
                                    typeof(DateTimeOffset),
                                    typeof(int),
                                    typeof(uint),
                                    typeof(short),
                                    typeof(ushort),
                                    typeof(long),
                                    typeof(ulong),
                                    typeof(float),
                                    typeof(double),
                                    typeof(decimal),
                                    typeof(Index),
                                    typeof(Range),
                                    typeof(IntPtr),
                                    typeof(UIntPtr),
                                    typeof(TimeSpan),
                                    typeof(TimeOnly)
                                ]);
        }


        public bool IsEqualType( Type           other )       => self           == other;
        public bool IsEqualType<TValue>( TValue _ )           => typeof(TValue) == self;
        public bool IsEqualType<TValue>()                     => typeof(TValue) == self;
        public bool IsOneOf( ReadOnlySpan<Type>       types ) => types.Any(self.IsEqualType);
        public bool IsGenericType( ReadOnlySpan<Type> types ) => self.IsAnyBuiltInType() || self.IsOneOf(types) || self.IsJToken();
        public bool IsNullableType()                          => self.IsGenericType && self.GetGenericTypeDefinition() == typeof(Nullable<>);
        public bool IsAnyBuiltInType()                        => self.IsGenericType && ( self.IsBuiltInType() || self.IsBuiltInNullableType() );
        public bool IsJToken()                                => self.IsAssignableTo(typeof(JToken));
    }



    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")] [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public static object? Construct( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] this Type target, params Type[] args )
    {
        Type type = target.MakeGenericType(args);
        return Activator.CreateInstance(type);
    }
}
