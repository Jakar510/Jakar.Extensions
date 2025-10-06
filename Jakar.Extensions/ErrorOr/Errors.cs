// Jakar.Extensions :: Jakar.Extensions
// 04/23/2024  11:04

using ZLinq;
using ZLinq.Linq;



namespace Jakar.Extensions;


[Serializable]
[DefaultValue(nameof(Empty))]
[method: JsonConstructor]
public sealed class Errors() : BaseClass<Errors>, IEqualComparable<Errors>, IJsonModel<Errors>, IValueEnumerable<FromArray<Error>, Error>
{
    private static readonly Error[] __details = [];
    public static readonly Errors Empty = new()
                                          {
                                              Alert   = null,
                                              Details = __details
                                          };


    public static                  JsonSerializerContext  JsonContext   => JakarExtensionsContext.Default;
    public static                  JsonTypeInfo<Errors>   JsonTypeInfo  => JakarExtensionsContext.Default.Errors;
    public static                  JsonTypeInfo<Errors[]> JsonArrayInfo => JakarExtensionsContext.Default.ErrorsArray;
    [JsonRequired] public required Alert?                 Alert         { get; init; }
    public                         string                 Description   => Details.GetMessage();
    [JsonRequired] public required Error[]                Details       { get; init; }
    public                         bool                   IsValid       => Alert?.IsValid is true || ( !ReferenceEquals(Details, __details) && Details.Length > 0 );


    public static Errors Create( params Error[]? details ) => Create(null,                                     details);
    public static Errors Create( Error           details ) => Create(new Alert(details.Title, details.Detail), details);
    public static Errors Create( Alert           details ) => Create(details,                                  details.ToError());
    public static Errors Create( Alert? alert, params Error[]? details ) => new()
                                                                            {
                                                                                Alert   = alert,
                                                                                Details = details ?? __details
                                                                            };


    public        ValueEnumerable<FromArray<Error>, Error> AsValueEnumerable()         => new(new FromArray<Error>(Details));
    public        Status                                   GetStatus()                 => Details.GetStatus(Status.Ok);
    public static Status                                   GetStatus( Errors? errors ) => errors?.GetStatus() ?? Status.Ok;


    public static implicit operator ReadOnlySpan<Error>( Errors   result )  => result.Details;
    public static implicit operator ReadOnlyMemory<Error>( Errors result )  => result.Details;
    public static implicit operator Errors( string                title )   => Create(new Alert(title));
    public static implicit operator Errors( Alert                 details ) => Create(details);
    public static implicit operator Errors( Error                 details ) => Create(details);
    public static implicit operator Errors( Error[]               details ) => Create(null, details);
    public static implicit operator Errors( List<Error>           details ) => Create(details.ToArray());
    public static implicit operator Errors( ReadOnlySpan<Error>   details ) => Create(details.ToArray());


    public override bool Equals( Errors? other )
    {
        if ( other is null ) { return false; }

        return ReferenceEquals(this, other) || ( Nullable.Equals(Alert, other.Alert) && Error.Equals(Details, other.Details) );
    }
    public override int  CompareTo( Errors? other )                 => string.CompareOrdinal(Description, other?.Description);
    public override bool Equals( object?    obj )                   => ReferenceEquals(this, obj) || ( obj is Errors other && Equals(other) );
    public override int  GetHashCode()                              => HashCode.Combine(Alert, Details);
    public static   bool operator ==( Errors? left, Errors? right ) => EqualityComparer<Errors>.Default.Equals(left, right);
    public static   bool operator !=( Errors? left, Errors? right ) => !EqualityComparer<Errors>.Default.Equals(left, right);
    public static   bool operator >( Errors   left, Errors  right ) => Comparer<Errors>.Default.Compare(left, right) > 0;
    public static   bool operator >=( Errors  left, Errors  right ) => Comparer<Errors>.Default.Compare(left, right) >= 0;
    public static   bool operator <( Errors   left, Errors  right ) => Comparer<Errors>.Default.Compare(left, right) < 0;
    public static   bool operator <=( Errors  left, Errors  right ) => Comparer<Errors>.Default.Compare(left, right) <= 0;
}
