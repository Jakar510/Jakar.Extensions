// Jakar.Extensions :: Jakar.Extensions
// 05/23/2025  16:34

using ZXing.Aztec.Internal;



namespace Jakar.Extensions;


[method: JsonConstructor]
public readonly struct OneOfErrors( JToken? json, string? text, Errors? errors, StringTags tags ) : IParsable<OneOfErrors>, IJsonModel<OneOfErrors>, IEquatable<OneOfErrors>
{
    public static readonly         OneOfErrors Empty  = new(null, null, null, StringTags.Empty);
    [JsonRequired] public readonly Errors?     Errors = errors;
    [JsonRequired] public readonly JToken?     Json   = json;
    [JsonRequired] public readonly string?     Text   = text;
    [JsonRequired] public readonly StringTags  Tags   = tags;


    public bool IsErrors { [MemberNotNullWhen(true, nameof(Errors))] get => Errors is not null; }
    public bool IsJson   { [MemberNotNullWhen(true, nameof(Json))] get => Json is not null; }
    public bool IsText   { [MemberNotNullWhen(true, nameof(Text))] get => Text is not null; }
    public bool IsTags   { [MemberNotNullWhen(true, nameof(Tags))] get => !Tags.IsEmpty; }


    public static implicit operator OneOfErrors( JToken     input ) => From(input);
    public static implicit operator OneOfErrors( string     input ) => From(input);
    public static implicit operator OneOfErrors( Errors     input ) => From(input);
    public static implicit operator OneOfErrors( StringTags input ) => From(input);
    public static implicit operator OneOfErrors( Exception  input ) => Extensions.Errors.Create(input);


    public static OneOfErrors From( JToken     input ) => new(input, null, null, StringTags.Empty);
    public static OneOfErrors From( string     input ) => new(null, input, null, StringTags.Empty);
    public static OneOfErrors From( Errors     input ) => new(null, null, input, StringTags.Empty);
    public static OneOfErrors From( StringTags input ) => new(null, null, null, input);


    public void Switch( Action<JToken>? f0, Action<string>? f1, Action<Errors>? f2, Action<StringTags>? f3 )
    {
        if ( IsJson        && f0 is not null ) { f0(Json); }
        else if ( IsText   && f1 is not null ) { f1(Text); }
        else if ( IsErrors && f2 is not null ) { f2(Errors); }
        else if ( IsTags   && f3 is not null ) { f3(Tags); }
        else { throw new InvalidOperationException(); }
    }


    public TValue? Match<TValue>( Func<JToken, TValue>? f0, Func<string, TValue>? f1, Func<Errors, TValue>? f2, Func<StringTags, TValue>? f3 )
    {
        if ( IsJson && f0 is not null ) { return f0(Json); }

        if ( IsText && f1 is not null ) { return f1(Text); }

        if ( IsErrors && f2 is not null ) { return f2(Errors); }

        if ( IsTags && f3 is not null ) { return f3(Tags); }

        return default;
    }
    public TValue? Match<TArg, TValue>( TArg arg, Func<TArg, JToken, TValue>? f0, Func<TArg, string, TValue>? f1, Func<TArg, Errors, TValue>? f2, Func<TArg, StringTags, TValue>? f3 )
    {
        if ( IsJson && f0 is not null ) { return f0(arg, Json); }

        if ( IsText && f1 is not null ) { return f1(arg, Text); }

        if ( IsErrors && f2 is not null ) { return f2(arg, Errors); }

        if ( IsTags && f3 is not null ) { return f3(arg, Tags); }

        return default;
    }


    public async ValueTask<TValue?> MatchAsync<TValue>( Func<JToken, ValueTask<TValue>>? f0, Func<string, ValueTask<TValue>>? f1, Func<Errors, ValueTask<TValue>>? f2, Func<StringTags, ValueTask<TValue>>? f3 )
    {
        if ( IsJson && f0 is not null )
        {
            return await f0(Json)
                      .ConfigureAwait(false);
        }

        if ( IsText && f1 is not null )
        {
            return await f1(Text)
                      .ConfigureAwait(false);
        }

        if ( IsErrors && f2 is not null )
        {
            return await f2(Errors)
                      .ConfigureAwait(false);
        }

        if ( IsTags && f3 is not null )
        {
            return await f3(Tags)
                      .ConfigureAwait(false);
        }

        return default;
    }
    public async ValueTask<TValue?> MatchAsync<TValue>( Func<JToken, CancellationToken, ValueTask<TValue>>? f0, Func<string, CancellationToken, ValueTask<TValue>>? f1, Func<Errors, CancellationToken, ValueTask<TValue>>? f2, Func<StringTags, CancellationToken, ValueTask<TValue>>? f3, CancellationToken token )
    {
        if ( IsJson && f0 is not null )
        {
            return await f0(Json, token)
                      .ConfigureAwait(false);
        }

        if ( IsText && f1 is not null )
        {
            return await f1(Text, token)
                      .ConfigureAwait(false);
        }

        if ( IsErrors && f2 is not null )
        {
            return await f2(Errors, token)
                      .ConfigureAwait(false);
        }

        if ( IsTags && f3 is not null )
        {
            return await f3(Tags, token)
                      .ConfigureAwait(false);
        }

        return default;
    }


    public async ValueTask<TValue?> MatchAsync<TArg, TValue>( TArg arg, Func<TArg, JToken, ValueTask<TValue>>? f0, Func<TArg, string, ValueTask<TValue>>? f1, Func<TArg, Errors, ValueTask<TValue>>? f2, Func<TArg, StringTags, ValueTask<TValue>>? f3 )
    {
        if ( IsJson && f0 is not null )
        {
            return await f0(arg, Json)
                      .ConfigureAwait(false);
        }

        if ( IsText && f1 is not null )
        {
            return await f1(arg, Text)
                      .ConfigureAwait(false);
        }

        if ( IsErrors && f2 is not null )
        {
            return await f2(arg, Errors)
                      .ConfigureAwait(false);
        }

        if ( IsTags && f3 is not null )
        {
            return await f3(arg, Tags)
                      .ConfigureAwait(false);
        }

        return default;
    }
    public async ValueTask<TValue?> MatchAsync<TArg, TValue>( TArg arg, Func<TArg, JToken, CancellationToken, ValueTask<TValue>>? f0, Func<TArg, string, CancellationToken, ValueTask<TValue>>? f1, Func<TArg, Errors, CancellationToken, ValueTask<TValue>>? f2, Func<TArg, StringTags, CancellationToken, ValueTask<TValue>>? f3, CancellationToken token )
    {
        if ( IsJson && f0 is not null )
        {
            return await f0(arg, Json, token)
                      .ConfigureAwait(false);
        }

        if ( IsText && f1 is not null )
        {
            return await f1(arg, Text, token)
                      .ConfigureAwait(false);
        }

        if ( IsErrors && f2 is not null )
        {
            return await f2(arg, Errors, token)
                      .ConfigureAwait(false);
        }

        if ( IsTags && f3 is not null )
        {
            return await f3(arg, Tags, token)
                      .ConfigureAwait(false);
        }

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
        result = Parse(json, CultureInfo.InvariantCulture);
        return true;
    }
    public static OneOfErrors FromJson( string json ) => json.FromJson<OneOfErrors>();


    public int CompareTo( OneOfErrors other )
    {
        int errorsComparison = Comparer<Errors?>.Default.Compare(Errors, other.Errors);
        if ( errorsComparison != 0 ) { return errorsComparison; }

        return string.Compare(Text, other.Text, StringComparison.InvariantCultureIgnoreCase);
    }
    public          bool Equals( OneOfErrors other ) => Equals(Errors, other.Errors) && Equals(Json, other.Json) && string.Equals(Text, other.Text, StringComparison.InvariantCulture);
    public override bool Equals( object?     other ) => other is OneOfErrors x       && Equals(x);
    public override int GetHashCode()
    {
        HashCode hashCode = new HashCode();
        hashCode.Add(Errors);
        hashCode.Add(Json);
        hashCode.Add(Text, StringComparer.InvariantCulture);
        return hashCode.ToHashCode();
    }
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
