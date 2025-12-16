using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using ZLinq;



namespace Jakar.Extensions;


public static class ExceptionExtensions
{
    extension( Exception e )
    {
        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
        public Dictionary<string, object?> GetInnerExceptions( ref Dictionary<string, object?> dict, bool includeFullMethodInfo )
        {
            if ( e is null ) { throw new NullReferenceException(nameof(e)); }

            if ( e.InnerException is null ) { return dict; }

            e.Details(out Dictionary<string, object?> inner, includeFullMethodInfo);

            dict[nameof(e.InnerException)] = e.InnerException.GetInnerExceptions(ref inner, includeFullMethodInfo);

            return dict;
        }
        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
        public Dictionary<string, object?> GetProperties()
        {
            Dictionary<string, object?> dictionary = new();

            e.GetProperties(ref dictionary);

            return dictionary;
        }
        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
        public ExceptionDetails Details() => new(e, false);
        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
        public ExceptionDetails FullDetails() => new(e);
    }



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



    extension( Exception self )
    {
        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public string? MethodClass()     => self.TargetSite?.MethodClass();
        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public string? MethodName()      => self.TargetSite?.MethodName();
        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public string? MethodSignature() => self.TargetSite?.MethodSignature();


        [RequiresUnreferencedCode(SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
        public JToken GetData() => self.Data.ToJson();

        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
        public void Details( out Dictionary<string, string?> dict )
        {
            dict = new Dictionary<string, string?>(10);
            self.Details(dict);
        }

        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
        public void Details<TValue>( in TValue dict )
            where TValue : class, IDictionary<string, string?>
        {
            dict[nameof(Type)] = self.GetType()
                                     .FullName;

            dict[nameof(self.Source)]        = self.Source;
            dict[nameof(self.Message)]       = self.Message;
            dict[nameof(self.StackTrace)]    = self.StackTrace;
            dict[nameof(Exception.HelpLink)] = self.HelpLink;
            dict[nameof(MethodSignature)]    = self.MethodSignature();
            dict[nameof(self.ToString)]      = self.ToString();
        }


        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
        public StringTags GetTags()
        {
            Pair type = new(nameof(Type),
                            self.GetType()
                                .FullName);

            Pair source          = new(nameof(self.Source), self.Source);
            Pair message         = new(nameof(self.Message), self.Message);
            Pair stackTrace      = new(nameof(self.StackTrace), self.StackTrace);
            Pair methodSignature = new(nameof(MethodSignature), self.MethodSignature());

            using PooledArray<Pair> array = self.Data.AsValueEnumerable<DictionaryEntry>()
                                                .Select(static pair => new Pair(pair.Key.ToString() ?? EMPTY, pair.Value?.ToString()))
                                                .ToArrayPool();

            StringTags tags = new([type, message, source, stackTrace, methodSignature, ..array.Span], [self.ToString()]);

            return tags;
        }

        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
        public void Details( out Dictionary<string, object?> dict, bool includeFullMethodInfo )
        {
            dict = new Dictionary<string, object?>
                   {
                       [nameof(Type)] = self.GetType()
                                            .FullName,
                       [nameof(Exception.HResult)]    = self.HResult,
                       [nameof(Exception.HelpLink)]   = self.HelpLink,
                       [nameof(Exception.Source)]     = self.Source,
                       [nameof(Exception.Message)]    = self.Message,
                       [nameof(Exception.Data)]       = self.GetData(),
                       [nameof(Exception.StackTrace)] = self.StackTrace?.SplitAndTrimLines()
                   };


            if ( includeFullMethodInfo ) { dict[nameof(Exception.TargetSite)]            = self.MethodInfo(); }
            else if ( self.TargetSite is not null ) { dict[nameof(Exception.TargetSite)] = $"{self.MethodClass()}::{self.MethodSignature()}"; }

            self.GetProperties(ref dict);
        }
    }



    extension<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TValue>( TValue e )
        where TValue : Exception
    {
        public void GetProperties( ref Dictionary<string, object?> dictionary )
        {
            foreach ( PropertyInfo info in typeof(TValue).GetProperties(BindingFlags.Instance | BindingFlags.Public) )
            {
                string key = info.Name;

                if ( dictionary.ContainsKey(key) || !info.CanRead || key == "TargetSite" ) { continue; }

                dictionary[key] = info.GetValue(e, null);
            }
        }
        [RequiresUnreferencedCode(SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
        public void GetProperties( ref JObject dictionary )
        {
            foreach ( PropertyInfo info in typeof(TValue).GetProperties(BindingFlags.Instance | BindingFlags.Public) )
            {
                string key = info.Name;
                if ( dictionary.ContainsKey(key) || !info.CanRead || key == "TargetSite" ) { continue; }

                object? value = info.GetValue(e, null);
                dictionary[key] = value.ToToken();
            }
        }
    }



    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + SERIALIZATION_UNREFERENCED_CODE)] [RequiresDynamicCode(SERIALIZATION_REQUIRES_DYNAMIC_CODE)]
    public static void Details( this Exception e, out JObject dict, bool includeFullMethodInfo )
    {
        JArray               array = [];
        ReadOnlySpan<string> lines = e.StackTrace?.SplitAndTrimLines();

        foreach ( string line in lines )
        {
            JToken node = line;
            array.Add(node);
        }

        dict = new JObject
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
            dict[nameof(Exception.TargetSite)] = info?.ToToken();
        }
        else if ( e.TargetSite is not null ) { dict[nameof(Exception.TargetSite)] = $"{e.MethodClass()}::{e.MethodSignature()}"; }

        e.GetProperties(ref dict);
    }


/*
    public static void GetProperties<[ DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicProperties ) ] TValue>( this TValue e, ref JObject dictionary )
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
    public static void Details( this Exception e, out JObject dict, bool includeFullMethodInfo )
    {
        dict = new JObject
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
