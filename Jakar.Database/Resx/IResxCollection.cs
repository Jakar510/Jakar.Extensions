// Jakar.Extensions :: Jakar.Database.Resx
// 10/07/2022  9:53 PM

namespace Jakar.Database;


public interface IResxCollection : IReadOnlyCollection<ResxRowTable>
{
    public ResxSet GetSet( in SupportedLanguage          language );
    public ValueTask<ResxSet> GetSetAsync( IResxProvider provider, SupportedLanguage language, CancellationToken token = default );


    public ValueTask Init( IResxProvider  provider,   CancellationToken token                                                         = default );
    public ValueTask Init( IConnectableDb db,         DbTable<ResxRowTable>  table,       CancellationToken token                          = default );
    public ValueTask Init( DbConnection   connection, DbTransaction     transaction, DbTable<ResxRowTable>  table, CancellationToken token = default );
}
