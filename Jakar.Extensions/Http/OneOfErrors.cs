// Jakar.Extensions :: Jakar.Extensions
// 05/23/2025  16:34

namespace Jakar.Extensions;


[method: JsonConstructor]
public readonly struct OneOfErrors( JToken? json, string? text, Errors? errors ) : IParsable<OneOfErrors>, IJsonModel<OneOfErrors>
{
    public static readonly OneOfErrors Empty  = new(null, null, null);
    public readonly        Errors?     Errors = errors;
    public readonly        JToken?     Json   = json;
    public readonly        string?     Text   = text;

    public bool IsErrors { [MemberNotNullWhen(true, nameof(Errors))] get => Errors is not null; }
    public bool IsJson   { [MemberNotNullWhen(true, nameof(Json))] get => Json is not null; }
    public bool IsText   { [MemberNotNullWhen(true, nameof(Text))] get => Text is not null; }


    public static implicit operator OneOfErrors( JToken input ) => From(input);

    public static implicit operator OneOfErrors( string input ) => From(input);

    public static implicit operator OneOfErrors( Errors    input ) => From(input);
    public static implicit operator OneOfErrors( Exception input ) => Extensions.Errors.Create(input);


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


    public TValue? Match<TValue>( Func<JToken, TValue>? f0, Func<string, TValue>? f1, Func<Errors, TValue>? f2 )
    {
        if ( IsJson && f0 is not null ) { return f0(Json); }

        if ( IsText && f1 is not null ) { return f1(Text); }

        if ( IsErrors && f2 is not null ) { return f2(Errors); }

        return default;
    }
    public TValue? Match<TArg, TValue>( TArg arg, Func<TArg, JToken, TValue>? f0, Func<TArg, string, TValue>? f1, Func<TArg, Errors, TValue>? f2 )
    {
        if ( IsJson && f0 is not null ) { return f0(arg, Json); }

        if ( IsText && f1 is not null ) { return f1(arg, Text); }

        if ( IsErrors && f2 is not null ) { return f2(arg, Errors); }

        return default;
    }


    public async ValueTask<TValue?> MatchAsync<TValue>( Func<JToken, ValueTask<TValue>>? f0, Func<string, ValueTask<TValue>>? f1, Func<Errors, ValueTask<TValue>>? f2 )
    {
        if ( IsJson && f0 is not null ) { return await f0(Json).ConfigureAwait(false); }

        if ( IsText && f1 is not null ) { return await f1(Text).ConfigureAwait(false); }

        if ( IsErrors && f2 is not null ) { return await f2(Errors).ConfigureAwait(false); }

        return default;
    }
    public async ValueTask<TValue?> MatchAsync<TValue>( Func<JToken, CancellationToken, ValueTask<TValue>>? f0, Func<string, CancellationToken, ValueTask<TValue>>? f1, Func<Errors, CancellationToken, ValueTask<TValue>>? f2, CancellationToken token )
    {
        if ( IsJson && f0 is not null ) { return await f0(Json, token).ConfigureAwait(false); }

        if ( IsText && f1 is not null ) { return await f1(Text, token).ConfigureAwait(false); }

        if ( IsErrors && f2 is not null ) { return await f2(Errors, token).ConfigureAwait(false); }

        return default;
    }


    public async ValueTask<TValue?> MatchAsync<TArg, TValue>( TArg arg, Func<TArg, JToken, ValueTask<TValue>>? f0, Func<TArg, string, ValueTask<TValue>>? f1, Func<TArg, Errors, ValueTask<TValue>>? f2 )
    {
        if ( IsJson && f0 is not null ) { return await f0(arg, Json).ConfigureAwait(false); }

        if ( IsText && f1 is not null ) { return await f1(arg, Text).ConfigureAwait(false); }

        if ( IsErrors && f2 is not null ) { return await f2(arg, Errors).ConfigureAwait(false); }

        return default;
    }
    public async ValueTask<TValue?> MatchAsync<TArg, TValue>( TArg arg, Func<TArg, JToken, CancellationToken, ValueTask<TValue>>? f0, Func<TArg, string, CancellationToken, ValueTask<TValue>>? f1, Func<TArg, Errors, CancellationToken, ValueTask<TValue>>? f2, CancellationToken token )
    {
        if ( IsJson && f0 is not null ) { return await f0(arg, Json, token).ConfigureAwait(false); }

        if ( IsText && f1 is not null ) { return await f1(arg, Text, token).ConfigureAwait(false); }

        if ( IsErrors && f2 is not null ) { return await f2(arg, Errors, token).ConfigureAwait(false); }

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

    public static bool TryFromJson( string? json, out OneOfErrors result )
    {
        result = default;
        return false;
    }
    public static OneOfErrors FromJson( string json ) => json.FromJson<OneOfErrors>();


    public int CompareTo( OneOfErrors other )
    {
        int errorsComparison = Comparer<Errors?>.Default.Compare(Errors, other.Errors);
        if ( errorsComparison != 0 ) { return errorsComparison; }

        return string.Compare(Text, other.Text, StringComparison.InvariantCultureIgnoreCase);
    }
    public          bool Equals( OneOfErrors other ) => Equals(Errors, other.Errors) && Equals(Json, other.Json) && string.Equals(Text, other.Text, StringComparison.InvariantCultureIgnoreCase);
    public override bool Equals( object?     other ) => other is OneOfErrors x       && Equals(x);
    public override int  GetHashCode()               => HashCode.Combine(Errors, Json, Text);
    public int CompareTo( object? other ) => other is OneOfErrors x
                                                 ? CompareTo(x)
                                                 : throw new ExpectedValueTypeException(other, typeof(OneOfErrors));


    public static bool operator ==( OneOfErrors? left, OneOfErrors? right ) => Nullable.Equals(left, right);
    public static bool operator !=( OneOfErrors? left, OneOfErrors? right ) => !Nullable.Equals(left, right);
    public static bool operator ==( OneOfErrors  left, OneOfErrors  right ) => EqualityComparer<OneOfErrors>.Default.Equals(left, right);
    public static bool operator !=( OneOfErrors  left, OneOfErrors  right ) => !EqualityComparer<OneOfErrors>.Default.Equals(left, right);
    public static bool operator >( OneOfErrors   left, OneOfErrors  right ) => Comparer<OneOfErrors>.Default.Compare(left, right) > 0;
    public static bool operator >=( OneOfErrors  left, OneOfErrors  right ) => Comparer<OneOfErrors>.Default.Compare(left, right) >= 0;
    public static bool operator <( OneOfErrors   left, OneOfErrors  right ) => Comparer<OneOfErrors>.Default.Compare(left, right) < 0;
    public static bool operator <=( OneOfErrors  left, OneOfErrors  right ) => Comparer<OneOfErrors>.Default.Compare(left, right) <= 0;
}
