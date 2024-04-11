// Jakar.Extensions :: Jakar.Extensions
// 04/10/2024  21:04

using Microsoft.Extensions.Primitives;



namespace Jakar.Extensions;


[Serializable, DefaultValue( nameof(Empty) )]
public readonly record struct Error( Status? StatusCode, string? Type, string? Title, string? Detail, string? Instance, in StringValues Errors ) : IErrorDetails
{
    public const  string NULL = "null";
    public static Error  Empty => new(null, null, null, null, null, StringValues.Empty);


    public static implicit operator Error( Status               result )                         => Create( result,        StringValues.Empty );
    public static implicit operator Error( string               error )                          => Create( Status.NotSet, error );
    public static implicit operator Error( string[]             error )                          => Create( Status.NotSet, error );
    public static implicit operator Error( StringValues         error )                          => Create( Status.NotSet, error );
    public static                   Error Create( Status?       status, in StringValues errors ) => new(status, null, null, null, null, errors);
    public static                   Error Create( IErrorDetails details ) => new(details.StatusCode, details.Type, details.Title, details.Detail, details.Instance, details.Errors);
}



[Serializable, DefaultValue( nameof(Empty) )]
public readonly record struct ErrorOr<T>( T? Value, Error? Error )
{
    public static ErrorOr<T> Empty { get; } = new(default, null);

#if NET6_0_OR_GREATER
    [MemberNotNullWhen( true, nameof(Error) )] public bool HasError { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Error is not null; }
    [MemberNotNullWhen( true, nameof(Value) )] public bool HasValue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Value is not null; }
#endif


    public bool TryGetValue( [NotNullWhen( true )] out T? value, [NotNullWhen( false )] out Error? error )
    {
        if ( Value is not null )
        {
            value = Value;
            error = default;
            return true;
        }

        value = default;
        error = Error ?? Extensions.Error.Empty;
        return false;
    }
    public bool TryGetValue( [NotNullWhen( true )] out T? value )
    {
        value = Value;
        return value is not null;
    }
    public bool TryGetValue( [NotNullWhen( true )] out Error? error )
    {
        error = Error;
        return error.HasValue;
    }


    public static implicit operator T?( ErrorOr<T>     result ) => result.Value;
    public static implicit operator Error?( ErrorOr<T> result ) => result.Error;
    public static implicit operator ErrorOr<T>( T      value )  => new(value, null);
    public static implicit operator ErrorOr<T>( Error  error )  => new(default, error);
}
