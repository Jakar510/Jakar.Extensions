// Jakar.Extensions :: Jakar.Database
// 10/15/2023  1:13 PM

namespace Jakar.Database;


public static class SQL // TODO: move to Jakar.Extensions.Sizes
{
    public const string AND                   = " AND ";
    public const int    ANSI_CAPACITY         = 8000;
    public const int    ANSI_TEXT_CAPACITY    = 2_147_483_647;
    public const int    BINARY_CAPACITY       = ANSI_TEXT_CAPACITY;
    public const string COUNT                 = "count";
    public const string DATE_CREATED          = nameof(IRecordPair.DateCreated);
    public const int    DECIMAL_MAX_PRECISION = 38;
    public const int    DECIMAL_MAX_SCALE     = 29;
    public const string GUID_FORMAT           = "D";
    public const string ID                    = nameof(IRecordPair.ID);
    public const string IDS                   = "ids";
    public const string LAST_MODIFIED         = nameof(ITableRecord.LastModified);
    public const string LIST_SEPARATOR        = ", ";
    public const string OR                    = " OR ";
    public const string OWNER_USER_ID         = nameof(IOwnedTableRecord.OwnerUserID);
    public const char   QUOTE                 = '"';
    public const int    UNICODE_CAPACITY      = 4000;
    public const int    UNICODE_TEXT_CAPACITY = 1_073_741_823;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetAndOr( this bool matchAll ) => matchAll
                                                               ? AND
                                                               : OR;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static int GetHash( this DynamicParameters parameters ) => parameters.ParameterNames.GetHash();

    /*
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static ulong GetHash( this IEnumerable<string> parameters )
    {
        using var sb = new ValueStringBuilder( 1000 );
        sb.Append( parameters );
        ReadOnlySpan<char> span = sb.Span;
        ulong              hash = OFFSET_BASIS;

        foreach ( char c in span )
        {
            hash ^= c;
            hash *= PRIME;
        }

        return hash;
    }

   [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
   public static ulong GetLongHash<T>( this IEnumerable<T> values )
   {
       ulong hash = OFFSET_BASIS;

       foreach ( var value in values )
       {
           int valueHash = value?.GetHashCode() ?? 0;

           hash ^= (ulong)valueHash;
           hash *= PRIME;
       }

   return hash;
   }
    */


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int GetHash<T>( this IEnumerable<T> values )
    {
        HashCode hash = new HashCode();
        foreach ( T value in values ) { hash.Add( value ); }

        return hash.ToHashCode();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetCreatedBy( this DbTypeInstance instance ) =>
        instance switch
        {
            DbTypeInstance.Postgres => $"{QUOTE}{OWNER_USER_ID}{QUOTE}",
            DbTypeInstance.MsSql    => OWNER_USER_ID,
            _                       => throw new OutOfRangeException( nameof(instance), instance )
        };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetID_ColumnName( this DbTypeInstance instance ) =>
        instance switch
        {
            DbTypeInstance.Postgres => $"{QUOTE}{ID}{QUOTE}",
            DbTypeInstance.MsSql    => ID,
            _                       => throw new OutOfRangeException( nameof(instance), instance )
        };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetLastModified( this DbTypeInstance instance ) =>
        instance switch
        {
            DbTypeInstance.Postgres => $"{QUOTE}{LAST_MODIFIED}{QUOTE}",
            DbTypeInstance.MsSql    => LAST_MODIFIED,
            _                       => throw new OutOfRangeException( nameof(instance), instance )
        };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetOwnerUserID( this DbTypeInstance instance ) =>
        instance switch
        {
            DbTypeInstance.Postgres => $"{QUOTE}{OWNER_USER_ID}{QUOTE}",
            DbTypeInstance.MsSql    => OWNER_USER_ID,
            _                       => throw new OutOfRangeException( nameof(instance), instance )
        };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetTableName<TRecord>( this DbTypeInstance instance )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord> =>
        instance switch
        {
            DbTypeInstance.Postgres => $"{QUOTE}{TRecord.TableName}{QUOTE}",
            DbTypeInstance.MsSql    => TRecord.TableName,
            _                       => throw new OutOfRangeException( nameof(instance), instance )
        };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetRandomMethod( this DbTypeInstance instance ) =>
        instance switch
        {
            DbTypeInstance.MsSql    => "NEWID()",
            DbTypeInstance.Postgres => "RANDOM()",
            _                       => throw new OutOfRangeException( nameof(instance), instance )
        };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetDateCreated( this DbTypeInstance instance ) =>
        instance switch
        {
            DbTypeInstance.Postgres => $"{QUOTE}{DATE_CREATED}{QUOTE}",
            DbTypeInstance.MsSql    => DATE_CREATED,
            _                       => throw new OutOfRangeException( nameof(instance), instance )
        };


    public static FrozenDictionary<DbTypeInstance, FrozenDictionary<string, Descriptor>> CreateDescriptorMapping( this Type type )
    {
        const BindingFlags ATTRIBUTES = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty;

        PropertyInfo[] properties = type.GetProperties( ATTRIBUTES ).Where( static x => !x.HasAttribute<DataBaseIgnoreAttribute>() ).ToArray();

        Debug.Assert( properties.Length > 0 );
        Debug.Assert( properties.Any( Descriptor.IsDbKey ) );

        return new Dictionary<DbTypeInstance, FrozenDictionary<string, Descriptor>>
               {
                   [DbTypeInstance.Postgres] = properties.ToFrozenDictionary( static x => x.Name, Descriptor.Postgres ),
                   [DbTypeInstance.MsSql]    = properties.ToFrozenDictionary( static x => x.Name, Descriptor.MsSql )
               }.ToFrozenDictionary();
    }


    public static FrozenDictionary<DbTypeInstance, FrozenDictionary<string, Descriptor>> CreateDescriptorMapping<TRecord>()
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        const BindingFlags ATTRIBUTES = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty;

        PropertyInfo[] properties = typeof(TRecord).GetProperties( ATTRIBUTES ).Where( static x => !x.HasAttribute<DataBaseIgnoreAttribute>() ).ToArray();

        Debug.Assert( properties.Length > 0 );
        Debug.Assert( properties.Any( Descriptor.IsDbKey ) );

        return new Dictionary<DbTypeInstance, FrozenDictionary<string, Descriptor>>
               {
                   [DbTypeInstance.Postgres] = properties.ToFrozenDictionary( static x => x.Name, Descriptor.Postgres ),
                   [DbTypeInstance.MsSql]    = properties.ToFrozenDictionary( static x => x.Name, Descriptor.MsSql )
               }.ToFrozenDictionary( EqualityComparer<DbTypeInstance>.Default );
    }
}
