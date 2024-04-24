// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:53 PM

namespace Jakar.Database;


public abstract partial class Database
{
    [SuppressMessage( "ReSharper", "UnusedParameter.Global" )] public virtual ValueTask<DateTimeOffset?> GetSubscriptionExpiration( DbConnection connection, DbTransaction? transaction, UserRecord record, CancellationToken token = default ) => new(default(DateTimeOffset?));
    public virtual ValueTask<TRecord?> TryGetSubscription<TRecord>( DbConnection connection, DbTransaction? transaction, UserRecord record, CancellationToken token = default )
        where TRecord : UserSubscription<TRecord>, IDbReaderMapping<TRecord> => default;


    /// <summary> </summary>
    /// <returns> <see langword="true"/> is Subscription is valid; otherwise <see langword="false"/> </returns>
    [SuppressMessage( "ReSharper", "UnusedParameter.Global" )]
    public virtual ValueTask<bool> ValidateSubscription( DbConnection connection, DbTransaction? transaction, UserRecord record, CancellationToken token = default ) => new(true);


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
                return Error.Create( Status.Disabled );
            }

            if ( record.IsDisabled )
            {
                record = record.MarkBadLogin();
                return Error.Create( Status.Disabled );
            }

            if ( record.IsLocked )
            {
                record = record.MarkBadLogin();
                return Error.Create( Status.Locked );
            }

            if ( !await ValidateSubscription( connection, transaction, record, token ) )
            {
                record = record.MarkBadLogin();
                return Error.Create( Status.PaymentRequired );
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


    public virtual async ValueTask<ErrorOr<Tokens>> Register( DbConnection connection, DbTransaction transaction, VerifyRequest<UserModel<Guid>> request, CancellationToken token = default )
    {
        if ( request.Data is null ) { return Error.Create( Status.BadRequest, $"{nameof(request.Data)} is null" ); }

        UserRecord? record = await Users.Get( connection, transaction, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is not null ) { return Error.Create( Status.Conflict, $"{nameof(UserRecord.UserName)} is already taken. Chose another {nameof(request.UserName)}" ); }

        record = CreateNewUser( request );
        record = await Users.Insert( connection, transaction, record, token );
        return await GetToken( connection, transaction, record, DEFAULT_CLAIM_TYPES, token );
    }
    protected virtual UserRecord CreateNewUser( VerifyRequest<UserModel<Guid>> request ) => UserRecord.Create( request, string.Empty );


    public ValueTask<ErrorOr<Tokens>> Register( VerifyRequest<UserModel<Guid>> request, CancellationToken              token                         = default ) => this.TryCall( Register, request, token );
    public ValueTask<ErrorOr<T>>      Verify<T>( VerifyRequest                 request, Func<UserRecord, T>            func, CancellationToken token = default ) => this.TryCall( Verify,   request, func, token );
    public ValueTask<ErrorOr<T>>      Verify<T>( VerifyRequest                 request, Func<UserRecord, ValueTask<T>> func, CancellationToken token = default ) => this.TryCall( Verify,   request, func, token );
    public ValueTask<ErrorOr<T>>      Verify<T>( VerifyRequest                 request, Func<UserRecord, Task<T>>      func, CancellationToken token = default ) => this.TryCall( Verify,   request, func, token );
}
