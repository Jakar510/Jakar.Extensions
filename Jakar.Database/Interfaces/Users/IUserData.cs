namespace Jakar.Database;


public interface IUserDataRecord : IUserData
{
    public new UserRecord Update( IUserData model );
}
