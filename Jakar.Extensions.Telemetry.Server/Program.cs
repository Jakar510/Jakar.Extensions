using Jakar.Extensions.Blazor;
using Jakar.Extensions.Telemetry.Server.Data;
using Syncfusion.Blazor;


WebApplicationBuilder builder = WebApplication.CreateBuilder( args );
Encoding.RegisterProvider( CodePagesEncodingProvider.Instance );

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddCircuitOptions( static options => options.DetailedErrors = true ).AddHubOptions( static options => { options.MaximumReceiveMessageSize = null; } );

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


builder.Services.AddBlazorServices();
builder.Services.AddSyncfusionBlazor();

builder.Services.AddDataProtection();
ConfigureApiDatabase.Setup( builder );

await using WebApplication app = builder.Build();

if ( app.Environment.IsDevelopment() is false )
{
    app.UseExceptionHandler( "/Error", true );
    app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}

app.UseHttpsRedirection();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAntiforgery();
app.UseAuthorization();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

await app.RunAsync();
