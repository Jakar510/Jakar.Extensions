using FluentMigrator;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Logging;
using Microsoft.OpenApi.Models;


string[] urls =
{
    "https://localhost",
    "https://0.0.0.0:443",
    "https://0.0.0.0:6969"
};


WebApplicationBuilder builder = WebApplication.CreateBuilder( args );
builder.AddEnvironmentVariables( "LOGGER_", "ASPNET_", "DOTNET_" );


builder.AddDefaultLogging<AppLoggerPortal>( true )
       .AddProvider( new FluentMigratorConsoleLoggerProvider( new OptionsWrapper<FluentMigratorLoggerOptions>( new FluentMigratorLoggerOptions
                                                                                                               {
                                                                                                                   ShowElapsedTime = true,
                                                                                                                   ShowSql         = true
                                                                                                               } ) ) );


builder.Services.AddControllers().UseNewtonsoftJson();

builder.Services.AddControllersWithViews().UseNewtonsoftJson();

builder.Services.AddHttpClient();
builder.Services.AddHealthChecks();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
builder.AddAppServices();

builder.AddDatabase<LoggerDB>( options =>
                               {
                                   options.DbType        = DbInstance.Postgres;
                                   options.Version       = AppVersion.FromAssembly<Program>();
                                   options.TokenAudience = nameof(AppLoggerPortal);
                                   options.TokenIssuer   = nameof(AppLoggerPortal);
                               } );

builder.Services.AddScoped<CircuitHandler, ScriptManagerCircuitHandler>();
builder.AddTokenizer();
builder.AddEmailer();
builder.Services.AddWebEncoders();
builder.Services.AddEndpointsApiExplorer();


const string VERSION = "V1";

builder.Services.AddSwaggerGen( swagger =>
                                {
                                    swagger.SwaggerDoc( VERSION,
                                                        new OpenApiInfo
                                                        {
                                                            Title          = nameof(AppLoggerPortal),
                                                            Description    = "Cross-platform application logger framework",
                                                            Version        = "V1",
                                                            TermsOfService = new Uri( "https://github.com/Jakar510/Jakar.AppLogger/blob/master/LICENSE.md" ),
                                                            Contact = new OpenApiContact
                                                                      {
                                                                          Url  = new Uri( "https://github.com/Jakar510/Jakar.AppLogger" ),
                                                                          Name = "Jakar.AppLogger"
                                                                      },
                                                            License = new OpenApiLicense
                                                                      {
                                                                          Url  = new Uri( "https://www.gnu.org/licenses/gpl-3.0.en.html" ),
                                                                          Name = "GNU General Public License v3.0"
                                                                      }
                                                        } );


                                    // swagger.AddServer(new OpenApiServer()
                                    //                   {
                                    //                       Url         = "https://AppLogger.com",
                                    //                       Description = type.Name,
                                    //                   });


                                    // var jwtScheme = new OpenApiSecurityScheme
                                    //                 {
                                    //                     Name        = "JwtToken",
                                    //                     Description = "Bearer Token",
                                    //                     Type        = SecuritySchemeType.Http,
                                    //                     Scheme      = JwtBearerDefaults.AuthenticationScheme,
                                    //                     In          = ParameterLocation.Cookie
                                    //                 };
                                    //
                                    // swagger.AddSecurityDefinition( nameof(jwtScheme), jwtScheme );
                                    //
                                    //
                                    // var apiScheme = new OpenApiSecurityScheme
                                    //                 {
                                    //                     Name        = "Authorization",
                                    //                     Description = "Authorization Api Key",
                                    //                     Type        = SecuritySchemeType.ApiKey,
                                    //                     Scheme      = "ApiKey",
                                    //                     In          = ParameterLocation.Header
                                    //                 };
                                    //
                                    // swagger.AddSecurityDefinition( nameof(apiScheme), apiScheme );
                                    //
                                    //
                                    // swagger.AddSecurityRequirement( new OpenApiSecurityRequirement
                                    //                                 {
                                    //                                     {
                                    //                                         new OpenApiSecurityScheme
                                    //                                         {
                                    //                                             Reference = new OpenApiReference
                                    //                                                         {
                                    //                                                             Id   = apiScheme.Scheme,
                                    //                                                             Type = ReferenceType.SecurityScheme
                                    //                                                         }
                                    //                                         },
                                    //                                         new List<string>() // scopes
                                    //                                     },
                                    //                                     {
                                    //                                         new OpenApiSecurityScheme
                                    //                                         {
                                    //                                             Reference = new OpenApiReference
                                    //                                                         {
                                    //                                                             Id   = jwtScheme.Scheme,
                                    //                                                             Type = ReferenceType.SecurityScheme
                                    //                                                         }
                                    //                                         },
                                    //                                         new List<string>() // scopes
                                    //                                     }
                                    //                                 } );


                                    // swagger.EnableAnnotations();
                                    swagger.SwaggerGeneratorOptions.IgnoreObsoleteActions            = true;
                                    swagger.SwaggerGeneratorOptions.DescribeAllParametersInCamelCase = false;


                                    // swagger.OperationFilter<ApiKeyAttribute>();
                                    /*
                                    foreach ( (Type header, object[] args) in headers )
                                    {
                                        swagger.OperationFilterDescriptors.Add( new FilterDescriptor
                                                                                {
                                                                                    Type      = header,
                                                                                    Arguments = args
                                                                                } );
                                    }
                                    */
                                } );


builder.AddFluentMigrator( configure =>
                           {
                               configure.AddPostgres();

                               configure.ScanIn( typeof(Program).Assembly, typeof(Database).Assembly ).For.All();

                               return configure;
                           } );

builder.Services.AddTransient<IMigrationContext>( provider =>
                                                  {
                                                      var     querySchema              = provider.GetRequiredService<IQuerySchema>();
                                                      var     connectionStringAccessor = provider.GetRequiredService<IConnectionStringAccessor>();
                                                      string? connectionString         = connectionStringAccessor.ConnectionString;
                                                      return new MigrationContext( querySchema, provider, provider.GetRequiredService<Database>(), connectionString );
                                                  } );


// builder.Services.AddHsts(options =>
//                          {
//                              options.Preload           = true;
//                              options.IncludeSubDomains = true;
//                              options.MaxAge            = TimeSpan.FromDays(60);
//                          });


builder.Services.AddHttpsRedirection( options =>
                                      {
                                          options.RedirectStatusCode = (int)Status.PermanentRedirect;
                                          options.HttpsPort          = 443;
                                      } );


/*
// https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-6.0&tabs=visual-studio
// https://www.codeproject.com/Articles/5315010/How-to-Use-Certificates-in-ASP-NET-Core
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-6.0#replace-the-default-certificate-from-configuration

builder.WebHost.UseKestrel( options =>
                            {
                                const string FILE = "AppLogger.pfx";

                                options.ConfigureHttpsDefaults( listenOptions =>
                                                                {
                                                                    listenOptions.ServerCertificate          = new X509Certificate2( FILE );
                                                                    listenOptions.CheckCertificateRevocation = true;
                                                                } );

                                options.Listen( IPAddress.Any, 443, listenOptions => { listenOptions.UseHttps( FILE, default ); } );
                            } );
*/


await using WebApplication app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage( "/_Host" );


// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() )
{
    app.Logger.LogInformation( "\n UseSwagger \n" );
    app.UseSwagger();
    app.UseSwaggerUI( options => options.SwaggerEndpoint( $"/swagger/{VERSION}/swagger.json", $"{nameof(AppLoggerPortal)} {VERSION}" ) );

    app.UseExceptionHandler( "/Error" );
}


try
{
    app.UseUrls( urls );
    app.Logger.LogInformation( "Starting Migrations" );
    await app.MigrateUp();


#if DEBUG
    _ = Task.Run( async () =>
                  {
                      try
                      {
                          await TimeSpan.FromSeconds( 1 )
                                        .Delay();

                          using WebRequester requester = WebRequester.Builder.Create( new Uri( "https://localhost/" ) )
                                                                     .Build();

                          WebResponse<JToken> openApiSpec = await requester.Get( $"swagger/{VERSION}/swagger.json", default )
                                                                           .AsJson();


                          using var file = new LocalFile( $"W:\\WorkSpace\\Jakar.AppLogger\\api_spec.{VERSION}.json" );

                          if ( file.Parent?.Exists is true )
                          {
                              file.FullPath.WriteToConsole();

                              await file.WriteAsync( openApiSpec.GetPayload()
                                                                .ToString( Formatting.Indented ) );
                          }
                      }
                      catch ( Exception e ) { e.WriteToConsole(); }
                  } );
#endif


    app.Logger.LogInformation( "Services Starting" );
    await app.StartAsync();
    await app.WaitForShutdownAsync();
}
catch ( Exception e )
{
    e.WriteToDebug();
    app.Logger.LogCritical( e, nameof(Program) );
}
