// Jakar.Extensions :: Jakar.Database
// 05/31/2024  23:05

namespace Jakar.Database;


public readonly record struct DbConnContext( DbConnection Connection, DbTransaction? Transaction, Activity? Activity );
