// Jakar.Extensions :: Jakar.Extensions
// 03/03/2025  13:03

using ZLinq;



namespace Jakar.Extensions;


public static class ErrorExtensions
{
    public const string BULLET = "• ";
    public const string SPACER = "\n    -";


    public static StringTags ToValues( this ref readonly PasswordValidator.Results results )
    {
        using IMemoryOwner<string> owner  = MemoryPool<string>.Shared.Rent(10);
        Span<string>               errors = owner.Memory.Span;
        int                        count  = 0;
        if ( results.LengthPassed ) { errors[count++] = Error.Titles.BlockedPassed; }

        if ( results.MustBeTrimmed ) { errors[count++] = Error.Titles.MustBeTrimmed; }

        if ( results.SpecialPassed ) { errors[count++] = Error.Titles.SpecialPassed; }

        if ( results.NumericPassed ) { errors[count++] = Error.Titles.NumericPassed; }

        if ( results.LowerPassed ) { errors[count++] = Error.Titles.LowerPassed; }

        if ( results.UpperPassed ) { errors[count++] = Error.Titles.UpperPassed; }

        if ( results.BlockedPassed ) { errors[count] = Error.Titles.BlockedPassed; }

        string[] values = [.. errors[..count]];
        return values;
    }


    public static string GetMessage( this Errors             errors ) => string.Join('\n', errors.Details.Select(GetMessage));
    public static string GetMessage( this IEnumerable<Error> errors ) => string.Join('\n', errors.Select(GetMessage));

    public static string GetMessage( this ref readonly ReadOnlySpan<Error> errors )
    {
        using IMemoryOwner<string?> owner = MemoryPool<string?>.Shared.Rent(errors.Length);
        Span<string?>               span  = owner.Memory.Span;
        int                         count = 0;

        foreach ( string error in errors.AsValueEnumerable()
                                        .Select(GetMessage) ) { span[count++] = error; }

        StringBuilder sb = new(4096);
        sb.AppendJoin('\n', span);
        return sb.ToString();
    }

    public static string GetMessage( this Error error ) => GetMessage(error.Title, in error.errors);
    public static string GetMessage( this string? title, ref readonly StringTags? tags )
    {
        if ( tags is null ) { return title ?? EMPTY; }

        StringTags values = tags.Value;
        if ( values.Values.Length == 0 || values.Tags.Length == 0 ) { return title ?? EMPTY; }

        using ValueStringBuilder builder = new(4096);

        builder.Append(BULLET)
               .Append(title ?? EMPTY);

        foreach ( string value in values.Values.AsSpan() )
        {
            builder.Append(SPACER)
                   .Append(value);
        }

        return builder.ToString();
    }


    public static Status GetStatus( this IEnumerable<Error>? errors ) => errors?.Max(static x => x.GetStatus()) ?? Status.Ok;
    public static Status GetStatus( this Error[]? errors, Status status )
    {
        ReadOnlySpan<Error> span = errors;
        return GetStatus(in span, status);
    }
    public static Status GetStatus( this ref readonly ReadOnlySpan<Error> errors, Status status )
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
