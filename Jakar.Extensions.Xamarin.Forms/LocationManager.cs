using Jakar.Extensions.Xamarin.Forms.Statics;
using Xamarin.Essentials;





namespace Jakar.Extensions.Xamarin.Forms;


public class LocationManager
{
    public enum State
    {
        Default,
        Success,
        UnknownError,
        PermissionIssue,
        FeatureNotEnabled,
        FeatureNotSupported,
        IsFromMockProvider,
    }



    public State     Status   { get; protected set; }
    public Location? Location { get; protected set; }

    protected void Reset()
    {
        Status   = default;
        Location = null;
    }

    protected async Task<State> GetLocationAsync( GeolocationAccuracy accuracy = GeolocationAccuracy.Default )
    {
        Reset();
        var request = new GeolocationRequest(accuracy);
        Location = await Geolocation.GetLocationAsync(request).ConfigureAwait(false);

        if ( Location is null ) { return State.UnknownError; }

        Status = Location.IsFromMockProvider
                     ? State.IsFromMockProvider
                     : State.Success;

        // try
        // {
        // }
        // catch ( FeatureNotSupportedException ) // Handle not supported on device exception
        // {
        // 	Status = StatusState.FeatureNotSupported;
        // }
        // catch ( FeatureNotEnabledException ) // Handle not enabled on device exception
        // {
        // 	Status = StatusState.FeatureNotEnabled;
        // }
        // catch ( PermissionException ) // Handle permission exception
        // {
        // 	Status = StatusState.PermissionIssue;
        // }
        // catch ( Exception ) // Unable to get location
        // {
        // 	Status = StatusState.UnknownError;
        // }

        return Status;
    }

    public async Task<bool> Update()
    {
        if ( await AppPermissions.LocationWhenInUsePermission().ConfigureAwait(false) != PermissionStatus.Granted ) { return false; }

        State status = await GetLocationAsync().ConfigureAwait(false);
        return status == State.Success;
    }


    public static async Task<Location?> GetLocation()
    {
        var manager = new LocationManager();

        if ( await manager.Update().ConfigureAwait(false) ) { return manager.Location; }

        return null;
    }
}
