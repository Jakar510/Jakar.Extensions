// Jakar.Extensions :: Jakar.Extensions
// 09/07/2022  3:56 PM

namespace Jakar.Extensions;


public readonly ref struct PasswordRequirements
{
    public ReadOnlySpan<string> BlockedPasswords         { get; init; }
    public int                  MinLength                { get; init; } = 10;
    public bool                 RequireLowerCase         { get; init; } = true;
    public ReadOnlySpan<char>   LowerCase                { get; init; } = Randoms.LowerCase;
    public bool                 RequireUpperCase         { get; init; } = true;
    public ReadOnlySpan<char>   UpperCase                { get; init; } = Randoms.UpperCase;
    public bool                 CantStartWithNumber      { get; init; } = true;
    public bool                 RequireNumber            { get; init; } = true;
    public ReadOnlySpan<char>   Numbers                  { get; init; } = Randoms.Numeric;
    public bool                 RequireSpecialChar       { get; init; } = true;
    public bool                 CantStartWithSpecialChar { get; init; } = true;
    public ReadOnlySpan<char>   SpecialChars             { get; init; } = Randoms.SpecialChars;


    public PasswordRequirements() => BlockedPasswords = Array.Empty<string>();
    public PasswordRequirements( IEnumerable<string> blockedPasswords ) : this(10, blockedPasswords) { }
    public PasswordRequirements( int minLength, IEnumerable<string> blockedPasswords )
    {
        MinLength = minLength;

        BlockedPasswords = blockedPasswords.Where(x => x.Length >= minLength)
                                           .ToArray();
    }
    public static PasswordRequirements Default => new();
}



public readonly ref struct PasswordValidator
{
    private readonly PasswordRequirements _requirements;

    public PasswordValidator( PasswordRequirements requirements ) => _requirements = requirements;


    public bool Validate( in ReadOnlySpan<char> password )
    {
        ReadOnlySpan<char> span = password.Trim();
        return span.Length >= _requirements.MinLength && HandleSpecial(span) && HandleNumeric(span) && HandleLower(span) && HandleUpper(span) && HandleBlocked(span);
    }
    private bool HandleNumeric( in ReadOnlySpan<char> span )
    {
        if ( !_requirements.RequireNumber ) { return true; }

        int index = _requirements.CantStartWithNumber
                        ? 1
                        : 0;

        return span.IndexOfAny(_requirements.Numbers) >= index;
    }
    private bool HandleSpecial( in ReadOnlySpan<char> span )
    {
        if ( !_requirements.RequireSpecialChar ) { return true; }

        int index = _requirements.CantStartWithSpecialChar
                        ? 1
                        : 0;

        return span.IndexOfAny(_requirements.SpecialChars) >= index;
    }
    private bool HandleUpper( in ReadOnlySpan<char> span )
    {
        if ( !_requirements.RequireUpperCase ) { return true; }

        return span.IndexOfAny(_requirements.UpperCase) >= 0;
    }
    private bool HandleLower( in ReadOnlySpan<char> span )
    {
        if ( !_requirements.RequireLowerCase ) { return true; }

        return span.IndexOfAny(_requirements.LowerCase) >= 0;
    }
    private bool HandleBlocked( in ReadOnlySpan<char> span )
    {
        if ( _requirements.BlockedPasswords.IsEmpty ) { return true; }

        foreach ( ReadOnlySpan<char> password in _requirements.BlockedPasswords )
        {
            if ( span.Equals(password.Trim(), StringComparison.OrdinalIgnoreCase) ) { return false; }
        }

        return true;
    }


    public static bool ValidatePassword( in ReadOnlySpan<char> password ) => ValidatePassword(password, PasswordRequirements.Default);
    public static bool ValidatePassword( in ReadOnlySpan<char> password, in PasswordRequirements requirements )
    {
        var validator = new PasswordValidator(requirements);
        return validator.Validate(password);
    }
}
