﻿// Jakar.Extensions :: Jakar.Database
// 10/15/2023  1:13 PM

namespace Jakar.Database;


public static class SQL
{
    public const string AND            = " AND ";
    public const string COUNT          = "count";
    public const string CREATED_BY     = nameof(IOwnedTableRecord.CreatedBy);
    public const string DATE_CREATED   = nameof(IRecordPair.DateCreated);
    public const string GUID_FORMAT    = "D";
    public const string ID             = nameof(IRecordPair.ID);
    public const string IDS            = "ids";
    public const string LAST_MODIFIED  = nameof(ITableRecord.LastModified);
    public const string LIST_SEPARATOR = ", ";
    public const string OR             = " OR ";
    public const string OWNER_USER_ID  = nameof(IOwnedTableRecord.OwnerUserID);
    public const char   QUOTE          = '"';


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
        var hash = new HashCode();
        foreach ( T value in values ) { hash.Add( value ); }

        return hash.ToHashCode();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetCreatedBy( this DbInstance instance ) =>
        instance switch
        {
            DbInstance.Postgres => $"{QUOTE}{CREATED_BY}{QUOTE}",
            DbInstance.MsSql    => CREATED_BY,
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetID_ColumnName( this DbInstance instance ) =>
        instance switch
        {
            DbInstance.Postgres => $"{QUOTE}{ID}{QUOTE}",
            DbInstance.MsSql    => ID,
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetLastModified( this DbInstance instance ) =>
        instance switch
        {
            DbInstance.Postgres => $"{QUOTE}{LAST_MODIFIED}{QUOTE}",
            DbInstance.MsSql    => LAST_MODIFIED,
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetOwnerUserID( this DbInstance instance ) =>
        instance switch
        {
            DbInstance.Postgres => $"{QUOTE}{OWNER_USER_ID}{QUOTE}",
            DbInstance.MsSql    => OWNER_USER_ID,
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetTableName<TRecord>( this DbInstance instance )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord> =>
        instance switch
        {
            DbInstance.Postgres => $"{QUOTE}{TRecord.TableName}{QUOTE}",
            DbInstance.MsSql    => TRecord.TableName,
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetRandomMethod( this DbInstance instance ) =>
        instance switch
        {
            DbInstance.MsSql    => "NEWID()",
            DbInstance.Postgres => "RANDOM()",
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetDateCreated( this DbInstance instance ) =>
        instance switch
        {
            DbInstance.Postgres => $"{QUOTE}{DATE_CREATED}{QUOTE}",
            DbInstance.MsSql    => DATE_CREATED,
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };


    public static FrozenDictionary<DbInstance, FrozenDictionary<string, Descriptor>> CreateDescriptorMapping( this Type type )
    {
        const BindingFlags ATTRIBUTES = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty;

        PropertyInfo[] properties = type.GetProperties( ATTRIBUTES ).Where( static x => !x.HasAttribute<DataBaseIgnoreAttribute>() ).ToArray();

        Debug.Assert( properties.Length > 0 );
        Debug.Assert( properties.Any( Descriptor.IsDbKey ) );

        return new Dictionary<DbInstance, FrozenDictionary<string, Descriptor>>
               {
                   [DbInstance.Postgres] = properties.ToFrozenDictionary( static x => x.Name, Descriptor.Postgres ),
                   [DbInstance.MsSql]    = properties.ToFrozenDictionary( static x => x.Name, Descriptor.MsSql )
               }.ToFrozenDictionary();
    }


    public static FrozenDictionary<DbInstance, FrozenDictionary<string, Descriptor>> CreateDescriptorMapping<TRecord>()
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        const BindingFlags ATTRIBUTES = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty;

        PropertyInfo[] properties = typeof(TRecord).GetProperties( ATTRIBUTES ).Where( static x => !x.HasAttribute<DataBaseIgnoreAttribute>() ).ToArray();

        Debug.Assert( properties.Length > 0 );
        Debug.Assert( properties.Any( Descriptor.IsDbKey ) );

        return new Dictionary<DbInstance, FrozenDictionary<string, Descriptor>>
               {
                   [DbInstance.Postgres] = properties.ToFrozenDictionary( static x => x.Name, Descriptor.Postgres ),
                   [DbInstance.MsSql]    = properties.ToFrozenDictionary( static x => x.Name, Descriptor.MsSql )
               }.ToFrozenDictionary( EqualityComparer<DbInstance>.Default );
    }
}
