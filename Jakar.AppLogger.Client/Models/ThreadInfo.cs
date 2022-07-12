// Jakar.Extensions :: Jakar.AppLogger
// 07/06/2022  3:14 PM

using System.Diagnostics;



namespace Jakar.AppLogger.Client;


public class ThreadInfo : IThreadInfo
{
    public int     ProcessID         { get; init; }
    public string? ProcessName       { get; init; }
    public int?    ParentProcessID   { get; init; }
    public string? ParentProcessName { get; init; }
    public long?   ErrorThreadID     { get; init; }
    public string? ErrorThreadName   { get; init; }
    public string? Architecture      { get; init; }


    public ThreadInfo() { }
    public ThreadInfo( int processId, string processName, int? parentProcessId = default, string? parentProcessName = default, long? errorThreadId = default, string? errorThreadName = default, string? architecture = default )
    {
        ProcessID         = processId;
        ProcessName       = processName;
        ParentProcessID   = parentProcessId;
        ParentProcessName = parentProcessName;
        ErrorThreadID     = errorThreadId;
        ErrorThreadName   = errorThreadName;
        Architecture      = architecture;
    }
    public static ThreadInfo Create( Exception? e )
    {
        var process = Process.GetCurrentProcess();

        return new ThreadInfo()
               {
                   ProcessID   = process.Id,
                   ProcessName = process.ProcessName
               };
    }
}
