namespace Jakar.Database;


// TODO: asp.net authorization dapper
public static partial class DbExtensions
{
    public static IServiceCollection AddAuth<T>( this IServiceCollection services )
        where T : class, IAuthenticationService
    {
        services.AddHttpContextAccessor();
        services.AddTransient<IAuthenticationService, T>();
        return services;
    }
}
