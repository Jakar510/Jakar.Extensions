// Jakar.Extensions :: Jakar.Extensions
// 04/10/2024  21:04

using ZXing;



namespace Jakar.Extensions;


/// <summary> Inspired by https://github.com/amantinband/error-or/tree/main </summary>
/// <typeparam name="T"> </typeparam>
/// <param name="Value"> </param>
/// <param name="Errors"> </param>
[Serializable, DefaultValue( nameof(Empty) ), SuppressMessage( "ReSharper", "RedundantExplicitPositionalPropertyDeclaration" )]
public readonly record struct ErrorOrResult( in bool? Value, in Error[]? Errors )
{
    public static ErrorOrResult Empty { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(default, null); }


#if NET6_0_OR_GREATER
    [MemberNotNullWhen( true, nameof(Errors) ), MemberNotNullWhen( false, nameof(Value) )]
#endif
    public bool HasErrors { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TryGetValue( out Error[]? _ ) is false; }


#if NET6_0_OR_GREATER
    [MemberNotNullWhen( true, nameof(Value) ), MemberNotNullWhen( false, nameof(Errors) )]
#endif
    public bool HasValue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TryGetValue( out Error[]? _ ); }
    public bool Passed { [MethodImpl(   MethodImplOptions.AggressiveInlining )] get => Value is true; }


    public Status GetStatus() => Errors.GetStatus();


    public TResult Match<TResult>( Func<bool?, TResult> value, Func<Error[], TResult> errors ) => TryGetValue( out Error[]? e )
                                                                                                      ? errors( e )
                                                                                                      : value( Value );
    public TResult Match<TResult>( Func<bool, TResult> value, Func<Error[], TResult> errors ) => TryGetValue( out Error[]? e )
                                                                                                     ? errors( e )
                                                                                                     : value( Value is true );
    public ValueTask<TResult> Match<TResult>( Func<bool?, ValueTask<TResult>> value, Func<Error[], ValueTask<TResult>> errors ) => TryGetValue( out Error[]? e )
                                                                                                                                       ? errors( e )
                                                                                                                                       : value( Value );
    public ValueTask<TResult> Match<TResult>( Func<bool, ValueTask<TResult>> value, Func<Error[], ValueTask<TResult>> errors ) => TryGetValue( out Error[]? e )
                                                                                                                                      ? errors( e )
                                                                                                                                      : value( Value is true );
    public Task<TResult> Match<TResult>( Func<bool?, Task<TResult>> value, Func<Error[], Task<TResult>> errors ) => TryGetValue( out Error[]? e )
                                                                                                                        ? errors( e )
                                                                                                                        : value( Value );
    public Task<TResult> Match<TResult>( Func<bool, Task<TResult>> value, Func<Error[], Task<TResult>> errors ) => TryGetValue( out Error[]? e )
                                                                                                                       ? errors( e )
                                                                                                                       : value( this );


    public static ErrorOrResult Create( in     bool?   value )  => new(value, null);
    public static ErrorOrResult Create( params Error[] errors ) => new(null, errors);


    public bool TryGetValue( [NotNullWhen( true )] out Error[]? errors )
    {
        errors = Errors;
        return errors is not null;
    }
    public bool TryGetValue( out ReadOnlyMemory<Error> errors )
    {
        errors = Errors;
        return errors.IsEmpty is false;
    }
    public bool TryGetValue( out ReadOnlySpan<Error> errors )
    {
        errors = Errors;
        return errors.IsEmpty is false;
    }


    public static implicit operator OneOf<bool?, Error[]>( ErrorOrResult result ) => result.TryGetValue( out Error[]? errors )
                                                                                         ? errors
                                                                                         : result.Value;
    public static implicit operator OneOf<bool, Error[]>( ErrorOrResult result ) => result.TryGetValue( out Error[]? errors )
                                                                                        ? errors
                                                                                        : true;
    public static implicit operator bool?( ErrorOrResult                 result ) => result.Value;
    public static implicit operator bool( ErrorOrResult                  result ) => result.Value is true;
    public static implicit operator ErrorOrResult( bool                  value )  => Create( value );
    public static implicit operator ErrorOrResult( bool?                 value )  => Create( value );
    public static implicit operator ErrorOrResult( Error                 error )  => Create( error );
    public static implicit operator ErrorOrResult( List<Error>           errors ) => Create( [..errors] );
    public static implicit operator ErrorOrResult( Error[]               errors ) => Create( errors );
    public static implicit operator Error[]( ErrorOrResult               result ) => result.Errors ?? [];
    public static implicit operator ReadOnlySpan<Error>( ErrorOrResult   result ) => result.Errors;
    public static implicit operator ReadOnlyMemory<Error>( ErrorOrResult result ) => result.Errors;
}



/// <summary> Inspired by https://github.com/amantinband/error-or/tree/main </summary>
/// <typeparam name="T"> </typeparam>
/// <param name="Value"> </param>
/// <param name="Errors"> </param>
[Serializable, DefaultValue( nameof(Empty) )]
public readonly record struct ErrorOrResult<T>( in T? Value, in Error[]? Errors )
{
    public static ErrorOrResult<T> Empty { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(default, null); }


#if NET6_0_OR_GREATER
    [MemberNotNullWhen( true, nameof(Errors) ), MemberNotNullWhen( false, nameof(Value) )]
#endif
    public bool HasErrors { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TryGetValue( out _, out _ ) is false; }


#if NET6_0_OR_GREATER
    [MemberNotNullWhen( true, nameof(Value) ), MemberNotNullWhen( false, nameof(Errors) )]
#endif
    public bool HasValue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TryGetValue( out _, out _ ); }


    public Status GetStatus() => Errors.GetStatus();


    public TResult Match<TResult>( Func<T, TResult> value, Func<Error[], TResult> errors ) =>
        TryGetValue( out T? x, out Error[]? e )
            ? value( x )
            : errors( e );
    public ValueTask<TResult> Match<TResult>( Func<T, ValueTask<TResult>> value, Func<Error[], ValueTask<TResult>> errors ) =>
        TryGetValue( out T? x, out Error[]? e )
            ? value( x )
            : errors( e );
    public Task<TResult> Match<TResult>( Func<T, Task<TResult>> value, Func<Error[], Task<TResult>> errors ) =>
        TryGetValue( out T? x, out Error[]? e )
            ? value( x )
            : errors( e );


    public static ErrorOrResult<T> Create( in     T       value )  => new(value, null);
    public static ErrorOrResult<T> Create( params Error[] errors ) => new(default, errors);


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
    public bool TryGetValue( out ReadOnlyMemory<Error> errors )
    {
        errors = Errors;
        return errors.IsEmpty is false;
    }
    public bool TryGetValue( out ReadOnlySpan<Error> errors )
    {
        errors = Errors;
        return errors.IsEmpty is false;
    }


    public static implicit operator OneOf<T, Error[]>( ErrorOrResult<T> result ) => result.TryGetValue( out T? value, out Error[]? errors )
                                                                                        ? value
                                                                                        : errors;
    public static implicit operator T?( ErrorOrResult<T>                    result ) => result.Value;
    public static implicit operator Error[]( ErrorOrResult<T>               result ) => result.Errors ?? [];
    public static implicit operator ReadOnlySpan<Error>( ErrorOrResult<T>   result ) => result.Errors;
    public static implicit operator ReadOnlyMemory<Error>( ErrorOrResult<T> result ) => result.Errors;
    public static implicit operator ErrorOrResult<T>( T                     value )  => Create( value );
    public static implicit operator ErrorOrResult<T>( Error                 error )  => Create( error );
    public static implicit operator ErrorOrResult<T>( List<Error>           errors ) => Create( [..errors] );
    public static implicit operator ErrorOrResult<T>( Error[]               errors ) => Create( errors );
}
