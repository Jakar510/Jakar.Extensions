// Jakar.Extensions :: Jakar.Database
// 02/20/2023  10:37 AM

namespace Jakar.Database;


#pragma warning disable CS8424
[SuppressMessage( "ReSharper", "ReturnTypeCanBeEnumerable.Global" )]
public sealed class TypePropertiesCache<TRecord> where TRecord : TableRecord<TRecord>
{
    private readonly IConnectableDb                                                           _table;
    private readonly IReadOnlyDictionary<DbInstance, IReadOnlyDictionary<string, Descriptor>> _dictionary = GetDescriptors();


    public Descriptor this[ string columnName ] => Descriptors[columnName];
    public IEnumerable<Descriptor>                 NotKeys     => Descriptors.Values.Where( x => !x.IsKey );
    public IReadOnlyDictionary<string, Descriptor> Descriptors => _dictionary[_table.Instance];


    public TypePropertiesCache( IConnectableDb table ) => _table = table;


    private static IReadOnlyDictionary<DbInstance, IReadOnlyDictionary<string, Descriptor>> GetDescriptors()
    {
        PropertyInfo[] properties = typeof(TRecord).GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty );

        return new Dictionary<DbInstance, IReadOnlyDictionary<string, Descriptor>>
               {
                   [DbInstance.Postgres] = properties.ToDictionary<PropertyInfo, string, Descriptor>( x => x.Name, x => new PostgresDescriptor( x ) ),
                   [DbInstance.MsSql]    = properties.ToDictionary<PropertyInfo, string, Descriptor>( x => x.Name, x => new MsSqlDescriptor( x ) )
               };
    }
}
