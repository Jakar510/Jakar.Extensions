// **********************************************
// *** Synchronized access wrapper class V1.0 ***
// **********************************************
// *** (C)2009 S.T.A. snc                     ***
// **********************************************


#nullable enable
namespace Jakar.Extensions;


/// <summary>
///     <seealso href = "https://www.codeproject.com/Articles/33559/Handy-wrapper-class-for-thread-safe-property-acces" />
/// </summary>
/// <typeparam name = "TStruct" > The value type. </typeparam>
public class Synchronized<TStruct> where TStruct : struct
{
    private readonly object _lock;

    private TStruct _value;

    public TStruct Value
    {
        get
        {
            lock (_lock) { return _value; }
        }
        set
        {
            lock (_lock) { _value = value; }
        }
    }


    public Synchronized() : this( default ) { }
    public Synchronized( TStruct value ) : this( value, new object() ) { }

    public Synchronized( TStruct value, object @lock )
    {
        _lock = @lock;
        Value = value;
    }


    // public static implicit operator Synchronized<TStruct>( TStruct value ) => value.Value;
    public static implicit operator TStruct( Synchronized<TStruct> value ) => value.Value;
}
