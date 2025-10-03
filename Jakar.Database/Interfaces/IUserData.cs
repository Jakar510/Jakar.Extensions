namespace Jakar.Database;


public interface IUserDataRecord : IUserRecord<Guid>, IUserData
{
    public UserRecord WithUserData( IUserData<Guid> model );
}
