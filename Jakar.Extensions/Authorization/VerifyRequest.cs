#nullable enable
namespace Jakar.Extensions;


[Serializable]
public class VerifyRequest : BaseClass, ILoginRequest, ICredentials, ICloneable, IEquatable<VerifyRequest>
{
    public virtual bool IsValid => !string.IsNullOrWhiteSpace( UserLogin ) && !string.IsNullOrWhiteSpace( UserPassword );


    [Required( ErrorMessage = "Email address is required." )]
    [JsonProperty( nameof(UserLogin), Required = Required.Always )]
    public string UserLogin { get; init; } = string.Empty;


    [Required( ErrorMessage = "Password is required." )]
    [JsonProperty( nameof(UserPassword), Required = Required.Always )]
    public string UserPassword { get; init; } = string.Empty;
    

    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }


    public VerifyRequest() { }
    public VerifyRequest( string? userName, string? userPassword )
    {
        UserLogin    = userName ?? throw new ArgumentNullException( nameof(userName) );
        UserPassword = userPassword ?? throw new ArgumentNullException( nameof(userPassword) );
    }


    public bool ValidatePassword() => ValidatePassword( PasswordValidator.Default );
    public bool ValidatePassword( PasswordValidator validator ) => validator.Validate( UserPassword );


    public virtual NetworkCredential GetCredential( Uri uri, string authType ) => new(UserLogin, UserPassword.ToSecureString());


    public VerifyRequest Clone() => new(UserLogin, UserPassword);
    object ICloneable.Clone() => Clone();


    public bool Equals( VerifyRequest? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( UserLogin, other.UserLogin, StringComparison.Ordinal ) && string.Equals( UserPassword, other.UserPassword, StringComparison.Ordinal );
    }
    public override bool Equals( object? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return other is VerifyRequest request && Equals( request );
    }
    public override int GetHashCode() => HashCode.Combine( UserLogin, UserPassword );


    public static bool operator ==( VerifyRequest? left, VerifyRequest? right ) => Equals( left, right );
    public static bool operator !=( VerifyRequest? left, VerifyRequest? right ) => !Equals( left, right );
}



[SuppressMessage( "ReSharper", "NullableWarningSuppressionIsUsed" )]
public class VerifyRequest<T> : VerifyRequest, IEquatable<VerifyRequest<T>>
{
    [JsonProperty( nameof(Data), Required = Required.AllowNull )] public T? Data { get; init; }

    public override bool IsValid => Data is IValidator validator
                                        ? base.IsValid && validator.IsValid
                                        : base.IsValid;


    public VerifyRequest() { }
    public VerifyRequest( string?       userLogin, string? userPassword, T? data ) : base( userLogin, userPassword ) => Data = data;
    public VerifyRequest( VerifyRequest request,   T?      data ) : this( request.UserLogin, request.UserPassword, data ) { }


    public new VerifyRequest<T> Clone() => new(UserLogin, UserPassword, Data);


    public bool Equals( VerifyRequest<T>? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return base.Equals( other ) && EqualityComparer<T?>.Default.Equals( Data, other.Data );
    }
    public override bool Equals( object? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return other is VerifyRequest<T> request && Equals( request );
    }
    public override int GetHashCode() => HashCode.Combine( base.GetHashCode(), Data );


    public static bool operator ==( VerifyRequest<T>? left, VerifyRequest<T>? right ) => Equals( left, right );
    public static bool operator !=( VerifyRequest<T>? left, VerifyRequest<T>? right ) => !Equals( left, right );
}
