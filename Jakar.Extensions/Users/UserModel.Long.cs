// Jakar.Extensions :: Jakar.Extensions
// 4/4/2024  13:50


// ReSharper disable CheckNamespace

namespace Jakar.Extensions.UserLong;
// ReSharper restore CheckNamespace



[Serializable]
public sealed class UserAddress : UserAddress<UserAddress, long>, IAddress<UserAddress, long>, IJsonModel<UserAddress>
{
    public static JsonSerializerContext       JsonContext   => JakarModelsLongContext.Default;
    public static JsonTypeInfo<UserAddress>   JsonTypeInfo  => JakarModelsLongContext.Default.UserAddress;
    public static JsonTypeInfo<UserAddress[]> JsonArrayInfo => JakarModelsLongContext.Default.UserAddressArray;
    public UserAddress() : base() { }
    public UserAddress( Match                        match ) : base(match) { }
    public UserAddress( IAddress<long>               address ) : base(address) { }
    public UserAddress( string                       line1, string line2, string city, string stateOrProvince, string postalCode, string country, long id = 0 ) : base(line1, line2, city, stateOrProvince, postalCode, country, id) { }
    public static UserAddress Create( Match          match )                                                                                                    => new(match);
    public static UserAddress Create( IAddress<long> address )                                                                                                  => new(address);
    public static UserAddress Create( string         line1, string line2, string city, string stateOrProvince, string postalCode, string country, long id = 0 ) => new(line1, line2, city, stateOrProvince, postalCode, country, id);
    public new static UserAddress Parse( string value, IFormatProvider? provider )
    {
        Match match = Validate.Re.Address.Match(value);
        return new UserAddress(match);
    }
    public new static bool TryParse( string? value, IFormatProvider? provider, [NotNullWhen(true)] out UserAddress? result )
    {
        try
        {
            result = !string.IsNullOrWhiteSpace(value)
                         ? Parse(value, provider)
                         : null;

            return result is not null;
        }
        catch ( Exception )
        {
            result = null;
            return false;
        }
    }
    public override bool Equals( object? other )                              => other is UserAddress x && Equals(x);
    public override int  GetHashCode()                                        => base.GetHashCode();
    public static   bool operator ==( UserAddress? left, UserAddress? right ) => EqualityComparer<UserAddress>.Default.Equals(left, right);
    public static   bool operator !=( UserAddress? left, UserAddress? right ) => !EqualityComparer<UserAddress>.Default.Equals(left, right);
    public static   bool operator >( UserAddress   left, UserAddress  right ) => left.CompareTo(right) > 0;
    public static   bool operator >=( UserAddress  left, UserAddress  right ) => left.CompareTo(right) >= 0;
    public static   bool operator <( UserAddress   left, UserAddress  right ) => left.CompareTo(right) < 0;
    public static   bool operator <=( UserAddress  left, UserAddress  right ) => left.CompareTo(right) <= 0;
}



[Serializable]
public sealed class GroupModel : GroupModel<GroupModel, long>, IGroupModel<GroupModel, long>, IJsonModel<GroupModel>
{
    public static JsonSerializerContext      JsonContext   => JakarModelsLongContext.Default;
    public static JsonTypeInfo<GroupModel>   JsonTypeInfo  => JakarModelsLongContext.Default.GroupModel;
    public static JsonTypeInfo<GroupModel[]> JsonArrayInfo => JakarModelsLongContext.Default.GroupModelArray;
    public        long                        ID            { get; init; }
    public GroupModel( string                            nameOfGroup, long? ownerID, long? createdBy, long id, string rights ) : base(nameOfGroup, ownerID, createdBy, id, rights) { }
    public GroupModel( IGroupModel<long>                 model ) : base(model) { }
    public static   GroupModel Create( IGroupModel<long> model )            => new(model);
    public override bool       Equals( object?           other )            => other is GroupModel x && Equals(x);
    public override int        GetHashCode()                                => base.GetHashCode();
    public static   bool operator ==( GroupModel? left, GroupModel? right ) => EqualityComparer<GroupModel>.Default.Equals(left, right);
    public static   bool operator !=( GroupModel? left, GroupModel? right ) => !EqualityComparer<GroupModel>.Default.Equals(left, right);
    public static   bool operator >( GroupModel   left, GroupModel  right ) => left.CompareTo(right) > 0;
    public static   bool operator >=( GroupModel  left, GroupModel  right ) => left.CompareTo(right) >= 0;
    public static   bool operator <( GroupModel   left, GroupModel  right ) => left.CompareTo(right) < 0;
    public static   bool operator <=( GroupModel  left, GroupModel  right ) => left.CompareTo(right) <= 0;
}



[Serializable]
public sealed class RoleModel : RoleModel<RoleModel, long>, IRoleModel<RoleModel, long>, IJsonModel<RoleModel>
{
    public static JsonSerializerContext     JsonContext   => JakarModelsLongContext.Default;
    public static JsonTypeInfo<RoleModel>   JsonTypeInfo  => JakarModelsLongContext.Default.RoleModel;
    public static JsonTypeInfo<RoleModel[]> JsonArrayInfo => JakarModelsLongContext.Default.RoleModelArray;
    public RoleModel( string                           nameOfRole, string rights, long id ) : base(nameOfRole, rights, id) { }
    public RoleModel( IRoleModel<long>                 model ) : base(model) { }
    public static   RoleModel Create( IRoleModel<long> model )            => new(model);
    public override bool      Equals( object?          other )            => other is UserModel x && Equals(x);
    public override int       GetHashCode()                               => base.GetHashCode();
    public static   bool operator ==( RoleModel? left, RoleModel? right ) => EqualityComparer<RoleModel>.Default.Equals(left, right);
    public static   bool operator !=( RoleModel? left, RoleModel? right ) => !EqualityComparer<RoleModel>.Default.Equals(left, right);
    public static   bool operator >( RoleModel   left, RoleModel  right ) => left.CompareTo(right) > 0;
    public static   bool operator >=( RoleModel  left, RoleModel  right ) => left.CompareTo(right) >= 0;
    public static   bool operator <( RoleModel   left, RoleModel  right ) => left.CompareTo(right) < 0;
    public static   bool operator <=( RoleModel  left, RoleModel  right ) => left.CompareTo(right) <= 0;
}



[Serializable]
[method: SetsRequiredMembers]
public sealed class FileData( long fileSize, string hash, string payload, FileMetaData metaData, long id = 0 ) : FileData<FileData, long, FileMetaData>(fileSize, hash, payload, id, metaData), IFileData<FileData, long, FileMetaData>, IJsonModel<FileData>, IEqualComparable<FileData>
{
    public static JsonSerializerContext    JsonContext   => JakarModelsLongContext.Default;
    public static JsonTypeInfo<FileData>   JsonTypeInfo  => JakarModelsLongContext.Default.FileData;
    public static JsonTypeInfo<FileData[]> JsonArrayInfo => JakarModelsLongContext.Default.FileDataArray;
    [SetsRequiredMembers] public FileData( IFileData<long, FileMetaData> file ) : this(file, file.MetaData) { }
    [SetsRequiredMembers] public FileData( IFileData<long>               file,     FileMetaData              metaData ) : this(file.FileSize, file.Hash, file.Payload, metaData) { }
    [SetsRequiredMembers] public FileData( FileMetaData                  metaData, params ReadOnlySpan<byte> content ) : this(content.Length, content.Hash_SHA512(), Convert.ToBase64String(content), metaData) { }


    public static FileData Create( long       fileSize, string    hash, string payload, long id, FileMetaData metaData ) => new(fileSize, hash, payload, metaData, id);
    public static bool operator ==( FileData? left,     FileData? right ) => EqualityComparer<FileData>.Default.Equals(left, right);
    public static bool operator !=( FileData? left,     FileData? right ) => !EqualityComparer<FileData>.Default.Equals(left, right);
    public static bool operator >( FileData   left,     FileData  right ) => left.CompareTo(right) > 0;
    public static bool operator >=( FileData  left,     FileData  right ) => left.CompareTo(right) >= 0;
    public static bool operator <( FileData   left,     FileData  right ) => left.CompareTo(right) < 0;
    public static bool operator <=( FileData  left,     FileData  right ) => left.CompareTo(right) <= 0;
}



[Serializable]
public sealed class CurrentLocation : BaseClass<CurrentLocation>, ICurrentLocation<long>, IEqualComparable<CurrentLocation>, IJsonModel<CurrentLocation>
{
    public static JsonSerializerContext           JsonContext             => JakarModelsLongContext.Default;
    public static JsonTypeInfo<CurrentLocation>   JsonTypeInfo            => JakarModelsLongContext.Default.CurrentLocation;
    public static JsonTypeInfo<CurrentLocation[]> JsonArrayInfo           => JakarModelsLongContext.Default.CurrentLocationArray;
    public        double?                         Accuracy                { get; init; }
    public        double?                         Altitude                { get; init; }
    public        AltitudeReference               AltitudeReferenceSystem { get; init; }
    public        double?                         Course                  { get; init; }
    [Key] public  long                            ID                      { get; init; }
    public        Guid                            InstanceID              { get; init; } = Guid.Empty;
    public        bool                            IsFromMockProvider      { get; init; }
    public        double                          Latitude                { get; init; }
    public        double                          Longitude               { get; init; }
    public        double?                         Speed                   { get; init; }
    public        DateTimeOffset                  Timestamp               { get; init; }
    public        double?                         VerticalAccuracy        { get; init; }


    public CurrentLocation() { }
    public CurrentLocation( ICurrentLocation<long> point )
    {
        Latitude                = point.Latitude;
        Longitude               = point.Longitude;
        Timestamp               = point.Timestamp;
        Altitude                = point.Altitude;
        Accuracy                = point.Accuracy;
        VerticalAccuracy        = point.VerticalAccuracy;
        Speed                   = point.Speed;
        Course                  = point.Course;
        IsFromMockProvider      = point.IsFromMockProvider;
        AltitudeReferenceSystem = point.AltitudeReferenceSystem;
        ID                      = point.ID;
    }


    public static double CalculateDistance( double latitudeStart, double longitudeStart, ICurrentLocation<long> locationEnd, DistanceUnit units ) =>
        CalculateDistance(latitudeStart, longitudeStart, locationEnd.Latitude, locationEnd.Longitude, units);
    public static double CalculateDistance( ICurrentLocation<long> locationStart, double latitudeEnd, double longitudeEnd, DistanceUnit units ) =>
        CalculateDistance(locationStart.Latitude, locationStart.Longitude, latitudeEnd, longitudeEnd, units);
    public static double CalculateDistance( ICurrentLocation<long> locationStart, ICurrentLocation<long> locationEnd, DistanceUnit units ) =>
        CalculateDistance(locationStart.Latitude, locationStart.Longitude, locationEnd.Latitude, locationEnd.Longitude, units);
    public static double CalculateDistance( double latitudeStart, double longitudeStart, double latitudeEnd, double longitudeEnd, DistanceUnit unit ) =>
        unit switch
        {
            DistanceUnit.Kilometers => UnitConverters.CoordinatesToKilometers(latitudeStart, longitudeStart, latitudeEnd, longitudeEnd),
            DistanceUnit.Miles      => UnitConverters.CoordinatesToMiles(latitudeStart, longitudeStart, latitudeEnd, longitudeEnd),
            _                       => throw new OutOfRangeException(unit)
        };


    // private CurrentLocation( Location? point )
    // {
    //     InstanceID = long.CreateVersion7();
    //     if ( point is null ) { return; }
    //
    //     Latitude                = point.Latitude;
    //     Longitude               = point.Longitude;
    //     Timestamp               = point.Timestamp;
    //     Altitude                = point.Altitude;
    //     Accuracy                = point.Accuracy;
    //     VerticalAccuracy        = point.VerticalAccuracy;
    //     Speed                   = point.Speed;
    //     Course                  = point.Course;
    //     IsFromMockProvider      = point.IsFromMockProvider;
    //     AltitudeReferenceSystem = (AltitudeReference)point.AltitudeReferenceSystem;
    // }
    //
    // public static async Task<CurrentLocation> Create( CancellationToken token, GeolocationAccuracy accuracy = GeolocationAccuracy.Best )
    // {
    //     var       request  = new GeolocationRequest(accuracy);
    //     Location? location = await Geolocation.GetLocationAsync(request, token);
    //     return new CurrentLocation(location);
    // }


    // public static implicit operator Location( CurrentLocation point ) => new()
    //                                                                           {
    //                                                                               Latitude                = point.Latitude,
    //                                                                               Longitude               = point.Longitude,
    //                                                                               Timestamp               = point.Timestamp,
    //                                                                               Altitude                = point.Altitude,
    //                                                                               Accuracy                = point.Accuracy,
    //                                                                               VerticalAccuracy        = point.VerticalAccuracy,
    //                                                                               Speed                   = point.Speed,
    //                                                                               Course                  = point.Course,
    //                                                                               IsFromMockProvider      = point.IsFromMockProvider,
    //                                                                               AltitudeReferenceSystem = point.AltitudeReferenceSystem
    //                                                                           };


    public bool IsValid( ICurrentLocation<long> location, DistanceUnit units, double maxDistance )
    {
        if ( InstanceID == Guid.Empty ) { return false; }

        return CalculateDistance(this, location, units) <= maxDistance;
    }


    public double CalculateDistance( ICurrentLocation<long> locationStart, DistanceUnit units )                              => CalculateDistance(locationStart, this,           units);
    public double CalculateDistance( double                 latitudeStart, double       longitudeStart, DistanceUnit units ) => CalculateDistance(latitudeStart, longitudeStart, this, units);


    public bool EqualInstance( ICurrentLocation<long> other ) => InstanceID.Equals(other.InstanceID);
    public override bool Equals( CurrentLocation? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return InstanceID.Equals(other.InstanceID)                       &&
               Timestamp.Equals(other.Timestamp)                         &&
               Latitude.Equals(other.Latitude)                           &&
               Longitude.Equals(other.Longitude)                         &&
               Nullable.Equals(Altitude,         other.Altitude)         &&
               Nullable.Equals(Accuracy,         other.Accuracy)         &&
               Nullable.Equals(VerticalAccuracy, other.VerticalAccuracy) &&
               Nullable.Equals(Speed,            other.Speed)            &&
               Nullable.Equals(Course,           other.Course)           &&
               IsFromMockProvider      == other.IsFromMockProvider       &&
               AltitudeReferenceSystem == other.AltitudeReferenceSystem;
    }
    public override int CompareTo( CurrentLocation? other )
    {
        if ( other is null ) { return -1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return Timestamp.CompareTo(other.Timestamp);
    }
    public override bool Equals( object? other ) => other is LocalDirectory x && Equals(x);
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(ID);
        hashCode.Add(InstanceID);
        hashCode.Add(Timestamp);
        hashCode.Add(Latitude);
        hashCode.Add(Longitude);
        hashCode.Add(Altitude);
        hashCode.Add(Accuracy);
        hashCode.Add(VerticalAccuracy);
        hashCode.Add(Speed);
        hashCode.Add(Course);
        hashCode.Add(IsFromMockProvider);
        hashCode.Add((int)AltitudeReferenceSystem);
        return hashCode.ToHashCode();
    }
    public static bool operator ==( CurrentLocation? left, CurrentLocation? right ) => EqualityComparer<CurrentLocation>.Default.Equals(left, right);
    public static bool operator !=( CurrentLocation? left, CurrentLocation? right ) => !EqualityComparer<CurrentLocation>.Default.Equals(left, right);
    public static bool operator >( CurrentLocation   left, CurrentLocation  right ) => Comparer<CurrentLocation>.Default.Compare(left, right) > 0;
    public static bool operator >=( CurrentLocation  left, CurrentLocation  right ) => Comparer<CurrentLocation>.Default.Compare(left, right) >= 0;
    public static bool operator <( CurrentLocation   left, CurrentLocation  right ) => Comparer<CurrentLocation>.Default.Compare(left, right) < 0;
    public static bool operator <=( CurrentLocation  left, CurrentLocation  right ) => Comparer<CurrentLocation>.Default.Compare(left, right) <= 0;
}



[Serializable]
public sealed class UserModel : UserModel<UserModel, long, UserAddress, GroupModel, RoleModel>, ICreateUserModel<UserModel, long, UserAddress, GroupModel, RoleModel>, IJsonModel<UserModel>
{
    public static JsonSerializerContext     JsonContext   => JakarModelsLongContext.Default;
    public static JsonTypeInfo<UserModel>   JsonTypeInfo  => JakarModelsLongContext.Default.UserModel;
    public static JsonTypeInfo<UserModel[]> JsonArrayInfo => JakarModelsLongContext.Default.UserModelArray;
    public UserModel() : base() { }
    public UserModel( IUserData<long> value ) : base(value) { }
    public UserModel( string          firstName, string lastName ) : base(firstName, lastName) { }


    public static UserModel Create( IUserData<long> model )                                                                                                                                    => new(model);
    public static UserModel Create( IUserData<long> model, IEnumerable<UserAddress>            addresses, IEnumerable<GroupModel>            groups, IEnumerable<RoleModel>            roles ) => Create(model).With(addresses).With(groups).With(roles);
    public static UserModel Create( IUserData<long> model, scoped in ReadOnlySpan<UserAddress> addresses, scoped in ReadOnlySpan<GroupModel> groups, scoped in ReadOnlySpan<RoleModel> roles ) => Create(model).With(addresses).With(groups).With(roles);
    public static async ValueTask<UserModel> CreateAsync( IUserData<long> model, IAsyncEnumerable<UserAddress> addresses, IAsyncEnumerable<GroupModel> groups, IAsyncEnumerable<RoleModel> roles, CancellationToken token = default )
    {
        UserModel user = Create(model);
        await user.Addresses.Add(addresses, token);
        await user.Groups.Add(groups, token);
        await user.Roles.Add(roles, token);
        return user;
    }
    public override bool Equals( object? other )                          => other is UserModel x && Equals(x);
    public override int  GetHashCode()                                    => base.GetHashCode();
    public static   bool operator ==( UserModel? left, UserModel? right ) => EqualityComparer<UserModel>.Default.Equals(left, right);
    public static   bool operator !=( UserModel? left, UserModel? right ) => !EqualityComparer<UserModel>.Default.Equals(left, right);
    public static   bool operator >( UserModel   left, UserModel  right ) => left.CompareTo(right) > 0;
    public static   bool operator >=( UserModel  left, UserModel  right ) => left.CompareTo(right) >= 0;
    public static   bool operator <( UserModel   left, UserModel  right ) => left.CompareTo(right) < 0;
    public static   bool operator <=( UserModel  left, UserModel  right ) => left.CompareTo(right) <= 0;
}



[Serializable]
public sealed class CreateUserModel : CreateUserModel<CreateUserModel, long, UserAddress, GroupModel, RoleModel>, ICreateUserModel<CreateUserModel, long, UserAddress, GroupModel, RoleModel>, IJsonModel<CreateUserModel>
{
    public static JsonSerializerContext           JsonContext   => JakarModelsLongContext.Default;
    public static JsonTypeInfo<CreateUserModel>   JsonTypeInfo  => JakarModelsLongContext.Default.CreateUserModel;
    public static JsonTypeInfo<CreateUserModel[]> JsonArrayInfo => JakarModelsLongContext.Default.CreateUserModelArray;
    public CreateUserModel() : base() { }
    public CreateUserModel( IUserData<long> value ) : base(value) { }
    public CreateUserModel( string          firstName, string lastName ) : base(firstName, lastName) { }


    public static CreateUserModel Create( IUserData<long> model )                                                                                                                                    => new(model);
    public static CreateUserModel Create( IUserData<long> model, IEnumerable<UserAddress>            addresses, IEnumerable<GroupModel>            groups, IEnumerable<RoleModel>            roles ) => Create(model).With(addresses).With(groups).With(roles);
    public static CreateUserModel Create( IUserData<long> model, scoped in ReadOnlySpan<UserAddress> addresses, scoped in ReadOnlySpan<GroupModel> groups, scoped in ReadOnlySpan<RoleModel> roles ) => Create(model).With(addresses).With(groups).With(roles);
    public static async ValueTask<CreateUserModel> CreateAsync( IUserData<long> model, IAsyncEnumerable<UserAddress> addresses, IAsyncEnumerable<GroupModel> groups, IAsyncEnumerable<RoleModel> roles, CancellationToken token = default )
    {
        CreateUserModel user = Create(model);
        await user.Addresses.Add(addresses, token);
        await user.Groups.Add(groups, token);
        await user.Roles.Add(roles, token);
        return user;
    }


    public override bool Equals( object? other )                                      => other is CreateUserModel x && Equals(x);
    public override int  GetHashCode()                                                => base.GetHashCode();
    public static   bool operator ==( CreateUserModel? left, CreateUserModel? right ) => EqualityComparer<CreateUserModel>.Default.Equals(left, right);
    public static   bool operator !=( CreateUserModel? left, CreateUserModel? right ) => !EqualityComparer<CreateUserModel>.Default.Equals(left, right);
    public static   bool operator >( CreateUserModel   left, CreateUserModel  right ) => Comparer<CreateUserModel>.Default.Compare(left, right) > 0;
    public static   bool operator >=( CreateUserModel  left, CreateUserModel  right ) => Comparer<CreateUserModel>.Default.Compare(left, right) >= 0;
    public static   bool operator <( CreateUserModel   left, CreateUserModel  right ) => Comparer<CreateUserModel>.Default.Compare(left, right) < 0;
    public static   bool operator <=( CreateUserModel  left, CreateUserModel  right ) => Comparer<CreateUserModel>.Default.Compare(left, right) <= 0;
}



[JsonSourceGenerationOptions(MaxDepth = 128,
                             IndentSize = 4,
                             NewLine = "\n",
                             IndentCharacter = ' ',
                             WriteIndented = true,
                             RespectNullableAnnotations = true,
                             AllowTrailingCommas = true,
                             AllowOutOfOrderMetadataProperties = true,
                             IgnoreReadOnlyProperties = true,
                             IncludeFields = true,
                             IgnoreReadOnlyFields = false,
                             PropertyNameCaseInsensitive = false,
                             ReadCommentHandling = JsonCommentHandling.Skip,
                             UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
                             RespectRequiredConstructorParameters = true)]
[JsonSerializable(typeof(UserModel[]))]
[JsonSerializable(typeof(CreateUserModel[]))]
[JsonSerializable(typeof(FileData[]))]
[JsonSerializable(typeof(GroupModel[]))]
[JsonSerializable(typeof(RoleModel[]))]
[JsonSerializable(typeof(CurrentLocation[]))]
[JsonSerializable(typeof(UserAddress[]))]
public sealed partial class JakarModelsLongContext : JsonSerializerContext
{
    static JakarModelsLongContext()
    {
        Default.UserModel.Register();
        Default.CreateUserModel.Register();
        Default.FileData.Register();
        Default.GroupModel.Register();
        Default.RoleModel.Register();
        Default.CurrentLocation.Register();
    }
}



[Serializable]
public class UserDevice : DeviceInformation, IUserDevice<long>
{
    private long    __id;
    private string? __ip;


    public long           ID        { get => __id; set => SetProperty(ref __id, value); }
    public string?        IP        { get => __ip; set => SetProperty(ref __ip, value); }
    public DateTimeOffset TimeStamp { get;         init; } = DateTimeOffset.UtcNow;


    public UserDevice() { }
    public UserDevice( IUserDevice<long> device ) : base(device)
    {
        ID        = device.ID;
        IP        = device.IP;
        TimeStamp = device.TimeStamp;
    }
    public UserDevice( string? model, string? manufacturer, string? deviceName, DeviceTypes deviceType, DeviceCategory idiom, DevicePlatform platform, AppVersion? osVersion, string deviceID, long id = default ) : base(model, manufacturer, deviceName, deviceType, idiom, platform, osVersion, deviceID) => __id = id;
}
