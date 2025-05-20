// Jakar.Extensions :: Jakar.Extensions
// 04/22/2024  14:04


namespace Jakar.Extensions.Loggers;



/*
public sealed class FileLoggerProviderOptions : IOptions<FileLoggerProviderOptions>
{
    public const string                                           FILE_NAME = "App.logs";
    public       string?                                          AppName   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public       Encoding                                         Encoding  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = Encoding.Default;
    public       Func<FileLoggerProvider.LogEvent, string>        Formatter { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = DefaultFormatter;
    public       LocalFile                                        File      { get;                                                      set; } = FILE_NAME;
    FileLoggerProviderOptions IOptions<FileLoggerProviderOptions>.Value     => this;
    public OneOf<LocalFile, FileLoggerRolloverOptions>            Mode      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = new LocalFile( FILE_NAME );


    public static string DefaultFormatter( FileLoggerProvider.LogEvent log ) => log.ToString();
}



public readonly record struct FileLoggerRolloverOptions( LocalDirectory Directory, TimeSpan? LifeSpan, int MaxFiles = 10, long MaxSize = 10_485_760 );
*/
