// Jakar.Extensions :: Jakar.Extensions
// 09/30/2022  6:57 PM

namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <see href="https://github.com/dotnet/roslyn/blob/main/docs/features/nullable-metadata.md"/>
///     </para>
///     <para>
///         <see href="https://stackoverflow.com/a/58454489/9530917"/>
///     </para>
///     <para>
///         <see href="https://github.com/RicoSuter/Namotion.Reflection"/>
///     </para>
/// </summary>
[SuppressMessage( "ReSharper", "NullableWarningSuppressionIsUsed" )]
public static partial class TypeExtensions
{
    private const          string NULLABLE         = "System.Runtime.CompilerServices.NullableAttribute";
    private const          string NULLABLE_CONTEXT = "System.Runtime.CompilerServices.NullableContextAttribute";
    public static readonly Type   NullableType     = typeof(Nullable<>);
    public static          bool   IsNullable( this PropertyInfo  property )  => property.PropertyType.IsNullableHelper( property.DeclaringType, property.CustomAttributes );
    public static          bool   IsNullable( this FieldInfo     field )     => field.FieldType.IsNullableHelper( field.DeclaringType, field.CustomAttributes );
    public static          bool   IsNullable( this ParameterInfo parameter ) => parameter.ParameterType.IsNullableHelper( parameter.Member, parameter.CustomAttributes );
    private static bool IsNullableHelper( this Type memberType, MemberInfo? declaringType, IEnumerable<CustomAttributeData> customAttributes )
    {
        if ( memberType.IsValueType ) { return Nullable.GetUnderlyingType( memberType ) != null; }

        CustomAttributeData? nullable = customAttributes.FirstOrDefault( x => x.AttributeType.FullName == NULLABLE );

        if ( nullable is not null && nullable.ConstructorArguments.Count == 1 )
        {
            CustomAttributeTypedArgument attributeArgument = nullable.ConstructorArguments[0];

            if ( attributeArgument.ArgumentType == typeof(byte[]) )
            {
                var args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value!;

                if ( args.Count > 0 && args[0].ArgumentType == typeof(byte) ) { return (byte)args[0].Value! == 2; }
            }
            else if ( attributeArgument.ArgumentType == typeof(byte) ) { return (byte)attributeArgument.Value! == 2; }
        }

        for ( MemberInfo? type = declaringType; type != null; type = type.DeclaringType )
        {
            CustomAttributeData? context = type.CustomAttributes.FirstOrDefault( x => x.AttributeType.FullName == NULLABLE_CONTEXT );

            if ( context is not null && context.ConstructorArguments.Count == 1 && context.ConstructorArguments[0].ArgumentType == typeof(byte) ) { return (byte)context.ConstructorArguments[0].Value! == 2; }
        }

        return false; // Couldn't find a suitable attribute
    }


    public static bool TryGetUnderlyingEnumType( this Type propertyType, [NotNullWhen( true )] out Type? result )
    {
        if ( propertyType.IsEnum )
        {
            result = Enum.GetUnderlyingType( propertyType );
            return true;
        }

        if ( propertyType.IsGenericType )
        {
            if ( propertyType.GetGenericTypeDefinition() == typeof(Nullable<>) )
            {
                Type argType = propertyType.GenericTypeArguments.First();

                if ( argType.IsEnum )
                {
                    result = Enum.GetUnderlyingType( argType );
                    return true;
                }
            }
        }

        result = default;
        return false;
    }
}
