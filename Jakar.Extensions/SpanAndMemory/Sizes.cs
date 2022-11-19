namespace Jakar.Extensions;


public static class Sizes
{
    public static bool Register( Type type, int size ) => _sizes.TryAdd( type, size );
    private static readonly ConcurrentDictionary<Type, int> _sizes = new()
                                                                     {
                                                                         [typeof(byte)]           = 3,
                                                                         [typeof(sbyte)]          = 3,
                                                                         [typeof(short)]          = 5,
                                                                         [typeof(ushort)]         = 5,
                                                                         [typeof(int)]            = 10,
                                                                         [typeof(uint)]           = 10,
                                                                         [typeof(long)]           = 19,
                                                                         [typeof(ulong)]          = 20,
                                                                         [typeof(float)]          = 13,
                                                                         [typeof(double)]         = 23,
                                                                         [typeof(decimal)]        = 29,
                                                                         [typeof(TimeSpan)]       = 50,
                                                                         [typeof(DateTime)]       = 50,
                                                                         [typeof(DateTimeOffset)] = 75,
                                                                         [typeof(AppVersion)]     = 65,
                                                                     };


    public static int GetSize<T>() => GetSize( typeof(T) );
    public static int GetSize<T>( this T _ ) => typeof(T).GetSize();
    public static int GetSize( this Type type ) => _sizes.TryGetValue( type, out int value )
                                                       ? value
                                                       : default;
}
