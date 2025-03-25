// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


public interface IRecordPair : IUniqueID<Guid>
{
    public DateTimeOffset DateCreated { get; }
}



public interface IDbReaderMapping<out TRecord>
    where TRecord : IDbReaderMapping<TRecord>, IRecordPair
{
    public abstract static        string                    TableName { [Pure] get; }
    [Pure] public abstract static TRecord                   Create( DbDataReader      reader );
    [Pure] public abstract static IAsyncEnumerable<TRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default );
    [Pure] public                 DynamicParameters         ToDynamicParameters();
}



public interface ITableRecord : IRecordPair
{
    public DateTimeOffset? LastModified { get; }
}



public interface IOwnedTableRecord : ITableRecord
{
    public RecordID<UserRecord>? CreatedBy { get; }
}



public interface ITableRecord<TRecord> : ITableRecord
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public abstract static SqlCache<TRecord> SQL { get; }
    Guid IUniqueID<Guid>.                    ID  => ID.value;
    public new RecordID<TRecord>             ID  { get; }


    [Pure] public RecordPair<TRecord> ToPair();
    [Pure] public UInt128             GetHash();
    public        TRecord             NewID( RecordID<TRecord> id );
}



[Serializable]
public abstract record TableRecord<TRecord>( ref readonly RecordID<TRecord> ID, ref readonly DateTimeOffset DateCreated, ref readonly DateTimeOffset? LastModified ) : BaseRecord, ITableRecord<TRecord>, IComparable<TRecord>
    where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    protected internal static readonly PropertyInfo[]    Properties    = typeof(TRecord).GetProperties( BindingFlags.Instance | BindingFlags.Public );
    public static readonly             SqlCache<TRecord> SQLCache      = new();
    protected                          DateTimeOffset?   _lastModified = LastModified;
    private                            RecordID<TRecord> _id           = ID;


    public static SqlCache<TRecord> SQL          => SQLCache;
    [Key] public  RecordID<TRecord> ID           { get => _id;           init => _id = value; }
    public        DateTimeOffset?   LastModified { get => _lastModified; init => _lastModified = value; }


    [Pure]
    public UInt128 GetHash()
    {
        ReadOnlySpan<char> json = this.ToJson();
        return json.Hash128();
    }
    public TRecord Modified()
    {
        _lastModified = DateTimeOffset.UtcNow;
        return (TRecord)this;
    }
    public TRecord NewID( RecordID<TRecord> id )
    {
        _id = id;
        return (TRecord)this;
    }
    [Pure] public RecordPair<TRecord> ToPair() => new(ID, DateCreated);


    [Pure]
    protected internal TRecord Validate()
    {
        if ( Debugger.IsAttached is false ) { return (TRecord)this; }

        DynamicParameters parameters = ToDynamicParameters();
        int               length     = parameters.ParameterNames.Count();
        if ( length == Properties.Length ) { return (TRecord)this; }

        HashSet<string> missing = [..Properties.Select( static x => x.Name )];
        missing.ExceptWith( parameters.ParameterNames );

        string message = $"""
                          {typeof(TRecord).Name}: {nameof(ToDynamicParameters)}.Length ({length}) != {nameof(Properties)}.Length ({Properties.Length})
                          {missing.ToPrettyJson()}
                          """;

        throw new InvalidOperationException( message );
    }


    public static DynamicParameters GetDynamicParameters( TRecord record ) => GetDynamicParameters( in record._id );
    public static DynamicParameters GetDynamicParameters( ref readonly RecordID<TRecord> id )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(ID), id.value );
        return parameters;
    }

    public virtual DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(ID),           ID.value );
        parameters.Add( nameof(DateCreated),  DateCreated );
        parameters.Add( nameof(LastModified), LastModified );
        return parameters;
    }


    [Pure]
    protected static TValue TryGet<TValue>( DbDataReader reader, string key )
    {
        int index = reader.GetOrdinal( key );
        return (TValue)reader.GetValue( index );
    }


    public virtual int CompareTo( TRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int lastModifiedComparison = Nullable.Compare( LastModified, other.LastModified );
        if ( lastModifiedComparison != 0 ) { return lastModifiedComparison; }

        return DateCreated.CompareTo( other.DateCreated );
    }



    [Serializable]
    public class RecordCollection( int capacity = Buffers.DEFAULT_CAPACITY ) : RecordCollection<TRecord>( capacity )
    {
        public RecordCollection( params ReadOnlySpan<TRecord> values ) : this() => Add( values );
        public RecordCollection( IEnumerable<TRecord>         values ) : this() => Add( values );
    }
}



[Serializable]
public abstract record OwnedTableRecord<TRecord>( ref readonly RecordID<UserRecord>? CreatedBy, ref readonly RecordID<TRecord> ID, ref readonly DateTimeOffset DateCreated, ref readonly DateTimeOffset? LastModified ) : TableRecord<TRecord>( in ID, in DateCreated, in LastModified ), IOwnedTableRecord
    where TRecord : OwnedTableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public RecordID<UserRecord>? CreatedBy { get; set; } = CreatedBy;


    public static DynamicParameters GetDynamicParameters( UserRecord user )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(CreatedBy), user.ID.value );
        return parameters;
    }
    protected static DynamicParameters GetDynamicParameters( OwnedTableRecord<TRecord> record )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(CreatedBy), record.CreatedBy );
        return parameters;
    }

    public override DynamicParameters ToDynamicParameters()
    {
        var parameters = base.ToDynamicParameters();
        parameters.Add( nameof(CreatedBy), CreatedBy );
        return parameters;
    }


    public async ValueTask<UserRecord?> GetUser( DbConnection           connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, true,      GetDynamicParameters( this ), token );
    public async ValueTask<UserRecord?> GetUserWhoCreated( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, CreatedBy, token );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TRecord WithOwner( UserRecord  user )   => (TRecord)(this with { CreatedBy = user.ID });
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool    Owns( UserRecord       record ) => CreatedBy == record.ID;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool    DoesNotOwn( UserRecord record ) => CreatedBy != record.ID;


    public override int CompareTo( TRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int userIDComparison = Nullable.Compare( CreatedBy, other.CreatedBy );
        if ( userIDComparison != 0 ) { return userIDComparison; }

        return base.CompareTo( other );
    }
}
