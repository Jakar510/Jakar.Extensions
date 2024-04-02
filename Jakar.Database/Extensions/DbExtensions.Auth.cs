namespace Jakar.Database;


// TODO: asp.net authorization dapper
public static partial class DbExtensions
{
    public static IServiceCollection AddAuth<T>( this IServiceCollection services )
        where T : class, IAuthenticatorService
    {
        services.AddHttpContextAccessor();
        services.AddTransient<IAuthenticatorService, T>();
        return services;
    }
}
