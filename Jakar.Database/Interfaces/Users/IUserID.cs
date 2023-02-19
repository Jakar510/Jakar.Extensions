namespace Jakar.Database;


public interface IUserID : IUniqueID<string>
{
    public Guid UserID { get; init; }
}
