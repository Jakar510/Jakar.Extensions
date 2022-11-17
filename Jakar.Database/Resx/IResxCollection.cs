// Jakar.Extensions :: Jakar.Database.Resx
// 10/07/2022  9:53 PM

namespace Jakar.Database.Resx;


public interface IResxCollection : IReadOnlyCollection<ResxRowTable>
{
    public ResxSet GetSet( in SupportedLanguage language );


    public ValueTask Init( IResxProvider                 provider,   CancellationToken         token                                                                 = default );
    public ValueTask Init( IConnectableDb                db,         DbTableBase<ResxRowTable> table,       CancellationToken         token                          = default );
    public ValueTask Init( DbConnection                  connection, DbTransaction             transaction, DbTableBase<ResxRowTable> table, CancellationToken token = default );
    public ValueTask<ResxSet> GetSetAsync( IResxProvider provider,   SupportedLanguage         language,    CancellationToken         token = default );
}
