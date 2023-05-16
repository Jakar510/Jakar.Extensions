// Jakar.Extensions :: Jakar.Database
// 08/17/2022  8:48 PM

namespace Jakar.Database.Caches;


[SuppressMessage( "ReSharper", "ReturnTypeCanBeEnumerable.Global" )]
public sealed class TypePropertiesCache : ConcurrentDictionary<Type, TypePropertiesCache.Properties>
{
    public static TypePropertiesCache Current { get; } = new();


    static TypePropertiesCache()
    {
        var assembly = Assembly.GetEntryAssembly();
        if ( assembly is not null ) { Current.Register( assembly ); }

        Current.Register( Assembly.GetCallingAssembly() );
    }
    internal TypePropertiesCache() { }


    [Pure]
    public Descriptor Get( Type type, DbInstance instance, string columnName ) => Get( type, instance )
       .First( x => x.ColumnName == columnName );

    [Pure]
    public IEnumerable<Descriptor> Get( Type type, DbInstance instance )
    {
        if ( !ContainsKey( type ) ) { Register( type ); }

        return this[type]
           .NotKeys( instance );
    }

    [Pure]
    public IEnumerable<Descriptor> Get( Type type, IConnectableDb value )
    {
        if ( !ContainsKey( type ) ) { Register( type ); }

        return this[type]
           .NotKeys( value );
    }


    public void Register( Assembly assembly ) => Register( assembly.DefinedTypes.Where( x => x.GetCustomAttribute<TableAttribute>( true ) is not null ) );
    public void Register( IEnumerable<Type> types )
    {
        foreach ( Type type in types ) { Register( type ); }
    }
    public void Register( params Type[] types )
    {
        foreach ( Type type in types ) { Register( type ); }
    }
    public void Register( Type type )
    {
        if ( ContainsKey( type ) ) { return; }


        this[type] = new Properties( type );
    }



    [SuppressMessage( "ReSharper", "ReturnTypeCanBeEnumerable.Global" )]
    [SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" )]
    public sealed class Properties : IReadOnlyDictionary<DbInstance, Properties.Descriptors>
    {
        internal const BindingFlags ATTRIBUTES = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty;


        private readonly IReadOnlyDictionary<DbInstance, Descriptors> _dictionary;
        public int Count
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _dictionary.Count;
        }


        public Descriptors this[ IConnectableDb value ]
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _dictionary[value.Instance];
        }
        public Descriptors this[ DbInstance value ]
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _dictionary[value];
        }
        public IEnumerable<DbInstance> Keys
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _dictionary.Keys;
        }
        public IEnumerable<Descriptors> Values
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _dictionary.Values;
        }


        public Properties( Type type )
        {
            PropertyInfo[] properties = type.GetProperties( ATTRIBUTES )
                                            .Where( x => !x.HasAttribute<DataBaseIgnoreAttribute>() )
                                            .ToArray();

            Debug.Assert( properties.Length > 0 );
            Debug.Assert( properties.Any( Descriptor.IsDbKey ) );

            _dictionary = new Dictionary<DbInstance, Descriptors>
                          {
                              [DbInstance.Postgres] = properties.ToDictionary( x => x.Name, Descriptor.Postgres ),
                              [DbInstance.MsSql]    = properties.ToDictionary( x => x.Name, Descriptor.MsSql ),
                          };
        }


        public bool ContainsKey( DbInstance     value ) => _dictionary.ContainsKey( value );
        public bool ContainsKey( IConnectableDb value ) => ContainsKey( value.Instance );


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public IEnumerable<Descriptor> GetValues( DbInstance value ) => _dictionary[value]
           .Values;
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public IEnumerable<Descriptor> GetValues( IConnectableDb value ) => _dictionary[value.Instance]
           .Values;


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public IEnumerable<Descriptor> NotKeys( DbInstance value ) => _dictionary[value]
                                                                     .Values.Where( x => !x.IsKey );
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public IEnumerable<Descriptor> NotKeys( IConnectableDb value ) => _dictionary[value.Instance]
                                                                         .Values.Where( x => !x.IsKey );


        public bool TryGetValue( IConnectableDb key, [NotNullWhen( true )] out Descriptors? value ) => TryGetValue( key.Instance, out value );
        public bool TryGetValue( DbInstance     key, [NotNullWhen( true )] out Descriptors? value ) => _dictionary.TryGetValue( key, out value );


        public IEnumerator<KeyValuePair<DbInstance, Descriptors>> GetEnumerator() => _dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();


        [MethodImpl( MethodImplOptions.AggressiveInlining )] public Descriptor Get( DbInstance     table, string columnName ) => _dictionary[table][columnName];
        [MethodImpl( MethodImplOptions.AggressiveInlining )] public Descriptor Get( IConnectableDb table, string columnName ) => Get( table.Instance, columnName );



        public sealed class Descriptors : IReadOnlyDictionary<string, Descriptor>
        {
            private readonly IReadOnlyDictionary<string, Descriptor> _dictionary;
            public           int                                     Count => _dictionary.Count;


            public Descriptor this[ string key ] => _dictionary[key];
            public IEnumerable<string>     Keys   => _dictionary.Keys;
            public IEnumerable<Descriptor> Values => _dictionary.Values;


            public Descriptors( Dictionary<string, Descriptor>                          dictionary ) => _dictionary = dictionary;
            public static implicit operator Descriptors( Dictionary<string, Descriptor> dictionary ) => new(dictionary);


            public IEnumerator<KeyValuePair<string, Descriptor>> GetEnumerator() => _dictionary.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();


            public bool ContainsKey( string key ) => _dictionary.ContainsKey( key );
            public bool TryGetValue( string key, [NotNullWhen( true )] out Descriptor? value ) => _dictionary.TryGetValue( key, out value );
        }
    }
}
