using System.Security.Claims;
using Microsoft.AspNetCore.Identity;



namespace Jakar.Database;


[SuppressMessage("ReSharper", "UnusedType.Global")]
public interface IUserRecord<TRecord, TID> : JsonModels.IJsonStringModel, IComparable<TRecord>, IEquatable<TRecord>, IUserData, IUserControl<TRecord>, IRefreshToken, IUserSubscription<TID>
    where TRecord : BaseTableRecord<TRecord, TID>, IUserRecord<TRecord, TID>
    where TID : IComparable<TID>, IEquatable<TID>
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
    public void UpdatePassword(string password);


    /// <summary>
    /// <para><see href = "https://stackoverflow.com/a/63733365/9530917" /></para>
    /// <para><see href = "https://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file" /></para>
    /// <see cref="PasswordHasher{TRecord}"/> 
    /// </summary>
    public PasswordVerificationResult VerifyPassword(string password);


    public Task<TRecord?> GetBoss(DbConnection           connection, DbTransaction? transaction, DbTable<TRecord, TID> table, CancellationToken token);
    public Task<TRecord?> GetUserWhoCreated(DbConnection connection, DbTransaction? transaction, DbTable<TRecord, TID> db,    CancellationToken token);
}