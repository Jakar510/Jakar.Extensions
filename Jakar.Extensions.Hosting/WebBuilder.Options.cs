namespace Jakar.Extensions.Hosting;


[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
public static partial class WebBuilder
{
    public static OptionsBuilder<TOptions> AddOptions<TOptions>( this WebApplicationBuilder builder ) where TOptions : class => builder.Services.AddOptions<TOptions>();
    public static OptionsBuilder<TOptions> AddOptions<TOptions>( this WebApplicationBuilder builder, string name ) where TOptions : class => builder.Services.AddOptions<TOptions>( name );
    public static OptionsBuilder<TOptions> AddOptions<TOptions>( this WebApplicationBuilder builder, Action<TOptions> configure ) where TOptions : class => builder.AddOptions<TOptions>()
                                                                                                                                                                   .Configure( configure );
    public static OptionsBuilder<TOptions> AddOptions<TOptions>( this WebApplicationBuilder builder, string name, Action<TOptions> configure ) where TOptions : class => builder.AddOptions<TOptions>( name )
                                                                                                                                                                                .Configure( configure );
    public static WebApplicationBuilder AddOptions( this WebApplicationBuilder builder )
    {
        builder.Services.AddOptions();
        return builder;
    }
}
