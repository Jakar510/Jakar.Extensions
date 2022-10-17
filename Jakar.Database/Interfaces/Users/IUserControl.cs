namespace Jakar.Database;


public interface IUserControl
{
    public bool            IsActive       { get; }
    public bool            IsDisabled     { get; }
    public bool            IsLocked       { get; }
    public DateTimeOffset? LastActive     { get; }
    public DateTimeOffset? LastBadAttempt { get; }
    public DateTimeOffset? LockDate       { get; }
    public Guid?           SessionID      { get; }
    public int             BadLogins      { get; }


    /// <summary> Disable the user, with current rights. </summary>
    public UserRecord Disable();


    /// <summary> Enable the user, with current rights. </summary>
    public UserRecord Enable();
    public UserRecord Lock();


    public UserRecord MarkBadLogin();
    public UserRecord SetActive();
    public UserRecord Unlock();
}
