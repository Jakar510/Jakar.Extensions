#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public static class LocationExtensions
{
    public static Plugin.Media.Abstractions.Location ToPluginLocation( this Location location ) =>
        new()
        {
            Altitude           = location.Altitude ?? double.NaN,
            HorizontalAccuracy = location.Accuracy ?? double.NaN,
            Latitude           = location.Latitude,
            Longitude          = location.Longitude,
            Speed              = location.Speed ?? double.NaN,
            Timestamp          = new DateTime(location.Timestamp.Ticks),
        };
}
