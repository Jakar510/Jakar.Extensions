// Jakar.Extensions :: Jakar.Database.Resx
// 10/07/2022  9:55 PM

namespace Jakar.Database.Resx;


public static class ResxExtensions
{
    public static WebApplicationBuilder AddResx( this WebApplicationBuilder builder )
    {
        builder.AddSingleton<IResxCollection, ResxCollection>();
        return builder;
    }
}
