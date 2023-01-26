#nullable enable
namespace Jakar.Extensions;


[Serializable]
public sealed class LanguageApi : ObservableClass
{
    private Language?           _selectedLanguage;
    private Language.Collection _languages = new(Language.Common);


    public Language SelectedLanguage
    {
        get => _selectedLanguage ?? throw new NullReferenceException( nameof(_selectedLanguage) ); // ?? throw new NullReferenceException(nameof(_selectedLanguage));
        set
        {
            if ( !SetProperty( ref _selectedLanguage, value ) ) { return; }

            CultureInfo.DefaultThreadCurrentCulture   = value;
            CultureInfo.DefaultThreadCurrentUICulture = value;
            CultureInfo.CurrentCulture                = value;
            CultureInfo.CurrentUICulture              = value;
            Thread.CurrentThread.CurrentCulture       = value;
            Thread.CurrentThread.CurrentUICulture     = value;
        }
    }
    public Language.Collection Languages
    {
        get => _languages;
        set
        {
            if ( !SetProperty( ref _languages, value ) ) { return; }

            if ( value.Contains( SelectedLanguage ) ) { return; }

            value.Add( SelectedLanguage );
        }
    }


    public LanguageApi() : this( CultureInfo.CurrentUICulture ) { }
    public LanguageApi( Language culture )
    {
        SelectedLanguage = culture;
        if ( Languages.Contains( culture ) ) { return; }

        Languages.Add( culture );
    }
}
