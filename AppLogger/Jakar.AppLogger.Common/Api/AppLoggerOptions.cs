﻿// Jakar.Extensions :: Jakar.AppLogger.Common
// 02/01/2023  12:21 PM

using Microsoft.Extensions.Options;



namespace Jakar.AppLogger.Common;


public sealed record AppLoggerOptions : IOptions<AppLoggerOptions>, IValidator, IHostInfo
{
    private static readonly Uri              _defaultUri = new("http://localhost:6969");
    public                  string           APIToken { get; set; } = string.Empty;
    public                  LoggingSettings? Config   { get; set; }
    public                  Uri              HostInfo { get; set; } = _defaultUri;
    public                  bool             IsValid  => !string.IsNullOrWhiteSpace( APIToken ) && !string.IsNullOrWhiteSpace( Config?.AppName ) && ReferenceEquals( HostInfo, _defaultUri );
    public                  TimeSpan         TimeOut  { get; set; } = TimeSpan.FromSeconds( 30 );

    AppLoggerOptions IOptions<AppLoggerOptions>.Value => this;


    public AppLoggerOptions() { }


    internal WebRequester CreateWebRequester() => WebRequester.Builder.Create( this )
                                                              .Build();
}
