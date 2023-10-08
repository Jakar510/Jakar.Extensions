namespace Jakar.Database;


public interface IUserID : IUniqueID<Guid>
{
    public Guid UserID { get; }
}
