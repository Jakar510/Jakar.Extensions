﻿namespace Jakar.Extensions;


[Serializable]
public class VerifyRequest : BaseClass, ILoginRequest, ICredentials, ICloneable, IEquatable<VerifyRequest>, JsonModels.IJsonModel
{
    [JsonExtensionData]                                            public         IDictionary<string, JToken?>? AdditionalData { get; set; }
    [JsonIgnore]                                                   public virtual bool                          IsValid        => !string.IsNullOrWhiteSpace( UserName ) && !string.IsNullOrWhiteSpace( Password );
    [JsonProperty( nameof(Password), Required = Required.Always )] public         string                        Password       { get; init; } = string.Empty;
    [JsonProperty( nameof(UserName), Required = Required.Always )] public         string                        UserName       { get; init; } = string.Empty;


    public VerifyRequest() { }
    public VerifyRequest( string? userName, string? password )
    {
        UserName = userName ?? throw new ArgumentNullException( nameof(userName) );
        Password = password ?? throw new ArgumentNullException( nameof(password) );
    }


    public bool ValidatePassword()                                 => ValidatePassword( PasswordValidator.Default );
    public bool ValidatePassword( in PasswordValidator validator ) => validator.Validate( Password );


    public virtual NetworkCredential GetCredential( Uri uri, string authType ) => new(UserName, Password.ToSecureString());


    public VerifyRequest Clone() => new(UserName, Password);
    object ICloneable.   Clone() => Clone();


    public bool Equals( VerifyRequest? other )
    {
        if ( other is null ) { return false; }

        return ReferenceEquals( this, other ) || string.Equals( UserName, other.UserName, StringComparison.Ordinal ) && string.Equals( Password, other.Password, StringComparison.Ordinal );
    }
    public override bool Equals( object? other )
    {
        if ( other is null ) { return false; }

        return ReferenceEquals( this, other ) || other is VerifyRequest request && Equals( request );
    }
    public override int GetHashCode() => HashCode.Combine( UserName, Password );


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
    public VerifyRequest( VerifyRequest request,   T?      data ) : this( request.UserName, request.Password, data ) { }


    public new VerifyRequest<T> Clone() => new(UserName, Password, Data);


    public bool Equals( VerifyRequest<T>? other ) => Equals( other, EqualityComparer<T?>.Default );
    public bool Equals( VerifyRequest<T>? other, IEqualityComparer<T?> comparer )
    {
        if ( other is null ) { return false; }

        return ReferenceEquals( this, other ) || base.Equals( other ) && comparer.Equals( Data, other.Data );
    }
    public override bool Equals( object? other )
    {
        if ( other is null ) { return false; }

        return ReferenceEquals( this, other ) || other is VerifyRequest<T> request && Equals( request );
    }
    public override int GetHashCode() => HashCode.Combine( base.GetHashCode(), Data );


    public static bool operator ==( VerifyRequest<T>? left, VerifyRequest<T>? right ) => Equals( left, right );
    public static bool operator !=( VerifyRequest<T>? left, VerifyRequest<T>? right ) => !Equals( left, right );
}
