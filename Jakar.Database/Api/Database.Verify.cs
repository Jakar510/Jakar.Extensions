// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:53 PM

namespace Jakar.Database;


public enum SubscriptionStatus : ulong
{
    None,
    Invalid,
    Expired,
    Ok
}



public abstract partial class Database
{
    public virtual ValueTask<DateTimeOffset?> GetSubscriptionExpiration( NpgsqlConnection connection, DbTransaction? transaction, UserRecord record, CancellationToken token = default ) => new(record.SubscriptionExpires);
    public virtual ValueTask<ErrorOrResult<TSelf>> TryGetSubscription<TSelf>( NpgsqlConnection connection, DbTransaction? transaction, UserRecord record, CancellationToken token = default )
        where TSelf : UserSubscription<TSelf>, ITableRecord<TSelf> => default;


    /// <summary> </summary>
    /// <returns> <see langword="true"/> is Subscription is valid; otherwise <see langword="false"/> </returns>
    [SuppressMessage("ReSharper", "UnusedParameter.Global")]
    public virtual ValueTask<ErrorOrResult<SubscriptionStatus>> ValidateSubscription( NpgsqlConnection connection, DbTransaction? transaction, UserRecord record, CancellationToken token = default ) => new(SubscriptionStatus.Ok);


    protected virtual async ValueTask<ErrorOrResult<UserRecord>> VerifyLogin( NpgsqlConnection connection, DbTransaction transaction, ILoginRequest request, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get(connection, transaction, true, UserRecord.GetDynamicParameters(request), token);
        if ( record is null ) { return Error.NotFound(); }

        try
        {
            if ( !UserRecord.VerifyPassword(ref record, request) )
            {
                record = record.MarkBadLogin();
                return Error.Unauthorized(request.UserName);
            }

            if ( !record.IsActive )
            {
                record = record.MarkBadLogin();
                return Error.Disabled();
            }

            if ( record.IsDisabled )
            {
                record = record.MarkBadLogin();
                return Error.Disabled();
            }

            if ( record.IsLocked )
            {
                record = record.MarkBadLogin();
                return Error.Locked();
            }

            ErrorOrResult<SubscriptionStatus> status = await ValidateSubscription(connection, transaction, record, token);

            if ( status.HasErrors )
            {
                record = record.MarkBadLogin();
                return status.Error;
            }

            return record;
        }
        finally
        {
            record = record.SetActive(true);
            await Users.Update(connection, transaction, record, token);
        }
    }


    public virtual async ValueTask<ErrorOrResult<TValue>> Verify<TValue>( NpgsqlConnection connection, DbTransaction transaction, ILoginRequest request, Func<NpgsqlConnection, DbTransaction, UserRecord, ErrorOrResult<TValue>> func, CancellationToken token = default )
    {
        ErrorOrResult<UserRecord> loginResult = await VerifyLogin(connection, transaction, request, token);

        return loginResult.TryGetValue(out UserRecord? record, out Errors? errors)
                   ? func(connection, transaction, record)
                   : errors;
    }

    public virtual async ValueTask<ErrorOrResult<TValue>> Verify<TValue>( NpgsqlConnection connection, DbTransaction transaction, ILoginRequest request, Func<NpgsqlConnection, DbTransaction, UserRecord, CancellationToken, ValueTask<ErrorOrResult<TValue>>> func, CancellationToken token = default )
    {
        ErrorOrResult<UserRecord> loginResult = await VerifyLogin(connection, transaction, request, token);

        return loginResult.TryGetValue(out UserRecord? record, out Errors? errors)
                   ? await func(connection, transaction, record, token)
                   : errors;
    }

    public virtual async ValueTask<ErrorOrResult<TValue>> Verify<TValue>( NpgsqlConnection connection, DbTransaction transaction, ILoginRequest request, Func<NpgsqlConnection, DbTransaction, UserRecord, CancellationToken, Task<ErrorOrResult<TValue>>> func, CancellationToken token = default )
    {
        ErrorOrResult<UserRecord> loginResult = await VerifyLogin(connection, transaction, request, token);

        return loginResult.TryGetValue(out UserRecord? record, out Errors? errors)
                   ? await func(connection, transaction, record, token)
                   : errors;
    }

    public virtual async ValueTask<ErrorOrResult<SessionToken>> Verify( NpgsqlConnection connection, DbTransaction transaction, ILoginRequest request, ClaimType types, CancellationToken token = default )
    {
        ErrorOrResult<UserRecord> loginResult = await VerifyLogin(connection, transaction, request, token);

        return loginResult.TryGetValue(out UserRecord? record, out Errors? errors)
                   ? await GetToken(connection, transaction, record, types, token)
                   : errors;
    }


    public virtual async ValueTask<ErrorOrResult<SessionToken>> Register<TUser>( NpgsqlConnection connection, DbTransaction transaction, ILoginRequest<TUser> request, CancellationToken token = default )
        where TUser : class, IUserData<Guid>
    {
        UserRecord? record = await Users.Get(connection, transaction, true, UserRecord.GetDynamicParameters(request), token);
        if ( record is not null ) { return Error.Conflict($"{nameof(UserRecord.UserName)} is already taken. Chose another {nameof(request.UserName)}"); }

        record = CreateNewUser(request);
        record = await Users.Insert(connection, transaction, record, token);
        return await GetToken(connection, transaction, record, DEFAULT_CLAIM_TYPES, token);
    }
    protected virtual UserRecord CreateNewUser<TUser>( ILoginRequest<TUser> request, UserRecord? caller = null )
        where TUser : class, IUserData<Guid> => UserRecord.Create(request, request.Data.Rights, caller);


    public ValueTask<ErrorOrResult<SessionToken>> Register<TUser>( ILoginRequest<TUser> request, CancellationToken token = default )
        where TUser : class, IUserData<Guid> => this.TryCall(Register, request, token);
    public ValueTask<ErrorOrResult<TValue>> Verify<TValue>( ILoginRequest request, Func<NpgsqlConnection, DbTransaction, UserRecord, ErrorOrResult<TValue>>                               func, CancellationToken token = default ) => this.TryCall(Verify, request, func, token);
    public ValueTask<ErrorOrResult<TValue>> Verify<TValue>( ILoginRequest request, Func<NpgsqlConnection, DbTransaction, UserRecord, CancellationToken, ValueTask<ErrorOrResult<TValue>>> func, CancellationToken token = default ) => this.TryCall(Verify, request, func, token);
    public ValueTask<ErrorOrResult<TValue>> Verify<TValue>( ILoginRequest request, Func<NpgsqlConnection, DbTransaction, UserRecord, CancellationToken, Task<ErrorOrResult<TValue>>>      func, CancellationToken token = default ) => this.TryCall(Verify, request, func, token);
}
