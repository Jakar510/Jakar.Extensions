namespace Jakar.Extensions;


public static class ExceptionExtensions
{
    [RequiresUnreferencedCode( nameof(GetInnerExceptions) )]
    private static Dictionary<string, object?> GetInnerExceptions( this Exception e, ref Dictionary<string, object?> dict, bool includeFullMethodInfo )
    {
        if ( e is null ) { throw new NullReferenceException( nameof(e) ); }

        if ( e.InnerException is null ) { return dict; }

        e.Details( out Dictionary<string, object?> inner, includeFullMethodInfo );

        dict[nameof(e.InnerException)] = e.InnerException.GetInnerExceptions( ref inner, includeFullMethodInfo );

        return dict;
    }


    [RequiresUnreferencedCode( nameof(GetProperties) )]
    public static Dictionary<string, object?> GetProperties( this Exception e )
    {
        Dictionary<string, object?> dictionary = new();

        e.GetProperties( ref dictionary );

        return dictionary;
    }


    [RequiresUnreferencedCode( nameof(Details) )]     public static ExceptionDetails Details( this     Exception e ) => new(e);
    [RequiresUnreferencedCode( nameof(FullDetails) )] public static ExceptionDetails FullDetails( this Exception e ) => new(e, true);

    [RequiresUnreferencedCode( nameof(Frames) )]
    public static IEnumerable<string> Frames( StackTrace trace )
    {
        foreach ( StackFrame frame in trace.GetFrames() )
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


    [RequiresUnreferencedCode( nameof(MethodInfo) )] public static MethodDetails? MethodInfo( this Exception e ) => e.TargetSite?.MethodInfo();
    [RequiresUnreferencedCode( nameof(CallStack) )]  public static string         CallStack( Exception       e ) => CallStack( new StackTrace( e ) );
    [RequiresUnreferencedCode( nameof(CallStack) )]  public static string         CallStack()                    => CallStack( new StackTrace() );
    [RequiresUnreferencedCode( nameof(CallStack) )]  public static string         CallStack( StackTrace trace )  => string.Join( "->", Frames( trace ) );

    [RequiresUnreferencedCode( nameof(Frame) )]
    public static string Frame( StackFrame frame )
    {
        MethodBase method    = frame.GetMethod()    ?? throw new NullReferenceException( nameof(frame.GetMethod) );
        string     className = method.MethodClass() ?? throw new NullReferenceException( nameof(TypeExtensions.MethodClass) );

        return $"{className}::{method.Name}";
    }


    [RequiresUnreferencedCode( nameof(MethodClass) )] public static string? MethodClass( this Exception e ) => e.TargetSite?.MethodClass();


    [RequiresUnreferencedCode( "Metadata for the method might be incomplete or removed" )] public static string? MethodName( this Exception e ) => e.TargetSite?.MethodName();


    [RequiresUnreferencedCode( nameof(MethodSignature) )] public static string? MethodSignature( this Exception e ) => e.TargetSite?.MethodSignature();


    public static Dictionary<string, JToken?> GetData( this Exception e )
    {
        Dictionary<string, JToken?> data = new();

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

    [RequiresUnreferencedCode( nameof(CallStack) )]
    public static void Details( this Exception e, out Dictionary<string, string?> dict )
    {
        dict = new Dictionary<string, string?>( 10 );
        e.Details( dict );
    }

    [RequiresUnreferencedCode( nameof(CallStack) )]
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


    [RequiresUnreferencedCode( nameof(CallStack) )]
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
    public static void GetProperties<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicProperties )] T>( this T e, ref Dictionary<string, object?> dictionary )
        where T : Exception
    {
        foreach ( PropertyInfo info in typeof(T).GetProperties( BindingFlags.Instance | BindingFlags.Public ) )
        {
            string key = info.Name;

            if ( dictionary.ContainsKey( key ) || !info.CanRead || key == "TargetSite" ) { continue; }

            dictionary[key] = info.GetValue( e, null );
        }
    }


    public static void GetProperties<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicProperties )] T>( this T e, ref Dictionary<string, JToken?> dictionary )
        where T : Exception
    {
        foreach ( PropertyInfo info in typeof(T).GetProperties( BindingFlags.Instance | BindingFlags.Public ) )
        {
            string key = info.Name;

            if ( dictionary.ContainsKey( key ) || !info.CanRead || key == "TargetSite" ) { continue; }


            object? value = info.GetValue( e, null );

            dictionary[key] = value is not null
                                  ? JToken.FromObject( value )
                                  : null;
        }
    }


    [RequiresUnreferencedCode( nameof(Details) )]
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


/*
    public static void GetProperties<[ DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicProperties ) ] T>( this T e, ref Dictionary<string, JToken?> dictionary )
        where T : Exception
    {
        foreach ( PropertyInfo info in typeof(T).GetProperties( BindingFlags.Instance | BindingFlags.Public ) )
        {
            string key = info.AppName;

            if ( dictionary.ContainsKey( key ) || !info.CanRead || key == "TargetSite" ) { continue; }


            dictionary[key] = info.GetValue( e, null )?.ToJson();
        }
    }

    [ RequiresUnreferencedCode( nameof(Details) ) ]
    public static void Details( this Exception e, out Dictionary<string, JToken?> dict, bool includeFullMethodInfo )
    {
        dict = new Dictionary<string, JToken?>
               {
                   [nameof(Type)] = e.GetType().FullName,
                   [nameof(Exception.HResult)] = e.HResult,
                   [nameof(Exception.HelpLink)] = e.HelpLink,
                   [nameof(Exception.Source)] = e.Source,
                   [nameof(Exception.Message)] = e.Message,
                   [nameof(Exception.Data)] = e.GetData().ToJson(),
                   [nameof(Exception.StackTrace)] = (e.StackTrace?.SplitAndTrimLines() ?? Array.Empty<string>()).ToJson()
               };


        if ( includeFullMethodInfo )
        {
            MethodDetails? info = e.MethodInfo();
            dict[nameof(Exception.TargetSite)] = JsonSerializer.SerializeToNode( info, MethodDetailsContext.MethodDetails );
        }
        else if ( e.TargetSite is not null ) { dict[nameof(Exception.TargetSite)] = $"{e.MethodClass()}::{e.MethodSignature()}"; }

        e.GetProperties( ref dict );
    }
*/
}
