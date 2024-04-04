namespace Jakar.Database;


public interface IUserDataRecord : IUserRecord<Guid>
{
    public UserRecord WithUserData( IUserData<Guid> model );
}
