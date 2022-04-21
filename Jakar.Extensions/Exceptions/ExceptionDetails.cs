namespace Jakar.Extensions.General;


public class ExceptionDetails
{
    public string                       Str             { get; init; } = string.Empty;
    public string                       Message         { get; init; } = string.Empty;
    public string                       Type            { get; init; } = string.Empty;
    public string?                      HelpLink        { get; init; }
    public string?                      Source          { get; init; }
    public int                          HResult         { get; init; }
    public List<string>?                StackTrace      { get; init; }
    public string?                      MethodSignature { get; init; }
    public MethodDetails?               TargetSite      { get; init; }
    public Dictionary<string, JToken?>? Data            { get; init; }
    public ExceptionDetails?            Inner           { get; init; }


    public ExceptionDetails() { }

    public ExceptionDetails( Exception e, bool includeMethodInfo = false )
    {
        Message         = e.Message;
        HResult         = e.HResult;
        Type            = e.GetType().FullName;
        HelpLink        = e.HelpLink;
        Source          = e.Source;
        StackTrace      = e.StackTrace?.SplitAndTrimLines().ToList();
        MethodSignature = $"{e.MethodClass()}::{e.MethodSignature()}";
        Data            = e.GetData();
        Str             = e.ToString();


        if ( includeMethodInfo ) { TargetSite = e.MethodInfo(); }


        Inner = e.InnerException is null
                    ? null
                    : new ExceptionDetails(e.InnerException);
    }
}



public class MethodDetails
{
    public string?                 DeclaringType       { get; init; }
    public string                  Name                { get; init; } = string.Empty;
    public string                  Signature           { get; init; } = string.Empty;
    public MethodAttributes        Attributes          { get; init; }
    public bool                    IsSpecialName       { get; init; }
    public bool                    IsStatic            { get; init; }
    public bool                    IsConstructor       { get; init; }
    public bool                    IsFinal             { get; init; }
    public bool                    IsVirtual           { get; init; }
    public bool                    IsAbstract          { get; init; }
    public bool                    IsPrivate           { get; init; }
    public bool                    IsPublic            { get; init; }
    public bool                    IsFamily            { get; init; }
    public bool                    IsAssembly          { get; init; }
    public bool                    IsFamilyAndAssembly { get; init; }
    public bool                    IsFamilyOrAssembly  { get; init; }
    public List<ParameterDetails>? Parameters          { get; init; }


    public MethodDetails() { }
    public MethodDetails( Exception e ) : this(e.TargetSite ?? throw new NullReferenceException(nameof(e.TargetSite))) { }

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
}



public class ParameterDetails
{
    public string  Name            { get; init; } = string.Empty;
    public int     Position        { get; init; }
    public bool    IsIn            { get; init; }
    public bool    IsOut           { get; init; }
    public bool    IsOptional      { get; init; }
    public bool    HasDefaultValue { get; init; }
    public string? Type            { get; init; }


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


    public static List<ParameterDetails> Create( MethodBase                 method ) => Create(method.GetParameters());
    public static List<ParameterDetails> Create( IEnumerable<ParameterInfo> items ) => new(items.Select(x => new ParameterDetails(x)));
}
