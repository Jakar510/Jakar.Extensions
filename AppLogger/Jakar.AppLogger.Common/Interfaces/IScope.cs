namespace Jakar.AppLogger.Common;


/// <summary> Sets the <see cref="IScopeID.ScopeID"/> until it's disposed and set to <see langword="null"/> </summary>
public interface IScope : IScopeID, IDisposable { }



public interface IScope<TState> : IScope
{
    public TState State { get; }
}
