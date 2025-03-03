// Jakar.Extensions :: Jakar.Extensions
// 03/03/2025  13:03

using Microsoft.Extensions.Primitives;



namespace Jakar.Extensions;


public static class ErrorExtensions
{
    public const string BULLET = "• ";
    public const string SPACER = "\n    -";


    public static StringValues ToValues( this ref readonly PasswordValidator.Results results )
    {
        using IMemoryOwner<string> owner  = MemoryPool<string>.Shared.Rent( 7 );
        Span<string>               errors = owner.Memory.Span;
        int                        count  = 0;
        if ( results.LengthPassed ) { errors[count++] = Error.Titles.BlockedPassed; }

        if ( results.MustBeTrimmed ) { errors[count++] = Error.Titles.MustBeTrimmed; }

        if ( results.SpecialPassed ) { errors[count++] = Error.Titles.SpecialPassed; }

        if ( results.NumericPassed ) { errors[count++] = Error.Titles.NumericPassed; }

        if ( results.LowerPassed ) { errors[count++] = Error.Titles.LowerPassed; }

        if ( results.UpperPassed ) { errors[count++] = Error.Titles.UpperPassed; }

        if ( results.BlockedPassed ) { errors[count] = Error.Titles.BlockedPassed; }

        StringValues values = [.. errors[..count]];
        return values;
    }
    public static string GetMessage( this IEnumerable<Error> errors ) => string.Join( '\n', errors.Select( GetMessage ) );
    public static string GetMessage( this ReadOnlySpan<Error> errors )
    {
        using IMemoryOwner<string?> owner = MemoryPool<string?>.Shared.Rent( errors.Length );
        Span<string?>               span  = owner.Memory.Span;

        errors.Select<Error, string?>( GetMessage ).ConsumeInto( span );

        StringBuilder sb = new(4096);
        sb.AppendJoin( '\n', span );
        return sb.ToString();
    }
    public static string GetMessage( this Error error ) => GetMessage( error.Title, error.Errors );
    public static string GetMessage( this string? title, StringValues values )
    {
        if ( values.Count == 0 ) { return title ?? string.Empty; }

        using ValueStringBuilder builder = new(4096);
        builder.Append( BULLET ).Append( title );

        foreach ( string? value in values ) { builder.Append( SPACER ).Append( value ); }

        return builder.ToString();
    }


    public static Status GetStatus( this IEnumerable<Error>? errors )                            => errors?.Max( static x => x.GetStatus() ) ?? Status.Ok;
    public static Status GetStatus( this Error[]?            errors, Status status = Status.Ok ) => GetStatus( new ReadOnlySpan<Error>( errors ), status );
    public static Status GetStatus( this ReadOnlySpan<Error> errors, Status status = Status.Ok )
    {
        if ( errors.IsEmpty ) { return status; }

        foreach ( Error error in errors )
        {
            Status? code = error.StatusCode;
            if ( code > status ) { status = code.Value; }
        }

        return status;
    }
}
