using AsyncAwaitBestPractices;



namespace Jakar.Extensions;


[Serializable]
public class LanguageApi : ObservableClass
{
    protected readonly WeakEventManager<Language> _weakEventManager  = new();
    private            LanguageCollection         __languages        = new(Language.Supported);
    private            CultureInfo                __currentCulture   = CultureInfo.CurrentCulture;
    private            CultureInfo                __currentUiCulture = CultureInfo.CurrentUICulture;
    private            CultureInfo?               __defaultThreadCurrentCulture;
    private            CultureInfo?               __defaultThreadCurrentUiCulture;
    private            Language?                  __selectedLanguage;


    public virtual CultureInfo CurrentCulture
    {
        get => __currentCulture;
        set
        {
            if ( !SetProperty(ref __currentCulture, value) ) { return; }

            CultureInfo.CurrentCulture = value;
        }
    }
    public virtual CultureInfo CurrentUICulture
    {
        get => __currentUiCulture;
        set
        {
            if ( !SetProperty(ref __currentUiCulture, value) ) { return; }

            CultureInfo.CurrentUICulture = value;
        }
    }
    public virtual CultureInfo? DefaultThreadCurrentCulture
    {
        get => __defaultThreadCurrentCulture;
        set
        {
            if ( !SetProperty(ref __defaultThreadCurrentCulture, value) ) { return; }

            CultureInfo.DefaultThreadCurrentCulture = value;
        }
    }
    public virtual CultureInfo? DefaultThreadCurrentUiCulture
    {
        get => __defaultThreadCurrentUiCulture;
        set
        {
            if ( !SetProperty(ref __defaultThreadCurrentUiCulture, value) ) { return; }

            CultureInfo.DefaultThreadCurrentUICulture = value;
        }
    }
    public virtual LanguageCollection Languages
    {
        get => __languages;
        set
        {
            if ( !SetProperty(ref __languages, value) ) { return; }

            if ( value.Contains(SelectedLanguage) ) { return; }

            value.Add(SelectedLanguage);
        }
    }
    public virtual Language SelectedLanguage
    {
        get => __selectedLanguage ?? throw new NullReferenceException(nameof(__selectedLanguage)); // ?? throw new NullReferenceException(nameof(_selectedLanguage));
        set
        {
            if ( !SetProperty(ref __selectedLanguage, value) ) { return; }

            _weakEventManager.RaiseEvent(value, nameof(OnLanguageChanged));
            CurrentUICulture = value;
        }
    }


    public event Action<Language> OnLanguageChanged { add => _weakEventManager.AddEventHandler(value); remove => _weakEventManager.RemoveEventHandler(value); }


    public LanguageApi() : this(CultureInfo.CurrentUICulture) { }
    public LanguageApi( Language culture )
    {
        SelectedLanguage = culture;
        Languages.TryAdd(culture);
    }
}
