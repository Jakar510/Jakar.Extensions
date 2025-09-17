// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


public interface IRecordPair : IUniqueID<Guid>
{
    public DateTimeOffset DateCreated { get; }
}



public interface ITableRecord : IRecordPair
{
    public DateTimeOffset? LastModified { get; }
}



public interface IOwnedTableRecord : ITableRecord
{
    public RecordID<UserRecord>? CreatedBy { get; }
}



public interface ITableRecord<TClass> : ITableRecord, IEquatable<TClass>, IComparable<TClass>, IComparable
    where TClass : class, ITableRecord<TClass>, IDbReaderMapping<TClass>
{
    Guid IUniqueID<Guid>.       ID => ID.value;
    public new RecordID<TClass> ID { get; }


    [Pure] public RecordPair<TClass> ToPair();
    [Pure] public UInt128            GetHash();
    public        TClass             NewID( RecordID<TClass> id );
}



public interface IDbReaderMapping<TClass> : ITableRecord<TClass>, IEqualComparable<TClass>
    where TClass : class, IDbReaderMapping<TClass>, IRecordPair
{
    public abstract static        string            TableName { [Pure] get; }
    [Pure] public abstract static TClass            Create( DbDataReader reader );
    [Pure] public                 DynamicParameters ToDynamicParameters();

    // [Pure] public abstract static IAsyncEnumerable<TClass> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default );
}



[Serializable]
public abstract record TableRecord<TClass>( ref readonly RecordID<TClass> ID, ref readonly DateTimeOffset DateCreated, ref readonly DateTimeOffset? LastModified ) : BaseRecord<TClass>, ITableRecord<TClass>
    where TClass : TableRecord<TClass>, IDbReaderMapping<TClass>
{
    protected internal static readonly PropertyInfo[]   Properties    = typeof(TClass).GetProperties(BindingFlags.Instance | BindingFlags.Public);
    protected                          DateTimeOffset?  _lastModified = LastModified;
    private                            RecordID<TClass> __id          = ID;


    [Key] public RecordID<TClass> ID           { get => __id;          init => __id = value; }
    public       DateTimeOffset?  LastModified { get => _lastModified; init => _lastModified = value; }


    [Pure]
    public UInt128 GetHash()
    {
        ReadOnlySpan<char> json = ToJson();
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


    [Pure]
    protected internal TClass Validate()
    {
        if ( !Debugger.IsAttached ) { return (TClass)this; }

        DynamicParameters parameters = ToDynamicParameters();
        int               length     = parameters.ParameterNames.Count();
        if ( length == Properties.Length ) { return (TClass)this; }

        HashSet<string> missing = [..Properties.Select(static x => x.Name)];
        missing.ExceptWith(parameters.ParameterNames);

        string message = $"""
                          {typeof(TClass).Name}: {nameof(ToDynamicParameters)}.Length ({length}) != {nameof(Properties)}.Length ({Properties.Length})
                          {missing.ToPrettyJson()}
                          """;

        throw new InvalidOperationException(message);
    }


    public static DynamicParameters GetDynamicParameters( TClass record ) => GetDynamicParameters(in record.__id);
    public static DynamicParameters GetDynamicParameters( ref readonly RecordID<TClass> id )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(ID), id.value);
        return parameters;
    }

    public virtual DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(ID),           ID.value);
        parameters.Add(nameof(DateCreated),  DateCreated);
        parameters.Add(nameof(LastModified), LastModified);
        return parameters;
    }


    [Pure]
    protected static TValue TryGet<TValue>( DbDataReader reader, string key )
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
}



[Serializable]
public abstract record OwnedTableRecord<TClass>( ref readonly RecordID<UserRecord>? CreatedBy, ref readonly RecordID<TClass> ID, ref readonly DateTimeOffset DateCreated, ref readonly DateTimeOffset? LastModified ) : TableRecord<TClass>(in ID, in DateCreated, in LastModified), IOwnedTableRecord
    where TClass : OwnedTableRecord<TClass>, IDbReaderMapping<TClass>
{
    public RecordID<UserRecord>? CreatedBy { get; set; } = CreatedBy;


    public static DynamicParameters GetDynamicParameters( UserRecord user )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(CreatedBy), user.ID.value);
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
}
