// Jakar.Extensions :: Jakar.Database.Resx
// 10/07/2022  9:53 PM

namespace Jakar.Database.Resx;


public interface IResxCollection : IReadOnlyCollection<ResxRowRecord>
{
    public ResxSet GetSet( in SupportedLanguage language );


    public ValueTask          Init( Activity?        activity,   IResxProvider  provider,    CancellationToken      token                                                           = default );
    public ValueTask          Init( Activity?        activity,   IConnectableDb db,          DbTable<ResxRowRecord> table,    CancellationToken      token                          = default );
    public ValueTask          Init( DbConnection     connection, DbTransaction  transaction, Activity?              activity, DbTable<ResxRowRecord> table, CancellationToken token = default );
    public ValueTask<ResxSet> GetSetAsync( Activity? activity,   IResxProvider  provider,    SupportedLanguage      language, CancellationToken      token = default );
}
