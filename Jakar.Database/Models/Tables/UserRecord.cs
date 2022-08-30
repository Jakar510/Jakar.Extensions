// ToothFairyDispatch :: ToothFairyDispatch.Cloud
// 08/29/2022  9:55 PM

using System.Security.Claims;
using Newtonsoft.Json.Linq;



namespace Jakar.Database;


public abstract record UserRecord<TRecord, TID> : BaseTableRecord<TRecord, TID>, IUserRecord<TRecord, TID> where TRecord : UserRecord<TRecord, TID>
                                                                                                           where TID : IComparable<TID>, IEquatable<TID>
{
    protected static readonly PasswordHasher<TRecord> _hasher = new();


    public Guid              UserID                 { get; init; } = default!;
    public string            UserName               { get; set; }  = string.Empty;
    public string            FirstName              { get; set; }  = string.Empty;
    public string            LastName               { get; set; }  = string.Empty;
    public string?           FullName               { get; set; }
    public string?           Description            { get; set; }
    public Guid?             SessionID              { get; set; }
    public string?           Address                { get; set; }
    public string?           Line1                  { get; set; }
    public string?           Line2                  { get; set; }
    public string?           City                   { get; set; }
    public string?           State                  { get; set; }
    public string?           Country                { get; set; }
    public string?           PostalCode             { get; set; }
    public string?           Website                { get; set; }
    public string?           Email                  { get; set; }
    public string?           PhoneNumber            { get; set; }
    public string?           Ext                    { get; set; }
    public string?           Title                  { get; set; }
    public string?           Department             { get; set; }
    public string?           Company                { get; set; }
    public SupportedLanguage PreferredLanguage      { get; set; }
    public DateTimeOffset?   SubscriptionExpires    { get; set; }
    public TID?              SubscriptionID         { get; set; }
    public DateTimeOffset    DateCreated            { get; set; }
    public TID               CreatedBy              { get; set; } = default!;
    public TID               EscalateTo             { get; set; } = default!;
    public bool              IsActive               { get; set; }
    public bool              IsDisabled             { get; set; }
    public bool              IsLocked               { get; set; }
    public int?              BadLogins              { get; set; }
    public DateTimeOffset?   LastActive             { get; set; }
    public DateTimeOffset?   LastBadAttempt         { get; set; }
    public DateTimeOffset?   LockDate               { get; set; }
    public string?           RefreshToken           { get; set; }
    public DateTimeOffset?   RefreshTokenExpiryTime { get; set; }
    public DateTimeOffset?   TokenExpiration        { get; set; }
    public bool              IsEmailConfirmed       { get; set; }
    public bool              IsPhoneNumberConfirmed { get; set; }
    public bool              IsTwoFactorEnabled     { get; set; }
    public string?           LoginProvider          { get; set; }
    public string?           ProviderKey            { get; set; }
    public string?           ProviderDisplayName    { get; set; }
    public string?           SecurityStamp          { get; set; }
    public string?           ConcurrencyStamp       { get; set; }
    public string?           AdditionalData         { get; set; }
    public string?           PasswordHash           { get; set; }


    IDictionary<string, JToken?>? JsonModels.IJsonModel.AdditionalData
    {
        get => AdditionalData?.FromJson<Dictionary<string, JToken?>>();
        set => AdditionalData = value?.ToJson();
    }


    protected UserRecord() { }
    protected UserRecord( IUserData value, TRecord? caller )
    {
        FirstName         = value.FirstName;
        LastName          = value.LastName;
        FullName          = value.FullName;
        Address           = value.Address;
        Line1             = value.Line1;
        Line2             = value.Line2;
        City              = value.City;
        State             = value.State;
        Country           = value.Country;
        PostalCode        = value.PostalCode;
        Description       = value.Description;
        Website           = value.Website;
        Email             = value.Email;
        PhoneNumber       = value.PhoneNumber;
        Ext               = value.Ext;
        Title             = value.Title;
        Department        = value.Department;
        Company           = value.Company;
        PreferredLanguage = value.PreferredLanguage;
        DateCreated       = DateTimeOffset.Now;

        CreatedBy = caller is not null
                        ? caller.ID
                        : default!;
    }
    protected virtual void VerifyContextManager() { }


    public void UpdatePassword( string password )
    {
        VerifyContextManager();
        PasswordHash = _hasher.HashPassword((TRecord)this, password);
    }
    public PasswordVerificationResult VerifyPassword( string password ) => _hasher.VerifyHashedPassword((TRecord)this, PasswordHash, password);


    public TRecord MarkBadLogin()
    {
        VerifyContextManager();
        LastBadAttempt = DateTimeOffset.Now;
        BadLogins      = 0;
        IsDisabled     = BadLogins > 5;

        return IsDisabled
                   ? Lock()
                   : Unlock();
    }
    public TRecord SetActive()
    {
        VerifyContextManager();
        LastActive = DateTimeOffset.Now;
        return (TRecord)this;
    }
    public TRecord Disable()
    {
        VerifyContextManager();
        IsDisabled = true;
        return Lock();
    }
    public TRecord Lock()
    {
        VerifyContextManager();
        IsDisabled = true;
        LockDate   = DateTimeOffset.Now;
        return (TRecord)this;
    }
    public TRecord Enable()
    {
        VerifyContextManager();
        LockDate = default;
        IsActive = true;
        return Unlock();
    }
    public TRecord Unlock()
    {
        VerifyContextManager();
        BadLogins      = 0;
        IsDisabled     = BadLogins > 5;
        LastBadAttempt = default;
        LockDate       = default;
        return (TRecord)this;
    }


    public void ClearRefreshToken()
    {
        VerifyContextManager();
        RefreshToken           = default;
        RefreshTokenExpiryTime = default;
    }
    public void SetRefreshToken( string token, DateTimeOffset date )
    {
        VerifyContextManager();
        RefreshToken           = token;
        RefreshTokenExpiryTime = date;
    }


    public List<Claim> GetUserClaims() => new()
                                          {
                                              new Claim(nameof(UserID),   UserID.ToString()),
                                              new Claim(nameof(UserName), UserName),
                                          };


    public async Task<TRecord?> GetBoss( DbConnection connection, DbTransaction? transaction, DbTable<TRecord, TID> table, CancellationToken token ) => EscalateTo.Equals(default)
                                                                                                                                                            ? default
                                                                                                                                                            : await table.Get(connection, transaction, EscalateTo, token);
    public async Task<TRecord?> GetUserWhoCreated( DbConnection connection, DbTransaction? transaction, DbTable<TRecord, TID> table, CancellationToken token ) => CreatedBy.Equals(default)
                                                                                                                                                                      ? default
                                                                                                                                                                      : await table.Get(connection, transaction, CreatedBy, token);


    public void Update( IUserData value )
    {
        VerifyContextManager();
        FirstName         = value.FirstName;
        LastName          = value.LastName;
        FullName          = value.FullName;
        Address           = value.Address;
        Line1             = value.Line1;
        Line2             = value.Line2;
        City              = value.City;
        State             = value.State;
        Country           = value.Country;
        PostalCode        = value.PostalCode;
        Description       = value.Description;
        Website           = value.Website;
        Email             = value.Email;
        PhoneNumber       = value.PhoneNumber;
        Ext               = value.Ext;
        Title             = value.Title;
        Department        = value.Department;
        Company           = value.Company;
        PreferredLanguage = value.PreferredLanguage;
    }


    public override int CompareTo( TRecord? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( ReferenceEquals(null, other) ) { return 1; }

        int userNameComparison = string.Compare(UserName, other.UserName, StringComparison.Ordinal);
        if ( userNameComparison != 0 ) { return userNameComparison; }

        int firstNameComparison = string.Compare(FirstName, other.FirstName, StringComparison.Ordinal);
        if ( firstNameComparison != 0 ) { return firstNameComparison; }

        int lastNameComparison = string.Compare(LastName, other.LastName, StringComparison.Ordinal);
        if ( lastNameComparison != 0 ) { return lastNameComparison; }

        int fullNameComparison = string.Compare(FullName, other.FullName, StringComparison.Ordinal);
        if ( fullNameComparison != 0 ) { return fullNameComparison; }

        int descriptionComparison = string.Compare(Description, other.Description, StringComparison.Ordinal);
        if ( descriptionComparison != 0 ) { return descriptionComparison; }

        int sessionIDComparison = Nullable.Compare(SessionID, other.SessionID);
        if ( sessionIDComparison != 0 ) { return sessionIDComparison; }

        int addressComparison = string.Compare(Address, other.Address, StringComparison.Ordinal);
        if ( addressComparison != 0 ) { return addressComparison; }

        int line1Comparison = string.Compare(Line1, other.Line1, StringComparison.Ordinal);
        if ( line1Comparison != 0 ) { return line1Comparison; }

        int line2Comparison = string.Compare(Line2, other.Line2, StringComparison.Ordinal);
        if ( line2Comparison != 0 ) { return line2Comparison; }

        int cityComparison = string.Compare(City, other.City, StringComparison.Ordinal);
        if ( cityComparison != 0 ) { return cityComparison; }

        int stateComparison = string.Compare(State, other.State, StringComparison.Ordinal);
        if ( stateComparison != 0 ) { return stateComparison; }

        int countryComparison = string.Compare(Country, other.Country, StringComparison.Ordinal);
        if ( countryComparison != 0 ) { return countryComparison; }

        int postalCodeComparison = string.Compare(PostalCode, other.PostalCode, StringComparison.Ordinal);
        if ( postalCodeComparison != 0 ) { return postalCodeComparison; }

        int websiteComparison = string.Compare(Website, other.Website, StringComparison.Ordinal);
        if ( websiteComparison != 0 ) { return websiteComparison; }

        int emailComparison = string.Compare(Email, other.Email, StringComparison.Ordinal);
        if ( emailComparison != 0 ) { return emailComparison; }

        int phoneNumberComparison = string.Compare(PhoneNumber, other.PhoneNumber, StringComparison.Ordinal);
        if ( phoneNumberComparison != 0 ) { return phoneNumberComparison; }

        int extComparison = string.Compare(Ext, other.Ext, StringComparison.Ordinal);
        if ( extComparison != 0 ) { return extComparison; }

        int titleComparison = string.Compare(Title, other.Title, StringComparison.Ordinal);
        if ( titleComparison != 0 ) { return titleComparison; }

        int departmentComparison = string.Compare(Department, other.Department, StringComparison.Ordinal);
        if ( departmentComparison != 0 ) { return departmentComparison; }

        return string.Compare(Company, other.Company, StringComparison.Ordinal);
    }
    public override bool Equals( TRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return base.Equals(other) && UserName == other.UserName && FirstName == other.FirstName && LastName == other.LastName && FullName == other.FullName && Description == other.Description && Address == other.Address && Line1 == other.Line1 &&
               Line2 == other.Line2 && City == other.City && State == other.State && Country == other.Country && PostalCode == other.PostalCode && Website == other.Website && Email == other.Email && PhoneNumber == other.PhoneNumber &&
               Ext == other.Ext && Title == other.Title && Department == other.Department && Company == other.Company;
    }


    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(UserName);
        hashCode.Add(FirstName);
        hashCode.Add(LastName);
        hashCode.Add(FullName);
        hashCode.Add(Description);
        hashCode.Add(Address);
        hashCode.Add(Line1);
        hashCode.Add(Line2);
        hashCode.Add(City);
        hashCode.Add(State);
        hashCode.Add(Country);
        hashCode.Add(PostalCode);
        hashCode.Add(Website);
        hashCode.Add(Email);
        hashCode.Add(PhoneNumber);
        hashCode.Add(Ext);
        hashCode.Add(Title);
        hashCode.Add(Department);
        hashCode.Add(Company);
        return hashCode.ToHashCode();
    }
}
