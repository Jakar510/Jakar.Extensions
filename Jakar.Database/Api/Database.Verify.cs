// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:53 PM

namespace Jakar.Database;


public enum SubscriptionStatus
{
    None,
    Invalid,
    Expired,
    Ok,
}



public abstract partial class Database
{
    public virtual ValueTask<DateTimeOffset?> GetSubscriptionExpiration( DbConnection connection, DbTransaction? transaction, UserRecord record, CancellationToken token = default ) => new(record.SubscriptionExpires);
    public virtual ValueTask<TRecord?> TryGetSubscription<TRecord>( DbConnection connection, DbTransaction? transaction, UserRecord record, CancellationToken token = default )
        where TRecord : UserSubscription<TRecord>, IDbReaderMapping<TRecord> => default;


    /// <summary> </summary>
    /// <returns> <see langword="true"/> is Subscription is valid; otherwise <see langword="false"/> </returns>
    [SuppressMessage( "ReSharper", "UnusedParameter.Global" )]
    public virtual ValueTask<ErrorOrResult<SubscriptionStatus>> ValidateSubscription( DbConnection connection, DbTransaction? transaction, UserRecord record, CancellationToken token = default ) => new(SubscriptionStatus.Ok);


    protected virtual async ValueTask<ErrorOrResult<UserRecord>> VerifyLogin( DbConnection connection, DbTransaction transaction, Activity? activity, ILoginRequest request, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, activity, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is null ) { return Error.NotFound(); }

        try
        {
            if ( UserRecord.VerifyPassword( ref record, request ) is false )
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

            ErrorOrResult<SubscriptionStatus> status = await ValidateSubscription( connection, transaction, record, token );

            if ( status.HasErrors )
            {
                record = record.MarkBadLogin();
                return status.Errors;
            }

            return record;
        }
        finally
        {
            record = record.SetActive( true );
            await Users.Update( connection, transaction, activity, record, token );
        }
    }


    public virtual async ValueTask<ErrorOrResult<T>> Verify<T>( DbConnection connection, DbTransaction transaction, Activity? activity, ILoginRequest request, Func<DbConnection, DbTransaction, UserRecord, ErrorOrResult<T>> func, CancellationToken token = default )
    {
        ErrorOrResult<UserRecord> loginResult = await VerifyLogin( connection, transaction, activity, request, token );

        return loginResult.TryGetValue( out UserRecord? record, out Error[]? errors )
                   ? func( connection, transaction, record )
                   : errors;
    }

    public virtual async ValueTask<ErrorOrResult<T>> Verify<T>( DbConnection connection, DbTransaction transaction, Activity? activity, ILoginRequest request, Func<DbConnection, DbTransaction, UserRecord, CancellationToken, ValueTask<ErrorOrResult<T>>> func, CancellationToken token = default )
    {
        ErrorOrResult<UserRecord> loginResult = await VerifyLogin( connection, transaction, activity, request, token );

        return loginResult.TryGetValue( out UserRecord? record, out Error[]? errors )
                   ? await func( connection, transaction, record, token )
                   : errors;
    }

    public virtual async ValueTask<ErrorOrResult<T>> Verify<T>( DbConnection connection, DbTransaction transaction, Activity? activity, ILoginRequest request, Func<DbConnection, DbTransaction, UserRecord, CancellationToken, Task<ErrorOrResult<T>>> func, CancellationToken token = default )
    {
        ErrorOrResult<UserRecord> loginResult = await VerifyLogin( connection, transaction, activity, request, token );

        return loginResult.TryGetValue( out UserRecord? record, out Error[]? errors )
                   ? await func( connection, transaction, record, token )
                   : errors;
    }

    public virtual async ValueTask<ErrorOrResult<Tokens>> Verify( DbConnection connection, DbTransaction transaction, Activity? activity, ILoginRequest request, ClaimType types, CancellationToken token = default )
    {
        ErrorOrResult<UserRecord> loginResult = await VerifyLogin( connection, transaction, activity, request, token );

        return loginResult.TryGetValue( out UserRecord? record, out Error[]? errors )
                   ? await GetToken( connection, transaction, activity, record, types, token )
                   : errors;
    }


    public virtual async ValueTask<ErrorOrResult<Tokens>> Register<TUser>( DbConnection connection, DbTransaction transaction, Activity? activity, ILoginRequest<TUser> request, CancellationToken token = default )
        where TUser : class, IUserData<Guid>
    {
        UserRecord? record = await Users.Get( connection, transaction, activity, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is not null ) { return Error.Create( Status.Conflict, $"{nameof(UserRecord.UserName)} is already taken. Chose another {nameof(request.UserName)}" ); }

        record = CreateNewUser( request );
        record = await Users.Insert( connection, transaction, activity, record, token );
        return await GetToken( connection, transaction, activity, record, DEFAULT_CLAIM_TYPES, token );
    }
    protected virtual UserRecord CreateNewUser<TUser>( ILoginRequest<TUser> request, UserRecord? caller = default )
        where TUser : class, IUserData<Guid> => UserRecord.Create( request, request.Data.Rights, caller );


    public ValueTask<ErrorOrResult<Tokens>> Register<TUser>( Activity? activity, ILoginRequest<TUser> request, CancellationToken token = default )
        where TUser : class, IUserData<Guid> => this.TryCall( Register, activity, request, token );
    public ValueTask<ErrorOrResult<T>> Verify<T>( Activity? activity, ILoginRequest request, Func<DbConnection, DbTransaction, UserRecord, ErrorOrResult<T>>                               func, CancellationToken token = default ) => this.TryCall( Verify, activity, request, func, token );
    public ValueTask<ErrorOrResult<T>> Verify<T>( Activity? activity, ILoginRequest request, Func<DbConnection, DbTransaction, UserRecord, CancellationToken, ValueTask<ErrorOrResult<T>>> func, CancellationToken token = default ) => this.TryCall( Verify, activity, request, func, token );
    public ValueTask<ErrorOrResult<T>> Verify<T>( Activity? activity, ILoginRequest request, Func<DbConnection, DbTransaction, UserRecord, CancellationToken, Task<ErrorOrResult<T>>>      func, CancellationToken token = default ) => this.TryCall( Verify, activity, request, func, token );
}
