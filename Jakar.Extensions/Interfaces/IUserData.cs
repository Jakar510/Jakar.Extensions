// Jakar.Extensions :: Jakar.Extensions
// 11/15/2022  6:02 PM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "UnusedMemberInSuper.Global" )]
public interface IUserData : JsonModels.IJsonModel
{
    [Required] public string            FirstName         { get; set; }
    [Required] public string            LastName          { get; set; }
    public            string?           Address           { get; set; }
    public            string?           City              { get; set; }
    public            string?           Company           { get; set; }
    public            string?           Country           { get; set; }
    public            string?           Department        { get; set; }
    public            string?           Description       { get; set; }
    public            string?           Email             { get; set; }
    public            string?           Ext               { get; set; }
    public            string?           FullName          { get; set; }
    public            string?           Line1             { get; set; }
    public            string?           Line2             { get; set; }
    public            string?           PhoneNumber       { get; set; }
    [Required] public string?           PostalCode        { get; set; }
    public            string?           State             { get; set; }
    public            string?           Title             { get; set; }
    public            string?           Website           { get; set; }
    [Required] public SupportedLanguage PreferredLanguage { get; set; }


    public void Update( IUserData model );
}
