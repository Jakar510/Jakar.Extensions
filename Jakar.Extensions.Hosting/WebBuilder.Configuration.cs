﻿using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;



namespace Jakar.Extensions.Hosting;


public static partial class WebBuilder
{
    public const string CONNECTION_STRINGS = "ConnectionStrings";
    public const string DEFAULT            = "Default";


    public static string ConnectionString( this IConfiguration configuration, string name = DEFAULT ) => configuration.GetSection( CONNECTION_STRINGS )
                                                                                                                      .GetValue<string>( name ) ?? throw new InvalidOperationException( $"{CONNECTION_STRINGS}::{DEFAULT} is not found" );
    public static string ConnectionString( this IServiceProvider provider, string name = DEFAULT ) => provider.GetRequiredService<IConfiguration>()
                                                                                                              .ConnectionString( name );
    public static string ConnectionString( this WebApplication configuration, string name = DEFAULT ) => configuration.Services.ConnectionString( name );


    public static IConfigurationBuilder AddCommandLine( this          WebApplicationBuilder builder, string[] args ) => builder.Configuration.AddCommandLine( args );
    public static IConfigurationBuilder AddCommandLine( this          WebApplicationBuilder builder, string[] args, IDictionary<string, string> switchMappings ) => builder.Configuration.AddCommandLine( args, switchMappings );
    public static IConfigurationBuilder AddCommandLine( this          WebApplicationBuilder builder, Action<CommandLineConfigurationSource> configureSource ) => builder.Configuration.AddCommandLine( configureSource );
    public static IConfigurationBuilder AddEnvironmentVariables( this WebApplicationBuilder builder ) => builder.Configuration.AddEnvironmentVariables();
    public static IConfigurationBuilder AddEnvironmentVariables( this WebApplicationBuilder builder, string prefix ) => builder.Configuration.AddEnvironmentVariables( prefix );
    public static IConfigurationBuilder AddEnvironmentVariables( this WebApplicationBuilder builder, params string[] prefix )
    {
        foreach ( string s in prefix ) { builder.Configuration.AddEnvironmentVariables( s ); }

        return builder.Configuration;
    }


    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, string path ) => builder.Configuration.AddIniFile( path );
    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, string path, bool optional ) => builder.Configuration.AddIniFile( path, optional );
    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, string path, bool optional, bool reloadOnChange ) => builder.Configuration.AddIniFile( path, optional, reloadOnChange );
    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange ) => builder.Configuration.AddIniFile( provider, path, optional, reloadOnChange );
    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, Action<IniConfigurationSource> configureSource ) => builder.Configuration.AddIniFile( configureSource );
    public static IConfigurationBuilder AddIniStream( this WebApplicationBuilder builder, Stream stream ) => builder.Configuration.AddIniStream( stream );


    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, string path ) => builder.Configuration.AddJsonFile( path );
    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, string path, bool optional ) => builder.Configuration.AddJsonFile( path, optional );
    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, string path, bool optional, bool reloadOnChange ) => builder.Configuration.AddJsonFile( path, optional, reloadOnChange );
    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange ) => builder.Configuration.AddJsonFile( provider, path, optional, reloadOnChange );
    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, Action<JsonConfigurationSource> configureSource ) => builder.Configuration.AddJsonFile( configureSource );
    public static IConfigurationBuilder AddJsonStream( this WebApplicationBuilder builder, Stream stream ) => builder.Configuration.AddJsonStream( stream );
}
