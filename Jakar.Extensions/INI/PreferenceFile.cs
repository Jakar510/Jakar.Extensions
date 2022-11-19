// #nullable enable
// using static Org.BouncyCastle.Math.EC.ECCurve;
//
//
//
// namespace Jakar.Extensions;
//
//
// public class PreferenceFile : IDisposable, IAsyncDisposable // TODO: Add watcher to update if file changes
// {
//     private readonly object     _lock = new();
//     protected        LocalFile? _file;
//     private          string?    _fileName;
//
//     public LocalFile Path
//     {
//         get => _file ??= LocalDirectory.CurrentDirectory.Join( FileName );
//         protected set => _file = value;
//     }
//     public virtual string    FileName => _fileName ??= $"{GetType().Name}.ini";
//     public         IniConfig Config   { get; }
//
//
//     public PreferenceFile() : this( new IniConfig() ) { }
//     public PreferenceFile( IniConfig config )
//     {
//         Config = config;
//         Load();
//     }
//
//     public static PreferenceFile Create( LocalFile file )
//     {
//         var ini = IniConfig.ReadFromFile( file ) ?? new IniConfig();
//
//         return new PreferenceFile( ini )
//                {
//                    Path = file,
//                };
//     }
//     public static async Task<PreferenceFile> CreateAsync( LocalFile file )
//     {
//         var ini = await IniConfig.ReadFromFileAsync( file ) ?? new IniConfig();
//
//         return new PreferenceFile( ini )
//                {
//                    Path = file,
//                };
//     }
//
//
//     protected Task Load() => Task.Run( LoadAsync );
//
//
//     protected virtual async Task LoadAsync()
//     {
//         IniConfig? cfg = await IniConfig.ReadFromFileAsync( Path )
//                                         .ConfigureAwait( false );
//
//         if ( cfg is null ) { return; }
//
//         foreach ( KeyValuePair<string, IniConfig.Section> pair in cfg ) { Add( pair ); }
//     }
//
//
//     protected Task Save() => Task.Run( SaveAsync );
//     protected virtual async Task SaveAsync() => await IniConfig.WriteToFile( Path )
//                                                                .ConfigureAwait( false );
//     public virtual void Dispose( bool disposing )
//     {
//         if ( !disposing ) { return; }
//
//         Task.Run( async () => await DisposeAsync() )
//             .Wait();
//     }
//     public virtual async ValueTask DisposeAsync()
//     {
//         await SaveAsync()
//            .ConfigureAwait( false );
//
//         _file?.Dispose();
//         _file = null;
//     }
//     public void Dispose()
//     {
//         Dispose( true );
//         GC.SuppressFinalize( this );
//     }
// }
