namespace Jakar.Extensions.Models.Users;


public interface IEmail : IDataBaseID, IEquatable<IEmail>
{
    bool   IsPrimary { get; set; }
    bool   IsValid   { get; set; }
    string Address   { get; set; }
}



[Serializable]
public class ValidEmail : ValueOf<string, ValidEmail>
{
    /// <summary>
    /// <see href="https://www.tutorialspoint.com/how-to-validate-an-email-address-in-chash"/>
    /// </summary>
    public static Regex validator = new(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([azA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200));

    public ValidEmail() { }
    public ValidEmail( string value ) => Value = value;

    public static implicit operator ValidEmail( string value ) => new(value);


    protected override bool TryValidate() => validator.IsMatch(Value);
    protected override void Validate()
    {
        if ( !TryValidate() ) { throw new FormatException($"Provided value is in the wrong format: '{Value}'"); }
    }


    public override string ToString() => Value;
}



[Serializable]
[JsonObject]
public class Email : ObservableClass, IEmail
{
    private      bool   _isPrimary;
    private      bool   _isValid;
    private      string _address = string.Empty;
    [Key] public long   ID { get; init; }


    public bool IsPrimary
    {
        get => _isPrimary;
        set => SetProperty(ref _isPrimary, value);
    }


    public bool IsValid
    {
        get => _isValid;
        set => SetProperty(ref _isValid, value);
    }


    public string Address
    {
        get => _address;
        set
        {
            SetProperty(ref _address, value);
            IsValid = ValidEmail.validator.IsMatch(Address);
        }
    }


    public Email() { }

    public Email( string address, bool isPrimary )
    {
        Address   = address;
        IsPrimary = isPrimary;
    }


    public override string ToString() => Address;
    public virtual ValidEmail ToValidEmail() => Address;


    public override bool Equals( object? obj )
    {
        if ( obj is null ) { return false; }

        if ( ReferenceEquals(this, obj) ) { return true; }

        if ( obj.GetType() != GetType() ) { return false; }

        return Equals((Email)obj);
    }


    public bool Equals( IEmail? other )
    {
        if ( ReferenceEquals(null, other) ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return ID == other.ID && Address == other.Address;
    }

    public override int GetHashCode() => HashCode.Combine(ID, IsPrimary, Address);

    public static bool operator ==( Email? left, Email? right ) => Equals(left, right);
    public static bool operator !=( Email? left, Email? right ) => !Equals(left, right);
}
