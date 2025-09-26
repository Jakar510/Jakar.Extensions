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



public interface ICreatedBy : ILastModified
{
    public RecordID<UserRecord>? CreatedBy { get; }
}



public interface IRecordPair<TClass> : ILastModified
    where TClass : IRecordPair<TClass>, ITableRecord<TClass>
{
    Guid IUniqueID<Guid>.       ID => ID.Value;
    public new RecordID<TClass> ID { get; }

    [Pure] public UInt128 GetHash();
}



public interface ITableRecord<TClass> : IRecordPair<TClass>, IJsonModel<TClass>
    where TClass : ITableRecord<TClass>
{
    public abstract static        string            TableName { [Pure] get; }
    [Pure] public abstract static TClass            Create( DbDataReader reader );
    [Pure] public                 DynamicParameters ToDynamicParameters();

    [Pure] public RecordPair<TClass> ToPair();
    public        TClass             NewID( RecordID<TClass> id );
}



[Serializable]
public abstract record TableRecord<TClass> : BaseRecord<TClass>, IRecordPair<TClass>
    where TClass : TableRecord<TClass>, ITableRecord<TClass>
{
    protected internal static readonly PropertyInfo[]   Properties = typeof(TClass).GetProperties(BindingFlags.Instance | BindingFlags.Public);
    protected                          DateTimeOffset?  _lastModified;
    private                            RecordID<TClass> __id;


    [Key] public RecordID<TClass> ID           { get => __id;          init => __id = value; }
    public       DateTimeOffset?  LastModified { get => _lastModified; init => _lastModified = value; }
    public       DateTimeOffset   DateCreated  { get;                  init; }


    protected TableRecord( ref readonly RecordID<TClass> id, ref readonly DateTimeOffset dateCreated, ref readonly DateTimeOffset? lastModified, JsonObject? additionalData = null )
    {
        this.DateCreated    = dateCreated;
        _lastModified       = lastModified;
        __id                = id;
        this.AdditionalData = additionalData;
    }


    [Pure] public UInt128 GetHash()
    {
        ReadOnlySpan<char> json = ( (TClass)this ).ToJson();
        return json.Hash128();
    }
    public TClass Modified()
    {
        _lastModified = DateTimeOffset.UtcNow;
        return (TClass)this;
    }
    public TClass NewID( RecordID<TClass> id )
    {
        __id = id;
        return (TClass)this;
    }
    [Pure] public RecordPair<TClass> ToPair() => new(ID, DateCreated);


    [Pure] protected internal TClass Validate()
    {
        if ( !Debugger.IsAttached ) { return (TClass)this; }

        DynamicParameters parameters = ToDynamicParameters();
        int               length     = parameters.ParameterNames.Count();
        if ( length == Properties.Length ) { return (TClass)this; }

        HashSet<string> missing = [..Properties.Select(static x => x.Name)];
        missing.ExceptWith(parameters.ParameterNames);

        string message = $"""
                          {typeof(TClass).Name}: {nameof(ToDynamicParameters)}.Length ({length}) != {nameof(Properties)}.Length ({Properties.Length})
                          {missing.ToJson(JakarDatabaseContext.Default.HashSetString)}
                          """;

        throw new InvalidOperationException(message);
    }


    public static DynamicParameters GetDynamicParameters( TClass record ) => GetDynamicParameters(in record.__id);
    public static DynamicParameters GetDynamicParameters( ref readonly RecordID<TClass> id )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(ID), id.Value);
        return parameters;
    }

    public virtual DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = new();
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



    [Serializable]
    public class RecordCollection( int capacity = Buffers.DEFAULT_CAPACITY ) : RecordCollection<TClass>(capacity)
    {
        public RecordCollection( params ReadOnlySpan<TClass> values ) : this() => Add(values);
        public RecordCollection( IEnumerable<TClass>         values ) : this() => Add(values);
    }



    public void Deconstruct( out RecordID<TClass> id, out DateTimeOffset dateCreated, out DateTimeOffset? lastModified, out JsonObject? additionalData )
    {
        additionalData = this.AdditionalData;
        id             = this.ID;
        dateCreated    = this.DateCreated;
        lastModified   = this.LastModified;
    }
}



[Serializable]
public abstract record OwnedTableRecord<TClass> : TableRecord<TClass>, ICreatedBy
    where TClass : OwnedTableRecord<TClass>, ITableRecord<TClass>
{
    public RecordID<UserRecord>? CreatedBy { get; set; }


    protected OwnedTableRecord( ref readonly RecordID<UserRecord>? createdBy, ref readonly RecordID<TClass> id, ref readonly DateTimeOffset dateCreated, ref readonly DateTimeOffset? lastModified, JsonObject? additionalData = null ) : base(in id, in dateCreated, in lastModified, additionalData) { this.CreatedBy = createdBy; }


    public static DynamicParameters GetDynamicParameters( UserRecord user )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(CreatedBy), user.ID.Value);
        return parameters;
    }
    protected static DynamicParameters GetDynamicParameters( OwnedTableRecord<TClass> record )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(CreatedBy), record.CreatedBy);
        return parameters;
    }

    public override DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = base.ToDynamicParameters();
        parameters.Add(nameof(CreatedBy), CreatedBy);
        return parameters;
    }


    public async ValueTask<UserRecord?> GetUser( NpgsqlConnection           connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get(connection, transaction, true,      GetDynamicParameters(this), token);
    public async ValueTask<UserRecord?> GetUserWhoCreated( NpgsqlConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get(connection, transaction, CreatedBy, token);


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public TClass WithOwner( UserRecord  user )   => (TClass)( this with { CreatedBy = user.ID } );
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool   Owns( UserRecord       record ) => CreatedBy == record.ID;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool   DoesNotOwn( UserRecord record ) => CreatedBy != record.ID;


    public override int CompareTo( TClass? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return Nullable.Compare(CreatedBy, other.CreatedBy);
    }
    public void Deconstruct( out RecordID<UserRecord>? CreatedBy, out RecordID<TClass> ID, out DateTimeOffset DateCreated, out DateTimeOffset? LastModified, out JsonObject? AdditionalData )
    {
        CreatedBy      = this.CreatedBy;
        ID             = this.ID;
        DateCreated    = this.DateCreated;
        LastModified   = this.LastModified;
        AdditionalData = this.AdditionalData;
    }
}
