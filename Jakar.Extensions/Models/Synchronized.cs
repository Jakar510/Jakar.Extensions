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
    private TValue _value = value;
    
    public TValue Value { get => Interlocked.CompareExchange( ref _value!, default, default ); set { Interlocked.Exchange( ref _value, value ); } }


    public static implicit operator TValue( Synchronized<TValue> value ) => value.Value;
}
