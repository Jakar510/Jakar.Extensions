namespace Jakar.Extensions.Hosting;


/// <summary>
///     <para>
///         <see href = "https://stackoverflow.com/a/61726193/9530917" > AddTransient, AddScoped and AddSingleton Services Differences </see>
///     </para>
/// </summary>
[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
public static partial class WebBuilder
{
    public static WebApplicationBuilder AddHostedService<THostedService>( this WebApplicationBuilder builder, THostedService service ) where THostedService : class, IHostedService
    {
        builder.AddSingleton( service );
        builder.Services.AddHostedService( provider => service );
        return builder;
    }


    public static WebApplicationBuilder AddHostedService<THostedService>( this WebApplicationBuilder builder ) where THostedService : class, IHostedService =>
        builder.AddHostedService( provider => provider.GetRequiredService<THostedService>() );
    public static WebApplicationBuilder AddHostedService<THostedService>( this WebApplicationBuilder builder, Func<IServiceProvider, THostedService> func ) where THostedService : class, IHostedService
    {
        builder.AddSingleton<THostedService>();
        builder.Services.AddHostedService( func );
        return builder;
    }


    public static WebApplicationBuilder AddHostedService<TService, THostedService>( this WebApplicationBuilder builder ) where TService : class, IHostedService
                                                                                                                         where THostedService : class, TService =>
        builder.AddHostedService<TService, THostedService>( provider => provider.GetRequiredService<TService>() );
    public static WebApplicationBuilder AddHostedService<TService, THostedService>( this WebApplicationBuilder builder, Func<IServiceProvider, TService> func ) where TService : class, IHostedService
                                                                                                                                                                where THostedService : class, TService
    {
        builder.AddSingleton<TService, THostedService>();
        builder.Services.AddHostedService( func );
        return builder;
    }


    /// <summary>
    ///     Adds a scoped service of the type specified in
    ///     <typeparamref name = "TService" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <typeparam name = "TService" > The type of the service to add. </typeparam>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Scoped" />
    public static WebApplicationBuilder AddScoped<TService>( this WebApplicationBuilder builder ) where TService : class => builder.AddScoped( typeof(TService) );

    /// <summary>
    ///     Adds a scoped service of the type specified in
    ///     <typeparamref name = "TService" />
    ///     with a
    ///     factory specified in
    ///     <paramref name = "implementationFactory" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <typeparam name = "TService" > The type of the service to add. </typeparam>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "implementationFactory" > The factory that creates the service. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Scoped" />
    public static WebApplicationBuilder AddScoped<TService>( this WebApplicationBuilder builder, Func<IServiceProvider, TService> implementationFactory ) where TService : class => builder.AddScoped( typeof(TService), implementationFactory );

    /// <summary>
    ///     Adds a scoped service of the type specified in
    ///     <paramref name = "serviceType" />
    ///     with a
    ///     factory specified in
    ///     <paramref name = "implementationFactory" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "serviceType" > The type of the service to register. </param>
    /// <param name = "implementationFactory" > The factory that creates the service. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Scoped" />
    public static WebApplicationBuilder AddScoped( this WebApplicationBuilder builder, Type serviceType, Func<IServiceProvider, object> implementationFactory ) => TryAdd( builder, serviceType, implementationFactory, ServiceLifetime.Scoped );
    /// <summary>
    ///     Adds a scoped service of the type specified in
    ///     <typeparamref name = "TService" />
    ///     with an
    ///     implementation type specified in
    ///     <typeparamref name = "TImplementation" />
    ///     using the
    ///     factory specified in
    ///     <paramref name = "implementationFactory" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <typeparam name = "TService" > The type of the service to add. </typeparam>
    /// <typeparam name = "TImplementation" > The type of the implementation to use. </typeparam>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "implementationFactory" > The factory that creates the service. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Scoped" />
    public static WebApplicationBuilder AddScoped<TService, TImplementation>( this WebApplicationBuilder builder, Func<IServiceProvider, TImplementation> implementationFactory ) where TService : class
                                                                                                                                                                                  where TImplementation : class, TService =>
        builder.AddScoped( typeof(TService), implementationFactory );

    /// <summary>
    ///     Adds a scoped service of the type specified in
    ///     <typeparamref name = "TService" />
    ///     with an
    ///     implementation type specified in
    ///     <typeparamref name = "TImplementation" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <typeparam name = "TService" > The type of the service to add. </typeparam>
    /// <typeparam name = "TImplementation" > The type of the implementation to use. </typeparam>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Scoped" />
    public static WebApplicationBuilder AddScoped<TService, TImplementation>( this WebApplicationBuilder builder ) where TService : class
                                                                                                                   where TImplementation : class, TService =>
        builder.AddScoped( typeof(TService), typeof(TImplementation) );

    /// <summary>
    ///     Adds a scoped service of the type specified in
    ///     <paramref name = "serviceType" />
    ///     with an
    ///     implementation of the type specified in
    ///     <paramref name = "implementationType" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "serviceType" > The type of the service to register. </param>
    /// <param name = "implementationType" > The implementation type of the service. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Scoped" />
    public static WebApplicationBuilder AddScoped( this WebApplicationBuilder builder, Type serviceType, Type implementationType ) => TryAdd( builder, serviceType, implementationType, ServiceLifetime.Scoped );

    /// <summary>
    ///     Adds a scoped service of the type specified in
    ///     <paramref name = "serviceType" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "serviceType" > The type of the service to register and the implementation to use. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Scoped" />
    public static WebApplicationBuilder AddScoped( this WebApplicationBuilder builder, Type serviceType ) => builder.AddScoped( serviceType, serviceType );


    /// <summary>
    ///     Adds a singleton service of the type specified in
    ///     <typeparamref name = "TService" />
    ///     with an
    ///     implementation type specified in
    ///     <typeparamref name = "TImplementation" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <typeparam name = "TService" > The type of the service to add. </typeparam>
    /// <typeparam name = "TImplementation" > The type of the implementation to use. </typeparam>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Singleton" />
    public static WebApplicationBuilder AddSingleton<TService, TImplementation>( this WebApplicationBuilder builder ) where TService : class
                                                                                                                      where TImplementation : class, TService => builder.AddSingleton( typeof(TService), typeof(TImplementation) );

    /// <summary>
    ///     Adds a singleton service of the type specified in
    ///     <typeparamref name = "TService" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <typeparam name = "TService" > The type of the service to add. </typeparam>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Singleton" />
    public static WebApplicationBuilder AddSingleton<TService>( this WebApplicationBuilder builder ) where TService : class => builder.AddSingleton( typeof(TService) );

    /// <summary>
    ///     Adds a singleton service of the type specified in
    ///     <typeparamref name = "TService" />
    ///     with an
    ///     instance specified in
    ///     <paramref name = "implementationInstance" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "implementationInstance" > The instance of the service. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Singleton" />
    public static WebApplicationBuilder AddSingleton<TService>( this WebApplicationBuilder builder, TService implementationInstance ) where TService : class => builder.AddSingleton( typeof(TService), implementationInstance );

    /// <summary>
    ///     Adds a singleton service of the type specified in
    ///     <typeparamref name = "TService" />
    ///     with a
    ///     factory specified in
    ///     <paramref name = "implementationFactory" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <typeparam name = "TService" > The type of the service to add. </typeparam>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "implementationFactory" > The factory that creates the service. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Singleton" />
    public static WebApplicationBuilder AddSingleton<TService>( this WebApplicationBuilder builder, Func<IServiceProvider, TService> implementationFactory ) where TService : class => builder.AddSingleton( typeof(TService), implementationFactory );

    /// <summary>
    ///     Adds a singleton service of the type specified in
    ///     <typeparamref name = "TService" />
    ///     with an
    ///     implementation type specified in
    ///     <typeparamref name = "TImplementation" />
    ///     using the
    ///     factory specified in
    ///     <paramref name = "implementationFactory" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <typeparam name = "TService" > The type of the service to add. </typeparam>
    /// <typeparam name = "TImplementation" > The type of the implementation to use. </typeparam>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "implementationFactory" > The factory that creates the service. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Singleton" />
    public static WebApplicationBuilder AddSingleton<TService, TImplementation>( this WebApplicationBuilder builder, Func<IServiceProvider, TImplementation> implementationFactory ) where TService : class
                                                                                                                                                                                     where TImplementation : class, TService =>
        builder.AddSingleton( typeof(TService), implementationFactory );

    /// <summary>
    ///     Adds a singleton service of the type specified in
    ///     <paramref name = "serviceType" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "serviceType" > The type of the service to register and the implementation to use. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Singleton" />
    public static WebApplicationBuilder AddSingleton( this WebApplicationBuilder builder, Type serviceType ) => builder.AddSingleton( serviceType, serviceType );

    /// <summary>
    ///     Adds a singleton service of the type specified in
    ///     <paramref name = "serviceType" />
    ///     with an
    ///     instance specified in
    ///     <paramref name = "implementationInstance" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "serviceType" > The type of the service to register. </param>
    /// <param name = "implementationInstance" > The instance of the service. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Singleton" />
    public static WebApplicationBuilder AddSingleton( this WebApplicationBuilder builder, Type serviceType, object implementationInstance )
    {
        var serviceDescriptor = new ServiceDescriptor( serviceType, implementationInstance );
        builder.Services.Add( serviceDescriptor );
        return builder;
    }


    /// <summary>
    ///     Adds a singleton service of the type specified in
    ///     <paramref name = "serviceType" />
    ///     with an
    ///     implementation of the type specified in
    ///     <paramref name = "implementationType" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "serviceType" > The type of the service to register. </param>
    /// <param name = "implementationType" > The implementation type of the service. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Singleton" />
    public static WebApplicationBuilder AddSingleton( this WebApplicationBuilder builder, Type serviceType, Type implementationType ) => TryAdd( builder, serviceType, implementationType, ServiceLifetime.Singleton );

    /// <summary>
    ///     Adds a singleton service of the type specified in
    ///     <paramref name = "serviceType" />
    ///     with a
    ///     factory specified in
    ///     <paramref name = "implementationFactory" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "serviceType" > The type of the service to register. </param>
    /// <param name = "implementationFactory" > The factory that creates the service. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Singleton" />
    public static WebApplicationBuilder AddSingleton( this WebApplicationBuilder builder, Type serviceType, Func<IServiceProvider, object> implementationFactory ) => TryAdd( builder, serviceType, implementationFactory, ServiceLifetime.Singleton );
    /// <summary>
    ///     Adds a transient service of the type specified in
    ///     <typeparamref name = "TService" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <typeparam name = "TService" > The type of the service to add. </typeparam>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Transient" />
    public static WebApplicationBuilder AddTransient<TService>( this WebApplicationBuilder builder ) where TService : class => builder.AddTransient( typeof(TService) );

    /// <summary>
    ///     Adds a transient service of the type specified in
    ///     <typeparamref name = "TService" />
    ///     with a
    ///     factory specified in
    ///     <paramref name = "implementationFactory" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <typeparam name = "TService" > The type of the service to add. </typeparam>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "implementationFactory" > The factory that creates the service. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Transient" />
    public static WebApplicationBuilder AddTransient<TService>( this WebApplicationBuilder builder, Func<IServiceProvider, TService> implementationFactory ) where TService : class => builder.AddTransient( typeof(TService), implementationFactory );

    /// <summary>
    ///     Adds a transient service of the type specified in
    ///     <typeparamref name = "TService" />
    ///     with an
    ///     implementation type specified in
    ///     <typeparamref name = "TImplementation" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <typeparam name = "TService" > The type of the service to add. </typeparam>
    /// <typeparam name = "TImplementation" > The type of the implementation to use. </typeparam>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Transient" />
    public static WebApplicationBuilder AddTransient<TService, TImplementation>( this WebApplicationBuilder builder ) where TService : class
                                                                                                                      where TImplementation : class, TService =>
        builder.AddTransient( typeof(TService), typeof(TImplementation) );

    /// <summary>
    ///     Adds a transient service of the type specified in
    ///     <typeparamref name = "TService" />
    ///     with an
    ///     implementation type specified in
    ///     <typeparamref name = "TImplementation" />
    ///     using the
    ///     factory specified in
    ///     <paramref name = "implementationFactory" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <typeparam name = "TService" > The type of the service to add. </typeparam>
    /// <typeparam name = "TImplementation" > The type of the implementation to use. </typeparam>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "implementationFactory" > The factory that creates the service. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Transient" />
    public static WebApplicationBuilder AddTransient<TService, TImplementation>( this WebApplicationBuilder builder, Func<IServiceProvider, TImplementation> implementationFactory ) where TService : class
                                                                                                                                                                                     where TImplementation : class, TService =>
        builder.AddTransient( typeof(TService), implementationFactory );

    /// <summary>
    ///     Adds a transient service of the type specified in
    ///     <paramref name = "serviceType" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "serviceType" > The type of the service to register and the implementation to use. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Transient" />
    public static WebApplicationBuilder AddTransient( this WebApplicationBuilder builder, Type serviceType ) => builder.AddTransient( serviceType, serviceType );

    /// <summary>
    ///     Adds a transient service of the type specified in
    ///     <paramref name = "serviceType" />
    ///     with an
    ///     implementation of the type specified in
    ///     <paramref name = "implementationType" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "serviceType" > The type of the service to register. </param>
    /// <param name = "implementationType" > The implementation type of the service. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Transient" />
    public static WebApplicationBuilder AddTransient( this WebApplicationBuilder builder, Type serviceType, Type implementationType ) => TryAdd( builder, serviceType, implementationType, ServiceLifetime.Transient );

    /// <summary>
    ///     Adds a transient service of the type specified in
    ///     <paramref name = "serviceType" />
    ///     with a
    ///     factory specified in
    ///     <paramref name = "implementationFactory" />
    ///     to the
    ///     specified
    ///     <see cref = "WebApplicationBuilder" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add the service to.
    /// </param>
    /// <param name = "serviceType" > The type of the service to register. </param>
    /// <param name = "implementationFactory" > The factory that creates the service. </param>
    /// <returns> A reference to this instance after the operation has completed. </returns>
    /// <seealso cref = "ServiceLifetime.Transient" />
    public static WebApplicationBuilder AddTransient( this WebApplicationBuilder builder, Type serviceType, Func<IServiceProvider, object> implementationFactory ) => TryAdd( builder, serviceType, implementationFactory, ServiceLifetime.Transient );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool Contains( this WebApplicationBuilder builder, Type serviceType ) => builder.Services.Any( x => x.ServiceType == serviceType );


    public static WebApplicationBuilder TryAdd( WebApplicationBuilder builder, Type serviceType, Type implementationType, in ServiceLifetime lifetime )
    {
        if (builder.Contains( serviceType )) { return builder; }

        var descriptor = new ServiceDescriptor( serviceType, implementationType, lifetime );
        builder.Services.Add( descriptor );
        return builder;
    }
    public static WebApplicationBuilder TryAdd( WebApplicationBuilder builder, Type serviceType, Func<IServiceProvider, object> implementationFactory, in ServiceLifetime lifetime )
    {
        if (builder.Contains( serviceType )) { return builder; }

        var descriptor = new ServiceDescriptor( serviceType, implementationFactory, lifetime );
        builder.Services.Add( descriptor );
        return builder;
    }
    public static bool TryAdd( this WebApplicationBuilder builder, ServiceDescriptor descriptor )
    {
        if (builder.Contains( descriptor.ServiceType )) { return false; }

        builder.Services.Add( descriptor );
        return true;
    }
}
