namespace Jakar.Database;


/// <summary>
///     <para>
///         <see cref="IUserID"/>
///     </para>
///     <para>
///         <see cref="IUserData"/>
///     </para>
///     <para>
///         <see cref="IUserControl"/>
///     </para>
///     <para>
///         <see cref="IUserSubscription"/>
///     </para>
///     <para>
///         <see cref="IRefreshToken"/>
///     </para>
/// </summary>
/// <typeparam name="TRecord"> </typeparam>
[SuppressMessage( "ReSharper", "UnusedType.Global" )]

// ReSharper disable once PossibleInterfaceMemberAmbiguity
public interface IUserRecord<TRecord> : IComparable<TRecord>, IEquatable<TRecord>, JsonModels.IJsonStringModel, IRefreshToken, IUserControl, IUserID, IUserDataRecord, IUserSecurity, IUserSubscription
    where TRecord : TableRecord<TRecord>, IUserRecord<TRecord>
{
    public DateTimeOffset DateCreated { get; }
    public long?          CreatedBy   { get; }


    /// <summary> A unique User ID for which user to contact </summary>
    public long? EscalateTo { get; }

    public string  PasswordHash { get; }
    public string? UserName     { get; }


    public List<Claim> GetUserClaims();
    /// <summary>
    ///     <para>
    ///         <see href="https://stackoverflow.com/a/63733365/9530917"/>
    ///     </para>
    ///     <para>
    ///         <see href="https://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file"/>
    ///     </para>
    ///     <see cref="PasswordHasher{TRecord}"/>
    /// </summary>
    public PasswordVerificationResult VerifyPassword( string password );


    /// <summary>
    ///     <para>
    ///         <see href="https://stackoverflow.com/a/63733365/9530917"/>
    ///     </para>
    ///     <para>
    ///         <see href="https://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file"/>
    ///     </para>
    ///     <see cref="PasswordHasher{TRecord}"/>
    /// </summary>
    public UserRecord UpdatePassword( string password );


    public ValueTask<UserRecord?> GetBoss( DbConnection           connection, DbTransaction? transaction, Database db, CancellationToken token );
    public ValueTask<UserRecord?> GetUserWhoCreated( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token );
}
