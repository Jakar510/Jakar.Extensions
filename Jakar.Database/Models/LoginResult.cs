// Jakar.Extensions :: Jakar.Database
// 10/10/2022  5:17 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "InconsistentNaming" )]
public readonly struct LoginResult
{
    public readonly State                 Result    = default;
    public readonly UserRecord?           User      = default;
    public readonly Exception?            Exception = default;
    public readonly ModelStateDictionary? Model     = default;
    public          bool                  Succeeded => User is not null && Result == State.Success;


    public LoginResult( ModelStateDictionary value ) : this( State.UnknownError ) => Model = value;
    public LoginResult( Exception            value ) : this( State.UnknownError ) => Exception = value;
    public LoginResult( UserRecord           value ) : this( State.Success ) => User = value;
    public LoginResult( State                value ) => Result = value;


    public static implicit operator LoginResult( State                result ) => new(result);
    public static implicit operator LoginResult( ModelStateDictionary result ) => new(result);
    public static implicit operator LoginResult( Exception            result ) => new(result);
    public static implicit operator LoginResult( UserRecord           result ) => new(result);


    public ActionResult? GetResult()
    {
        return Result switch
               {
                   State.Success => default,
                   State.UnknownError when Model is not null => new UnprocessableEntityObjectResult( Model )
                                                                {
                                                                    StatusCode = (int)Status.InternalServerError
                                                                },
                   State.UnknownError when Exception is not null => new UnprocessableEntityObjectResult( new ProblemDetails
                                                                                                         {
                                                                                                             Detail   = Exception.Message,
                                                                                                             Instance = Exception.Source,
                                                                                                             Status   = (int)Status.InternalServerError,
                                                                                                             Title    = Exception.MethodName(),
                                                                                                             Type = Exception.GetType()
                                                                                                                             .Name
                                                                                                         } )
                                                                    {
                                                                        StatusCode = (int)Status.InternalServerError
                                                                    },
                   State.UnknownError => new UnprocessableEntityObjectResult( default(object) )
                                         {
                                             StatusCode = (int)Status.InternalServerError
                                         },
                   State.BadCredentials => new UnauthorizedObjectResult( Result )
                                           {
                                               StatusCode = (int)Status.Unauthorized
                                           },
                   State.Inactive => new UnauthorizedObjectResult( Result )
                                     {
                                         StatusCode = (int)Status.Forbidden
                                     },
                   State.Locked => new UnauthorizedObjectResult( Result )
                                   {
                                       StatusCode = (int)Status.Locked
                                   },
                   State.Disabled => new UnauthorizedObjectResult( Result )
                                     {
                                         StatusCode = (int)Status.Disabled
                                     },
                   State.ExpiredSubscription => new BadRequestObjectResult( Result )
                                                {
                                                    StatusCode = (int)Status.PaymentRequired
                                                },
                   State.NoSubscription => new BadRequestObjectResult( Result )
                                           {
                                               StatusCode = (int)Status.PaymentRequired
                                           },
                   _ => throw new OutOfRangeException( nameof(Result), Result ),
               };
    }
    public bool GetResult( [NotNullWhen( false )] out UserRecord? caller )
    {
        caller = User;
        return caller is null;
    }
    public bool GetResult( [NotNullWhen( true )] out ActionResult? actionResult )
    {
        actionResult = GetResult();
        return actionResult is not null;
    }
    public bool GetResult( [NotNullWhen( true )] out ActionResult? actionResult, [NotNullWhen( false )] out UserRecord? caller )
    {
        actionResult = GetResult();
        caller       = User;
        return actionResult is not null && caller is null;
    }


    public ModelStateDictionary GetModelStateDictionary()
    {
        var dictionary = Model ?? new ModelStateDictionary();
        dictionary.AddModelError( nameof(State), Result.ToString() );

        dictionary.AddModelError( nameof(Status),
                                  GetStatus()
                                     .ToStringFast() );

        if ( Exception is null ) { return dictionary; }

        dictionary.AddModelError( nameof(ProblemDetails.Detail),   Exception.Message );
        dictionary.AddModelError( nameof(ProblemDetails.Instance), Exception.Source ?? string.Empty );
        dictionary.AddModelError( nameof(ProblemDetails.Title),    Exception.MethodName() ?? string.Empty );

        dictionary.AddModelError( nameof(ProblemDetails.Type),
                                  Exception.GetType()
                                           .Name );

        return dictionary;
    }
    public Status GetStatus() => Result switch
                                 {
                                     State.Success             => Status.Ok,
                                     State.UnknownError        => Status.InternalServerError,
                                     State.BadCredentials      => Status.Unauthorized,
                                     State.Inactive            => Status.Forbidden,
                                     State.Locked              => Status.Locked,
                                     State.Disabled            => Status.Disabled,
                                     State.ExpiredSubscription => Status.PaymentRequired,
                                     State.NoSubscription      => Status.PaymentRequired,
                                     _                         => throw new OutOfRangeException( nameof(Result), Result ),
                                 };



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
