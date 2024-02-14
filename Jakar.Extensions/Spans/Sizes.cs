namespace Jakar.Extensions;


public static class Sizes
{
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
    private const int DEFAULT_SIZE = 500;


    public static bool Register( Type              type, int size ) => _sizes.TryAdd( type, size );
    public static int  GetBufferSize<T>( int       defaultSize           = DEFAULT_SIZE ) => _sizes.GetValueOrDefault( typeof(T), defaultSize );
    public static int  GetBufferSize<T>( this T    _,    int defaultSize = DEFAULT_SIZE ) => _sizes.GetValueOrDefault( typeof(T), defaultSize );
    public static int  GetBufferSize( this    Type type, int defaultSize = DEFAULT_SIZE ) => _sizes.GetValueOrDefault( type,      defaultSize );
}
