namespace Jakar.Extensions.Hosting;


public partial class WebBuilder
{
    /// <summary>
    ///     Adds services for controllers to the specified
    ///     <see cref = "IServiceCollection" />
    ///     . This method will not
    ///     register services used for views or pages.
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add services to.
    /// </param>
    /// <returns>
    ///     An
    ///     <see cref = "IMvcBuilder" />
    ///     that can be used to further configure the MVC services.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This method configures the MVC services for the commonly used features with controllers for an API. This
    ///         combines the effects of
    ///         <see cref = "MvcCoreServiceCollectionExtensions.AddMvcCore(IServiceCollection)" />
    ///         ,
    ///         <see cref = "MvcApiExplorerMvcCoreBuilderExtensions.AddApiExplorer(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcCoreMvcCoreBuilderExtensions.AddAuthorization(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcCorsMvcCoreBuilderExtensions.AddCors(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcDataAnnotationsMvcCoreBuilderExtensions.AddDataAnnotations(IMvcCoreBuilder)" />
    ///         ,
    ///         and
    ///         <see cref = "MvcCoreMvcCoreBuilderExtensions.AddFormatterMappings(IMvcCoreBuilder)" />
    ///         .
    ///     </para>
    ///     <para>
    ///         To add services for controllers with views call
    ///         <see cref = "MvcServiceCollectionExtensions.AddControllersWithViews(IServiceCollection)" />
    ///         on the resulting builder.
    ///     </para>
    ///     <para>
    ///         To add services for pages call
    ///         <see cref = "MvcServiceCollectionExtensions.AddRazorPages(IServiceCollection)" />
    ///         on the resulting builder.
    ///     </para>
    /// </remarks>
    public static IMvcBuilder AddControllers( this WebApplicationBuilder builder ) => builder.Services.AddControllers();

    /// <summary>
    ///     Adds services for controllers to the specified
    ///     <see cref = "IServiceCollection" />
    ///     . This method will not
    ///     register services used for views or pages.
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add services to.
    /// </param>
    /// <param name = "configure" >
    ///     An
    ///     <see cref = "Action{MvcOptions}" />
    ///     to configure the provided
    ///     <see cref = "MvcOptions" />
    ///     .
    /// </param>
    /// <returns>
    ///     An
    ///     <see cref = "IMvcBuilder" />
    ///     that can be used to further configure the MVC services.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This method configures the MVC services for the commonly used features with controllers for an API. This
    ///         combines the effects of
    ///         <see cref = "MvcCoreServiceCollectionExtensions.AddMvcCore(IServiceCollection)" />
    ///         ,
    ///         <see cref = "MvcApiExplorerMvcCoreBuilderExtensions.AddApiExplorer(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcCoreMvcCoreBuilderExtensions.AddAuthorization(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcCorsMvcCoreBuilderExtensions.AddCors(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcDataAnnotationsMvcCoreBuilderExtensions.AddDataAnnotations(IMvcCoreBuilder)" />
    ///         ,
    ///         and
    ///         <see cref = "MvcCoreMvcCoreBuilderExtensions.AddFormatterMappings(IMvcCoreBuilder)" />
    ///         .
    ///     </para>
    ///     <para>
    ///         To add services for controllers with views call
    ///         <see cref = "MvcServiceCollectionExtensions.AddControllersWithViews(IServiceCollection)" />
    ///         on the resulting builder.
    ///     </para>
    ///     <para>
    ///         To add services for pages call
    ///         <see cref = "MvcServiceCollectionExtensions.AddRazorPages(IServiceCollection)" />
    ///         on the resulting builder.
    ///     </para>
    /// </remarks>
    public static IMvcBuilder AddControllers( this WebApplicationBuilder builder, Action<MvcOptions>? configure ) => builder.Services.AddControllers( configure );


    /// <summary>
    ///     Adds services for controllers to the specified
    ///     <see cref = "IServiceCollection" />
    ///     . This method will not
    ///     register services used for pages.
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add services to.
    /// </param>
    /// <returns>
    ///     An
    ///     <see cref = "IMvcBuilder" />
    ///     that can be used to further configure the MVC services.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This method configures the MVC services for the commonly used features with controllers with views. This
    ///         combines the effects of
    ///         <see cref = "MvcCoreServiceCollectionExtensions.AddMvcCore(IServiceCollection)" />
    ///         ,
    ///         <see cref = "MvcApiExplorerMvcCoreBuilderExtensions.AddApiExplorer(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcCoreMvcCoreBuilderExtensions.AddAuthorization(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcCorsMvcCoreBuilderExtensions.AddCors(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcDataAnnotationsMvcCoreBuilderExtensions.AddDataAnnotations(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcCoreMvcCoreBuilderExtensions.AddFormatterMappings(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "TagHelperServicesExtensions.AddCacheTagHelper(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcViewFeaturesMvcCoreBuilderExtensions.AddViews(IMvcCoreBuilder)" />
    ///         ,
    ///         and
    ///         <see cref = "MvcRazorMvcCoreBuilderExtensions.AddRazorViewEngine(IMvcCoreBuilder)" />
    ///         .
    ///     </para>
    ///     <para>
    ///         To add services for pages call
    ///         <see cref = "MvcServiceCollectionExtensions.AddRazorPages(IServiceCollection)" />
    ///         .
    ///     </para>
    /// </remarks>
    public static IMvcBuilder AddControllersWithViews( this WebApplicationBuilder builder ) => builder.Services.AddControllersWithViews();

    /// <summary>
    ///     Adds services for controllers to the specified
    ///     <see cref = "IServiceCollection" />
    ///     . This method will not
    ///     register services used for pages.
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add services to.
    /// </param>
    /// <param name = "configure" >
    ///     An
    ///     <see cref = "Action{MvcOptions}" />
    ///     to configure the provided
    ///     <see cref = "MvcOptions" />
    ///     .
    /// </param>
    /// <returns>
    ///     An
    ///     <see cref = "IMvcBuilder" />
    ///     that can be used to further configure the MVC services.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This method configures the MVC services for the commonly used features with controllers with views. This
    ///         combines the effects of
    ///         <see cref = "MvcCoreServiceCollectionExtensions.AddMvcCore(IServiceCollection)" />
    ///         ,
    ///         <see cref = "MvcApiExplorerMvcCoreBuilderExtensions.AddApiExplorer(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcCoreMvcCoreBuilderExtensions.AddAuthorization(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcCorsMvcCoreBuilderExtensions.AddCors(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcDataAnnotationsMvcCoreBuilderExtensions.AddDataAnnotations(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcCoreMvcCoreBuilderExtensions.AddFormatterMappings(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "TagHelperServicesExtensions.AddCacheTagHelper(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcViewFeaturesMvcCoreBuilderExtensions.AddViews(IMvcCoreBuilder)" />
    ///         ,
    ///         and
    ///         <see cref = "MvcRazorMvcCoreBuilderExtensions.AddRazorViewEngine(IMvcCoreBuilder)" />
    ///         .
    ///     </para>
    ///     <para>
    ///         To add services for pages call
    ///         <see cref = "MvcServiceCollectionExtensions.AddRazorPages(IServiceCollection)" />
    ///         .
    ///     </para>
    /// </remarks>
    public static IMvcBuilder AddControllersWithViews( this WebApplicationBuilder builder, Action<MvcOptions>? configure ) => builder.Services.AddControllersWithViews( configure );


    /// <summary>
    ///     Adds MVC services to the specified
    ///     <see cref = "IServiceCollection" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add services to.
    /// </param>
    /// <returns>
    ///     An
    ///     <see cref = "IMvcBuilder" />
    ///     that can be used to further configure the MVC services.
    /// </returns>
    public static IMvcBuilder AddMvc( this WebApplicationBuilder builder ) => builder.Services.AddMvc();

    /// <summary>
    ///     Adds MVC services to the specified
    ///     <see cref = "IServiceCollection" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add services to.
    /// </param>
    /// <param name = "setupAction" >
    ///     An
    ///     <see cref = "Action{MvcOptions}" />
    ///     to configure the provided
    ///     <see cref = "MvcOptions" />
    ///     .
    /// </param>
    /// <returns>
    ///     An
    ///     <see cref = "IMvcBuilder" />
    ///     that can be used to further configure the MVC services.
    /// </returns>
    public static IMvcBuilder AddMvc( this WebApplicationBuilder builder, Action<MvcOptions> setupAction ) => builder.Services.AddMvc( setupAction );


    /// <summary>
    ///     Adds services for pages to the specified
    ///     <see cref = "IServiceCollection" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add services to.
    /// </param>
    /// <returns>
    ///     An
    ///     <see cref = "IMvcBuilder" />
    ///     that can be used to further configure the MVC services.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This method configures the MVC services for the commonly used features for pages. This
    ///         combines the effects of
    ///         <see cref = "MvcCoreServiceCollectionExtensions.AddMvcCore(IServiceCollection)" />
    ///         ,
    ///         <see cref = "MvcCoreMvcCoreBuilderExtensions.AddAuthorization(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcDataAnnotationsMvcCoreBuilderExtensions.AddDataAnnotations(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "TagHelperServicesExtensions.AddCacheTagHelper(IMvcCoreBuilder)" />
    ///         ,
    ///         and
    ///         <see cref = "MvcRazorPagesMvcCoreBuilderExtensions.AddRazorPages(IMvcCoreBuilder)" />
    ///         .
    ///     </para>
    ///     <para>
    ///         To add services for controllers for APIs call
    ///         <see cref = "MvcServiceCollectionExtensions.AddControllers(IServiceCollection)" />
    ///         .
    ///     </para>
    ///     <para>
    ///         To add services for controllers with views call
    ///         <see cref = "MvcServiceCollectionExtensions.AddControllersWithViews(IServiceCollection)" />
    ///         .
    ///     </para>
    /// </remarks>
    public static IMvcBuilder AddRazorPages( this WebApplicationBuilder builder ) => builder.Services.AddRazorPages();

    /// <summary>
    ///     Adds services for pages to the specified
    ///     <see cref = "IServiceCollection" />
    ///     .
    /// </summary>
    /// <param name = "builder" >
    ///     The
    ///     <see cref = "WebApplicationBuilder" />
    ///     to add services to.
    /// </param>
    /// <param name = "configure" >
    ///     An
    ///     <see cref = "Action{MvcOptions}" />
    ///     to configure the provided
    ///     <see cref = "MvcOptions" />
    ///     .
    /// </param>
    /// <returns>
    ///     An
    ///     <see cref = "IMvcBuilder" />
    ///     that can be used to further configure the MVC services.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This method configures the MVC services for the commonly used features for pages. This
    ///         combines the effects of
    ///         <see cref = "MvcCoreServiceCollectionExtensions.AddMvcCore(IServiceCollection)" />
    ///         ,
    ///         <see cref = "MvcCoreMvcCoreBuilderExtensions.AddAuthorization(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "MvcDataAnnotationsMvcCoreBuilderExtensions.AddDataAnnotations(IMvcCoreBuilder)" />
    ///         ,
    ///         <see cref = "TagHelperServicesExtensions.AddCacheTagHelper(IMvcCoreBuilder)" />
    ///         ,
    ///         and
    ///         <see cref = "MvcRazorPagesMvcCoreBuilderExtensions.AddRazorPages(IMvcCoreBuilder)" />
    ///         .
    ///     </para>
    ///     <para>
    ///         To add services for controllers for APIs call
    ///         <see cref = "MvcServiceCollectionExtensions.AddControllers(IServiceCollection)" />
    ///         .
    ///     </para>
    ///     <para>
    ///         To add services for controllers with views call
    ///         <see cref = "MvcServiceCollectionExtensions.AddControllersWithViews(IServiceCollection)" />
    ///         .
    ///     </para>
    /// </remarks>
    public static IMvcBuilder AddRazorPages( this WebApplicationBuilder builder, Action<RazorPagesOptions>? configure ) => builder.Services.AddRazorPages( configure );
    public static IServerSideBlazorBuilder AddServerSideBlazor( this WebApplicationBuilder builder, Action<CircuitOptions>? configure = null ) => builder.Services.AddServerSideBlazor( configure );
}
