// Jakar.Extensions :: Jakar.Extensions
// 12/8/2023  17:13


namespace Jakar.Extensions;


public sealed class MethodDetails : IJsonModel<MethodDetails>
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
    public ParameterDetails[] Parameters          { get; init; } = [];
    public string             Signature           { get; init; } = string.Empty;


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


    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")]
    public static MethodDetails? TryCreate( MethodBase? method ) => method is not null
                                                                        ? new MethodDetails(method)
                                                                        : null;
    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public static MethodDetails? TryCreate( Exception e ) => TryCreate(e.TargetSite);


    public static JsonTypeInfo<MethodDetails> JsonTypeInfo => JakarExtensionsContext.Default.MethodDetails;
    public static JsonSerializerContext       JsonContext  => JakarExtensionsContext.Default;
    public        JsonNode                    ToJsonNode() => Validate.ThrowIfNull(JsonSerializer.SerializeToNode(this, JsonTypeInfo));
    public        string                      ToJson()     => Validate.ThrowIfNull(JsonSerializer.Serialize(this, JsonTypeInfo));
    public static bool TryFromJson( string? json, [NotNullWhen(true)] out MethodDetails? result )
    {
        try
        {
            if ( string.IsNullOrWhiteSpace(json) )
            {
                result = null;
                return false;
            }

            result = FromJson(json);
            return true;
        }
        catch ( Exception e ) { SelfLogger.WriteLine("{Exception}", e.ToString()); }

        result = null;
        return false;
    }
    public static MethodDetails FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));
}