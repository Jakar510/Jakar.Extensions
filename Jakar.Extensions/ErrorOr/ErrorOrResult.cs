// Jakar.Extensions :: Jakar.Extensions
// 04/10/2024  21:04

namespace Jakar.Extensions;


public interface IErrorOrResult<TValue>
{
    bool               HasErrors { get; }
    bool               HasValue  { get; }
    Status             GetStatus();
    TResult            Match<TResult>( Func<TValue, TResult>                      value, Func<Errors, TResult>            errors );
    ValueTask<TResult> Match<TResult>( Func<TValue, ValueTask<TResult>>           value, Func<Errors, ValueTask<TResult>> errors );
    Task<TResult>      Match<TResult>( Func<TValue, Task<TResult>>                value, Func<Errors, Task<TResult>>      errors );
    bool               TryGetValue( [NotNullWhen(true)] out TValue?               value, [NotNullWhen(false)] out Errors? errors );
    bool               TryGetValue( [NotNullWhen(true)] out TValue?               value );
    bool               TryGetValue( [NotNullWhen(true)] out Errors?               errors );
    bool               TryGetValue( out                     ReadOnlyMemory<Error> errors );
    bool               TryGetValue( out                     ReadOnlySpan<Error>   errors );
    void               Deconstruct( out                     TValue?               value, out Errors? error );
}



/// <summary> Inspired by https://github.com/amantinband/error-or/tree/main </summary>
[Serializable]
[DefaultValue(nameof(Empty))]
public readonly struct ErrorOrResult( bool? value, Errors? error ) : IErrorOrResult<bool?>
{
    public static readonly ErrorOrResult Empty = new(null, Errors.Empty);
    public readonly        bool?         Value = value;
    public readonly        Errors?       Error = error;


    [MemberNotNullWhen(true, nameof(Error))] public bool HasErrors => Error?.IsValid is true;
    [MemberNotNullWhen(true, nameof(Value))] public bool HasValue  => Value is not null;
    [MemberNotNullWhen(true, nameof(Value))] public bool Passed    => Value is true;


    public static ErrorOrResult Create( bool   value )  => new(value, Errors.Empty);
    public static ErrorOrResult Create( Errors errors ) => new(null, errors);
    public        Status        GetStatus()             => Error?.GetStatus() ?? Status.Ok;


    public TResult Match<TResult>( Func<bool?, TResult> value, Func<Errors, TResult> errors ) => TryGetValue(out Errors? e)
                                                                                                     ? errors(e)
                                                                                                     : value(Value);
    public TResult Match<TResult>( Func<bool, TResult> value, Func<Errors, TResult> errors ) => TryGetValue(out Errors? e)
                                                                                                    ? errors(e)
                                                                                                    : value(Value is true);
    public ValueTask<TResult> Match<TResult>( Func<bool?, ValueTask<TResult>> value, Func<Errors, ValueTask<TResult>> errors ) => TryGetValue(out Errors? e)
                                                                                                                                      ? errors(e)
                                                                                                                                      : value(Value);
    public ValueTask<TResult> Match<TResult>( Func<bool, ValueTask<TResult>> value, Func<Errors, ValueTask<TResult>> errors ) => TryGetValue(out Errors? e)
                                                                                                                                     ? errors(e)
                                                                                                                                     : value(Value is true);
    public Task<TResult> Match<TResult>( Func<bool?, Task<TResult>> value, Func<Errors, Task<TResult>> errors ) => TryGetValue(out Errors? e)
                                                                                                                       ? errors(e)
                                                                                                                       : value(Value);
    public Task<TResult> Match<TResult>( Func<bool, Task<TResult>> value, Func<Errors, Task<TResult>> errors ) => TryGetValue(out Errors? e)
                                                                                                                      ? errors(e)
                                                                                                                      : value(this);


    [MemberNotNullWhen(true, nameof(Value))] [MemberNotNullWhen(false, nameof(Error))] public bool TryGetValue( [NotNullWhen(true)] out bool? value, [NotNullWhen(false)] out Errors? errors )
    {
        value  = Value;
        errors = null;
        return value is true;
    }


    [MemberNotNullWhen(true, nameof(Value))] [MemberNotNullWhen(false, nameof(Error))] public bool TryGetValue( [NotNullWhen(true)] out bool? value )
    {
        value = Value;
        return value is true;
    }


    [MemberNotNullWhen(true, nameof(Error))] public bool TryGetValue( [NotNullWhen(true)] out Errors? errors )
    {
        errors = Error;
        return errors?.IsValid is true;
    }
    public bool TryGetValue( out ReadOnlyMemory<Error> errors )
    {
        errors = Error?.Details;
        return !errors.IsEmpty;
    }
    public bool TryGetValue( out ReadOnlySpan<Error> errors )
    {
        errors = Error?.Details;
        return !errors.IsEmpty;
    }


    public void Deconstruct( out bool? value, out Errors? error )
    {
        value = Value;
        error = Error;
    }
    public void Deconstruct( out bool value, out Errors? error )
    {
        value = false;
        error = null;
    }


    public static implicit operator OneOf<bool?, Errors>( ErrorOrResult result ) => result.TryGetValue(out Errors? errors)
                                                                                        ? errors
                                                                                        : result.Value;
    public static implicit operator OneOf<bool, Errors>( ErrorOrResult result ) => result.TryGetValue(out Errors? errors)
                                                                                       ? errors
                                                                                       : true;
    public static implicit operator bool?( ErrorOrResult                 result ) => result.Value;
    public static implicit operator bool( ErrorOrResult                  result ) => result.Value is true;
    public static implicit operator ErrorOrResult( bool                  value )  => Create(value);
    public static implicit operator ErrorOrResult( Error                 error )  => Create(error);
    public static implicit operator ErrorOrResult( Error[]               errors ) => Create(errors);
    public static implicit operator ErrorOrResult( List<Error>           errors ) => Create(errors);
    public static implicit operator ErrorOrResult( Errors                errors ) => Create(errors);
    public static implicit operator Errors?( ErrorOrResult               result ) => result.Error;
    public static implicit operator ReadOnlySpan<Error>( ErrorOrResult   result ) => result.Error?.Details;
    public static implicit operator ReadOnlyMemory<Error>( ErrorOrResult result ) => result.Error?.Details;
}



/// <summary> Inspired by https://github.com/amantinband/error-or/tree/main </summary>
[Serializable]
[DefaultValue(nameof(Empty))]
public readonly struct ErrorOrResult<TValue>( TValue? value, Errors? error ) : IErrorOrResult<TValue>
{
    public static readonly ErrorOrResult<TValue> Empty = new(default, Errors.Empty);
    public readonly        Errors?               Error = error;
    public readonly        TValue?               Value = value;


    [MemberNotNullWhen(false, nameof(Value))] [MemberNotNullWhen(true, nameof(Error))] public bool HasErrors => Error?.IsValid is true && Value is null;
    [MemberNotNullWhen(true,  nameof(Value))]                                          public bool HasValue  => Value is not null;


    public static ErrorOrResult<TValue> Create( TValue value )  => new(value, Errors.Empty);
    public static ErrorOrResult<TValue> Create( Errors errors ) => new(default, errors);
    public        Status                GetStatus()             => Error?.GetStatus() ?? Status.Ok;


    public TResult Match<TResult>( Func<TValue, TResult> value, Func<Errors, TResult> errors ) =>
        TryGetValue(out TValue? x, out Errors? e)
            ? value(x)
            : errors(e);
    public ValueTask<TResult> Match<TResult>( Func<TValue, ValueTask<TResult>> value, Func<Errors, ValueTask<TResult>> errors ) =>
        TryGetValue(out TValue? x, out Errors? e)
            ? value(x)
            : errors(e);
    public Task<TResult> Match<TResult>( Func<TValue, Task<TResult>> value, Func<Errors, Task<TResult>> errors ) =>
        TryGetValue(out TValue? x, out Errors? e)
            ? value(x)
            : errors(e);


    [MemberNotNullWhen(true, nameof(Value))] [MemberNotNullWhen(false, nameof(Error))] public bool TryGetValue( [NotNullWhen(true)] out TValue? value, [NotNullWhen(false)] out Errors? errors )
    {
        errors = Error;
        value  = Value;
        return value is not null;
    }


    [MemberNotNullWhen(true, nameof(Value))] [MemberNotNullWhen(false, nameof(Error))] public bool TryGetValue( [NotNullWhen(true)] out TValue? value )
    {
        value = Value;
        return value is not null;
    }


    [MemberNotNullWhen(false, nameof(Error))] public bool TryGetValue( [NotNullWhen(true)] out Errors? errors )
    {
        errors = Error;
        return errors?.IsValid is true;
    }
    public bool TryGetValue( out ReadOnlyMemory<Error> errors )
    {
        errors = Error?.Details;
        return !errors.IsEmpty;
    }
    public bool TryGetValue( out ReadOnlySpan<Error> errors )
    {
        errors = Error?.Details;
        return !errors.IsEmpty;
    }


    public void Deconstruct( out TValue? value, out Errors? error )
    {
        value = Value;
        error = Error;
    }


    public static implicit operator OneOf<TValue, Errors>( ErrorOrResult<TValue> result ) => result.TryGetValue(out TValue? value, out Errors? errors)
                                                                                                 ? value
                                                                                                 : errors;
    public static implicit operator TValue?( ErrorOrResult<TValue>               result ) => result.Value;
    public static implicit operator Errors?( ErrorOrResult<TValue>               result ) => result.Error;
    public static implicit operator ReadOnlySpan<Error>( ErrorOrResult<TValue>   result ) => result.Error?.Details;
    public static implicit operator ReadOnlyMemory<Error>( ErrorOrResult<TValue> result ) => result.Error?.Details;
    public static implicit operator ErrorOrResult<TValue>( TValue                value )  => Create(value);
    public static implicit operator ErrorOrResult<TValue>( Error                 error )  => Create(error);
    public static implicit operator ErrorOrResult<TValue>( List<Error>           errors ) => Create(errors);
    public static implicit operator ErrorOrResult<TValue>( Error[]               errors ) => Create(errors);
    public static implicit operator ErrorOrResult<TValue>( Errors                errors ) => Create(errors);
}
