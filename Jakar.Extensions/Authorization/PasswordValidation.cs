// Jakar.Extensions :: Jakar.Extensions
// 09/07/2022  3:56 PM


using Microsoft.Extensions.Options;



namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "OutParameterValueIsAlwaysDiscarded.Global" )]
public readonly ref struct PasswordValidator
{
    // ReSharper disable once MemberHidesStaticFromOuterClass
    public static Requirements          Requirements => Current ??= new PasswordRequirements();
    public static PasswordRequirements? Current      { get; set; }


    private readonly Requirements      _requirements;
    public static    PasswordValidator Default => new(Requirements);
    public PasswordValidator( Requirements requirements ) => _requirements = requirements;


    public static bool Check( ReadOnlySpan<char> password ) => Current is not null
                                                                   ? Check( password, Current )
                                                                   : Check( password, Requirements );
    public static bool Check( ReadOnlySpan<char> password, Requirements requirements )
    {
        var validator = new PasswordValidator( requirements );
        return validator.Validate( password );
    }


    public bool Validate( ReadOnlySpan<char> password ) => Validate( password, out bool _, out bool _, out bool _, out bool _, out bool _, out bool _ );
    public bool Validate( ReadOnlySpan<char> password, out bool lengthPassed, out bool specialPassed, out bool numericPassed, out bool lowerPassed, out bool upperPassed, out bool blockedPassed )
    {
        password     = password.Trim();
        lengthPassed = password.Length >= _requirements.MinLength;
        lowerPassed  = !_requirements.RequireLowerCase || password.IndexOfAny( _requirements.LowerCase ) >= 0;
        upperPassed  = !_requirements.RequireUpperCase || password.IndexOfAny( _requirements.UpperCase ) >= 0;


        if ( _requirements.RequireSpecialChar )
        {
            int index = password.IndexOfAny( _requirements.SpecialChars );

            specialPassed = _requirements.CantStartWithSpecialChar
                                ? index >= 1
                                : index >= 0;
        }
        else { specialPassed = true; }


        if ( _requirements.RequireNumber )
        {
            int index = password.IndexOfAny( _requirements.Numbers );

            numericPassed = _requirements.CantStartWithNumber
                                ? index >= 1
                                : index >= 0;
        }
        else { numericPassed = true; }


        if ( !_requirements.BlockedPasswords.IsEmpty )
        {
            blockedPassed = true;

            foreach ( ReadOnlySpan<char> blocked in _requirements.BlockedPasswords )
            {
                if ( !password.Equals( blocked, StringComparison.OrdinalIgnoreCase ) ) { continue; }

                blockedPassed = false;
                break;
            }
        }
        else { blockedPassed = true; }


        return lengthPassed && specialPassed && numericPassed && lowerPassed && upperPassed && blockedPassed;
    }
}



[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameterInConstructor" )]
public readonly ref struct Requirements
{
    public ReadOnlySpan<string> BlockedPasswords         { get; init; }
    public int                  MinLength                { get; init; }
    public bool                 RequireLowerCase         { get; init; }
    public ReadOnlySpan<char>   LowerCase                { get; init; }
    public bool                 RequireUpperCase         { get; init; }
    public ReadOnlySpan<char>   UpperCase                { get; init; }
    public bool                 CantStartWithNumber      { get; init; }
    public bool                 RequireNumber            { get; init; }
    public ReadOnlySpan<char>   Numbers                  { get; init; }
    public bool                 RequireSpecialChar       { get; init; }
    public bool                 CantStartWithSpecialChar { get; init; }
    public ReadOnlySpan<char>   SpecialChars             { get; init; }


    public Requirements( ReadOnlySpan<string> blockedPasswords,
                         int                  minLength,
                         bool                 requireLowerCase,
                         ReadOnlySpan<char>   lowerCase,
                         bool                 requireUpperCase,
                         ReadOnlySpan<char>   upperCase,
                         bool                 cantStartWithNumber,
                         bool                 requireNumber,
                         ReadOnlySpan<char>   numbers,
                         bool                 requireSpecialChar,
                         bool                 cantStartWithSpecialChar,
                         ReadOnlySpan<char>   specialChars
    )
    {
        BlockedPasswords         = blockedPasswords;
        MinLength                = minLength;
        RequireLowerCase         = requireLowerCase;
        LowerCase                = lowerCase;
        RequireUpperCase         = requireUpperCase;
        UpperCase                = upperCase;
        CantStartWithNumber      = cantStartWithNumber;
        RequireNumber            = requireNumber;
        Numbers                  = numbers;
        RequireSpecialChar       = requireSpecialChar;
        CantStartWithSpecialChar = cantStartWithSpecialChar;
        SpecialChars             = specialChars;
    }


    public static implicit operator Requirements( PasswordRequirements data ) => new(data.BlockedPasswords.ToArray(),
                                                                                     data.MinLength,
                                                                                     data.RequireLowerCase,
                                                                                     data.LowerCase,
                                                                                     data.RequireUpperCase,
                                                                                     data.UpperCase,
                                                                                     data.CantStartWithNumber,
                                                                                     data.RequireNumber,
                                                                                     data.Numbers,
                                                                                     data.RequireSpecialChar,
                                                                                     data.CantStartWithSpecialChar,
                                                                                     data.SpecialChars);
}



public sealed record PasswordRequirements : IOptions<PasswordRequirements>
{
    public string[]                                     BlockedPasswords         { get; set; } = Array.Empty<string>();
    public bool                                         CantStartWithNumber      { get; set; } = true;
    public bool                                         CantStartWithSpecialChar { get; set; } = true;
    public string                                       LowerCase                { get; set; } = new(Randoms.LowerCase);
    public int                                          MinLength                { get; init; }
    public string                                       Numbers                  { get; set; } = new(Randoms.Numeric);
    public bool                                         RequireLowerCase         { get; set; } = true;
    public bool                                         RequireNumber            { get; set; } = true;
    public bool                                         RequireSpecialChar       { get; set; } = true;
    public bool                                         RequireUpperCase         { get; set; } = true;
    public string                                       SpecialChars             { get; set; } = new(Randoms.SpecialChars);
    public string                                       UpperCase                { get; set; } = new(Randoms.UpperCase);
    PasswordRequirements IOptions<PasswordRequirements>.Value                    => this;


    public void SetBlockedPasswords( IEnumerable<string> passwords ) =>
        BlockedPasswords = passwords.ToHashSet()
                                    .ToArray();
}
