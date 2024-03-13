// Jakar.Extensions :: Jakar.Extensions
// 12/8/2023  17:13


namespace Jakar.Extensions;


public sealed class MethodDetails
{
    public MethodAttributes   Attributes          { get; init; }
    public string?            DeclaringType       { get; init; }
    public bool               IsAbstract          { get; init; }
    public bool               IsAssembly          { get; init; }
    public bool               IsConstructor       { get; init; }
    public bool               IsFamily            { get; init; }
    public bool               IsFamilyAndAssembly { get; init; }
    public bool               IsFamilyOrAssembly  { get; init; }
    public bool               IsFinal             { get; init; }
    public bool               IsPrivate           { get; init; }
    public bool               IsPublic            { get; init; }
    public bool               IsSpecialName       { get; init; }
    public bool               IsStatic            { get; init; }
    public bool               IsVirtual           { get; init; }
    public string             Name                { get; init; } = string.Empty;
    public ParameterDetails[] Parameters          { get; init; } = Array.Empty<ParameterDetails>();
    public string             Signature           { get; init; } = string.Empty;


    public MethodDetails() { }
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(MethodDetails) )]
#endif
    public MethodDetails( Exception e ) : this( e.TargetSite ?? throw new NullReferenceException( nameof(e.TargetSite) ) ) { }
    public MethodDetails( MethodBase method )
    {
        DeclaringType       = method.MethodClass();
        Signature           = method.MethodSignature();
        Name                = method.Name;
        Attributes          = method.Attributes;
        IsSpecialName       = method.IsSpecialName;
        IsStatic            = method.IsStatic;
        IsConstructor       = method.IsConstructor;
        IsFinal             = method.IsFinal;
        IsVirtual           = method.IsVirtual;
        IsAbstract          = method.IsAbstract;
        IsPrivate           = method.IsPrivate;
        IsPublic            = method.IsPublic;
        IsFamily            = method.IsFamily;
        IsAssembly          = method.IsAssembly;
        IsFamilyAndAssembly = method.IsFamilyAndAssembly;
        IsFamilyOrAssembly  = method.IsFamilyOrAssembly;
        Parameters          = ParameterDetails.Create( method );
    }
}
