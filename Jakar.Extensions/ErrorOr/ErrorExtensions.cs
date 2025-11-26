// Jakar.Extensions :: Jakar.Extensions
// 03/03/2025  13:03

using System.Linq;
using ZLinq;



namespace Jakar.Extensions;


public static class ErrorExtensions
{
    public const string BULLET = "• ";
    public const string SPACER = "\n    -";


    public static StringTags ToStringTags( this ref readonly PasswordValidator.Results results )
    {
        using IMemoryOwner<string> owner  = MemoryPool<string>.Shared.Rent(10);
        Span<string>               errors = owner.Memory.Span;
        int                        count  = 0;
        IErrorTitles               titles = IErrorTitles.Current;
        if ( results.LengthPassed ) { errors[count++] = titles.BlockedPassed; }

        if ( results.MustBeTrimmed ) { errors[count++] = titles.MustBeTrimmed; }

        if ( results.SpecialPassed ) { errors[count++] = titles.SpecialPassed; }

        if ( results.NumericPassed ) { errors[count++] = titles.NumericPassed; }

        if ( results.LowerPassed ) { errors[count++] = titles.LowerPassed; }

        if ( results.UpperPassed ) { errors[count++] = titles.UpperPassed; }

        if ( results.BlockedPassed ) { errors[count] = titles.BlockedPassed; }

        string[] values = [.. errors[..count]];
        return values;
    }


    public static string GetMessage( this Errors errors )
    {
        using PooledArray<string> array = errors.Details.AsValueEnumerable()
                                                .Select(GetMessage)
                                                .ToArrayPool();

        return string.Join('\n', array.Span);
    }
    public static string GetMessage( this IEnumerable<Error> errors ) => string.Join('\n', errors.Select(GetMessage));


    public static string GetMessage( this in ReadOnlySpan<Error> errors )
    {
        using IMemoryOwner<string?> owner = MemoryPool<string?>.Shared.Rent(errors.Length);
        Span<string?>               span  = owner.Memory.Span;
        int                         count = 0;

        foreach ( string error in errors.Select(GetMessage) ) { span[count++] = error; }

        StringBuilder sb = new(4096);
        sb.AppendJoin('\n', span);
        return sb.ToString();
    }

    public static string GetMessage( this Error error ) => GetMessage(error.Title, in error.details);
    public static string GetMessage( this string? title, ref readonly StringTags tags )
    {
        if ( tags.IsEmpty ) { return title ?? EMPTY; }

        using ValueStringBuilder builder = new(4096);

        builder.Append(BULLET)
               .Append(title ?? EMPTY);

        foreach ( string value in tags.Entries.AsSpan() )
        {
            builder.Append(SPACER)
                   .Append(value);
        }

        foreach ( ref readonly Pair value in tags.Tags.AsSpan() )
        {
            if ( value.Value is not null )
            {
                builder.Append(SPACER)
                       .Append(value.Value);
            }
        }

        return builder.ToString();
    }


    public static Status GetStatus( this IEnumerable<Error>? errors ) => errors?.Max(Error.GetStatus) ?? Status.Ok;
    public static Status GetStatus( this Error[]? errors, Status status )
    {
        ReadOnlySpan<Error> span = errors;
        return GetStatus(in span, status);
    }
    public static Status GetStatus( this in ReadOnlySpan<Error> errors, Status minStatus )
    {
        if ( errors.IsEmpty ) { return minStatus; }

        foreach ( Error error in errors )
        {
            Status? code = error.StatusCode;
            if ( code > minStatus ) { minStatus = code.Value; }
        }

        return minStatus;
    }
}
