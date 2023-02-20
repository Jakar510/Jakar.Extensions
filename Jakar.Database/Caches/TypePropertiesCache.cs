// Jakar.Extensions :: Jakar.Database
// 08/17/2022  8:48 PM

namespace Jakar.Database.Caches;


[SuppressMessage( "ReSharper", "ReturnTypeCanBeEnumerable.Global" )]
public sealed class TypePropertiesCache : ConcurrentDictionary<Type, TypePropertiesCache.Properties>
{
    public static TypePropertiesCache Current { get; } = new();


    static TypePropertiesCache() => Current.Register( Assembly.GetCallingAssembly() );
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


    public void Register( Assembly assembly ) => Register( assembly.DefinedTypes.Where( x => x.GetCustomAttribute<SerializableAttribute>( true ) is not null ) );
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
        private readonly IReadOnlyDictionary<DbInstance, Descriptors> _dictionary;


        public Descriptor this[ string          columnName, IConnectableDb table ] => this[table][columnName];
        public Descriptor this[ string          columnName, DbInstance     value ] => this[value][columnName];
        public Descriptors this[ DbInstance     value ] => _dictionary[value];
        public Descriptors this[ IConnectableDb value ] => this[value.Instance];
        public IEnumerable<DbInstance>  Keys   => _dictionary.Keys;
        public IEnumerable<Descriptors> Values => _dictionary.Values;
        public int                      Count  => _dictionary.Count;


        public Properties( Type type )
        {
            PropertyInfo[] properties = type.GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty );

            _dictionary = new Dictionary<DbInstance, Descriptors>
                          {
                              [DbInstance.Postgres] = properties.ToDictionary( x => x.Name, Descriptor.Postgres ),
                              [DbInstance.MsSql]    = properties.ToDictionary( x => x.Name, Descriptor.MsSql )
                          };
        }


        public IEnumerable<Descriptor> NotKeys( DbInstance value ) => this[value]
                                                                     .Values.Where( x => !x.IsKey );
        public IEnumerable<Descriptor> NotKeys( IConnectableDb value ) => this[value]
                                                                         .Values.Where( x => !x.IsKey );


        public bool TryGetValue( IConnectableDb key, [NotNullWhen( true )] out Descriptors? value ) => TryGetValue( key.Instance, out value );
        public bool TryGetValue( DbInstance     key, [NotNullWhen( true )] out Descriptors? value ) => _dictionary.TryGetValue( key, out value );


        public bool ContainsKey( DbInstance     value ) => _dictionary.ContainsKey( value );
        public bool ContainsKey( IConnectableDb value ) => ContainsKey( value.Instance );


        public IEnumerator<KeyValuePair<DbInstance, Descriptors>> GetEnumerator() => _dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();



        public sealed class Descriptors : IReadOnlyDictionary<string, Descriptor>
        {
            private readonly IReadOnlyDictionary<string, Descriptor> _dictionary;
            public Descriptor this[ string key ] => _dictionary[key];
            public IEnumerable<Descriptor> Values => _dictionary.Values;
            public IEnumerable<string>     Keys   => _dictionary.Keys;
            public int                     Count  => _dictionary.Count;
            public Descriptors( Dictionary<string, Descriptor>                          dictionary ) => _dictionary = dictionary;
            public static implicit operator Descriptors( Dictionary<string, Descriptor> dictionary ) => new(dictionary);


            public IEnumerator<KeyValuePair<string, Descriptor>> GetEnumerator() => _dictionary.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();


            public bool ContainsKey( string key ) => _dictionary.ContainsKey( key );
            public bool TryGetValue( string key, [NotNullWhen( true )] out Descriptor? value ) => _dictionary.TryGetValue( key, out value );
        }
    }
}
