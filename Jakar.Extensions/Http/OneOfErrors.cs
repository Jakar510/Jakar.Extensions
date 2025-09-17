// Jakar.Extensions :: Jakar.Extensions
// 05/23/2025  16:34

namespace Jakar.Extensions;


public readonly record struct OneOfErrors( JToken? Json, string? Text, Errors? Errors ) : IParsable<OneOfErrors>
{
    public const           string      ERROR_MESSAGE = "Error Message: ";
    public const           string      UNKNOWN_ERROR = "Unknown Error";
    public static readonly OneOfErrors Empty         = new(null, null, null);
    public readonly        Errors?     Errors        = Errors;
    public readonly        JToken?     Json          = Json;
    public readonly        string?     Text          = Text;
    public                 bool        IsErrors { [MemberNotNullWhen(true, nameof(Errors))] get => Errors is not null; }


    public bool IsJson { [MemberNotNullWhen(true, nameof(Json))] get => Json is not null; }
    public bool IsText { [MemberNotNullWhen(true, nameof(Text))] get => Text is not null; }


    public static implicit operator OneOfErrors( JToken input ) => From(input);

    public static implicit operator OneOfErrors( string input ) => From(input);

    public static implicit operator OneOfErrors( Errors input ) => From(input);


    public static OneOfErrors From( JToken input ) => new(input, null, null);
    public static OneOfErrors From( string input ) => new(null, input, null);
    public static OneOfErrors From( Errors input ) => new(null, null, input);


    public void Switch( Action<JToken>? f0, Action<string>? f1, Action<Errors>? f2 )
    {
        if ( IsJson        && f0 is not null ) { f0(Json); }
        else if ( IsText   && f1 is not null ) { f1(Text); }
        else if ( IsErrors && f2 is not null ) { f2(Errors); }
        else { throw new InvalidOperationException(); }
    }


    public TResult Match<TResult>( Func<JToken, TResult>? f0, Func<string, TResult>? f1, Func<Errors, TResult>? f2 )
    {
        if ( IsJson && f0 is not null ) { return f0(Json); }

        if ( IsText && f1 is not null ) { return f1(Text); }

        if ( IsErrors && f2 is not null ) { return f2(Errors); }

        throw new InvalidOperationException();
    }
    public async ValueTask<TResult> MatchAsync<TResult>( Func<JToken, ValueTask<TResult>>? f0, Func<string, ValueTask<TResult>>? f1, Func<Errors, ValueTask<TResult>>? f2 )
    {
        if ( IsJson && f0 is not null ) { return await f0(Json); }

        if ( IsText && f1 is not null ) { return await f1(Text); }

        if ( IsErrors && f2 is not null ) { return await f2(Errors); }

        throw new InvalidOperationException();
    }
    public async ValueTask<TResult> MatchAsync<TResult>( Func<JToken, CancellationToken, ValueTask<TResult>>? f0, Func<string, CancellationToken, ValueTask<TResult>>? f1, Func<Errors, CancellationToken, ValueTask<TResult>>? f2, CancellationToken token )
    {
        if ( IsJson && f0 is not null ) { return await f0(Json, token); }

        if ( IsText && f1 is not null ) { return await f1(Text, token); }

        if ( IsErrors && f2 is not null ) { return await f2(Errors, token); }

        throw new InvalidOperationException();
    }

    
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)]
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
}
