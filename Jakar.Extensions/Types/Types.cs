using ZLinq;



namespace Jakar.Extensions;


public static partial class Types
{
    public static readonly Type NullableType = typeof(Nullable<>);


    public static bool IsNullable( this PropertyInfo  property )  => property.PropertyType.IsNullableHelper(property.DeclaringType, property.CustomAttributes);
    public static bool IsNullable( this FieldInfo     field )     => field.FieldType.IsNullableHelper(field.DeclaringType, field.CustomAttributes);
    public static bool IsNullable( this ParameterInfo parameter ) => parameter.ParameterType.IsNullableHelper(parameter.Member, parameter.CustomAttributes);



    extension( Type self )
    {
        private bool IsNullableHelper( in MemberInfo? declaringType, IEnumerable<CustomAttributeData> customAttributes )
        {
            if ( self.IsValueType ) { return Nullable.GetUnderlyingType(self) is not null; }

            CustomAttributeData? nullable = customAttributes.FirstOrDefault(static x => x.AttributeType.FullName == NULLABLE);

            if ( nullable is not null && nullable.ConstructorArguments.Count == 1 )
            {
                CustomAttributeTypedArgument attributeArgument = nullable.ConstructorArguments[0];

                if ( attributeArgument.ArgumentType == typeof(byte[]) )
                {
                    ReadOnlyCollection<CustomAttributeTypedArgument> args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value!;
                    if ( args.Count > 0 && args[0].ArgumentType == typeof(byte) ) { return (byte)args[0].Value! == 2; }
                }
                else if ( attributeArgument.ArgumentType == typeof(byte) ) { return (byte)attributeArgument.Value! == 2; }
            }

            for ( MemberInfo? type = declaringType; type != null; type = type.DeclaringType )
            {
                CustomAttributeData? context = type.CustomAttributes.FirstOrDefault(static x => x.AttributeType.FullName == NULLABLE_CONTEXT);
                if ( context is not null && context.ConstructorArguments.Count == 1 && context.ConstructorArguments[0].ArgumentType == typeof(byte) ) { return (byte)context.ConstructorArguments[0].Value! == 2; }
            }

            return false; // Couldn't find a suitable attribute
        }

        public bool TryGetUnderlyingType( [NotNullWhen(true)] out Type? result )
        {
            if ( self.IsGenericType && self.GetGenericTypeDefinition() == typeof(Nullable<>) )
            {
                foreach ( Type argument in self.GenericTypeArguments.AsSpan() )
                {
                    result = argument;
                    return true;
                }
            }

            result = null;
            return false;
        }

        public bool TryGetUnderlyingEnumType( [NotNullWhen(true)] out Type? result )
        {
            if ( self.IsEnum )
            {
                result = Enum.GetUnderlyingType(self);
                return true;
            }

            if ( self.IsGenericType && self.GetGenericTypeDefinition() == typeof(Nullable<>) )
            {
                foreach ( Type argument in self.GenericTypeArguments.AsSpan() )
                {
                    if ( argument.TryGetUnderlyingEnumType(out result) ) { return true; }
                }
            }

            result = null;
            return false;
        }
    }



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



    extension( MemberInfo self )
    {
        public NullabilityInfo GetNullabilityInfo()
        {
            NullabilityInfoContext context = new();

            return self switch
                   {
                       FieldInfo fieldInfo       => context.Create(fieldInfo),
                       PropertyInfo propertyInfo => context.Create(propertyInfo),
                       EventInfo eventInfo       => context.Create(eventInfo),
                       _                         => throw new InvalidOperationException($"MemberInfo type {self.MemberType} is not supported.")
                   };
        }

        public bool IsNullableType()
        {
            Type? type = self switch
                         {
                             FieldInfo fieldInfo       => fieldInfo.FieldType,
                             PropertyInfo propertyInfo => propertyInfo.PropertyType,
                             EventInfo eventInfo       => eventInfo.EventHandlerType,
                             _                         => throw new InvalidOperationException($"MemberInfo type {self.MemberType} is not supported.")
                         };

            return type?.IsNullableType() is true        ||
                   type?.IsBuiltInNullableType() is true ||
                   self.GetNullabilityInfo()
                       .ReadState is NullabilityState.NotNull;
        }
    }



    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")] [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public static object? Construct( [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] this Type target, params Type[] args )
    {
        Type type = target.MakeGenericType(args);
        return Activator.CreateInstance(type);
    }
}
