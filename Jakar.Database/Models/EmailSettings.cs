// ToothFairyDispatch :: ToothFairyDispatch.Cloud
// 10/10/2022  1:47 PM

using System.Net;
using MailKit.Security;
using MimeKit;



namespace Jakar.Database;


[Serializable]
public sealed record EmailSettings : ICredentials
{
    [JsonProperty( Required = Required.Always )]    public int                 Port     { get; init; }
    [JsonProperty( Required = Required.Always )]    public SecureSocketOptions Options  { get; init; } = SecureSocketOptions.Auto;
    [JsonProperty( Required = Required.AllowNull )] public string              Password { get; init; } = string.Empty;
    [JsonProperty( Required = Required.AllowNull )] public string              Site     { get; init; } = string.Empty;
    [JsonProperty( Required = Required.Always )]    public string              UserName { get; init; } = string.Empty;


    public EmailSettings() { }
    public static EmailSettings Create( IConfiguration configuration ) => configuration.GetSection( nameof(EmailSettings) )
                                                                                       .Get<EmailSettings>();


    public MailboxAddress Address() => MailboxAddress.Parse( UserName );
    public NetworkCredential? GetCredential( Uri uri, string authType ) => new(UserName, Password, Site);
}
