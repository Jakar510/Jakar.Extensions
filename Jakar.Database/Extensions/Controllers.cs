// Jakar.Extensions :: Jakar.Extensions.Hosting
// 09/22/2022  4:02 PM

namespace Jakar.Database;


public static class Controllers
{
    public const  string       DETAILS = nameof(DETAILS);
    public const  string       ERROR   = nameof(ERROR);
    public static ActionResult ClientClosed( this ControllerBase _ ) => new StatusCodeResult(Status.ClientClosedRequest.AsInt());


    public static ActionResult Duplicate( this ControllerBase controller, Exception e )
    {
        controller.AddError(e);
        return controller.ItemNotFound();
    }
    public static ActionResult Duplicate( this ControllerBase controller, Exception e, params string[] errors )
    {
        controller.AddError(e);
        return controller.ItemNotFound(errors);
    }
    public static ActionResult Duplicate( this ControllerBase controller, params string[] errors )
    {
        controller.AddError(errors);
        return controller.Duplicate();
    }
    public static ActionResult Duplicate( this ControllerBase controller )
    {
        ModelStateDictionary modelState = controller.ModelState;

        return modelState.ErrorCount > 0
                   ? new NotFoundObjectResult(modelState)
                   : controller.NotFound();
    }


    public static ActionResult FileNotFound( this ControllerBase controller, FileNotFoundException e )
    {
        controller.AddError(e);
        ModelStateDictionary modelState = controller.ModelState;

        return modelState.ErrorCount > 0
                   ? controller.NotFound(modelState)
                   : !string.IsNullOrWhiteSpace(e.Message)
                       ? controller.NotFound(e.Message)
                       : controller.NotFound();
    }


    public static ActionResult GetBadRequest( this ControllerBase controller, Exception e, params string[] errors )
    {
        controller.AddError(e);
        return controller.GetBadRequest(errors);
    }
    public static ActionResult GetBadRequest( this ControllerBase controller, params string[] errors )
    {
        controller.AddError(errors);
        return controller.GetBadRequest();
    }
    public static ActionResult GetBadRequest( this ControllerBase controller )
    {
        ModelStateDictionary modelState = controller.ModelState;
        Guard.IsGreaterThan(modelState.ErrorCount, 0, nameof(modelState));

        return controller.BadRequest(modelState);
    }


    public static ActionResult ItemNotFound( this ControllerBase controller, Exception e )
    {
        controller.AddError(e);
        return controller.ItemNotFound();
    }
    public static ActionResult ItemNotFound( this ControllerBase controller, Exception e, params string[] errors )
    {
        controller.AddError(e);
        return controller.ItemNotFound(errors);
    }
    public static ActionResult ItemNotFound( this ControllerBase controller, params string[] errors )
    {
        controller.AddError(errors);
        return controller.ItemNotFound();
    }
    public static ActionResult ItemNotFound( this ControllerBase controller )
    {
        ModelStateDictionary modelState = controller.ModelState;

        return modelState.ErrorCount > 0
                   ? new NotFoundObjectResult(modelState)
                   : controller.NotFound();
    }


    public static ActionResult NotAcceptable( this ControllerBase controller, Exception e )
    {
        controller.AddError(e);
        return controller.NotAcceptable();
    }
    public static ActionResult NotAcceptable( this ControllerBase controller, FormatException e )
    {
        controller.AddError(e);
        return controller.NotAcceptable();
    }
    public static ActionResult NotAcceptable( this ControllerBase controller, NotAcceptableException e )
    {
        controller.AddError(e);
        return controller.NotAcceptable();
    }
    public static ActionResult NotAcceptable( this ControllerBase controller, string message )
    {
        controller.AddError(message);
        return controller.NotAcceptable();
    }
    public static ActionResult NotAcceptable( this ControllerBase controller )
    {
        ModelStateDictionary modelState = controller.ModelState;
        Guard.IsGreaterThan(modelState.ErrorCount, 0, nameof(modelState));

        return modelState.ErrorCount > 0
                   ? new UnprocessableEntityObjectResult(modelState) { StatusCode = Status.NotAcceptable.AsInt() }
                   : controller.UnprocessableEntity();
    }


    public static ActionResult Problem( this ControllerBase controller, in Status      status )  => controller.Problem(controller.ToProblemDetails(status));
    public static ActionResult Problem( this ControllerBase _,          ProblemDetails details ) => new ObjectResult(details) { StatusCode = details.Status };


    public static ActionResult ServerProblem( this ControllerBase controller, Exception e )                         => controller.ServerProblem(e.Message);
    public static ActionResult ServerProblem( this ControllerBase controller, string    message = "Unknown Error" ) => controller.Problem(message, statusCode: Status.InternalServerError.AsInt());


    public static ActionResult TimeoutOccurred( this ControllerBase controller, Exception e )
    {
        controller.AddError(e);
        return controller.NotAcceptable();
    }
    public static ActionResult TimeoutOccurred( this ControllerBase controller, TimeoutException e )
    {
        controller.AddError(e);
        return controller.NotAcceptable();
    }
    public static ActionResult TimeoutOccurred( this ControllerBase controller )
    {
        ModelStateDictionary modelState = controller.ModelState;
        Guard.IsGreaterThan(modelState.ErrorCount, 0, nameof(modelState));

        return new ObjectResult(new SerializableError(modelState)) { StatusCode = Status.GatewayTimeout.AsInt() };
    }


    public static ActionResult UnauthorizedAccess( this ControllerBase controller, Exception e )
    {
        controller.AddError(e);
        ModelStateDictionary modelState = controller.ModelState;

        return modelState.ErrorCount > 0
                   ? controller.UnprocessableEntity(modelState)
                   : string.IsNullOrWhiteSpace(e.Message)
                       ? controller.UnprocessableEntity(e.Message)
                       : controller.UnprocessableEntity();
    }
    public static ActionResult UnauthorizedAccess( this ControllerBase controller, UnauthorizedAccessException e )
    {
        controller.AddError(e);
        ModelStateDictionary modelState = controller.ModelState;

        return modelState.ErrorCount > 0
                   ? controller.UnprocessableEntity(modelState)
                   : string.IsNullOrWhiteSpace(e.Message)
                       ? controller.UnprocessableEntity(e.Message)
                       : controller.UnprocessableEntity();
    }
    public static ActionResult UnauthorizedAccess( this ControllerBase controller )
    {
        ModelStateDictionary modelState = controller.ModelState;

        return modelState.ErrorCount > 0
                   ? controller.UnprocessableEntity(modelState)
                   : controller.UnprocessableEntity();
    }


    public static ProblemDetails ToProblemDetails( this ControllerBase controller, in Status status ) => controller.ModelState.ToProblemDetails(status);
    public static ProblemDetails ToProblemDetails( this ModelStateDictionary state, in Status status )
    {
        state.TryGetValue(nameof(ProblemDetails.Detail),   out ModelStateEntry? detail);
        state.TryGetValue(nameof(ProblemDetails.Instance), out ModelStateEntry? instance);
        state.TryGetValue(nameof(ProblemDetails.Title),    out ModelStateEntry? title);
        state.TryGetValue(nameof(ProblemDetails.Type),     out ModelStateEntry? type);

        ProblemDetails problem = new()
                                 {
                                     Detail   = detail?.AttemptedValue,
                                     Instance = instance?.AttemptedValue,
                                     Title    = title?.AttemptedValue,
                                     Type     = type?.AttemptedValue,
                                     Status   = status.AsInt()
                                 };

        foreach ( ( string key, ModelStateEntry value ) in state )
        {
            if ( key.IsOneOf(nameof(ProblemDetails.Detail), nameof(ProblemDetails.Instance), nameof(ProblemDetails.Title), nameof(ProblemDetails.Type)) ) { continue; }

            problem.Extensions.Add(key, value.AttemptedValue);
        }

        return problem;
    }


    public static void AddError( this ControllerBase controller, string              error, string? title = null, string key = ERROR ) { controller.ModelState.AddError(error, title, key); }
    public static void AddError( this ControllerBase controller, params string[]     errors ) { controller.ModelState.AddError(errors.AsSpan()); }
    public static void AddError( this ControllerBase controller, IEnumerable<string> errors ) { controller.ModelState.AddError(errors); }
    public static void AddError( this ControllerBase controller, Exception           e )      { controller.ModelState.AddError(e); }


    public static void AddError( this ModelStateDictionary controller, Exception e )
    {
        controller.AddError(e.Message);
        foreach ( DictionaryEntry pair in e.Data ) { controller.AddError($"{pair.Key}: {pair.Value}"); }
    }


    public static void AddError( this ModelStateDictionary state, string error, string? title = null, string key = ERROR )
    {
        state.AddError(key,
                       title is null
                           ? error
                           : $"{title} : {error}");
    }


    public static void AddError( this ModelStateDictionary state, scoped in ReadOnlySpan<string> errors )
    {
        if ( errors.IsEmpty ) { return; }

        foreach ( string error in errors ) { state.AddError(error); }
    }


    public static void AddError( this ModelStateDictionary state, IEnumerable<string> errors )
    {
        foreach ( string error in errors ) { state.AddError(error); }
    }
}
