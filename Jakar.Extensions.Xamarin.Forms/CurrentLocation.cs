#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public interface ICurrentLocation<TID> : IUniqueID<TID>, IEquatable<ICurrentLocation<TID>> where TID : struct, IComparable<TID>, IEquatable<TID>
{
    AltitudeReferenceSystem AltitudeReferenceSystem { get; }
    bool                    IsFromMockProvider      { get; }
    DateTimeOffset          Timestamp               { get; }
    double                  Latitude                { get; }
    double                  Longitude               { get; }
    double?                 Accuracy                { get; }
    double?                 Altitude                { get; }
    double?                 Course                  { get; }
    double?                 Speed                   { get; }
    double?                 VerticalAccuracy        { get; }
    Guid                    InstanceID              { get; }
}



[Serializable]
public class CurrentLocation<TID> : ICurrentLocation<TID>, IDataBaseIgnore where TID : struct, IComparable<TID>, IEquatable<TID>
{
    public       AltitudeReferenceSystem AltitudeReferenceSystem { get; init; }
    public       bool                    IsFromMockProvider      { get; init; }
    public       DateTimeOffset          Timestamp               { get; init; }
    public       double                  Latitude                { get; init; }
    public       double                  Longitude               { get; init; }
    public       double?                 Accuracy                { get; init; }
    public       double?                 Altitude                { get; init; }
    public       double?                 Course                  { get; init; }
    public       double?                 Speed                   { get; init; }
    public       double?                 VerticalAccuracy        { get; init; }
    public       Guid                    InstanceID              { get; init; } = Guid.Empty;
    [Key] public TID                     ID                      { get; init; }
    public CurrentLocation() { }
    public CurrentLocation( ICurrentLocation<TID> point )
    {
        Latitude                = point.Latitude;
        Longitude               = point.Longitude;
        Timestamp               = point.Timestamp;
        Altitude                = point.Altitude;
        Accuracy                = point.Accuracy;
        VerticalAccuracy        = point.VerticalAccuracy;
        Speed                   = point.Speed;
        Course                  = point.Course;
        IsFromMockProvider      = point.IsFromMockProvider;
        AltitudeReferenceSystem = point.AltitudeReferenceSystem;
        ID                      = point.ID;
    }
    private CurrentLocation( Location point )
    {
        InstanceID              = Guid.NewGuid();
        Latitude                = point.Latitude;
        Longitude               = point.Longitude;
        Timestamp               = point.Timestamp;
        Altitude                = point.Altitude;
        Accuracy                = point.Accuracy;
        VerticalAccuracy        = point.VerticalAccuracy;
        Speed                   = point.Speed;
        Course                  = point.Course;
        IsFromMockProvider      = point.IsFromMockProvider;
        AltitudeReferenceSystem = point.AltitudeReferenceSystem;
    }


    public static double CalculateDistance( double latitudeStart, double longitudeStart, ICurrentLocation<TID> locationEnd, DistanceUnits units ) =>
        CalculateDistance( latitudeStart, longitudeStart, locationEnd.Latitude, locationEnd.Longitude, units );

    public static double CalculateDistance( ICurrentLocation<TID> locationStart, double latitudeEnd, double longitudeEnd, DistanceUnits units ) => CalculateDistance( locationStart.Latitude, locationStart.Longitude, latitudeEnd, longitudeEnd, units );

    public static double CalculateDistance( ICurrentLocation<TID> locationStart, ICurrentLocation<TID> locationEnd, DistanceUnits units ) =>
        CalculateDistance( locationStart.Latitude, locationStart.Longitude, locationEnd.Latitude, locationEnd.Longitude, units );

    public static double CalculateDistance( double latitudeStart, double longitudeStart, double latitudeEnd, double longitudeEnd, DistanceUnits units ) =>
        units switch
        {
            DistanceUnits.Kilometers => UnitConverters.CoordinatesToKilometers( latitudeStart, longitudeStart, latitudeEnd, longitudeEnd ),
            DistanceUnits.Miles      => UnitConverters.CoordinatesToMiles( latitudeStart, longitudeStart, latitudeEnd, longitudeEnd ),
            _                        => throw new ArgumentOutOfRangeException( nameof(units) )
        };


    /// <summary>
    ///     Check distance from starting to this location.
    /// </summary>
    /// <param name = "locationStart" > </param>
    /// <param name = "units" > </param>
    /// <returns>
    ///     Distance as a
    ///     <see cref = "double" />
    /// </returns>
    public double CalculateDistance( ICurrentLocation<TID> locationStart, DistanceUnits units ) => CalculateDistance( locationStart, this, units );

    /// <summary>
    ///     Check distance from starting to this location.
    /// </summary>
    /// <param name = "latitudeStart" > </param>
    /// <param name = "longitudeStart" > </param>
    /// <param name = "units" > </param>
    /// <returns>
    ///     Distance as a
    ///     <see cref = "double" />
    /// </returns>
    public double CalculateDistance( double latitudeStart, double longitudeStart, DistanceUnits units ) => CalculateDistance( latitudeStart, longitudeStart, this, units );

    public static async Task<CurrentLocation<TID>> Create( CancellationToken token, GeolocationAccuracy accuracy = GeolocationAccuracy.Best )
    {
        var      request  = new GeolocationRequest( accuracy );
        Location location = await Geolocation.GetLocationAsync( request, token );
        return new CurrentLocation<TID>( location );
    }


    public bool EqualInstance( ICurrentLocation<TID> other ) => InstanceID.Equals( other.InstanceID );


    public override bool Equals( object? obj )
    {
        if (obj is null) { return false; }

        if (ReferenceEquals( this, obj )) { return true; }

        return obj is CurrentLocation<TID> location && Equals( location );
    }
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add( ID );
        hashCode.Add( InstanceID );
        hashCode.Add( Timestamp );
        hashCode.Add( Latitude );
        hashCode.Add( Longitude );
        hashCode.Add( Altitude );
        hashCode.Add( Accuracy );
        hashCode.Add( VerticalAccuracy );
        hashCode.Add( Speed );
        hashCode.Add( Course );
        hashCode.Add( IsFromMockProvider );
        hashCode.Add( (int)AltitudeReferenceSystem );
        return hashCode.ToHashCode();
    }


    public bool IsValid( ICurrentLocation<TID> location, DistanceUnits units, double maxDistance )
    {
        if (InstanceID == Guid.Empty) { return false; }


        return CalculateDistance( this, location, units ) <= maxDistance;
    }


    public static implicit operator Location( CurrentLocation<TID> point ) => new()
                                                                              {
                                                                                  Latitude                = point.Latitude,
                                                                                  Longitude               = point.Longitude,
                                                                                  Timestamp               = point.Timestamp,
                                                                                  Altitude                = point.Altitude,
                                                                                  Accuracy                = point.Accuracy,
                                                                                  VerticalAccuracy        = point.VerticalAccuracy,
                                                                                  Speed                   = point.Speed,
                                                                                  Course                  = point.Course,
                                                                                  IsFromMockProvider      = point.IsFromMockProvider,
                                                                                  AltitudeReferenceSystem = point.AltitudeReferenceSystem
                                                                              };


    public bool Equals( ICurrentLocation<TID>? other )
    {
        if (other is null) { return false; }

        if (ReferenceEquals( this, other )) { return true; }

        return InstanceID.Equals( other.InstanceID ) && Timestamp.Equals( other.Timestamp ) && Latitude.Equals( other.Latitude ) && Longitude.Equals( other.Longitude ) && Nullable.Equals( Altitude, other.Altitude ) &&
               Nullable.Equals( Accuracy, other.Accuracy ) && Nullable.Equals( VerticalAccuracy, other.VerticalAccuracy ) && Nullable.Equals( Speed, other.Speed ) && Nullable.Equals( Course, other.Course ) &&
               IsFromMockProvider == other.IsFromMockProvider && AltitudeReferenceSystem == other.AltitudeReferenceSystem;
    }
}
