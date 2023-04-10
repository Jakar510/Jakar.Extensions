//using Plugin.Permissions;
//using Plugin.Permissions.Abstractions;

#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public static class AppPermissions
{
    public static async ValueTask<PermissionStatus> CalendarReadPermission() => await CheckAndRequestPermissionAsync( new Permissions.CalendarRead() );
    public static async ValueTask<PermissionStatus> CalendarWritePermission() => await CheckAndRequestPermissionAsync( new Permissions.CalendarWrite() );
    public static async ValueTask<PermissionStatus> CameraPermission() => await CheckAndRequestPermissionAsync( new Permissions.Camera() );
    public static async ValueTask<PermissionStatus> ContactsReadPermission() => await CheckAndRequestPermissionAsync( new Permissions.ContactsRead() );
    public static async ValueTask<PermissionStatus> ContactsWritePermission() => await CheckAndRequestPermissionAsync( new Permissions.ContactsWrite() );
    public static async ValueTask<PermissionStatus> LocationAlwaysPermission() => await CheckAndRequestPermissionAsync( new Permissions.LocationAlways() );
    public static async ValueTask<PermissionStatus> LocationWhenInUsePermission() => await CheckAndRequestPermissionAsync( new Permissions.LocationWhenInUse() );
    public static async ValueTask<PermissionStatus> MediaLibraryPermission() => await CheckAndRequestPermissionAsync( new Permissions.Media() );
    public static async ValueTask<PermissionStatus> MicrophonePermission() => await CheckAndRequestPermissionAsync( new Permissions.Microphone() );
    public static async ValueTask<PermissionStatus> PhonePermission() => await CheckAndRequestPermissionAsync( new Permissions.Phone() );
    public static async ValueTask<PermissionStatus> PhotosPermission() => await CheckAndRequestPermissionAsync( new Permissions.Photos() );
    public static async ValueTask<PermissionStatus> RemindersPermission() => await CheckAndRequestPermissionAsync( new Permissions.Reminders() );
    public static async ValueTask<PermissionStatus> SensorsPermission() => await CheckAndRequestPermissionAsync( new Permissions.Sensors() );
    public static async ValueTask<PermissionStatus> SmsPermission() => await CheckAndRequestPermissionAsync( new Permissions.Sms() );
    public static async ValueTask<PermissionStatus> SpeechPermission() => await CheckAndRequestPermissionAsync( new Permissions.Speech() );
    public static async ValueTask<PermissionStatus> StorageReadPermission() => await CheckAndRequestPermissionAsync( new Permissions.StorageRead() );
    public static async ValueTask<PermissionStatus> StorageWritePermission() => await CheckAndRequestPermissionAsync( new Permissions.StorageWrite() );


    private static async ValueTask<PermissionStatus> CheckAndRequestPermissionAsync<T>( T permission ) where T : Permissions.BasePermission, new()
    {
        PermissionStatus status = await permission.CheckStatusAsync();
        if ( status != PermissionStatus.Granted ) { status = await permission.RequestAsync(); }

        return status;
    }


    public static T? Handle<T>( PermissionStatus status, Func<T>? denial, Func<T>? granted, Func<T>? unknown ) =>
        status switch
        {
            //Query permission
            PermissionStatus.Granted when granted is null => default,
            PermissionStatus.Granted                      => granted(),

            //Permission denied
            PermissionStatus.Unknown when unknown is null => default,
            PermissionStatus.Unknown                      => unknown(),

            // Notify user permission was denied
            PermissionStatus.Denied when denial is null => default,
            PermissionStatus.Denied                     => denial(),

            PermissionStatus.Disabled   => default,
            PermissionStatus.Restricted => default,
            _                           => throw new ArgumentOutOfRangeException( nameof(status), status, null ),
        };
    public static async ValueTask Handle( PermissionStatus status, Func<ValueTask>? denial, Func<ValueTask>? granted, Func<ValueTask>? unknown )
    {
        switch ( status )
        {
            //Query permission
            case PermissionStatus.Granted when granted is null: return;

            case PermissionStatus.Granted:
                await granted();

                break;

            //Permission denied
            case PermissionStatus.Unknown when unknown is null: return;

            case PermissionStatus.Unknown:
                await unknown();

                break;

            case PermissionStatus.Denied: // Notify user permission was denied
                if ( denial is null ) { return; }

                await denial();

                break;

            case PermissionStatus.Disabled: break;

            case PermissionStatus.Restricted: break;

            default: throw new ArgumentOutOfRangeException( nameof(status), status, null );
        }
    }
    public static async ValueTask<T?> Handle<T>( PermissionStatus status, Func<ValueTask<T>>? denial, Func<ValueTask<T>>? granted, Func<ValueTask<T>>? unknown ) =>
        status switch
        {
            //Query permission
            PermissionStatus.Granted when granted is null => default,
            PermissionStatus.Granted                      => await granted(),

            //Permission denied
            PermissionStatus.Denied when denial is null => default,
            PermissionStatus.Denied                     => await denial(),

            //Permission denied
            PermissionStatus.Unknown when unknown is null => default,
            PermissionStatus.Unknown                      => await unknown(),

            _ => default,
        };
    public static void Handle( PermissionStatus status, Action? denial, Action? granted, Action? unknown )
    {
        switch ( status )
        {
            //Query permission
            case PermissionStatus.Granted when granted is null: return;

            case PermissionStatus.Granted:
                granted();
                return;

            //Permission denied
            case PermissionStatus.Unknown when unknown is null: return;

            case PermissionStatus.Unknown:
                unknown();
                return;

            case PermissionStatus.Denied when denial is null: return;

            case PermissionStatus.Denied:
                denial();
                return;

            case PermissionStatus.Disabled: break;

            case PermissionStatus.Restricted: break;

            default: throw new ArgumentOutOfRangeException( nameof(status), status, null );
        }
    }
    public static void Handle( PermissionStatus status, ICommand? denial, ICommand? granted, ICommand? unknown )
    {
        switch ( status )
        {
            //Query permission
            case PermissionStatus.Granted when granted is null: return;

            case PermissionStatus.Granted:
                granted.Execute( null );
                return;

            //Permission denied
            case PermissionStatus.Unknown when unknown is null: return;

            case PermissionStatus.Unknown:
                unknown.Execute( null );
                return;

            case PermissionStatus.Denied when denial is null: return;

            case PermissionStatus.Denied:
                denial.Execute( null );
                return;

            case PermissionStatus.Disabled: break;

            case PermissionStatus.Restricted: break;

            default: throw new ArgumentOutOfRangeException( nameof(status), status, null );
        }
    }
    public static void Handle<T>( PermissionStatus status, Command<T>? denial, Command<T>? granted, Command<T>? unknown, T obj )
    {
        switch ( status )
        {
            //Query permission
            case PermissionStatus.Granted when granted is null: return;

            case PermissionStatus.Granted:
                if ( granted.CanExecute( obj ) ) { granted.Execute( obj ); }

                return;

            //Permission denied
            case PermissionStatus.Unknown when unknown is null: return;

            case PermissionStatus.Unknown:
                if ( unknown.CanExecute( obj ) ) { unknown.Execute( obj ); }

                return;

            case PermissionStatus.Denied when denial is null: return;

            case PermissionStatus.Denied:
                if ( denial.CanExecute( obj ) ) { denial.Execute( obj ); }

                return;

            case PermissionStatus.Disabled: break;

            case PermissionStatus.Restricted: break;

            default: throw new ArgumentOutOfRangeException( nameof(status), status, null );
        }
    }

    /*

    public enum Permission
    {
        Unknown = 0,
        Calendar = 1,
        Camera = 2,
        Contacts = 3,
        Location = 4,
        Microphone = 5,
        Phone = 6,
        Photos = 7,
        Reminders = 8,
        Sensors = 9,
        Sms = 10,
        Storage = 11,
        Speech = 12,
        LocationAlways = 13,
        LocationWhenInUse = 14,
        MediaLibrary = 15
    }

    */
}
