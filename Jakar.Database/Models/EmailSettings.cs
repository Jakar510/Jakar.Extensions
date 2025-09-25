// ToothFairyDispatch :: ToothFairyDispatch.Cloud
// 10/10/2022  1:47 PM

using MailKit.Security;



namespace Jakar.Database;


[Serializable]
public sealed class EmailSettings : BaseClass<EmailSettings>, ILoginRequest, IBaseClass<EmailSettings>
{
    public static JsonSerializerContext         JsonContext   => JakarDatabaseContext.Default;
    public static JsonTypeInfo<EmailSettings>   JsonTypeInfo  => JakarDatabaseContext.Default.EmailSettings;
    public static JsonTypeInfo<EmailSettings[]> JsonArrayInfo => JakarDatabaseContext.Default.EmailSettingsArray;
    public        SecureSocketOptions           Options       { get; init; } = SecureSocketOptions.Auto;
    public        string                        Password      { get; init; } = string.Empty;
    public        int                           Port          { get; init; }
    public        string                        Site          { get; init; } = string.Empty;
    public        string                        UserName      { get; init; } = string.Empty;
    public        bool                          IsValid       => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Site) && Port.IsValidPort();
    public EmailSettings() { }
    public static EmailSettings     Create( IConfiguration configuration )    => configuration.GetSection(nameof(EmailSettings)).Get<EmailSettings>() ?? throw new InvalidOperationException($"Section '{nameof(EmailSettings)}' is invalid");
    public        MailboxAddress    Address()                                 => MailboxAddress.Parse(UserName);
    public        NetworkCredential GetCredential( Uri uri, string authType ) => new(UserName, Password, Site);


    public override bool Equals( EmailSettings? other ) => ReferenceEquals(this, other) || other is not null && string.Equals(UserName, other.UserName, StringComparison.InvariantCulture) && string.Equals(Password, other.Password, StringComparison.InvariantCulture) && string.Equals(Site, other.Site, StringComparison.InvariantCulture) && Port == other.Port;
    public override int CompareTo( EmailSettings? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( other is null ) { return 1; }

        int siteComparison = string.Compare(Site, other.Site, StringComparison.InvariantCultureIgnoreCase);
        if ( siteComparison != 0 ) { return siteComparison; }

        int userNameComparison = string.Compare(UserName, other.UserName, StringComparison.InvariantCultureIgnoreCase);
        if ( userNameComparison != 0 ) { return userNameComparison; }

        int portComparison = Port.CompareTo(other.Port);
        if ( portComparison != 0 ) { return portComparison; }

        int optionsComparison = Options.CompareTo(other.Options);
        if ( optionsComparison != 0 ) { return optionsComparison; }

        return string.Compare(Password, other.Password, StringComparison.InvariantCultureIgnoreCase);
    }
    public override int  GetHashCode()           => HashCode.Combine(UserName, Password, Site, Port, Options);
    public override bool Equals( object? other ) => base.Equals(other);


    public static bool operator ==( EmailSettings? left, EmailSettings? right ) => EqualityComparer<EmailSettings>.Default.Equals(left, right);
    public static bool operator !=( EmailSettings? left, EmailSettings? right ) => !EqualityComparer<EmailSettings>.Default.Equals(left, right);
    public static bool operator >( EmailSettings   left, EmailSettings  right ) => Comparer<EmailSettings>.Default.Compare(left, right) > 0;
    public static bool operator >=( EmailSettings  left, EmailSettings  right ) => Comparer<EmailSettings>.Default.Compare(left, right) >= 0;
    public static bool operator <( EmailSettings   left, EmailSettings  right ) => Comparer<EmailSettings>.Default.Compare(left, right) < 0;
    public static bool operator <=( EmailSettings  left, EmailSettings  right ) => Comparer<EmailSettings>.Default.Compare(left, right) <= 0;
}
