// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


public interface IUniqueID : IUniqueID<Guid>;



public interface IDateCreated : IUniqueID
{
    public DateTimeOffset DateCreated { get; }
}



public interface ILastModified : IDateCreated
{
    public DateTimeOffset? LastModified { get; }
}



public interface ICreatedBy : IDateCreated
{
    public RecordID<UserRecord>? CreatedBy { get; }
}



public interface IRecordPair<TSelf> : IDateCreated
    where TSelf : IRecordPair<TSelf>, ITableRecord<TSelf>
{
    Guid IUniqueID<Guid>.       ID => ID.Value;
    public new RecordID<TSelf> ID { get; }

    [Pure] public UInt128 GetHash();
}



public interface ITableRecord<TSelf> : IRecordPair<TSelf>, IJsonModel<TSelf>
    where TSelf : ITableRecord<TSelf>
{
    public abstract static ReadOnlyMemory<PropertyInfo>                ClassProperties  { [Pure] get; }
    public abstract static int                                         PropertyCount    { get; }
    public abstract static ImmutableDictionary<string, ColumnMetaData> PropertyMetaData { [Pure] get; }
    public abstract static string                                      TableName        { [Pure] get; }


    static ITableRecord() => DbMigrations.Migrations.AddMigrations<TSelf>();


    [Pure] public abstract static MigrationRecord    CreateTable( ulong   migrationID );
    [Pure] public abstract static TSelf             Create( DbDataReader reader );
    [Pure] public                 PostgresParameters ToDynamicParameters();


    [Pure] public RecordPair<TSelf> ToPair();
    public        TSelf             NewID( RecordID<TSelf> id );
}



[Serializable]
public abstract record TableRecord<TSelf> : BaseRecord<TSelf>, IRecordPair<TSelf>, ILastModified
    where TSelf : TableRecord<TSelf>, ITableRecord<TSelf>
{
    protected internal static readonly PropertyInfo[]   Properties = typeof(TSelf).GetProperties(BindingFlags.Instance | BindingFlags.Public);
    protected                          DateTimeOffset?  _lastModified;
    private                            RecordID<TSelf> __id;


    public static ReadOnlyMemory<PropertyInfo> ClassProperties { [Pure] get => Properties; }
    public static int                          PropertyCount   => Properties.Length;
    public        DateTimeOffset               DateCreated     { get;                  init; }
    [Key] public  RecordID<TSelf>             ID              { get => __id;          init => __id = value; }
    public        DateTimeOffset?              LastModified    { get => _lastModified; init => _lastModified = value; }


    protected TableRecord( ref readonly RecordID<TSelf> id, ref readonly DateTimeOffset dateCreated, ref readonly DateTimeOffset? lastModified, JsonObject? additionalData = null )
    {
        this.DateCreated    = dateCreated;
        _lastModified       = lastModified;
        __id                = id;
        this.AdditionalData = additionalData;
    }


    [Pure] public UInt128 GetHash()
    {
        ReadOnlySpan<char> json = ( (TSelf)this ).ToJson();
        return json.Hash128();
    }
    public TSelf Modified()
    {
        _lastModified = DateTimeOffset.UtcNow;
        return (TSelf)this;
    }
    public TSelf NewID( RecordID<TSelf> id )
    {
        __id = id;
        return (TSelf)this;
    }
    [Pure] public RecordPair<TSelf> ToPair() => new(ID, DateCreated);


    public static PostgresParameters GetDynamicParameters( TSelf record ) => GetDynamicParameters(in record.__id);
    public static PostgresParameters GetDynamicParameters( ref readonly RecordID<TSelf> id )
    {
        PostgresParameters parameters = new();
        parameters.Add(nameof(ID), id.Value);
        return parameters;
    }

    public virtual PostgresParameters ToDynamicParameters()
    {
        PostgresParameters parameters = new();
        parameters.Add(nameof(ID),           ID.Value);
        parameters.Add(nameof(DateCreated),  DateCreated);
        parameters.Add(nameof(LastModified), LastModified);
        return parameters;
    }


    [Pure] protected static TValue TryGet<TValue>( DbDataReader reader, string key )
    {
        int index = reader.GetOrdinal(key);
        return (TValue)reader.GetValue(index);
    }


    public void Deconstruct( out RecordID<TSelf> id, out DateTimeOffset dateCreated, out DateTimeOffset? lastModified, out JsonObject? additionalData )
    {
        additionalData = this.AdditionalData;
        id             = this.ID;
        dateCreated    = this.DateCreated;
        lastModified   = this.LastModified;
    }



    [Serializable]
    public class RecordCollection( int capacity = Buffers.DEFAULT_CAPACITY ) : RecordCollection<TSelf>(capacity)
    {
        public RecordCollection( params ReadOnlySpan<TSelf> values ) : this() => Add(values);
        public RecordCollection( IEnumerable<TSelf>         values ) : this() => Add(values);
    }
}



[Serializable]
public abstract record OwnedTableRecord<TSelf> : TableRecord<TSelf>, ICreatedBy
    where TSelf : OwnedTableRecord<TSelf>, ITableRecord<TSelf>
{
    public RecordID<UserRecord>? CreatedBy { get; set; }


    protected OwnedTableRecord( ref readonly RecordID<UserRecord>? createdBy, ref readonly RecordID<TSelf> id, ref readonly DateTimeOffset dateCreated, ref readonly DateTimeOffset? lastModified, JsonObject? additionalData = null ) : base(in id, in dateCreated, in lastModified, additionalData) { this.CreatedBy = createdBy; }


    public static PostgresParameters GetDynamicParameters( UserRecord user )
    {
        PostgresParameters parameters = new();
        parameters.Add(nameof(CreatedBy), user.ID.Value);
        return parameters;
    }
    protected static PostgresParameters GetDynamicParameters( OwnedTableRecord<TSelf> record )
    {
        PostgresParameters parameters = new();
        parameters.Add(nameof(CreatedBy), record.CreatedBy);
        return parameters;
    }


    public override PostgresParameters ToDynamicParameters()
    {
        PostgresParameters parameters = base.ToDynamicParameters();
        parameters.Add(nameof(CreatedBy), CreatedBy);
        return parameters;
    }


    public async ValueTask<UserRecord?> GetUser( NpgsqlConnection           connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get(connection, transaction, true,      GetDynamicParameters(this), token);
    public async ValueTask<UserRecord?> GetUserWhoCreated( NpgsqlConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get(connection, transaction, CreatedBy, token);


     public TSelf WithOwner( UserRecord  user )   => (TSelf)( this with { CreatedBy = user.ID } );
     public bool   Owns( UserRecord       record ) => CreatedBy == record.ID;
     public bool   DoesNotOwn( UserRecord record ) => CreatedBy != record.ID;


    public override int CompareTo( TSelf? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return Nullable.Compare(CreatedBy, other.CreatedBy);
    }
    public void Deconstruct( out RecordID<UserRecord>? createdBy, out RecordID<TSelf> ID, out DateTimeOffset DateCreated, out DateTimeOffset? LastModified, out JsonObject? AdditionalData )
    {
        createdBy      = this.CreatedBy;
        ID             = this.ID;
        DateCreated    = this.DateCreated;
        LastModified   = this.LastModified;
        AdditionalData = this.AdditionalData;
    }
}
