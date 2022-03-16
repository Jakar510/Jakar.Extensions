namespace Jakar.Extensions.Languages;


public abstract class LanguageApi
{
    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public ObservableCollection<Language> Languages { get; } = Language.Collection.Default();


    public abstract SupportedLanguage CurrentLangVersion   { get; set; }
    public abstract string            SelectedLanguageName { get; set; }


    private Language? _selectedLanguage;

    public Language SelectedLanguage
    {
        get => _selectedLanguage ?? throw new NullReferenceException(nameof(_selectedLanguage));
        set
        {
            _selectedLanguage = value;
            if ( _selectedLanguage is null ) return;

            CultureInfo          = _selectedLanguage.Info;
            CurrentLangVersion   = _selectedLanguage.Version;
            SelectedLanguageName = _selectedLanguage.DisplayName;
        }
    }


    private CultureInfo? _currentCultureInfo;

    public virtual CultureInfo CultureInfo
    {
        get => _currentCultureInfo ?? throw new NullReferenceException(nameof(_currentCultureInfo));
        protected set
        {
            _currentCultureInfo                 = value;
            Thread.CurrentThread.CurrentCulture = value;
            CultureInfo.CurrentUICulture        = value;
        }
    }


    protected LanguageApi()
    {
        string id = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        // ReSharper disable once VirtualMemberCallInConstructor
        CultureInfo = new CultureInfo(id);

        try { SelectedLanguage = Languages.First(language => language.ShortName == id); }
        catch ( Exception ) { SelectedLanguage = Languages.First(language => language.ShortName == "en"); }
    }
}
