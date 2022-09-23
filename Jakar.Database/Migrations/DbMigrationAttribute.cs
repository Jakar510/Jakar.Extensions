// Jakar.Extensions :: Jakar.Database
// 09/07/2022  5:57 PM

namespace Jakar.Database.Migrations;


[AttributeUsage(AttributeTargets.Class)] public sealed class DbMigrationAttribute : Attribute { }



[AttributeUsage(AttributeTargets.Property)] public sealed class DbIgnoreAttribute : Attribute { }



[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)] public sealed class DbTypeAttribute : Attribute { }
