#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public interface ICurrentLocation : IEquatable<ICurrentLocation>
{
    double?                 Accuracy                { get; }
    double?                 Altitude                { get; }
    AltitudeReferenceSystem AltitudeReferenceSystem { get; }
    double?                 Course                  { get; }
    Guid                    InstanceID              { get; }
    bool                    IsFromMockProvider      { get; }
    double                  Latitude                { get; }
    double                  Longitude               { get; }
    double?                 Speed                   { get; }
    DateTimeOffset          Timestamp               { get; }
    double?                 VerticalAccuracy        { get; }
}



[Serializable]
public class CurrentLocation : ICurrentLocation, IDataBaseIgnore
{
    public double?                 Accuracy                { get; init; }
    public double?                 Altitude                { get; init; }
    public AltitudeReferenceSystem AltitudeReferenceSystem { get; init; }
    public double?                 Course                  { get; init; }
    public Guid                    InstanceID              { get; init; } = Guid.Empty;
    public bool                    IsFromMockProvider      { get; init; }
    public double                  Latitude                { get; init; }
    public double                  Longitude               { get; init; }
    public double?                 Speed                   { get; init; }
    public DateTimeOffset          Timestamp               { get; init; }
    public double?                 VerticalAccuracy        { get; init; }


    public CurrentLocation() { }
    public CurrentLocation( ICurrentLocation point )
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


    public static double CalculateDistance( double latitudeStart, double longitudeStart, ICurrentLocation locationEnd, DistanceUnits units ) =>
        CalculateDistance( latitudeStart, longitudeStart, locationEnd.Latitude, locationEnd.Longitude, units );

    public static double CalculateDistance( ICurrentLocation locationStart, double latitudeEnd, double longitudeEnd, DistanceUnits units ) => CalculateDistance( locationStart.Latitude, locationStart.Longitude, latitudeEnd, longitudeEnd, units );

    public static double CalculateDistance( ICurrentLocation locationStart, ICurrentLocation locationEnd, DistanceUnits units ) =>
        CalculateDistance( locationStart.Latitude, locationStart.Longitude, locationEnd.Latitude, locationEnd.Longitude, units );

    public static double CalculateDistance( double latitudeStart, double longitudeStart, double latitudeEnd, double longitudeEnd, DistanceUnits units ) =>
        units switch
        {
            DistanceUnits.Kilometers => UnitConverters.CoordinatesToKilometers( latitudeStart, longitudeStart, latitudeEnd, longitudeEnd ),
            DistanceUnits.Miles      => UnitConverters.CoordinatesToMiles( latitudeStart, longitudeStart, latitudeEnd, longitudeEnd ),
            _                        => throw new ArgumentOutOfRangeException( nameof(units) ),
        };


    public static implicit operator Location( CurrentLocation point ) => new()
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
                                                                             AltitudeReferenceSystem = point.AltitudeReferenceSystem,
                                                                         };

    public static async Task<CurrentLocation> Create( CancellationToken token, GeolocationAccuracy accuracy = GeolocationAccuracy.Best )
    {
        var      request  = new GeolocationRequest( accuracy );
        Location location = await Geolocation.GetLocationAsync( request, token );
        return new CurrentLocation( location );
    }


    public bool EqualInstance( ICurrentLocation other ) => InstanceID.Equals( other.InstanceID );


    public override bool Equals( object? obj )
    {
        if ( obj is null ) { return false; }

        if ( ReferenceEquals( this, obj ) ) { return true; }

        return obj is CurrentLocation location && Equals( location );
    }


    public bool IsValid( ICurrentLocation location, DistanceUnits units, double maxDistance )
    {
        if ( InstanceID == Guid.Empty ) { return false; }


        return CalculateDistance( this, location, units ) <= maxDistance;
    }


    /// <summary> Check distance from starting to this location. </summary>
    /// <param name="locationStart"> </param>
    /// <param name="units"> </param>
    /// <returns> Distance as a <see cref="double"/> </returns>
    public double CalculateDistance( ICurrentLocation locationStart, DistanceUnits units ) => CalculateDistance( locationStart, this, units );

    /// <summary> Check distance from starting to this location. </summary>
    /// <param name="latitudeStart"> </param>
    /// <param name="longitudeStart"> </param>
    /// <param name="units"> </param>
    /// <returns> Distance as a <see cref="double"/> </returns>
    public double CalculateDistance( double latitudeStart, double longitudeStart, DistanceUnits units ) => CalculateDistance( latitudeStart, longitudeStart, this, units );
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
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


    public bool Equals( ICurrentLocation? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return InstanceID.Equals( other.InstanceID ) && Timestamp.Equals( other.Timestamp ) && Latitude.Equals( other.Latitude ) && Longitude.Equals( other.Longitude ) && Nullable.Equals( Altitude, other.Altitude ) &&
               Nullable.Equals( Accuracy, other.Accuracy ) && Nullable.Equals( VerticalAccuracy, other.VerticalAccuracy ) && Nullable.Equals( Speed, other.Speed ) && Nullable.Equals( Course, other.Course ) &&
               IsFromMockProvider == other.IsFromMockProvider && AltitudeReferenceSystem == other.AltitudeReferenceSystem;
    }
}
