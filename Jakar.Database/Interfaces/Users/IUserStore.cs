// Jakar.Extensions :: Jakar.Database
// 08/26/2022  11:22 AM

namespace Jakar.Database;


public interface IUserStore<TUserRecord> : IUserLoginStore<TUserRecord>,
                                           IUserClaimStore<TUserRecord>,
                                           IUserPasswordStore<TUserRecord>,
                                           IUserSecurityStampStore<TUserRecord>,
                                           IUserTwoFactorStore<TUserRecord>,
                                           IUserEmailStore<TUserRecord>,
                                           IUserLockoutStore<TUserRecord>,
                                           IUserAuthenticatorKeyStore<TUserRecord>,
                                           IUserTwoFactorRecoveryCodeStore<TUserRecord>,
                                           IUserPhoneNumberStore<TUserRecord> where TUserRecord : TableRecord<TUserRecord>, IUserRecord<TUserRecord> { }
