// Jakar.Extensions :: Jakar.Database
// 08/19/2022  12:29 AM

namespace Jakar.Database;


public interface ICollectionWrapper<TValue> : IReadOnlyCollection<string>, IEquatable<ICollectionWrapper<TValue>>, IConvertible, ICollectionAlerts, ISpanFormattable, IDisposable
    where TValue : IUniqueID<string>

{
    bool    IsEmpty    { get; }
    bool    IsNotEmpty { get; }
    string? Json       { get; set; }
    bool    Add( TValue      value );
    bool    Contains( TValue value );
    bool    Remove( TValue   value );


    string ToJson();
    string ToPrettyJson();

    /// <summary> Gets the JSON representation of this collection </summary>
    string ToString();
    string ToString( ReadOnlySpan<char> format, IFormatProvider? _ );


    void Add( IEnumerable<TValue>?        value );
    void Add( params TValue[]?            value );
    void Add( HashSet<TValue>?            value );
    void Add( ICollectionWrapper<TValue>? value );
    void SetValues( string?               json, [CallerMemberName] string? caller = default );
}
