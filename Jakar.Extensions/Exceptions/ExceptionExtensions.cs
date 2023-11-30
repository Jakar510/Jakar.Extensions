using System.Text.Json.Nodes;



namespace Jakar.Extensions;


public static class ExceptionExtensions
{
    private static Dictionary<string, object?> GetInnerExceptions( this Exception e, ref Dictionary<string, object?> dict, bool includeFullMethodInfo )
    {
        if ( e is null ) { throw new NullReferenceException( nameof(e) ); }

        if ( e.InnerException is null ) { return dict; }

        e.Details( out Dictionary<string, object?> inner, includeFullMethodInfo );

        dict[nameof(e.InnerException)] = e.InnerException.GetInnerExceptions( ref inner, includeFullMethodInfo );

        return dict;
    }


    public static Dictionary<string, object?> GetProperties( this Exception e )
    {
        var dictionary = new Dictionary<string, object?>();

        e.GetProperties( ref dictionary );

        return dictionary;
    }


    private static Dictionary<string, object?>? GetInnerExceptions( this Exception e, bool includeFullMethodInfo )
    {
        if ( e.InnerException is null ) { return null; }

        var innerDetails = new Dictionary<string, object?>();

        return e.InnerException.GetInnerExceptions( ref innerDetails, includeFullMethodInfo );
    }


    public static ExceptionDetails Details( this     Exception e ) => new(e);
    public static ExceptionDetails FullDetails( this Exception e ) => new(e, true);
    public static IEnumerable<string> Frames( StackTrace trace )
    {
        foreach ( StackFrame frame in trace.GetFrames() ?? Array.Empty<StackFrame>() )
        {
            MethodBase method    = frame.GetMethod()    ?? throw new NullReferenceException( nameof(frame.GetMethod) );
            string     className = method.MethodClass() ?? throw new NullReferenceException( nameof(TypeExtensions.MethodClass) );


            switch ( method.Name )
            {
                case nameof(CallStack) when className == nameof(ExceptionExtensions):
                case nameof(Frames) when className    == nameof(ExceptionExtensions):
                case nameof(Frame) when className     == nameof(ExceptionExtensions):
                    continue;

                default:
                    yield return $"{className}::{method.Name}";
                    break;
            }
        }
    }


    public static MethodDetails? MethodInfo( this Exception e ) => e.TargetSite?.MethodInfo();
    public static string         CallStack( Exception       e ) => CallStack( new StackTrace( e ) );
    public static string         CallStack()                    => CallStack( new StackTrace() );
    public static string         CallStack( StackTrace trace )  => string.Join( "->", Frames( trace ) );
    public static string Frame( StackFrame frame )
    {
        MethodBase method    = frame.GetMethod()    ?? throw new NullReferenceException( nameof(frame.GetMethod) );
        string     className = method.MethodClass() ?? throw new NullReferenceException( nameof(TypeExtensions.MethodClass) );

        return $"{className}::{method.Name}";
    }
    public static string? MethodClass( this     Exception e ) => e.TargetSite?.MethodClass();
    public static string? MethodName( this      Exception e ) => e.TargetSite?.MethodName();
    public static string? MethodSignature( this Exception e ) => e.TargetSite?.MethodSignature();


    public static Dictionary<string, JToken?> GetData( this Exception e )
    {
        var data = new Dictionary<string, JToken?>();

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach ( DictionaryEntry pair in e.Data )
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            string? key = pair.Key?.ToString();
            if ( key is null ) { continue; }

            data[key] = pair.Value is null
                            ? null
                            : JToken.FromObject( pair.Value );
        }

        return data;
    }
    public static void Details( this Exception e, out Dictionary<string, string?> dict )
    {
        dict = new Dictionary<string, string?>( 10 );
        e.Details( dict );
    }

    public static void Details<T>( this Exception e, in T dict )
        where T : class, IDictionary<string, string?>
    {
        dict[nameof(Type)]               = e.GetType().FullName;
        dict[nameof(e.Source)]           = e.Source;
        dict[nameof(e.Message)]          = e.Message;
        dict[nameof(e.StackTrace)]       = e.StackTrace;
        dict[nameof(Exception.HelpLink)] = e.HelpLink;
        dict[nameof(MethodSignature)]    = e.MethodSignature();
        dict[nameof(e.ToString)]         = e.ToString();
    }


    public static void Details( this Exception e, out Dictionary<string, object?> dict, bool includeFullMethodInfo )
    {
        dict = new Dictionary<string, object?>
               {
                   [nameof(Type)]                 = e.GetType().FullName,
                   [nameof(Exception.HResult)]    = e.HResult,
                   [nameof(Exception.HelpLink)]   = e.HelpLink,
                   [nameof(Exception.Source)]     = e.Source,
                   [nameof(Exception.Message)]    = e.Message,
                   [nameof(Exception.Data)]       = e.GetData(),
                   [nameof(Exception.StackTrace)] = e.StackTrace?.SplitAndTrimLines()
               };


        if ( includeFullMethodInfo ) { dict[nameof(Exception.TargetSite)]         = e.MethodInfo(); }
        else if ( e.TargetSite is not null ) { dict[nameof(Exception.TargetSite)] = $"{e.MethodClass()}::{e.MethodSignature()}"; }

        e.GetProperties( ref dict );
    }
    public static void GetProperties( this Exception e, ref Dictionary<string, object?> dictionary )
    {
        foreach ( PropertyInfo info in e.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public ) )
        {
            string key = info.Name;

            if ( dictionary.ContainsKey( key ) || !info.CanRead || key == "TargetSite" ) { continue; }

            dictionary[key] = info.GetValue( e, null );
        }
    }


    public static void GetProperties( this Exception e, ref Dictionary<string, JToken?> dictionary )
    {
        foreach ( PropertyInfo info in e.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public ) )
        {
            string key = info.Name;

            if ( dictionary.ContainsKey( key ) || !info.CanRead || key == "TargetSite" ) { continue; }

            dictionary[key] = JToken.FromObject( info.GetValue( e, null ) );
        }
    }
    public static void Details( this Exception e, out Dictionary<string, JToken?> dict, bool includeFullMethodInfo )
    {
        dict = new Dictionary<string, JToken?>
               {
                   [nameof(Type)]                 = e.GetType().FullName,
                   [nameof(Exception.HResult)]    = e.HResult,
                   [nameof(Exception.HelpLink)]   = e.HelpLink,
                   [nameof(Exception.Source)]     = e.Source,
                   [nameof(Exception.Message)]    = e.Message,
                   [nameof(Exception.Data)]       = JToken.FromObject( e.GetData() ),
                   [nameof(Exception.StackTrace)] = JToken.FromObject( e.StackTrace?.SplitAndTrimLines() ?? Array.Empty<string>() )
               };


        if ( includeFullMethodInfo )
        {
            MethodDetails? info = e.MethodInfo();

            dict[nameof(Exception.TargetSite)] = info is not null
                                                     ? JToken.FromObject( info )
                                                     : null;
        }
        else if ( e.TargetSite is not null ) { dict[nameof(Exception.TargetSite)] = $"{e.MethodClass()}::{e.MethodSignature()}"; }

        e.GetProperties( ref dict );
    }


    public static void GetProperties( this Exception e, ref Dictionary<string, JsonNode?> dictionary )
    {
        foreach ( PropertyInfo info in e.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public ) )
        {
            string key = info.Name;

            if ( dictionary.ContainsKey( key ) || !info.CanRead || key == "TargetSite" ) { continue; }


            dictionary[key] = System.Text.Json.JsonSerializer.SerializeToNode( info.GetValue( e, null ) );
        }
    }
    public static void Details( this Exception e, out Dictionary<string, JsonNode?> dict, bool includeFullMethodInfo )
    {
        dict = new Dictionary<string, JsonNode?>
               {
                   [nameof(Type)]                 = e.GetType().FullName,
                   [nameof(Exception.HResult)]    = e.HResult,
                   [nameof(Exception.HelpLink)]   = e.HelpLink,
                   [nameof(Exception.Source)]     = e.Source,
                   [nameof(Exception.Message)]    = e.Message,
                   [nameof(Exception.Data)]       = System.Text.Json.JsonSerializer.SerializeToNode( e.GetData() ),
                   [nameof(Exception.StackTrace)] = System.Text.Json.JsonSerializer.SerializeToNode( e.StackTrace?.SplitAndTrimLines() ?? Array.Empty<string>() )
               };


        if ( includeFullMethodInfo )
        {
            MethodDetails? info = e.MethodInfo();

            dict[nameof(Exception.TargetSite)] = info is not null
                                                     ? System.Text.Json.JsonSerializer.SerializeToNode( info )
                                                     : null;
        }
        else if ( e.TargetSite is not null ) { dict[nameof(Exception.TargetSite)] = $"{e.MethodClass()}::{e.MethodSignature()}"; }

        e.GetProperties( ref dict );
    }
}
