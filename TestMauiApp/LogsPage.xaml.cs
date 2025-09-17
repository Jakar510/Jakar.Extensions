using CommunityToolkit.Mvvm.Input;
using Serilog.Core;



namespace TestMauiApp;


public sealed partial class LogsPage : ContentPage
{
    private readonly Logger                         __logger;
    public           ObservableCollection<FileLogs> Files          { get; } = new(64);
    public           AsyncRelayCommand              RefreshCommand { get; }


    public LogsPage()
    {
        InitializeComponent();
        BindingContext = this;
        RefreshCommand = new AsyncRelayCommand( RefreshAsync );
        __logger        = App.Logger.CreateLogger<LogsPage>();
    }
    protected override void OnAppearing()    => RefreshCommand.Execute( null );
    protected override void OnDisappearing() => Files.Clear();


    private async Task RefreshAsync( CancellationToken token )
    {
        try
        {
            IsBusy = true;
            Files.Clear();
            foreach ( LocalFile file in App.Logger.Options.Paths.Logs.GetFiles() ) { Files.Add( await FileLogs.Create( file, token ) ); }
        }
        finally { IsBusy = false; }
    }
}



public sealed class FileLogs( string fileName, string[] lines ) : ObservableCollection<string>( lines ), IEquatable<FileLogs>
{
    private static readonly char[] __separator = ['\n', '\r'];
    public                  string Name { get; } = fileName;


    public static async ValueTask<FileLogs> Create( LocalFile file, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        string              content       = await file.ReadAsync().AsString( token );
        return new FileLogs( file.Name, content.Split( __separator, StringSplitOptions.RemoveEmptyEntries ) );
    }


    public bool Equals( FileLogs? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( Name, other.Name, StringComparison.Ordinal );
    }
    public override bool Equals( object? obj ) => ReferenceEquals( this, obj ) || ( obj is FileLogs other && Equals( other ) );
    public override int  GetHashCode()         => Name.GetHashCode();
}
