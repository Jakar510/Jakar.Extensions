// Jakar.Extensions :: Jakar.Extensions
// 10/20/2022  11:56 AM

#if !NET6_0_OR_GREATER



// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;


/// <summary>
///     <see href="https://stackoverflow.com/a/70034587/9530917"/>
/// </summary>
[AttributeUsage( AttributeTargets.Parameter )]
public sealed class CallerArgumentExpressionAttribute( string parameterName ) : Attribute
{
    public string ParameterName { get; } = parameterName;
}



#endif
