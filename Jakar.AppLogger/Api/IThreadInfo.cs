// Jakar.Extensions :: Jakar.AppLogger
// 07/06/2022  4:01 PM

namespace Jakar.AppLogger;


public interface IThreadInfo
{
    public int     ProcessID         { get; }
    public string? ProcessName       { get; }
    public int?    ParentProcessID   { get; }
    public string? ParentProcessName { get; }
    public long?   ErrorThreadID     { get; }
    public string? ErrorThreadName   { get; }
    public string? Architecture      { get; }
}
