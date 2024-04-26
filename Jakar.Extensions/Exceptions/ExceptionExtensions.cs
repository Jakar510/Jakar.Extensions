namespace Jakar.Extensions;


public static class ExceptionExtensions
{
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(GetInnerExceptions) )]
#endif
    private static Dictionary<string, object?> GetInnerExceptions( this Exception e, ref Dictionary<string, object?> dict, bool includeFullMethodInfo )
    {
        if ( e is null ) { throw new NullReferenceException( nameof(e) ); }

        if ( e.InnerException is null ) { return dict; }

        e.Details( out Dictionary<string, object?> inner, includeFullMethodInfo );

        dict[nameof(e.InnerException)] = e.InnerException.GetInnerExceptions( ref inner, includeFullMethodInfo );

        return dict;
    }


#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(GetProperties) )]
#endif
    public static Dictionary<string, object?> GetProperties( this Exception e )
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object?>();

        e.GetProperties( ref dictionary );

        return dictionary;
    }


#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(Details) )]
#endif
    public static ExceptionDetails Details( this Exception e ) => new(e);


#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(FullDetails) )]
#endif
    public static ExceptionDetails FullDetails( this Exception e ) => new(e, true);

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(Frames) )]
#endif
    public static IEnumerable<string> Frames( StackTrace trace )
    {
        foreach ( StackFrame frame in trace.GetFrames()! )
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


#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(MethodInfo) )]
#endif
    public static MethodDetails? MethodInfo( this Exception e ) => e.TargetSite?.MethodInfo();

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(CallStack) )]
#endif
    public static string CallStack( Exception e ) => CallStack( new StackTrace( e ) );

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(CallStack) )]
#endif
    public static string CallStack() => CallStack( new StackTrace() );

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(CallStack) )]
#endif
    public static string CallStack( StackTrace trace ) => string.Join( "->", Frames( trace ) );

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(Frame) )]
#endif
    public static string Frame( StackFrame frame )
    {
        MethodBase method    = frame.GetMethod()    ?? throw new NullReferenceException( nameof(frame.GetMethod) );
        string     className = method.MethodClass() ?? throw new NullReferenceException( nameof(TypeExtensions.MethodClass) );

        return $"{className}::{method.Name}";
    }


#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(MethodClass) )]
#endif
    public static string? MethodClass( this Exception e ) => e.TargetSite?.MethodClass();


#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( "Metadata for the method might be incomplete or removed" )]
#endif
    public static string? MethodName( this Exception e ) => e.TargetSite?.MethodName();


#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(MethodSignature) )]
#endif
    public static string? MethodSignature( this Exception e ) => e.TargetSite?.MethodSignature();


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

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(CallStack) )]
#endif
    public static void Details( this Exception e, out Dictionary<string, string?> dict )
    {
        dict = new Dictionary<string, string?>( 10 );
        e.Details( dict );
    }

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(CallStack) )]
#endif
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


#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(CallStack) )]
#endif
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
    public static void GetProperties<
    #if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicProperties )]
    #endif
        T>( this T e, ref Dictionary<string, object?> dictionary )
        where T : Exception
    {
        foreach ( PropertyInfo info in typeof(T).GetProperties( BindingFlags.Instance | BindingFlags.Public ) )
        {
            string key = info.Name;

            if ( dictionary.ContainsKey( key ) || !info.CanRead || key == "TargetSite" ) { continue; }

            dictionary[key] = info.GetValue( e, null );
        }
    }


    public static void GetProperties<
    #if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicProperties )]
    #endif
        T>( this T e, ref Dictionary<string, JToken?> dictionary )
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


#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( nameof(Details) )]
#endif
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
#if NET6_0_OR_GREATER
    public static void GetProperties<[ DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicProperties ) ] T>( this T e, ref Dictionary<string, JToken?> dictionary )
        where T : Exception
    {
        foreach ( PropertyInfo info in typeof(T).GetProperties( BindingFlags.Instance | BindingFlags.Public ) )
        {
            string key = info.Name;

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
#endif
*/
}
