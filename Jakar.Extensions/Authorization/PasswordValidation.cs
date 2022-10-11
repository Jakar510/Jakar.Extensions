// Jakar.Extensions :: Jakar.Extensions
// 09/07/2022  3:56 PM


namespace Jakar.Extensions;


public readonly ref struct PasswordValidator
{
    public static JsonData? Current { get; set; }
    public static LocalFile File    { get; set; } = "PasswordRequirements.json";


    private readonly Requirements _requirements;


    public PasswordValidator() : this( Requirements.Default ) { }
    public PasswordValidator( in Requirements requirements ) => _requirements = requirements;


    public bool Validate( in ReadOnlySpan<char> password )
    {
        ReadOnlySpan<char> span = password.Trim();
        return HandleLength( span ) && HandleSpecial( span ) && HandleNumeric( span ) && HandleLower( span ) && HandleUpper( span ) && HandleBlocked( span );
    }
    private bool HandleLength( in ReadOnlySpan<char> span ) => span.Length >= _requirements.MinLength;
    private bool HandleNumeric( in ReadOnlySpan<char> span )
    {
        if (!_requirements.RequireNumber) { return true; }

        int index = _requirements.CantStartWithNumber
                        ? 1
                        : 0;

        return span.IndexOfAny( _requirements.Numbers ) >= index;
    }
    private bool HandleSpecial( in ReadOnlySpan<char> span )
    {
        if (!_requirements.RequireSpecialChar) { return true; }

        int index = _requirements.CantStartWithSpecialChar
                        ? 1
                        : 0;

        return span.IndexOfAny( _requirements.SpecialChars ) >= index;
    }
    private bool HandleUpper( in ReadOnlySpan<char> span )
    {
        if (!_requirements.RequireUpperCase) { return true; }

        return span.IndexOfAny( _requirements.UpperCase ) >= 0;
    }
    private bool HandleLower( in ReadOnlySpan<char> span )
    {
        if (!_requirements.RequireLowerCase) { return true; }

        return span.IndexOfAny( _requirements.LowerCase ) >= 0;
    }
    private bool HandleBlocked( in ReadOnlySpan<char> span )
    {
        if (_requirements.BlockedPasswords.IsEmpty) { return true; }

        foreach (ReadOnlySpan<char> password in _requirements.BlockedPasswords)
        {
            if (span.Equals( password, StringComparison.OrdinalIgnoreCase )) { return false; }
        }

        return true;
    }


    public static async ValueTask<bool> CheckAsync( string password )
    {
        JsonData data = await JsonData.FromFile();
        return Check( password, data );
    }
    public static bool Check( in ReadOnlySpan<char> password ) => Check( password, Requirements.Default );
    public static bool Check( in ReadOnlySpan<char> password, in Requirements requirements )
    {
        var validator = new PasswordValidator( requirements );
        return validator.Validate( password );
    }



    [SuppressMessage( "ReSharper", "SuggestBaseTypeForParameterInConstructor" )]
    public readonly ref struct Requirements
    {
        public ReadOnlySpan<string> BlockedPasswords         { get; init; }
        public int                  MinLength                { get; init; }
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


        public static Requirements Default => Current ??= new JsonData();


        public Requirements( string[] blockedPasswords ) : this( 10, blockedPasswords ) { }
        public Requirements( int minLength, string[] blockedPasswords )
        {
            MinLength = minLength;

            BlockedPasswords = blockedPasswords;
        }
        public Requirements( JsonData data ) : this( data.MinLength, data.BlockedPasswords )
        {
            MinLength                = data.MinLength;
            RequireLowerCase         = data.RequireLowerCase;
            LowerCase                = data.LowerCase;
            RequireUpperCase         = data.RequireUpperCase;
            UpperCase                = data.UpperCase;
            CantStartWithNumber      = data.CantStartWithNumber;
            RequireNumber            = data.RequireNumber;
            Numbers                  = data.Numbers;
            RequireSpecialChar       = data.RequireSpecialChar;
            CantStartWithSpecialChar = data.CantStartWithSpecialChar;
            SpecialChars             = data.SpecialChars;
        }


        public static implicit operator Requirements( JsonData        data ) => new(data);
        public static implicit operator Requirements( string[]        blockedPasswords ) => new(blockedPasswords);
        public static implicit operator Requirements( HashSet<string> blockedPasswords ) => new(Filter( blockedPasswords ));


        public static string[] Filter( IEnumerable<string> blockedPasswords, int minLength = 10 ) => Filter( new HashSet<string>( blockedPasswords ), minLength );
        public static string[] Filter( HashSet<string> blockedPasswords, int minLength = 10 ) => blockedPasswords.Select( x => x.Trim() )
                                                                                                                 .Where( x => x.Length >= minLength )
                                                                                                                 .ToArray();


        public JsonData ToJsonData() => new(this);
        public override string ToString() => ToJsonData()
           .ToPrettyJson();
    }



    public sealed record JsonData : BaseRecord
    {
        public bool     CantStartWithNumber      { get; init; } = true;
        public bool     CantStartWithSpecialChar { get; init; } = true;
        public bool     RequireLowerCase         { get; init; } = true;
        public bool     RequireNumber            { get; init; } = true;
        public bool     RequireSpecialChar       { get; init; } = true;
        public bool     RequireUpperCase         { get; init; } = true;
        public int      MinLength                { get; init; }
        public string   LowerCase                { get; init; } = new(Randoms.LowerCase);
        public string   Numbers                  { get; init; } = new(Randoms.Numeric);
        public string   SpecialChars             { get; init; } = new(Randoms.SpecialChars);
        public string   UpperCase                { get; init; } = new(Randoms.UpperCase);
        public string[] BlockedPasswords         { get; init; } = Array.Empty<string>();


        public JsonData() { }
        public JsonData( in Requirements requirements )
        {
            BlockedPasswords         = Requirements.Filter( requirements.BlockedPasswords.ToArray() );
            MinLength                = requirements.MinLength;
            RequireLowerCase         = requirements.RequireLowerCase;
            LowerCase                = requirements.LowerCase.ToString();
            RequireUpperCase         = requirements.RequireUpperCase;
            UpperCase                = requirements.UpperCase.ToString();
            CantStartWithNumber      = requirements.CantStartWithNumber;
            RequireNumber            = requirements.RequireNumber;
            Numbers                  = requirements.Numbers.ToString();
            RequireSpecialChar       = requirements.RequireSpecialChar;
            CantStartWithSpecialChar = requirements.CantStartWithSpecialChar;
            SpecialChars             = requirements.SpecialChars.ToString();
        }


        public static async ValueTask<JsonData> FromFile()
        {
            if (File.DoesNotExist)
            {
                Current ??= new JsonData();
                await File.WriteAsync( Current.ToPrettyJson() );
            }

            Current ??= await File.ReadAsync()
                                  .AsJson<JsonData>();

            return Current;
        }
    }
}
