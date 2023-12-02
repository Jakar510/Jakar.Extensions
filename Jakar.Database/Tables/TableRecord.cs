// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


public interface IRecordPair : IUniqueID<Guid> // where TID : IComparable<TID>, IEquatable<TID>
{
    public DateTimeOffset DateCreated { get; }
}



public interface IDbReaderMapping<out TRecord>
    where TRecord : IDbReaderMapping<TRecord>
{
    public abstract static string                    TableName { get; }
    public abstract static TRecord                   Create( DbDataReader      reader );
    public abstract static IAsyncEnumerable<TRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default );


    public DynamicParameters ToDynamicParameters();
}



public interface ITableRecord : IRecordPair
{
    public DateTimeOffset? LastModified { get; }
}



public interface ITableRecord<TRecord> : ITableRecord
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public new RecordID<TRecord> ID { get; }
    Guid IUniqueID<Guid>.        ID => ID.Value;
    public TRecord               NewID( RecordID<TRecord> id );
}



public interface IOwnedTableRecord
{
    public RecordID<UserRecord>? CreatedBy   { get; }
    public Guid?                 OwnerUserID { get; }
}



[ Serializable ]
public abstract record TableRecord<TRecord>( [ property: Key ] RecordID<TRecord> ID, DateTimeOffset DateCreated, DateTimeOffset? LastModified ) : BaseRecord, ITableRecord<TRecord>, IComparable<TRecord>
    where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    protected TableRecord( RecordID<TRecord> id ) : this( id, DateTimeOffset.UtcNow, null ) { }


    [ Conditional( "DEBUG" ) ]
    public void Validate()
    {
        PropertyInfo[]    properties = typeof(TRecord).GetProperties( BindingFlags.Instance | BindingFlags.Public );
        DynamicParameters parameters = ToDynamicParameters();
        int               length     = parameters.ParameterNames.Count();
        if ( length == properties.Length ) { return; }

        var missing = new HashSet<string>( properties.Select( x => x.Name ) );
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
        var parameters = new DynamicParameters();
        parameters.Add( nameof(ID), id.Value );
        return parameters;
    }

    public virtual DynamicParameters ToDynamicParameters()
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(ID),           ID.Value );
        parameters.Add( nameof(DateCreated),  DateCreated );
        parameters.Add( nameof(LastModified), LastModified );
        return parameters;
    }

    protected static T TryGet<T>( DbDataReader reader, in string key )
    {
        int index = reader.GetOrdinal( key );
        return (T)reader.GetValue( index );
    }


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] protected internal TRecord NewID( Guid id ) => NewID( new RecordID<TRecord>( id ) );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public TRecord NewID( RecordID<TRecord> id ) => (TRecord)(this with
                                                              {
                                                                  ID = id
                                                              });


    public virtual int CompareTo( TRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int lastModifiedComparison = Nullable.Compare( LastModified, other.LastModified );
        if ( lastModifiedComparison != 0 ) { return lastModifiedComparison; }

        return DateCreated.CompareTo( other.DateCreated );
    }
}



[ Serializable ]
public abstract record OwnedTableRecord<TRecord>
    ( RecordID<TRecord> ID, RecordID<UserRecord>? CreatedBy, Guid? OwnerUserID, DateTimeOffset DateCreated, DateTimeOffset? LastModified ) : TableRecord<TRecord>( ID, DateCreated, LastModified ), IOwnedTableRecord
    where TRecord : OwnedTableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    protected OwnedTableRecord( UserRecord?       owner ) : this( RecordID<TRecord>.New(), owner ) { }
    protected OwnedTableRecord( RecordID<TRecord> id, UserRecord? owner = default ) : this( id, owner?.ID, owner?.UserID, DateTimeOffset.UtcNow, null ) { }


    public static DynamicParameters GetDynamicParameters( UserRecord user )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(CreatedBy),   user.ID.Value );
        parameters.Add( nameof(OwnerUserID), user.UserID );
        return parameters;
    }
    protected static DynamicParameters GetDynamicParameters( OwnedTableRecord<TRecord> record )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(OwnerUserID), record.OwnerUserID );
        parameters.Add( nameof(CreatedBy),   record.CreatedBy?.Value );
        return parameters;
    }

    public override DynamicParameters ToDynamicParameters()
    {
        var parameters = base.ToDynamicParameters();
        parameters.Add( nameof(CreatedBy),   CreatedBy?.Value );
        parameters.Add( nameof(OwnerUserID), OwnerUserID );
        return parameters;
    }

    public async ValueTask<UserRecord?> GetUser( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) =>
        await db.Users.Get( connection, transaction, true, GetDynamicParameters( this ), token );
    public async ValueTask<UserRecord?> GetUserWhoCreated( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) =>
        await db.Users.Get( connection, transaction, CreatedBy, token );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public TRecord WithOwner( UserRecord user ) => (TRecord)(this with
                                                             {
                                                                 OwnerUserID = user.UserID
                                                             });


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public bool Owns( UserRecord       record ) => CreatedBy == record.ID;
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public bool DoesNotOwn( UserRecord record ) => CreatedBy != record.ID;


    public override int CompareTo( TRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int createdByComparison = Nullable.Compare( CreatedBy, other.CreatedBy );
        if ( createdByComparison != 0 ) { return createdByComparison; }

        int userIDComparison = Nullable.Compare( OwnerUserID, other.OwnerUserID );
        if ( userIDComparison != 0 ) { return userIDComparison; }

        return base.CompareTo( other );
    }
}
