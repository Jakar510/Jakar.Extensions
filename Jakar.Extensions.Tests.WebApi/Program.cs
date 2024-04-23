using Jakar.Extensions.Loggers;
using Microsoft.AspNetCore.Mvc;


WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel( LogLevel.Debug );
builder.Logging.AddFileLogger( new FileLoggerProviderOptions { AppName = builder.Environment.ApplicationName } );


await using WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() )
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

string[] summaries =
[
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching"
];

app.MapGet( "/forecast",
            ( [FromServices] ILogger logger ) =>
            {
                WeatherForecast[] forecast = Enumerable.Range( 1, Random.Shared.Next( 5, 25 ) ).Select( index => new WeatherForecast( DateOnly.FromDateTime( DateTime.Now.AddDays( index ) ), Random.Shared.Next( -20, 55 ), summaries[Random.Shared.Next( summaries.Length )] ) ).ToArray();
                logger.LogDebug( "{Length}", forecast.Length );
                return forecast;
            } )

    // .WithName( "forecast" )
    // .WithDisplayName( "GetWeatherForecast" )
    // .WithDescription( "Get Weather Forecast" )
   .WithOpenApi();

await app.RunAsync();



internal record WeatherForecast( DateOnly Date, int TemperatureC, string? Summary )
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
