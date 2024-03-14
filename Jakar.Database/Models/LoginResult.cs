// Jakar.Extensions :: Jakar.Database
// 10/10/2022  5:17 PM

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "InconsistentNaming" )]
public readonly record struct LoginResult( LoginResult.State Result, UserRecord? User = default, Exception? Exception = default, ModelStateDictionary? Model = default )
{
    [MemberNotNullWhen( true,  nameof(User) )]
    [MemberNotNullWhen( false, nameof(Exception) )]
    public bool Succeeded => User is not null
                                 ? Result == State.Success
                                 : Exception is null;


    public LoginResult( ModelStateDictionary value ) : this( State.UnknownError, default, default, value ) { }
    public LoginResult( Exception            value ) : this( State.UnknownError, default, value ) { }
    public LoginResult( UserRecord           value ) : this( State.Success, value ) { }


    public static implicit operator LoginResult( State                result ) => new(result);
    public static implicit operator LoginResult( ModelStateDictionary result ) => new(result);
    public static implicit operator LoginResult( Exception            result ) => new(result);
    public static implicit operator LoginResult( UserRecord           result ) => new(result);


    public Error? GetResult() =>
        Result switch
        {
            State.Success => default,
            State.UnknownError when Exception is not null => new Error( Status.InternalServerError,
                                                                        new ProblemDetails
                                                                        {
                                                                            Detail   = Exception.Message,
                                                                            Instance = Exception.Source,
                                                                            Status   = (int)Status.InternalServerError,
                                                                            Title    = Exception.MethodName(),
                                                                            Type     = Exception.GetType().Name
                                                                        } ),
            State.UnknownError when Model is not null => new Error( Status.InternalServerError, GetModelStateDictionary() ),
            State.UnknownError                        => new Error( Status.InternalServerError ),
            State.BadCredentials                      => new Error( Status.Unauthorized ),
            State.Inactive                            => new Error( Status.Forbidden ),
            State.Locked                              => new Error( Status.Locked ),
            State.Disabled                            => new Error( Status.Disabled ),
            State.ExpiredSubscription                 => new Error( Status.PaymentRequired ),
            State.NoSubscription                      => new Error( Status.PaymentRequired ),
            State.NotFound                            => new Error( Status.NotFound ),
            _                                         => throw new OutOfRangeException( nameof(Result), Result )
        };
    public bool GetResult( [NotNullWhen( false )] out UserRecord? caller )
    {
        caller = User;
        return caller is null;
    }
    public bool GetResult( [NotNullWhen( true )] out Error? actionResult )
    {
        actionResult = GetResult();
        return actionResult is not null;
    }
    public bool GetResult( [NotNullWhen( true )] out Error? actionResult, [NotNullWhen( false )] out UserRecord? caller )
    {
        actionResult = GetResult();
        caller       = User;
        return actionResult is not null && caller is null;
    }
    public bool GetResult( [NotNullWhen( true )] out ActionResult? actionResult )
    {
        Error? result = GetResult();
        actionResult = result?.ToActionResult();
        return actionResult is not null;
    }
    public bool GetResult( [NotNullWhen( true )] out ActionResult? actionResult, [NotNullWhen( false )] out UserRecord? caller )
    {
        Error? result = GetResult();
        actionResult = result?.ToActionResult();
        caller       = User;
        return actionResult is not null && caller is null;
    }


    public ModelStateDictionary GetModelStateDictionary()
    {
        ModelStateDictionary dictionary = Model ?? new ModelStateDictionary();
        dictionary.AddModelError( nameof(State), Result.ToString() );

        dictionary.AddModelError( nameof(Status), GetStatus().ToStringFast() );


        if ( Exception is null ) { return dictionary; }

        dictionary.AddModelError( nameof(ProblemDetails.Detail),   Exception.Message );
        dictionary.AddModelError( nameof(ProblemDetails.Instance), Exception.Source       ?? string.Empty );
        dictionary.AddModelError( nameof(ProblemDetails.Title),    Exception.MethodName() ?? string.Empty );

        dictionary.AddModelError( nameof(ProblemDetails.Type), Exception.GetType().Name );

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
                                     State.NotFound            => Status.NotFound,
                                     _                         => throw new OutOfRangeException( nameof(Result), Result )
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
        NotFound
    }
}
