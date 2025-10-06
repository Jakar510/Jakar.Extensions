// Jakar.Extensions :: Jakar.Extensions
// 12/8/2023  17:13


namespace Jakar.Extensions;


public sealed class MethodDetails : BaseClass<MethodDetails>, IEqualComparable<MethodDetails>, IJsonModel<MethodDetails>
{
    public static JsonTypeInfo<MethodDetails[]> JsonArrayInfo       => JakarExtensionsContext.Default.MethodDetailsArray;
    public static JsonSerializerContext         JsonContext         => JakarExtensionsContext.Default;
    public static JsonTypeInfo<MethodDetails>   JsonTypeInfo        => JakarExtensionsContext.Default.MethodDetails;
    public        MethodAttributes              Attributes          { get; init; }
    public        string?                       DeclaringType       { get; init; }
    public        bool                          IsAbstract          { get; init; }
    public        bool                          IsAssembly          { get; init; }
    public        bool                          IsConstructor       { get; init; }
    public        bool                          IsFamily            { get; init; }
    public        bool                          IsFamilyAndAssembly { get; init; }
    public        bool                          IsFamilyOrAssembly  { get; init; }
    public        bool                          IsFinal             { get; init; }
    public        bool                          IsPrivate           { get; init; }
    public        bool                          IsPublic            { get; init; }
    public        bool                          IsSpecialName       { get; init; }
    public        bool                          IsStatic            { get; init; }
    public        bool                          IsVirtual           { get; init; }
    public        string                        Name                { get; init; } = string.Empty;
    public        ParameterDetails[]            Parameters          { get; init; } = [];
    public        string                        Signature           { get; init; } = string.Empty;


    public MethodDetails() { }
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
        Parameters          = ParameterDetails.Create(method);
    }


    public static MethodDetails? TryCreate( MethodBase? method ) => method is not null
                                                                        ? new MethodDetails(method)
                                                                        : null;
    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public static MethodDetails? TryCreate( Exception e ) => TryCreate(e.TargetSite);


    public override int CompareTo( MethodDetails? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( other is null ) { return 1; }

        int nameComparison = string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        if ( nameComparison != 0 ) { return nameComparison; }

        int attributesComparison = Attributes.CompareTo(other.Attributes);
        if ( attributesComparison != 0 ) { return attributesComparison; }

        int declaringTypeComparison = string.Compare(DeclaringType, other.DeclaringType, StringComparison.InvariantCultureIgnoreCase);
        if ( declaringTypeComparison != 0 ) { return declaringTypeComparison; }

        int isAbstractComparison = IsAbstract.CompareTo(other.IsAbstract);
        if ( isAbstractComparison != 0 ) { return isAbstractComparison; }

        int isAssemblyComparison = IsAssembly.CompareTo(other.IsAssembly);
        if ( isAssemblyComparison != 0 ) { return isAssemblyComparison; }

        int isConstructorComparison = IsConstructor.CompareTo(other.IsConstructor);
        if ( isConstructorComparison != 0 ) { return isConstructorComparison; }

        int isFamilyComparison = IsFamily.CompareTo(other.IsFamily);
        if ( isFamilyComparison != 0 ) { return isFamilyComparison; }

        int isFamilyAndAssemblyComparison = IsFamilyAndAssembly.CompareTo(other.IsFamilyAndAssembly);
        if ( isFamilyAndAssemblyComparison != 0 ) { return isFamilyAndAssemblyComparison; }

        int isFamilyOrAssemblyComparison = IsFamilyOrAssembly.CompareTo(other.IsFamilyOrAssembly);
        if ( isFamilyOrAssemblyComparison != 0 ) { return isFamilyOrAssemblyComparison; }

        int isFinalComparison = IsFinal.CompareTo(other.IsFinal);
        if ( isFinalComparison != 0 ) { return isFinalComparison; }

        int isPrivateComparison = IsPrivate.CompareTo(other.IsPrivate);
        if ( isPrivateComparison != 0 ) { return isPrivateComparison; }

        int isPublicComparison = IsPublic.CompareTo(other.IsPublic);
        if ( isPublicComparison != 0 ) { return isPublicComparison; }

        int isSpecialNameComparison = IsSpecialName.CompareTo(other.IsSpecialName);
        if ( isSpecialNameComparison != 0 ) { return isSpecialNameComparison; }

        int isStaticComparison = IsStatic.CompareTo(other.IsStatic);
        if ( isStaticComparison != 0 ) { return isStaticComparison; }

        int isVirtualComparison = IsVirtual.CompareTo(other.IsVirtual);
        if ( isVirtualComparison != 0 ) { return isVirtualComparison; }

        return string.Compare(Signature, other.Signature, StringComparison.InvariantCultureIgnoreCase);
    }
    public override bool Equals( object?        obj )   => ReferenceEquals(this, obj)   || obj is MethodDetails other && Equals(other);
    public override bool Equals( MethodDetails? other ) => ReferenceEquals(this, other) || other is not null          && string.Equals(Name, other.Name) && string.Equals(DeclaringType, other.DeclaringType);
    public override int GetHashCode()
    {
        HashCode hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add((int)Attributes);
        hashCode.Add(DeclaringType, StringComparer.InvariantCultureIgnoreCase);
        hashCode.Add(IsAbstract);
        hashCode.Add(IsAssembly);
        hashCode.Add(IsConstructor);
        hashCode.Add(IsFamily);
        hashCode.Add(IsFamilyAndAssembly);
        hashCode.Add(IsFamilyOrAssembly);
        hashCode.Add(IsFinal);
        hashCode.Add(IsPrivate);
        hashCode.Add(IsPublic);
        hashCode.Add(IsSpecialName);
        hashCode.Add(IsStatic);
        hashCode.Add(IsVirtual);
        hashCode.Add(Name, StringComparer.InvariantCultureIgnoreCase);
        hashCode.Add(Parameters);
        hashCode.Add(Signature, StringComparer.InvariantCultureIgnoreCase);
        return hashCode.ToHashCode();
    }


    public static bool operator <( MethodDetails?  left, MethodDetails? right ) => Comparer<MethodDetails>.Default.Compare(left, right) < 0;
    public static bool operator >( MethodDetails?  left, MethodDetails? right ) => Comparer<MethodDetails>.Default.Compare(left, right) > 0;
    public static bool operator <=( MethodDetails? left, MethodDetails? right ) => Comparer<MethodDetails>.Default.Compare(left, right) <= 0;
    public static bool operator >=( MethodDetails? left, MethodDetails? right ) => Comparer<MethodDetails>.Default.Compare(left, right) >= 0;
    public static bool operator ==( MethodDetails? left, MethodDetails? right ) => EqualityComparer<MethodDetails>.Default.Equals(left, right);
    public static bool operator !=( MethodDetails? left, MethodDetails? right ) => !EqualityComparer<MethodDetails>.Default.Equals(left, right);
}
