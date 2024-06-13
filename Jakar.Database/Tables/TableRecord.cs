// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


public interface IRecordPair : IUniqueID<Guid>
{
    public DateTimeOffset DateCreated { get; }
}



public interface IDbReaderMapping<out TRecord>
    where TRecord : IDbReaderMapping<TRecord>
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



public interface ITableRecord<TRecord> : ITableRecord
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    Guid IUniqueID<Guid>.          ID => ID.Value;
    public new RecordID<TRecord>   ID { get; }
    public     RecordPair<TRecord> ToPair();
    public     TRecord             NewID( in RecordID<TRecord> id );
    public     UInt128             GetHash();
}



public interface IOwnedTableRecord
{
    public RecordID<UserRecord>? CreatedBy { get; }
}



[Serializable]
public abstract record TableRecord<TRecord>( RecordID<TRecord> ID, DateTimeOffset DateCreated, DateTimeOffset? LastModified ) : BaseRecord, ITableRecord<TRecord>, IComparable<TRecord>
    where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    private   RecordID<TRecord> _id           = ID;
    protected DateTimeOffset?   _lastModified = LastModified;


    public       DateTimeOffset?   LastModified { get => _lastModified; init => _lastModified = value; }
    [Key] public RecordID<TRecord> ID           { get => _id;           init => _id = value; }


    protected TableRecord( RecordID<TRecord> id ) : this( id, DateTimeOffset.UtcNow, null ) { }


    [Pure] public RecordPair<TRecord> ToPair() => new(ID, DateCreated);


    [Conditional( "DEBUG" )]
    public void Validate()
    {
        PropertyInfo[]    properties = typeof(TRecord).GetProperties( BindingFlags.Instance | BindingFlags.Public );
        DynamicParameters parameters = ToDynamicParameters();
        int               length     = parameters.ParameterNames.Count();
        if ( length == properties.Length ) { return; }

        HashSet<string> missing = new HashSet<string>( properties.Select( x => x.Name ) );
        missing.ExceptWith( parameters.ParameterNames );

        string message = $"""
                          {typeof(TRecord).Name}: {nameof(ToDynamicParameters)}.Length ({length}) != {nameof(properties)}.Length ({properties.Length})
                          {missing.ToPrettyJson()}
                          """;

        throw new InvalidOperationException( message );
    }


    public static DynamicParameters GetDynamicParameters( TRecord record ) => GetDynamicParameters( record.ID );
    public static DynamicParameters GetDynamicParameters( in RecordID<TRecord> id )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(ID), id.Value );
        return parameters;
    }

    public virtual DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(ID),           ID.Value );
        parameters.Add( nameof(DateCreated),  DateCreated );
        parameters.Add( nameof(LastModified), LastModified );
        return parameters;
    }


    [Pure]
    protected static T TryGet<T>( DbDataReader reader, in string key )
    {
        int index = reader.GetOrdinal( key );
        return (T)reader.GetValue( index );
    }


    [Pure]
    public TRecord Modified()
    {
        _lastModified = DateTimeOffset.UtcNow;
        return (TRecord)this;
    }

    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] protected internal TRecord NewID( Guid id ) => NewID( new RecordID<TRecord>( id ) );

    [Pure]
    public TRecord NewID( in RecordID<TRecord> id )
    {
        _id = id;
        return (TRecord)this;
    }


    [Pure]
    public UInt128 GetHash()
    {
        string json = this.ToJson();
        return Spans.Hash128( json );
    }


    public virtual int CompareTo( TRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int lastModifiedComparison = Nullable.Compare( LastModified, other.LastModified );
        if ( lastModifiedComparison != 0 ) { return lastModifiedComparison; }

        return DateCreated.CompareTo( other.DateCreated );
    }
}



[Serializable]
public abstract record OwnedTableRecord<TRecord>( RecordID<UserRecord>? CreatedBy, RecordID<TRecord> ID, DateTimeOffset DateCreated, DateTimeOffset? LastModified ) : TableRecord<TRecord>( ID, DateCreated, LastModified ), IOwnedTableRecord
    where TRecord : OwnedTableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public RecordID<UserRecord>? CreatedBy { get; set; } = CreatedBy;


    protected OwnedTableRecord( UserRecord?       owner ) : this( RecordID<TRecord>.New(), owner ) { }
    protected OwnedTableRecord( RecordID<TRecord> id, UserRecord? owner = default ) : this( owner?.ID, id, DateTimeOffset.UtcNow, null ) { }


    public static DynamicParameters GetDynamicParameters( UserRecord user )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(CreatedBy), user.ID.Value );
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
        DynamicParameters parameters = base.ToDynamicParameters();
        parameters.Add( nameof(CreatedBy), CreatedBy );
        return parameters;
    }


    public async ValueTask<UserRecord?> GetUser( DbConnection           connection, DbTransaction? transaction, Activity? activity, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, activity, true,        GetDynamicParameters( this ), token );
    public async ValueTask<UserRecord?> GetUserWhoCreated( DbConnection connection, DbTransaction? transaction, Activity? activity, Database db, CancellationToken token ) => await db.Users.Get( connection, transaction, activity, CreatedBy, token );


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
