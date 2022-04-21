// unset


namespace Jakar.Extensions.Models.Users;


public interface IPhoneNumber : IDataBaseID, IEquatable<IPhoneNumber>
{
    bool   IsPrimary { get; set; }
    bool   IsFax     { get; set; }
    bool   IsValid   { get; set; }
    string Number    { get; set; }
}



/// <summary>
///     You may also use one of
///     <see cref = "Validate.Re.PhoneNumbers" />
///     Regex objects to validate phone numbers, use the default
///     <see cref = "validator" />
///     , or provide your own as the validation
///     <see cref = "Regex" />
/// </summary>
[Serializable]
public class PhoneNumber : ObservableClass, IPhoneNumber
{
    /// <summary>
    ///     <para>
    ///         <see href = "https://www.twilio.com/blog/validating-phone-numbers-effectively-with-c-and-the-net-frameworks" />
    ///     </para>
    ///     <para>
    ///         <see href = "https://countrycode.org/" />
    ///     </para>
    /// </summary>
    public static readonly Regex validator = new(@"^([0-9]{1-3})?[-. ]?\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", RegexOptions.Compiled | RegexOptions.Singleline);


    private readonly Regex?  _validator;
    private          bool    _isFax;
    private          bool    _isPrimary;
    private          bool    _isValid;
    private          string? _number;


    /// <summary>
    ///     Uses the default
    ///     <see cref = "validator" />
    ///     as the validation
    ///     <see cref = "Regex" />
    ///     <para> Subclass this class to override the default validator. </para>
    /// </summary>
    public PhoneNumber() : this(validator) { }
    /// <summary>
    ///     You may also use one of
    ///     <see cref = "Validate.Re.PhoneNumbers" />
    ///     Regex objects, use the default
    ///     <see cref = "validator" />
    ///     , or provide your own as the
    ///     <paramref name = "validator" />
    ///     <see cref = "Regex" />
    /// </summary>
    public PhoneNumber( Regex validator ) => _validator = validator;
    /// <summary>
    ///     Uses the default
    ///     <see cref = "validator" />
    ///     as the validation
    ///     <see cref = "Regex" />
    /// </summary>
    public PhoneNumber( string number, bool isPrimary, bool isFax ) : this()
    {
        Number    = number;
        IsPrimary = isPrimary;
        IsFax     = isFax;
    }
    /// <summary>
    ///     You may also use one of
    ///     <see cref = "Validate.Re.PhoneNumbers" />
    ///     Regex objects, use the default
    ///     <see cref = "validator" />
    ///     , or provide your own as the
    ///     <paramref name = "validator" />
    ///     <see cref = "Regex" />
    /// </summary>
    public PhoneNumber( Regex validator, string number, bool isPrimary, bool isFax ) : this(validator)
    {
        Number    = number;
        IsPrimary = isPrimary;
        IsFax     = isFax;
    }


    public override string ToString() => Number;

    public override bool Equals( object? obj )
    {
        if ( obj is null ) { return false; }

        if ( ReferenceEquals(this, obj) ) { return true; }

        if ( obj.GetType() != GetType() ) { return false; }

        return Equals((PhoneNumber)obj);
    }

    public override int GetHashCode() => HashCode.Combine(ID, IsPrimary, IsFax, Number);

    public static bool operator ==( PhoneNumber? left, PhoneNumber? right ) => Equals(left, right);
    public static bool operator !=( PhoneNumber? left, PhoneNumber? right ) => !Equals(left, right);


    [Key] public long ID { get; init; }

    public bool IsPrimary
    {
        get => _isPrimary;
        set => SetProperty(ref _isPrimary, value);
    }


    public bool IsFax
    {
        get => _isFax;
        set => SetProperty(ref _isFax, value);
    }


    public bool IsValid
    {
        get => _isValid;
        set => SetProperty(ref _isValid, value);
    }


    public string Number
    {
        get => _number ?? string.Empty;
        set
        {
            SetProperty(ref _number, value);
            IsValid = _validator?.IsMatch(value) ?? !string.IsNullOrWhiteSpace(value);
        }
    }


    public bool Equals( IPhoneNumber? other )
    {
        if ( ReferenceEquals(null, other) ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return Number == other.Number;
    }
}
