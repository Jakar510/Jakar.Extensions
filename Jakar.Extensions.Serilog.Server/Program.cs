using Jakar.Extensions.Blazor;
using System.Text;
using FluentMigrator.Runner;
using Jakar.Extensions.Serilog.Server.Components;
using Jakar.Extensions.Serilog.Server.Data;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using SerilogTracing;


WebApplicationBuilder builder = WebApplication.CreateBuilder( args );
Encoding.RegisterProvider( CodePagesEncodingProvider.Instance );

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddCircuitOptions( static options => options.DetailedErrors = true ).AddHubOptions( static options => { options.MaximumReceiveMessageSize = null; } );

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Logging.AddSerilog().AddFluentMigratorConsole().AddOpenTelemetry().AddFluentMigratorLogger();

builder.Services.AddFusionCache().WithStackExchangeRedisBackplane().WithMemoryBackplane();

builder.Services.AddFusionCacheNewtonsoftJsonSerializer();

builder.Services.AddOpenTelemetry().WithTracing( static x => x.AddHttpClientInstrumentation().AddAspNetCoreInstrumentation() ).WithMetrics( static x => x.AddHttpClientInstrumentation().AddAspNetCoreInstrumentation() );

// using var listener = new ActivityListenerConfiguration().TraceToSharedLogger();

builder.Services.AddBlazorServices();

builder.Services.AddDataProtection( static options => options.ApplicationDiscriminator = TelemetryServer.AppName );
ConfigureApiDatabase.Setup( builder );


await using WebApplication app = builder.Build();


if ( app.Environment.IsDevelopment() is false )
{
    app.UseExceptionHandler( "/Error", true );
    app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAntiforgery();
app.UseAuthorization();
app.UseAuthorization();


app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

await app.RunAsync();
