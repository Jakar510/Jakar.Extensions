// Jakar.Extensions :: Jakar.Database
// 03/06/2023  12:59 AM

namespace Jakar.Database;


public readonly struct Error
{
    public object?               Value   { get; }
    public ModelStateDictionary? State   { get; }
    public string[]?             Errors  { get; }
    public Status                Status  { get; }
    public ProblemDetails?       Details { get; }


    public Error() => throw new NotImplementedException();
    public Error( Status status ) => Status = status;
    public Error( Status status, object?              value = default ) : this( status ) => Value = value;
    public Error( Status status, ProblemDetails       details ) : this( status ) => Details = details;
    public Error( Status status, ModelStateDictionary value ) : this( status ) => State = value;
    public Error( Status status, params string[]      errors ) : this( status ) => Errors = errors;


    private object? GetResult() => State is not null
                                       ? new SerializableError( State )
                                       : Errors is not null
                                           ? new SerializableError
                                             {
                                                 [nameof(Errors)] = Errors,
                                             }
                                           : Details ?? Value;
    public ActionResult ToActionResult() => new ObjectResult( GetResult() )
                                            {
                                                StatusCode = (int)Status,
                                            };
}



public static class ErrorExtensions
{
    public static ActionResult<T> Match<T>( this OneOf<T, Error> value ) => value.Match<ActionResult<T>>( x => x, x => x.ToActionResult() );
}
