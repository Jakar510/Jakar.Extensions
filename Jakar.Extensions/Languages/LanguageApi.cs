using AsyncAwaitBestPractices;



namespace Jakar.Extensions;


[Serializable]
public class LanguageApi : BaseClass
{
    protected CultureInfo        _currentCulture   = CultureInfo.CurrentCulture;
    protected CultureInfo        _currentUiCulture = CultureInfo.CurrentUICulture;
    protected CultureInfo?       _defaultThreadCurrentCulture;
    protected CultureInfo?       _defaultThreadCurrentUiCulture;
    protected Language?          _selectedLanguage;
    protected LanguageCollection _languages = new(Language.Supported);


    public virtual CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if ( !SetProperty(ref _currentCulture, value) ) { return; }

            CultureInfo.CurrentCulture = value;
        }
    }
    public virtual CultureInfo CurrentUICulture
    {
        get => _currentUiCulture;
        set
        {
            if ( !SetProperty(ref _currentUiCulture, value) ) { return; }

            CultureInfo.CurrentUICulture = value;
        }
    }
    public virtual CultureInfo? DefaultThreadCurrentCulture
    {
        get => _defaultThreadCurrentCulture;
        set
        {
            if ( !SetProperty(ref _defaultThreadCurrentCulture, value) ) { return; }

            CultureInfo.DefaultThreadCurrentCulture = value;
        }
    }
    public virtual CultureInfo? DefaultThreadCurrentUiCulture
    {
        get => _defaultThreadCurrentUiCulture;
        set
        {
            if ( !SetProperty(ref _defaultThreadCurrentUiCulture, value) ) { return; }

            CultureInfo.DefaultThreadCurrentUICulture = value;
        }
    }
    public virtual LanguageCollection Languages
    {
        get => _languages;
        set
        {
            if ( !SetProperty(ref _languages, value) ) { return; }

            if ( value.Contains(SelectedLanguage) ) { return; }

            value.Add(SelectedLanguage);
        }
    }
    public virtual Language SelectedLanguage
    {
        get => _selectedLanguage ?? throw new NullReferenceException(nameof(_selectedLanguage)); // ?? throw new NullReferenceException(nameof(_selectedLanguage));
        set
        {
            if ( !SetProperty(ref _selectedLanguage, value) ) { return; }

            OnLanguageChanged?.Invoke(value);
            CurrentUICulture = value;
        }
    }


    public event Action<Language>? OnLanguageChanged;


    public LanguageApi() : this(CultureInfo.CurrentUICulture) { }
    public LanguageApi( Language culture )
    {
        SelectedLanguage = culture;
        Languages.TryAdd(culture);
    }
}
