// Jakar.Extensions :: Jakar.Database
// 10/16/2022  5:41 PM

namespace Jakar.Database.Implementations;


public class MsSqlDbTable<TRecord> : DbTableBase<TRecord> where TRecord : TableRecord<TRecord>
{
    public MsSqlDbTable( IConnectableDb database ) : base( database ) { }
}
