namespace Jakar.Extensions.Models.Users;


public interface IUserModel<TAddress> : IUserModel where TAddress : IUserAddress
{
    TAddress? AddressDetail { get; set; }
}



[Serializable]
public abstract class BaseUserModel<T, TAddress> : BaseUserModel<T>, IUserModel<TAddress> where T : BaseUserModel<T, TAddress>
                                                                                          where TAddress : BaseUserAddress<TAddress>
{
    private TAddress? _addressDetail;


    protected BaseUserModel() { }
    protected BaseUserModel( IUserModel<TAddress> model ) : base(model) => AddressDetail = model.AddressDetail;


    public virtual void SetValue( TAddress address ) => AddressDetail = address;


    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(AddressDetail);
        return hashCode.ToHashCode();
    }


    public TAddress? AddressDetail
    {
        get => _addressDetail;
        set
        {
            SetProperty(ref _addressDetail, value);
            if ( value is null ) { return; }

            Address = value.ToString();
        }
    }
}
