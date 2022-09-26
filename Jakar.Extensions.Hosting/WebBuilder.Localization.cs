namespace Jakar.Extensions.Hosting;


public static partial class WebBuilder
{
    public static WebApplicationBuilder AddLocalization( this WebApplicationBuilder builder )
    {
        builder.Services.AddLocalization();
        return builder;
    }
    public static WebApplicationBuilder AddLocalization( this WebApplicationBuilder builder, Action<LocalizationOptions> configure )
    {
        builder.Services.AddLocalization(configure);
        return builder;
    }
}
