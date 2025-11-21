namespace Jakar.Extensions.Telemetry.Server.Data;


public static class ApiEndpoints
{
    public static void AddEndpoints( this WebApplication application )
    {
        application.MapPost(Endpoints.SAVE_ACTIVITY, SaveActivity)
                   .WithDisplayName(nameof(SaveActivity));

        // application.MapPost(  )
    }


    public static async Task SaveActivity( [FromBody] Activity activity, CancellationToken token ) { }
}
