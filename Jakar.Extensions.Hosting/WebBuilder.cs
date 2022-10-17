namespace Jakar.Extensions.Hosting;


public static partial class WebBuilder
{
    public static WebApplicationBuilder AddEndpointsApiExplorer( this WebApplicationBuilder builder )
    {
        builder.Services.AddEndpointsApiExplorer();
        return builder;
    }
    public static WebApplicationBuilder AddSwaggerGen( this WebApplicationBuilder builder )
    {
        builder.Services.AddSwaggerGen();
        return builder;
    }
    public static WebApplicationBuilder AddWebEncoders( this WebApplicationBuilder builder )
    {
        builder.Services.AddWebEncoders();
        return builder;
    }
}
