using AsyncAwaitBestPractices;



namespace Jakar.Extensions;


[Serializable]
public sealed class LanguageApi : ObservableClass
{
    private readonly WeakEventManager<Language> _weakEventManager = new();
    private          Language.Collection        _languages        = new(Language.Supported);
    private          CultureInfo                _currentCulture   = CultureInfo.CurrentCulture;
    private          CultureInfo                _currentUiCulture = CultureInfo.CurrentUICulture;
    private          CultureInfo?               _defaultThreadCurrentCulture;
    private          CultureInfo?               _defaultThreadCurrentUiCulture;
    private          Language?                  _selectedLanguage;


    public CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if ( SetProperty( ref _currentCulture, value ) is false ) { return; }

            CultureInfo.CurrentCulture = value;
        }
    }
    public CultureInfo CurrentUICulture
    {
        get => _currentUiCulture;
        set
        {
            if ( SetProperty( ref _currentUiCulture, value ) is false ) { return; }

            CultureInfo.CurrentUICulture = value;
        }
    }
    public CultureInfo? DefaultThreadCurrentCulture
    {
        get => _defaultThreadCurrentCulture;
        set
        {
            if ( SetProperty( ref _defaultThreadCurrentCulture, value ) is false ) { return; }

            CultureInfo.DefaultThreadCurrentCulture = value;
        }
    }
    public CultureInfo? DefaultThreadCurrentUiCulture
    {
        get => _defaultThreadCurrentUiCulture;
        set
        {
            if ( SetProperty( ref _defaultThreadCurrentUiCulture, value ) is false ) { return; }

            CultureInfo.DefaultThreadCurrentUICulture = value;
        }
    }
    public Language.Collection Languages
    {
        get => _languages;
        set
        {
            if ( SetProperty( ref _languages, value ) is false ) { return; }

            if ( value.Contains( SelectedLanguage ) ) { return; }

            value.Add( SelectedLanguage );
        }
    }
    public Language SelectedLanguage
    {
        get => _selectedLanguage ?? throw new NullReferenceException( nameof(_selectedLanguage) ); // ?? throw new NullReferenceException(nameof(_selectedLanguage));
        set
        {
            if ( SetProperty( ref _selectedLanguage, value ) is false ) { return; }

            _weakEventManager.RaiseEvent( value, nameof(OnLanguageChanged) );
            CurrentUICulture = value;
        }
    }


    public event Action<Language> OnLanguageChanged { add => _weakEventManager.AddEventHandler( value ); remove => _weakEventManager.RemoveEventHandler( value ); }


    public LanguageApi() : this( CultureInfo.CurrentUICulture ) { }
    public LanguageApi( Language culture )
    {
        SelectedLanguage = culture;
        if ( Languages.Contains( culture ) ) { return; }

        Languages.Add( culture );
    }
}
