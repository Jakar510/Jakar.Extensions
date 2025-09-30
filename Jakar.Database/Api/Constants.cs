// Jakar.Extensions :: Jakar.Database
// 10/15/2023  1:13 PM

namespace Jakar.Database;


public static class Constants // TODO: move to Jakar.Extensions.Sizes
{
    public const string AND                   = " AND ";
    public const string COUNT                 = "count";
    public const string GUID_FORMAT           = "D";
    public const string LIST_SEPARATOR        = ", ";
    public const string OR                    = " OR ";
    public const char   QUOTE                 = '"';
    public const int    ANSI_CAPACITY         = BaseRecord.ANSI_CAPACITY;
    public const int    ANSI_TEXT_CAPACITY    = BaseRecord.ANSI_TEXT_CAPACITY;
    public const int    BINARY_CAPACITY       = ANSI_TEXT_CAPACITY;
    public const int    DECIMAL_MAX_PRECISION = 38;
    public const int    DECIMAL_MAX_SCALE     = 29;
    public const string EMPTY                 = BaseRecord.EMPTY;
    public const int    MAX_STRING_SIZE       = int.MaxValue;
    public const string NULL                  = BaseRecord.NULL;
    public const int    UNICODE_CAPACITY      = BaseRecord.UNICODE_CAPACITY;
    public const int    UNICODE_TEXT_CAPACITY = BaseRecord.UNICODE_TEXT_CAPACITY;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static string GetAndOr( this bool matchAll ) => matchAll
                                                                                                                  ? AND
                                                                                                                  : OR;


    public static int GetHash32( this DynamicParameters parameters )
    {
        HashCode code = new();
        foreach ( string parameterName in parameters.ParameterNames ) { code.Add(parameterName); }

        return code.ToHashCode();
    }
    public static ulong GetHash64( this DynamicParameters parameters )
    {
        ReadOnlySpan<string> values = parameters.ParameterNames.ToArray();
        return Hashes.Hash(in values);
    }
    public static UInt128 GetHash128( this DynamicParameters parameters )
    {
        ReadOnlySpan<string> values = parameters.ParameterNames.ToArray();
        return Hashes.Hash128(in values);
    }
}
