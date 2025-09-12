// Jakar.Extensions :: Jakar.Extensions
// 09/07/2022  3:56 PM


namespace Jakar.Extensions;


[DefaultValue( nameof(Default) ), SuppressMessage( "ReSharper", "OutParameterValueIsAlwaysDiscarded.Global" )]
public readonly ref struct PasswordValidator
{
    private readonly Requirements _requirements;

    public static PasswordRequirements Requirements { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => PasswordRequirements.Current; [MethodImpl( MethodImplOptions.AggressiveInlining )] set => PasswordRequirements.Current = value; }
    public static PasswordValidator    Default      => new(Requirements);
    public PasswordValidator() => throw new InvalidOperationException( "Use the constructor with Requirements" );
    public PasswordValidator( scoped in Requirements requirements ) => _requirements = requirements;


    public static bool Check( scoped in ReadOnlySpan<char> password ) => Check( in password, Requirements );
    public static bool Check( scoped in ReadOnlySpan<char> password, scoped in Requirements requirements )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        PasswordValidator   validator     = new(requirements);
        return validator.Validate( password );
    }


    public bool Validate( scoped in ReadOnlySpan<char> span ) => Validate( in span, out _ );
    public bool Validate( scoped in ReadOnlySpan<char> span, out Results results )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        ReadOnlySpan<char>  password      = span.Trim();
        bool                lengthPassed  = password.Length                                                                   >= _requirements.minLength && password.Length < _requirements.minLength;
        bool                mustBeTrimmed = !_requirements.mustBeTrimmed    || password.Length                                == span.Length;
        bool                lowerPassed   = !_requirements.requireLowerCase || password.IndexOfAny( _requirements.lowerCase ) >= 0;
        bool                upperPassed   = !_requirements.requireUpperCase || password.IndexOfAny( _requirements.upperCase ) >= 0;
        bool                specialPassed;
        bool                numericPassed;
        bool                blockedPassed;


        if ( _requirements.requireSpecialChar )
        {
            int index = password.IndexOfAny( _requirements.specialChars );

            specialPassed = _requirements.cantStartWithSpecialChar
                                ? index >= 1
                                : index >= 0;
        }
        else { specialPassed = true; }


        if ( _requirements.requireNumber )
        {
            int index = password.IndexOfAny( _requirements.numbers );

            numericPassed = _requirements.cantStartWithNumber
                                ? index >= 1
                                : index >= 0;
        }
        else { numericPassed = true; }


        if ( !_requirements.blockedPasswords.IsEmpty )
        {
            blockedPassed = true;

            foreach ( ReadOnlySpan<char> blocked in _requirements.blockedPasswords )
            {
                if ( !password.Equals( blocked, StringComparison.OrdinalIgnoreCase ) ) { continue; }

                blockedPassed = false;
                break;
            }
        }
        else { blockedPassed = true; }


        results = new Results( lengthPassed, mustBeTrimmed, specialPassed, numericPassed, lowerPassed, upperPassed, blockedPassed );
        return results.HasPassed;
    }



    public readonly struct Results( bool lengthPassed, bool mustBeTrimmed, bool specialPassed, bool numericPassed, bool lowerPassed, bool upperPassed, bool blockedPassed )
    {
        public readonly bool HasPassed     = lengthPassed && mustBeTrimmed && specialPassed && numericPassed && lowerPassed && upperPassed && blockedPassed;
        public readonly bool LengthPassed  = lengthPassed;
        public readonly bool MustBeTrimmed = mustBeTrimmed;
        public readonly bool SpecialPassed = specialPassed;
        public readonly bool NumericPassed = numericPassed;
        public readonly bool LowerPassed   = lowerPassed;
        public readonly bool UpperPassed   = upperPassed;
        public readonly bool BlockedPassed = blockedPassed;
        public static   bool operator true( Results  x ) => x.HasPassed;
        public static   bool operator false( Results x ) => !x.HasPassed;
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
    public readonly ReadOnlySpan<string> blockedPasswords         = blockedPasswords;
    public readonly int                  minLength                = minLength;
    public readonly int                  maxLength                = maxLength;
    public readonly bool                 requireLowerCase         = requireLowerCase;
    public readonly ReadOnlySpan<char>   lowerCase                = lowerCase;
    public readonly bool                 mustBeTrimmed            = mustBeTrimmed;
    public readonly bool                 requireUpperCase         = requireUpperCase;
    public readonly ReadOnlySpan<char>   upperCase                = upperCase;
    public readonly bool                 cantStartWithNumber      = cantStartWithNumber;
    public readonly bool                 requireNumber            = requireNumber;
    public readonly ReadOnlySpan<char>   numbers                  = numbers;
    public readonly bool                 requireSpecialChar       = requireSpecialChar;
    public readonly bool                 cantStartWithSpecialChar = cantStartWithSpecialChar;
    public readonly ReadOnlySpan<char>   specialChars             = specialChars;


    public static Requirements Default { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => PasswordRequirements.Current; }


    public static implicit operator Requirements( PasswordRequirements data ) => new(data.BlockedPasswords,
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


    public string[]                                     BlockedPasswords         { get;               set; } = [];
    public bool                                         CantStartWithNumber      { get;               set; } = true;
    public bool                                         CantStartWithSpecialChar { get;               set; } = true;
    public string                                       LowerCase                { get;               set; } = Randoms.LOWER_CASE;
    public int                                          MaxLength                { get => _maxLength; set => _maxLength = Math.Clamp( value, _minLength, MAX_LENGTH ); }
    public int                                          MinLength                { get => _minLength; set => _minLength = Math.Clamp( value, MIN_LENGTH, _maxLength ); }
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
    public PasswordValidator GetValidator() => new(this);


    public void SetBlockedPasswords( IEnumerable<string> passwords )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        BlockedPasswords = new HashSet<string>( passwords ).ToArray();
    }
    public void SetBlockedPasswords( string content )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        try { SetBlockedPasswords( content.FromJson<string[]>() ); }
        catch ( Exception ) { SetBlockedPasswords( content.SplitAndTrimLines() ); }
    }
    public async ValueTask SetBlockedPasswords( Uri uri, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using HttpClient    client        = new();
        client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( MimeTypeNames.Application.JSON ) );
        client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( MimeTypeNames.Text.PLAIN ) );
        await SetBlockedPasswords( client, uri, token );
    }
    public async ValueTask SetBlockedPasswords( HttpClient client, Uri uri, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        string              content       = await client.GetStringAsync( uri, token );
        SetBlockedPasswords( content );
    }
    public async ValueTask SetBlockedPasswords( LocalFile file, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        SetBlockedPasswords( await file.ReadAsync().AsString( token ) );
    }
}
