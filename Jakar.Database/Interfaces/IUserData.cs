namespace Jakar.Database;


public interface IUserName
{
    public string UserName { get; }
}



public interface IUserDataRecord : IUserData, IUserName
{
    public new UserRecord WithUserData(IUserData model);
}
