namespace Jakar.Extensions.Hosting;


/// <summary> Extension methods to configure an <see cref="WebApplicationBuilder"/> for <see cref="IHttpClientFactory"/> . </summary>
public partial class WebBuilder
{
    /// <summary> Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a named <see cref="HttpClient"/> . </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="name"> The logical name of the <see cref="HttpClient"/> to configure. </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para> Use <see cref="Options.DefaultName"/> as the name to configure the default client. </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient( this WebApplicationBuilder builder, string name ) => builder.Services.AddHttpClient( name );

    /// <summary> Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a named <see cref="HttpClient"/> . </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="name"> The logical name of the <see cref="HttpClient"/> to configure. </param>
    /// <param name="configureClient"> A delegate that is used to configure an <see cref="HttpClient"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para> Use <see cref="Options.DefaultName"/> as the name to configure the default client. </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient( this WebApplicationBuilder builder, string name, Action<HttpClient> configureClient ) => builder.Services.AddHttpClient( name, configureClient );

    /// <summary> Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a named <see cref="HttpClient"/> . </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="name"> The logical name of the <see cref="HttpClient"/> to configure. </param>
    /// <param name="configureClient"> A delegate that is used to configure an <see cref="HttpClient"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para> Use <see cref="Options.DefaultName"/> as the name to configure the default client. </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient( this WebApplicationBuilder builder, string name, Action<IServiceProvider, HttpClient> configureClient ) => builder.Services.AddHttpClient( name, configureClient );

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     . The client name will be set to the type name of
    ///     <typeparamref
    ///         name="TClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicConstructors )] TClient>( this WebApplicationBuilder builder ) where TClient : class => builder.Services.AddHttpClient<TClient>();

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     . The client name will be set to the type name of
    ///     <typeparamref
    ///         name="TClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <typeparam name="TImplementation"> The implementation type of the typed client. The type specified will be instantiated by the <see cref="ITypedHttpClientFactory{TImplementation}"/> </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<TClient, [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicConstructors )] TImplementation>( this WebApplicationBuilder builder ) where TClient : class
                                                                                                                                                                                                     where TImplementation : class, TClient =>
        builder.Services.AddHttpClient<TImplementation>();

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="name"> The logical name of the <see cref="HttpClient"/> to configure. </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    ///     <para> Use <see cref="Options.DefaultName"/> as the name to configure the default client. </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicConstructors )] TClient>( this WebApplicationBuilder builder, string name ) where TClient : class =>
        builder.Services.AddHttpClient<TClient>( name );

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     . The client name will be set to the type name of
    ///     <typeparamref
    ///         name="TClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <typeparam name="TImplementation"> The implementation type of the typed client. The type specified will be instantiated by the <see cref="ITypedHttpClientFactory{TImplementation}"/> </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="name"> The logical name of the <see cref="HttpClient"/> to configure. </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    ///     <para> Use <see cref="Options.DefaultName"/> as the name to configure the default client. </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<TClient, [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicConstructors )] TImplementation>( this WebApplicationBuilder builder, string name ) where TClient : class
                                                                                                                                                                                                                  where TImplementation : class,
                                                                                                                                                                                                                  TClient =>
        builder.Services.AddHttpClient<TClient, TImplementation>( name );

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     . The client name will be set to the type name of
    ///     <typeparamref
    ///         name="TClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="configureClient"> A delegate that is used to configure an <see cref="HttpClient"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicConstructors )] TClient>( this WebApplicationBuilder builder, Action<HttpClient> configureClient ) where TClient : class =>
        builder.Services.AddHttpClient<TClient>( configureClient );

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     . The client name will be set to the type name of
    ///     <typeparamref
    ///         name="TClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="configureClient"> A delegate that is used to configure an <see cref="HttpClient"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicConstructors )] TClient>( this WebApplicationBuilder builder, Action<IServiceProvider, HttpClient> configureClient )
        where TClient : class => builder.Services.AddHttpClient<TClient>( configureClient );

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     . The client name will be set to the type name of
    ///     <typeparamref
    ///         name="TClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <typeparam name="TImplementation"> The implementation type of the typed client. The type specified will be instantiated by the <see cref="ITypedHttpClientFactory{TImplementation}"/> </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="configureClient"> A delegate that is used to configure an <see cref="HttpClient"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<TClient, [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicConstructors )] TImplementation>( this WebApplicationBuilder builder, Action<HttpClient> configureClient )
        where TClient : class
        where TImplementation : class, TClient => builder.Services.AddHttpClient<TClient>( configureClient );

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     . The client name will be set to the type name of
    ///     <typeparamref
    ///         name="TClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <typeparam name="TImplementation"> The implementation type of the typed client. The type specified will be instantiated by the <see cref="ITypedHttpClientFactory{TImplementation}"/> </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="configureClient"> A delegate that is used to configure an <see cref="HttpClient"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<TClient, [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicConstructors )] TImplementation>( this WebApplicationBuilder builder, Action<IServiceProvider, HttpClient> configureClient )
        where TClient : class
        where TImplementation : class, TClient => builder.Services.AddHttpClient<TClient, TImplementation>( configureClient );

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="name"> The logical name of the <see cref="HttpClient"/> to configure. </param>
    /// <param name="configureClient"> A delegate that is used to configure an <see cref="HttpClient"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    ///     <para> Use <see cref="Options.DefaultName"/> as the name to configure the default client. </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicConstructors )] TClient>( this WebApplicationBuilder builder, string name, Action<HttpClient> configureClient )
        where TClient : class => builder.Services.AddHttpClient<TClient>( name, configureClient );

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="name"> The logical name of the <see cref="HttpClient"/> to configure. </param>
    /// <param name="configureClient"> A delegate that is used to configure an <see cref="HttpClient"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    ///     <para> Use <see cref="Options.DefaultName"/> as the name to configure the default client. </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicConstructors )] TClient>( this WebApplicationBuilder builder, string name, Action<IServiceProvider, HttpClient> configureClient )
        where TClient : class => builder.Services.AddHttpClient<TClient>( name, configureClient );

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <typeparam name="TImplementation"> The implementation type of the typed client. The type specified will be instantiated by the <see cref="ITypedHttpClientFactory{TImplementation}"/> </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="name"> The logical name of the <see cref="HttpClient"/> to configure. </param>
    /// <param name="configureClient"> A delegate that is used to configure an <see cref="HttpClient"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    ///     <para> Use <see cref="Options.DefaultName"/> as the name to configure the default client. </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<TClient, [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicConstructors )] TImplementation>( this WebApplicationBuilder builder, string name, Action<HttpClient> configureClient )
        where TClient : class
        where TImplementation : class, TClient => builder.Services.AddHttpClient<TClient, TImplementation>( name, configureClient );

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <typeparam name="TImplementation"> The implementation type of the typed client. The type specified will be instantiated by the <see cref="ITypedHttpClientFactory{TImplementation}"/> </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="name"> The logical name of the <see cref="HttpClient"/> to configure. </param>
    /// <param name="configureClient"> A delegate that is used to configure an <see cref="HttpClient"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    ///     <para> Use <see cref="Options.DefaultName"/> as the name to configure the default client. </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<TClient, [DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicConstructors )] TImplementation>(
        this WebApplicationBuilder           builder,
        string                               name,
        Action<IServiceProvider, HttpClient> configureClient
    ) where TClient : class
      where TImplementation : class, TClient => builder.Services.AddHttpClient<TClient, TImplementation>( name, configureClient );

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <typeparam name="TImplementation"> The implementation type of the typed client. </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="factory"> A delegate that is used to create an instance of <typeparamref name="TClient"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<TClient, TImplementation>( this WebApplicationBuilder builder, Func<HttpClient, TImplementation> factory ) where TClient : class
                                                                                                                                                              where TImplementation : class, TClient =>
        builder.Services.AddHttpClient<TClient, TImplementation>( factory );

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <typeparam name="TImplementation"> The implementation type of the typed client. </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="name"> The logical name of the <see cref="HttpClient"/> to configure. </param>
    /// <param name="factory"> A delegate that is used to create an instance of <typeparamref name="TClient"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    ///     <typeparamref name="TImplementation"> </typeparamref>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<TClient, TImplementation>( this WebApplicationBuilder builder, string name, Func<HttpClient, TImplementation> factory ) where TClient : class
                                                                                                                                                                           where TImplementation : class, TClient =>
        builder.Services.AddHttpClient<TClient, TImplementation>( name, factory );

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <typeparam name="TImplementation"> The implementation type of the typed client. </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="factory"> A delegate that is used to create an instance of <typeparamref name="TClient"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<TClient, TImplementation>( this WebApplicationBuilder builder, Func<HttpClient, IServiceProvider, TImplementation> factory ) where TClient : class
                                                                                                                                                                                where TImplementation : class, TClient =>
        builder.Services.AddHttpClient<TClient, TImplementation>( factory );

    /// <summary>
    ///     Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> and configures a binding between the <typeparamref name="TClient"/> type and a named
    ///     <see
    ///         cref="HttpClient"/>
    ///     .
    /// </summary>
    /// <typeparam name="TClient"> The type of the typed client. The type specified will be registered in the service collection as a transient service. See <see cref="ITypedHttpClientFactory{TClient}"/> for more details about authoring typed clients. </typeparam>
    /// <typeparam name="TImplementation"> The implementation type of the typed client. </typeparam>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <param name="name"> The logical name of the <see cref="HttpClient"/> to configure. </param>
    /// <param name="factory"> A delegate that is used to create an instance of <typeparamref name="TClient"/> . </param>
    /// <returns> An <see cref="IHttpClientBuilder"/> that can be used to configure the client. </returns>
    /// <remarks>
    ///     <para> <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name. </para>
    ///     <para>
    ///         <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient"/> can be retrieved from <see cref="IServiceProvider.GetService(Type)"/> (and related methods) by providing
    ///         <typeparamref
    ///             name="TClient"/>
    ///         as the service type.
    ///     </para>
    /// </remarks>
    public static IHttpClientBuilder AddHttpClient<TClient, TImplementation>( this WebApplicationBuilder builder, string name, Func<HttpClient, IServiceProvider, TImplementation> factory ) where TClient : class
                                                                                                                                                                                             where TImplementation : class, TClient =>
        builder.Services.AddHttpClient<TClient, TImplementation>( name, factory );
    /// <summary> Adds the <see cref="IHttpClientFactory"/> and related builder to the <see cref="WebApplicationBuilder"/> . </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> . </param>
    /// <returns> The <see cref="WebApplicationBuilder"/> . </returns>
    public static WebApplicationBuilder AddHttpClient( this WebApplicationBuilder builder )
    {
        builder.Services.AddHttpClient();
        return builder;
    }
}
