using System.Text.Json.Serialization;



namespace Jakar.Extensions;


public sealed class ExceptionDetails
{
    public                               Dictionary<string, JToken?>? Data            { get; init; }
    [ MaxLength( 4096 ) ] public         string?                      HelpLink        { get; init; }
    public                               int                          HResult         { get; init; }
    public                               ExceptionDetails?            Inner           { get; init; }
    [ MaxLength( int.MaxValue ) ] public string                       Message         { get; init; } = string.Empty;
    [ MaxLength( 10240 ) ]        public string?                      MethodSignature { get; init; }
    [ MaxLength( 4096 ) ]         public string?                      Source          { get; init; }
    public                               string[]                     StackTrace      { get; init; } = Array.Empty<string>();
    [ MaxLength( 4096 ) ] public         string                       Str             { get; init; } = string.Empty;
    public                               MethodDetails?               TargetSite      { get; init; }
    [ MaxLength( 4096 ) ] public         string?                      Type            { get; init; }


    public ExceptionDetails() { }
    public ExceptionDetails( Exception e, bool includeMethodInfo = false )
    {
        Message = e.Message;
        HResult = e.HResult;

        Type = e.GetType().FullName;

        HelpLink = e.HelpLink;
        Source   = e.Source;

        StackTrace = e.StackTrace?.SplitAndTrimLines().ToArray() ?? Array.Empty<string>();

        MethodSignature = $"{e.MethodClass()}::{e.MethodSignature()}";
        Data            = e.GetData();
        Str             = e.ToString();


        if ( includeMethodInfo ) { TargetSite = e.MethodInfo(); }


        Inner = e.InnerException is null
                    ? null
                    : new ExceptionDetails( e.InnerException );
    }
}



public sealed class MethodDetails
{
    public                        MethodAttributes   Attributes          { get; init; }
    [ MaxLength( 10240 ) ] public string?            DeclaringType       { get; init; }
    public                        bool               IsAbstract          { get; init; }
    public                        bool               IsAssembly          { get; init; }
    public                        bool               IsConstructor       { get; init; }
    public                        bool               IsFamily            { get; init; }
    public                        bool               IsFamilyAndAssembly { get; init; }
    public                        bool               IsFamilyOrAssembly  { get; init; }
    public                        bool               IsFinal             { get; init; }
    public                        bool               IsPrivate           { get; init; }
    public                        bool               IsPublic            { get; init; }
    public                        bool               IsSpecialName       { get; init; }
    public                        bool               IsStatic            { get; init; }
    public                        bool               IsVirtual           { get; init; }
    [ MaxLength( 4096 ) ] public  string             Name                { get; init; } = string.Empty;
    public                        ParameterDetails[] Parameters          { get; init; } = Array.Empty<ParameterDetails>();
    [ MaxLength( 10240 ) ] public string             Signature           { get; init; } = string.Empty;


    public MethodDetails() { }
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



public sealed class ParameterDetails
{
    public                        bool    HasDefaultValue { get; init; }
    public                        bool    IsIn            { get; init; }
    public                        bool    IsOptional      { get; init; }
    public                        bool    IsOut           { get; init; }
    [ MaxLength( 4096 ) ] public  string? Name            { get; init; }
    public                        int     Position        { get; init; }
    [ MaxLength( 10240 ) ] public string? Type            { get; init; }


    public ParameterDetails() { }
    public ParameterDetails( ParameterInfo parameter )
    {
        Name            = parameter.Name;
        Position        = parameter.Position;
        IsIn            = parameter.IsIn;
        IsOut           = parameter.IsOut;
        IsOptional      = parameter.IsOptional;
        HasDefaultValue = parameter.HasDefaultValue;
        Type            = parameter.ParameterType.FullName;
    }


    public static ParameterDetails[] Create( MethodBase                 method ) => Create( method.GetParameters() );
    public static ParameterDetails[] Create( IEnumerable<ParameterInfo> items )  => items.Select( x => new ParameterDetails( x ) ).ToArray();
}



// [ JsonConverter( typeof(ParameterDetails) ) ] public partial class ParameterDetailsContext : JsonSerializerContext { }
