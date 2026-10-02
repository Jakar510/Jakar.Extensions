using ZLinq;



namespace Jakar.Extensions;


public static class ExceptionExtensions
{
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


    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")]
    public static void Details( this Exception e, out JsonObject dict, bool includeFullMethodInfo )
    {
        JsonArray            array = [];
        ReadOnlySpan<string> lines = e.StackTrace?.SplitAndTrimLines();
        foreach ( string line in lines ) { array.Add((JsonNode?)JsonValue.Create(line)); }

        dict = new JsonObject(Json.NodeOptions)
               {
                   [nameof(Type)]                 = e.GetType().FullName,
                   [nameof(Exception.HResult)]    = e.HResult,
                   [nameof(Exception.HelpLink)]   = e.HelpLink,
                   [nameof(Exception.Source)]     = e.Source,
                   [nameof(Exception.Message)]    = e.Message,
                   [nameof(Exception.Data)]       = e.GetData(),
                   [nameof(Exception.StackTrace)] = array
               };

        if ( includeFullMethodInfo ) { dict[nameof(Exception.TargetSite)]          = e.MethodInfo()?.ToJsonNode(); }
        else if ( e.TargetSite is not null ) { dict[nameof(Exception.TargetSite)] = $"{e.MethodClass()}::{e.MethodSignature()}"; }

        e.GetProperties(ref dict);
    }


    /// <summary> A JSON value for an arbitrary object without reflection-based serialization: primitives keep their JSON type, nodes are cloned, anything else becomes its <see cref="object.ToString"/>. </summary>
    public static JsonNode? ToJsonValue( object? value ) => value switch
                                                           {
                                                               null             => null,
                                                               JsonNode node    => node.Parent is null ? node : node.DeepClone(),
                                                               JsonElement e    => JsonValue.Create(e),
                                                               string str       => JsonValue.Create(str),
                                                               bool b           => JsonValue.Create(b),
                                                               char c           => JsonValue.Create(c),
                                                               byte n           => JsonValue.Create(n),
                                                               sbyte n          => JsonValue.Create(n),
                                                               short n          => JsonValue.Create(n),
                                                               ushort n         => JsonValue.Create(n),
                                                               int n            => JsonValue.Create(n),
                                                               uint n           => JsonValue.Create(n),
                                                               long n           => JsonValue.Create(n),
                                                               ulong n          => JsonValue.Create(n),
                                                               float n          => JsonValue.Create(n),
                                                               double n         => JsonValue.Create(n),
                                                               decimal n        => JsonValue.Create(n),
                                                               Guid g           => JsonValue.Create(g),
                                                               DateTime d       => JsonValue.Create(d),
                                                               DateTimeOffset d => JsonValue.Create(d),
                                                               Enum en          => JsonValue.Create(en.ToString()),
                                                               IFormattable f   => JsonValue.Create(f.ToString(null, CultureInfo.InvariantCulture)),
                                                               _                => JsonValue.Create(value.ToString())
                                                           };



    extension( Exception e )
    {
        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")]
        public Dictionary<string, object?> GetInnerExceptions( ref Dictionary<string, object?> dict, bool includeFullMethodInfo )
        {
            if ( e is null ) { throw new NullReferenceException(nameof(e)); }

            if ( e.InnerException is null ) { return dict; }

            e.Details(out Dictionary<string, object?> inner, includeFullMethodInfo);

            dict[nameof(e.InnerException)] = e.InnerException.GetInnerExceptions(ref inner, includeFullMethodInfo);

            return dict;
        }
        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")]
        public Dictionary<string, object?> GetProperties()
        {
            Dictionary<string, object?> dictionary = new();

            e.GetProperties(ref dictionary);

            return dictionary;
        }
        /// <summary> Trim/AOT-safe details (no <see cref="ExceptionDetails.TargetSite"/>); see <see cref="FullDetails"/>. </summary>
        public ExceptionDetails Details() => ExceptionDetails.Create(e);
        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")]
        public ExceptionDetails FullDetails() => ExceptionDetails.CreateWithMethodInfo(e);
    }



    extension( Exception self )
    {
        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public string? MethodClass()     => self.TargetSite?.MethodClass();
        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public string? MethodName()      => self.TargetSite?.MethodName();
        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public string? MethodSignature() => self.TargetSite?.MethodSignature();


        /// <summary> Trim/AOT-safe <see cref="MethodSignature"/> for diagnostic text: <see langword="null"/> when the method metadata isn't available (trimmed, or Native AOT). </summary>
        [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Diagnostic text only. Exception.TargetSite returns null when the metadata was trimmed, and any failure reading it is reported as null ('unknown').")]
        public string? TryGetMethodSignature()
        {
            try { return self.TargetSite?.MethodSignature(); }
            catch ( Exception ) { return null; }
        }


        /// <summary> <see cref="Exception.Data"/> as a JSON object (no reflection-based serialization; see <see cref="ToJsonValue"/>). </summary>
        public JsonObject GetData()
        {
            JsonObject result = new(Json.NodeOptions);
            foreach ( DictionaryEntry entry in self.Data ) { result[entry.Key.ToString() ?? EMPTY] = ToJsonValue(entry.Value); }

            return result;
        }

        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")]
        public void Details( out Dictionary<string, string?> dict )
        {
            dict = new Dictionary<string, string?>(10);
            self.Details(dict);
        }

        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")]
        public void Details<TValue>( in TValue dict )
            where TValue : class, IDictionary<string, string?>
        {
            dict[nameof(Type)] = self.GetType().FullName;

            dict[nameof(self.Source)]        = self.Source;
            dict[nameof(self.Message)]       = self.Message;
            dict[nameof(self.StackTrace)]    = self.StackTrace;
            dict[nameof(Exception.HelpLink)] = self.HelpLink;
            dict[nameof(MethodSignature)]    = self.MethodSignature();
            dict[nameof(self.ToString)]      = self.ToString();
        }


        [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")]
        public void Details( out Dictionary<string, object?> dict, bool includeFullMethodInfo )
        {
            dict = new Dictionary<string, object?>
                   {
                       [nameof(Type)]                 = self.GetType().FullName,
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


        public StringTags GetTags()
        {
            Pair type = new(nameof(Type), self.GetType().FullName);

            Pair source          = new(nameof(self.Source), self.Source);
            Pair message         = new(nameof(self.Message), self.Message);
            Pair stackTrace      = new(nameof(self.StackTrace), self.StackTrace);
            Pair methodSignature = new(nameof(MethodSignature), self.TryGetMethodSignature());

            using PooledArray<Pair> array = self.Data.AsValueEnumerable<DictionaryEntry>().Select(static pair => new Pair(pair.Key.ToString() ?? EMPTY, pair.Value?.ToString())).ToArrayPool();

            StringTags tags = new([type, message, source, stackTrace, methodSignature, ..array.Span], [self.ToString()]);
            return tags;
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


        public void GetProperties( ref JsonObject dictionary )
        {
            foreach ( PropertyInfo info in typeof(TValue).GetProperties(BindingFlags.Instance | BindingFlags.Public) )
            {
                string key = info.Name;
                if ( dictionary.ContainsKey(key) || !info.CanRead || key == "TargetSite" ) { continue; }

                dictionary[key] = ToJsonValue(info.GetValue(e, null));
            }
        }
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
