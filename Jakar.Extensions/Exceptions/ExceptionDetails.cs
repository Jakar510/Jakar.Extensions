namespace Jakar.Extensions;


public sealed class ExceptionDetails
{
    public Dictionary<string, JToken?>? Data            { get; init; }
    public string?                      HelpLink        { get; init; }
    public int                          HResult         { get; init; }
    public ExceptionDetails?            Inner           { get; init; }
    public string                       Message         { get; init; } = string.Empty;
    public string?                      MethodSignature { get; init; }
    public string?                      Source          { get; init; }
    public string[]                     StackTrace      { get; init; } = Array.Empty<string>();
    public string                       Str             { get; init; } = string.Empty;
    public MethodDetails?               TargetSite      { get; init; }
    public string?                      Type            { get; init; }


    public ExceptionDetails() { }


    [RequiresUnreferencedCode( nameof(ExceptionDetails) )]
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
