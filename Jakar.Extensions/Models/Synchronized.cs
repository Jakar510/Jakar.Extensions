// **********************************************
// *** Synchronized access wrapper class V1.0 ***
// **********************************************
// *** (C)2009 S.T.A. snc                     ***
// **********************************************


namespace Jakar.Extensions;


/// <summary>
///     <seealso href="https://www.codeproject.com/Articles/33559/Handy-wrapper-class-for-thread-safe-property-acces"/>
/// </summary>
/// <typeparam name="TValue"> The value type. </typeparam>
public sealed class Synchronized<TValue>( TValue value ) 
{
    private readonly Lock _lock = new();
    private TValue _value = value;

    public TValue Value
    {
        get
        {
            lock (_lock ) { return _value; }
        }
        set
        {
            lock (_lock ) { _value = value; }
        }
    }


    public static implicit operator TValue( Synchronized<TValue> value ) => value.Value;
}
