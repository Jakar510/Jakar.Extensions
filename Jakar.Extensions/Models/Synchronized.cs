// **********************************************
// *** Synchronized access wrapper class V1.0 ***
// **********************************************
// *** (C)2009 S.T.A. snc                     ***
// **********************************************


namespace Jakar.Extensions;


/// <summary> <seealso href="https://www.codeproject.com/Articles/33559/Handy-wrapper-class-for-thread-safe-property-acces"/> </summary>
/// <typeparam name="TValue"> The value type. </typeparam>
public sealed class Synchronized<TValue>
{
    private TValue _value;

    public TValue Value
    {
        get
        {
            lock (this) { return _value; }
        }
        set
        {
            lock (this) { _value = value; }
        }
    }


    public Synchronized( TValue value ) => _value = value;


    public static implicit operator TValue( Synchronized<TValue> value ) => value.Value;
}
