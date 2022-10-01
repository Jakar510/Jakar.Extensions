namespace Jakar.Database;


public interface IUserDataRecord : IUserData 
{
    public new UserRecord Update( IUserData model );
}



public interface IUserData : JsonModels.IJsonModel
{
    public string            FirstName         { get; set; }
    public string            LastName          { get; set; }
    public string?           FullName          { get; set; }
    public string?           Address           { get; set; }
    public string?           Line1             { get; set; }
    public string?           Line2             { get; set; }
    public string?           City              { get; set; }
    public string?           State             { get; set; }
    public string?           Country           { get; set; }
    public string?           PostalCode        { get; set; }
    public string?           Description       { get; set; }
    public string?           Website           { get; set; }
    public string?           Email             { get; set; }
    public string?           PhoneNumber       { get; set; }
    public string?           Ext               { get; set; }
    public string?           Title             { get; set; }
    public string?           Department        { get; set; }
    public string?           Company           { get; set; }
    public SupportedLanguage PreferredLanguage { get; set; }


    public void Update( IUserData model );
}
