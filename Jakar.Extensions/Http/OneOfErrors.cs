// Jakar.Extensions :: Jakar.Extensions
// 05/23/2025  16:34

namespace Jakar.Extensions;


[method: JsonConstructor]
public readonly record struct OneOfErrors( JsonNode? Json, string? Text, Errors? Errors ) : IParsable<OneOfErrors>, IJsonModel<OneOfErrors>
{
    public const           string      ERROR_MESSAGE = "Error Message: ";
    public const           string      UNKNOWN_ERROR = "Unknown Error";
    public static readonly OneOfErrors Empty         = new(null, null, null);
    public readonly        Errors?     Errors        = Errors;
    public readonly        JsonNode?   Json          = Json;
    public readonly        string?     Text          = Text;
    public                 bool        IsErrors { [MemberNotNullWhen(true, nameof(Errors))] get => Errors is not null; }


    public bool IsJson { [MemberNotNullWhen(true, nameof(Json))] get => Json is not null; }
    public bool IsText { [MemberNotNullWhen(true, nameof(Text))] get => Text is not null; }


    public static implicit operator OneOfErrors( JsonNode input ) => From(input);

    public static implicit operator OneOfErrors( string input ) => From(input);

    public static implicit operator OneOfErrors( Errors input ) => From(input);


    public static OneOfErrors From( JsonNode input ) => new(input, null, null);
    public static OneOfErrors From( string   input ) => new(null, input, null);
    public static OneOfErrors From( Errors   input ) => new(null, null, input);


    public void Switch( Action<JsonNode>? f0, Action<string>? f1, Action<Errors>? f2 )
    {
        if ( IsJson        && f0 is not null ) { f0(Json); }
        else if ( IsText   && f1 is not null ) { f1(Text); }
        else if ( IsErrors && f2 is not null ) { f2(Errors); }
        else { throw new InvalidOperationException(); }
    }


    public TValue? Match<TValue>( Func<JsonNode, TValue>? f0, Func<string, TValue>? f1, Func<Errors, TValue>? f2 )
    {
        if ( IsJson && f0 is not null ) { return f0(Json); }

        if ( IsText && f1 is not null ) { return f1(Text); }

        if ( IsErrors && f2 is not null ) { return f2(Errors); }

        return default;
    }
    public async ValueTask<TValue?> MatchAsync<TValue>( Func<JsonNode, ValueTask<TValue>>? f0, Func<string, ValueTask<TValue>>? f1, Func<Errors, ValueTask<TValue>>? f2 )
    {
        if ( IsJson && f0 is not null ) { return await f0(Json); }

        if ( IsText && f1 is not null ) { return await f1(Text); }

        if ( IsErrors && f2 is not null ) { return await f2(Errors); }

        return default;
    }
    public async ValueTask<TValue?> MatchAsync<TValue>( Func<JsonNode, CancellationToken, ValueTask<TValue>>? f0, Func<string, CancellationToken, ValueTask<TValue>>? f1, Func<Errors, CancellationToken, ValueTask<TValue>>? f2, CancellationToken token )
    {
        if ( IsJson && f0 is not null ) { return await f0(Json, token); }

        if ( IsText && f1 is not null ) { return await f1(Text, token); }

        if ( IsErrors && f2 is not null ) { return await f2(Errors, token); }

        return default;
    }


    public static OneOfErrors Parse( string? error, IFormatProvider? provider = null )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        if ( string.IsNullOrWhiteSpace(error) ) { return UNKNOWN_ERROR; }

        const string ESCAPED_QUOTE = @"\""";
        const string QUOTE         = @"""";
        if ( error.Contains(ESCAPED_QUOTE) ) { error = error.Replace(ESCAPED_QUOTE, QUOTE); }

        try { return error.FromJson<Errors>(); }
        catch ( Exception e )
        {
            telemetrySpan.AddException(e);

            try { return error.FromJson(); }
            catch ( Exception ) { return error; }
        }
    }
    public static bool TryParse( [NotNullWhen(true)] string? error, IFormatProvider? provider, out OneOfErrors result )
    {
        result = Parse(error, provider);
        return true;
    }
    public static JsonSerializerContext       JsonContext   => JakarExtensionsContext.Default;
    public static JsonTypeInfo<OneOfErrors>   JsonTypeInfo  => JakarExtensionsContext.Default.OneOfErrors;
    public static JsonTypeInfo<OneOfErrors[]> JsonArrayInfo => JakarExtensionsContext.Default.OneOfErrorsArray;
    public        JsonNode                    ToJsonNode()  => Validate.ThrowIfNull(this.ToJsonNode<OneOfErrors>());
    public        string                      ToJson()      => this.ToJson(JsonTypeInfo);
    public static bool TryFromJson( string? json, out OneOfErrors result )
    {
        result = default;
        return false;
    }
    public static OneOfErrors FromJson( string json ) => json.FromJson(JsonTypeInfo);
}
