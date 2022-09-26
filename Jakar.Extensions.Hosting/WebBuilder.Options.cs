namespace Jakar.Extensions.Hosting;


public static partial class WebBuilder
{
    public static WebApplicationBuilder AddOptions( this WebApplicationBuilder builder )
    {
        builder.Services.AddOptions();
        return builder;
    }
    public static OptionsBuilder<TOptions> AddOptions<TOptions>( this WebApplicationBuilder builder ) where TOptions : class => builder.Services.AddOptions<TOptions>();
    public static OptionsBuilder<TOptions> AddOptions<TOptions>( this WebApplicationBuilder builder, string name ) where TOptions : class => builder.Services.AddOptions<TOptions>(name);
}
