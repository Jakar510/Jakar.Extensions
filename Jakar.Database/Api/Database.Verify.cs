// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:53 PM

namespace Jakar.Database;


public abstract partial class Database
{
    /// <summary> </summary>
    /// <returns>
    /// <see langword="true"/> is Subscription is valid; otherwise <see langword="false"/>
    /// </returns>
    [SuppressMessage( "ReSharper", "UnusedParameter.Global" )]
    protected virtual ValueTask<bool> ValidateSubscription( DbConnection connection, DbTransaction? transaction, UserRecord record, CancellationToken token = default ) =>

        // if ( user.SubscriptionID is null || user.SubscriptionID.Value != Guid.Empty ) { return true; }
        new(true);


    protected virtual async ValueTask<LoginResult> VerifyLogin( DbConnection connection, DbTransaction? transaction, VerifyRequest request, CancellationToken token = default )
    {
        UserRecord? record = await Users.Get( connection, transaction, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is null ) { return LoginResult.State.NotFound; }

        try
        {
            if ( !record.VerifyPassword( request.UserPassword ) ) { return LoginResult.State.BadCredentials; }

            if ( !record.IsActive ) { return LoginResult.State.Inactive; }

            if ( record.IsDisabled ) { return LoginResult.State.Disabled; }

            if ( record.IsLocked ) { return LoginResult.State.Locked; }

            if ( !await ValidateSubscription( connection, transaction, record, token ) ) { return LoginResult.State.ExpiredSubscription; }

            return record;
        }
        finally { await Users.Update( connection, transaction, record, token ); }
    }


    public virtual async ValueTask<OneOf<T, Error>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, T> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out Error? error, out UserRecord? record )
                   ? error.Value
                   : func( record );
    }

    public virtual async ValueTask<OneOf<T, Error>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, ValueTask<T>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out Error? error, out UserRecord? record )
                   ? error.Value
                   : await func( record );
    }

    public virtual async ValueTask<OneOf<T, Error>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, Task<T>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out Error? error, out UserRecord? record )
                   ? error.Value
                   : await func( record );
    }

    public virtual async ValueTask<OneOf<Tokens, Error>> Verify( DbConnection connection, DbTransaction transaction, VerifyRequest request, ClaimType types, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out Error? error, out UserRecord? record )
                   ? error.Value
                   : await GetToken( connection, transaction, record, types, token );
    }

    public virtual async ValueTask<ActionResult<T>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, ActionResult<T>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : func( caller );
    }

    public virtual async ValueTask<ActionResult<T>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, ValueTask<ActionResult<T>>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : await func( caller );
    }

    public virtual async ValueTask<ActionResult<T>> Verify<T>( DbConnection connection, DbTransaction? transaction, VerifyRequest request, Func<UserRecord, Task<ActionResult<T>>> func, CancellationToken token = default )
    {
        LoginResult loginResult = await VerifyLogin( connection, transaction, request, token );

        return loginResult.GetResult( out ActionResult? actionResult, out UserRecord? caller )
                   ? actionResult
                   : await func( caller );
    }


    public virtual async ValueTask<OneOf<Tokens, Error>> Register( DbConnection connection, DbTransaction transaction, VerifyRequest<UserData> request, CancellationToken token = default )
    {
        if ( request.Data is null ) { return new Error( Status.BadRequest, $"{nameof(request.Data)} is null" ); }

        UserRecord? record = await Users.Get( connection, transaction, true, UserRecord.GetDynamicParameters( request ), token );
        if ( record is not null ) { return new Error( Status.Conflict, $"{nameof(UserRecord.UserName)} is already taken. Chose another {nameof(request.UserLogin)}" ); }

        record = CreateNewUser( request );
        record = await Users.Insert( connection, transaction, record, token );
        return await GetToken( connection, transaction, record, DEFAULT_CLAIM_TYPES, token );
    }
    protected virtual UserRecord CreateNewUser( VerifyRequest<UserData> request ) => UserRecord.Create( request, string.Empty );


    public ValueTask<OneOf<Tokens, Error>> Register( VerifyRequest<UserData> request, CancellationToken                            token                          = default ) => this.TryCall( Register, request, token );
    public ValueTask<OneOf<T, Error>> Verify<T>( VerifyRequest               request, Func<UserRecord, T>                          func,  CancellationToken token = default ) => this.TryCall( Verify,   request, func,  token );
    public ValueTask<OneOf<T, Error>> Verify<T>( VerifyRequest               request, Func<UserRecord, ValueTask<T>>               func,  CancellationToken token = default ) => this.TryCall( Verify,   request, func,  token );
    public ValueTask<OneOf<T, Error>> Verify<T>( VerifyRequest               request, Func<UserRecord, Task<T>>                    func,  CancellationToken token = default ) => this.TryCall( Verify,   request, func,  token );
    public ValueTask<OneOf<Tokens, Error>> Verify( VerifyRequest             request, ClaimType                                    types, CancellationToken token = default ) => this.TryCall( Verify,   request, types, token );
    public ValueTask<ActionResult<T>> Verify<T>( VerifyRequest               request, Func<UserRecord, ActionResult<T>>            func,  CancellationToken token = default ) => this.TryCall( Verify,   request, func,  token );
    public ValueTask<ActionResult<T>> Verify<T>( VerifyRequest               request, Func<UserRecord, ValueTask<ActionResult<T>>> func,  CancellationToken token = default ) => this.TryCall( Verify,   request, func,  token );
    public ValueTask<ActionResult<T>> Verify<T>( VerifyRequest               request, Func<UserRecord, Task<ActionResult<T>>>      func,  CancellationToken token = default ) => this.TryCall( Verify,   request, func,  token );
}
