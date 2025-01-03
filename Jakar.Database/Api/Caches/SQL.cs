// Jakar.Extensions :: Jakar.Database
// 10/15/2023  1:13 PM

namespace Jakar.Database;


public static class SQL // TODO: move to Jakar.Extensions.Sizes
{
    public const string AND                   = " AND ";
    public const int    ANSI_CAPACITY         = 8000;
    public const int    ANSI_TEXT_CAPACITY    = 2_147_483_647;
    public const int    BINARY_CAPACITY       = ANSI_TEXT_CAPACITY;
    public const string COUNT                 = "count";
    public const int    DECIMAL_MAX_PRECISION = 38;
    public const int    DECIMAL_MAX_SCALE     = 29;
    public const string GUID_FORMAT           = "D";
    public const string LIST_SEPARATOR        = ", ";
    public const string OR                    = " OR ";
    public const char   QUOTE                 = '"';
    public const int    UNICODE_CAPACITY      = 4000;
    public const int    UNICODE_TEXT_CAPACITY = 1_073_741_823;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetAndOr( this bool matchAll ) => matchAll
                                                               ? AND
                                                               : OR;


    public static int GetHash32( this DynamicParameters parameters )
    {
        HashCode code = new();
        foreach ( string parameterName in parameters.ParameterNames ) { code.Add( parameterName ); }

        return code.ToHashCode();
    }
    public static ulong   GetHash64( this  DynamicParameters parameters ) => Hashes.Hash( parameters.ParameterNames.ToArray() );
    public static UInt128 GetHash128( this DynamicParameters parameters ) => Hashes.Hash128( parameters.ParameterNames.ToArray() );
}
