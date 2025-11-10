namespace Jakar.Extensions;


[Serializable]
public class LanguageApi : BaseClass
{
    private const  string             SHARED_KEY               = "LanguageApi";
    public const   string             CURRENT_LANGUAGE_VERSION = "CurrentLanguageVersion";
    public const   string             SELECTED_LANGUAGE_NAME   = "SelectedLanguageDisplayName";
    private static LanguageApi?       _api;
    private        CultureInfo        _currentCulture   = CultureInfo.CurrentCulture;
    private        CultureInfo        _currentUiCulture = CultureInfo.CurrentUICulture;
    private        CultureInfo?       _defaultThreadCurrentCulture;
    private        CultureInfo?       _defaultThreadCurrentUiCulture;
    private        Language?          _selectedLanguage;
    private        LanguageCollection _languages = new(Language.Supported);


    public static LanguageApi Current => _api ??= Create();


    public CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if ( SetProperty(ref _currentCulture, value) ) { CultureInfo.CurrentCulture = value; }
        }
    }
    public CultureInfo CurrentUICulture
    {
        get => _currentUiCulture;
        set
        {
            if ( SetProperty(ref _currentUiCulture, value) ) { CultureInfo.CurrentUICulture = value; }
        }
    }
    public CultureInfo? DefaultThreadCurrentCulture
    {
        get => _defaultThreadCurrentCulture;
        set
        {
            if ( SetProperty(ref _defaultThreadCurrentCulture, value) ) { CultureInfo.DefaultThreadCurrentCulture = value; }
        }
    }
    public CultureInfo? DefaultThreadCurrentUiCulture
    {
        get => _defaultThreadCurrentUiCulture;
        set
        {
            if ( SetProperty(ref _defaultThreadCurrentUiCulture, value) ) { CultureInfo.DefaultThreadCurrentUICulture = value; }
        }
    }
    public LanguageCollection Languages
    {
        get => _languages;
        set
        {
            if ( SetProperty(ref _languages, value) ) { value.TryAdd(SelectedLanguage); }
        }
    }
    public Language SelectedLanguage
    {
        get => _selectedLanguage ?? throw new NullReferenceException(nameof(_selectedLanguage));
        set
        {
            if ( SetProperty(ref _selectedLanguage, value) ) { LanguageChanged(value); }
        }
    }


    public event Action<Language>? OnLanguageChanged { add => _eventManager.AddEventHandler(value); remove => _eventManager.RemoveEventHandler(value); }


    public LanguageApi() : this(CultureInfo.CurrentUICulture) { }
    public LanguageApi( Language culture )
    {
        SelectedLanguage = culture;
        Languages.TryAdd(culture);
    }


    private static LanguageApi Create()
    {
        SupportedLanguage language = (SupportedLanguage)SHARED_KEY.GetPreference(CURRENT_LANGUAGE_VERSION, SupportedLanguage.English.AsLong());
        LanguageApi       api      = new(language);
        return api;
    }


    protected virtual void LanguageChanged( Language value )
    {
        CurrentUICulture = value;
        SHARED_KEY.SetPreference(SELECTED_LANGUAGE_NAME,   value.DisplayName);
        SHARED_KEY.SetPreference(CURRENT_LANGUAGE_VERSION, value.Version.AsLong());
        _eventManager.RaiseEvent(this, value, nameof(OnLanguageChanged));
    }
}
