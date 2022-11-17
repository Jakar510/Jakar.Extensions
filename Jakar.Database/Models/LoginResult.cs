// Jakar.Extensions :: Jakar.Database
// 10/10/2022  5:17 PM

using Microsoft.AspNetCore.Mvc;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "InconsistentNaming" )]
public readonly struct LoginResult
{
    public readonly State       Result = default;
    public readonly UserRecord? User   = default!;
    public          bool        Succeeded => User is not null && Result == State.Success;


    public LoginResult( State      result ) => Result = result;
    public LoginResult( UserRecord user ) : this( State.Success ) => User = user;


    public static implicit operator LoginResult( State      result ) => new(result);
    public static implicit operator LoginResult( UserRecord result ) => new(result);


    public bool GetResult( ControllerBase controller, [NotNullWhen( true )] out ActionResult? actionResult, [NotNullWhen( false )] out UserRecord? caller )
    {
        caller = User;

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
                           _                         => throw new OutOfRangeException( nameof(Result), Result ),
                       };

        return actionResult is not null && caller is null;
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
        NoSubscription,
    }
}
