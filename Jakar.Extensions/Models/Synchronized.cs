// **********************************************
// *** Synchronized access wrapper class V1.0 ***
// **********************************************
// *** (C)2009 S.TValue.A. snc                     ***
// **********************************************


namespace Jakar.Extensions;


/// <summary>
///     <seealso href="https://www.codeproject.com/Articles/33559/Handy-wrapper-class-for-thread-safe-property-acces"/>
/// </summary>
/// <typeparam name="TValue"> The value type. </typeparam>
public sealed class Synchronized<TValue>( TValue value )
    where TValue : class?
{
    private volatile TValue __value = value;

    public TValue Value { get => Interlocked.CompareExchange(ref __value!, null, null); set => Interlocked.Exchange(ref __value, value); }


    public static implicit operator TValue( Synchronized<TValue> value ) => value.Value;
}



public sealed class SynchronizedValue<TValue>( TValue value )
{
    private readonly Lock   __lock  = new();

    public TValue Value
    {
        get
        {
            lock ( __lock ) { return field; }
        }
        set
        {
            lock ( __lock ) { field = value; }
        }
    } = value;


    public static implicit operator TValue( SynchronizedValue<TValue> value ) => value.Value;
}
