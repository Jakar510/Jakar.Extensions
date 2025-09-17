namespace Jakar.Extensions;


public interface ICurrentLocation<out TID> : IUniqueID<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    double?           Accuracy                { get; }
    double?           Altitude                { get; }
    AltitudeReference AltitudeReferenceSystem { get; }
    double?           Course                  { get; }
    Guid              InstanceID              { get; }
    bool              IsFromMockProvider      { get; }
    double            Latitude                { get; }
    double            Longitude               { get; }
    double?           Speed                   { get; }
    DateTimeOffset    Timestamp               { get; }
    double?           VerticalAccuracy        { get; }
}
