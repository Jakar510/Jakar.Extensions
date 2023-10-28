namespace Jakar.Database;


public interface IUserName
{
    public string UserName { get; }
}



public interface IUserDataRecord : IUserData, IUserName
{
    public UserRecord WithUserData( IUserData model );
}
