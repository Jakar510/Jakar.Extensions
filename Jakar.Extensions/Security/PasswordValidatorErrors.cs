// Jakar.Extensions :: Jakar.Extensions
// 11/16/2026

namespace Jakar.Extensions;



/// <summary> <see cref="Error"/> factories for <see cref="PasswordValidator.Results"/> (Jakar.ErrorOr can't see <see cref="PasswordValidator"/>). </summary>
public static class PasswordValidatorErrors
{
    extension( PasswordValidator.Results results )
    {
        /// <summary> The failed password rules as <see cref="StringTags"/> entries (titles from <see cref="IErrorTitles.Current"/>). </summary>
        public StringTags ToStringTags()
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
    }



    extension( Error )
    {
        /// <summary> <see cref="Error.Unauthorized(StringTags, string?, string?, string?, string?)"/> with the failed password rules (<see cref="PasswordValidator.Results.ToStringTags"/>) and <see cref="IErrorTitles.PasswordValidation"/> as the title. </summary>
        public static Error Unauthorized( ref readonly PasswordValidator.Results results, string? instance = null, string? description = null, string? type = null ) => Error.Unauthorized(results.ToStringTags(), instance, description, IErrorTitles.Current.PasswordValidation, type);
    }
}
