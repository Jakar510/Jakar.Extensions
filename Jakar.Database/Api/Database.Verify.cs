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
    public virtual ValueTask<ErrorOr<SubscriptionStatus>> ValidateSubscription( DbConnection connection, DbTransaction? transaction, UserRecord record, CancellationToken token = default ) => new(SubscriptionStatus.Ok);


    protected virtual async ValueTask<ErrorOr<UserRecord>> VerifyLogin( DbConnection connection, DbTransaction transaction, VerifyRequest request, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is null ) { return Error.NotFound(); }

        try
        {
            if ( UserRecord.VerifyPassword( ref record, request ) is false )
            {
                record = record.MarkBadLogin();
                return Error.Unauthorized();
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

            ErrorOr<SubscriptionStatus> status = await ValidateSubscription( connection, transaction, record, token );

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
            await Users.Update( connection, transaction, record, token );
        }
    }


    public virtual async ValueTask<ErrorOr<T>> Verify<T>( DbConnection connection, DbTransaction transaction, VerifyRequest request, Func<UserRecord, T> func, CancellationToken token = default )
    {
        ErrorOr<UserRecord> loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.TryGetValue( out UserRecord? record, out Error[]? errors )
                   ? func( record )
                   : errors;
    }

    public virtual async ValueTask<ErrorOr<T>> Verify<T>( DbConnection connection, DbTransaction transaction, VerifyRequest request, Func<UserRecord, ValueTask<T>> func, CancellationToken token = default )
    {
        ErrorOr<UserRecord> loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.TryGetValue( out UserRecord? record, out Error[]? errors )
                   ? await func( record )
                   : errors;
    }

    public virtual async ValueTask<ErrorOr<T>> Verify<T>( DbConnection connection, DbTransaction transaction, VerifyRequest request, Func<UserRecord, Task<T>> func, CancellationToken token = default )
    {
        ErrorOr<UserRecord> loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.TryGetValue( out UserRecord? record, out Error[]? errors )
                   ? await func( record )
                   : errors;
    }

    public virtual async ValueTask<ErrorOr<Tokens>> Verify( DbConnection connection, DbTransaction transaction, VerifyRequest request, ClaimType types, CancellationToken token = default )
    {
        ErrorOr<UserRecord> loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.TryGetValue( out UserRecord? record, out Error[]? errors )
                   ? await GetToken( connection, transaction, record, types, token )
                   : errors;
    }


    public virtual async ValueTask<ErrorOr<Tokens>> Register<TUser>( DbConnection connection, DbTransaction transaction, ILoginRequest<TUser> request, CancellationToken token = default )
        where TUser : class, IUserData<Guid>
    {
        UserRecord? record = await Users.Get( connection, transaction, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is not null ) { return Error.Create( Status.Conflict, $"{nameof(UserRecord.UserName)} is already taken. Chose another {nameof(request.UserName)}" ); }

        record = CreateNewUser( request );
        record = await Users.Insert( connection, transaction, record, token );
        return await GetToken( connection, transaction, record, DEFAULT_CLAIM_TYPES, token );
    }
    protected virtual UserRecord CreateNewUser<TUser>( ILoginRequest<TUser> request, UserRecord? caller = default )
        where TUser : class, IUserData<Guid> => UserRecord.Create( request, request.Data.Rights, caller );


    public ValueTask<ErrorOr<Tokens>> Register<TUser>( ILoginRequest<TUser> request, CancellationToken token = default )
        where TUser : class, IUserData<Guid> => this.TryCall( Register, request, token );
    public ValueTask<ErrorOr<T>> Verify<T>( VerifyRequest request, Func<UserRecord, T>            func, CancellationToken token = default ) => this.TryCall( Verify, request, func, token );
    public ValueTask<ErrorOr<T>> Verify<T>( VerifyRequest request, Func<UserRecord, ValueTask<T>> func, CancellationToken token = default ) => this.TryCall( Verify, request, func, token );
    public ValueTask<ErrorOr<T>> Verify<T>( VerifyRequest request, Func<UserRecord, Task<T>>      func, CancellationToken token = default ) => this.TryCall( Verify, request, func, token );
}
