namespace Jakar.Extensions;


/// <summary> Options that customize the behavior of <see cref="Command{TValue}"/> (mirrors <c>CommunityToolkit.Mvvm.Input.AsyncRelayCommandOptions</c>). </summary>
[Flags]
public enum CommandOptions
{
    /// <summary> Only one execution at a time: <see cref="Command{TValue}.CanExecute(object?)"/> returns <see langword="false"/> while running, and exceptions from <see cref="Command{TValue}.Execute(object?)"/> are rethrown on the calling synchronization context. </summary>
    None = 0,

    /// <summary> New executions may start while a previous one is still running; <see cref="Command{TValue}.CanExecute(object?)"/> no longer checks <see cref="Command{TValue}.IsRunning"/>. </summary>
    AllowConcurrentExecutions = 1 << 0,

    /// <summary> Exceptions from <see cref="Command{TValue}.Execute(object?)"/> are not rethrown; they stay on <see cref="Command{TValue}.ExecutionTask"/> and flow to <see cref="TaskScheduler.UnobservedTaskException"/> if never observed. </summary>
    FlowExceptionsToTaskScheduler = 1 << 1
}
