using System.Security.Claims;



namespace Jakar.Database;


/// <summary>
/// <para><see cref="IUserID"/></para>
/// <para><see cref="IUserData"/></para>
/// <para><see cref="IUserControl{TRecord}"/></para>
/// <para><see cref="IUserSubscription{TID}"/></para>
/// <para><see cref="IRefreshToken"/></para>
/// </summary>
/// <typeparam name="TRecord"></typeparam>
/// <typeparam name="TID"></typeparam>
[SuppressMessage("ReSharper", "UnusedType.Global")]

// ReSharper disable once PossibleInterfaceMemberAmbiguity
public interface IUserRecord<TRecord, TID> : IComparable<TRecord>, IEquatable<TRecord>, JsonModels.IJsonStringModel, IRefreshToken, IUserControl<TRecord>, IUserID, IUserData, IUserSecurity, IUserSubscription<TID>
    where TRecord : BaseTableRecord<TRecord, TID>, IUserRecord<TRecord, TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>
{
    public string?        UserName     { get; }
    public string?        PasswordHash { get; }
    public DateTimeOffset DateCreated  { get; }
    public TID?           CreatedBy    { get; }


    /// <summary> A unique User ID for which user to contact </summary>
    public TID? EscalateTo { get; }


    public List<Claim> GetUserClaims();


    /// <summary>
    /// <para><see href = "https://stackoverflow.com/a/63733365/9530917" /></para>
    /// <para><see href = "https://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file" /></para>
    /// <see cref="PasswordHasher{TRecord}"/> 
    /// </summary>
    public void UpdatePassword( string password );
    /// <summary>
    /// <para><see href = "https://stackoverflow.com/a/63733365/9530917" /></para>
    /// <para><see href = "https://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file" /></para>
    /// <see cref="PasswordHasher{TRecord}"/> 
    /// </summary>
    public PasswordVerificationResult VerifyPassword( string password );


    public Task<TRecord?> GetBoss( DbConnection           connection, DbTransaction? transaction, DbTable<TRecord, TID> table, CancellationToken token );
    public Task<TRecord?> GetUserWhoCreated( DbConnection connection, DbTransaction? transaction, DbTable<TRecord, TID> db,    CancellationToken token );
}
