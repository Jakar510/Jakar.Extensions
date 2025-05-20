// Jakar.Extensions :: Jakar.Extensions
// 04/22/2024  14:04

using Microsoft.Extensions.DependencyInjection;



namespace Jakar.Extensions.Loggers;



/*
public static class FileLoggerProviderExtensions
{
    public static ILoggingBuilder AddFileLogger( this ILoggingBuilder builder, FileLoggerProviderOptions options )
    {
        builder.Services.AddOptions();
        builder.Services.AddSingleton<IOptions<FileLoggerProviderOptions>>( options );
        builder.Services.AddSingleton( new FileLoggerProvider( options ) );
        builder.Services.AddHostedService( static provider => provider.GetRequiredService<FileLoggerProvider>() );
        builder.Services.AddTransient<ILoggerProvider>( static provider => provider.GetRequiredService<FileLoggerProvider>() );
        return builder;
    }
}
*/
