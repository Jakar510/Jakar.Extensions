// Jakar.Extensions :: Jakar.Database
// 08/19/2022  12:29 AM

namespace Jakar.Database;


public interface ICollectionWrapper<TValue, TID> : IReadOnlyCollection<TID>, IEquatable<ICollectionWrapper<TValue, TID>>, IConvertible, ICollectionAlerts, ISpanFormattable, IDisposable where TValue : IUniqueID<TID>
                                                                                                                                                                                         where TID : struct, IComparable<TID>, IEquatable<TID>
{
    bool    IsEmpty    { get; }
    bool    IsNotEmpty { get; }
    string? Json       { get; set; }


    void Add( IEnumerable<TValue>?       value );
    void Add( params TValue[]?           value );
    void Add( HashSet<TValue>?           value );
    bool Add( TValue                     value );
    void Add( IDCollection<TValue, TID>? value );
    bool Remove( TValue                  value );
    bool Contains( TValue                value );
    void SetValues( string?              json, [CallerMemberName] string? caller = default );


    string ToJson();
    string ToPrettyJson();

    /// <summary> Gets the JSON representation of this collection </summary>
    string ToString();
    string ToString( ReadOnlySpan<char> format, IFormatProvider? _ );
}
