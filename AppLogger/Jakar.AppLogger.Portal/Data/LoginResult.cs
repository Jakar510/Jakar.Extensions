// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/20/2022  9:51 PM

namespace Jakar.AppLogger.Portal.Data;


public readonly struct LoginResult
{
    public State       Result    { get; init; } = default;
    public UserRecord? User      { get; init; } = default!;
    public bool        Succeeded => User is not null && Result == State.Success;


    public LoginResult( State      result ) => Result = result;
    public LoginResult( UserRecord user ) : this( State.Success ) => User = user;


    public static implicit operator LoginResult( State      result ) => new(result);
    public static implicit operator LoginResult( UserRecord result ) => new(result);

    public bool GetResult( ControllerBase controller, [NotNullWhen( true )] out ActionResult? actionResult, [NotNullWhen( false )] out UserRecord? user )
    {
        user = User;

        actionResult = Result switch
                       {
                           State.UnknownError        => controller.UnprocessableEntity(),
                           State.Success             => default,
                           State.BadCredentials      => controller.Unauthorized(),
                           State.Inactive            => controller.Unauthorized(),
                           State.Locked              => controller.Unauthorized(),
                           State.Disabled            => controller.Unauthorized(),
                           State.ExpiredSubscription => controller.BadRequest(),
                           State.NoSubscription      => controller.BadRequest(),
                           _                         => throw new OutOfRangeException( nameof(Result), Result )
                       };

        return actionResult is not null && user is null;
    }



    public enum State
    {
        UnknownError,
        Success,
        BadCredentials,
        Inactive,
        Locked,
        Disabled,
        ExpiredSubscription,
        NoSubscription
    }
}
