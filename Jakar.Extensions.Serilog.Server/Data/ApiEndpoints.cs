using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;



namespace Jakar.Extensions.Serilog.Server.Data;


[Experimental( nameof(RemoteLogger) )]
public static class ApiEndpoints
{
    public static void AddEndpoints( this WebApplication application )
    {
        application.MapPost( RemoteLogger.Api.INGEST, SaveActivity ).WithDisplayName( nameof(SaveActivity) ).WithOpenApi().WithDescription( "Ingest logs and save them" );

        // application.MapPost( RemoteLogger.Api.INGEST, SaveActivity ).WithDisplayName( nameof( SaveActivity ) ).WithOpenApi().WithDescription( "Ingest logs and save them" );
    }


    public static async Task SaveActivity( [FromBody] RemoteLogger.Log[] collection, CancellationToken token ) { }
}
