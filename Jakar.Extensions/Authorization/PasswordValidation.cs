// Jakar.Extensions :: Jakar.Extensions
// 09/07/2022  3:56 PM


using Microsoft.Extensions.Options;



namespace Jakar.Extensions;


[DefaultValue( nameof(Default) ), SuppressMessage( "ReSharper", "OutParameterValueIsAlwaysDiscarded.Global" )]
public readonly ref struct PasswordValidator
{
    private readonly Requirements _requirements;

    public static PasswordValidator Default => new(Requirements.Default);
    public PasswordValidator() => throw new InvalidOperationException( "Use the constructor with Requirements" );
    public PasswordValidator( Requirements requirements ) => _requirements = requirements;


    public static bool Check( ReadOnlySpan<char> password ) => Check( password, Requirements.Default );
    public static bool Check( ReadOnlySpan<char> password, Requirements requirements )
    {
        var validator = new PasswordValidator( requirements );
        return validator.Validate( password );
    }


    public bool Validate( scoped in ReadOnlySpan<char> span ) => Validate( span, out Results _ );
    public bool Validate( scoped in ReadOnlySpan<char> span, out Results results )
    {
        ReadOnlySpan<char> password      = span.Trim();
        bool               lengthPassed  = password.Length                                                                           >= _requirements.MinLength && password.Length < _requirements.MinLength;
        bool               mustBeTrimmed = _requirements.MustBeTrimmed is false    || password.Length                                == span.Length;
        bool               lowerPassed   = _requirements.RequireLowerCase is false || password.IndexOfAny( _requirements.LowerCase ) >= 0;
        bool               upperPassed   = _requirements.RequireUpperCase is false || password.IndexOfAny( _requirements.UpperCase ) >= 0;
        bool               specialPassed;
        bool               numericPassed;
        bool               blockedPassed;


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


        if ( _requirements.BlockedPasswords.IsEmpty is false )
        {
            blockedPassed = true;

            foreach ( ReadOnlySpan<char> blocked in _requirements.BlockedPasswords )
            {
                if ( password.Equals( blocked, StringComparison.OrdinalIgnoreCase ) is false ) { continue; }

                blockedPassed = false;
                break;
            }
        }
        else { blockedPassed = true; }


        results = new Results( lengthPassed, mustBeTrimmed, specialPassed, numericPassed, lowerPassed, upperPassed, blockedPassed );
        return results.IsValid;
    }



    public readonly record struct Results( bool LengthPassed, bool MustBeTrimmed, bool SpecialPassed, bool NumericPassed, bool LowerPassed, bool UpperPassed, bool BlockedPassed ) : IValidator
    {
        public        bool IsValid                     => LengthPassed && MustBeTrimmed && SpecialPassed && NumericPassed && LowerPassed && UpperPassed && BlockedPassed;
        public static bool operator true( Results  x ) => x.IsValid;
        public static bool operator false( Results x ) => x.IsValid is false;
    }
}



[DefaultValue( nameof(Default) ), SuppressMessage( "ReSharper", "SuggestBaseTypeForParameterInConstructor" )]
public readonly ref struct Requirements( ReadOnlySpan<string> blockedPasswords,
                                         int                  minLength,
                                         int                  maxLength,
                                         bool                 mustBeTrimmed,
                                         bool                 requireLowerCase,
                                         ReadOnlySpan<char>   lowerCase,
                                         bool                 requireUpperCase,
                                         ReadOnlySpan<char>   upperCase,
                                         bool                 cantStartWithNumber,
                                         bool                 requireNumber,
                                         ReadOnlySpan<char>   numbers,
                                         bool                 requireSpecialChar,
                                         bool                 cantStartWithSpecialChar,
                                         ReadOnlySpan<char>   specialChars )
{
    public static Requirements         Default                  => PasswordRequirements.Current;
    public        ReadOnlySpan<string> BlockedPasswords         { get; } = blockedPasswords;
    public        int                  MinLength                { get; } = minLength;
    public        int                  MaxLength                { get; } = maxLength;
    public        bool                 RequireLowerCase         { get; } = requireLowerCase;
    public        ReadOnlySpan<char>   LowerCase                { get; } = lowerCase;
    public        bool                 MustBeTrimmed            { get; } = mustBeTrimmed;
    public        bool                 RequireUpperCase         { get; } = requireUpperCase;
    public        ReadOnlySpan<char>   UpperCase                { get; } = upperCase;
    public        bool                 CantStartWithNumber      { get; } = cantStartWithNumber;
    public        bool                 RequireNumber            { get; } = requireNumber;
    public        ReadOnlySpan<char>   Numbers                  { get; } = numbers;
    public        bool                 RequireSpecialChar       { get; } = requireSpecialChar;
    public        bool                 CantStartWithSpecialChar { get; } = cantStartWithSpecialChar;
    public        ReadOnlySpan<char>   SpecialChars             { get; } = specialChars;


    public static implicit operator Requirements( PasswordRequirements data ) => new(data.BlockedPasswords.ToArray(),
                                                                                     data.MinLength,
                                                                                     data.MaxLength,
                                                                                     data.MustBeTrimmed,
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
    public const   int                   MAX_LENGTH = 255;
    public const   int                   MIN_LENGTH = 10;
    private static PasswordRequirements? _current;
    private        int                   _maxLength = MAX_LENGTH;
    private        int                   _minLength = MIN_LENGTH;


    public static PasswordRequirements Current { get => _current ??= new PasswordRequirements(); set => _current = value; }


    public HashSet<string>                              BlockedPasswords         { get;               init; } = [];
    public bool                                         CantStartWithNumber      { get;               set; }  = true;
    public bool                                         CantStartWithSpecialChar { get;               set; }  = true;
    public string                                       LowerCase                { get;               set; }  = Randoms.LOWER_CASE;
    public int                                          MaxLength                { get => _maxLength; set => _maxLength = Math.Min( MAX_LENGTH, value ); }
    public int                                          MinLength                { get => _minLength; set => _minLength = Math.Max( MIN_LENGTH, value ); }
    public bool                                         MustBeTrimmed            { get;               set; } = true;
    public string                                       Numbers                  { get;               set; } = Randoms.NUMERIC;
    public bool                                         RequireLowerCase         { get;               set; } = true;
    public bool                                         RequireNumber            { get;               set; } = true;
    public bool                                         RequireSpecialChar       { get;               set; } = true;
    public bool                                         RequireUpperCase         { get;               set; } = true;
    public string                                       SpecialChars             { get;               set; } = Randoms.SPECIAL_CHARS;
    public string                                       UpperCase                { get;               set; } = Randoms.UPPER_CASE;
    PasswordRequirements IOptions<PasswordRequirements>.Value                    => this;


    public PasswordRequirements() => Current = this;
    public void              SetBlockedPasswords( IEnumerable<string> passwords ) => BlockedPasswords.Add( passwords );
    public PasswordValidator GetValidator()                                       => new(this);
}