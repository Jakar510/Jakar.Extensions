namespace Jakar.Database;


public interface IUserData :  JsonModels.IJsonModel
{
    public string?           FirstName              { get; }
    public string?           LastName               { get; }
    public string?           FullName               { get; }
    public string?           Address                { get; }
    public string?           Line1                  { get; }
    public string?           Line2                  { get; }
    public string?           City                   { get; }
    public string?           State                  { get; }
    public string?           Country                { get; }
    public string?           PostalCode             { get; }
    public string?           Title                  { get; }
    public string?           Department             { get; }
    public string?           Company                { get; }
    public string?           Website                { get; }
    public string?           Email                  { get; }
    public bool              IsEmailConfirmed       { get; }
    public string?           Description            { get; }
    public string?           PhoneNumber            { get; }
    public bool              IsPhoneNumberConfirmed { get; }
    public string?           Ext                    { get; }
    public SupportedLanguage PreferredLanguage      { get; }


    public void Update( IUserData model );
}
