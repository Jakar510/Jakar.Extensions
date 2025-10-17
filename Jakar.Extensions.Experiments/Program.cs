﻿Console.WriteLine(DateTimeOffset.UtcNow.ToString());

// Console.WriteLine(SpanDuration.ToString(TimeSpan.FromHours(1.1243123), "End. Duration: "));


// TestJson.Print();


// BenchmarkRunner.Run<JsonNet_SystemTextJson_Benchmarks>();


// BenchmarkRunner.Run<JsonNet_SystemTextJson_Benchmarks>(new DebugInProcessConfig());


// JsonNet_SystemTextJson_Benchmarks.Test();
{
    using Permissions<TestRight> permissions = Permissions<TestRight>.Default;
    permissions.Grant(TestRight.Analytics_ClearTraces);
    permissions.Grant(TestRight.Analytics_CreateAlert);
    permissions.Grant(TestRight.User_AssignGroup);
    permissions.Grant(TestRight.User_AssignRole);
    permissions.Grant(TestRight.User_ViewSessions);
    permissions.Grant(TestRight.User_View);
    permissions.Revoke(TestRight.System_ViewStatus);
    permissions.Grant(TestRight.System_ViewStatus);
    permissions.Revoke(TestRight.Network_FlushDNSCache);
    permissions.Grant(TestRight.Network_FlushDNSCache);


    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine(permissions.ToString());
    Console.WriteLine();
    Console.WriteLine();
}

{
    Permissions.Count = Permissions<TestRight>.Count;
    using Permissions permissions = Permissions.Default;
    permissions.Grant(TestRight.Analytics_ClearTraces);
    permissions.Grant(TestRight.Analytics_CreateAlert);
    permissions.Grant(TestRight.User_AssignGroup);
    permissions.Grant(TestRight.User_AssignRole);
    permissions.Grant(TestRight.User_ViewSessions);
    permissions.Grant(TestRight.User_View);
    permissions.Revoke(TestRight.System_ViewStatus);
    permissions.Grant(TestRight.System_ViewStatus);
    permissions.Revoke(TestRight.Network_FlushDNSCache);
    permissions.Grant(TestRight.Network_FlushDNSCache);


    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine(permissions.ToString());
    Console.WriteLine();
    Console.WriteLine();
}
