namespace Jakar.Extensions.Hosting;


public static partial class WebBuilder
{
    /// <summary> My config is Pascal Case </summary>
    /// <param name="services"> </param>
    /// <returns> </returns>
    public static IMvcBuilder UseNewtonsoftJson( this IMvcBuilder services )
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                                            {
                                                ContractResolver = new DefaultContractResolver(),
                                            };

        return services.AddJsonOptions( options => options.JsonSerializerOptions.PropertyNamingPolicy = null )
                       .AddNewtonsoftJson( options => options.SerializerSettings.ContractResolver = new DefaultContractResolver() );
    }
}
