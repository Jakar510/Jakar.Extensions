namespace Jakar.Extensions.Models.Users;


public interface IUserModelDetails<TAddress> : IUserModel<TAddress> where TAddress : BaseUserAddress<TAddress>
{
    public ObservableCollection<Email>?       Emails       { get; }
    public ObservableCollection<TAddress>?    Addresses    { get; }
    public ObservableCollection<PhoneNumber>? PhoneNumbers { get; }
}



[Serializable]
public abstract class BaseUserModelDetails<T, TAddress> : BaseUserModel<T, TAddress>, IUserModelDetails<TAddress> where T : BaseUserModelDetails<T, TAddress>
                                                                                                                  where TAddress : BaseUserAddress<TAddress>
{
    private ObservableCollection<TAddress>?    _addresses;
    private ObservableCollection<PhoneNumber>? _phoneNumbers;
    private ObservableCollection<Email>?       _emails;

    public ObservableCollection<TAddress>? Addresses
    {
        get => _addresses;
        set => SetProperty(ref _addresses, value);
    }


    public ObservableCollection<PhoneNumber>? PhoneNumbers
    {
        get => _phoneNumbers;
        set => SetProperty(ref _phoneNumbers, value);
    }


    public ObservableCollection<Email>? Emails
    {
        get => _emails;
        set => SetProperty(ref _emails, value);
    }


    protected BaseUserModelDetails() { }
    protected BaseUserModelDetails( IUserModel<TAddress>        model, long id ) : base(model, id) { }
    protected BaseUserModelDetails( IUserModelDetails<TAddress> model, long id ) : this(model, id, model.Addresses, model.PhoneNumbers, model.Emails) { }

    protected BaseUserModelDetails( IUserModel<TAddress> model, long id, IEnumerable<TAddress>? addresses, IEnumerable<PhoneNumber>? numbers, IEnumerable<Email>? emails ) : base(model, id)
    {
        Addresses = addresses is not null
                        ? new ObservableCollection<TAddress>(addresses)
                        : default;

        Emails = emails is not null
                     ? new ObservableCollection<Email>(emails)
                     : default;

        PhoneNumbers = numbers is not null
                           ? new ObservableCollection<PhoneNumber>(numbers)
                           : default;
    }


    public void Add( TAddress address )
    {
        Addresses ??= new ObservableCollection<TAddress>();

        if ( address.IsPrimary )
        {
            Addresses.ForEach(a => a.IsPrimary = false);
            AddressDetail = address;
        }

        if ( Addresses.Contains(address) ) { return; }

        Addresses.Add(address);
    }

    public void Add( Email email )
    {
        Emails ??= new ObservableCollection<Email>();

        if ( email.IsPrimary )
        {
            Emails.ForEach(a => a.IsPrimary = false);
            Email = email.ToString();
        }

        if ( Emails.Contains(email) ) { return; }

        Emails.Add(email);
    }

    public void Add( PhoneNumber phone )
    {
        PhoneNumbers ??= new ObservableCollection<PhoneNumber>();

        if ( phone.IsPrimary )
        {
            if ( phone.IsFax )
            {
                PhoneNumbers.Where(p => p.IsFax).ForEach(a => a.IsPrimary = false);
                Fax = phone.ToString();
            }
            else
            {
                PhoneNumbers.Where(p => !p.IsFax).ForEach(a => a.IsPrimary = false);
                PhoneNumber = phone.ToString();
            }
        }

        if ( PhoneNumbers.Contains(phone) ) { return; }

        PhoneNumbers.Add(phone);
    }


    public override void SetValue( TAddress address )
    {
        address.IsPrimary = true;
        Add(address);
    }

    public override void SetValue( PhoneNumber phone )
    {
        phone.IsPrimary = true;
        Add(phone);
    }

    public override void SetValue( Email email )
    {
        email.IsPrimary = true;
        Add(email);
    }
}
