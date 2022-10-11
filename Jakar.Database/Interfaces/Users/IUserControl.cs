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
    public int?            BadLogins      { get; }


    public UserRecord MarkBadLogin();
    public UserRecord SetActive();
    public UserRecord Unlock();
    public UserRecord Lock();


    /// <summary> Disable the user, with current rights. </summary>
    public UserRecord Disable();


    /// <summary> Enable the user, with current rights. </summary>
    public UserRecord Enable();
}
