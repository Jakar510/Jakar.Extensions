namespace Jakar.Extensions.General;


public static class ExceptionExtensions
{
    public static Dictionary<string, object?> GetProperties( this Exception e )
    {
        var dictionary = new Dictionary<string, object?>();

        e.GetProperties(ref dictionary);

        return dictionary;
    }

    public static void GetProperties( this Exception e, ref Dictionary<string, object?> dictionary )
    {
        foreach ( PropertyInfo info in e.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public) )
        {
            string key = info.Name;

            if ( dictionary.ContainsKey(key) || !info.CanRead || key == "TargetSite" ) { continue; }

            dictionary[key] = info.GetValue(e, null);
        }
    }


    public static MethodDetails? MethodInfo( this      Exception e ) => e.TargetSite?.MethodInfo();
    public static string?        MethodName( this      Exception e ) => e.TargetSite?.MethodName();
    public static string?        MethodSignature( this Exception e ) => e.TargetSite?.MethodSignature();
    public static string?        MethodClass( this     Exception e ) => e.TargetSite?.MethodClass();


    public static void Details( this Exception e, out Dictionary<string, string?> dict )
    {
        dict = new Dictionary<string, string?>
               {
                   [nameof(Type)]               = e.GetType().FullName,
                   [nameof(e.Source)]           = e.Source,
                   [nameof(e.Message)]          = e.Message,
                   [nameof(e.StackTrace)]       = e.StackTrace,
                   [nameof(Exception.HelpLink)] = e.HelpLink,
                   [nameof(MethodSignature)]    = e.MethodSignature(),
                   [nameof(e.ToString)]         = e.ToString()
               };
    }

    public static void Details( this Exception e, out Dictionary<string, object?> dict, bool includeFullMethodInfo = false )
    {
        dict = new Dictionary<string, object?>
               {
                   [nameof(Type)]                 = e.GetType().FullName,
                   [nameof(Exception.HResult)]    = e.HResult,
                   [nameof(Exception.HelpLink)]   = e.HelpLink,
                   [nameof(Exception.Source)]     = e.Source,
                   [nameof(Exception.Message)]    = e.Message,
                   [nameof(Exception.Data)]       = e.GetData(),
                   [nameof(Exception.StackTrace)] = e.StackTrace?.SplitAndTrimLines(),
               };


        if ( includeFullMethodInfo ) { dict[nameof(Exception.TargetSite)]         = e.MethodInfo(); }
        else if ( e.TargetSite is not null ) { dict[nameof(Exception.TargetSite)] = $"{e.MethodClass()}::{e.MethodSignature()}"; }

        e.GetProperties(ref dict);
    }


    public static Dictionary<string, JToken?> GetData( this Exception e )
    {
        var data = new Dictionary<string, JToken?>();

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach ( DictionaryEntry pair in e.Data )
        {
            if ( pair.Key is null ) { continue; }


            data[pair.Key.ToString()] = pair.Value is null
                                            ? null
                                            : JToken.FromObject(pair.Value);
        }

        return data;
    }


    public static ExceptionDetails Details( this     Exception e ) => new(e);
    public static ExceptionDetails FullDetails( this Exception e ) => new(e, true);


    [Obsolete($"Use {nameof(ExceptionDetails)} instead")]
    public static Dictionary<string, object?> FullDetails( this Exception e, bool includeFullMethodInfo = false )
    {
        if ( e is null ) throw new ArgumentNullException(nameof(e));

        e.Details(out Dictionary<string, object?> dict, includeFullMethodInfo);

        dict[nameof(e.InnerException)] = e.GetInnerExceptions(includeFullMethodInfo);

        return dict;
    }

    private static Dictionary<string, object?>? GetInnerExceptions( this Exception e, bool includeFullMethodInfo )
    {
        if ( e.InnerException is null ) { return null; }

        var innerDetails = new Dictionary<string, object?>();

        return e.InnerException.GetInnerExceptions(ref innerDetails, includeFullMethodInfo);
    }

    private static Dictionary<string, object?> GetInnerExceptions( this Exception e, ref Dictionary<string, object?> dict, bool includeFullMethodInfo )
    {
        if ( e is null ) { throw new NullReferenceException(nameof(e)); }

        if ( e.InnerException is null ) { return dict; }

        e.Details(out Dictionary<string, object?> inner, includeFullMethodInfo);

        dict[nameof(e.InnerException)] = e.InnerException.GetInnerExceptions(ref inner, includeFullMethodInfo);

        return dict;
    }
}
