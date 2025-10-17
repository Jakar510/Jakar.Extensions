namespace Jakar.Extensions;


public static class Sizes
{
    private const int DEFAULT_SIZE = 500;


    private static readonly ConcurrentDictionary<Type, int> __sizes = new(Environment.ProcessorCount, DEFAULT_CAPACITY, TypeEqualityComparer.Instance)
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
                                                                          [typeof(TimeOnly)]        = 50,
                                                                          [typeof(TimeOnly?)]       = 50,
                                                                          [typeof(TimeSpan)]        = 50,
                                                                          [typeof(TimeSpan?)]       = 50,
                                                                          [typeof(DateOnly)]        = 50,
                                                                          [typeof(DateOnly?)]       = 50,
                                                                          [typeof(DateTime)]        = 50,
                                                                          [typeof(DateTime?)]       = 50,
                                                                          [typeof(DateTimeOffset)]  = 75,
                                                                          [typeof(DateTimeOffset?)] = 75,
                                                                          [typeof(AppVersion)]      = 200,
                                                                          [typeof(Int128)] = Int128.MaxValue.ToString()
                                                                                                   .Length,
                                                                          [typeof(UInt128)] = UInt128.MaxValue.ToString()
                                                                                                     .Length,
                                                                          [typeof(Int128?)] = Int128.MaxValue.ToString()
                                                                                                    .Length,
                                                                          [typeof(UInt128?)] = UInt128.MaxValue.ToString()
                                                                                                      .Length,
                                                                      };


    public static bool Register( Type           type, int size ) => __sizes.TryAdd(type, size);
    public static bool Register<TValue>( int    size )        => Register(typeof(TValue), size);
    public static bool Register<TValue>( TValue _, int size ) => Register<TValue>(size);


    public static int GetBufferSize<TValue>( int         defaultSize           = DEFAULT_SIZE ) => __sizes.GetValueOrDefault(typeof(TValue), defaultSize);
    public static int GetBufferSize<TValue>( this TValue _,    int defaultSize = DEFAULT_SIZE ) => __sizes.GetValueOrDefault(typeof(TValue), defaultSize);
    public static int GetBufferSize( this         Type   type, int defaultSize = DEFAULT_SIZE ) => __sizes.GetValueOrDefault(type,           defaultSize);
}
