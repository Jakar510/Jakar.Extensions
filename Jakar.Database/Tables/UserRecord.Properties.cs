// Jakar.Extensions :: Jakar.Database
// 01/30/2023  9:29 PM

namespace Jakar.Database;


public sealed partial record UserRecord
{
    IDictionary<string, JToken?>? JsonModels.IJsonModel.AdditionalData
    {
        get => AdditionalData?.FromJson<Dictionary<string, JToken?>>();
        set => AdditionalData = value?.ToJson();
    }


    [ ProtectedPersonalData, MaxLength( int.MaxValue ) ] public                                             string?               AdditionalData         { get; set; }
    [ ProtectedPersonalData, MaxLength( 4096 ) ]         public                                             string                Address                { get; set; }
    public                                                                                                  int                   BadLogins              { get; set; }
    [ ProtectedPersonalData, MaxLength( 256 ) ]                                                      public string                City                   { get; set; }
    [ ProtectedPersonalData, MaxLength( 256 ) ]                                                      public string                Company                { get; set; }
    [ MaxLength(                        TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] public string                ConcurrencyStamp       { get; set; }
    [ ProtectedPersonalData, MaxLength( 256 ) ]                                                      public string                Country                { get; set; }
    [ ProtectedPersonalData, MaxLength( 256 ) ]                                                      public string                Department             { get; set; }
    [ ProtectedPersonalData, MaxLength( 256 ) ]                                                      public string                Description            { get; set; }
    [ ProtectedPersonalData, MaxLength( 1024 ) ]                                                     public string                Email                  { get; set; }
    [ MaxLength(                        256 ) ]                                                      public RecordID<UserRecord>? EscalateTo             { get; set; }
    [ ProtectedPersonalData, MaxLength( 256 ) ]                                                      public string                Ext                    { get; set; }
    [ ProtectedPersonalData, MaxLength( 256 ) ]                                                      public string                FirstName              { get; set; }
    [ ProtectedPersonalData, MaxLength( 512 ) ]                                                      public string                FullName               { get; set; }
    [ MaxLength(                        256 ) ]                                                      public string                Gender                 { get; set; }
    public                                                                                                  bool                  IsActive               { get; set; }
    public                                                                                                  bool                  IsDisabled             { get; set; }
    public                                                                                                  bool                  IsEmailConfirmed       { get; set; }
    public                                                                                                  bool                  IsLocked               { get; set; }
    public                                                                                                  bool                  IsPhoneNumberConfirmed { get; set; }
    public                                                                                                  bool                  IsTwoFactorEnabled     { get; set; }
    public                                                                                                  DateTimeOffset?       LastBadAttempt         { get; set; }
    public                                                                                                  DateTimeOffset?       LastLogin              { get; set; }
    [ ProtectedPersonalData, MaxLength( 256 ) ] public                                                      string                LastName               { get; set; }
    [ ProtectedPersonalData, MaxLength( 512 ) ] public                                                      string                Line1                  { get; set; }
    [ ProtectedPersonalData, MaxLength( 256 ) ] public                                                      string                Line2                  { get; set; }
    public                                                                                                  DateTimeOffset?       LockDate               { get; set; }
    public                                                                                                  DateTimeOffset?       LockoutEnd             { get; set; }
    [ MaxLength(                        TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] public string                PasswordHash           { get; set; }
    [ ProtectedPersonalData, MaxLength( 256 ) ]                                                      public string                PhoneNumber            { get; set; }
    [ ProtectedPersonalData, MaxLength( 256 ) ]                                                      public string                PostalCode             { get; set; }
    public                                                                                                  SupportedLanguage     PreferredLanguage      { get; set; }
    [ MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] public                        string                RefreshToken           { get; set; }
    public                                                                                                  DateTimeOffset?       RefreshTokenExpiryTime { get; set; }
    [ MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] public                        string                Rights                 { get; set; }
    [ MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] public                        string                SecurityStamp          { get; set; }
    public                                                                                                  Guid?                 SessionID              { get; set; }
    [ ProtectedPersonalData, MaxLength( 256 ) ] public                                                      string                StateOrProvince        { get; set; }
    public                                                                                                  DateTimeOffset?       SubscriptionExpires    { get; set; }
    [ MaxLength(                        256 ) ] public                                                      Guid?                 SubscriptionID         { get; set; }
    [ ProtectedPersonalData, MaxLength( 256 ) ] public                                                      string                Title                  { get; set; }
    public                                                                                                  DateTimeOffset?       TokenExpiration        { get; set; }
    public                                                                                                  Guid                  UserID                 { get; init; }
    Guid IUserID.                                                                                                                 UserID                 => UserID;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string                                                                    UserName               { get; set; }
    [ ProtectedPersonalData, MaxLength( 4096 ) ] public string                                                                    Website                { get; set; }



    public override int CompareTo( UserRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }


        int userNameComparison = string.Compare( UserName, other.UserName, StringComparison.Ordinal );
        if ( userNameComparison != 0 ) { return userNameComparison; }

        int firstNameComparison = string.Compare( FirstName, other.FirstName, StringComparison.Ordinal );
        if ( firstNameComparison != 0 ) { return firstNameComparison; }

        int lastNameComparison = string.Compare( LastName, other.LastName, StringComparison.Ordinal );
        if ( lastNameComparison != 0 ) { return lastNameComparison; }

        int fullNameComparison = string.Compare( FullName, other.FullName, StringComparison.Ordinal );
        if ( fullNameComparison != 0 ) { return fullNameComparison; }

        int descriptionComparison = string.Compare( Description, other.Description, StringComparison.Ordinal );
        if ( descriptionComparison != 0 ) { return descriptionComparison; }

        int sessionIDComparison = Nullable.Compare( SessionID, other.SessionID );
        if ( sessionIDComparison != 0 ) { return sessionIDComparison; }

        int addressComparison = string.Compare( Address, other.Address, StringComparison.Ordinal );
        if ( addressComparison != 0 ) { return addressComparison; }

        int line1Comparison = string.Compare( Line1, other.Line1, StringComparison.Ordinal );
        if ( line1Comparison != 0 ) { return line1Comparison; }

        int line2Comparison = string.Compare( Line2, other.Line2, StringComparison.Ordinal );
        if ( line2Comparison != 0 ) { return line2Comparison; }

        int cityComparison = string.Compare( City, other.City, StringComparison.Ordinal );
        if ( cityComparison != 0 ) { return cityComparison; }

        int stateComparison = string.Compare( StateOrProvince, other.StateOrProvince, StringComparison.Ordinal );
        if ( stateComparison != 0 ) { return stateComparison; }

        int countryComparison = string.Compare( Country, other.Country, StringComparison.Ordinal );
        if ( countryComparison != 0 ) { return countryComparison; }

        int postalCodeComparison = string.Compare( PostalCode, other.PostalCode, StringComparison.Ordinal );
        if ( postalCodeComparison != 0 ) { return postalCodeComparison; }

        int websiteComparison = string.Compare( Website, other.Website, StringComparison.Ordinal );
        if ( websiteComparison != 0 ) { return websiteComparison; }

        int emailComparison = string.Compare( Email, other.Email, StringComparison.Ordinal );
        if ( emailComparison != 0 ) { return emailComparison; }

        int phoneNumberComparison = string.Compare( PhoneNumber, other.PhoneNumber, StringComparison.Ordinal );
        if ( phoneNumberComparison != 0 ) { return phoneNumberComparison; }

        int extComparison = string.Compare( Ext, other.Ext, StringComparison.Ordinal );
        if ( extComparison != 0 ) { return extComparison; }

        int titleComparison = string.Compare( Title, other.Title, StringComparison.Ordinal );
        if ( titleComparison != 0 ) { return titleComparison; }

        int departmentComparison = string.Compare( Department, other.Department, StringComparison.Ordinal );
        if ( departmentComparison != 0 ) { return departmentComparison; }

        return string.Compare( Company, other.Company, StringComparison.Ordinal );
    }
}
