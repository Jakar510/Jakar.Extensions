namespace Jakar.Database;


public interface IUserControl<out TRecord> where TRecord : IUserControl<TRecord>
{
    public bool            IsActive       { get; }
    public bool            IsDisabled     { get; }
    public bool            IsLocked       { get; }
    public int?            BadLogins      { get; }
    public Guid?           SessionID      { get; }
    public DateTimeOffset? LastActive     { get; }
    public DateTimeOffset? LastBadAttempt { get; }
    public DateTimeOffset? LockDate       { get; }


    public TRecord MarkBadLogin();
    public TRecord SetActive();
    public TRecord Unlock();
    public TRecord Lock();


    /// <summary> Disable the user, with current rights. </summary>
    public TRecord Disable();


    /// <summary> Enable the user, with current rights. </summary>
    public TRecord Enable();
}