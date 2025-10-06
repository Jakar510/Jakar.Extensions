namespace Jakar.Extensions;


public static class ExceptionExtensions
{
    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + Json.SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(Json.SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
    public static Dictionary<string, object?> GetInnerExceptions( this Exception e, ref Dictionary<string, object?> dict, bool includeFullMethodInfo )
    {
        if ( e is null ) { throw new NullReferenceException(nameof(e)); }

        if ( e.InnerException is null ) { return dict; }

        e.Details(out Dictionary<string, object?> inner, includeFullMethodInfo);

        dict[nameof(e.InnerException)] = e.InnerException.GetInnerExceptions(ref inner, includeFullMethodInfo);

        return dict;
    }


    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + Json.SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(Json.SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
    public static Dictionary<string, object?> GetProperties( this Exception e )
    {
        Dictionary<string, object?> dictionary = new();

        e.GetProperties(ref dictionary);

        return dictionary;
    }


    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + Json.SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(Json.SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
    public static ExceptionDetails Details( this Exception e ) => new(e, false);


    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + Json.SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(Json.SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
    public static ExceptionDetails FullDetails( this Exception e ) => new(e);


    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public static IEnumerable<string> Frames( StackTrace trace )
    {
        foreach ( StackFrame frame in trace.GetFrames() )
        {
            MethodBase method    = frame.GetMethod()    ?? throw new NullReferenceException(nameof(frame.GetMethod));
            string     className = method.MethodClass() ?? throw new NullReferenceException(nameof(Types.MethodClass));


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


    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public static MethodDetails? MethodInfo( this Exception e ) => e.TargetSite?.MethodInfo();
    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public static string         CallStack( Exception       e ) => CallStack(new StackTrace(e));
    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public static string         CallStack()                    => CallStack(new StackTrace());
    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public static string         CallStack( StackTrace trace )  => string.Join("->", Frames(trace));


    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public static string Frame( StackFrame frame )
    {
        MethodBase method    = frame.GetMethod()    ?? throw new NullReferenceException(nameof(frame.GetMethod));
        string     className = method.MethodClass() ?? throw new NullReferenceException(nameof(Types.MethodClass));

        return $"{className}::{method.Name}";
    }


    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public static string? MethodClass( this     Exception e ) => e.TargetSite?.MethodClass();
    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public static string? MethodName( this      Exception e ) => e.TargetSite?.MethodName();
    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public static string? MethodSignature( this Exception e ) => e.TargetSite?.MethodSignature();


    [RequiresUnreferencedCode(Json.SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(Json.SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
    public static JsonNode? GetData( this Exception e ) => JsonSerializer.SerializeToNode(e.Data, Json.Options);


    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + Json.SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(Json.SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
    public static void Details( this Exception e, out Dictionary<string, string?> dict )
    {
        dict = new Dictionary<string, string?>(10);
        e.Details(dict);
    }


    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + Json.SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(Json.SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
    public static void Details<TValue>( this Exception e, in TValue dict )
        where TValue : class, IDictionary<string, string?>
    {
        dict[nameof(Type)] = e.GetType()
                              .FullName;

        dict[nameof(e.Source)]           = e.Source;
        dict[nameof(e.Message)]          = e.Message;
        dict[nameof(e.StackTrace)]       = e.StackTrace;
        dict[nameof(Exception.HelpLink)] = e.HelpLink;
        dict[nameof(MethodSignature)]    = e.MethodSignature();
        dict[nameof(e.ToString)]         = e.ToString();
    }


    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + Json.SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(Json.SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
    public static void Details( this Exception e, out Dictionary<string, object?> dict, bool includeFullMethodInfo )
    {
        dict = new Dictionary<string, object?>
               {
                   [nameof(Type)] = e.GetType()
                                     .FullName,
                   [nameof(Exception.HResult)]    = e.HResult,
                   [nameof(Exception.HelpLink)]   = e.HelpLink,
                   [nameof(Exception.Source)]     = e.Source,
                   [nameof(Exception.Message)]    = e.Message,
                   [nameof(Exception.Data)]       = e.GetData(),
                   [nameof(Exception.StackTrace)] = e.StackTrace?.SplitAndTrimLines()
               };


        if ( includeFullMethodInfo ) { dict[nameof(Exception.TargetSite)]         = e.MethodInfo(); }
        else if ( e.TargetSite is not null ) { dict[nameof(Exception.TargetSite)] = $"{e.MethodClass()}::{e.MethodSignature()}"; }

        e.GetProperties(ref dict);
    }


    public static void GetProperties<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TValue>( this TValue e, ref Dictionary<string, object?> dictionary )
        where TValue : Exception
    {
        foreach ( PropertyInfo info in typeof(TValue).GetProperties(BindingFlags.Instance | BindingFlags.Public) )
        {
            string key = info.Name;

            if ( dictionary.ContainsKey(key) || !info.CanRead || key == "TargetSite" ) { continue; }

            dictionary[key] = info.GetValue(e, null);
        }
    }


    [RequiresUnreferencedCode(Json.SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(Json.SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
    public static void GetProperties<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TValue>( this TValue e, ref JsonObject dictionary )
        where TValue : Exception
    {
        foreach ( PropertyInfo info in typeof(TValue).GetProperties(BindingFlags.Instance | BindingFlags.Public) )
        {
            string key = info.Name;
            if ( dictionary.ContainsKey(key) || !info.CanRead || key == "TargetSite" ) { continue; }

            object? value = info.GetValue(e, null);
            dictionary[key] = JsonSerializer.SerializeToNode(value, Json.Options);
        }
    }


    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + Json.SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(Json.SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
    public static void Details( this Exception e, out JsonObject dict, bool includeFullMethodInfo )
    {
        JsonArray            array = new();
        ReadOnlySpan<string> lines = e.StackTrace?.SplitAndTrimLines();

        foreach ( string line in lines )
        {
            JsonNode node = line;
            array.Add(node);
        }

        dict = new JsonObject
               {
                   [nameof(Type)] = e.GetType()
                                     .FullName,
                   [nameof(Exception.HResult)]    = e.HResult,
                   [nameof(Exception.HelpLink)]   = e.HelpLink,
                   [nameof(Exception.Source)]     = e.Source,
                   [nameof(Exception.Message)]    = e.Message,
                   [nameof(Exception.Data)]       = e.GetData(),
                   [nameof(Exception.StackTrace)] = array
               };

        if ( includeFullMethodInfo )
        {
            MethodDetails? info = e.MethodInfo();
            dict[nameof(Exception.TargetSite)] = info?.ToJsonNode();
        }
        else if ( e.TargetSite is not null ) { dict[nameof(Exception.TargetSite)] = $"{e.MethodClass()}::{e.MethodSignature()}"; }

        e.GetProperties(ref dict);
    }


/*
    public static void GetProperties<[ DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicProperties ) ] TValue>( this TValue e, ref JsonObject dictionary )
        where TValue : Exception
    {
        foreach ( PropertyInfo info in typeof(TValue).GetProperties( BindingFlags.Instance | BindingFlags.Public ) )
        {
            string key = info.AppName;

            if ( dictionary.ContainsKey( key ) || !info.CanRead || key == "TargetSite" ) { continue; }


            dictionary[key] = info.GetValue( e, null )?.ToJson();
        }
    }


    [RequiresUnreferencedCode( "Metadata for the method might be incomplete or removed" )]
    public static void Details( this Exception e, out JsonObject dict, bool includeFullMethodInfo )
    {
        dict = new JsonObject
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
