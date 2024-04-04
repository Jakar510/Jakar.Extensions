namespace Jakar.Extensions;


public static class Sizes
{
    public const  int ANSI_CAPACITY    = 8000;
    public const  int ANSI_TEXT_CAPACITY      = 2_147_483_647;
    public const  int BINARY_CAPACITY         = ANSI_TEXT_CAPACITY;
    public const  int DECIMAL_MAX_CAPACITY    = 38;
    public const  int DECIMAL_MAX_SCALE       = 28;
    private const int DEFAULT_SIZE            = 500;
    public const  int UNICODE_CAPACITY = 4000;
    public const  int UNICODE_TEXT_CAPACITY   = 1_073_741_823;
    private static readonly ConcurrentDictionary<Type, int> _sizes = new(Environment.ProcessorCount, 50)
                                                                     {
                                                                         [typeof(byte)]            = 3,
                                                                         [typeof(byte?)]           = 3,
                                                                         [typeof(sbyte)]           = 3,
                                                                         [typeof(sbyte?)]          = 3,
                                                                         [typeof(short)]           = 5,
                                                                         [typeof(short?)]          = 5,
                                                                         [typeof(ushort)]          = 5,
                                                                         [typeof(ushort?)]         = 5,
                                                                         [typeof(int)]             = 10,
                                                                         [typeof(int?)]            = 10,
                                                                         [typeof(uint)]            = 10,
                                                                         [typeof(uint?)]           = 10,
                                                                         [typeof(long)]            = 19,
                                                                         [typeof(long?)]           = 19,
                                                                         [typeof(ulong)]           = 20,
                                                                         [typeof(ulong?)]          = 20,
                                                                         [typeof(float)]           = 13,
                                                                         [typeof(float?)]          = 13,
                                                                         [typeof(double)]          = 23,
                                                                         [typeof(double?)]         = 23,
                                                                         [typeof(decimal)]         = 29,
                                                                         [typeof(decimal?)]        = 29,
                                                                         [typeof(TimeSpan)]        = 50,
                                                                         [typeof(TimeSpan?)]       = 50,
                                                                         [typeof(DateTime)]        = 50,
                                                                         [typeof(DateTime?)]       = 50,
                                                                         [typeof(DateTimeOffset)]  = 75,
                                                                         [typeof(DateTimeOffset?)] = 75,
                                                                         [typeof(AppVersion)]      = 200
                                                                     };


    public static bool Register( Type              type, int size ) => _sizes.TryAdd( type, size );
    public static int  GetBufferSize<T>( int       defaultSize           = DEFAULT_SIZE ) => _sizes.GetValueOrDefault( typeof(T), defaultSize );
    public static int  GetBufferSize<T>( this T    _,    int defaultSize = DEFAULT_SIZE ) => _sizes.GetValueOrDefault( typeof(T), defaultSize );
    public static int  GetBufferSize( this    Type type, int defaultSize = DEFAULT_SIZE ) => _sizes.GetValueOrDefault( type,      defaultSize );
}
