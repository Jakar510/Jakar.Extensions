// Jakar.Extensions :: Jakar.Extensions
// 04/10/2024  21:04

namespace Jakar.Extensions;


/// <summary> Inspired by https://github.com/amantinband/error-or/tree/main </summary>
/// <typeparam name="T"> </typeparam>
/// <param name="Value"> </param>
/// <param name="Errors"> </param>
[Serializable, DefaultValue( nameof(Empty) )]
public readonly record struct ErrorOrResult<T>( in T? Value, in Error[]? Errors )
{
    public static ErrorOrResult<T> Empty { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(default, null); }

#if NET6_0_OR_GREATER
    [MemberNotNullWhen( true, nameof(Errors) )]
#endif
    public bool HasErrors { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Errors?.Length is > 0; }


#if NET6_0_OR_GREATER
    [MemberNotNullWhen( true, nameof(Value) )]
#endif
    public bool HasValue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Value is not null; }


    public static ErrorOrResult<T> Create( in     T       value )  => new(value, null);
    public static ErrorOrResult<T> Create( params Error[] errors ) => new(default, errors);


    public Status GetStatus() => Errors?.Max( static x => x.StatusCode ) ?? Status.Ok;


    public bool TryGetValue( [NotNullWhen( true )] out T? value, [NotNullWhen( false )] out Error[]? errors )
    {
        if ( Value is not null )
        {
            value  = Value;
            errors = null;
            return true;
        }

        value  = default;
        errors = Errors ?? [];
        return false;
    }
    public bool TryGetValue( [NotNullWhen( true )] out T? value, out ReadOnlyMemory<Error> errors )
    {
        if ( Value is not null )
        {
            value  = Value;
            errors = default;
            return true;
        }

        value  = default;
        errors = Errors;
        return false;
    }
    public bool TryGetValue( [NotNullWhen( true )] out T? value )
    {
        value = Value;
        return value is not null;
    }
    public bool TryGetValue( [NotNullWhen( true )] out Error[]? errors )
    {
        errors = Errors;
        return errors is not null;
    }
    public bool TryGetValue( out ReadOnlyMemory<Error> error )
    {
        error = Errors;
        return error.IsEmpty is false;
    }
    public bool TryGetValue( out ReadOnlySpan<Error> error )
    {
        error = Errors;
        return error.IsEmpty is false;
    }


    public static implicit operator OneOf<T?, Error[]>( ErrorOrResult<T> result ) => result.HasValue
                                                                                         ? result.Value
                                                                                         : result.Errors ?? [];
    public static implicit operator T?( ErrorOrResult<T>                    result ) => result.Value;
    public static implicit operator Error[]( ErrorOrResult<T>               result ) => result.Errors ?? [];
    public static implicit operator ReadOnlySpan<Error>( ErrorOrResult<T>   result ) => result.Errors;
    public static implicit operator ReadOnlyMemory<Error>( ErrorOrResult<T> result ) => result.Errors;
    public static implicit operator ErrorOrResult<T>( T                     value )  => Create( value );
    public static implicit operator ErrorOrResult<T>( Error                 error )  => Create( error );
    public static implicit operator ErrorOrResult<T>( List<Error>           errors ) => Create( [..errors] );
    public static implicit operator ErrorOrResult<T>( Error[]               errors ) => Create( errors );
}
