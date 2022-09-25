namespace Jakar.Database;


public interface IUserID : IDataBaseID
{
    public Guid UserID { get; init; }
}
