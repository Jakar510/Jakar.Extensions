using Jakar.Extensions;
using Jakar.Json;
using Jakar.Json.AspNetCore;
using Microsoft.AspNetCore.Mvc;


// camelCase names, matched case-insensitively: what ASP.NET Core's System.Text.Json defaults read and write, for this app's [JsonModel] types.
[assembly: JsonDefaults(Naming = JsonNaming.CamelCase, NameMatching = JsonNameMatching.OrdinalIgnoreCase)]


WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Jakar.Json for [JsonModel] types: MVC formatters, plus options for the minimal API results and JsonBody<T>. Other types stay on System.Text.Json.
builder.Services.AddControllers().AddJakarJson();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel( LogLevel.Debug );


await using WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() )
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.MapControllers();

// Minimal APIs: plain return values of [JsonModel] types are written by Jakar.Json; JsonBody<T> reads request bodies with it.
RouteGroupBuilder api = app.MapGroup( "/api" ).WithJakarJson();

api.MapGet( "/forecast",
            ( [FromServices] ILogger<Program> logger ) =>
            {
                WeatherForecast[] forecast = WeatherForecast.Create( Random.Shared.Next( 5, 25 ) );
                logger.LogDebug( "{Length}", forecast.Length );
                return forecast;
            } );

api.MapGet( "/forecast/{days:int}", ( int days ) => JakarResults.JsonArray( WeatherForecast.Create( days ) ) );

api.MapPost( "/forecast", ( JsonBody<WeatherForecast> forecast ) => JakarResults.Created( $"/api/forecast/{forecast.Value.Date:yyyy-MM-dd}", forecast.Value ) );

await app.RunAsync();



[JsonModel]
public sealed partial record WeatherForecast( DateOnly Date, int TemperatureC, string? Summary )
{
    private static readonly string[] __summaries =
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


    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);


    public static WeatherForecast[] Create( int days ) => Enumerable.Range( 1, days ).Select( static index => new WeatherForecast( DateOnly.FromDateTime( DateTime.Now.AddDays( index ) ), Random.Shared.Next( -20, 55 ), __summaries[Random.Shared.Next( __summaries.Length )] ) ).ToArray();
}



/// <summary> The same forecasts through MVC: <c>AddJakarJson()</c> reads and writes them with Jakar.Json. </summary>
[ApiController]
[Route( "mvc/forecast" )]
public sealed class ForecastController : ControllerBase
{
    [HttpGet] public ActionResult<WeatherForecast[]> Get() => WeatherForecast.Create( 5 );

    [HttpPost] public ActionResult<WeatherForecast> Post( WeatherForecast forecast ) => CreatedAtAction( nameof(Get), forecast );
}
