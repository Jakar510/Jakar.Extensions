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

         

    Guid IUserID. UserID => UserID;


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
