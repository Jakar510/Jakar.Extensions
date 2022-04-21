namespace Jakar.Extensions.Languages;


[Serializable]
public class LanguageApi : ObservableClass
{
    private CultureInfo?      _currentCultureInfo;
    private SupportedLanguage _currentLangVersion;
    private Language          _selectedLanguage;
    private string            _selectedLanguageName = string.Empty;


    public ObservableCollection<Language> Languages { get; } = Language.Collection.Default();


    public virtual SupportedLanguage CurrentLangVersion
    {
        get => _currentLangVersion;
        set => SetProperty(ref _currentLangVersion, value);
    }


    public virtual string SelectedLanguageName
    {
        get => _selectedLanguageName;
        set => SetProperty(ref _selectedLanguageName, value);
    }


    public Language SelectedLanguage
    {
        get => _selectedLanguage; // ?? throw new NullReferenceException(nameof(_selectedLanguage));
        set
        {
            _selectedLanguage    = value;
            CultureInfo          = value.ToCultureInfo();
            CurrentLangVersion   = value.Version;
            SelectedLanguageName = value.DisplayName;
        }
    }


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


    public LanguageApi() : this(CultureInfo.CurrentUICulture) { }
    public LanguageApi( in CultureInfo culture )
    {
        string id = culture.TwoLetterISOLanguageName;

        // ReSharper disable once VirtualMemberCallInConstructor
        CultureInfo = new CultureInfo(id);

        try { SelectedLanguage = Languages.First(language => language.ShortName == id); }
        catch ( Exception ) { SelectedLanguage = Languages.First(language => language.ShortName == "en"); }
    }
}
