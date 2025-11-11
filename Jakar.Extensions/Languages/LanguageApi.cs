namespace Jakar.Extensions;


[Serializable]
public class LanguageApi : BaseClass
{
    protected const  string             SHARED_KEY               = "LanguageApi";
    public const     string             CURRENT_LANGUAGE_VERSION = "CurrentLanguageVersion";
    public const     string             SELECTED_LANGUAGE_NAME   = "SelectedLanguageDisplayName";
    protected static LanguageApi?       _api;
    protected        CultureInfo        _currentCulture   = CultureInfo.CurrentCulture;
    protected        CultureInfo        _currentUiCulture = CultureInfo.CurrentUICulture;
    protected        CultureInfo?       _defaultThreadCurrentCulture;
    protected        CultureInfo?       _defaultThreadCurrentUiCulture;
    protected        Language?          _selectedLanguage;
    protected        LanguageCollection _languages = new(Language.Supported);


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


    public event EventHandler<Language>? OnLanguageChanged;


    public LanguageApi() : this(CultureInfo.CurrentUICulture) { }
    public LanguageApi( Language culture )
    {
        SelectedLanguage = culture;
        Languages.TryAdd(culture);
    }


    protected static LanguageApi Create()
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
        OnLanguageChanged?.Invoke(this, value);
    }
}
