#nullable enable
namespace Jakar.Extensions.Models.Users;


public interface IUserData<out TUser> : IUserModel
{
    public TUser?          CreatedBy              { get; }
    public TUser?          EscalateTo             { get; }
    public DateTimeOffset  DateCreated            { get; }
    public DateTimeOffset? LastBadAttempt         { get; }
    public DateTimeOffset? LockDate               { get; }
    public DateTimeOffset? SubscriptionExpires    { get; }
    public DateTimeOffset? LastActive             { get; }
    public int?            BadLogins              { get; }
    public long            LogoID                 { get; }
    public bool            IsLocked               { get; }
    public bool            IsDisabled             { get; }
    public bool            IsPhoneNumberConfirmed { get; }
    public bool            IsEmailConfirmed       { get; }
    public bool            IsTwoFactorEnabled     { get; }
    public Guid            GUID                   { get; }
}



public interface IUserData : IUserData<long?> { }
