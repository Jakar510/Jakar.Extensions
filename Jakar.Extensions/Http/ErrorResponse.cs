// Jakar.Extensions :: Jakar.Extensions
// 05/23/2025  16:34

using ZXing.Aztec.Internal;



namespace Jakar.Extensions;


[method: JsonConstructor]
public readonly struct ErrorResponse( string? text ) : IParsable<ErrorResponse>, IJsonModel<ErrorResponse>, IEquatable<ErrorResponse>
{
    public static readonly         ErrorResponse Empty        = new(null);
    public static readonly         ErrorResponse UnknownError = new(UNKNOWN_ERROR);
    [JsonRequired] public readonly string?       Text         = text;


    public bool HasValue { [MemberNotNullWhen(true, nameof(Text))] get => !string.IsNullOrWhiteSpace(Text); }


    public static implicit operator ErrorResponse( JToken     input ) => From(input);
    public static implicit operator ErrorResponse( string     input ) => From(input);
    public static implicit operator ErrorResponse( Errors     input ) => From(input);
    public static implicit operator ErrorResponse( StringTags input ) => From(input);
    public static implicit operator ErrorResponse( Exception  input ) => Extensions.Errors.Create(input);


    public static ErrorResponse From( string     input ) => new(input);
    public static ErrorResponse From( Errors     input ) => new(input.ToJson());
    public static ErrorResponse From( StringTags input ) => new(input.ToJson());
    public static ErrorResponse From( JToken     input ) => new(input.ToString());


    public StringTags? AsTags() => Text?.TryFromJson<StringTags>();
    public bool AsTags( [NotNullWhen(true)] out StringTags? token )
    {
        token = AsTags();
        return token is { IsEmpty: false };
    }
    public Errors? AsErrors() => Text?.TryFromJson<Errors>();
    public bool AsErrors( [NotNullWhen(true)] out Errors? token )
    {
        token = AsErrors();
        return token is not null;
    }
    public JToken? AsJson() => Text?.TryFromJson();
    public bool AsJson( [NotNullWhen(true)] out JToken? token )
    {
        token = AsJson();
        return token is not null;
    }
    public TValue? AsJson<TValue>() => Text.TryFromJson<TValue>();
    public bool AsErrors<TValue>( [NotNullWhen(true)] out TValue? token )
    {
        token = AsJson<TValue>();
        return token is not null;
    }


    public void Switch( Action<JToken>? f0, Action<string>? f1, Action<Errors>? f2, Action<StringTags>? f3 )
    {
        if ( !HasValue ) { return; }

        if ( AsErrors(out Errors? errors)      && f2 is not null ) { f2(errors); }
        else if ( AsTags(out StringTags? tags) && f3 is not null ) { f3(tags.Value); }
        else if ( AsJson(out JToken? jToken)   && f0 is not null ) { f0(jToken); }
        else if ( f1 is not null ) { f1(Text); }
    }


    public TValue? Match<TValue>( Func<JToken, TValue>? f0, Func<string, TValue>? f1, Func<Errors, TValue>? f2, Func<StringTags, TValue>? f3 )
    {
        if ( !HasValue ) { return default; }

        if ( AsErrors(out Errors? errors) && f2 is not null ) { return f2(errors); }

        if ( AsTags(out StringTags? tags) && f3 is not null ) { return f3(tags.Value); }

        if ( AsJson(out JToken? jToken) && f0 is not null ) { return f0(jToken); }

        return f1 is not null
                   ? f1(Text)
                   : default;
    }
    public TValue? Match<TArg, TValue>( TArg arg, Func<TArg, JToken, TValue>? f0, Func<TArg, string, TValue>? f1, Func<TArg, Errors, TValue>? f2, Func<TArg, StringTags, TValue>? f3 )
    {
        if ( !HasValue ) { return default; }

        if ( AsErrors(out Errors? errors) && f2 is not null ) { return f2(arg, errors); }

        if ( AsTags(out StringTags? tags) && f3 is not null ) { return f3(arg, tags.Value); }

        if ( AsJson(out JToken? jToken) && f0 is not null ) { return f0(arg, jToken); }

        return f1 is not null
                   ? f1(arg, Text)
                   : default;
    }


    public async ValueTask<TValue?> MatchAsync<TValue>( Func<JToken, ValueTask<TValue>>? f0, Func<string, ValueTask<TValue>>? f1, Func<Errors, ValueTask<TValue>>? f2, Func<StringTags, ValueTask<TValue>>? f3 )
    {
        if ( !HasValue ) { return default; }

        if ( AsErrors(out Errors? errors) && f2 is not null )
        {
            return await f2(errors)
                      .ConfigureAwait(false);
        }

        if ( AsTags(out StringTags? tags) && f3 is not null )
        {
            return await f3(tags.Value)
                      .ConfigureAwait(false);
        }

        if ( AsJson(out JToken? jToken) && f0 is not null )
        {
            return await f0(jToken)
                      .ConfigureAwait(false);
        }

        return f1 is not null
                   ? await f1(Text)
                        .ConfigureAwait(false)
                   : default;
    }
    public async ValueTask<TValue?> MatchAsync<TValue>( Func<JToken, CancellationToken, ValueTask<TValue>>? f0, Func<string, CancellationToken, ValueTask<TValue>>? f1, Func<Errors, CancellationToken, ValueTask<TValue>>? f2, Func<StringTags, CancellationToken, ValueTask<TValue>>? f3, CancellationToken token )
    {
        if ( !HasValue ) { return default; }

        if ( AsErrors(out Errors? errors) && f2 is not null )
        {
            return await f2(errors, token)
                      .ConfigureAwait(false);
        }

        if ( AsTags(out StringTags? tags) && f3 is not null )
        {
            return await f3(tags.Value, token)
                      .ConfigureAwait(false);
        }

        if ( AsJson(out JToken? jToken) && f0 is not null )
        {
            return await f0(jToken, token)
                      .ConfigureAwait(false);
        }

        return f1 is not null
                   ? await f1(Text, token)
                        .ConfigureAwait(false)
                   : default;
    }


    public async ValueTask<TValue?> MatchAsync<TArg, TValue>( TArg arg, Func<TArg, JToken, ValueTask<TValue>>? f0, Func<TArg, string, ValueTask<TValue>>? f1, Func<TArg, Errors, ValueTask<TValue>>? f2, Func<TArg, StringTags, ValueTask<TValue>>? f3 )
    {
        if ( !HasValue ) { return default; }

        if ( AsErrors(out Errors? errors) && f2 is not null )
        {
            return await f2(arg, errors)
                      .ConfigureAwait(false);
        }

        if ( AsTags(out StringTags? tags) && f3 is not null )
        {
            return await f3(arg, tags.Value)
                      .ConfigureAwait(false);
        }

        if ( AsJson(out JToken? jToken) && f0 is not null )
        {
            return await f0(arg, jToken)
                      .ConfigureAwait(false);
        }

        return f1 is not null
                   ? await f1(arg, Text)
                        .ConfigureAwait(false)
                   : default;
    }
    public async ValueTask<TValue?> MatchAsync<TArg, TValue>( TArg arg, Func<TArg, JToken, CancellationToken, ValueTask<TValue>>? f0, Func<TArg, string, CancellationToken, ValueTask<TValue>>? f1, Func<TArg, Errors, CancellationToken, ValueTask<TValue>>? f2, Func<TArg, StringTags, CancellationToken, ValueTask<TValue>>? f3, CancellationToken token )
    {
        if ( !HasValue ) { return default; }

        if ( AsErrors(out Errors? errors) && f2 is not null )
        {
            return await f2(arg, errors, token)
                      .ConfigureAwait(false);
        }

        if ( AsTags(out StringTags? tags) && f3 is not null )
        {
            return await f3(arg, tags.Value, token)
                      .ConfigureAwait(false);
        }

        if ( AsJson(out JToken? jToken) && f0 is not null )
        {
            return await f0(arg, jToken, token)
                      .ConfigureAwait(false);
        }

        return f1 is not null
                   ? await f1(arg, Text, token)
                        .ConfigureAwait(false)
                   : default;
    }


    [Pure] public static ErrorResponse Parse( string? error, IFormatProvider? provider = null )
    {
        if ( string.IsNullOrWhiteSpace(error) ) { return UnknownError; }

        const string ESCAPED_QUOTE = @"\""";
        const string QUOTE         = @"""";
        if ( error.Contains(ESCAPED_QUOTE) ) { error = error.Replace(ESCAPED_QUOTE, QUOTE); }

        return error;
    }
    public static bool TryParse( [NotNullWhen(true)] string? error, IFormatProvider? provider, out ErrorResponse result )
    {
        result = Parse(error, provider);
        return true;
    }

    public static bool TryFromJson( string? json, out ErrorResponse result )
    {
        result = Parse(json, CultureInfo.InvariantCulture);
        return true;
    }
    [Pure] public static ErrorResponse FromJson( string json ) => json.FromJson<ErrorResponse>();


    public          int  CompareTo( ErrorResponse other ) => string.Compare(Text, other.Text, StringComparison.InvariantCulture);
    public          bool Equals( ErrorResponse    other ) => string.Equals(Text, other.Text, StringComparison.InvariantCulture);
    public override bool Equals( object?          other ) => other is ErrorResponse x && Equals(x);
    public override int GetHashCode()
    {
        HashCode hashCode = new HashCode();
        hashCode.Add(Text, StringComparer.InvariantCulture);
        return hashCode.ToHashCode();
    }
    public int CompareTo( object? other ) => other is ErrorResponse x
                                                 ? CompareTo(x)
                                                 : throw new ExpectedValueTypeException(other, typeof(ErrorResponse));


    public static bool operator ==( ErrorResponse? left, ErrorResponse? right ) => Nullable.Equals(left, right);
    public static bool operator !=( ErrorResponse? left, ErrorResponse? right ) => !Nullable.Equals(left, right);
    public static bool operator ==( ErrorResponse  left, ErrorResponse  right ) => EqualityComparer<ErrorResponse>.Default.Equals(left, right);
    public static bool operator !=( ErrorResponse  left, ErrorResponse  right ) => !EqualityComparer<ErrorResponse>.Default.Equals(left, right);
    public static bool operator >( ErrorResponse   left, ErrorResponse  right ) => Comparer<ErrorResponse>.Default.Compare(left, right) > 0;
    public static bool operator >=( ErrorResponse  left, ErrorResponse  right ) => Comparer<ErrorResponse>.Default.Compare(left, right) >= 0;
    public static bool operator <( ErrorResponse   left, ErrorResponse  right ) => Comparer<ErrorResponse>.Default.Compare(left, right) < 0;
    public static bool operator <=( ErrorResponse  left, ErrorResponse  right ) => Comparer<ErrorResponse>.Default.Compare(left, right) <= 0;
}
