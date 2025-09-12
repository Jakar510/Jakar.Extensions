using AsyncAwaitBestPractices;



namespace Jakar.Extensions;


[Serializable]
public class LanguageApi : ObservableClass
{
    protected readonly WeakEventManager<Language> _weakEventManager = new();
    private          Language.Collection        _languages        = new(Language.Supported);
    private          CultureInfo                _currentCulture   = CultureInfo.CurrentCulture;
    private          CultureInfo                _currentUiCulture = CultureInfo.CurrentUICulture;
    private          CultureInfo?               _defaultThreadCurrentCulture;
    private          CultureInfo?               _defaultThreadCurrentUiCulture;
    private          Language?                  _selectedLanguage;


    public virtual CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if ( !SetProperty( ref _currentCulture, value ) ) { return; }

            CultureInfo.CurrentCulture = value;
        }
    }
    public virtual CultureInfo CurrentUICulture
    {
        get => _currentUiCulture;
        set
        {
            if ( !SetProperty( ref _currentUiCulture, value ) ) { return; }

            CultureInfo.CurrentUICulture = value;
        }
    }
    public virtual CultureInfo? DefaultThreadCurrentCulture
    {
        get => _defaultThreadCurrentCulture;
        set
        {
            if ( !SetProperty( ref _defaultThreadCurrentCulture, value ) ) { return; }

            CultureInfo.DefaultThreadCurrentCulture = value;
        }
    }
    public virtual CultureInfo? DefaultThreadCurrentUiCulture
    {
        get => _defaultThreadCurrentUiCulture;
        set
        {
            if ( !SetProperty( ref _defaultThreadCurrentUiCulture, value ) ) { return; }

            CultureInfo.DefaultThreadCurrentUICulture = value;
        }
    }
    public virtual Language.Collection Languages
    {
        get => _languages;
        set
        {
            if ( !SetProperty( ref _languages, value ) ) { return; }

            if ( value.Contains( SelectedLanguage ) ) { return; }

            value.Add( SelectedLanguage );
        }
    }
    public virtual Language SelectedLanguage
    {
        get => _selectedLanguage ?? throw new NullReferenceException( nameof(_selectedLanguage) ); // ?? throw new NullReferenceException(nameof(_selectedLanguage));
        set
        {
            if ( !SetProperty( ref _selectedLanguage, value ) ) { return; }

            _weakEventManager.RaiseEvent( value, nameof(OnLanguageChanged) );
            CurrentUICulture = value;
        }
    }


    public event Action<Language> OnLanguageChanged { add => _weakEventManager.AddEventHandler( value ); remove => _weakEventManager.RemoveEventHandler( value ); }


    public LanguageApi() : this( CultureInfo.CurrentUICulture ) { }
    public LanguageApi( Language culture )
    {
        SelectedLanguage = culture;
        Languages.TryAdd( culture );
    }
}
